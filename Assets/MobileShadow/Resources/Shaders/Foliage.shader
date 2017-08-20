Shader "MobileShadow/Foliage" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _AlphaR ("AlphaR", 2D) = "white" {}
        _Settings ("Settings", Vector) = (1.5,0.02,540,10)
    }
    SubShader {
        Tags {
            "Queue"="Transparent"
            "MobileShadow"="Foliage"
        }
        Pass {
			Tags { LightMode = Vertex } 
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "TerrainEngine.cginc"
            #pragma target 2.0
 			#pragma multi_compile __ FOG_LINEAR
            #pragma multi_compile __ SHADOWTINT

            uniform sampler2D _MainTex;
            uniform fixed4 _Settings;
            uniform sampler2D _AlphaR;
            uniform sampler2D _MobileShadowTexture;
			uniform float4x4 _MobileShadowMatrix;
			uniform fixed _MobileShadowOpacity;
            fixed4 _MobileShadowColor;

            struct VertexInput {
                half4 vertex : POSITION;
                half3 normal : NORMAL;
                half2 texcoord0 : TEXCOORD0;
                fixed4 vertexColor : COLOR;
            };

            struct VertexOutput {
                half4 pos : SV_POSITION;
                half2 uv : TEXCOORD0;
                fixed3 uvShadow : TEXCOORD1;
                fixed4 vertexColor : COLOR;
            };

            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv = v.texcoord0;
                
                half3 world = mul(unity_ObjectToWorld, v.vertex).rgb;
                half3 settings = (_Settings.b * world).rgb;
                fixed4 waves;
                waves = (_Settings.r * _Time.y) + settings.g + settings.b;
                fixed4 s, c;
                waves = frac (waves);
                FastSinCos (waves, s,c);
                fixed deltaT = (_Time.y + world.z*0.2f);
                v.vertex.x += v.vertexColor.r * _Settings.g * s + saturate(v.vertexColor.g) * 0.1f * sin(deltaT) * _Settings.a;
                v.vertex.y += v.vertexColor.r * _Settings.g * c;

                o.pos = UnityObjectToClipPos(v.vertex);

                o.uvShadow = mul(_MobileShadowMatrix, mul(UNITY_MATRIX_M, v.vertex));
                o.vertexColor = 0;
                fixed3 viewpos = mul (UNITY_MATRIX_MV, v.vertex).xyz;
				float3 viewN = normalize (mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal));
				for (int i = 0; i < 4; i++)
				{
					half3 toLight = unity_LightPosition[i].xyz - viewpos.xyz * unity_LightPosition[i].w;
					half lengthSq = dot(toLight, toLight);

                    lengthSq = max(lengthSq, 0.000001);
		            toLight *= rsqrt(lengthSq);

					half atten = 1.0 / (1.0 + lengthSq * unity_LightAtten[i].z );
 
					fixed vertexColor = max ( 0, (dot (viewN, toLight)) );
					o.vertexColor += unity_LightColor[i] * (vertexColor * atten);
				}
                o.uvShadow.z = min(0.5, o.vertexColor) * 2;
				o.vertexColor += unity_AmbientSky;
				#if defined(FOG_LINEAR)
					UNITY_CALC_FOG_FACTOR(o.pos.z);
					o.vertexColor.a = unityFogFactor;
                #endif
                return o;
            }

            fixed4 frag(VertexOutput i) : COLOR {
                fixed4 _AlphaR_var = tex2D(_AlphaR,i.uv);
                clip(_AlphaR_var.r - 0.5);

                fixed4 shadowMap = tex2D(_MobileShadowTexture, i.uvShadow.xy);
                fixed4 shadow = 1 - saturate(shadowMap.g + shadowMap.b) * _MobileShadowOpacity * i.uvShadow.z;

                fixed3 col = tex2D(_MainTex, i.uv);
                fixed3 finalColor = i.vertexColor.rgb * col;
                finalColor = lerp(finalColor, col, 0.25);

                #ifdef SHADOWTINT
					finalColor *= lerp(_MobileShadowColor, 1, shadow);
				#else
					finalColor *= shadow;
				#endif

                #if defined(FOG_LINEAR)
                  	finalColor = lerp(unity_FogColor, finalColor, saturate(i.vertexColor.a));
				#endif
                return fixed4(finalColor, 1);
            }
            ENDCG
        }

        Pass {
			Tags { LightMode = VertexLM } 
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "TerrainEngine.cginc"
            #pragma target 2.0
 			#pragma multi_compile __ FOG_LINEAR
            #pragma multi_compile __ SHADOWTINT

            uniform sampler2D _MainTex;
            uniform fixed4 _Settings;
            uniform sampler2D _AlphaR;
            uniform sampler2D _MobileShadowTexture;
			uniform float4x4 _MobileShadowMatrix;
			uniform fixed _MobileShadowOpacity;
            fixed4 _MobileShadowColor;

            struct VertexInput {
                half4 vertex : POSITION;
                half3 normal : NORMAL;
                half2 texcoord0 : TEXCOORD0;
                half2 texcoord1 : TEXCOORD1;
                fixed4 vertexColor : COLOR;
            };

            struct VertexOutput {
                half4 pos : SV_POSITION;
                half2 uv : TEXCOORD0;
                fixed3 uvShadow : TEXCOORD1;
                fixed4 vertexColor : COLOR;
                fixed2 lmap : TEXCOORD2;
            };

            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv = v.texcoord0;
                
                half3 world = mul(unity_ObjectToWorld, v.vertex).rgb;
                half3 settings = (_Settings.b * world).rgb;
                fixed4 waves;
                waves = (_Settings.r * _Time.y) + settings.g + settings.b;
                fixed4 s, c;
                waves = frac (waves);
                FastSinCos (waves, s,c);
                fixed deltaT = (_Time.y + world.z*0.2f);
                v.vertex.x += v.vertexColor.r * _Settings.g * s + saturate(v.vertexColor.g) * 0.1f * sin(deltaT) * _Settings.a;
                v.vertex.y += v.vertexColor.r * _Settings.g * c;

                o.pos = UnityObjectToClipPos(v.vertex);

                o.uvShadow = mul(_MobileShadowMatrix, mul(UNITY_MATRIX_M, v.vertex));
                o.vertexColor = 0;
                fixed3 viewpos = mul (UNITY_MATRIX_MV, v.vertex).xyz;
				float3 viewN = normalize (mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal));
				for (int i = 0; i < 4; i++)
				{
					half3 toLight = unity_LightPosition[i].xyz - viewpos.xyz * unity_LightPosition[i].w;
					half lengthSq = dot(toLight, toLight);

                    lengthSq = max(lengthSq, 0.000001);
		            toLight *= rsqrt(lengthSq);

					half atten = 1.0 / (1.0 + lengthSq * unity_LightAtten[i].z );
 
					fixed vertexColor = max ( 0, (dot (viewN, toLight)) );
					o.vertexColor += unity_LightColor[i] * (vertexColor * atten);
				}
                o.uvShadow.z = min(0.5, o.vertexColor) * 2;
				o.vertexColor += unity_AmbientSky;

                o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;

				#if defined(FOG_LINEAR)
					UNITY_CALC_FOG_FACTOR(o.pos.z);
					o.vertexColor.a = unityFogFactor;
                #endif
                return o;
            }

            fixed4 frag(VertexOutput i) : COLOR {
                fixed4 _AlphaR_var = tex2D(_AlphaR,i.uv);
                clip(_AlphaR_var.r - 0.5);

                fixed4 shadowMap = tex2D(_MobileShadowTexture, i.uvShadow.xy);
                fixed4 shadow = 1 - saturate(shadowMap.g + shadowMap.b) * _MobileShadowOpacity * i.uvShadow.z;

                fixed3 col = tex2D(_MainTex, i.uv);

                fixed3 lm = DecodeLightmap (UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmap));

                fixed3 finalColor = i.vertexColor.rgb * col * lm;
                finalColor = lerp(finalColor, col, 0.25);

                #ifdef SHADOWTINT
					finalColor *= lerp(_MobileShadowColor, 1, shadow);
				#else
					finalColor *= shadow;
				#endif

                #if defined(FOG_LINEAR)
                  	finalColor = lerp(unity_FogColor, finalColor, saturate(i.vertexColor.a));
				#endif
                return fixed4(finalColor, 1);
            }
            ENDCG
        }

                Pass {
			Tags { LightMode = VertexLMRGBM } 
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "TerrainEngine.cginc"
            #pragma target 2.0
 			#pragma multi_compile __ FOG_LINEAR
            #pragma multi_compile __ SHADOWTINT

            uniform sampler2D _MainTex;
            uniform fixed4 _Settings;
            uniform sampler2D _AlphaR;
            uniform sampler2D _MobileShadowTexture;
			uniform float4x4 _MobileShadowMatrix;
			uniform fixed _MobileShadowOpacity;
            fixed4 _MobileShadowColor;

            struct VertexInput {
                half4 vertex : POSITION;
                half3 normal : NORMAL;
                half2 texcoord0 : TEXCOORD0;
                half2 texcoord1 : TEXCOORD1;
                fixed4 vertexColor : COLOR;
            };

            struct VertexOutput {
                half4 pos : SV_POSITION;
                half2 uv : TEXCOORD0;
                fixed3 uvShadow : TEXCOORD1;
                fixed4 vertexColor : COLOR;
                fixed2 lmap : TEXCOORD2;
            };

            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv = v.texcoord0;
                
                half3 world = mul(unity_ObjectToWorld, v.vertex).rgb;
                half3 settings = (_Settings.b * world).rgb;
                fixed4 waves;
                waves = (_Settings.r * _Time.y) + settings.g + settings.b;
                fixed4 s, c;
                waves = frac (waves);
                FastSinCos (waves, s,c);
                fixed deltaT = (_Time.y + world.z*0.2f);
                v.vertex.x += v.vertexColor.r * _Settings.g * s + saturate(v.vertexColor.g) * 0.1f * sin(deltaT) * _Settings.a;
                v.vertex.y += v.vertexColor.r * _Settings.g * c;

                o.pos = UnityObjectToClipPos(v.vertex);

                o.uvShadow = mul(_MobileShadowMatrix, mul(UNITY_MATRIX_M, v.vertex));
                o.vertexColor = 0;
                fixed3 viewpos = mul (UNITY_MATRIX_MV, v.vertex).xyz;
				float3 viewN = normalize (mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal));
				for (int i = 0; i < 4; i++)
				{
					half3 toLight = unity_LightPosition[i].xyz - viewpos.xyz * unity_LightPosition[i].w;
					half lengthSq = dot(toLight, toLight);

                    lengthSq = max(lengthSq, 0.000001);
		            toLight *= rsqrt(lengthSq);

					half atten = 1.0 / (1.0 + lengthSq * unity_LightAtten[i].z );
 
					fixed vertexColor = max ( 0, (dot (viewN, toLight)) );
					o.vertexColor += unity_LightColor[i] * (vertexColor * atten);
				}
                o.uvShadow.z = min(0.5, o.vertexColor) * 2;
				o.vertexColor += unity_AmbientSky;

                o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;

				#if defined(FOG_LINEAR)
					UNITY_CALC_FOG_FACTOR(o.pos.z);
					o.vertexColor.a = unityFogFactor;
                #endif
                return o;
            }

            fixed4 frag(VertexOutput i) : COLOR {
                fixed4 _AlphaR_var = tex2D(_AlphaR,i.uv);
                clip(_AlphaR_var.r - 0.5);

                fixed4 shadowMap = tex2D(_MobileShadowTexture, i.uvShadow.xy);
                fixed4 shadow = 1 - saturate(shadowMap.g + shadowMap.b) * _MobileShadowOpacity * i.uvShadow.z;

                fixed3 col = tex2D(_MainTex, i.uv);

                fixed3 lm = DecodeLightmap (UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmap));

                fixed3 finalColor = i.vertexColor.rgb * col * lm;
                finalColor = lerp(finalColor, col, 0.25);

                #ifdef SHADOWTINT
					finalColor *= lerp(_MobileShadowColor, 1, shadow);
				#else
					finalColor *= shadow;
				#endif

                #if defined(FOG_LINEAR)
                  	finalColor = lerp(unity_FogColor, finalColor, saturate(i.vertexColor.a));
				#endif
                return fixed4(finalColor, 1);
            }
            ENDCG
        }

        Pass 
	{
		Name "ShadowCaster"
		Tags { "LightMode" = "ShadowCaster" }
		
		ZWrite On ZTest LEqual Cull Off

		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma target 2.0
		#pragma multi_compile_shadowcaster
		#include "UnityCG.cginc"

        uniform sampler2D _AlphaR;

		struct v2f { 
			V2F_SHADOW_CASTER;
            half2 uv : TEXCOORD0;
		};

		v2f vert( appdata_base v )
		{
			v2f o;
            o.uv = v.texcoord;
			TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
			return o;
		}

		float4 frag( v2f i ) : SV_Target
		{
            fixed4 _AlphaR_var = tex2D(_AlphaR,i.uv);
            clip(_AlphaR_var.r - 0.5);
			SHADOW_CASTER_FRAGMENT(i)
		}
		ENDCG
	}	
    }
}
Shader "MobileShadow/Terrain4Texture" {
    Properties {
        _MainTex ("Main Texture", 2D) = "white" {}
        _SecondTex ("Second Texture", 2D) = "white" {}
		_ThirdTex("Third Texture", 2D) = "white" {}
        _FourthTex("Fourth Texture", 2D) = "white" {}
		_MixTex("Height Mix Texture", 2D) = "gray" {}
        _BlendFactor ("Blend Factor", Range(0.1,0.75)) = 0.4
    }
    SubShader {
        Tags {
            "Queue"="Background"
            "MobileShadow"="Geometry"
        }
        Pass {
            Tags { 
				"LightMode" = "Vertex" 
			 } 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#pragma target 2.0
 			#pragma multi_compile __ FOG_LINEAR
            #pragma multi_compile __ SHADOWTINT
            #include "UnityCG.cginc"
            #pragma shader_feature HEIGHT_MIX
            #pragma shader_feature UV_MIXING

            uniform sampler2D _MainTex; uniform half4 _MainTex_ST;
            uniform sampler2D _SecondTex; uniform half4 _SecondTex_ST;
            uniform sampler2D _ThirdTex; uniform half4 _ThirdTex_ST;
            uniform sampler2D _FourthTex; uniform half4 _FourthTex_ST;
            uniform sampler2D _MixTex; uniform half4 _MixTex_ST;

            fixed _BlendFactor;
            sampler2D _MobileShadowTexture;
			float4x4 _MobileShadowMatrix;
			fixed _MobileShadowOpacity;
            fixed4 _MobileShadowColor;

            struct VertexInput {
                half4 vertex : POSITION;
                half2 uv : TEXCOORD0;
                half3 normal : NORMAL;
                fixed4 vertexColor : COLOR;
            };
            struct v2f {
                half4 pos : SV_POSITION;
                half2 uv0 : TEXCOORD0;
                half2 uv1 : TEXCOORD1;
				half2 uv2 : TEXCOORD2;
                half2 uv3 : TEXCOORD3;
				half2 uvMix : TEXCOORD4;
                fixed3 uvShadow : TEXCOORD5;
				fixed4 color : TEXCOORD6;
                fixed4 vertexcolor : COLOR;
            };

            v2f vert (appdata_full v) {
                v2f o;
		    	o.pos = UnityObjectToClipPos(v.vertex);
                float2 worldPos = mul (unity_ObjectToWorld, v.vertex).xz;
		    	o.uv0 = TRANSFORM_TEX (worldPos, _MainTex);
                o.uv1 = TRANSFORM_TEX (worldPos, _SecondTex);
                o.uv2 = TRANSFORM_TEX (worldPos, _ThirdTex);
                o.uv3 = TRANSFORM_TEX (worldPos, _FourthTex);
                o.uvMix = TRANSFORM_TEX (worldPos, _MixTex);

                o.color = v.color;

				fixed3 viewpos = UnityObjectToViewPos(v.vertex);
                float3 viewN = normalize (mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal));
				o.vertexcolor = 0;
 				
				o.uvShadow = mul(_MobileShadowMatrix, mul(UNITY_MATRIX_M, v.vertex));
				
				//All calculations are in object space
				for (int i = 0; i < 4; i++)
				{
					half3 toLight = unity_LightPosition[i].xyz - viewpos.xyz * unity_LightPosition[i].w;
					half lengthSq = dot(toLight, toLight);

                    lengthSq = max(lengthSq, 0.000001);
					toLight *= rsqrt(lengthSq);

					half atten = 1.0 / (1.0 + lengthSq * unity_LightAtten[i].z );
 
					fixed vertexcolor = max (0, dot (viewN, toLight));
					o.vertexcolor += unity_LightColor[i] * (vertexcolor * atten);
				}
				o.uvShadow.z = min(0.5, o.vertexcolor) * 2;
				o.vertexcolor += unity_AmbientSky;
                #if defined(FOG_LINEAR)
					UNITY_CALC_FOG_FACTOR(o.pos.z);
					o.vertexcolor.a = unityFogFactor;
                #endif
                return o;
            }

            fixed4 frag(v2f i) : COLOR {
                fixed4 shadowMap = tex2D(_MobileShadowTexture, i.uvShadow.xy);
                fixed shadow = 1 - saturate(shadowMap.r + shadowMap.g + shadowMap.b) * _MobileShadowOpacity * i.uvShadow.z;

				fixed4 first = tex2D( _MainTex, i.uv0);
                fixed4 second = tex2D( _SecondTex, i.uv1);
                fixed4 third = tex2D(_ThirdTex, i.uv2);
                fixed4 fourth = tex2D(_FourthTex, i.uv3);

                first = 0.5f * (first + tex2D( _MainTex, i.uv0 * -0.25f));
                second = 0.5f * (second + tex2D( _SecondTex, i.uv1 * -0.25f));
                third = 0.5f * (third + tex2D(_ThirdTex, i.uv2 * -0.25f));
                fourth = 0.5f * (fourth + tex2D(_FourthTex, i.uv3 * -0.25f));

                fixed mix = tex2D(_MixTex, i.uvMix);

                fixed3 c = lerp(first, second, smoothstep(mix - _BlendFactor, mix + _BlendFactor, i.color.r));
                c = lerp(c, third, smoothstep(mix - _BlendFactor, mix + _BlendFactor, i.color.g));
                c = lerp(c, fourth, smoothstep(mix - _BlendFactor, mix + _BlendFactor, i.color.b));

                c *= i.vertexcolor.rgb;

                #ifdef SHADOWTINT
					c *= lerp(_MobileShadowColor, 1, shadow);
				#else
					c *= shadow;
				#endif

				#if defined(FOG_LINEAR)
                  	c = lerp(unity_FogColor, c, saturate(i.vertexcolor.a));
				#endif

                return fixed4(c,1);
            }
            ENDCG
        }
        Pass {
            Tags { 
				"LightMode" = "VertexLM" 
			 } 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#pragma target 2.0
 			#pragma multi_compile __ FOG_LINEAR
            #pragma multi_compile __ SHADOWTINT
            #include "UnityCG.cginc"
            #pragma shader_feature HEIGHT_MIX
            #pragma shader_feature UV_MIXING

            uniform sampler2D _MainTex; uniform half4 _MainTex_ST;
            uniform sampler2D _SecondTex; uniform half4 _SecondTex_ST;
            uniform sampler2D _ThirdTex; uniform half4 _ThirdTex_ST;
            uniform sampler2D _FourthTex; uniform half4 _FourthTex_ST;
            uniform sampler2D _MixTex; uniform half4 _MixTex_ST;

            fixed _BlendFactor;
            sampler2D _MobileShadowTexture;
			float4x4 _MobileShadowMatrix;
			fixed _MobileShadowOpacity;
            fixed4 _MobileShadowColor;

            struct VertexInput {
                half4 vertex : POSITION;
                half2 uv : TEXCOORD0;
                half3 normal : NORMAL;
                fixed4 vertexColor : COLOR;
            };
            struct v2f {
                half4 pos : SV_POSITION;
                half2 uv0 : TEXCOORD0;
                half2 uv1 : TEXCOORD1;
				half2 uv2 : TEXCOORD2;
                half2 uv3 : TEXCOORD3;
				half2 uvMix : TEXCOORD4;
                fixed3 uvShadow : TEXCOORD5;
				fixed4 color : TEXCOORD6;
                fixed4 vertexcolor : COLOR;
                fixed2 lmap : TEXCOORD7;
            };

            v2f vert (appdata_full v) {
                v2f o;
		    	o.pos = UnityObjectToClipPos(v.vertex);
                float2 worldPos = mul (unity_ObjectToWorld, v.vertex).xz;
		    	o.uv0 = TRANSFORM_TEX (worldPos, _MainTex);
                o.uv1 = TRANSFORM_TEX (worldPos, _SecondTex);
                o.uv2 = TRANSFORM_TEX (worldPos, _ThirdTex);
                o.uv3 = TRANSFORM_TEX (worldPos, _FourthTex);
                o.uvMix = TRANSFORM_TEX (worldPos, _MixTex);

                o.color = v.color;

				fixed3 viewpos = UnityObjectToViewPos(v.vertex);
                float3 viewN = normalize (mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal));
				o.vertexcolor = 0;
 				
				o.uvShadow = mul(_MobileShadowMatrix, mul(UNITY_MATRIX_M, v.vertex));
				
				//All calculations are in object space
				for (int i = 0; i < 4; i++)
				{
					half3 toLight = unity_LightPosition[i].xyz - viewpos.xyz * unity_LightPosition[i].w;
					half lengthSq = dot(toLight, toLight);

                    lengthSq = max(lengthSq, 0.000001);
					toLight *= rsqrt(lengthSq);

					half atten = 1.0 / (1.0 + lengthSq * unity_LightAtten[i].z );
 
					fixed vertexcolor = max (0, dot (viewN, toLight));
					o.vertexcolor += unity_LightColor[i] * (vertexcolor * atten);
				}
				o.uvShadow.z = min(0.5, o.vertexcolor) * 2;
				o.vertexcolor += unity_AmbientSky;

                o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;

                #if defined(FOG_LINEAR)
					UNITY_CALC_FOG_FACTOR(o.pos.z);
					o.vertexcolor.a = unityFogFactor;
                #endif
                return o;
            }

            fixed4 frag(v2f i) : COLOR {
                fixed4 shadowMap = tex2D(_MobileShadowTexture, i.uvShadow.xy);
                fixed shadow = 1 - saturate(shadowMap.r + shadowMap.g + shadowMap.b) * _MobileShadowOpacity * i.uvShadow.z;

				fixed4 first = tex2D( _MainTex, i.uv0);
                fixed4 second = tex2D( _SecondTex, i.uv1);
                fixed4 third = tex2D(_ThirdTex, i.uv2);
                fixed4 fourth = tex2D(_FourthTex, i.uv3);

                first = 0.5f * (first + tex2D( _MainTex, i.uv0 * -0.25f));
                second = 0.5f * (second + tex2D( _SecondTex, i.uv1 * -0.25f));
                third = 0.5f * (third + tex2D(_ThirdTex, i.uv2 * -0.25f));
                fourth = 0.5f * (fourth + tex2D(_FourthTex, i.uv3 * -0.25f));

                fixed mix = tex2D(_MixTex, i.uvMix);

                fixed3 lm = DecodeLightmap (UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmap));

                fixed3 c = lerp(first, second, smoothstep(mix - _BlendFactor, mix + _BlendFactor, i.color.r));
                c = lerp(c, third, smoothstep(mix - _BlendFactor, mix + _BlendFactor, i.color.g));
                c = lerp(c, fourth, smoothstep(mix - _BlendFactor, mix + _BlendFactor, i.color.b));

                c *= i.vertexcolor.rgb * lm;

                #ifdef SHADOWTINT
					c *= lerp(_MobileShadowColor, 1, shadow);
				#else
					c *= shadow;
				#endif

				#if defined(FOG_LINEAR)
                  	c = lerp(unity_FogColor, c, saturate(i.vertexcolor.a));
				#endif

                return fixed4(c,1);
            }
            ENDCG
        }
        Pass {
            Tags { 
				"LightMode" = "VertexLMRGBM" 
			 } 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#pragma target 2.0
 			#pragma multi_compile __ FOG_LINEAR
            #pragma multi_compile __ SHADOWTINT
            #include "UnityCG.cginc"
            #pragma shader_feature HEIGHT_MIX
            #pragma shader_feature UV_MIXING

            uniform sampler2D _MainTex; uniform half4 _MainTex_ST;
            uniform sampler2D _SecondTex; uniform half4 _SecondTex_ST;
            uniform sampler2D _ThirdTex; uniform half4 _ThirdTex_ST;
            uniform sampler2D _FourthTex; uniform half4 _FourthTex_ST;
            uniform sampler2D _MixTex; uniform half4 _MixTex_ST;

            fixed _BlendFactor;
            sampler2D _MobileShadowTexture;
			float4x4 _MobileShadowMatrix;
			fixed _MobileShadowOpacity;
            fixed4 _MobileShadowColor;

            struct VertexInput {
                half4 vertex : POSITION;
                half2 uv : TEXCOORD0;
                half3 normal : NORMAL;
                fixed4 vertexColor : COLOR;
            };
            struct v2f {
                half4 pos : SV_POSITION;
                half2 uv0 : TEXCOORD0;
                half2 uv1 : TEXCOORD1;
				half2 uv2 : TEXCOORD2;
                half2 uv3 : TEXCOORD3;
				half2 uvMix : TEXCOORD4;
                fixed3 uvShadow : TEXCOORD5;
				fixed4 color : TEXCOORD6;
                fixed4 vertexcolor : COLOR;
                fixed2 lmap : TEXCOORD7;
            };

            v2f vert (appdata_full v) {
                v2f o;
		    	o.pos = UnityObjectToClipPos(v.vertex);
                float2 worldPos = mul (unity_ObjectToWorld, v.vertex).xz;
		    	o.uv0 = TRANSFORM_TEX (worldPos, _MainTex);
                o.uv1 = TRANSFORM_TEX (worldPos, _SecondTex);
                o.uv2 = TRANSFORM_TEX (worldPos, _ThirdTex);
                o.uv3 = TRANSFORM_TEX (worldPos, _FourthTex);
                o.uvMix = TRANSFORM_TEX (worldPos, _MixTex);

                o.color = v.color;

				fixed3 viewpos = UnityObjectToViewPos(v.vertex);
                float3 viewN = normalize (mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal));
				o.vertexcolor = 0;
 				
				o.uvShadow = mul(_MobileShadowMatrix, mul(UNITY_MATRIX_M, v.vertex));
				
				//All calculations are in object space
				for (int i = 0; i < 4; i++)
				{
					half3 toLight = unity_LightPosition[i].xyz - viewpos.xyz * unity_LightPosition[i].w;
					half lengthSq = dot(toLight, toLight);

                    lengthSq = max(lengthSq, 0.000001);
					toLight *= rsqrt(lengthSq);

					half atten = 1.0 / (1.0 + lengthSq * unity_LightAtten[i].z );
 
					fixed vertexcolor = max (0, dot (viewN, toLight));
					o.vertexcolor += unity_LightColor[i] * (vertexcolor * atten);
				}
				o.uvShadow.z = min(0.5, o.vertexcolor) * 2;
				o.vertexcolor += unity_AmbientSky;

                o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;

                #if defined(FOG_LINEAR)
					UNITY_CALC_FOG_FACTOR(o.pos.z);
					o.vertexcolor.a = unityFogFactor;
                #endif
                return o;
            }

            fixed4 frag(v2f i) : COLOR {
                fixed4 shadowMap = tex2D(_MobileShadowTexture, i.uvShadow.xy);
                fixed shadow = 1 - saturate(shadowMap.r + shadowMap.g + shadowMap.b) * _MobileShadowOpacity * i.uvShadow.z;

				fixed4 first = tex2D( _MainTex, i.uv0);
                fixed4 second = tex2D( _SecondTex, i.uv1);
                fixed4 third = tex2D(_ThirdTex, i.uv2);
                fixed4 fourth = tex2D(_FourthTex, i.uv3);

                first = 0.5f * (first + tex2D( _MainTex, i.uv0 * -0.25f));
                second = 0.5f * (second + tex2D( _SecondTex, i.uv1 * -0.25f));
                third = 0.5f * (third + tex2D(_ThirdTex, i.uv2 * -0.25f));
                fourth = 0.5f * (fourth + tex2D(_FourthTex, i.uv3 * -0.25f));

                fixed mix = tex2D(_MixTex, i.uvMix);

                fixed3 lm = DecodeLightmap (UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmap));

                fixed3 c = lerp(first, second, smoothstep(mix - _BlendFactor, mix + _BlendFactor, i.color.r));
                c = lerp(c, third, smoothstep(mix - _BlendFactor, mix + _BlendFactor, i.color.g));
                c = lerp(c, fourth, smoothstep(mix - _BlendFactor, mix + _BlendFactor, i.color.b));

                c *= i.vertexcolor.rgb * lm;

                #ifdef SHADOWTINT
					c *= lerp(_MobileShadowColor, 1, shadow);
				#else
					c *= shadow;
				#endif

				#if defined(FOG_LINEAR)
                  	c = lerp(unity_FogColor, c, saturate(i.vertexcolor.a));
				#endif

                return fixed4(c,1);
            }
            ENDCG
        }
    }
}

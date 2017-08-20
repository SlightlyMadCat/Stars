Shader "MobileShadow/VertexLit" {
	Properties {
		_MainTex ("Main Texture", 2D) = "white" {}
		_AlphaR ("Alpha Texture", 2D) = "white" {}
		_EmitTex ("Emission Texture", 2D) = "white" {}
		_EmitValue  ("Emission Value", Range (0, 1)) = 0
		_Shininess ("Shininess", Range (.5, 50)) = 8
		_Dissolve ("Dissolve", Range(0, 1)) = 0
		
		//OUTLINE
		_Outline ("Outline Width", Float) = 1
		_OutlineColor ("Outline Color", Color) = (0.0, 1.0, 0.0, 1)

		// Blending state
		[HideInInspector] _Mode ("__mode", Float) = 0.0
		[HideInInspector] _SrcBlend ("__src", Float) = 1.0
		[HideInInspector] _DstBlend ("__dst", Float) = 0.0
		[HideInInspector] _ZWrite ("__zw", Float) = 1.0

		[HideInInspector] _Stencil ("Stencil ID", Float) = 0
		[HideInInspector] _StencilComp ("Stencil Comparison", Float) = 0
		[HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
	}
	SubShader {
		Tags {"Queue"="Geometry" "MobileShadow"="Geometry"}

		Pass {
			Tags { 
				"LightMode" = "Vertex" 
			 } 

			Stencil
			{
				Ref [_Stencil]
				Comp [_StencilComp]
				Pass [_StencilOp] 
			}

			Blend [_SrcBlend] [_DstBlend]
			ZWrite [_ZWrite]
			CGPROGRAM
			#pragma vertex vert  
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile __ FOG_LINEAR
			#pragma multi_compile __ SHADOWTINT
			#pragma shader_feature __ SPECULAR SPECULARFROMCAMERA
			#pragma shader_feature __ EMISSION EMISSION_UV2
			#pragma shader_feature __ CHARACTER GROUND
			#pragma shader_feature CLIP
			#include "UnityCG.cginc"
 
			sampler2D _MainTex; fixed4 _MainTex_ST;

			#ifdef CLIP
				sampler2D _AlphaR;
			#endif

			sampler2D _MobileShadowTexture;
			float4x4 _MobileShadowMatrix;
			fixed _MobileShadowOpacity;
			half3 _MobileShadowSunPosition;
			fixed4 _MobileShadowColor;

			fixed _Dissolve;

			#if defined(SPECULAR) || defined(SPECULARFROMCAMERA)
				fixed _Shininess;
 			#endif

			#if defined(EMISSION) || defined(EMISSION_UV2)
				sampler2D _EmitTex;
				fixed _EmitValue;
 			#endif

			struct v2f
			{
				fixed4 pos : SV_POSITION;
				fixed2 uv0 : TEXCOORD0;
				
				#ifdef EMISSION_UV2
					fixed2 uv1 : TEXCOORD3;
				#endif

				fixed3 uvShadow : TEXCOORD1;
				#if defined(SPECULAR) || defined(SPECULARFROMCAMERA)
					fixed specular : TEXCOORD2;
				#endif
				fixed4 vertexcolor : COLOR;
			};

			v2f vert (appdata_full v)
			{
		    	v2f o;
		    	o.pos = UnityObjectToClipPos(v.vertex);
		    	o.uv0 = TRANSFORM_TEX (v.texcoord, _MainTex);

				#ifdef EMISSION_UV2
					o.uv1 = v.texcoord1;
 				#endif
 
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

				#if defined(SPECULAR)
						half3 worldN = UnityObjectToWorldNormal(v.normal);
						half3 viewDir = normalize(UnityWorldSpaceViewDir(mul(unity_ObjectToWorld, v.vertex)));
						half3 lightDir = _MobileShadowSunPosition;//normalize(UnityWorldSpaceLightDir(mul(unity_ObjectToWorld, v.vertex)));
						half3 reflection = reflect(lightDir,worldN);
						o.specular = pow(max(0, dot(reflection,viewDir)), _Shininess) * v.color.r * o.uvShadow.z;
				#elif defined(SPECULARFROMCAMERA)
						half3 worldN = UnityObjectToWorldNormal(v.normal);
						half3 viewDir = normalize(UnityWorldSpaceViewDir(mul(unity_ObjectToWorld, v.vertex)));
						o.specular = pow(max(0, dot(worldN,viewDir)), _Shininess) * v.color.r * o.uvShadow.z;
				#endif
				return o;
			}
 
			fixed4 frag (v2f i) : COLOR
			{
				#ifdef CLIP
					fixed alpha = tex2D(_AlphaR, i.uv0).r;
                	clip(alpha - _Dissolve);
				#endif

				fixed4 shadowMap = tex2D(_MobileShadowTexture, i.uvShadow.xy);

				#if defined(GROUND)
					fixed shadow = 1 - saturate(shadowMap.r + shadowMap.g + shadowMap.b) * _MobileShadowOpacity * i.uvShadow.z;
				#elif defined(CHARACTER)
					fixed shadow = 1 - saturate(shadowMap.r + shadowMap.b) * _MobileShadowOpacity * i.uvShadow.z;
				#else
					fixed shadow = 1 - saturate(shadowMap.r + shadowMap.g) * _MobileShadowOpacity * i.uvShadow.z;
				#endif

				fixed3 finalColor = tex2D(_MainTex, i.uv0).rgb * i.vertexcolor.rgb;
				
				#ifdef SHADOWTINT
					finalColor *= lerp(_MobileShadowColor, 1, shadow);
				#else
					finalColor *= shadow;
				#endif
                
				#if defined(EMISSION)
					finalColor += tex2D(_EmitTex, i.uv0).rgb * _EmitValue;
				#elif defined(EMISSION_UV2)
					finalColor += tex2D(_EmitTex, i.uv1).rgb * _EmitValue;
 				#endif

				#if defined(SPECULAR) || defined(SPECULARFROMCAMERA)
					finalColor += i.specular;
				#endif

				#if defined(FOG_LINEAR)
                  	finalColor = lerp(unity_FogColor, finalColor, saturate(i.vertexcolor.a));
				#endif

				return fixed4(finalColor, 1);
			}
			ENDCG
		}

		Pass {
			Tags { 
				"LightMode" = "VertexLM" 
			 } 

			Stencil
			{
				Ref [_Stencil]
				Comp [_StencilComp]
				Pass [_StencilOp] 
			}

			Blend [_SrcBlend] [_DstBlend]
			ZWrite [_ZWrite]
			CGPROGRAM
			#pragma vertex vert  
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile __ FOG_LINEAR
			#pragma multi_compile __ SHADOWTINT
			#pragma shader_feature __ SPECULAR SPECULARFROMCAMERA
			#pragma shader_feature __ EMISSION EMISSION_UV2
			#pragma shader_feature __ CHARACTER GROUND
			#pragma shader_feature CLIP
			#include "UnityCG.cginc"
 
			sampler2D _MainTex; fixed4 _MainTex_ST;

			#ifdef CLIP
				sampler2D _AlphaR;
			#endif

			sampler2D _MobileShadowTexture;
			float4x4 _MobileShadowMatrix;
			fixed _MobileShadowOpacity;
			half3 _MobileShadowSunPosition;
			fixed4 _MobileShadowColor;

			fixed _Dissolve;

			#if defined(SPECULAR) || defined(SPECULARFROMCAMERA)
				fixed _Shininess;
 			#endif

			#if defined(EMISSION) || defined(EMISSION_UV2)
				sampler2D _EmitTex;
				fixed _EmitValue;
 			#endif

			struct v2f
			{
				fixed4 pos : SV_POSITION;
				fixed2 uv0 : TEXCOORD0;
				
				fixed3 uvShadow : TEXCOORD1;
				#if defined(SPECULAR) || defined(SPECULARFROMCAMERA)
					fixed specular : TEXCOORD2;
				#endif
				fixed4 vertexcolor : COLOR;
				fixed2 lmap : TEXCOORD4;
			};

			v2f vert (appdata_full v)
			{
		    	v2f o;
		    	o.pos = UnityObjectToClipPos(v.vertex);
		    	o.uv0 = TRANSFORM_TEX (v.texcoord, _MainTex);
 
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

				o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;

				#if defined(SPECULAR)
						half3 worldN = UnityObjectToWorldNormal(v.normal);
						half3 viewDir = normalize(UnityWorldSpaceViewDir(mul(unity_ObjectToWorld, v.vertex)));
						half3 lightDir = _MobileShadowSunPosition;//normalize(UnityWorldSpaceLightDir(mul(unity_ObjectToWorld, v.vertex)));
						half3 reflection = reflect(lightDir,worldN);
						o.specular = pow(max(0, dot(reflection,viewDir)), _Shininess) * v.color.r * o.uvShadow.z;
				#elif defined(SPECULARFROMCAMERA)
						half3 worldN = UnityObjectToWorldNormal(v.normal);
						half3 viewDir = normalize(UnityWorldSpaceViewDir(mul(unity_ObjectToWorld, v.vertex)));
						o.specular = pow(max(0, dot(worldN,viewDir)), _Shininess) * v.color.r * o.uvShadow.z;
				#endif
				return o;
			}
 
			fixed4 frag (v2f i) : COLOR
			{
				#ifdef CLIP
					fixed alpha = tex2D(_AlphaR, i.uv0).r;
                	clip(alpha - _Dissolve);
				#endif

				fixed4 shadowMap = tex2D(_MobileShadowTexture, i.uvShadow.xy);

				#if defined(GROUND)
					fixed shadow = 1 - saturate(shadowMap.r + shadowMap.g + shadowMap.b) * _MobileShadowOpacity * i.uvShadow.z;
				#elif defined(CHARACTER)
					fixed shadow = 1 - saturate(shadowMap.r + shadowMap.b) * _MobileShadowOpacity * i.uvShadow.z;
				#else
					fixed shadow = 1 - saturate(shadowMap.r + shadowMap.g) * _MobileShadowOpacity * i.uvShadow.z;
				#endif

				fixed3 lm = DecodeLightmap (UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmap));
				fixed3 finalColor = tex2D(_MainTex, i.uv0).rgb * i.vertexcolor.rgb * lm;
                
				#ifdef SHADOWTINT
					finalColor *= lerp(_MobileShadowColor, 1, shadow);
				#else
					finalColor *= shadow;
				#endif

				#if defined(EMISSION)
					finalColor += tex2D(_EmitTex, i.uv0).rgb * _EmitValue;
 				#endif

				#if defined(SPECULAR) || defined(SPECULARFROMCAMERA)
					finalColor += i.specular;
				#endif

				#if defined(FOG_LINEAR)
                  	finalColor = lerp(unity_FogColor, finalColor, saturate(i.vertexcolor.a));
				#endif

				return fixed4(finalColor, 1);
			}
			ENDCG
		}

		Pass {
			Tags { 
				"LightMode" = "VertexLMRGBM" 
			 } 

			Stencil
			{
				Ref [_Stencil]
				Comp [_StencilComp]
				Pass [_StencilOp] 
			}

			Blend [_SrcBlend] [_DstBlend]
			ZWrite [_ZWrite]
			CGPROGRAM
			#pragma vertex vert  
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile __ FOG_LINEAR
			#pragma multi_compile __ SHADOWTINT
			#pragma shader_feature __ SPECULAR SPECULARFROMCAMERA
			#pragma shader_feature __ EMISSION EMISSION_UV2
			#pragma shader_feature __ CHARACTER GROUND
			#pragma shader_feature CLIP
			#include "UnityCG.cginc"
 
			sampler2D _MainTex; fixed4 _MainTex_ST;

			#ifdef CLIP
				sampler2D _AlphaR;
			#endif

			sampler2D _MobileShadowTexture;
			float4x4 _MobileShadowMatrix;
			fixed _MobileShadowOpacity;
			half3 _MobileShadowSunPosition;
			fixed4 _MobileShadowColor;

			fixed _Dissolve;

			#if defined(SPECULAR) || defined(SPECULARFROMCAMERA)
				fixed _Shininess;
 			#endif

			#if defined(EMISSION) || defined(EMISSION_UV2)
				sampler2D _EmitTex;
				fixed _EmitValue;
 			#endif

			struct v2f
			{
				fixed4 pos : SV_POSITION;
				fixed2 uv0 : TEXCOORD0;
				
				fixed3 uvShadow : TEXCOORD1;
				#if defined(SPECULAR) || defined(SPECULARFROMCAMERA)
					fixed specular : TEXCOORD2;
				#endif
				fixed4 vertexcolor : COLOR;
				fixed2 lmap : TEXCOORD4;
			};

			v2f vert (appdata_full v)
			{
		    	v2f o;
		    	o.pos = UnityObjectToClipPos(v.vertex);
		    	o.uv0 = TRANSFORM_TEX (v.texcoord, _MainTex);

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

				#if defined(SPECULAR)
						half3 worldN = UnityObjectToWorldNormal(v.normal);
						half3 viewDir = normalize(UnityWorldSpaceViewDir(mul(unity_ObjectToWorld, v.vertex)));
						half3 lightDir = _MobileShadowSunPosition;//normalize(UnityWorldSpaceLightDir(mul(unity_ObjectToWorld, v.vertex)));
						half3 reflection = reflect(lightDir,worldN);
						o.specular = pow(max(0, dot(reflection,viewDir)), _Shininess) * v.color.r * o.uvShadow.z;
				#elif defined(SPECULARFROMCAMERA)
						half3 worldN = UnityObjectToWorldNormal(v.normal);
						half3 viewDir = normalize(UnityWorldSpaceViewDir(mul(unity_ObjectToWorld, v.vertex)));
						o.specular = pow(max(0, dot(worldN,viewDir)), _Shininess) * v.color.r * o.uvShadow.z;
				#endif
				return o;
			}
 
			fixed4 frag (v2f i) : COLOR
			{
				#ifdef CLIP
					fixed alpha = tex2D(_AlphaR, i.uv0).r;
                	clip(alpha - _Dissolve);
				#endif

				fixed4 shadowMap = tex2D(_MobileShadowTexture, i.uvShadow.xy);

				#if defined(GROUND)
					fixed shadow = 1 - saturate(shadowMap.r + shadowMap.g + shadowMap.b) * _MobileShadowOpacity * i.uvShadow.z;
				#elif defined(CHARACTER)
					fixed shadow = 1 - saturate(shadowMap.r + shadowMap.b) * _MobileShadowOpacity * i.uvShadow.z;
				#else
					fixed shadow = 1 - saturate(shadowMap.r + shadowMap.g) * _MobileShadowOpacity * i.uvShadow.z;
				#endif

				fixed3 lm = DecodeLightmap (UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmap));
				fixed3 finalColor = tex2D(_MainTex, i.uv0).rgb * i.vertexcolor.rgb * lm;
                
				#ifdef SHADOWTINT
					finalColor *= lerp(_MobileShadowColor, 1, shadow);
				#else
					finalColor *= shadow;
				#endif

				#if defined(EMISSION)
					finalColor += tex2D(_EmitTex, i.uv0).rgb * _EmitValue;
 				#endif

				#if defined(SPECULAR) || defined(SPECULARFROMCAMERA)
					finalColor += i.specular;
				#endif

				#if defined(FOG_LINEAR)
                  	finalColor = lerp(unity_FogColor, finalColor, saturate(i.vertexcolor.a));
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
		#pragma shader_feature CLIP
		#include "UnityCG.cginc"

		#ifdef CLIP
			sampler2D _AlphaR;
		#endif

		struct v2f { 
			V2F_SHADOW_CASTER;
			#ifdef CLIP
            	half2 uv : TEXCOORD0;
			#endif
		};

		v2f vert( appdata_base v )
		{
			v2f o;
			#ifdef CLIP
            	o.uv = v.texcoord;
			#endif
			TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
			return o;
		}

		float4 frag( v2f i ) : SV_Target
		{
			#ifdef CLIP
            	fixed4 _AlphaR_var = tex2D(_AlphaR,i.uv);
            	clip(_AlphaR_var.r - 0.5);
			#endif
			SHADOW_CASTER_FRAGMENT(i)
		}
		ENDCG
	}
	}
	CustomEditor "VertexLitGUI"
}
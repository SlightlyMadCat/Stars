Shader "Hidden/MobileShadowReplacementShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_AlphaR ("Alpha Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags {"MobileShadow"="Geometry"}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return float4(0, 0, 1, 1);
			}
			ENDCG
		}
	}

	SubShader
	{
		Tags {"MobileShadow"="AlphaClip"}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			uniform sampler2D _AlphaR; uniform float4 _AlphaR_ST;
			fixed _Dissolve;

			struct appdata
			{
				float4 vertex : POSITION;
				half2 texcoord0 : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				fixed2 uv0 : TEXCOORD0;
			};
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv0 = TRANSFORM_TEX(v.texcoord0, _AlphaR);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed _alpha = tex2D(_AlphaR, i.uv0).r;
                clip(_alpha - _Dissolve);
				return float4(0, 0, 1, 1);
			}
			ENDCG
		}
	}

	SubShader
	{
		Tags {"MobileShadow"="Character"}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return float4(0, 1, 0, 1);
			}
			ENDCG
		}
	}

	SubShader
	{
		Tags {"MobileShadow"="CharacterAlphaClip"}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			uniform sampler2D _AlphaR; uniform float4 _AlphaR_ST;

			fixed _Dissolve;
			
			struct appdata
			{
				float4 vertex : POSITION;
				half2 texcoord0 : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				fixed2 uv0 : TEXCOORD0;
			};
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv0 = TRANSFORM_TEX(v.texcoord0, _AlphaR);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed _alpha = tex2D(_AlphaR, i.uv0).r;
                clip(_alpha - _Dissolve);
				return float4(0, 1, 0, 1);
			}
			ENDCG
		}
	}

		SubShader
	{
		Tags {"MobileShadow"="Ground"}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return float4(0, 0, 0, 1);
			}
			ENDCG
		}
	}

	SubShader
	{
		Tags {"MobileShadow"="GroundAlphaClip"}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			uniform sampler2D _AlphaR; uniform float4 _AlphaR_ST;
			fixed _Dissolve;

			struct appdata
			{
				float4 vertex : POSITION;
				half2 texcoord0 : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				fixed2 uv0 : TEXCOORD0;
			};
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv0 = TRANSFORM_TEX(v.texcoord0, _AlphaR);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed _alpha = tex2D(_AlphaR, i.uv0).r;
                clip(_alpha - _Dissolve);
				return float4(0, 0, 0, 1);
			}
			ENDCG
		}
	}

	 SubShader {
        Tags {
            "Queue"="Geometry+90"
            "MobileShadow"="Grass"
        }
            Cull Off
    Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal
            #pragma target 2.0

            uniform half4 _Settings;
            uniform sampler2D _AlphaR;

            struct VertexInput {
                half4 vertex : POSITION;
                half3 normal : NORMAL;
                half2 texcoord0 : TEXCOORD0;
                half4 vertexColor : COLOR;
            };

            struct VertexOutput {
                half4 pos : SV_POSITION;
                fixed2 uv0 : TEXCOORD0;
                fixed2 uv1 : TEXCOORD1;
            };

            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                    
				float2 settings = (_Settings.b*mul(unity_ObjectToWorld, v.vertex).rgb).rb;
                v.vertex.xz += v.vertexColor.r * _Settings.g * sin((_Settings.r * _Time.g) + settings.r + settings.g);
               
			    o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(VertexOutput i) : COLOR {
                // fixed4 _AlphaR_var = tex2D(_AlphaR, i.uv0);
                // clip(_AlphaR_var.r - 0.1);
                return float4(0, 0, 0, 0);
            }
            ENDCG
        }
    }

	SubShader {
        Tags {
            "Queue"="Transparent"
            "MobileShadow"="Foliage"
        }
            ZWrite Off
            Cull Off

        Pass {
            Lighting Off Fog { Mode Off }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "TerrainEngine.cginc"
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
            #pragma target 2.0
            uniform fixed4 _Settings;
            uniform sampler2D _AlphaR;

            struct VertexInput {
                half4 vertex : POSITION;
                half3 normal : NORMAL;
                half2 texcoord0 : TEXCOORD0;
                half2 texcoord1 : TEXCOORD1;
                fixed4 vertexColor : COLOR;
            };
            struct VertexOutput {
                half4 pos : SV_POSITION;
                half2 uv0 : TEXCOORD0;
            };

            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;

				half3 world = mul(unity_ObjectToWorld, v.vertex).rgb;
				half3 settings = (_Settings.b * world).rgb;
				fixed4 waves;
				waves = (_Settings.r * _Time.y) + settings.g + settings.b;
				fixed4 s, c;
				waves = frac (waves);
				FastSinCos (waves, s,c);
				fixed deltaT = (_Time.y + world.z*0.2f);
				v.vertex.x += v.vertexColor.r * _Settings.g * s + saturate(v.texcoord1.y) * 0.1f * sin(deltaT) * _Settings.a;
				v.vertex.y += v.vertexColor.r * _Settings.g * c;

                o.pos = UnityObjectToClipPos(v.vertex);

                return o;
            }

            fixed4 frag(VertexOutput i) : COLOR {
                float4 _AlphaR_var = tex2D(_AlphaR, i.uv0);
                clip(_AlphaR_var.r - 0.5);
                return fixed4(1, 0, 0, 1);
            }
            ENDCG
        }
    }
}

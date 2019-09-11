Shader "Hidden/ScalableFrame"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _RectSize("RectSize", vector) = (0, 0, 0, 0)
        _TexSize("TexSize", vector) = (0, 0, 0, 0)
    }
        SubShader
        {
            Blend SrcAlpha OneMinusSrcAlpha

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                    float4 color : COLOR0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 color : COLOR0;
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                float4 _RectSize;
                float4 _TexSize;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    o.color = v.color;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    float2 pixelPos = _RectSize.xy * i.uv;
                    float2 samplePos = float2(0, 0);
                    float2 sampleRange = _TexSize * 0.5;
                    samplePos.x =
                        pixelPos.x < sampleRange.x ? pixelPos.x / _TexSize.x :
                        pixelPos.x > _RectSize.x - sampleRange.x ? 0.5 + (pixelPos.x - (_RectSize.x - sampleRange.x)) / _TexSize.x :
                        0.5;
                    samplePos.y =
                        pixelPos.y < sampleRange.y ? pixelPos.y / _TexSize.y :
                        pixelPos.y > _RectSize.y - sampleRange.y ? 0.5 + (pixelPos.y - (_RectSize.y - sampleRange.y)) / _TexSize.y :
                        0.5;
                    float4 color = tex2D(_MainTex, samplePos);
                    return i.color * color;
                }
                ENDCG
            }
        }
}

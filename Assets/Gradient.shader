Shader "Hidden/Gradient"
{
    CGINCLUDE

float3 Plasma(float2 uv, float time)
{
    float l = sin(uv.x * sin(time * 1.3f) + sin(uv.y * 4 + time) * sin(time));
    return sin(float3(6, 7, 10) * 2 * l) * 0.5 + 0.5;
}

float3 Hue2RGB(float hue)
{
    float h = hue * 6 - 2;
    float r = abs(h - 1) - 1;
    float g = 2 - abs(h);
    float b = 2 - abs(h - 2);
    return saturate(float3(r, g, b));
}

void Vertex(float4 position : POSITION,
            float2 texCoord : TEXCOORD0,
            out float4 outPosition : SV_Position,
            out float2 outTexCoord : TEXCOORD0)
{
    outPosition = UnityObjectToClipPos(position);
    outTexCoord = texCoord;
}

float4 Fragment(float4 position : SV_Position,
                float2 texCoord : TEXCOORD0) : SV_Target
{
    float3 plasma = Plasma(texCoord, _Time.y);
    float gray = floor(texCoord.x * 11) / 10;
    float3 hue = Hue2RGB(gray);
    float3 rgb = texCoord.y < 0.6 ? plasma : (texCoord.y < 0.8 ? hue : gray);
    return float4(rgb, 1);
}

    ENDCG

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            ENDCG
        }
    }
}

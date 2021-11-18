Shader "Hidden/Gradient"
{
    CGINCLUDE

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
    float l = sin(texCoord.x * sin(_Time.y * 1.3f) +
              sin(texCoord.y * 4 + _Time.y) * sin(_Time.y));
    
    float3 rgb = sin(float3(6, 7, 10) * 2 * l) * 0.5 + 0.5;

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

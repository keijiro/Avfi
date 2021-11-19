Shader "Hidden/Conversion"
{
    Properties
    {
        _MainTex("", 2D) = ""
    }

    CGINCLUDE

sampler2D _MainTex;

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
    return tex2D(_MainTex, float2(texCoord.x, 1 - texCoord.y)).rgba;
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

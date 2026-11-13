sampler2D uImage : register(s0);
texture uNoise;
sampler uNoiseSampler =
sampler_state
{
    Texture = <uNoise>;
    MipFilter = LINEAR;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    AddressU = WRAP;
    AddressV = WRAP;
};
float duration;
float uNoiseSize;//不允许填0
float4 uDissolveColor;
float2 uNoiseXY;
float4x4 uTransform;

struct VSInput
{
    float2 Pos : POSITION0;
    float4 Color : COLOR0;
    float3 Texcoord : TEXCOORD0;
};

struct PSInput
{
    float4 Pos : SV_POSITION;
    float4 Color : COLOR0;
    float3 Texcoord : TEXCOORD0;
};

PSInput VertexShaderFunction(VSInput input)
{
    PSInput output;
    output.Color = input.Color;
    output.Texcoord = input.Texcoord;
    output.Pos = mul(float4(input.Pos, 0, 1), uTransform);
    return output;
}

float4 PixelShaderFunction(PSInput input) : COLOR0
{
    float4 colorNoise = tex2D(uNoiseSampler, input.Texcoord.xy / uNoiseSize + uNoiseXY);
	input.Texcoord.x %= 1;
    float4 mainTex = tex2D(uImage, input.Texcoord.xy);
    if (!any(mainTex))
        return float4(0, 0, 0, 0);
	if (colorNoise.r + input.Texcoord.z < duration)
        return mainTex * input.Color;
	if (colorNoise.r + input.Texcoord.z < duration + 0.2)
        return uDissolveColor;
    return float4(0, 0, 0, 0);
}

technique Technique1
{
    pass Test
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}

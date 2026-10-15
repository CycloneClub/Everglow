sampler2D uImage : register(s0);
texture uHeatMap;
sampler uHeatMapSampler =
sampler_state
{
    Texture = <uHeatMap>;
    MipFilter = LINEAR;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    AddressU = CLAMP;
    AddressV = CLAMP;
};
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
	float2 origCoord = input.Texcoord.xy - float2(0.5, 0.5);
	origCoord.y /= input.Texcoord.z;
	origCoord += float2(0.5, 0.5);
	float2 modifiedCoord = origCoord;
	if (modifiedCoord.y < 0 || modifiedCoord.y > 1)
	{
		return float4(0, 0, 0, 0);
	}
	
	float4 colorNoise = tex2D(uNoiseSampler, modifiedCoord);
	float4 origColor = tex2D(uImage, modifiedCoord) * colorNoise;
	float4 colorHeatMap = tex2D(uHeatMapSampler, float2(origColor.r, 0.5));
	float4 finalColor = colorHeatMap * input.Color;
	return finalColor;
}

technique Technique1
{
    pass Test
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}

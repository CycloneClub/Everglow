sampler2D uImage : register(s0);
float sv_Pos_Y;
float uSize;
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

float4x4 uTransform;


struct PSInput
{
    float4 Pos : SV_POSITION;
    float4 Color : COLOR0;
    float3 Texcoord : TEXCOORD0;
};

float4 PixelShaderFunction(PSInput input) : COLOR0
{
    float4 colorBase = tex2D(uImage, input.Texcoord.xy);
	if (!any(colorBase))
	{
		return float4(0, 0, 0, 0);
	}
	float deltaY = abs((input.Pos.y - sv_Pos_Y) * uSize);
	if (colorBase.r > 0.5)
	{
		return tex2D(uHeatMapSampler, float2(deltaY, 0.75));
	}
	if (colorBase.r < 0.5)
	{
		return tex2D(uHeatMapSampler, float2(deltaY, 0.25));
	}
    return float4(0, 0, 0, 0);
}

technique Technique1
{
    pass Test
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}

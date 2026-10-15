sampler2D uImage : register(s0);

struct PSInput
{
    float4 Pos : SV_POSITION;
    float4 Color : COLOR0;
    float3 Texcoord : TEXCOORD0;
};


float4 PixelShaderFunction(PSInput input) : COLOR0
{
	float4 texColor = tex2D(uImage, input.Texcoord.xy);
	if (texColor.r + texColor.g + texColor.b > 1.5)
	{
		return float4(1, 1, 1, 1) * input.Color;
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

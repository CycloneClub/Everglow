sampler2D uImage : register(s0);

struct PSInput
{
    float4 Pos : SV_POSITION;
    float4 Color : COLOR0;
    float3 Texcoord : TEXCOORD0;
};

float4 PixelShaderFunction(PSInput input) : COLOR0
{
    float light = tex2D(uImage, input.Texcoord.xy).r * input.Color.r;
    light *= sin((((input.Texcoord.y - 0.5) * 5) + 0.5) * 3.14159) * 2.4;
    if (light > 0.4 && light < 0.7)
	{
	    float4 c0 = float4(0.5, 0, 1, 1) * (0.7 - light) * 4;
		c0.a = 1;
	    return c0;
	}
	if (light >= 0.7)
        return float4(0, 0, 0, 1);
    return float4(0, 0, 0, 0);
}

technique Technique1
{
    pass Test
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}

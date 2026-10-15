sampler2D uImage : register(s0);
float hitTimer;
struct PSInput
{
    float4 Pos : SV_POSITION;
    float4 Color : COLOR0;
    float3 Texcoord : TEXCOORD0;
};

float4 PixelShaderFunction(PSInput input) : COLOR0
{
    float4 color = tex2D(uImage, input.Texcoord.xy);
	if (any(color))
	{
		float value = hitTimer / 20.0;
		return lerp(float4(1, 0, 0, 1), float4(1, 1, 1, 1), value) * value;
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

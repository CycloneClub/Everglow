sampler uImage0 : register(s0);

struct PSInput
{
	float4 Pos : SV_POSITION;
	float4 Color : COLOR0;
	float3 Texcoord : TEXCOORD0;
};

float4 PixelShaderFunction(float3 coords : TEXCOORD0, PSInput input) : COLOR0
{
	float cy = clamp(coords.y, 0.01, 0.99);
	float4 BackG = tex2D(uImage0, float2(coords.x - floor(coords.x), cy /*saturate(coords.y)*/));
	BackG.rgba *= input.Color.rgba;
	return BackG;
}

technique Technique1
{
	pass Test
	{
		PixelShader = compile ps_3_0 PixelShaderFunction();
	}
}

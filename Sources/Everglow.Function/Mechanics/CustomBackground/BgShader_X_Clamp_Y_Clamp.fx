sampler uImage0 : register(s0);

struct PSInput
{
	float4 Pos : SV_POSITION;
	float4 Color : COLOR0;
	float3 Texcoord : TEXCOORD0;
};

float4 PixelShaderFunction(float3 coords : TEXCOORD0, PSInput input) : COLOR0
{
	float4 BackG = tex2D(uImage0, float2(saturate(coords.x), saturate(coords.y)));
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

sampler2D uImage0 : register(s0);

float4x4 uTransform;
float uThredshold;

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
	float4 c0 = tex2D(uImage0, input.Texcoord.xy);
	float value = c0.r * input.Texcoord.z;
	if (value > uThredshold + 0.05)
	{
		return float4(0.52941, 0.52941, 0.52941, 1) * input.Color;
	}
	if (value > uThredshold)
	{
		return float4(1, 1, 1, 1) * input.Color;
	}
	return float4(0, 0 ,0 ,0);
}
technique Technique1
{
    pass Test
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}

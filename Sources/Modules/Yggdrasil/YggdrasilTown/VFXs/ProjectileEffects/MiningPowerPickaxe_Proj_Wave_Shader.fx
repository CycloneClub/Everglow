sampler2D uImage0 : register(s0);
float2 uMousePos;
float2 colorParaPlus;
float2 colorParaMul;
float2 uTime;
float disPara;
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
	float4 tex = tex2D(uImage0, input.Texcoord.xy);
	float dis = length(input.Pos - uMousePos) * disPara;
	float value = -abs(uTime - dis) + tex.r * colorParaMul + colorParaPlus;
	if (value > 0)
	{
		return input.Color;
	}
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

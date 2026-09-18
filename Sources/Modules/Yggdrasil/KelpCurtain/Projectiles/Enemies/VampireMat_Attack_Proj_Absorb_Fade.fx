sampler2D uImage : register(s0);

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
	float fade = input.Texcoord.z;
    float4 color = tex2D(uImage, input.Texcoord.xy);
	color -= float4(fade, fade, fade, fade);
	color = clamp(color, float4(0, 0, 0, 0), float4(1, 1, 1, 1));
	return color * input.Color;
}

technique Technique1
{
    pass Test
	{
		VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}

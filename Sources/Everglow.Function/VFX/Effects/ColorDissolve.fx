sampler2D uImage0 : register(s0);
texture2D uDissolveNoise;
sampler2D uDissolveNoiseTex = sampler_state
{
	Texture = <uDissolveNoise>;
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
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
	float4 tex = tex2D(uImage0, input.Texcoord.xy);
	float4 heat = tex2D(uDissolveNoiseTex, input.Texcoord.xy);
	if (!any(tex))
	{
		return float4(0, 0, 0, 0);
	}
	if (input.Texcoord.z * 1.2 < heat.r)
	{
		return float4(0, 0, 0, 0);
	}
	else if (input.Texcoord.z * 1.2 < heat.r + 0.2)
	{
		return lerp(tex, float4(0, 0, 0, 1), 0.5f) * input.Color;
	}
	return tex * input.Color;
}
technique Technique1
{
    pass Test
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}

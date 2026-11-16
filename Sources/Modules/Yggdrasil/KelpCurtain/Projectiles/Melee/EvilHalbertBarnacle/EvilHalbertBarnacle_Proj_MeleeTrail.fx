texture2D tex0;
sampler2D uShapeTex = sampler_state
{
    Texture = <tex0>;
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};
texture2D tex1;
sampler2D uColorTex = sampler_state
{
    Texture = <tex1>;
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
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
    output.Texcoord = input.Texcoord;
    output.Color = input.Color;
    output.Pos = mul(float4(input.Pos, 0, 1), uTransform);
    return output;
}

float4 PixelShaderFunction2(PSInput input) : COLOR0
{
	float3 coord = input.Texcoord;
	float4 light = tex2D(uShapeTex, coord.xy); //主纹理
	float4 c = tex2D(uColorTex, float2(clamp(light.r, 0.03f, 0.97f), 0.5f));
	c *= input.Color;
	c.a = 0;
	return c * 1.7;
}

float4 PixelShaderFunction3(PSInput input) : COLOR0
{
    float3 coord = input.Texcoord;
	float4 light = tex2D(uShapeTex, coord.xy); //主纹理
	float4 c = tex2D(uColorTex, float2(clamp(light.r, 0.01f, 0.99f), 0.5f));
    c *= input.Color;
	c.a = 0;
    return c;
}

float4 PixelShaderFunction4(PSInput input) : COLOR0
{
	float3 coord = input.Texcoord;
	float4 c = tex2D(uShapeTex, coord.xy); //主纹理
	c.a = c.r;
	c.r = 0;
	c.g = 0;
	c.b = 0;
	return c * input.Color;
}

technique Technique1
{
	pass ArcBladeAffectByEnvironmentLightPlus
	{
		VertexShader = compile vs_2_0 VertexShaderFunction();
		PixelShader = compile ps_2_0 PixelShaderFunction2();
	}
    pass ArcBladeAffectByEnvironmentLight
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction3();
    }
	pass ArcBladeConvertTransparent
	{
		VertexShader = compile vs_2_0 VertexShaderFunction();
		PixelShader = compile ps_2_0 PixelShaderFunction4();
	}
}

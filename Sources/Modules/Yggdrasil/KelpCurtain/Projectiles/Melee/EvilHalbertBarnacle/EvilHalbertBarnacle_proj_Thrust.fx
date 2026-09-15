sampler2D uImage : register(s0);
sampler2D uImage1 : register(s1);
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
    float newX = input.Texcoord.x % 1;
    float newY = input.Texcoord.y - 0.5;
    newY *= 1 / sin(input.Texcoord.z * 3.141592653589793238);
    newY += 0.5;
    newY = clamp(newY, 0, 1);
    float2 newCoord = float2(newX, newY);
    float4 light = tex2D(uImage, newCoord);
	float4 c = tex2D(uImage1, float2(clamp(light.r, 0.01f, 0.99f), 0.5f));
	c *= input.Color;
    return c;
}

float4 PixelShaderFunction_Dark(PSInput input) : COLOR0
{
	float newX = input.Texcoord.x % 1;
	float newY = input.Texcoord.y - 0.5;
	newY *= 1 / sin(input.Texcoord.z * 3.141592653589793238);
	newY += 0.5;
	newY = clamp(newY, 0, 1);
	float2 newCoord = float2(newX, newY);
	float4 c = tex2D(uImage, newCoord);
	c.a = c.r;
	c.r = 0;
	c.g = 0;
	c.b = 0;
	c *= input.Color;
	return c;
}

technique Technique1
{
    pass LightEffect
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }

	pass DarkEffect
	{
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 PixelShaderFunction_Dark();
	}
}

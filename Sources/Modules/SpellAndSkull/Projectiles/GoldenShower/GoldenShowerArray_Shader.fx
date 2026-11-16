sampler2D uImage0 : register(s0);
sampler2D uImage1 : register(s1);
float4x4 uTransform;
float uTime;
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
    float4 color0 = tex2D(uImage0, input.Texcoord);
    float4 color1 = tex2D(uImage1, input.Texcoord + float2(0, uTime));
    float4 mulColor = color0 * (color1.r * input.Texcoord.y) * 50;
    return mulColor * input.Color;
}

technique Technique1
{
    pass Shader2D
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}

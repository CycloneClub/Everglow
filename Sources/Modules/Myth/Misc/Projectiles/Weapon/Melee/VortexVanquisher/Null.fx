sampler uImage0 : register(s0);
sampler uImage1 : register(s1);

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
PSInput VSFunction(VSInput input)
{
    PSInput output;
    output.Texcoord = input.Texcoord;
    output.Color = input.Color;
    output.Pos = mul(float4(input.Pos, 0, 1), uTransform);
    return output;
}

float4 PSFunction(PSInput input) : COLOR0
{
    float4 c0 = tex2D(uImage0, input.Texcoord) * input.Color;

    return c0;   
}
technique Technique1
{
    pass Test
    {
        VertexShader = compile vs_3_0 VSFunction();
        PixelShader = compile ps_3_0 PSFunction();
    }
}
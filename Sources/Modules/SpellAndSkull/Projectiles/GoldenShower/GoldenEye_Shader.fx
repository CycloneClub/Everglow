sampler2D uImage0 : register(s0);

float4x4 uTransform;
float uTime;
struct VSInput
{
    float2 Pos : POSITION0;
    float4 Color : COLOR0;
    float4 Texcoord : TEXCOORD0;
};

struct PSInput
{
    float4 Pos : SV_POSITION;
    float4 Color : COLOR0;
    float4 Texcoord : TEXCOORD0;
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
    float2 newCoord = input.Texcoord - float2(0.5, 0.5);
    newCoord.y /= input.Texcoord.z;
    newCoord += float2(0.5, 0.5);
    newCoord.y = clamp(newCoord.y, 0, 1);
    newCoord.x += uTime;
    return tex2D(uImage0, newCoord) * input.Color;
}

technique Technique1
{
    pass Shader2D
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}

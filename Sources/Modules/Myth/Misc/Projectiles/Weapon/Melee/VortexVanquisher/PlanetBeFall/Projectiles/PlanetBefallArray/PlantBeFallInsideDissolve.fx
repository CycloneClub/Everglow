sampler2D uImage : register(s0);
texture uNoise;
sampler uNoiseSampler =
sampler_state
{
    Texture = <uNoise>;
    MipFilter = LINEAR;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    AddressU = WRAP;
    AddressV = WRAP;
};


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
    float4 colorNoise = tex2D(uNoiseSampler, input.Texcoord.xy + float2(uTime, 0));
    float4 color = tex2D(uImage, input.Texcoord.xy);
    if (color.r == 0)
        return float4(0, 0, 0, 0);
    color = colorNoise * color * 2;
    float toCenter = length(input.Texcoord.xy - float2(0.5, 0.5));
    color *= clamp(1 - abs(uTime - toCenter) * 1.5, 0, 1);
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
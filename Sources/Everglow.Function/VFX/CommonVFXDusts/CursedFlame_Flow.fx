sampler2D uImage : register(s0);
texture uHeatMap;
sampler uHeatMapSampler =
sampler_state
{
    Texture = <uHeatMap>;
    MipFilter = LINEAR;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    AddressU = CLAMP;
    AddressV = CLAMP;
};
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
//valueC由Color.r代替
//float valueC;
//utime由Color.g代替
//float utime;
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
    float4 color = tex2D(uNoiseSampler, float2(input.Texcoord.yz));
    float2 newTexCoord = float2(0.5, input.Texcoord.x);
    newTexCoord.y -= 0.5;
    newTexCoord.y /= input.Color.w;
    newTexCoord.y += 0.5;
    newTexCoord.y = clamp(newTexCoord.y, 0, 1);
    float4 colorHalo = tex2D(uImage, newTexCoord);
    float light = colorHalo.r * color.r * (1 - input.Color.b);
    
    //float4 color = tex2D(uNoiseSampler, float2(input.Texcoord.yz));
    //float2 newTexCoord = float2(0.5, input.Texcoord.x);
    //newTexCoord.y -= 0.5;
    //newTexCoord.y /= input.Color.w;
    //newTexCoord.y += 0.5;
    //newTexCoord.y = clamp(newTexCoord.y, 0, 1);
    //float4 colorHalo = tex2D(uImage, newTexCoord) * color;
    
    
    return float4(light, light, light, 0);
}

technique Technique1
{
    pass Test
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
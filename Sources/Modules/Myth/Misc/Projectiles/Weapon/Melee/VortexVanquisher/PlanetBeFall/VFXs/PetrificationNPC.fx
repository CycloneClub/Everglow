sampler2D uImage : register(s0);
texture uHeatMap;
int uImageSize;

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
float4 drawColor;
float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 colorTarget = tex2D(uImage, coords);
    if (!any(colorTarget))
        return colorTarget;
    float4 colorNoise = tex2D(uNoiseSampler, float2(coords.x, fmod(coords.y * uImageSize, 1)));
    float light = colorNoise.r;
    float4 colorHeatMap = tex2D(uHeatMapSampler, float2(light, 0));
    return float4(colorHeatMap);
}

technique Technique1
{
    pass Test
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
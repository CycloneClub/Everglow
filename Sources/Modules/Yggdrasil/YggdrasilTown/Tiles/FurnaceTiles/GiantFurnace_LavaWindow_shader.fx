sampler2D uImage0 : register(s0);
texture uLight;
float uSize;
float uTime;
sampler uLightSampler =
sampler_state
{
	Texture = <uLight>;
	MipFilter = POINT;
	MinFilter = POINT;
	MagFilter = POINT;
	AddressU = WRAP;
	AddressV = WRAP;
};
texture uHeatmap;
sampler uHeatmapSampler =
sampler_state
{
	Texture = <uHeatmap>;
	MipFilter = POINT;
	MinFilter = POINT;
	MagFilter = POINT;
	AddressU = CLAMP;
	AddressV = CLAMP;
};

struct PSInput
{
    float4 Pos : SV_POSITION;
    float4 Color : COLOR0;
    float3 Texcoord : TEXCOORD0;
};

float4 PixelShaderFunction(PSInput input) : COLOR0
{
    float4 drawColor=input.Color;
    float2 uv=input.Texcoord.xy;
    float4 col = tex2D(uImage0, uv);
	float lightColor = tex2D(uLightSampler, uv * uSize + float2(0, uTime * 0.05)).r;
	float uLavaLevelHeight = 0.25;
    if(!any(col))
    {
        return float4(0, 0, 0, 0);
    }

	for (int k = 0; k < 8;k++)
	{
		uLavaLevelHeight += 0.04 * pow(2, -k) * sin(uTime * cos(k * 3.73) + uv.x * 4 * pow(2, k));
	}
	float4 lavaColor = float4(1,0.5,0,1);
	if (uv.y < uLavaLevelHeight)
	{
		return float4(0.3, 0.1, 0.18, 1);
	}
	if (uv.y < uLavaLevelHeight + 0.03)
	{
		lavaColor = float4(0.75, 0, 0, 1);
	}
	return col * lavaColor * input.Color;
}

technique Technique1
{
    pass Shader2D
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}

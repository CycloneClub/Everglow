sampler uImage0 : register(s0);
float2 uSize;
float uBlurValue;
float uDelta;
float3x3 uColorMatrix = { 0.0625, 0.125, 0.0625, 0.125, 0.25, 0.125, 0.0625, 0.125, 0.0625 };

struct PSInput
{
    float4 Pos : SV_POSITION;
    float4 Color : COLOR0;
    float2 Texcoord : TEXCOORD0;
};

float4 Blur(PSInput input) : COLOR0
{
	float2 coord = input.Texcoord.xy;
    float dx = 1.0 / uSize.x;
    float dy = 1.0 / uSize.y;
	float4 blurColor = float4(0, 0, 0, 0);
	for (int i = -1; i <= 1;i++)
	{
		for (int j = -1; j <= 1; j++)
		{
			blurColor += tex2D(uImage0, coord + float2(i * dx, j * dy) * uDelta) * uColorMatrix[i + 1][j + 1];
		}
	}
	float4 color5 = tex2D(uImage0, coord);
	if (!any(color5))
	{
		return float4(0, 0, 0, 0);

	}
	float4 c = blurColor * uBlurValue + color5 * (1 - uBlurValue);
    return c * input.Color;
}


technique Technique1
{
    pass Blur
    {
        PixelShader = compile ps_3_0 Blur();
    }
}

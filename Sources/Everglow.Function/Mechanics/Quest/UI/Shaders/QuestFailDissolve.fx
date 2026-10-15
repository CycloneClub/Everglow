sampler2D uImage0 : register(s0);
sampler2D uImage1 : register(s1);
float uvMulti;
float2 uCenter;
float uDissolve;
float2 resolution;
float threthod;


struct PSInput
{
	float4 Pos : SV_POSITION;
	float4 Color : COLOR0;
	float3 Texcoord : TEXCOORD0;
};

float4 PixelShaderFunction(PSInput input) : COLOR0
{
	float4 drawColor = input.Color;
	float2 uv = input.Texcoord.xy;
	float4 col = tex2D(uImage0, uv);
	float uvm = 0.3;
	if (uvMulti != 0)
		uvm = uvMulti;
	float4 noise = tex2D(uImage1, uv * uvm);
	float n = noise.r;
	float2 toCenter = uv - uCenter;
	toCenter.x *= resolution.x / resolution.y;
	float distance = length(toCenter);
	float value = uDissolve + n - distance;
	if (value >= 1)
	{
		return float4(0, 0, 0, 0);
	}
	if (value < 1 && value >= threthod)
	{
		return float4(0, 0, 0, 0);
	}
	return col * drawColor;
}
technique Technique1
{
	pass Test
	{
		PixelShader = compile ps_3_0 PixelShaderFunction();
	}
}

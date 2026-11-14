sampler2D uImage0 : register(s0);
sampler2D uImage1 : register(s1);

float4x4 uTransform;


struct PSInput
{
	float4 Pos : SV_POSITION;
	float4 Color : COLOR0;
	float3 Texcoord : TEXCOORD0;
};

float4 PixelShaderFunction(PSInput input) : COLOR0
{
	float2 coord = input.Texcoord.xy;
	float4 origColor = tex2D(uImage0, coord);
	float value = origColor.r;
	
	return tex2D(uImage1, float2(clamp(value, 0.01, 0.99), 0.5)) * input.Color;
}
technique Technique1
{
    pass Test
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}

sampler uImage0 : register(s0);
texture2D tex0;
sampler2D uShapeTex = sampler_state
{
    Texture = <tex0>;
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Clamp;
    AddressV = Wrap;
};
float4x4 uTransform;
float uTime;
float Str;

float4 PixelShaderFunction(float4 drawColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    return float4(0, 0, 0, 0);
}

technique Technique1
{
	pass Test
	{
		PixelShader = compile ps_3_0 PixelShaderFunction();
	}
}

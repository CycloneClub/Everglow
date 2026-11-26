sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float4x4 uTransform;
float uTime;

float4 PixelShaderFunction(float4 drawColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 BackG = tex2D(uImage0, float2(coords.x + uTime , coords.y));
	return BackG;
}
float4 PixelShaderFunction2(float4 drawColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 BackG = tex2D(uImage0, float2(coords.x + uTime, coords.y));
    if (BackG.r > 0)
    {
        return float4(0.5, 0.35, 0.1, 0.6);
    }
    else
    {
        return float4(0, 0, 0, 0);
    }
}
float4 PixelShaderFunction3(float4 drawColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 BackG = tex2D(uImage0, float2(coords.x + uTime, coords.y));
    if (BackG.r > 0)
    {
        return tex2D(uImage1, float2(coords.x + uTime * 4, coords.y)) * drawColor * BackG.r;
    }
    else
    {
        return float4(0, 0, 0, 0);
    }
}

technique Technique1
{
	pass Test
	{
		PixelShader = compile ps_3_0 PixelShaderFunction();
	}
    pass Test2
    {
        PixelShader = compile ps_3_0 PixelShaderFunction2();
    }
    pass Test3
    {
        PixelShader = compile ps_3_0 PixelShaderFunction3();
    }
}

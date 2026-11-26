sampler uImage0 : register(s0);

float2 uScreenResolution;
float m;
float uIntensity;
float uRange;
float gauss[7] = { 0.02973, 0.10378, 0.21971, 0.28211, 0.21971, 0.10378, 0.02973};

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0//提取明度超过阈值m的部分
{
    float4 c = tex2D(uImage0,coords);
    if (max(max(c.r, c.g),c.b)>m)
        return c;
    else
        return float4(0,0,0,0);
}
float4 GlurH(float2 coords : TEXCOORD0) : COLOR0//水平方向模糊
{
    float4 color = float4(0, 0, 0, 1);
    float dx = uRange / uScreenResolution.x;
    
    color = float4(0, 0, 0, 1);
    
    for (int i = -3; i <= 3; i++)
    {
        color.rgb += gauss[i + 3] * tex2D(uImage0, float2(coords.x + dx * i, coords.y)).rgb;
    }
    return color*uIntensity;
}
float4 GlurV(float2 coords : TEXCOORD0) : COLOR0//竖直方向模糊
{
    float4 color = float4(0, 0, 0, 1);
    float dy = uRange / uScreenResolution.y;
    for (int i = -3; i <= 3; i++)
    {
        color.rgb += gauss[i + 3] * tex2D(uImage0, float2(coords.x, coords.y + dy * i)).rgb;
    }
    return color * uIntensity;
}
technique Technique1
{
	pass Bloom
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
    pass GlurH
    {
        PixelShader = compile ps_2_0 GlurH();
    }
    pass GlurV
    {
        PixelShader = compile ps_2_0 GlurV();
    }
}
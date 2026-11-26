sampler uImage0 : register(s0);

float _BlurOffset;
float2 _Center;

float4 Radial(float2 uv, float blurOffset, float2 center)
{
    float2 dir = -blurOffset * (center - uv);
    float4 color = 0;
    for (int j = 0; j < 10; j++)
    {
        color += tex2D(uImage0, uv);
        uv += dir;
    }
    return color / 10;
   
}

float4 PS_BlendBloom(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = Radial(coords, _BlurOffset, _Center);
    return color;
}


technique Technique1
{
    pass Blend
    {
        PixelShader = compile ps_3_0 PS_BlendBloom();
    }
}

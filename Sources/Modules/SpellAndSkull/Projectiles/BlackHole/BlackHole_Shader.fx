sampler uImage0 : register(s0);

float2 uPosition;
float uRatio;// width/height

float uRadius;
float uIntensity;

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float coord = coords;
    float2 pos = uPosition;
    float2 offset = (coords - pos);
    float dis = length(float2(offset.x * uRatio, offset.y));
    
    float scale = 1;
    if (dis<uRadius)
    {
        float a = (uRadius - dis)/uRadius;
        scale -= uIntensity * a;
        if (scale<-1)
        {
            float a = -(scale + 1);
            a = pow(a,0.75f);
            scale = -1 - a;

        }

    }

    return tex2D(uImage0,pos+offset*scale);
}
technique Technique1
{
	pass BlackHole
	{
		PixelShader = compile ps_3_0 PixelShaderFunction();
	}
}
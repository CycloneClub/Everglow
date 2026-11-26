sampler uImage0 : register(s0);
texture2D tex0;
sampler2D uShapeTex = sampler_state
{
    Texture = <tex0>;
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};
float4x4 uTransform;
float uTime;
float Str;

float4 PixelShaderFunction(float4 drawColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 BackG = tex2D(uImage0, float2(coords.x, coords.y));
    float light = max(max(BackG.r, BackG.g),BackG.b);
    if(!any(BackG))
         return float4(0, 0, 0, 0);
	return tex2D(uShapeTex, float2(0.5, uTime + coords.y * Str)) * light;
}

technique Technique1
{
	pass Test
	{
		PixelShader = compile ps_3_0 PixelShaderFunction();
	}
}

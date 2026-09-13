sampler2D uImage0 : register(s0);

float4x4 uTransform;

struct VSInput
{
    float2 Pos : POSITION0;
    float4 Color : COLOR0;
    float2 Texcoord : TEXCOORD0;
};

struct PSInput
{
    float4 Pos : SV_POSITION;
    float4 Color : COLOR0;
    float2 Texcoord : TEXCOORD0;
};

PSInput VertexShaderFunction(VSInput input)
{
    PSInput output;
    output.Color = input.Color;
    output.Texcoord = input.Texcoord;
    output.Pos = mul(float4(input.Pos, 0, 1), uTransform);
    return output;
}

float4 PixelShaderFunction(PSInput input) : COLOR0
{
    float4 tex = tex2D(uImage0, input.Texcoord);
    // 速度存储在 RGB, alpha 只当作遮罩: 用纹理亮度作为混合权重,
    // 让绘制的速度颜色按笔刷形状插值, 同时保留 RT 的 0.5 背景。
    float4 col = input.Color;
    col.a = tex.r * input.Color.a;
    return col;
}

technique Technique1
{
    pass Shader2D
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}

sampler2D uImage : register(s0);
sampler2D uNormalMap : register(s1);
float4x4 uTransform;
float4x4 uScreenToWorld;
float4x4 uWorldToScreen;
float4x4 uSurfaceRotation;
float2 uSize;
float2 uBlurStep;
float uWeights[9];
float uNormalStrength;
float uNormalPeriod;

struct VSInput
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
	float2 UV : TEXCOORD0;
};

struct PSInput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 UV : TEXCOORD0;
};

PSInput VertexShaderFunction(VSInput input)
{
	PSInput output;
	output.Position = mul(input.Position, uTransform);
	output.Color = input.Color;
	output.UV = input.UV;
	return output;
}

float4 SampleTransparent(float2 uv)
{
	float inside = step(0, uv.x) * step(uv.x, 1) * step(0, uv.y) * step(uv.y, 1);
	return tex2D(uImage, uv) * inside;
}

float4 Distort(PSInput input) : COLOR0
{
	float2 world = mul(float4(input.UV * uSize, 0, 1), uScreenToWorld).xy;
	float3 normal = normalize(tex2D(uNormalMap, world / uNormalPeriod).rgb * 2 - 1);
	float3 tangent = float3(normal.xy / max(normal.z, 0.25), 0);
	float2 displacement = mul(float4(tangent, 0), uSurfaceRotation).xy * uNormalStrength;
	displacement = mul(float4(displacement, 0, 0), uWorldToScreen).xy;
	return SampleTransparent(input.UV + displacement / uSize) * input.Color;
}

float4 Gaussian(PSInput input) : COLOR0
{
	// 两次可分离采样构成 9×9 高斯矩阵；RGBA 一起卷积，保留预乘透明度。
	float4 color = SampleTransparent(input.UV - 4 * uBlurStep) * uWeights[0];
	color += SampleTransparent(input.UV - 3 * uBlurStep) * uWeights[1];
	color += SampleTransparent(input.UV - 2 * uBlurStep) * uWeights[2];
	color += SampleTransparent(input.UV - uBlurStep) * uWeights[3];
	color += SampleTransparent(input.UV) * uWeights[4];
	color += SampleTransparent(input.UV + uBlurStep) * uWeights[5];
	color += SampleTransparent(input.UV + 2 * uBlurStep) * uWeights[6];
	color += SampleTransparent(input.UV + 3 * uBlurStep) * uWeights[7];
	color += SampleTransparent(input.UV + 4 * uBlurStep) * uWeights[8];
	return color * input.Color;
}

float4 GaussianLow(PSInput input) : COLOR0
{
	float4 color = SampleTransparent(input.UV - uBlurStep) * uWeights[0];
	color += SampleTransparent(input.UV) * uWeights[1];
	color += SampleTransparent(input.UV + uBlurStep) * uWeights[2];
	return color * input.Color;
}

float4 GaussianMedium(PSInput input) : COLOR0
{
	float4 color = SampleTransparent(input.UV - 2 * uBlurStep) * uWeights[0];
	color += SampleTransparent(input.UV - uBlurStep) * uWeights[1];
	color += SampleTransparent(input.UV) * uWeights[2];
	color += SampleTransparent(input.UV + uBlurStep) * uWeights[3];
	color += SampleTransparent(input.UV + 2 * uBlurStep) * uWeights[4];
	return color * input.Color;
}

technique NormalDistortion
{
	pass Pass0
	{
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 Distort();
	}
}

technique GaussianBlur
{
	pass Pass0
	{
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 Gaussian();
	}
}

technique GaussianBlurLow
{
	pass Pass0
	{
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 GaussianLow();
	}
}

technique GaussianBlurMedium
{
	pass Pass0
	{
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 GaussianMedium();
	}
}

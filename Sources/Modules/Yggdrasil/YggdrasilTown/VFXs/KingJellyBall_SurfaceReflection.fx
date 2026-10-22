sampler uScreenBuffer : register(s0);

float3 uFresnelF0;
float3 uKs;
float2 uViewportSize;
float4x4 uModel;
float4x4 uViewProj;
float4x4 uScreenTransform;
float2 uSurfaceCenter;
float uSurfaceScale;
float uTime;
float uRippleStrength;

struct VSInput
{
	float3 Pos : POSITION0;
	float4 Color : COLOR0;
	float3 Texcoord : TEXCOORD0;
};

struct PSInput
{
	float4 Pos : SV_POSITION;
	float4 Color : COLOR0;
	float2 SurfacePosition : TEXCOORD0;
};

PSInput VertexShaderFunction(VSInput input)
{
	PSInput output;
	float4 position = mul(float4(input.Pos, 1), uModel);
	output.Pos = mul(position, uViewProj);
	output.SurfacePosition = position.xy;
	output.Color = input.Color;
	return output;
}

float4 PixelShaderFunction(PSInput input) : COLOR0
{
	float scale = max(uSurfaceScale, 0.01);
	float2 localPosition = input.SurfacePosition - uSurfaceCenter;
	float2 radii = float2(260, 280) * scale;
	float2 surface = localPosition / radii;
	float radiusSquared = dot(surface, surface);
	if (radiusSquared >= 1)
		return 0;

	// A convex ellipsoidal cap facing the camera. The source plane is z = 0;
	// the cap is closest at its center. Depth and normal describe the same surface.
	float cap = sqrt(max(1 - radiusSquared, 0.0025));
	float depth = scale * (12 + 70 * (1 - cap));
	float2 gradient = 70 * scale * surface / (radii * cap);

	// Smooth traveling waves perturb both height and its analytic derivatives.
	// Coordinates scale with the body, so shrinking cannot amplify the distortion.
	float2 wavePosition = localPosition / scale;
	float2 frequency0 = float2(0.035, 0.022);
	float2 frequency1 = float2(-0.018, 0.041);
	float phase0 = dot(wavePosition, frequency0) - uTime * 1.8;
	float phase1 = dot(wavePosition, frequency1) + uTime * 1.3;
	depth += scale * uRippleStrength * (0.65 * sin(phase0) + 0.35 * sin(phase1));
	gradient += uRippleStrength * (0.65 * cos(phase0) * frequency0 + 0.35 * cos(phase1) * frequency1);

	// All positions and derivatives use screen axes: positive Y points down.
	float3 normal = normalize(float3(gradient, 1));
	float3 reflected = reflect(float3(0, 0, -1), normal);
	float visibility = smoothstep(0.12, 0.3, reflected.z);
	float2 sourcePosition = input.SurfacePosition + depth * reflected.xy / max(reflected.z, 0.12);

	// Keep the captured image in screen space; transform the intersection once.
	float2 screenPosition = mul(float4(sourcePosition, 0, 1), uScreenTransform).xy;
	float2 edgeDistance = min(screenPosition, uViewportSize - screenPosition);
	visibility *= smoothstep(0, 8, min(edgeDistance.x, edgeDistance.y));
	float2 uv = clamp(screenPosition / uViewportSize, 0.5 / uViewportSize, 1 - 0.5 / uViewportSize);

	float3 fresnel = uFresnelF0 + (1 - uFresnelF0) * pow(saturate(1 - normal.z), 5);
	float3 scene = pow(max(tex2D(uScreenBuffer, uv).rgb, 0), 2.2);
	float3 color = pow(max(scene * uKs * fresnel, 0), 1 / 2.2);
	return float4(color, 1) * input.Color * visibility;
}

technique Technique1
{
	pass Test
	{
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 PixelShaderFunction();
	}
}

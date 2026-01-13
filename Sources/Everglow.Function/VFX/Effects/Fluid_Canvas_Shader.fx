sampler2D uImage0 : register(s0);
sampler2D uImage1 : register(s1);

float4x4 uTransform;
float uResolutionX;
float uResolutionY;
float moveStep;
float viscosity;
float kinematic;
float timeStep;
float VORTICITY_AMOUNT;
float offset_small_RT2D;

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

float2 point1(float t)
{
	t *= 0.62;
	return float2(0.12, 0.5 + sin(t) * 0.2);
}
float2 point2(float t)
{
	t *= 0.62;
	return float2(0.88, 0.5 + cos(t + 1.5708) * 0.2);
}

float mag2(float2 p)
{
	return dot(p, p);
}

float4 PixelShaderFunction_Fluid(PSInput input) : COLOR0
{
	float dx = 1.0 / uResolutionX;
	float dy = 1.0 / uResolutionY;
	float dt = timeStep;
	float moveScale = moveStep;
	
	float4 mid = (tex2D(uImage0, input.Texcoord) - float4(0.5, 0.5, 0.5, 0.5)) * moveScale;
	float4 up = (tex2D(uImage0, input.Texcoord - float2(0, dy)) - float4(0.5, 0.5, 0.5, 0.5)) * moveScale;
	float4 down = (tex2D(uImage0, input.Texcoord + float2(0, dy)) - float4(0.5, 0.5, 0.5, 0.5)) * moveScale;
	float4 left = (tex2D(uImage0, input.Texcoord - float2(dx, 0)) - float4(0.5, 0.5, 0.5, 0.5)) * moveScale;
	float4 right = (tex2D(uImage0, input.Texcoord + float2(dx, 0)) - float4(0.5, 0.5, 0.5, 0.5)) * moveScale;
	
	// Gradient
	float3 gx = (right.xyz - left.xyz) * 0.5;
	float3 gy = (up.xyz - down.xyz) * 0.5;
	float2 gDensity = float2(gx.z, gy.z); //density
	
	mid.z -= dt * dot(float3(gDensity.xy, gx.x + gy.y), mid.xyz);
	
	float2 laplacian = up.xy + down.xy + left.xy + right.xy - mid.xy * 4;
	float2 viscForce = viscosity * laplacian;

	mid.xyw = (tex2D(uImage0, input.Texcoord - mid.xy * float2(dx, dy) * dt).xyw - float3(0.5, 0.5, 0.5)) * moveScale;
	
	float2 newForce = float2(0, 0);
	//newForce.xy += 0.75 * uMouseVel / (mag2(input.Texcoord - uMousePos) + 0.0001);
	//newForce.xy -= 0.75 * float2(0.0003, 0.00015) / (mag2(input.Texcoord - point2(uTime)) + 0.0001);

	mid.xy += dt * (viscForce - kinematic / dt * gDensity + newForce);
	mid.xy = max(float2(0, 0), abs(mid.xy) - 1e-4) * sign(mid.xy); //decay
	
	mid.w = (right.y - left.y - up.x + down.x);
	float2 vort = float2(abs(up.w) - abs(down.w), abs(left.w) - abs(right.w));
	vort *= VORTICITY_AMOUNT / length(vort + 1e-9) * mid.w;
	mid.xy += vort;

	float halfStep = moveScale / 2;
	mid.y *= smoothstep(0.5, 0.48, abs(input.Texcoord.y - 0.5)); //Boundaries
	mid.x *= smoothstep(0.5, 0.48, abs(input.Texcoord.x - 0.5)); //Boundaries
	mid = clamp(mid, float4(-halfStep, -halfStep, halfStep * 0.05, -halfStep), float4(halfStep, halfStep, halfStep * 0.3, halfStep));
	
	float4 col = mid / moveScale + float4(0.5, 0.5, 0.5, 0.5);
	return col;
}

float4 PixelShaderFunction_Fade(PSInput input) : COLOR0
{
	float dx = 1.0 / uResolutionX;
	float dy = 1.0 / uResolutionY;
	float dt = timeStep;
	float moveScale = moveStep;
	
	float4 vel = (tex2D(uImage1, input.Texcoord) - float4(0.5, 0.5, 0.5, 0.5)) * moveScale;
	float4 col = tex2D(uImage0, input.Texcoord - vel.xy * float2(dx, dy) * dt * 3);
	
	col = clamp(col, 0, 5);
	col = max(col - (0.0001 + col * 0.004) * 0.5, 0); //decay
	return col;
}

float4 PixelShaderFunction_Push(PSInput input) : COLOR0
{
	float2 move = input.Texcoord - float2(0.5, 0.5);
	float2 offsetTexcoord = float2(0.5, 0.5) + move * float2(uResolutionX / (uResolutionX - offset_small_RT2D * 2), uResolutionY / (uResolutionY - offset_small_RT2D * 2));
	float4 vel = (tex2D(uImage1, offsetTexcoord) - float4(0.5, 0.5, 0.5, 0.5));
	float4 col = tex2D(uImage0, input.Texcoord);
	col.xy += vel.xy;
	return col;
}



technique Technique1
{
    pass Fluid
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction_Fluid();
    }

	pass Fade
	{
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 PixelShaderFunction_Fade();
	}

	pass Push
	{
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 PixelShaderFunction_Push();
	}
}
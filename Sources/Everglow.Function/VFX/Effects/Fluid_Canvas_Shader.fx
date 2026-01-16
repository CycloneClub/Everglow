sampler2D uImage0 : register(s0);
sampler2D uImage1 : register(s1);

float4x4 uTransform;
float uResolutionX;
float uResolutionY;
float moveStep;
float viscosity;
float kinematic;
float timeStep;
float rho;
float VORTICITY_AMOUNT;
float offset_small_RT2D;
float successive_over_relation_value;
float pressure_move_value;
float velocity_apply_to_pressure_value;

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
	
	float4 finalVel = (tex2D(uImage0, input.Texcoord) - float4(0.5, 0.5, 0.5, 0.5)) * moveScale;
	float4 up = (tex2D(uImage0, input.Texcoord - float2(0, dy)) - float4(0.5, 0.5, 0.5, 0.5)) * moveScale;
	float4 down = (tex2D(uImage0, input.Texcoord + float2(0, dy)) - float4(0.5, 0.5, 0.5, 0.5)) * moveScale;
	float4 left = (tex2D(uImage0, input.Texcoord - float2(dx, 0)) - float4(0.5, 0.5, 0.5, 0.5)) * moveScale;
	float4 right = (tex2D(uImage0, input.Texcoord + float2(dx, 0)) - float4(0.5, 0.5, 0.5, 0.5)) * moveScale;
	
	// Gradient
	float3 gx = (right.xyz - left.xyz) * 0.5;
	float3 gy = (down.xyz - up.xyz) * 0.5;
	float2 gDensity = float2(gx.z, gy.z); //density
	
	finalVel.z -= dt * dot(float3(gDensity.xy, gx.x + gy.y), finalVel.xyz);
	
	float2 laplacian = up.xy + down.xy + left.xy + right.xy - finalVel.xy * 4;
	float2 viscForce = viscosity * laplacian;

	finalVel.xyw = (tex2D(uImage0, input.Texcoord - finalVel.xy * float2(dx, dy) * dt).xyw - float3(0.5, 0.5, 0.5)) * moveScale;
	
	float2 newForce = float2(0, 0);

	// viscosity
	finalVel.xy += dt * (viscForce - kinematic / dt * gDensity + newForce);
	finalVel.xy = max(float2(0, 0), abs(finalVel.xy) - 1e-3) * sign(finalVel.xy); //decay
	
	// vortex
	finalVel.w = (right.y - left.y - up.x + down.x);
	float2 vort = float2(abs(up.w) - abs(down.w), abs(left.w) - abs(right.w));
	vort *= VORTICITY_AMOUNT / length(vort + 1e-9) * finalVel.w;
	finalVel.xy += vort;

	float halfStep = moveScale / 2;
	finalVel.y *= smoothstep(0.5, 0.48, abs(input.Texcoord.y - 0.5)); //Boundaries
	finalVel.x *= smoothstep(0.5, 0.48, abs(input.Texcoord.x - 0.5)); //Boundaries
	finalVel = clamp(finalVel, float4(-halfStep, -halfStep, halfStep * 0.05, -halfStep), float4(halfStep, halfStep, halfStep * 0.3, halfStep));
	
	float4 col = finalVel / moveScale + float4(0.5, 0.5, 0.5, 0.5);
	return col;
}

float4 PixelShaderFunction_Fluid_Simple(PSInput input) : COLOR0
{
	float dx = 1.0 / uResolutionX;
	float dy = 1.0 / uResolutionY;
	float dt = timeStep;
	float moveScale = moveStep;
	
	float4 finalVel = (tex2D(uImage0, input.Texcoord) - float4(0.5, 0.5, 0.5, 0.5)) * moveScale;

	finalVel.xy = (tex2D(uImage0, input.Texcoord - finalVel.xy * float2(dx, dy) * dt).xy - float2(0.5, 0.5)) * moveScale;

	float halfStep = moveScale / 2;
	finalVel.y *= smoothstep(0.5, 0.48, abs(input.Texcoord.y - 0.5)); //Boundaries
	finalVel.x *= smoothstep(0.5, 0.48, abs(input.Texcoord.x - 0.5)); //Boundaries
	finalVel = clamp(finalVel, float4(-halfStep, -halfStep, halfStep * 0.05, -halfStep), float4(halfStep, halfStep, halfStep * 0.3, halfStep));
	
	float4 col = finalVel / moveScale + float4(0.5, 0.5, 0.5, 0.5);
	return col;
}

float SolveDivergence(sampler2D tex, float2 coord)
{
	float dx = 1.0 / uResolutionX;
	float dy = 1.0 / uResolutionY;

	float up = tex2D(tex, coord - float2(0, dy)).y - 0.5;
	float down = tex2D(tex, coord + float2(0, dy)).y - 0.5;
	float left = tex2D(tex, coord - float2(dx, 0)).x - 0.5;
	float right = tex2D(tex, coord + float2(dx, 0)).x - 0.5;
	
	up *= dx / dy;
	down *= dx / dy;
	
	float divergence = (right - left + down - up) * 0.5 * velocity_apply_to_pressure_value;
	return divergence;
}

float4 PixelShaderFunction_JacobiPressure(PSInput input) : COLOR0
{
	float dx = 1.0 / uResolutionX;
	float dy = 1.0 / uResolutionY;
	//float delta2 = dx * dx;
	
	float finalVel = tex2D(uImage0, input.Texcoord).x - 0.5;
	float up = tex2D(uImage0, input.Texcoord - float2(0, dy)).x - 0.5;
	float down = tex2D(uImage0, input.Texcoord + float2(0, dy)).x - 0.5;
	float left = tex2D(uImage0, input.Texcoord - float2(dx, 0)).x - 0.5;
	float right = tex2D(uImage0, input.Texcoord + float2(dx, 0)).x - 0.5;
	
	float divergence = SolveDivergence(uImage1, input.Texcoord);
	float pressure = (left + right + up + down - divergence) * 0.25;
	
	pressure = finalVel + (pressure - finalVel) * successive_over_relation_value;
	float4 col = float4(pressure + 0.5, 0, 0, 1);
	return col;
}

float4 PixelShaderFunction_ApplyPressure(PSInput input) : COLOR0
{
	float dx = 1.0 / uResolutionX;
	float dy = 1.0 / uResolutionY;
	float dt = timeStep / rho;
	
	float up = tex2D(uImage0, input.Texcoord - float2(0, dy)).x;
	float down = tex2D(uImage0, input.Texcoord + float2(0, dy)).x;
	float left = tex2D(uImage0, input.Texcoord - float2(dx, 0)).x;
	float right = tex2D(uImage0, input.Texcoord + float2(dx, 0)).x;
	
	float gradX = (right - left) * 0.5;
	float gradY = (down - up) * 0.5;

	float4 vel = (tex2D(uImage1, input.Texcoord) - float4(0.5, 0.5, 0.5, 0.5));
	float2 finalVel = vel.xy - dt * float2(gradX, gradY) * pressure_move_value;
	float halfStep = 0.5;
	finalVel = clamp(finalVel, float2(-halfStep, -halfStep), float2(halfStep, halfStep));
	return float4(finalVel, vel.zw) + float4(0.5, 0.5, 0.5, 0.5);
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

	pass Fluid_Simple
	{
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 PixelShaderFunction_Fluid_Simple();
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

	pass Jacobi
	{
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 PixelShaderFunction_JacobiPressure();
	}

	pass ApplyPressure
	{
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 PixelShaderFunction_ApplyPressure();
	}
}
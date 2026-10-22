sampler2D uImage0 : register(s0);
sampler2D uImage1 : register(s1);
sampler2D uImage2 : register(s2);
float4x4 uTransform;
const int layers = 16;
const float blur = 0.1;
const float speed = 4;
const float peaks = 4;
const float peakStrength = 0.3;
const float ringSpeed = 1.5;
const float smoke = 0.4;
const float smokeTime = 40;
//int layers;
//float blur;
//float speed;
//float peaks;
//float peakStrength;
//float ringSpeed;
//float smoke;
//float smokeTime;

struct VSInput
{
	float2 Pos : POSITION0;
	float4 Color : COLOR0;
	float3 Texcoord : TEXCOORD0;
};

struct PSInput
{
	float4 Pos : SV_POSITION;
	float4 Color : COLOR0;
	float3 Texcoord : TEXCOORD0;
};

PSInput VertexShaderFunction(VSInput input)
{
	PSInput output;
	output.Color = input.Color;
	output.Texcoord = input.Texcoord;
	output.Pos = mul(float4(input.Pos, 0, 1), uTransform);
	return output;
}

float hash(float seed)
{
	return fmod(sin(seed * 3602.64325) * 51.63891, 1);
}

float circle(float radius, float2 pos)
{
	return radius - pos.x;
}

float4 PixelShaderFunction(PSInput input) : COLOR0
{
	float time = input.Texcoord.z;
    
	float2 uv = input.Texcoord.xy;
	uv = uv * 2 - 1;
	float2 puv = float2(length(uv), atan2(uv.y, uv.x)); //polar coordinates

	float3 col = tex2D(uImage0, input.Texcoord.xy);
	for (int i = 0; i < layers; i++)
	{
		float prog = float(i) / float(layers);
		float radius = prog * ((1 - 1 / pow(time * speed, 1 / 3)) * 2); //decrease radius using cubed
		radius += sin(puv.y * peaks + hash(prog) * 513) * peakStrength; //modulate radius so it isnt enitly symetrical
		float3 color = float3(
            0.1 + 0.3 / radius,
            0.5 * (2 - radius),
            0.7 / radius
        ) / time / abs(log(time * ringSpeed) - puv.x); // base explosion color, decrease with time and with distance from center
		float cValue = (1 - time / smokeTime) * puv.x * smoke;
		color += float3(cValue, cValue, cValue); //add smoke color, falloff can be controlled with smoketime variable
		col += color * smoothstep(0, 1, circle(radius, puv) / blur);
	}

	return float4(col / float(layers), 1.0) * input.Color;
}
technique Technique1
{
	pass ExplosionShader
	{
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 PixelShaderFunction();
	}
}

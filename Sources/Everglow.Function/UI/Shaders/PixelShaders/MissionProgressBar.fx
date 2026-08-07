Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

float2 uScaleFactor;
float uRadius;
float uThickness;
float uProgress;
float4 uColor;
bool uOpposite;
float2 uInterval;

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MakeProgressBar(VertexShaderOutput input) : COLOR
{
	float2 coord = input.TextureCoordinates;
	float2 center = float2(0.5, 0.5);
	float2 relativePosition = coord - center;
	float dis = distance(coord, center);
	float pi = 3.1415926;
	float sectorRadian = pi * 2 * uProgress;
	float coordRadius = uRadius * uScaleFactor.x;
	if (dis > (uRadius - uThickness) * uScaleFactor.x && dis <= uRadius * uScaleFactor.x)
	{
		return uColor;
	}
	if(uProgress == 0)
	{
		return float4(0, 0, 0, 0);
	}
	if(coord.x == center.x && coord.y == center.y)
	{
		return uColor;
	}
	
	float relativeRadian = atan2(relativePosition.y, relativePosition.x);
	if (relativeRadian <= -pi / 2 && relativeRadian > -pi)
	{
		relativeRadian = 2.5 * pi + relativeRadian;
	}
	else
	{
		relativeRadian = 0.5 * pi + relativeRadian;
	}

	if (dis < coordRadius)
	{
		if (uOpposite)
		{
			if (relativeRadian <= sectorRadian && relativeRadian >= 0)
			{
				return uColor;
			}
			else
			{
				return float4(0, 0, 0, 0);
			}
		}
		else
		{
			if (relativeRadian < sectorRadian && relativeRadian > 0)
			{
				return float4(0, 0, 0, 0);
			}
			else
			{
				return uColor;
			}
		}
	}
	return float4(0, 0, 0, 0);
}

technique SpriteDrawing
{
	pass MakeProgressBar
	{
		PixelShader = compile ps_3_0 MakeProgressBar();
	}
};

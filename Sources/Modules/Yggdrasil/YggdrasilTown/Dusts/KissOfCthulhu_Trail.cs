using Terraria;

namespace Everglow.Yggdrasil.YggdrasilTown.Dusts;

public class KissOfCthulhu_Trail : ModDust
{
	public override void OnSpawn(Dust dust)
	{
	}

	public override bool Update(Dust dust)
	{
		if (dust.customData is not Vector4)
		{
			dust.active = false;
			return false;
		}
		if (dust.scale < 0.1f)
		{
			dust.active = false;
		}
		var cus = (Vector4)dust.customData;
		cus.Z *= 0.95f;
		cus.W *= 0.95f;
		float speed = 2.5f;
		if (cus.X < 90f)
		{
			speed *= cus.X / 90f;
		}
		dust.velocity = new Vector2(0, speed).RotatedBy(cus.Y) * MathF.Sin(cus.X * 0.035f + (float)Main.timeForVisualEffects * 0.2f) + cus.ZW();
		dust.position += dust.velocity;

		dust.scale *= 0.87f;
		return false;
	}

	public float Noise(Dust dust)
	{
		float value = 0;
		for (int i = 0; i < 8; i++)
		{
			value += MathF.Sin(dust.position.X * 0.05f * MathF.Pow(2, i) + (float)Main.timeForVisualEffects * 0.02f * i % 3) / MathF.Pow(2, i);
			value -= MathF.Sin(dust.position.Y * 0.05f * MathF.Pow(2, i) - (float)Main.timeForVisualEffects * 0.03f * i % 5) / MathF.Pow(2, i);
		}
		return value;
	}

	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		return new Color(15, 0, 30, 255);
	}

	public Color GetColor(Dust dust)
	{
		return new Color(0, 0, 0, 255);
	}
}

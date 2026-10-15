namespace Everglow.Commons.VFX.CommonVFXDusts;

public class ElectricMiddleDust : ModDust
{
	public override string Texture => "Terraria/Images/Projectile_0";

	public override void OnSpawn(Dust dust)
	{
		dust.alpha = 0;
	}

	public override bool Update(Dust dust)
	{
		if (dust.alpha >= 1)
		{
			for (int g = 0; g < dust.scale * 70f; g++)
			{
				float size = Main.rand.NextFloat(3f, 12f) * dust.scale * 10f;
				Vector2 afterVelocity = new Vector2(0, size * dust.scale * 3).RotatedByRandom(MathHelper.TwoPi);
				var electric = new ElectricCurrentDust
				{
					Velocity = afterVelocity,
					Active = true,
					Visible = true,
					Position = dust.position + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
					MaxTime = Main.rand.NextFloat(35, 105) * dust.scale * 3,
					Scale = size,
					ai = new float[] { Main.rand.NextFloat(0.0f, 0.6f), size, Main.rand.NextFloat(0.1f, 0.12f) },
				};
				Ins.VFXManager.Add(electric);
			}
			dust.active = false;
		}
		dust.alpha++;
		return false;
	}

	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		return Color.Transparent;
	}
}

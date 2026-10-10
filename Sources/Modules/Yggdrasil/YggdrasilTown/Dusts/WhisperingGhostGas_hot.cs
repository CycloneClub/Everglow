namespace Everglow.Yggdrasil.YggdrasilTown.Dusts;

public class WhisperingGhostGas_hot : ModDust
{
	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();
	}

	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		float value = 255 - dust.alpha;
		value /= 255f;
		return new Color(value, value, value, value * 0.2f);
	}

	public override void OnSpawn(Dust dust)
	{
		dust.alpha = 0;
		base.OnSpawn(dust);
	}

	public override bool Update(Dust dust)
	{
		dust.rotation += 0.4f;
		dust.alpha += 5;
		if (dust.alpha > 254)
		{
			dust.active = false;
		}
		dust.position += dust.velocity;
		dust.velocity *= 0.9f;
		float value = 255 - dust.alpha;
		value /= 255f;
		Lighting.AddLight(dust.position + new Vector2(4), new Vector3(1f, 0, 0) * value);
		return false;
	}
}

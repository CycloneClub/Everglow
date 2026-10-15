namespace Everglow.Yggdrasil.YggdrasilTown.Dusts;

public class SkullCollectionDust : ModDust
{
	public override bool Update(Dust dust)
	{
		dust.noGravity = true;
		dust.velocity.Y -= 0.13f;
		dust.velocity.X = MathF.Sin((float)Main.time * 0.23f + dust.GetHashCode());
		Lighting.AddLight(dust.position + new Vector2(4), new Vector3(0.3f * dust.scale, 0.4f * dust.scale, 0.4f * dust.scale));
		return base.Update(dust);
	}

	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		return new Color(1f, 1f, 1f, 0) * 0.35f;
	}
}

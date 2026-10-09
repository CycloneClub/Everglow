using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Enemies;

public class LightSeed : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.EnemyProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 16;
		Projectile.height = 16;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.ignoreWater = false;
		Projectile.tileCollide = true;
		Projectile.timeLeft = 150;
	}
	private Vector2 Point = Vector2.Zero;
	private float x;
	private float k;
	public override void AI()
	{
		if (Projectile.timeLeft >= 140)
		{
			Projectile.scale = (150 - Projectile.timeLeft) / 10f;
		}
		Projectile.scale *= 0.985f;
		if (Projectile.scale > 0.7f)
		{
			Projectile.ai[0] *= 0.985f;
		}
		else
		{
			Projectile.scale *= 1.008f;
		}
		if (Projectile.timeLeft < 30)
		{
			Projectile.hostile = false;
			Projectile.scale *= 0.85f;
		}
		x += Projectile.ai[0];
		Projectile.Center = Point + new Vector2(x, MathF.Sin(MathF.Abs(x) / 30) * 1800 / (MathF.Abs(x) + 12) + k * x);
		Lighting.AddLight(Projectile.Center, 1.6f * Projectile.scale, 1.6f * Projectile.scale, 0);
	}
	public override void OnSpawn(IEntitySource source)
	{
		Projectile.ai[0] = Projectile.velocity.X;
		k = Projectile.velocity.Y / Projectile.velocity.X;
		Point = Projectile.Center;
		x = 0;
		Projectile.velocity = Vector2.Zero;
		Projectile.scale = 0;
	}
	public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
	{
		target.AddBuff(BuffID.Poisoned, 600);
	}
	public override void OnKill(int timeLeft)
	{
	}
	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D texture = ModAsset.LightSeed.Value;
		Texture2D textureD = ModAsset.LightSeed_dark.Value;
		Texture2D textureB = ModAsset.LightSeed_bloom.Value;
		Main.EntitySpriteDraw(textureD, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 1f), 0, textureD.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
		float oxideValue = (150 - Projectile.timeLeft) / 150f;
		oxideValue = MathF.Pow(oxideValue, 0.3f);
		Main.EntitySpriteDraw(textureB, Projectile.Center - Main.screenPosition, null, new Color(oxideValue, 1f, 0f, 0), 0, textureB.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
		Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, new Color(oxideValue * 3f + 0.3f, 1f, 0f, 0), 0, texture.Size() * 0.5f, Projectile.scale * Projectile.scale * 0.6f, SpriteEffects.None, 0);

		return false;
	}
}

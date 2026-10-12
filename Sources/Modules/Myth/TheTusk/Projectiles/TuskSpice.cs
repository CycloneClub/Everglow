using Terraria.DataStructures;

namespace Everglow.Myth.TheTusk.Projectiles;

public class TuskSpice : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.BossProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = true;
		Projectile.timeLeft = 400;
		Projectile.penetrate = -1;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
		ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[Type] = true;
	}

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.frame = Main.rand.Next(5);
		Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
		Projectile.ai[0] = Projectile.rotation;
		Projectile.ai[1] = Projectile.rotation;
		Projectile.ai[2] = Projectile.rotation;
	}

	public override void AI()
	{
		Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
		Projectile.ai[0] = (float)Utils.Lerp(Projectile.ai[0], Projectile.rotation, 0.5f);
		Projectile.ai[1] = (float)Utils.Lerp(Projectile.ai[1], Projectile.ai[0], 0.5f);
		Projectile.ai[2] = (float)Utils.Lerp(Projectile.ai[2], Projectile.ai[1], 0.5f);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D texture = ModAsset.TuskSpice.Value;
		for (int i = 0; i < Projectile.oldPos.Length; i++)
		{
			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle(Projectile.frame * 24, 0, 24, 50), lightColor, Projectile.ai[i], new Vector2(12, 25), Projectile.scale, SpriteEffects.None, 0);
		}
		Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle(Projectile.frame * 24, 0, 24, 50), lightColor, Projectile.rotation, new Vector2(12, 25), Projectile.scale, SpriteEffects.None, 0);
		return false;
	}

	public override void OnKill(int timeLeft)
	{
		for (int i = -5; i < 5; i++)
		{
			Vector2 pos = Projectile.Center + new Vector2(i * 6, 0).RotatedBy(Projectile.rotation + MathHelper.PiOver2) - new Vector2(0, 4);
			Dust dust = Dust.NewDustDirect(pos, 0, 0, ModContent.DustType<Dusts.TuskBreak_small>());
			dust.velocity = Projectile.velocity;
		}
	}
}

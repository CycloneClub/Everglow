using Everglow.Myth.Misc.Dusts;
using Terraria.DataStructures;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.Clubs;

public class MythrilClub_smashMagicBall : ModProjectile
{
	public override string Texture => "Everglow/" + ModAsset.MythrilClub_Path;

	public override void SetDefaults()
	{
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.aiStyle = -1;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
	}

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.scale = 0;
		base.OnSpawn(source);
	}

	public override bool PreAI()
	{
		if (Projectile.timeLeft > 120)
		{
			return false;
		}
		if (Projectile.timeLeft == 120)
		{
			Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<MythrilClub_Shoot>(), (int)(Projectile.damage * 0.4f), Projectile.knockBack * 0.4f, Projectile.owner, 1f);
		}
		return base.PreAI();
	}

	public override bool ShouldUpdatePosition()
	{
		if (Projectile.timeLeft > 120)
		{
			return false;
		}
		if (Projectile.timeLeft < 80)
		{
			return false;
		}
		return base.ShouldUpdatePosition();
	}

	public int RotatingTimer = 0;

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		Projectile.friendly = true;

		if (Projectile.timeLeft < 80)
		{
			Vector2 targetCenter = player.Center + new Vector2(-90 * player.direction, -75 * player.gravDir + MathF.Sin((float)Main.time * 0.03f) * 15) + new Vector2(0, 45).RotatedBy(Main.time * 0.13 + Projectile.whoAmI);
			Projectile.velocity *= 0f;
			Projectile.Center = Vector2.Lerp(Projectile.Center, targetCenter, 0.1f);
			RotatingTimer++;
			if (RotatingTimer < 11000)
			{
				Projectile.timeLeft = 75;
			}
			Projectile.Kill();
		}
		else
		{
			Dust d0 = Dust.NewDustDirect(Projectile.Center - new Vector2(4), 0, 0, ModContent.DustType<MythrilFlare>());
			d0.velocity *= 0.1f;

			Projectile.velocity += new Vector2(0, 0.15f);
			float value = 1 - Math.Abs(100 - Projectile.timeLeft) / 20f;
			Projectile.scale = MathF.Pow(value, 2.5f);

			d0.scale *= Projectile.scale;

			Lighting.AddLight(Projectile.Center, new Vector3(0.1f, 0.4f, 0.6f) * Projectile.scale);
		}
		base.AI();
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		if (Projectile.timeLeft > 120)
		{
			return false;
		}
		Texture2D texture = Commons.ModAsset.LightPoint.Value;
		Texture2D texturedark = Commons.ModAsset.Point_black.Value;
		Main.EntitySpriteDraw(texturedark, Projectile.Center - Main.screenPosition, null, Color.White * Math.Max((30 - RotatingTimer) / 30f, 0), 0, texturedark.Size() * 0.5f, Projectile.scale * 0.18f, SpriteEffects.None, 0);
		var color = new Color(0.1f, 0.7f, 0.6f, 0) * Projectile.scale;
		Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, color, 0, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
		Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, color, 0, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

		Texture2D light = Commons.ModAsset.StarSlash.Value;
		float dark = Math.Max((30 - RotatingTimer) / 30f, 0);
		color = new Color(0.0f, 0.4f, 0.3f, 0) * ((Projectile.scale - 0.5f) * 2);
		Main.EntitySpriteDraw(light, Projectile.Center - Main.screenPosition, null, color, 0 + Projectile.ai[1], light.Size() / 2f, new Vector2(1f, dark * dark) * 0.75f, SpriteEffects.None, 0);
		Main.EntitySpriteDraw(light, Projectile.Center - Main.screenPosition, null, color, MathHelper.PiOver2 + Projectile.ai[1], light.Size() / 2f, new Vector2(0.5f, dark * 2f) * 0.75f, SpriteEffects.None, 0);
		return false;
	}

	public override void OnKill(int timeLeft)
	{
		base.OnKill(timeLeft);
	}
}

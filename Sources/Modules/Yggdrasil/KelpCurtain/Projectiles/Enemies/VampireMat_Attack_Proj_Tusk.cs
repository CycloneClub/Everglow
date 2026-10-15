using Everglow.Yggdrasil.KelpCurtain.Gores;
using Everglow.Yggdrasil.KelpCurtain.NPCs.VampireMat;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Enemies;

public class VampireMat_Attack_Proj_Tusk : ModProjectile
{
	public int TargetPlayerIndex => (int)Projectile.ai[0];

	public int ParentNPCIndex => (int)Projectile.ai[1];

	public override string LocalizationCategory => LocalizationUtils.Categories.MagicProjectiles;

	public int Timer = 0;

	public override void SetDefaults()
	{
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 600;
		Projectile.penetrate = -1;
		Projectile.tileCollide = true;
		Projectile.ignoreWater = true;
		Projectile.friendly = false;
		Projectile.hostile = true;
	}

	public override void AI()
	{
		Projectile.rotation = Projectile.velocity.ToRotationSafe();
		Timer++;
	}

	public override void OnKill(int timeLeft)
	{
		for (int x = 0; x < 7; x++)
		{
			VampireMat_Attack_Proj_Tusk_Gore gore = new VampireMat_Attack_Proj_Tusk_Gore();
			gore.Position = Projectile.Center + Projectile.rotation.ToRotationVector2() * 10 * (x - 3);
			gore.Velocity = new Vector2(Main.rand.NextFloat(5), 0).RotatedByRandom(MathHelper.TwoPi);
			gore.Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
			gore.RotateSpeed = Main.rand.NextFloat(-0.1f, 0.1f);
			gore.Active = true;
			gore.Visible = true;
			switch (x)
			{
				case 0:
					gore.Frame = new Rectangle(0, 0, 32, 14);
					break;
				case 1:
					gore.Frame = new Rectangle(0, 16, 36, 18);
					break;
				case 2:
					gore.Frame = new Rectangle(0, 36, 28, 16);
					break;
				case 3:
					gore.Frame = new Rectangle(0, 54, 12, 10);
					break;
				case 4:
					gore.Frame = new Rectangle(20, 54, 14, 18);
					break;
				case 5:
					gore.Frame = new Rectangle(0, 66, 16, 10);
					break;
				case 6:
					gore.Frame = new Rectangle(0, 78, 26, 10);
					break;
			}
			Ins.VFXManager.Add(gore);
		}
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo info)
	{
		VampireMat.OnVampireMatHitPlayer(target, info.Damage);
		base.OnHitPlayer(target, info);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
		Texture2D bloom = ModAsset.VampireMat_Attack_Proj_Tusk_bloom.Value;
		Texture2D white = ModAsset.VampireMat_Attack_Proj_Tusk_White.Value;
		float drawScale = Projectile.scale;
		lightColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates());
		Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() * 0.5f, drawScale, SpriteEffects.None, 0);
		Main.EntitySpriteDraw(bloom, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 0) * (MathF.Sin(Projectile.timeLeft * 0.5f) * 0.35f + 1f) * 0.5f, Projectile.rotation, bloom.Size() * 0.5f, drawScale, SpriteEffects.None, 0);
		if (Timer < 6 || (Timer < 30 && Timer % 8 < 4))
		{
			Main.EntitySpriteDraw(white, Projectile.Center - Main.screenPosition, null, Color.Lerp(Color.White, lightColor, 0.75f), Projectile.rotation, white.Size() * 0.5f, drawScale, SpriteEffects.None, 0);
		}
		return false;
	}
}

using Everglow.Yggdrasil.YggdrasilTown.Dusts.TownNPCAttack;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.TownNPCs;

public class Schorl_Explosion : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.TownNPCProjectiles;

	public int Timer;

	public override void SetDefaults()
	{
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 60;
		Projectile.ArmorPenetration = 0;
		Projectile.friendly = true;
		Projectile.timeLeft = 120;
		Projectile.tileCollide = false;
		Projectile.penetrate = -1;
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.extraUpdates = 8;
		Projectile.aiStyle = -1;
		Projectile.hostileDamageScaling = Terraria.DataStructures.GameDifficultyData.NoDifficultyPlayerDamageScaling;
	}

	public override void OnSpawn(IEntitySource source)
	{
		for (int i = 0; i < 30; i++)
		{
			var dust = Dust.NewDustDirect(Projectile.Center, 0, 0, ModContent.DustType<Schorl_GoldenDust>());
			dust.position = Projectile.Center;
			dust.velocity = new Vector2(0, MathF.Sin(i / 30f * MathHelper.TwoPi * 5) + 3).RotatedBy(Projectile.velocity.ToRotationSafe() + i / 30f * MathHelper.TwoPi) * 0.6f;
		}
		for (int t = 0; t < 3; t++)
		{
			for (int i = 0; i < 80; i++)
			{
				var dust = Dust.NewDustDirect(Projectile.Center, 0, 0, ModContent.DustType<Schorl_GoldenDustNoDiffusion>());
				dust.position = Projectile.Center;
				var dir = new Vector2(0, 6).RotatedBy(i / 80f * MathHelper.TwoPi);
				dir.Y *= 0.5f + 0.35f * MathF.Sin(t * 2 + Projectile.whoAmI * 0.7f);
				dir = dir.RotatedBy(t * 2 + Projectile.whoAmI);
				dust.velocity = dir;
			}
		}
		for (int i = 0; i < 30; i++)
		{
			var sFlame = new Schorl_Flame
			{
				Velocity = new Vector2(0, Main.rand.NextFloat(0.3f, 2f)).RotatedByRandom(MathHelper.TwoPi),
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(0, MathF.Sqrt(Main.rand.NextFloat()) * 32).RotatedByRandom(MathHelper.TwoPi),
				MaxTime = Main.rand.Next(24, 36),
				Scale = Main.rand.Next(14, 26),
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				Frame = Main.rand.Next(3),
				ai = new float[] { Main.rand.NextFloat(-0.8f, 0.8f) },
			};
			Ins.VFXManager.Add(sFlame);
		}
	}

	public override void AI()
	{
	}

	public override bool ShouldUpdatePosition() => false;

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		return base.Colliding(projHitbox, targetHitbox);
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		float timeValue = (120 - Projectile.timeLeft) / 120f;
		float dark = MathF.Pow(timeValue, 0.5f);
		var c = new Color(2f * (1 - timeValue), 1.6f * (1 - timeValue) * (1 - timeValue), 0, 0f);
		Texture2D light = Commons.ModAsset.StarSlash.Value;
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, light.Width / 2, light.Height), c, MathF.Sin(Projectile.whoAmI - Projectile.position.X) * 6 + Projectile.ai[1], light.Size() / 2f, new Vector2(1f, dark * dark) * 2.62f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, light.Width / 2, light.Height), c, MathF.Sin(Projectile.whoAmI) * 6 + Projectile.ai[1], light.Size() / 2f, new Vector2(0.5f, dark) * 2.62f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, c, MathF.Sin(Projectile.whoAmI + Projectile.position.Y) * 6 + Projectile.ai[1], light.Size() / 2f, new Vector2(1f, dark * dark) * 2.62f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, light.Width / 2, light.Height), c, MathF.Sin(Projectile.type * 0.4f + Projectile.whoAmI) * 6 + Projectile.ai[1], light.Size() / 2f, new Vector2(1f, dark * dark) * 2.62f, SpriteEffects.None, 0);
		return false;
	}
}

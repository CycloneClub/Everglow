using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Everglow.Yggdrasil.YggdrasilTown.VFXs.NPCEffects;
using static Everglow.Yggdrasil.YggdrasilTown.VFXs.RandomNPC.VFXPerson;
using Microsoft.Xna.Framework;
using Terraria;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Enemies;

public class PurpleBombCaterpillarDeath : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.EnemyProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 88;
		Projectile.height = 36;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.ignoreWater = false;
		Projectile.tileCollide = true;
		Projectile.timeLeft = 90;
	}

	public override void AI()
	{
		if (Projectile.timeLeft == 5)
		{
			Projectile.hostile = true;
			KillEffect();
		}
		float value = (90 - Projectile.timeLeft) * 0.02f;
		Lighting.AddLight(Projectile.Center, new Vector3(1f, 0.05f, 0.6f) * value);
		Projectile.scale += value * 0.004f;
	}

	public void KillEffect()
	{
		for (int g = 0; g < 48; g++)
		{
			Vector2 vel = new Vector2(0, Main.rand.NextFloat(12, 48)).RotatedByRandom(MathHelper.TwoPi);
			float dropScale = Main.rand.NextFloat(6f, 18f);
			var blood = new PurpleBombCaterpillarBloodDrop
			{
				Velocity = vel / MathF.Sqrt(dropScale),
				Active = true,
				Visible = true,
				Position = Projectile.position + new Vector2(Main.rand.NextFloat(0, Projectile.width), Main.rand.NextFloat(0, Projectile.height)) + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
				MaxTime = Main.rand.Next(42, 84),
				Scale = dropScale,
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { 0f, Main.rand.NextFloat(0.0f, 4.93f) },
			};
			Ins.VFXManager.Add(blood);
		}
		for (int g = 0; g < 12; g++)
		{
			Vector2 vel = new Vector2(0, MathF.Sqrt(Main.rand.NextFloat()) * 12f).RotatedByRandom(MathHelper.TwoPi);
			var somg = new PurpleBombCaterpillarSmog
			{
				Velocity = vel,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
				MaxTime = Main.rand.Next(60, 75),
				Scale = Main.rand.NextFloat(50f, 155f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
		for (int j = 0; j < 40; j++)
		{
			Vector2 newVelocity = new Vector2(0, 0.7f).RotatedBy(j / 20f * MathHelper.TwoPi);
			newVelocity.X *= 0.6f * MathF.Sin(j / 15f * MathHelper.TwoPi) + 0.2f;
			newVelocity = newVelocity.RotatedBy(-Main.time * 0.05f + Projectile.whoAmI + j * MathHelper.TwoPi / 3f);
			newVelocity.X *= 2f;
			var somg = new LightFruitParticleDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.position + new Vector2(Main.rand.NextFloat(0, Projectile.width), Main.rand.NextFloat(0, Projectile.height)) + newVelocity * 20,
				MaxTime = Main.rand.Next(40, 80),
				Scale = 1,
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(3, 12f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
	{
		target.AddBuff(BuffID.Poisoned, 600);
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		if (Projectile.timeLeft < 5)
		{
			return false;
		}
		Texture2D eyeStalks = ModAsset.PurpleBombCaterpillar.Value;
		Texture2D eyeStalks_glow = ModAsset.PurpleBombCaterpillar_glow.Value;
		Texture2D body = ModAsset.PurpleBombCaterpillar_Body.Value;
		Texture2D body_glow = ModAsset.PurpleBombCaterpillar_Body_glow.Value;
		Texture2D body_shape = ModAsset.PurpleBombCaterpillar_Body_Shape.Value;
		SpriteEffects flip = Projectile.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

		float squzzeValue = 1f;
		float originX = 0.5f;

		float value = (90 - Projectile.timeLeft) * 0.002f;
		float explosionValue = Projectile.timeLeft;
		explosionValue = MathF.Pow(explosionValue, -0.25f) * 150f;
		explosionValue = MathF.Sin(explosionValue * 0.6f) + 0.5f;
		float explosionValue2 = (90 - Projectile.timeLeft) / 90f;
		lightColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates());
		Main.EntitySpriteDraw(eyeStalks, Projectile.Bottom - Main.screenPosition, new Rectangle(0, 0, 126, 54), lightColor, Projectile.rotation, new Vector2(126, 54) * new Vector2(0.5f, 0.95f), Projectile.scale, flip, 0);
		Main.EntitySpriteDraw(eyeStalks_glow, Projectile.Bottom - Main.screenPosition, new Rectangle(0, 0, 126, 54), new Color(1f, 0.65f, 0.25f, 0), Projectile.rotation, new Vector2(126, 54) * new Vector2(0.5f, 0.95f), Projectile.scale, flip, 0);

		Main.EntitySpriteDraw(body, Projectile.Bottom - Main.screenPosition, null, lightColor, Projectile.rotation, body.Size() * new Vector2(originX, 0.95f), new Vector2(squzzeValue, 2 - squzzeValue + value) * Projectile.scale, flip, 0);
		Main.EntitySpriteDraw(body_glow, Projectile.Bottom - Main.screenPosition, null, new Color(1f, 0.65f, 0.25f, 0), Projectile.rotation, body.Size() * new Vector2(originX, 0.95f), new Vector2(squzzeValue, 2 - squzzeValue + value) * Projectile.scale, flip, 0);
		Main.EntitySpriteDraw(body_shape, Projectile.Bottom - Main.screenPosition, null, Color.White * explosionValue * explosionValue2, Projectile.rotation, body.Size() * new Vector2(originX, 0.95f), new Vector2(squzzeValue, 2 - squzzeValue + value) * Projectile.scale, flip, 0);
		return false;
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		return MathUtils.IntersectsCircleAABB(Projectile.Center, 100, targetHitbox);
	}
}

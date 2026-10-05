using Everglow.Yggdrasil.YggdrasilTown.Buffs;
using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Summon;

public class DeadBeetleEgg_egg : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.SummonProjectiles;

	private Player Owner => Main.player[Projectile.owner];

	public override void SetDefaults()
	{
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.width = 40;
		Projectile.height = 40;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 60;
		Projectile.aiStyle = -1;
		Projectile.DamageType = DamageClass.Summon;
	}

	public int ManaValue = 0;

	public override void OnSpawn(IEntitySource source)
	{
		base.OnSpawn(source);
	}

	public override void AI()
	{
		if (Projectile.frame < 5)
		{
			if (Projectile.timeLeft % 6 == 0)
			{
				Projectile.frame++;
				if (Projectile.frame == 5)
				{
					Crack();
				}
			}
		}
		else
		{
			Projectile.frame = 5;
		}
		Owner.heldProj = Projectile.whoAmI;
		Projectile.Center = Owner.Center + new Vector2(Owner.direction * 15f, 0);
	}

	public void Crack()
	{
		Vector2 addVel = Owner.velocity;
		Owner.AddBuff(ModContent.BuffType<DeadBeetleEggBuff>(), 1800000);
		Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(Owner.direction * 4, -6) + addVel, ModContent.ProjectileType<DeadBeetleEgg_beetle>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
		for (int i = 0; i < 10; i++)
		{
			var dust = Dust.NewDustDirect(Projectile.Center - new Vector2(4), 16, 8, ModContent.DustType<DeadBeetleEgg_FragileShell>());
			dust.velocity = new Vector2(0, -Main.rand.NextFloat(1, 6)).RotatedBy(Main.rand.NextFloat(-1.2f, 1.2f)) + addVel;
			dust.scale = Main.rand.NextFloat(0.7f, 1.5f);
		}
		for (int g = 0; g < 5; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 4f)).RotatedByRandom(MathHelper.TwoPi) + new Vector2(0, -2f) + addVel;
			var somg = new AirFlameSmogDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
				MaxTime = Main.rand.Next(37, 45),
				Scale = Main.rand.NextFloat(40f, 55f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
		for (int g = 0; g < 20; g++)
		{
			Vector2 newVelocity = new Vector2(0, -Main.rand.NextFloat(1, 6)).RotatedBy(Main.rand.NextFloat(-1.2f, 1.2f)) + addVel;
			var spark = new Spark_MoonBladeDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
				MaxTime = Main.rand.Next(70, 125),
				Scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(9f, 17.0f)),
				Rotation = Main.rand.NextFloat(6.283f),
				noGravity = true,
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.03f, 0.03f) },
			};
			Ins.VFXManager.Add(spark);
		}
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D texture = ModAsset.DeadBeetleEgg_egg.Value;
		Texture2D texture_glow = ModAsset.DeadBeetleEgg_egg_glow.Value;
		var frame = new Rectangle(0, Projectile.frame * 40, 44, 40);
		Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, frame, lightColor, 0, frame.Size() * 0.5f, Projectile.scale, Owner.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
		Main.spriteBatch.Draw(texture_glow, Projectile.Center - Main.screenPosition, frame, new Color(1f, 1f, 1f, 0), 0, frame.Size() * 0.5f, Projectile.scale, Owner.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);

		float newTimer = 40 - Projectile.timeLeft;
		if (newTimer > 0 && newTimer < 20)
		{
			Texture2D star = Commons.ModAsset.StarSlash.Value;
			float sinValue = MathF.Sin(newTimer / 20f * MathHelper.Pi);
			Vector2 drawCenter = Projectile.Center - Main.screenPosition + new Vector2(0, -12);
			var drawColor = new Color(0.1f, 0.7f, 0.9f, 0);
			Main.spriteBatch.Draw(star, drawCenter, null, drawColor, 0, star.Size() * 0.5f, new Vector2(0.8f * sinValue, 0.85f), SpriteEffects.None, 0);
			Main.spriteBatch.Draw(star, drawCenter, null, drawColor, MathHelper.PiOver2, star.Size() * 0.5f, new Vector2(0.5f * sinValue, 1f), SpriteEffects.None, 0);
			Main.spriteBatch.Draw(star, drawCenter, null, drawColor, -MathHelper.PiOver4, star.Size() * 0.5f, new Vector2(0.5f * sinValue, 0.35f), SpriteEffects.None, 0);
			Main.spriteBatch.Draw(star, drawCenter, null, drawColor, MathHelper.PiOver4, star.Size() * 0.5f, new Vector2(0.5f * sinValue, 0.35f), SpriteEffects.None, 0);
		}
		return false;
	}
}

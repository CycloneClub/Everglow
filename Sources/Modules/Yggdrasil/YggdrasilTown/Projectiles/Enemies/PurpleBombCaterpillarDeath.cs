using Everglow.Yggdrasil.YggdrasilTown.NPCs;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Everglow.Yggdrasil.YggdrasilTown.VFXs.NPCEffects;
using Terraria.Audio;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Enemies;

public class PurpleBombCaterpillarDeath : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.EnemyProjectiles;

	private Vector2[] bodyFeet = Array.Empty<Vector2>();
	private Vector2[] bodyTops = Array.Empty<Vector2>();
	private float bodyScale = 1;
	private Vector2 bodyOrigin;

	internal void SetBodyShape(IReadOnlyList<Vector2> feet, IReadOnlyList<Vector2> tops)
	{
		bodyFeet = feet.ToArray();
		bodyTops = tops.ToArray();
		bodyScale = Projectile.scale;
		bodyOrigin = Projectile.Center;
	}

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
			SpawnExplosionEffects();
		}
		if (!Main.dedServ && Projectile.timeLeft == 6)
		{
			SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, Projectile.Center);
		}
		if (!Main.dedServ && Projectile.timeLeft == 36)
		{
			var explosion = new PurpleBombCaterpillarExplosionDust
			{
				Position = Projectile.Center,
				Active = true,
				Visible = true,
				Timer = 0,
				MaxTime = 120,
				Scale = Main.rand.NextFloat(540f, 600f),
				Rotation = Main.rand.NextFloat(6.283f),
			};
			Ins.VFXManager.Add(explosion);
		}
		float value = (90 - Projectile.timeLeft) * 0.02f;
		if (!Main.dedServ)
		{
			Lighting.AddLight(Projectile.Center, new Vector3(1f, 0.05f, 0.6f) * value);
		}
		Projectile.scale += value * 0.004f;
	}

	private void SpawnExplosionEffects()
	{
		if (Main.dedServ)
		{
			return;
		}
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

	public override bool PreDraw(ref Color lightColor)
	{
		if (Main.dedServ || Projectile.timeLeft < 5 || bodyFeet.Length < 2)
		{
			return false;
		}
		float explosionValue = MathF.Pow(Projectile.timeLeft, -0.25f) * 150f;
		explosionValue = MathF.Sin(explosionValue * 0.6f) + 0.5f;
		float explosionProgress = (90 - Projectile.timeLeft) / 90f;
		lightColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates());
		Color glowColor = new Color(1f, 0.65f, 0.25f, 0);

		float scale = Projectile.scale / bodyScale;
		Vector2[] feet = bodyFeet.Select(point => Projectile.Center + (point - bodyOrigin) * scale).ToArray();
		Vector2[] tops = bodyTops.Select(point => Projectile.Center + (point - bodyOrigin) * scale).ToArray();
		PurpleBombCaterpillar.DrawBodyLayer(ModAsset.PurpleBombCaterpillar_Body.Value, feet, tops, lightColor, Projectile.spriteDirection);
		PurpleBombCaterpillar.DrawBodyLayer(ModAsset.PurpleBombCaterpillar_Body_glow.Value, feet, tops, glowColor, Projectile.spriteDirection);
		PurpleBombCaterpillar.DrawBodyLayer(ModAsset.PurpleBombCaterpillar_Body_Shape.Value, feet, tops, Color.White * explosionValue * explosionProgress, Projectile.spriteDirection);

		Texture2D eyeStalks = ModAsset.PurpleBombCaterpillar_Eye.Value;
		Rectangle frame = new Rectangle(0, 0, eyeStalks.Width, eyeStalks.Height / 10);
		SpriteEffects flip = Projectile.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
		var eyes = PurpleBombCaterpillar.GetEyePose(feet, tops, Projectile.spriteDirection, frame.Width);
		SpriteBatchUtils.SpriteBatch_Draw_To_Vertex_Draw(eyeStalks, eyes.Position - Main.screenPosition, frame, lightColor, eyes.Rotation, eyes.Origin, Projectile.scale, flip);
		SpriteBatchUtils.SpriteBatch_Draw_To_Vertex_Draw(ModAsset.PurpleBombCaterpillar_Eye_glow.Value, eyes.Position - Main.screenPosition, frame, glowColor, eyes.Rotation, eyes.Origin, Projectile.scale, flip);
		return false;
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		return MathUtils.IntersectsCircleAABB(Projectile.Center, 100, targetHitbox);
	}
}

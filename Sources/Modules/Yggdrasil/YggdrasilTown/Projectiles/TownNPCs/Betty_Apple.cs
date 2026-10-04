using Everglow.Commons.DataStructures;
using Everglow.Commons.Templates.Weapons;
using Everglow.Yggdrasil.YggdrasilTown.Dusts.TownNPCAttack;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Everglow.Yggdrasil.YggdrasilTown.VFXs.TownNPCAttack;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.TownNPCs;

public class Betty_Apple : TrailingProjectile
{
	public override void SetCustomDefaults()
	{
		Projectile.width = 14;
		Projectile.height = 14;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.ignoreWater = false;
		Projectile.tileCollide = true;
		Projectile.penetrate = 1;
		Projectile.timeLeft = 600;
		ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[Type] = true;
		TrailLength = 12;
		TrailColor = new Color(0.2f, 0.2f, 0.2f, 0f);
		TrailBackgroundDarkness = 0.1f;
		TrailWidth = 24f;
		SelfLuminous = false;
		TrailTexture = Commons.ModAsset.Trail_10.Value;
		TrailTextureBlack = Commons.ModAsset.Trail_10_black.Value;
		TrailShader = Commons.ModAsset.Trailing.Value;
	}

	public override void Behaviors()
	{
		if (TimeAfterEntityDestroy < 0)
		{
			if (Projectile.velocity.Y <= 12)
			{
				Projectile.velocity.Y += 0.6f;
			}
			if (Projectile.timeLeft % 6 == 0)
			{
				Projectile.frame++;
				if (Projectile.frame > 11)
				{
					Projectile.frame = 0;
				}
			}
		}
		Projectile.velocity *= 0.99f;
		Projectile.rotation += Projectile.ai[0];
	}

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.rotation = Main.rand.NextFloat(6.283f);
		Projectile.ai[0] = Main.rand.NextFloat(-0.15f, 0.15f);
	}

	public override Color GetTrailColor(int style, Vector2 worldPos, int index, ref float factor, float extraValue0 = 0, float extraValue1 = 0) => base.GetTrailColor(style, worldPos, index, ref factor, extraValue0, extraValue1);

	public override void DestroyEntityEffect()
	{
		for (int x = 0; x < 9; x++)
		{
			var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Betty_Apple_Mesocarp_Dust>());
			d.velocity = new Vector2(0, MathF.Sqrt(Main.rand.NextFloat(1f)) * 3f).RotatedByRandom(6.283);
			d.scale = Main.rand.NextFloat(0.6f, 1.2f);
			d.velocity -= Projectile.velocity.RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat() * 0.15f;
			d.position += d.velocity;
		}
		for (int x = 0; x < 4; x++)
		{
			var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Betty_Apple_Skin_Dust>());
			d.velocity = new Vector2(0, MathF.Sqrt(Main.rand.NextFloat(1f)) * 2.6f).RotatedByRandom(6.283);
			d.scale = Main.rand.NextFloat(0.6f, 1.4f);
			if (d.frame.Y == 0)
			{
				d.scale *= 0.5f;
			}
			d.velocity -= Projectile.velocity.RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat() * 0.15f;
			d.position += d.velocity;
		}
		for (int j = 0; j < 9; j++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(1f, 2f)).RotatedByRandom(MathHelper.TwoPi);
			var somg = new BettyAppleDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
				MaxTime = Main.rand.Next(60, 75),
				Scale = Main.rand.NextFloat(20f, 35f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
		SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundMiss, Projectile.Center);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		if (Projectile.penetrate == 1)
		{
			DestroyEntity();
		}
	}
}

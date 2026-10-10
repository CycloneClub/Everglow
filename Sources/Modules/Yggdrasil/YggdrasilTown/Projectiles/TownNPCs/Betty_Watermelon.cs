using Everglow.Commons.DataStructures;
using Everglow.Commons.Templates.Weapons;
using Everglow.Yggdrasil.YggdrasilTown.Dusts.TownNPCAttack;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Everglow.Yggdrasil.YggdrasilTown.VFXs.TownNPCAttack;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.TownNPCs;

public class Betty_Watermelon : TrailingProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.TownNPCProjectiles;

	public override void SetCustomDefaults()
	{
		Projectile.width = 28;
		Projectile.height = 28;
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
		TrailWidth = 60f;
		SelfLuminous = false;
		TrailTexture = Commons.ModAsset.Trail_10.Value;
		TrailTextureBlack = Commons.ModAsset.Trail_10_black.Value;
		TrailShader = Commons.ModAsset.Trailing.Value;
	}

	public override void Behaviors()
	{
		if (TimeAfterEntityDestroy < 0)
		{
			if (Projectile.velocity.Y <= 21)
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
		Projectile.rotation += Projectile.ai[0];
		Projectile.velocity *= 0.99f;
	}

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.frame = Main.rand.Next(12);
		Projectile.rotation = Main.rand.NextFloat(6.283f);
		Projectile.ai[0] = Main.rand.NextFloat(-0.15f, 0.15f);
	}

	public override void DestroyEntityEffect()
	{
		for (int x = 0; x < 48; x++)
		{
			var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Betty_Watermelon_Mesocarp_Dust>());
			d.velocity = new Vector2(0, MathF.Sqrt(Main.rand.NextFloat(1f)) * 8f).RotatedByRandom(6.283);
			d.scale = Main.rand.NextFloat(0.8f, 1.8f);
			d.velocity -= Projectile.velocity.RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat() * 0.6f;
			d.position += d.velocity;
		}
		for (int x = 0; x < 6; x++)
		{
			var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Betty_Watermelon_Skin_Dust>());
			d.velocity = new Vector2(0, MathF.Sqrt(Main.rand.NextFloat(1f)) * 3.6f).RotatedByRandom(6.283);
			d.scale = Main.rand.NextFloat(0.8f, 2f);
			if (d.frame.Y == 0)
			{
				d.scale *= 0.5f;
			}
			d.velocity -= Projectile.velocity.RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat() * 0.3f;
			d.position += d.velocity;
		}
		for (int j = 0; j < 4; j++)
		{
			Vector2 v0 = new Vector2(0, Main.rand.NextFloat(0, 6f)).RotatedByRandom(MathHelper.TwoPi);
			int type = ModContent.Find<ModGore>("Everglow/Betty_Watermelon_Gore" + Main.rand.Next(4)).Type;
			var gore = Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.Center, v0, type, Projectile.scale);
			gore.velocity *= 0.3f;
			gore.scale = Main.rand.NextFloat(0.8f, 1.2f);
			gore.velocity -= Projectile.velocity.RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat() * 0.1f;
		}
		for (int j = 0; j < 4; j++)
		{
			Vector2 v0 = new Vector2(0, Main.rand.NextFloat(0, 6f)).RotatedByRandom(MathHelper.TwoPi);
			int type = ModContent.Find<ModGore>("Everglow/Betty_Watermelon_Gore" + Main.rand.Next(4, 7)).Type;
			var gore = Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.Center, v0, type, Projectile.scale);
			gore.velocity *= 0.3f;
			gore.scale = Main.rand.NextFloat(0.8f, 1.2f);
			gore.velocity -= Projectile.velocity.RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat() * 0.1f;
		}
		for (int j = 0; j < 12; j++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(2f, 8f)).RotatedByRandom(MathHelper.TwoPi);
			var somg = new BettyWatermelonDust
			{
				Velocity = newVelocity,
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
		SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, Projectile.Center);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		if (Projectile.penetrate == 1)
		{
			DestroyEntity();
		}
	}

	public override void DrawSelf()
	{
		var texMain = (Texture2D)ModContent.Request<Texture2D>(Texture);
		Color color = TrailColor;
		if (!SelfLuminous)
		{
			color = Lighting.GetColor(Projectile.Center.ToTileCoordinates());
		}
		Rectangle frame = new Rectangle(0, texMain.Height / 12 * Projectile.frame, texMain.Width, texMain.Height / 12);
		Main.spriteBatch.Draw(texMain, Projectile.Center - Main.screenPosition, frame, color, Projectile.rotation, frame.Size() / 2f, 1f, SpriteEffects.None, 0);
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => base.Colliding(projHitbox, targetHitbox);
}

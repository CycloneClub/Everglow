using Everglow.Commons.Templates.Weapons;
using Everglow.Myth.TheFirefly.Dusts;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Myth.TheFirefly.Projectiles.DreamWeaver;

public class DreamWeaver_Rain : TrailingProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override string Texture => ModAsset.DreamWeaverII_Mod;

	public override void SetCustomDefaults()
	{
		TrailColor = new Color(0, 0.4f, 1, 0f);
		TrailBackgroundDarkness = 0.1f;
		TrailWidth = 8f;
		SelfLuminous = false;
		TrailLength = 12;
		TrailTexture = Commons.ModAsset.Trail_1.Value;
		TrailTextureBlack = Commons.ModAsset.Trail_black.Value;
		TrailShader = Commons.ModAsset.Trailing.Value;
		ProjectileID.Sets.DrawScreenCheckFluff[Type] = 10800;
	}

	private int breakTime = 200;

	public override void OnSpawn(IEntitySource source)
	{
		base.OnSpawn(source);
	}

	public override void Behaviors()
	{
		Projectile.velocity.Y += 0.6f;
		if (Projectile.timeLeft < breakTime && Projectile.ai[0] != 3)
		{
			if (Projectile.timeLeft == breakTime - 1)
			{
				for (int x = 0; x < 6; x++)
				{
					Vector2 velocity = new Vector2(0, Main.rand.NextFloat(2f, 12f)).RotatedByRandom(6.283);
					var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + velocity * -2 - Projectile.velocity * 4, velocity + Projectile.velocity, Type, Projectile.damage, Projectile.knockBack, Projectile.owner, 3f, Main.rand.NextFloat(0, 0.75f)/*If ai[0] equal to 3, another ai will be execute*/);
					SoundEngine.PlaySound(SoundID.Item54);
					p.friendly = false;
					p.damage = Projectile.damage / 4;
					p.timeLeft = 240;
					p.extraUpdates = 3;
				}
				for (int x = 0; x < 5; x++)
				{
					Vector2 BasePos = Projectile.Center - new Vector2(4) - Projectile.velocity;
					var d0 = Dust.NewDustDirect(BasePos, 0, 0, ModContent.DustType<BlueGlowAppearStoppedByTile>(), 0, 0, 0, default, 0.6f);
					d0.noGravity = true;
				}
			}
			Projectile.velocity *= 0;
			return;
		}

		if (Projectile.timeLeft < 239)
		{
			if (Collision.SolidCollision(Projectile.Center, 0, 0))
			{
				Projectile.velocity *= 0.1f;

				if (Projectile.extraUpdates == 1)
				{
					for (int x = 0; x < 15 / (Projectile.ai[0] + 1); x++)
					{
						Vector2 BasePos = Projectile.Center - new Vector2(4) - Projectile.velocity;
						var d1 = Dust.NewDustDirect(BasePos, 0, 0, ModContent.DustType<BlueGlowAppearStoppedByTile>(), 0, 0, 0, default, 0.6f);
						d1.velocity = new Vector2(0, Main.rand.NextFloat(0f, 3f)).RotatedByRandom(6.283);
						d1.noGravity = true;
					}
					if (Projectile.ai[0] != 3)
					{
						for (int x = 0; x < 3; x++)
						{
							Vector2 velocity = new Vector2(0, Main.rand.NextFloat(2f, 6f)).RotatedByRandom(6.283) - Projectile.velocity * 0.2f;
							var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + velocity * -2 - Projectile.velocity * 2, velocity, Type, Projectile.damage, Projectile.knockBack, Projectile.owner, 3f/*If ai[0] equal to 3, another ai will be execute*/);
							SoundEngine.PlaySound(SoundID.Item54);
							p.friendly = false;
						}
					}
					Projectile.extraUpdates = 2;
					SoundEngine.PlaySound(SoundID.Drip, Projectile.Center);
				}
			}
			else
			{
				if (Projectile.extraUpdates == 2)
				{
					Projectile.extraUpdates = 1;
				}
			}
		}
		if (Projectile.timeLeft == 210)
		{
			Projectile.friendly = true;
		}
	}

	public override void DrawSelf()
	{
		var texMain = (Texture2D)ModContent.Request<Texture2D>(Texture);
		Main.spriteBatch.Draw(texMain, Projectile.Center - Main.screenPosition - Projectile.velocity, null, TrailColor, Projectile.rotation, texMain.Size() / 2f, 1f, SpriteEffects.None, 0);
	}

	public override void DestroyEntityEffect()
	{
		SoundEngine.PlaySound(SoundID.Drip, Projectile.Center);
		for (int x = 0; x < 6; x++)
		{
			Vector2 BasePos = Projectile.Center - new Vector2(4);
			var d0 = Dust.NewDustDirect(BasePos, 0, 0, ModContent.DustType<BlueGlowAppearStoppedByTile>(), 0, 0, 0, default, Main.rand.NextFloat(0.4f, 1.5f));
			d0.noGravity = true;
			d0.velocity = new Vector2(0, -Main.rand.NextFloat(0.5f, 2f)) * 2;
		}
		Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<DreamWeaver_hit>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Main.rand.NextFloat(0.7f, 1.3f));
	}

	public override Color GetTrailColor(int style, Vector2 worldPos, int index, ref float factor, float extraValue0 = 0, float extraValue1 = 0) => base.GetTrailColor(style, worldPos, index, ref factor, extraValue0, extraValue1);

	public override Vector3 ModifyTrailTextureCoordinate(float factor, float timeValue, float phase, float widthValue) => base.ModifyTrailTextureCoordinate(factor, timeValue, phase, widthValue);

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		base.OnHitNPC(target, hit, damageDone);
	}
}

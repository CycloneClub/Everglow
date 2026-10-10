using Everglow.Commons.Templates.Weapons;
using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Ranged;

public class BrittleRockSlingshotStone : TrailingProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedProjectiles;

	public override void SetCustomDefaults()
	{
		Projectile.ranged = true;
		Projectile.width = 16;
		Projectile.height = 16;
		Projectile.scale = 0.75f;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.ignoreWater = false;
		Projectile.tileCollide = true;
		Projectile.penetrate = 1;
		Projectile.timeLeft = 600;

		TrailLength = 21;
		TrailColor = new Color(1, 0.3f, 1, 0f);
		TrailWidth = 40f;
		SelfLuminous = true;
		TrailTexture = Commons.ModAsset.Trail_2_thick.Value;
		TrailTextureBlack = Commons.ModAsset.Trail_2_black.Value;
		TrailShader = Commons.ModAsset.Trailing.Value;
		TrailBackgroundDarkness = 0.1f;
	}

	public override void Behaviors()
	{
		if (Projectile.velocity.Y <= 12)
		{
			Projectile.velocity.Y += 0.2f;
		}
		Projectile.rotation += Projectile.ai[0];
		if (Projectile.timeLeft > 540)
		{
			float value = (Projectile.timeLeft - 540) / 30f;
			var d = Dust.NewDustDirect(Projectile.position - new Vector2(12 * Projectile.scale), (int)(Projectile.width + 24 * Projectile.scale), (int)(Projectile.height + 24 * Projectile.scale), ModContent.DustType<RockElemental_Energy_normal>());
			d.velocity = Projectile.velocity * 0.5f;
			d.scale = Main.rand.NextFloat(0.75f, 1f) * Math.Min(value, 1) * Projectile.ai[2];
			d.noGravity = true;
		}
	}

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.rotation = Main.rand.NextFloat(6.283f);
		Projectile.ai[0] = Main.rand.NextFloat(-0.15f, 0.15f);
	}

	public override void DestroyEntityEffect()
	{
		for (int x = 0; x < 3; x++)
		{
			var d = Dust.NewDustDirect(Projectile.position - new Vector2(4), Projectile.width, Projectile.height, ModContent.DustType<RockElemental_fragments>(), 0f, 0f, 0, default, 0.7f);
			d.velocity = new Vector2(0, Main.rand.NextFloat(0.1f, 1.6f)).RotatedByRandom(6.283);
		}
		for (int x = 0; x < 5; x++)
		{
			var d = Dust.NewDustDirect(Projectile.position - new Vector2(4), Projectile.width, Projectile.height, DustID.WitherLightning, 0f, 0f, 0, default, Main.rand.NextFloat(0.4f, 0.6f));
			d.velocity = new Vector2(0, Main.rand.NextFloat(0.7f, 4.1f)).RotatedByRandom(6.283);
		}
		GenerateSmog(3);

		if (Projectile.ai[1] != 1)
		{
			int n = Main.rand.Next(3, 4);
			for (int i = 0; i < n; i++)
			{
				var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity.RotatedByRandom(MathHelper.TwoPi) * 0.4f, Type, Projectile.damage / n, Projectile.knockBack / n, Projectile.owner, 0, 1, Projectile.ai[2] * 0.8f);
				p.scale = Projectile.scale / MathF.Sqrt(n);
				p.penetrate = 1;
				p.ai[1] = 1;
			}
			if (Projectile.ai[2] >= 0.8f)
			{
				Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<BrittleRockSlingshotStone_Explosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 1);
			}
			SoundEngine.PlaySound(SoundID.DD2_GoblinBomb, Projectile.Center);
		}
		else
		{
			SoundEngine.PlaySound(SoundID.NPCHit2.WithVolume(Projectile.ai[2]), Projectile.Center);
		}
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		if (Projectile.penetrate == 1)
		{
			DestroyEntity();
		}
	}

	public void GenerateSmog(int Frequency)
	{
		for (int g = 0; g < Frequency; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 4f)).RotatedByRandom(MathHelper.TwoPi);
			var somg = new RockSmogDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
				MaxTime = Main.rand.Next(37, 45),
				Scale = Main.rand.NextFloat(10f, 25f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
	}

	public override void DrawSelf()
	{
		float value = (Projectile.timeLeft - 540) / 60f;
		TrailColor = new Color(1, 0.3f, 1, 0f) * value * Projectile.ai[2];
		Texture2D texture = ModAsset.RockElemental_Stonefragment.Value;
		Vector2 drawCenter = Projectile.Center - Main.screenPosition;
		Color lightColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates());
		if (Projectile.timeLeft > 540)
		{
			for (int i = 0; i < 7; i++)
			{
				Main.EntitySpriteDraw(texture, drawCenter, null, new Color(0.7f, 0.4f, 1f, 0f) * value * 0.4f * Projectile.ai[2], Projectile.rotation + i, texture.Size() * 0.5f, Projectile.scale * (1.15f + value * 0.3f), SpriteEffects.None, 0);
			}
		}
		Main.EntitySpriteDraw(texture, drawCenter, null, lightColor, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
	}

	public override void DrawTrail()
	{
		base.DrawTrail();
	}

	public override Color GetTrailColor(int style, Vector2 worldPos, int index, ref float factor, float extraValue0 = 0, float extraValue1 = 0) => base.GetTrailColor(style, worldPos, index, ref factor, extraValue0, extraValue1);

	public override Vector3 ModifyTrailTextureCoordinate(float factor, float timeValue, float phase, float widthValue)
	{
		float x = factor - timeValue * 0.5f;
		float y = 1;
		widthValue *= Projectile.scale;
		float z = widthValue;
		if (phase == 2)
		{
			y = 0;
		}
		if (phase % 2 == 1)
		{
			y = 0.5f;
		}
		return new Vector3(x, y, z);
	}
}

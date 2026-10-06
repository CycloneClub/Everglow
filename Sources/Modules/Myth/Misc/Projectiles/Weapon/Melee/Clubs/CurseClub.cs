using Everglow.Commons.DataStructures;
using Everglow.Commons.Graphics;
using Everglow.Commons.VFX.CommonVFXDusts;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.Clubs;

public class CurseClub : ClubProj
{
	public override void SetCustomDefaults()
	{
		Beta = 0.005f;
		MaxOmega = 0.45f;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		target.AddBuff(BuffID.CursedInferno, (int)(818 * Omega));
	}

	public override void AI()
	{
		base.AI();

		if (Omega > 0.1f)
		{
			for (float d = 0.1f; d < Omega; d += 0.1f)
			{
				GenerateDust();
			}
			if (flyClubCooling > 0)
			{
				flyClubCooling--;
			}

			if (flyClubCooling <= 0 && Omega > 0.2f)
			{
				flyClubCooling = 44;
				Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<CurseClub_fly>(), (int)(Projectile.damage * 0.3f), Projectile.knockBack * 0.4f, Projectile.owner);
			}
		}
		GenerateVFX();
	}

	private int flyClubCooling = 0;

	private void GenerateVFX()
	{
		Vector2 v2 = Main.player[Projectile.owner].velocity;
		float mulVelocity = 0.3f + Omega;
		var v0 = new Vector2(1, 1);
		v0 *= HitLength * Projectile.scale;
		v0.X *= Projectile.spriteDirection;
		if (Main.rand.NextBool(2))
		{
			v0 *= -1;
		}
		v0 = v0.RotatedBy(Projectile.rotation + Main.rand.NextFloat(Omega));
		float Speed = Math.Min(Omega * 0.15f, 0.061f) * Main.rand.NextFloat(1.1f, Main.rand.NextFloat(1.1f, 7f));
		var v1 = new Vector2(-v0.Y, v0.X) * Speed;
		/*
		var cf = new CurseFlame_HighQualityDust
		{
			Velocity = v1 + v2 * 0.9f,
			Active = true,
			Visible = true,
			Position = Projectile.Center + v0,
			MaxTime = Main.rand.Next(17, 35),
			ai = new float[] { Main.rand.NextFloat(0.1f, 1f), Omega * 0.5f, Main.rand.NextFloat(3.6f, 30f) * mulVelocity },
		};
		Ins.VFXManager.Add(cf);*/
		if (Main.rand.NextBool(4))
		{
			GradientColor color = new GradientColor();
			color.colorList.Add((new Color(1f, 1f, 0.1f), 0f));
			color.colorList.Add((new Color(0.1f, 0.6f, 0.1f), 0.3f));
			int time = Main.rand.Next(15, 35);
			var fire = new Flare()
			{
				Position = Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * 30, Main.rand.NextFloat(0.4f, 1.25f)),
				Velocity = Projectile.velocity * 0.5f,
				color = color,
				timeleft = time,
				maxTimeleft = time,
				Scale = Main.rand.NextFloat(0.3f, 0.6f),
			};
			Ins.VFXManager.Add(fire);
		}
		for (int g = 0; g < 4; g++)
		{
			v0 = new Vector2(1, 1);
			v0 *= Main.rand.NextFloat(Main.rand.NextFloat(HitLength * 0.75f, HitLength), HitLength);
			v0.X *= Projectile.spriteDirection;
			if (Main.rand.NextBool(2))
			{
				v0 *= -1;
			}

			v0 = v0.RotatedBy(Projectile.rotation + Main.rand.NextFloat(Omega));
			Vector2 newVelocity = new Vector2(-v0.Y, v0.X) * Speed;
			float v0Length = v0.Length();
			var spark = new CurseFlameSparkDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + v0,
				MaxTime = Main.rand.Next(37, Main.rand.Next(37, 225)),
				Scale = Main.rand.NextFloat(4f, 27.0f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Omega * 0.1f * v0Length / 14f, 15f },
			};
			Ins.VFXManager.Add(spark);
		}
	}

	private void GenerateDust()
	{
		var v0 = new Vector2(1, 1);
		v0 *= Main.rand.NextFloat(Main.rand.NextFloat(1, HitLength), HitLength);
		v0.X *= Projectile.spriteDirection;
		if (Main.rand.NextBool(2))
		{
			v0 *= -1;
		}

		v0 = v0.RotatedBy(Projectile.rotation);
		float Speed = Math.Min(Omega * 0.5f, 0.221f);
		var D = Dust.NewDustDirect(Projectile.Center + v0 - new Vector2(4)/*Dust的Size=8x8*/, 0, 0, DustID.CursedTorch, -v0.Y * Speed, v0.X * Speed, 150, default, Main.rand.NextFloat(0.4f, 1.1f));
		D.noGravity = true;
		D.velocity = new Vector2(-v0.Y * Speed, v0.X * Speed);
	}

	public override void PostPreDraw()
	{
		float fade = Omega * 2f + 0.2f;
		var color2 = new Color(Math.Min(fade * 0.6f, 0.6f), fade, fade * 0.01f, fade);
		var bars = CreateTrailVertices(paramA: 0.5f, paramB: 0.5f, useSpecialAplha: true, trailColor: color2);
		if (bars == null)
		{
			return;
		}

		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;

		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, TrailBlendState(), SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, model);

		Main.graphics.GraphicsDevice.Textures[0] = ModAsset.CurseClub_trail.Value;

		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
	}
}

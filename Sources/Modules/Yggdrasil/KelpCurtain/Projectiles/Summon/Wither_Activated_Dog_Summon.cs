using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.KelpCurtain.Buffs;
using Everglow.Yggdrasil.KelpCurtain.VFXs;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Summon;

public class Wither_Activated_Dog_Summon : ModProjectile
{
	public int Timer = 0;

	public override void SetDefaults()
	{
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.width = 40;
		Projectile.height = 40;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 60;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
	}

	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
	{
		overPlayers.Add(index);
	}

	public override void OnSpawn(IEntitySource source)
	{
		float minRange = 40f;
		float maxRange = 120f;
		for (int k = 0; k < 30; k++)
		{
			var leaf = new ActivatedDogStaff_Leaf
			{
				Velocity = new Vector2(0, 0.5f).RotatedByRandom(Math.PI * 2) + Projectile.velocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(0, Main.rand.NextFloat(minRange, maxRange)).RotatedByRandom(MathHelper.TwoPi),
				MaxTime = Main.rand.Next(70, 120),
				Scale = Main.rand.Next(8, 10),
				ParentEneity = Projectile,
				ai = new float[] { Main.rand.NextFloat(1f, 8f), Projectile.timeLeft },
			};
			Ins.VFXManager.Add(leaf);
		}
		for (int k = 0; k < 10; k++)
		{
			var branch = new ActivatedDogStaff_Branch
			{
				Velocity = new Vector2(0, 0.5f).RotatedByRandom(Math.PI * 2) + Projectile.velocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(0, Main.rand.NextFloat(minRange, maxRange)).RotatedByRandom(MathHelper.TwoPi),
				MaxTime = Main.rand.Next(70, 120),
				Scale = Main.rand.Next(18, 30),
				ParentEneity = Projectile,
				Frame = Main.rand.Next(5),
				ai = new float[] { Main.rand.NextFloat(1f, 8f), Projectile.timeLeft },
			};
			Ins.VFXManager.Add(branch);
		}
	}

	public override void AI()
	{
		Timer++;
		if (Projectile.timeLeft < 30f)
		{
			return;
		}
		float minRange = 40f;
		float maxRange = 120f;
		for (int k = 0; k < 3; k++)
		{
			var leaf = new ActivatedDogStaff_Leaf
			{
				Velocity = new Vector2(0, 0.5f).RotatedByRandom(Math.PI * 2) + Projectile.velocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(0, Main.rand.NextFloat(minRange, maxRange)).RotatedByRandom(MathHelper.TwoPi),
				MaxTime = Main.rand.Next(70, 120),
				Scale = Main.rand.Next(8, 10),
				ParentEneity = Projectile,
				ai = new float[] { Main.rand.NextFloat(1f, 8f), Projectile.timeLeft },
			};
			Ins.VFXManager.Add(leaf);
		}
		for (int k = 0; k < 1; k++)
		{
			var branch = new ActivatedDogStaff_Branch
			{
				Velocity = new Vector2(0, 0.5f).RotatedByRandom(Math.PI * 2) + Projectile.velocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(0, Main.rand.NextFloat(minRange, maxRange)).RotatedByRandom(MathHelper.TwoPi),
				MaxTime = Main.rand.Next(70, 120),
				Scale = Main.rand.Next(18, 30),
				ParentEneity = Projectile,
				Frame = Main.rand.Next(5),
				ai = new float[] { Main.rand.NextFloat(1f, 8f), Projectile.timeLeft },
			};
			Ins.VFXManager.Add(branch);
		}
	}

	public override void OnKill(int timeLeft)
	{
		base.OnKill(timeLeft);
		Player player = Main.player[Projectile.owner];
		Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<Wither_Activated_Dog>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
		player.AddBuff(ModContent.BuffType<Wither_Activated_Dog_Buff>(), 1800000);

		float minRange = 10f;
		float maxRange = 40f;
		for (int k = 0; k < 30; k++)
		{
			var leaf = new ActivatedDogStaff_Leaf_physical
			{
				Velocity = new Vector2(0, Main.rand.NextFloat(7, 12)).RotatedByRandom(Math.PI * 2),
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(0, Main.rand.NextFloat(minRange, maxRange)).RotatedByRandom(MathHelper.TwoPi),
				MaxTime = Main.rand.Next(70, 120),
				Scale = Main.rand.Next(8, 10),
				ai = new float[] { Main.rand.NextFloat(1f, 8f), Projectile.timeLeft },
			};
			Ins.VFXManager.Add(leaf);
		}
		for (int k = 0; k < 10; k++)
		{
			var branch = new ActivatedDogStaff_Branch_physical
			{
				Velocity = new Vector2(0, Main.rand.NextFloat(7, 12)).RotatedByRandom(Math.PI * 2),
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(0, Main.rand.NextFloat(minRange, maxRange)).RotatedByRandom(MathHelper.TwoPi),
				MaxTime = Main.rand.Next(70, 120),
				Scale = Main.rand.Next(18, 30),
				Frame = Main.rand.Next(5),
				ai = new float[] { Main.rand.NextFloat(1f, 8f), Projectile.timeLeft },
			};
			Ins.VFXManager.Add(branch);
		}
		for (int k = 0; k < 10; k++)
		{
			var branch = new ActivatedDogStaff_green_Dust_Trail
			{
				velocity = new Vector2(0, Main.rand.NextFloat(7, 12)).RotatedByRandom(Math.PI * 2),
				Active = true,
				Visible = true,
				position = Projectile.Center + new Vector2(0, Main.rand.NextFloat(minRange, maxRange)).RotatedByRandom(MathHelper.TwoPi),
				maxTime = Main.rand.Next(70, 120),
				scale = Main.rand.Next(18, 30),
				ai = new float[] { Main.rand.NextFloat(1f, 8f), Projectile.timeLeft },
			};
			Ins.VFXManager.Add(branch);
		}
		for (int k = 0; k < 30; k++)
		{
			var branch = new ActivatedDogStaff_green_Dust_Trail
			{
				velocity = new Vector2(0, Main.rand.NextFloat(4, 8)).RotatedByRandom(Math.PI * 2),
				Active = true,
				Visible = true,
				position = Projectile.Center + new Vector2(0, Main.rand.NextFloat(minRange, maxRange)).RotatedByRandom(MathHelper.TwoPi),
				maxTime = Main.rand.Next(35, 50),
				scale = Main.rand.Next(6, 12),
				ai = new float[] { Main.rand.NextFloat(1f, 8f), Projectile.timeLeft },
			};
			Ins.VFXManager.Add(branch);
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		float radius = 20 + MathF.Pow(Projectile.timeLeft / 60f, 0.5f) * 120;
		float fade = MathF.Pow(Timer / 60f, 2.5f);
		Vector2 ringWidth = new Vector2(0, -20f).RotatedBy(Projectile.spriteDirection);
		Color drawColor = new Color(155, 62, 48, 0);
		Color drawColor2 = new Color(255, 201, 0, 0);
		float timeValue = Timer * 0.05f;
		List<Vertex2D> bars = new List<Vertex2D>();
		List<Vertex2D> bars_Dark = new List<Vertex2D>();
		List<Vertex2D> bars2 = new List<Vertex2D>();
		for (int i = 0; i < 30; i++)
		{
			Vector2 circlePos = new Vector2(radius, 0).RotatedBy(i / 30f * MathHelper.TwoPi);
			Vector2 oval = circlePos;
			oval.Y *= 0.5f;
			oval = oval.RotatedBy(Projectile.spriteDirection);
			Vector2 drawPos = Projectile.Center + oval - Main.screenPosition;
			bars.Add(drawPos + ringWidth, drawColor * fade, new Vector3(i / 10f * Projectile.spriteDirection + timeValue, 0, 0));
			bars.Add(drawPos - ringWidth, drawColor * fade, new Vector3(i / 10f * Projectile.spriteDirection + timeValue, 1, 0));

			bars_Dark.Add(drawPos + ringWidth, Color.White * fade, new Vector3(i / 10f * Projectile.spriteDirection + timeValue, 0, 0));
			bars_Dark.Add(drawPos - ringWidth, Color.White * fade, new Vector3(i / 10f * Projectile.spriteDirection + timeValue, 1, 0));

			bars2.Add(drawPos + ringWidth * 2f, drawColor2 * fade * 2, new Vector3(-i / 3f * Projectile.spriteDirection - timeValue * 1.5f, 0, 0));
			bars2.Add(drawPos - ringWidth * 2f, drawColor2 * fade * 2, new Vector3(-i / 3f * Projectile.spriteDirection - timeValue * 1.5f, 1, 0));

			if (((-i / 3f * Projectile.spriteDirection - timeValue * 1.5f) + 10000) % 1 is > 0.5f and < 0.75f)
			{
				Lighting.AddLight(drawPos + Main.screenPosition, new Vector3(1f, 0.9f, 0.2f) * fade);
			}
		}
		List<Vertex2D> lightRays = new List<Vertex2D>();
		for (int r = 0; r <= 60; r++)
		{
			lightRays.Add(Projectile.Center - Main.screenPosition, new Color(0.0f, 0.4f, 0.2f, 0), new Vector3(r / 60f, Timer / 160f, 0));
			lightRays.Add(Projectile.Center + new Vector2(0, 50).RotatedBy(r / 60f * MathHelper.TwoPi) - Main.screenPosition, new Color(0, 0, 0, 0), new Vector3(r / 60f, Timer / 160f, 0));
		}
		if (bars.Count > 0 && bars_Dark.Count > 0 && bars2.Count > 0 && lightRays.Count > 0)
		{
			SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_9_black.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars_Dark.ToArray(), 0, bars_Dark.Count - 2);

			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_9.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.LightPoint2.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars2.ToArray(), 0, bars2.Count - 2);

			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Noise_hiveCyber.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, lightRays.ToArray(), 0, lightRays.Count - 2);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(sBS);
		}

		Texture2D star = Commons.ModAsset.StarSlash.Value;
		Texture2D star_black = Commons.ModAsset.StarSlash_black.Value;
		float scaleValue = Timer / 60f;
		scaleValue = MathF.Pow(scaleValue, 0.5f);
		if (Projectile.timeLeft <= 5)
		{
			scaleValue *= Projectile.timeLeft / 5f;
		}
		Main.spriteBatch.Draw(star_black, Projectile.Center - Main.screenPosition, null, Color.White * 0.2f * scaleValue, 0, star.Size() * 0.5f, scaleValue, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(star_black, Projectile.Center - Main.screenPosition, null, Color.White * 0.2f * scaleValue, MathHelper.PiOver2, star.Size() * 0.5f, scaleValue * 1.3f, SpriteEffects.None, 0);

		Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, new Color(0.3f, 1f, 0.1f, 0) * scaleValue, 0, star.Size() * 0.5f, scaleValue, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, new Color(0.3f, 1f, 0.1f, 0) * scaleValue, MathHelper.PiOver2, star.Size() * 0.5f, scaleValue * 1.3f, SpriteEffects.None, 0);

		Lighting.AddLight(Projectile.Center, new Vector3(0.3f, 1f, 0.1f) * scaleValue * 3);
		return false;
	}
}

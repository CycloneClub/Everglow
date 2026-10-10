using Everglow.Myth.Common;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.PrimordialJadeWinged_Spear;

public class PrimordialJadeWinged_Spear_thrust2 : ModProjectile, IWarpProjectile
{
	public override string Texture => "Everglow/Myth/Misc/Projectiles/Weapon/Melee/PrimordialJadeWinged_Spear/PrimordialJadeWinged_Spear";

	public override void SetDefaults()
	{
		Projectile.extraUpdates = 16;
		Projectile.timeLeft = 240;
		Projectile.width = 80;
		Projectile.height = 80;
		Projectile.tileCollide = false;
		Projectile.friendly = true;
		Projectile.penetrate = -1;
	}

	public override void OnSpawn(IEntitySource source)
	{
		Player player = Main.player[Projectile.owner];
		Projectile.extraUpdates = (int)(16 * player.meleeSpeed);
		Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 3.7F;
		SoundEngine.PlaySound(SoundID.Item71.WithVolume(0.6f).WithPitchOffset(-0.5f), Projectile.Center);
	}

	public override bool PreAI()
	{
		Player player = Main.player[Projectile.owner];
		Projectile.rotation = MathF.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathF.PI / 4f;
		Projectile.velocity *= 0.995f;
		player.heldProj = Projectile.whoAmI;

		if (Projectile.timeLeft < 6)
		{
			Projectile.velocity *= 0.4f;
		}
		if (Main.rand.Next(240) < Projectile.timeLeft)
		{
			if (Main.rand.NextBool(7))
			{
				GenerateVFX();
			}
			if (Main.rand.NextBool(6))
			{
				GenerateSpark();
			}
		}
		if (Projectile.timeLeft % 40 == 0)
		{
			Generate3DVFXRing();
		}
		MythContentPlayer myplayer = player.GetModPlayer<MythContentPlayer>();
		myplayer.InvincibleFrameTime = 15;
		return false;
	}

	public override void OnKill(int timeLeft)
	{
		Player player = Main.player[Projectile.owner];
		player.fullRotation = 0;
	}

	private void Generate3DVFXRing()
	{
		BlackRingVFX v = new BlackRingVFX()
		{
			pos = Projectile.Center,
			vel = Projectile.velocity,
			color = Color.Lerp(Color.White, Color.Transparent, 0.4f),
			scale = 3,
			maxtime = 20,
			timeleft = 20,
		};
		Ins.VFXManager.Add(v);
	}

	private void GenerateVFX()
	{
		Vector2 velocityLeft = Vector2.Normalize(Projectile.velocity).RotatedBy(-MathHelper.PiOver2);
		var positionVFX = Projectile.Center + velocityLeft * Main.rand.NextFloat(-30f, 30f) + Vector2.Normalize(Projectile.velocity) * Main.rand.NextFloat(Projectile.timeLeft - 320, Projectile.timeLeft + 60);
		if (Collision.SolidCollision(Projectile.Center, 1, 1))
		{
			positionVFX = Projectile.Center + velocityLeft * Main.rand.NextFloat(-40f, 40f) + Vector2.Normalize(Projectile.velocity) * Main.rand.NextFloat(-480f, 0f);
		}

		var filthy = new FilthyLucreFlame_darkDust
		{
			velocity = Projectile.velocity * 3f + velocityLeft * Main.rand.NextFloat(-10f, 10f),
			Active = true,
			Visible = true,
			position = positionVFX,
			maxTime = Main.rand.Next(17, 26),
			ai = new float[] { Main.rand.NextFloat(0.1f, 1f), Main.rand.NextFloat(-0.1f, 0.1f), Main.rand.NextFloat(18f, 30f) },
		};
		Ins.VFXManager.Add(filthy);

		var filthy2 = new FilthyLucreFlameDust
		{
			velocity = Projectile.velocity * 3f + velocityLeft * Main.rand.NextFloat(-10f, 10f),
			Active = true,
			Visible = true,
			position = positionVFX,
			maxTime = Main.rand.Next(17, 26),
			ai = new float[] { Main.rand.NextFloat(0.1f, 1f), Main.rand.NextFloat(-0.1f, 0.1f), Main.rand.NextFloat(18f, 30f) },
		};
		Ins.VFXManager.Add(filthy2);
	}

	public void GenerateSpark()
	{
		Vector2 velocityLeft = Vector2.Normalize(Projectile.velocity).RotatedBy(-MathHelper.PiOver2);
		var positionVFX = Projectile.Center + velocityLeft * Main.rand.NextFloat(-40f, 40f) + Vector2.Normalize(Projectile.velocity) * Main.rand.NextFloat(Projectile.timeLeft - 120, Projectile.timeLeft + 0);
		if (Collision.SolidCollision(Projectile.Center, 1, 1))
		{
			positionVFX = Projectile.Center + velocityLeft * Main.rand.NextFloat(-40f, 40f) + Vector2.Normalize(Projectile.velocity) * Main.rand.NextFloat(-480f, 0f);
		}
		var smog = new FilthyFragileDust
		{
			velocity = Projectile.velocity * Main.rand.NextFloat(1.4f, 2.4f),
			Active = true,
			Visible = true,
			position = positionVFX,
			coord = new Vector2(Main.rand.NextFloat(1f), Main.rand.NextFloat(1f)),
			maxTime = Main.rand.Next(60, 165),
			scale = Main.rand.NextFloat(Main.rand.NextFloat(3.4f, 6.4f), 14f),
			rotation = Main.rand.NextFloat(6.283f),
			rotation2 = Main.rand.NextFloat(6.283f),
			omega = Main.rand.NextFloat(-30f, 30f),
			phi = Main.rand.NextFloat(6.283f),
			ai = new float[] { Main.rand.NextFloat(0f, 0.2f), Main.rand.NextFloat(0.2f, 0.5f) },
		};
		Ins.VFXManager.Add(smog);
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
		float timeValue = 1f;
		if (Projectile.timeLeft < 60)
		{
			timeValue = Projectile.timeLeft / 60f;
		}
		Vector2 normalized = Vector2.Normalize(Projectile.velocity.RotatedBy(Math.PI * 0.5)) * 40f * timeValue;
		Vector2 start = Projectile.Center + Vector2.Normalize(Projectile.velocity) * 90;
		Vector2 end = Projectile.Center - Vector2.Normalize(Projectile.velocity) * 240;
		float value = Projectile.timeLeft / 135f;
		Vector2 middle = Vector2.Lerp(end, start, MathF.Sqrt(value) * 0.5f);
		float time = (float)(Main.time * 0.03);
		Color alphaColor = Color.White;
		alphaColor.A = 0;
		alphaColor.R = (byte)(((Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + 6.283 + Math.PI) % 6.283) / 6.283 * 255);
		alphaColor.G = (byte)((250 - collisionTimer * 6) * 0.1f);

		Color alphaColor2 = alphaColor;
		alphaColor2.G = 0;

		List<Vertex2D> bars = new List<Vertex2D>
			{
				new Vertex2D(start - Main.screenPosition, new Color(alphaColor.R, (int)(40 * (24 - collisionTimer) / 240f), 0, 0), new Vector3(1 + time, 0.25f, 0)),
				new Vertex2D(start - Main.screenPosition, new Color(alphaColor.R, (int)(40 * (24 - collisionTimer) / 240f), 0, 0), new Vector3(1 + time, 0.75f, 0)),
				new Vertex2D(middle + normalized - Main.screenPosition, alphaColor, new Vector3(0.5f + time, 0, 0.5f)),
				new Vertex2D(middle - normalized - Main.screenPosition, alphaColor, new Vector3(0.5f + time, 1, 0.5f)),
				new Vertex2D(end + normalized - Main.screenPosition, alphaColor2, new Vector3(0f + time, 0, 1)),
				new Vertex2D(end - normalized - Main.screenPosition, alphaColor2, new Vector3(0f + time, 1, 1)),
			};
		spriteBatch.Draw(Commons.ModAsset.Noise_spiderNet.Value, bars, PrimitiveType.TriangleStrip);
	}

	public int collisionTimer = 0;

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		Texture2D mainTex = ModAsset.PrimordialJadeWinged_Spear_PrimordialJadeWinged_Spear.Value;
		Vector2 drawCenter = Projectile.Center - Vector2.Normalize(Projectile.velocity) * 150f;
		float fadeLightColor = 1f;
		if (Projectile.timeLeft > 200)
		{
			fadeLightColor = 240 - Projectile.timeLeft;
			fadeLightColor /= 40f;
		}
		Main.spriteBatch.Draw(mainTex, drawCenter - Main.screenPosition, null, lightColor * fadeLightColor, Projectile.rotation, mainTex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
		drawCenter = Projectile.Center + Vector2.Normalize(Projectile.velocity) * 60f;
		float timeValue = 1f;
		if (Projectile.timeLeft < 60)
		{
			timeValue = Projectile.timeLeft / 60f;
		}
		Vector2 vel = Vector2.Normalize(Projectile.velocity);
		Vector2 width = vel.RotatedBy(MathF.PI * 0.5) * 90 * timeValue;
		Color drawColor = new Color(10, 255, 255, 0);
		int trailLength = 24;
		if ((240 - Projectile.timeLeft) / 3 < 24)
		{
			trailLength = (240 - Projectile.timeLeft) / 3;
		}
		if (24 - collisionTimer * 0.5f < trailLength)
		{
			trailLength = 24 - collisionTimer / 2;
			if (trailLength < 10)
			{
				trailLength = 10;
			}
		}

		float timeEffectValue = (float)(Main.time * 0.03f);
		List<Vertex2D> bars = new List<Vertex2D>();
		for (int x = 0; x < trailLength; x++)
		{
			float value = 1;
			if (x > trailLength - 17)
			{
				value = (trailLength - 1 - x) / 16f;
			}
			bars.Add(drawCenter - vel * 17 * x + width, drawColor * value, new Vector3(x / 1000f - timeEffectValue, 1, MathF.Sin(x / 48f)));
			bars.Add(drawCenter - vel * 17 * x - width, drawColor * value, new Vector3(x / 1000f - timeEffectValue, 0, MathF.Sin(x / 48f)));
		}

		List<Vertex2D> barsDark = new List<Vertex2D>();
		for (int x = 0; x < trailLength; x++)
		{
			float value = 1;
			if (x > trailLength - 17)
			{
				value = (trailLength - 1 - x) / 16f;
			}
			barsDark.Add(drawCenter - vel * 17 * x + width, Color.White * value, new Vector3(x / 1000f - timeEffectValue, 1, MathF.Sin(x / 48f)));
			barsDark.Add(drawCenter - vel * 17 * x - width, Color.White * value, new Vector3(x / 1000f - timeEffectValue, 0, MathF.Sin(x / 48f)));
		}

		List<Vertex2D> bars2 = new List<Vertex2D>();
		for (int x = 0; x < trailLength; x++)
		{
			float value = 1;
			if (x > trailLength - 20)
			{
				value = (trailLength - 1 - x) / 19f;
			}
			bars2.Add(drawCenter - vel * 5 * x + width * 0.6f, drawColor * value, new Vector3(x / 48f - timeEffectValue * 1.2f, 1, MathF.Sin(x / 72f)));
			bars2.Add(drawCenter - vel * 5 * x - width * 0.6f, drawColor * value, new Vector3(x / 48f - timeEffectValue * 1.2f, 0, MathF.Sin(x / 72f)));
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
		Effect effect = Commons.ModAsset.StabSwordEffect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.Parameters["uProcession"].SetValue(0.5f);
		effect.CurrentTechnique.Passes[0].Apply();
		Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail_2_black_thick.Value;
		Main.graphics.graphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, barsDark.ToArray(), 0, barsDark.Count - 2);

		Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail_1.Value;
		Main.graphics.graphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars2.ToArray(), 0, bars2.Count - 2);

		Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail_2.Value;
		Main.graphics.graphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		return false;
	}

	public static int CyanStrike = 0;

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		CyanStrike = 1;
		Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), target.Center, Vector2.Zero, ModContent.ProjectileType<XiaoHit>(), 0, 0, Projectile.owner, 0.45f);
	}

	public override void Load()
	{
		On_CombatText.NewText_Rectangle_Color_string_bool_bool += CombatText_NewText_Rectangle_Color_string_bool_bool;
	}

	private int CombatText_NewText_Rectangle_Color_string_bool_bool(On_CombatText.orig_NewText_Rectangle_Color_string_bool_bool orig, Rectangle location, Color color, string text, bool dramatic, bool dot)
	{
		if (CyanStrike > 0)
		{
			color = new Color(0, 255, 174);
			CyanStrike--;
		}
		return orig(location, color, text, dramatic, dot);
	}
}

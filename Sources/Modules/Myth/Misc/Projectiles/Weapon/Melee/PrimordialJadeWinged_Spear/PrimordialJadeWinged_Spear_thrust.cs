using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.PrimordialJadeWinged_Spear;

public class PrimordialJadeWinged_Spear_thrust : ModProjectile, IWarpProjectile
{
	public override string Texture => "Everglow/Myth/Misc/Projectiles/Weapon/Melee/PrimordialJadeWinged_Spear/PrimordialJadeWinged_Spear";

	public override void SetDefaults()
	{
		Projectile.extraUpdates = 12;
		Projectile.timeLeft = 240;
		Projectile.width = 80;
		Projectile.height = 80;
		Projectile.tileCollide = false;
		Projectile.friendly = true;
		Projectile.penetrate = -1;
	}

	public Vector2 PlayerSafePos = default(Vector2);

	public override void OnSpawn(IEntitySource source)
	{
		Player player = Main.player[Projectile.owner];
		Projectile.extraUpdates = (int)(12 * player.meleeSpeed);
		PlayerSafePos = player.position;
		Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 8;

		SoundEngine.PlaySound(SoundID.Item71);
	}

	public override bool PreAI()
	{
		Player player = Main.player[Projectile.owner];
		Projectile.rotation = MathF.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathF.PI / 4f;
		Projectile.velocity *= 0.995f;
		player.heldProj = Projectile.whoAmI;
		player.fullRotation = MathF.Sin((240 - Projectile.timeLeft) / 240f * MathF.PI) * Math.Sign(Projectile.velocity.X) * 1.26f;
		player.fullRotationOrigin = new Vector2(player.Hitbox.Width / 2f, player.Hitbox.Height / 2f);
		player.immune = true;
		player.immuneTime = 5;
		player.noFallDmg = true;
		if (Projectile.timeLeft > 1)
		{
			player.Center = Projectile.Center - Vector2.Normalize(Projectile.velocity) * (240 - Projectile.timeLeft);
		}
		if (!Collision.SolidCollision(player.position, player.Hitbox.Width, player.Hitbox.Height))
		{
			PlayerSafePos = player.position;
		}
		if (Collision.SolidCollision(Projectile.Center, 1, 1))
		{
			Projectile.velocity *= 0.9f;
			if (Projectile.timeLeft % 30 == 1)
			{
				for (int h = 0; h < 18; h++)
				{
					Vector2 v = Projectile.Center - Vector2.Normalize(Projectile.velocity) * 16 * h;
					if (Collision.SolidCollision(v, 1, 1))
					{
						Collision.HitTiles(v, Projectile.velocity * 20, 16, 16);
					}
				}
			}
			collisionTimer++;
		}
		if (Projectile.timeLeft < 6)
		{
			player.velocity *= 0f;
			Projectile.velocity *= 0.4f;
		}
		if (Main.rand.Next(240) < Projectile.timeLeft)
		{
			if (Main.rand.NextBool(9))
			{
				GenerateVFX();
			}
			if (Main.rand.NextBool(3))
			{
				GenerateSpark();
			}
		}
		if (Projectile.timeLeft % 60 == 0)
		{
			var v = Vector2.Normalize(Projectile.velocity);
			int h = Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<XiaoBlackWave>(), 0, 0, player.whoAmI, Math.Clamp(Projectile.velocity.Length() / 4f, 0f, 4f), 0);
			Main.projectile[h].rotation = (float)(Math.Atan2(v.Y, v.X) + Math.PI / 2d);
		}

		return false;
	}

	public override void OnKill(int timeLeft)
	{
		Player player = Main.player[Projectile.owner];
		player.fullRotation = 0;
		player.position = PlayerSafePos;
	}

	private void GenerateVFX()
	{
		Vector2 velocityLeft = Vector2.Normalize(Projectile.velocity).RotatedBy(-MathHelper.PiOver2);
		var positionVFX = Projectile.Center + velocityLeft * Main.rand.NextFloat(-30f, 30f) + Vector2.Normalize(Projectile.velocity) * Main.rand.NextFloat(Projectile.timeLeft - 120, Projectile.timeLeft + 60);
		if (Collision.SolidCollision(Projectile.Center, 1, 1))
		{
			positionVFX = Projectile.Center + velocityLeft * Main.rand.NextFloat(-40f, 40f) + Vector2.Normalize(Projectile.velocity) * Main.rand.NextFloat(-480f, 0f);
		}

		var filthy = new FilthyLucreFlame_darkDust
		{
			velocity = Projectile.velocity * 1.5f + velocityLeft * Main.rand.NextFloat(-10f, 10f),
			Active = true,
			Visible = true,
			position = positionVFX,
			maxTime = Main.rand.Next(17, 26),
			ai = new float[] { Main.rand.NextFloat(0.1f, 1f), Main.rand.NextFloat(-0.004f, 0.004f), Main.rand.NextFloat(18f, 30f) },
		};
		Ins.VFXManager.Add(filthy);
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
		Vector2 normalized = Vector2.Normalize(Projectile.velocity.RotatedBy(Math.PI * 0.5)) * 60f * timeValue;
		Vector2 start = Projectile.Center + Vector2.Normalize(Projectile.velocity) * 150;
		Vector2 end = Projectile.Center - Vector2.Normalize(Projectile.velocity) * 750;
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
		Main.spriteBatch.Draw(mainTex, drawCenter - Main.screenPosition, null, lightColor, Projectile.rotation, mainTex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
		drawCenter = Projectile.Center + Vector2.Normalize(Projectile.velocity) * 60f;
		float timeValue = 1f;
		if (Projectile.timeLeft < 60)
		{
			timeValue = Projectile.timeLeft / 60f;
		}
		Vector2 vel = Vector2.Normalize(Projectile.velocity);
		Vector2 width = vel.RotatedBy(MathF.PI * 0.5) * 150 * timeValue;
		Color drawColor = new Color(10, 255, 255, 0);
		int trailLength = 48;
		if ((240 - Projectile.timeLeft) / 3 < 48)
		{
			trailLength = (240 - Projectile.timeLeft) / 3;
		}
		if (48 - collisionTimer * 0.5f < trailLength)
		{
			trailLength = 48 - collisionTimer / 2;
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
			bars.Add(drawCenter - vel * 17 * x + width, drawColor * value, new Vector3(x / 1000f - timeEffectValue, 1, MathF.Sin(x / 64f)));
			bars.Add(drawCenter - vel * 17 * x - width, drawColor * value, new Vector3(x / 1000f - timeEffectValue, 0, MathF.Sin(x / 64f)));
		}

		List<Vertex2D> barsDark = new List<Vertex2D>();
		for (int x = 0; x < trailLength; x++)
		{
			float value = 1;
			if (x > trailLength - 17)
			{
				value = (trailLength - 1 - x) / 16f;
			}
			barsDark.Add(drawCenter - vel * 17 * x + width, Color.White * value, new Vector3(x / 1000f - timeEffectValue, 1, MathF.Sin(x / 64f)));
			barsDark.Add(drawCenter - vel * 17 * x - width, Color.White * value, new Vector3(x / 1000f - timeEffectValue, 0, MathF.Sin(x / 64f)));
		}

		List<Vertex2D> bars2 = new List<Vertex2D>();
		for (int x = 0; x < trailLength; x++)
		{
			float value = 1;
			if (x > trailLength - 10)
			{
				value = (trailLength - 1 - x) / 9f;
			}
			bars2.Add(drawCenter - vel * 20 * x + width * 0.16f, drawColor * value, new Vector3(x / 48f - timeEffectValue * 1.2f, 1, MathF.Sin(x / 48f)));
			bars2.Add(drawCenter - vel * 20 * x - width * 0.16f, drawColor * value, new Vector3(x / 48f - timeEffectValue * 1.2f, 0, MathF.Sin(x / 48f)));
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

		Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail_0.Value;
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

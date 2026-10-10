using Everglow.Myth.Common;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.PrimordialJadeWinged_Spear;

public class PrimordialJadeWinged_SpearDown : ModProjectile, IWarpProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeProjectiles;

	public override string Texture => "Everglow/Myth/Misc/Projectiles/Weapon/Melee/PrimordialJadeWinged_Spear/PrimordialJadeWinged_Spear";

	public override void SetDefaults()
	{
		Projectile.CloneDefaults(ProjectileID.Spear);
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 80;
		Projectile.extraUpdates = 12;
		Projectile.width = 80;
		Projectile.height = 80;
		Projectile.timeLeft = 240;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 90;
	}

	public bool Crash = false;
	private Vector2 stopPoint = Vector2.Zero;

	public override void OnSpawn(IEntitySource source)
	{
		Player player = Main.player[Projectile.owner];
		Projectile.extraUpdates = (int)(12 * player.meleeSpeed);
	}

	public override bool PreAI()
	{
		Player player = Main.player[Projectile.owner];
		if (player.direction == -Math.Sign(Projectile.velocity.X))
		{
			player.direction *= -1;
		}

		player.heldProj = Projectile.whoAmI;

		Projectile.rotation = MathF.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathHelper.PiOver4;
		if (Projectile.timeLeft % 10 == 1 && Projectile.timeLeft > 30 && !Crash)
		{
			var v = Vector2.Normalize(Projectile.velocity);
			int h = Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<XiaoBlackWave>(), 0, 0, player.whoAmI, Math.Clamp(Projectile.velocity.Length() / 8f, 0f, 4f), 0);
			Main.projectile[h].rotation = (float)(Math.Atan2(v.Y, v.X) + Math.PI / 2d);
		}

		Projectile.velocity *= 0.995f;
		if (Main.rand.NextBool(9))
		{
			GenerateVFX();
		}
		if (Main.rand.NextBool(4))
		{
			GenerateSpark();
		}
		bool shouldStop = false;

		for (int t = -8; t < Projectile.velocity.Length() + 8; t += 1)
		{
			if (TileUtils.PlatformCollision(Projectile.Center + Vector2.Normalize(Projectile.velocity) * t))
			{
				if (stopPoint == Vector2.zeroVector)
				{
					stopPoint = Projectile.Center + Vector2.Normalize(Projectile.velocity) * (t - 2);
				}
				shouldStop = true;
				break;
			}
		}
		if (Crash)
		{
			shouldStop = true;
		}
		if (shouldStop)
		{
			Projectile.Center = stopPoint;
			Projectile.velocity *= 0.5f;
			Projectile.timeLeft -= 1;
			Projectile.extraUpdates = 0;
			if (Projectile.timeLeft % 30 == 1)
			{
				for (int h = 0; h < 18; h++)
				{
					Vector2 v = Projectile.Center - Vector2.Normalize(Projectile.velocity) * 16 * h;
					Vector2 coordV = v / 16f;
					Tile tile2 = Main.tile[(int)coordV.X, (int)coordV.Y];
					if (TileID.Sets.Platforms[tile2.TileType])
					{
						Collision.HitTiles(v, Projectile.velocity * 20, 16, 16);
					}
				}
			}
			if (!Crash)
			{
				player.Center = stopPoint - new Vector2(0, 40 * player.gravDir);
				for (int f = 0; f < 8; f++)
				{
					Vector2 Pos = Projectile.Center + new Vector2((f - 3.5f) * 80, -300 * player.gravDir);
					bool empty = false;
					for (int i = 0; i < 75; i++)
					{
						if (!TileUtils.PlatformCollision(Pos))
						{
							Pos.Y += 8 * player.gravDir;
							empty = true;
						}
						else
						{
							empty = false;
						}
					}
					if (empty)
					{
						continue;
					}

					Projectile p = Projectile.NewProjectileDirect(Terraria.Entity.InheritSource(Projectile), Pos, Vector2.Zero, ModContent.ProjectileType<PrimordialJadeWinged_SpearSpice>(), (int)(Projectile.damage * Main.rand.NextFloat(0.85f, 1.15f)), Projectile.knockBack, player.whoAmI, Main.rand.NextFloat(50f, 110f), Main.rand.NextFloat(0f, 1f), Main.rand.NextFloat(0f, 1f));
					p.timeLeft = Main.rand.Next(128, 132);
					if (player.gravDir == 1)
					{
						p.rotation = Main.rand.NextFloat((f - 3.5f) / 15f - 0.3f, (f - 3.5f) / 15f + 0.3f);
					}
					else
					{
						p.rotation = MathF.PI - Main.rand.NextFloat((f - 3.5f) / 15f - 0.3f, (f - 3.5f) / 15f + 0.3f);
					}
				}

				for (int f = 0; f < 12; f++)
				{
					Vector2 Pos = Projectile.Center + new Vector2((f - 5.5f) * 50, -300 * player.gravDir);
					bool empty = false;
					for (int i = 0; i < 75; i++)
					{
						if (!TileUtils.PlatformCollision(Pos))
						{
							Pos.Y += 8 * player.gravDir;
							empty = true;
						}
						else
						{
							empty = false;
						}
					}
					if (empty)
					{
						continue;
					}

					int h = Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Pos, Vector2.Zero, ModContent.ProjectileType<PrimordialJadeWinged_SpearShake>(), 0, Projectile.knockBack, player.whoAmI, Main.rand.NextFloat(17f, 32f) * (Math.Abs(f - 5.5f) + 0.5f), 0);

					if (player.gravDir == 1)
					{
						Main.projectile[h].rotation = Main.rand.NextFloat((f - 5.5f) / 15f - 0.1f + Math.Sign(f - 5.5f) * 0.75f, (f - 5.5f) / 15f + 0.1f + Math.Sign(f - 5.5f) * 0.75f);
					}
					else
					{
						Main.projectile[h].rotation = MathF.PI - Main.rand.NextFloat((f - 5.5f) / 15f - 0.1f + Math.Sign(f - 5.5f) * 0.75f, (f - 5.5f) / 15f + 0.1f + Math.Sign(f - 5.5f) * 0.75f);
					}
				}
				int times = 24;
				for (int f = 0; f <= times; f++)
				{
					Vector2 newVec = new Vector2((f - times / 2) / 1f, -10).RotatedByRandom(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.NextFloat(1.4f, 2.3f);
					newVec.Y *= player.gravDir;
					var positionVFX = Projectile.Center + new Vector2((f - times / 2) * 24, -60 * player.gravDir) + newVec * Main.rand.NextFloat(0.7f, 1f);
					for (int i = 0; i < 75; i++)
					{
						if (!TileUtils.PlatformCollision(positionVFX))
						{
							positionVFX.Y += 12 * player.gravDir;
						}
						else
						{
							break;
						}
					}
					positionVFX.Y += 60 * player.gravDir;
					var filthy = new FilthyLucreFlame_darkDust
					{
						velocity = newVec,
						Active = true,
						Visible = true,
						position = positionVFX,
						maxTime = Main.rand.Next(17, 26),
						ai = new float[] { Main.rand.NextFloat(0.1f, 1f), Main.rand.NextFloat(-0.08f, 0.08f), Main.rand.NextFloat(18f, 60f) },
					};
					Ins.VFXManager.Add(filthy);
					var filthy2 = new FilthyLucreFlameDust
					{
						velocity = newVec,
						Active = true,
						Visible = true,
						position = positionVFX,
						maxTime = Main.rand.Next(17, 26),
						ai = new float[] { Main.rand.NextFloat(0.1f, 1f), Main.rand.NextFloat(-0f, 0f), Main.rand.NextFloat(18f, 40f) },
					};
					Ins.VFXManager.Add(filthy2);
					var smog = new FilthyFragileDust
					{
						velocity = -new Vector2(0, MathF.Sin(f / (float)times * MathF.PI)) * Main.rand.NextFloat(10f, 60f),
						Active = true,
						Visible = true,
						position = positionVFX - new Vector2(0, 80),
						coord = new Vector2(Main.rand.NextFloat(1f), Main.rand.NextFloat(1f)),
						maxTime = Main.rand.Next(60, 165),
						scale = Main.rand.NextFloat(Main.rand.NextFloat(8.4f, 10.4f), 14f),
						rotation = Main.rand.NextFloat(6.283f),
						rotation2 = Main.rand.NextFloat(6.283f),
						omega = Main.rand.NextFloat(-30f, 30f),
						phi = Main.rand.NextFloat(6.283f),
						ai = new float[] { Main.rand.NextFloat(0f, 0.2f), Main.rand.NextFloat(0.2f, 0.5f) },
					};
					Ins.VFXManager.Add(smog);
				}
				ShakerManager.AddShaker(Projectile.Center + new Vector2(0, 50), Projectile.velocity, 120, 120f, 65, 0.9f, 0.8f, 5);
				SoundEngine.PlaySound(new SoundStyle("Everglow/Myth/Sounds/Xiao"), Projectile.Center);

				Crash = true;
			}
		}
		else
		{
			player.Center = Projectile.Center - new Vector2(0, 80 * player.gravDir);
			if (Projectile.velocity.Length() < 100)
			{
				Projectile.velocity *= 1.05f;
			}
		}
		if (Projectile.timeLeft > 6 && Crash)
		{
			Projectile.timeLeft -= 4;
		}

		if (Projectile.timeLeft < 6)
		{
			player.velocity *= 0.4f;
			Projectile.velocity *= 0.4f;
		}
		player.noFallDmg = true;
		MythContentPlayer myplayer = player.GetModPlayer<MythContentPlayer>();
		myplayer.InvincibleFrameTime = 15;
		return false;
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
			velocity = Vector2.Normalize(Projectile.velocity) * 15f + velocityLeft * Main.rand.NextFloat(-10f, 10f),
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
		var positionVFX = Projectile.Center + velocityLeft * Main.rand.NextFloat(-90f, 90f) + Vector2.Normalize(Projectile.velocity) * Main.rand.NextFloat(Projectile.timeLeft - 120, Projectile.timeLeft + 0);
		if (TileUtils.PlatformCollision(Projectile.Center))
		{
			positionVFX = Projectile.Center + velocityLeft * Main.rand.NextFloat(-90f, 90f) + Vector2.Normalize(Projectile.velocity) * Main.rand.NextFloat(-480f, 0f);
		}
		var smog = new FilthyFragileDust
		{
			velocity = Vector2.Normalize(Projectile.velocity) * 15f * Main.rand.NextFloat(1.4f, 2.4f),
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

	public override bool PreDraw(ref Color lightColor)
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
		Vector2 width = vel.RotatedBy(MathF.PI * 0.5) * 160 * timeValue;
		Color drawColor = new Color(10, 255, 255, 0);
		int trailLength = 72;
		if ((240 - Projectile.timeLeft) / 3 < 72)
		{
			trailLength = (240 - Projectile.timeLeft) / 3;
		}
		if (72 - collisionTimer * 0.5f < trailLength)
		{
			trailLength = 72 - collisionTimer / 2;
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
			float width2 = 1 - Math.Min(x / 35f, 0.5f);

			bars.Add(drawCenter - vel * 17 * x + width, drawColor * value, new Vector3(x / 1000f - timeEffectValue, 1, width2));
			bars.Add(drawCenter - vel * 17 * x - width, drawColor * value, new Vector3(x / 1000f - timeEffectValue, 0, width2));
		}

		List<Vertex2D> barsDark = new List<Vertex2D>();
		for (int x = 0; x < trailLength; x++)
		{
			float value = 1;
			if (x > trailLength - 17)
			{
				value = (trailLength - 1 - x) / 16f;
			}
			float width2 = 1 - Math.Min(x / 35f, 0.5f);

			barsDark.Add(drawCenter - vel * 17 * x + width, Color.White * value, new Vector3(x / 1000f - timeEffectValue, 1, width2));
			barsDark.Add(drawCenter - vel * 17 * x - width, Color.White * value, new Vector3(x / 1000f - timeEffectValue, 0, width2));
		}

		List<Vertex2D> bars2 = new List<Vertex2D>();
		for (int x = 0; x < trailLength; x++)
		{
			float value = 1;
			if (x > trailLength - 20)
			{
				value = (trailLength - 1 - x) / 19f;
			}
			bars2.Add(drawCenter - vel * 45 * x + width * 0.6f, drawColor * value, new Vector3(x / 48f - timeEffectValue * 1.2f, 1, MathF.Sin(x / 216f)));
			bars2.Add(drawCenter - vel * 45 * x - width * 0.6f, drawColor * value, new Vector3(x / 48f - timeEffectValue * 1.2f, 0, MathF.Sin(x / 216f)));
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

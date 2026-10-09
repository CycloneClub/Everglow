using Everglow.Commons.Vertex;
using Everglow.Commons.Coroutines;
using Everglow.Ocean.VFXs;
using Terraria.DataStructures;
using SteelSeries.GameSense;
using Terraria.Audio;

namespace Everglow.Ocean.Projectiles.Weapons;

public class TsunamiShark_missile : ModProjectile
{
	private CoroutineManager _coroutineManager = new CoroutineManager();
	public override string Texture => "Everglow/Ocean/Projectiles/Weapons/TsunamiShark/TsunamiShark_missile";
	public override void SetDefaults()
	{
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.DamageType = DamageClass.Ranged;
		Projectile.aiStyle = -1;
		Projectile.penetrate = 1;
		Projectile.timeLeft = 3600000;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 20;
		Projectile.extraUpdates = 1;
		Projectile.tileCollide = false;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 30;
	}
	internal int TimeTokill = -1;
	public override void OnSpawn(IEntitySource source)
	{
		_coroutineManager.StartCoroutine(new Coroutine(Task()));
	}
	public override void AI()
	{
		_coroutineManager.Update();
		if (TimeTokill >= 0 && TimeTokill <= 2)
		{
			Projectile.Kill();
		}

		if (TimeTokill <= 15 && TimeTokill > 0)
		{
			Projectile.velocity = Projectile.oldVelocity;
		}

		TimeTokill--;
		if (TimeTokill >= 0)
		{
			if (TimeTokill < 10)
			{
				Projectile.damage = 0;
				Projectile.friendly = false;
			}
			Projectile.velocity *= 0f;
		}
	}
	private IEnumerator<ICoroutineInstruction> Task()
	{
		_coroutineManager.StartCoroutine(new Coroutine(Chase()));
		_coroutineManager.StartCoroutine(new Coroutine(Rotation()));
		_coroutineManager.StartCoroutine(new Coroutine(DecreasingSpeed()));
		_coroutineManager.StartCoroutine(new Coroutine(GenerateDust()));
		yield return new WaitForFrames(5);
	}
	private IEnumerator<ICoroutineInstruction> DecreasingSpeed()
	{
		for (int x = 0; x < 60; x++)
		{
			Projectile.velocity *= 0.9f;
			yield return new SkipThisFrame();
		}
	}
	private IEnumerator<ICoroutineInstruction> Rotation()
	{
		while (true)
		{
			Projectile.rotation = MathF.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
			yield return new SkipThisFrame();
		}
	}
	private IEnumerator<ICoroutineInstruction> GenerateDust()
	{
		while (true)
		{
			if (Main.rand.NextBool((int)(20f / (Projectile.velocity.Length() + 1f) + 1)))
			{
				Dust dust = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.Water, 0, 0, 200, default);
				dust.velocity = Vector2.Normalize(Projectile.velocity) * 2f;
				dust.noGravity = true;
			}
			yield return new SkipThisFrame();
		}
	}
	private float maxVel = 0f;
	private NPC OldTarget = null;
	private IEnumerator<ICoroutineInstruction> Chase()
	{
		while (true)
		{
			if (maxVel < 27)
			{
				maxVel += 0.3f;
			}
			Player player = Main.player[Projectile.owner];
			Item item = player.HeldItem;
			var tsunamiS = item.ModItem as Items.Weapons.TsunamiShark;
			if (tsunamiS != null)
			{
				NPC target = tsunamiS.MarkedTarget;
				if (target != null)
				{
					if (OldTarget == null)
					{
						OldTarget = target;
						uint length = (uint)Main.rand.Next(120);
						for (int a = 0; a < length; a++)
						{
							Projectile.friendly = false;
							Vector2 swing = new Vector2(MathF.Sin((float)Main.time * 0.3f + Projectile.whoAmI) * 72 * Projectile.ai[0] * Projectile.ai[0], MathF.Sin((float)Main.time * 0.1f + Projectile.whoAmI * 0.1f) * 10 - Projectile.ai[0] * 200 + 200);
							Vector2 aimVelPlayer = player.Center + new Vector2(-player.direction * 36, -player.gravDir * 30) + swing - Projectile.Center;
							if (aimVelPlayer != Vector2.zeroVector)
							{
								aimVelPlayer = Vector2.Normalize(aimVelPlayer) * maxVel * Projectile.ai[0] * Projectile.ai[0];
							}
							Projectile.velocity = Projectile.velocity * 0.95f + aimVelPlayer * 0.05f;
							yield return new SkipThisFrame();
						}
					}
					Projectile.friendly = true;
					Vector2 aimVel = target.Center - Projectile.Center - Projectile.velocity;
					if (aimVel != Vector2.zeroVector)
					{
						aimVel = Vector2.Normalize(aimVel) * maxVel;
					}
					Projectile.velocity = Projectile.velocity * 0.95f + aimVel * 0.05f;
				}
				else
				{
					OldTarget = null;
					Projectile.friendly = false;
					Vector2 swing = new Vector2(MathF.Sin((float)Main.time * 0.3f + Projectile.whoAmI) * 72 * Projectile.ai[0] * Projectile.ai[0], MathF.Sin((float)Main.time * 0.1f + Projectile.whoAmI * 0.1f) * 10 - Projectile.ai[0] * 200 + 200);
					Vector2 aimVel = player.Center + new Vector2(-player.direction * 36, -player.gravDir * 30) + swing - Projectile.Center;
					if (aimVel != Vector2.zeroVector)
					{
						aimVel = Vector2.Normalize(aimVel) * maxVel * Projectile.ai[0] * Projectile.ai[0];
					}
					Projectile.velocity = Projectile.velocity * 0.95f + aimVel * 0.05f;

				}
			}
			else
			{
				OldTarget = null;
				Projectile.friendly = false;
				Vector2 swing = new Vector2(MathF.Sin((float)Main.time * 0.3f + Projectile.whoAmI) * 72 * Projectile.ai[0] * Projectile.ai[0], MathF.Sin((float)Main.time * 0.1f + Projectile.whoAmI * 0.1f) * 10 - Projectile.ai[0] * 200 + 200);
				Vector2 aimVel = player.Center + new Vector2(-player.direction * 36, -player.gravDir * 30) + swing - Projectile.Center;
				if (aimVel != Vector2.zeroVector)
				{
					aimVel = Vector2.Normalize(aimVel) * maxVel * Projectile.ai[0] * Projectile.ai[0];
				}
				Projectile.velocity = Projectile.velocity * 0.95f + aimVel * 0.05f;
				if (Main.rand.NextBool(60))
				{
					AmmoHit();
				}
			}
			yield return new SkipThisFrame();
		}
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		AmmoHit();
		return false;
	}
	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		AmmoHit();
	}
	public virtual void AmmoHit()
	{
		TimeTokill = 30;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.velocity = Projectile.oldVelocity;
		GenerateVFXKill(24);
		SoundEngine.PlaySound(SoundID.Splash, Projectile.Center);
		Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<TsunamiShark_missile_hit>(), Projectile.damage / 7, Projectile.knockBack);
		SoundEngine.PlaySound(new SoundStyle("Everglow/Ocean/Sounds/WaterMissile" + Main.rand.Next(2)).WithVolumeScale(0.8f), Projectile.Center);
	}
	public void GenerateVFXKill(int Frequency)
	{
		float mulVelocity = 4.5f;
		for (int g = 0; g < Frequency; g++)
		{
			Vector2 afterVelocity = new Vector2(0, 2f).RotatedBy(g / (float)Frequency * 2 * Math.PI);
			var wave = new WaveSprayDust
			{
				velocity = afterVelocity * mulVelocity + Projectile.velocity.SafeNormalize(new Vector2(0, -1)),
				Active = true,
				Visible = true,
				position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
				maxTime = Main.rand.Next(10, 22),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0, Main.rand.NextFloat(6f, 12f) },
			};
			Ins.VFXManager.Add(wave);
		}
		mulVelocity = 2.6f;
		for (int g = 0; g < Frequency; g++)
		{
			Vector2 afterVelocity = new Vector2(0, 2f).RotatedBy(g / (float)Frequency * 2 * Math.PI);
			var wave = new WaveSprayDust
			{
				velocity = afterVelocity * mulVelocity + Projectile.velocity.SafeNormalize(new Vector2(0, -1)),
				Active = true,
				Visible = true,
				position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
				maxTime = Main.rand.Next(21, 32),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0, Main.rand.NextFloat(6f, 12f) },
			};
			Ins.VFXManager.Add(wave);
		}
	}
	public override bool PreDraw(ref Color lightColor)
	{
		return false;
	}
	public override void PostDraw(Color lightColor)
	{
		DrawWaterDarkTrail();
		DrawWaterTrail();
		Texture2D shark = ModAsset.TsunamiShark_missile.Value;
		if (TimeTokill < 0)
		{
			Lighting.AddLight(Projectile.Center, 0, 0.4f, 1f);
			Main.spriteBatch.Draw(shark, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, shark.Size() / 2f, 1f, SpriteEffects.None, 0);
		}
	}
	public void DrawWaterDarkTrail()
	{
		var bars = new List<Vertex2D>();
		int TrueL = 0;
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				if (i == 1)
				{
					return;
				}

				break;
			}

			TrueL = i;
		}
		for (int i = 1; i < TrueL; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			float width = 12;
			if (Projectile.timeLeft <= 30)
			{
				width *= Projectile.timeLeft / 30f;
			}

			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			normalDir = new Vector2(-normalDir.Y, normalDir.X).SafeNormalize(Vector2.Zero);

			var factor = i / (float)TrueL;
			var color = Color.Lerp(new Color(1f, 1f, 1f, 1f) * 0.4f, new Color(0, 0, 0, 0), factor);

			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width + new Vector2(5) - Main.screenPosition, color, new Vector3(factor, 0, 0)));
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width + new Vector2(5) - Main.screenPosition, color, new Vector3(factor, 1, 0)));
		}

		if (bars.Count > 2)
		{
			Texture2D t = ModAsset.WaterLineBlackShade.Value;
			Main.graphics.GraphicsDevice.Textures[0] = t;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
	}
	public void DrawWaterTrail()
	{
		var bars = new List<Vertex2D>();
		int TrueL = 0;
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				if (i == 1)
				{
					return;
				}

				break;
			}

			TrueL = i;
		}
		for (int i = 1; i < TrueL; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			float width = 12;
			if (Projectile.timeLeft <= 30)
			{
				width *= Projectile.timeLeft / 30f;
			}

			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			normalDir = new Vector2(-normalDir.Y, normalDir.X).SafeNormalize(Vector2.Zero);

			var factor = i / (float)TrueL;
			var color = Color.Lerp(new Color(0, (1 - factor) * 0.8f, (1 - factor * factor) * 2f, 0), new Color(0, 0, 0, 0), factor);

			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width + new Vector2(5) - Main.screenPosition, color, new Vector3(factor, 0, 0)));
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width + new Vector2(5) - Main.screenPosition, color, new Vector3(factor, 1, 0)));

		}

		if (bars.Count > 2)
		{
			Texture2D t = ModAsset.WaterLine.Value;
			Main.graphics.GraphicsDevice.Textures[0] = t;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
	}
	public void DrawSprayTrail()
	{
		var color = new Color(255, 255, 255, 0);
		var bars = new List<Vertex2D>();
		int TrueL = 0;
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				if (i == 1)
				{
					return;
				}

				break;
			}

			TrueL = i;
		}
		TrueL /= 2;
		for (int i = 1; i < TrueL; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			float width = 8;
			if (Projectile.timeLeft <= 30)
			{
				width *= Projectile.timeLeft / 30f;
			}

			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			normalDir = new Vector2(-normalDir.Y, normalDir.X).SafeNormalize(Vector2.Zero);

			var factor = i / (float)TrueL;
			width *= MathF.Sin(MathF.Sqrt(factor) * MathF.PI - 1.2f);
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width + new Vector2(5), color, new Vector3(factor, 0, 1 - factor)));
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width + new Vector2(5), color, new Vector3(factor, 1, 1 - factor)));
		}

		if (bars.Count > 2)
		{
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			Effect e = ModAsset.Powderlization.Value;
			var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
			var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;
			e.Parameters["uTransform"].SetValue(model * projection);
			e.Parameters["tex0"].SetValue(ModAsset.NoiseSandRectangle.Value);
			e.CurrentTechnique.Passes["Test"].Apply();
			Texture2D t = ModAsset.HiveCyberNoiseThicker.Value;
			Main.graphics.GraphicsDevice.Textures[0] = t;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		}
	}
}

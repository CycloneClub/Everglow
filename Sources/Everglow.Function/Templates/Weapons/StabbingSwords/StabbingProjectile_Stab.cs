using Everglow.Commons.Coroutines;
using Everglow.Commons.DataStructures;
using Everglow.Commons.MEAC;
using Everglow.Commons.Templates.Weapons.StabbingSwords.VFX;
using Everglow.Commons.TileHelper;
using Everglow.Commons.Utilities;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;
using Everglow.Commons.VFX.CommonVFXDusts;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Shaders;

namespace Everglow.Commons.Templates.Weapons.StabbingSwords;

public abstract class StabbingProjectile_Stab : ModProjectile, IWarpProjectile
{
	public override string LocalizationCategory => LocalizationUtils.Categories.MeleeProjectiles;

	/// <summary>
	/// Main color | 主要颜色
	/// </summary>
	public Color StabColor = Color.White;

	/// <summary>
	/// Shadow intensity | 阴影强度
	/// </summary>
	public float StabShade = 0f;

	/// <summary>
	/// Width of stab effect | 特效宽度
	/// </summary>
	public float StabEffectWidth = 1f;

	/// <summary>
	/// Color of spark when hit solid tile, default to new Color(1f, 0.45f, 0.05f, 0). | 撞击火花颜色，打到固体物块的时候溅出，默认new Color(1f, 0.45f, 0.05f, 0)
	/// </summary>
	public Color HitTileSparkColor = new Color(1f, 0.45f, 0.05f, 0);

	/// <summary>
	/// Stab range coefficient, multiplied by 80 | 攻击距离系数, 乘以80
	/// </summary>
	public float StabDistance = 1f;

	public Vector2 StabStartPoint_WorldPos = Vector2.Zero;
	public Vector2 StabEndPoint_WorldPos = Vector2.Zero;
	public int StabTimer = 120;

	public Player Owner => Main.player[Projectile.owner];

	/// <summary>
	/// Manager of Visual effect ring.
	/// </summary>
	private CoroutineManager _coroutineManager = new CoroutineManager();

	public override string Texture => ModAsset.StabbingProjectile_Mod;

	public override void SetDefaults()
	{
		Projectile.width = 24;
		Projectile.height = 24;
		Projectile.netImportant = true;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 15;
		Projectile.extraUpdates = 5;
		Projectile.tileCollide = false;
		Projectile.ArmorPenetration = 20;
		Projectile.melee = true;
		SetCustomDefaults();
	}

	/// <summary>
	/// Anouced that stab will default with 20 ArmorPenetration.
	/// </summary>
	public virtual void SetCustomDefaults()
	{
	}

	public override void OnSpawn(IEntitySource source)
	{
		// Calculate direction
		Vector2 toMouse = Main.MouseWorld - Owner.RotatedRelativePoint(Owner.MountedCenter);
		toMouse.Normalize();
		if (toMouse.HasNaNs())
		{
			toMouse = Vector2.UnitX * Owner.direction;
		}
		if (toMouse.X != Projectile.velocity.X || toMouse.Y != Projectile.velocity.Y)
		{
			Projectile.netUpdate = true;
		}
		Projectile.velocity = toMouse;

		// Ring effect
		_coroutineManager.StartCoroutine(new Coroutine(Generate3DRingVFX(toMouse)));

		// Dust effect
		Color dustC = Color.Lerp(StabColor, Color.White, 0.6f);
		dustC.A = 0;
		StabGasDust(toMouse, dustC);

		// Stab Sound
		var ss = new SoundStyle(ModAsset.StabbingSwordSound_Mod);
		SoundEngine.PlaySound(ss, Projectile.Center);
		StabStartPoint_WorldPos = Projectile.Center;
	}

	public virtual void StabGasDust(Vector2 velocity, Color color)
	{
		for (int i = 0; i < 6; i++)
		{
			StabLightDust v1;
			int maxTime = Main.rand.Next(12, 20);
			v1 = new StabLightDust()
			{
				Center = Projectile.Center + velocity.NormalizeSafe().RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(-12f, 12f) + velocity.NormalizeSafe() * (i * 10) * StabDistance,
				Velocity = velocity * (5 + Main.rand.NextFloatDirection() * 6f),
				EffectColor = color,
				Rotation = velocity.ToRotation() + MathHelper.PiOver2,
				Timeleft = maxTime,
				MaxTime = maxTime,
				Scale = Main.rand.NextFloat(0.24f, 0.4f),
			};
			Ins.VFXManager.Add(v1);
		}
		for (int i = 0; i < 10; i++)
		{
			StabLightDust v1;
			int maxTime = Main.rand.Next(10, 15);
			v1 = new StabLightDust()
			{
				Center = Projectile.Center + velocity.NormalizeSafe().RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(-12f, 12f) + velocity.NormalizeSafe() * (i * 10) * StabDistance,
				Velocity = velocity * (5 + Main.rand.NextFloatDirection() * 6f),
				EffectColor = color,
				Rotation = velocity.ToRotation() + MathHelper.PiOver2,
				Timeleft = maxTime,
				MaxTime = maxTime,
				Scale = Main.rand.NextFloat(0.04f, 0.07f),
			};
			Ins.VFXManager.Add(v1);
		}
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		Vector2 end = Projectile.Center + Projectile.velocity * 80 * StabDistance;
		if (StabEndPoint_WorldPos != Vector2.Zero)
		{
			end = StabEndPoint_WorldPos;
		}
		if (Collision.CanHit(StabStartPoint_WorldPos, 0, 0, targetHitbox.Center(), 0, 0))
		{
			if (CollisionUtils.Intersect(targetHitbox.Left(), targetHitbox.Right(), targetHitbox.Height, StabStartPoint_WorldPos, end, Projectile.width))
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Use Coroutine to handle ring VFX | 用协程管理环状特效
	/// </summary>
	/// <returns></returns>
	public virtual IEnumerator<ICoroutineInstruction> Generate3DRingVFX(Vector2 velocity)
	{
		yield return new WaitForFrames(40);
		var v = new StabVFX()
		{
			Position = Projectile.Center + Projectile.velocity * StabDistance * 80 * (1 - StabTimer / 135f),
			Velocity = velocity,
			StabEffectColor = Color.Lerp(StabColor, Color.Transparent, 0.4f),
			Scale = 26,
			MaxTime = (int)(240 / (float)(Projectile.extraUpdates + 1)),
			TimeLeft = (int)(240 / (float)(Projectile.extraUpdates + 1)),
		};
		if (StabEndPoint_WorldPos == Vector2.Zero)
		{
			Ins.VFXManager.Add(v);
		}
		yield return new WaitForFrames(40);
		v = new StabVFX()
		{
			Position = Projectile.Center + Projectile.velocity * StabDistance * 80 * (1 - StabTimer / 135f),
			Velocity = velocity,
			StabEffectColor = Color.Lerp(StabColor, Color.Transparent, 0.56f),
			Scale = 15,
			MaxTime = (int)(240 / (float)(Projectile.extraUpdates + 1)),
			TimeLeft = (int)(240 / (float)(Projectile.extraUpdates + 1)),
		};
		if (StabEndPoint_WorldPos == Vector2.Zero)
		{
			Ins.VFXManager.Add(v);
		}
	}

	public override void AI()
	{
		// Water interaction
		ProduceWaterRipples(new Vector2(Projectile.velocity.Length(), 30));

		// Ring effect coroutine update.
		_coroutineManager.Update();

		// Set player posture
		if (Projectile.timeLeft <= 1)
		{
			StabTimer--;
			if (StabTimer > 0)
			{
				Projectile.timeLeft++;
				float value = (Projectile.timeLeft + StabTimer) / 135f;
				float bodyRotation = MathF.Sin(value * MathF.PI) * Owner.direction * 0.2f;
				TestPlayerDrawer playerDrawer = Owner.GetModPlayer<TestPlayerDrawer>();
				playerDrawer.HeadRotation = 0;
				playerDrawer.HideLeg = true;
				Owner.headRotation = -bodyRotation;
				playerDrawer.HeadRotation = Owner.headRotation;
				Owner.fullRotation = bodyRotation;
				Owner.fullRotationOrigin = new Vector2(Owner.Hitbox.Width / 2f, Owner.gravDir == -1 ? 0 : Owner.Hitbox.Height);
			}
			else
			{
				TestPlayerDrawer playerDrawer = Owner.GetModPlayer<TestPlayerDrawer>();
				Owner.legFrame = new Rectangle(0, 0, Owner.legFrame.Width, Owner.legFrame.Height);
				Owner.fullRotation = 0;
				Owner.legRotation = 0;
				playerDrawer.HeadRotation = 0;
				playerDrawer.HideLeg = false;
				Owner.legPosition = Vector2.Zero;
			}
		}

		// Set projectile posture
		if (StabTimer >= 120)
		{
			Projectile.position = Owner.RotatedRelativePoint(Owner.MountedCenter, reverseRotation: false, addGfxOffY: false) - Projectile.Size / 2f + Projectile.velocity * (15 - Projectile.timeLeft) * 2;
			Projectile.rotation = Projectile.velocity.ToRotation();
			Projectile.spriteDirection = Projectile.direction;
		}
		else
		{
			Projectile.extraUpdates = 24;
		}
		if (StabEndPoint_WorldPos == Vector2.Zero)
		{
			float lengthDetect = Projectile.velocity.Length() * 100 * StabDistance;
			for (int k = 0; k < lengthDetect; k++)
			{
				Vector2 modifiedEnd = Projectile.Center + Projectile.velocity.NormalizeSafe() * k;
				if (StabbingProjectile.SolidTileButNotSolidTop(modifiedEnd))
				{
					StabEndPoint_WorldPos = modifiedEnd;
					Projectile.Center = modifiedEnd;
					Projectile.velocity *= 0.01f;
					HitTile();
					break;
				}
			}
		}
	}

	public override void OnKill(int timeLeft)
	{
		if (Owner.GetModPlayer<StabbingSwordStaminaPlayer>().StaminaRecovering)
		{
			OnStaminaDepleted(Owner);
		}
	}

	/// <summary>
	/// Handle what happens before the projectile is killed by stamina depletion.
	/// <para/>Called in <see cref="ModProjectile.Kill(int)"/> when the player runs out of stamina.
	/// </summary>
	public virtual void OnStaminaDepleted(Player player)
	{
	}

	public virtual void HitTile()
	{
		SoundStyle ss = SoundID.NPCHit4;
		SoundEngine.PlaySound(ss.WithPitchOffset(Main.rand.NextFloat(-0.4f, 0.4f)), Projectile.Center);
		for (int g = 0; g < 20; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(2f, 6f)).RotatedByRandom(MathHelper.TwoPi);
			var spark = new FireSpark_MetalStabDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = StabEndPoint_WorldPos,
				MaxTime = Main.rand.Next(1, 25),
				Scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(10f, 27.0f)),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.13f, 0.13f) },
			};
			Ins.VFXManager.Add(spark);
		}
		var hitSparkFixed = new StabbingProjectile_HitEffect()
		{
			Active = true,
			Visible = true,
			Position = StabEndPoint_WorldPos,
			MaxTime = 16,
			Scale = 0.24f,
			Rotation = Projectile.velocity.ToRotation(),
			Color = HitTileSparkColor,
		};
		Ins.VFXManager.Add(hitSparkFixed);

		Vector2 tilePos = StabEndPoint_WorldPos + new Vector2(1, 0).RotatedBy(Projectile.velocity.ToRotation());
		Point tileCoord = tilePos.ToTileCoordinates();
		Tile tile = WorldGenMisc.SafeGetTile(tileCoord);
		if (TileUtils.Sets.TileFragile[tile.TileType])
		{
			WorldGenMisc.DamageTile(tileCoord, 100);
		}
	}

	private void ProduceWaterRipples(Vector2 beamDims)
	{
		Vector2 mainVec = Projectile.velocity;
		var shaderData = (WaterShaderData)Terraria.Graphics.Effects.Filters.Scene["WaterDistortion"].GetShader();
		float waveSine = 1f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 20f);
		Vector2 ripplePos = Projectile.Center + new Vector2(beamDims.X * 0.5f, 0f).RotatedBy(mainVec.ToRotation());
		Color waveData = new Color(0.5f, 0.1f * Math.Sign(waveSine) + 0.5f, 0f, 1f) * Math.Abs(waveSine);
		shaderData.QueueRipple(ripplePos, waveData, beamDims, RippleShape.Square, mainVec.ToRotation());
	}

	public override bool PreDraw(ref Color lightColor)
	{
		DrawItem(lightColor);
		return false;
	}

	public virtual void DrawItem(Color lightColor)
	{
		if (StabTimer > 60)
		{
			Texture2D itemTexture = TextureAssets.Item[Owner.HeldItem.type].Value;
			Main.spriteBatch.Draw(itemTexture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation + MathF.PI * 0.25f, itemTexture.Size() / 2f, 1, SpriteEffects.None, 0f);
		}
	}

	public override void PostDraw(Color lightColor)
	{
		DrawEffect(lightColor);
	}

	public virtual void DrawEffect(Color lightColor)
	{
		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Vector2 normalized = Vector2.Normalize(Projectile.velocity.RotatedBy(Math.PI * 0.5)) * 90 * StabTimer / 120f * StabEffectWidth;
		Vector2 start = StabStartPoint_WorldPos;
		Vector2 end = Projectile.Center + Projectile.velocity * 100 * StabDistance;
		if (StabEndPoint_WorldPos != Vector2.Zero)
		{
			end = StabEndPoint_WorldPos;
		}
		float value = (Projectile.timeLeft + StabTimer) / 135f;
		var middle = Vector2.Lerp(end, start, MathF.Sqrt(value) * 0.5f);
		float time = (float)(Main.time * 0.03);
		float dark = MathF.Sin(value * MathF.PI) * 4;
		var bars = new List<Vertex2D>
		{
			new Vertex2D(start + normalized, new Color(0, 0, 0, 120) * StabShade, new Vector3(1 + time, 0, 0)),
			new Vertex2D(start - normalized, new Color(0, 0, 0, 120) * StabShade, new Vector3(1 + time, 1, 0)),
			new Vertex2D(middle + normalized, Color.White * 0.4f * dark * StabShade, new Vector3(0.5f + time, 0, 0.5f)),
			new Vertex2D(middle - normalized, Color.White * 0.4f * dark * StabShade, new Vector3(0.5f + time, 1, 0.5f)),
			new Vertex2D(end + normalized, Color.White * 0.9f * dark * StabShade, new Vector3(0f + time, 0, 1)),
			new Vertex2D(end - normalized, Color.White * 0.9f * dark * StabShade, new Vector3(0f + time, 1, 1)),
		};
		if (bars.Count >= 3)
		{
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
			Effect effect = ModAsset.StabSwordEffect.Value;
			var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
			var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
			effect.Parameters["uTransform"].SetValue(model * projection);
			effect.Parameters["uProcession"].SetValue(0.21f);
			effect.CurrentTechnique.Passes[0].Apply();
			Main.graphics.graphicsDevice.Textures[0] = ModAsset.Trail_black.Value;
			Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(sBS);
		}
		Color alphaColor = StabColor;
		alphaColor.A = 0;
		alphaColor.R = (byte)(alphaColor.R * lightColor.R / 255f);
		alphaColor.G = (byte)(alphaColor.G * lightColor.G / 255f);
		alphaColor.B = (byte)(alphaColor.B * lightColor.B / 255f);

		normalized = Vector2.Normalize(Projectile.velocity.RotatedBy(Math.PI * 0.5)) * 72 * StabTimer / 120f * StabEffectWidth;
		bars = new List<Vertex2D>
		{
			new Vertex2D(start + normalized, new Color(0, 0, 0, 0), new Vector3(1 + time, 0, 0)),
			new Vertex2D(start - normalized, new Color(0, 0, 0, 0), new Vector3(1 + time, 1, 0)),
			new Vertex2D(middle + normalized, alphaColor, new Vector3(0.5f + time, 0, 0.5f)),
			new Vertex2D(middle - normalized, alphaColor, new Vector3(0.5f + time, 1, 0.5f)),
			new Vertex2D(end + normalized, alphaColor * 1.2f, new Vector3(0f + time, 0, 1)),
			new Vertex2D(end - normalized, alphaColor * 1.2f, new Vector3(0f + time, 1, 1)),
		};
		if (bars.Count >= 3)
		{
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
			Effect effect = ModAsset.StabSwordEffect.Value;
			var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
			var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
			effect.Parameters["uTransform"].SetValue(model * projection);
			effect.Parameters["uProcession"].SetValue(value * 1.1f);
			effect.CurrentTechnique.Passes[0].Apply();
			Main.graphics.graphicsDevice.Textures[0] = ModAsset.Trail_1.Value;
			Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(sBS);
		}

		alphaColor.A = 0;
		alphaColor.R = (byte)(StabColor.R * 2 * lightColor.R / 255f);
		alphaColor.G = (byte)(StabColor.G * 2 * lightColor.G / 255f);
		alphaColor.B = (byte)(StabColor.B * 2 * lightColor.B / 255f);
		normalized = Vector2.Normalize(Projectile.velocity.RotatedBy(Math.PI * 0.5)) * 24 * StabTimer / 120f * StabEffectWidth;
		bars = new List<Vertex2D>
		{
			new Vertex2D(start + normalized, alphaColor * 0.4f, new Vector3(1 + time, 0, 0)),
			new Vertex2D(start - normalized, alphaColor * 0.4f, new Vector3(1 + time, 1, 0)),
			new Vertex2D(middle + normalized, alphaColor, new Vector3(0.5f + time, 0, 0.5f)),
			new Vertex2D(middle - normalized, alphaColor, new Vector3(0.5f + time, 1, 0.5f)),
			new Vertex2D(end + normalized, alphaColor * 2, new Vector3(0f + time, 0, 1)),
			new Vertex2D(end - normalized, alphaColor * 2, new Vector3(0f + time, 1, 1)),
		};
		if (bars.Count >= 3)
		{
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
			Effect effect = ModAsset.StabSwordEffect.Value;
			var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
			var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
			effect.Parameters["uTransform"].SetValue(model * projection);
			effect.Parameters["uProcession"].SetValue(value * 0.6f + 0.4f);
			effect.CurrentTechnique.Passes[0].Apply();
			Main.graphics.graphicsDevice.Textures[0] = ModAsset.Trail_7.Value;
			Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(sBS);
		}
	}

	public void DrawWarp(VFXBatch sb)
	{
		Vector2 normalized = Vector2.Normalize(Projectile.velocity.RotatedBy(Math.PI * 0.5)) * 50 * StabTimer / 120f * StabEffectWidth;
		Vector2 start = StabStartPoint_WorldPos;
		Vector2 end = Projectile.Center + Projectile.velocity * 100 * StabDistance;
		if (StabEndPoint_WorldPos != Vector2.Zero)
		{
			end = StabEndPoint_WorldPos;
		}
		float value = (Projectile.timeLeft + StabTimer) / 135f;
		var middle = Vector2.Lerp(end, start, MathF.Sqrt(value) * 0.5f);
		float time = (float)(Main.time * 0.03);
		Color alphaColor = StabColor;
		alphaColor.A = 0;
		alphaColor.R = (byte)(((Projectile.velocity.ToRotation() + Math.PI * 3) % MathHelper.TwoPi) / MathHelper.TwoPi * 255);
		alphaColor.G = 12;

		var bars = new List<Vertex2D>
		{
			new Vertex2D(start + normalized - Main.screenPosition, new Color(alphaColor.R, 0, 0, 0), new Vector3(1 + time, 0, 0)),
			new Vertex2D(start - normalized - Main.screenPosition, new Color(alphaColor.R, 0, 0, 0), new Vector3(1 + time, 1, 0)),
			new Vertex2D(middle + normalized - Main.screenPosition, alphaColor, new Vector3(0.5f + time, 0, 0.5f)),
			new Vertex2D(middle - normalized - Main.screenPosition, alphaColor, new Vector3(0.5f + time, 1, 0.5f)),
			new Vertex2D(end - Main.screenPosition, alphaColor, new Vector3(0f + time, 0.5f, 1)),
			new Vertex2D(end - Main.screenPosition, alphaColor, new Vector3(0f + time, 0.5f, 1)),
		};
		sb.Draw(ModAsset.Trail_1.Value, bars, PrimitiveType.TriangleStrip);
	}
}

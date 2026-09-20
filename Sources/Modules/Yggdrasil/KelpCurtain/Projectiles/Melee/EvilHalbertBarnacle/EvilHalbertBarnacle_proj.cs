using Everglow.Commons.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Melee.EvilHalbertBarnacle;

public class EvilHalbertBarnacle_proj : MeleeProj
{
	public float Omega;

	public bool NoTrail = false;

	public bool NoWeapon = false;

	public float RotationTowardScreen = 0;

	public Queue<float> DrawTrailFade = new Queue<float>();

	public float CurrentFade = 1;

	public Vector2 TargetVel;

	public int State = 0;

	public override void SetDef()
	{
		maxAttackType = 5;
		maxSlashTrailLength = 20;
		longHandle = true;
		autoEnd = true;
		canLongLeftClick = true;
		Omega = 0;
	}

	public override string TrailShapeTex()
	{
		return Commons.ModAsset.Melee_Mod;
	}

	public override string TrailColorTex()
	{
		return ModAsset.Barnacle_Color_Mod;
	}

	public override void AI()
	{
		base.AI();
		Player player = Main.player[Projectile.owner];
		if (player.ownedProjectileCounts[ModContent.ProjectileType<EvilHalbertBarnacle_proj_shuttle>()] <= 0)
		{
			State = 0;
		}
		else
		{
			State = 1;
		}
		if (!NoTrail)
		{
			CurrentFade = CurrentFade * 0.5f + 0.5f;
		}
		else
		{
			CurrentFade *= 0.5f;
		}
		DrawTrailFade.Enqueue(CurrentFade);
		if (DrawTrailFade.Count > maxSlashTrailLength)
		{
			DrawTrailFade.Dequeue();
		}
	}

	public override float TrailAlpha(float factor)
	{
		return base.TrailAlpha(factor) * 1.3f;
	}

	public override BlendState TrailBlendState()
	{
		return BlendState.NonPremultiplied;
	}

	public override void DrawSelf(SpriteBatch spriteBatch, Color lightColor, Vector4 diagonal = default, Vector2 drawScale = default, Texture2D glowTexture = null)
	{
		if (NoWeapon)
		{
			return;
		}
		else
		{
			if (diagonal == default)
			{
				diagonal = new Vector4(0, 1, 1, 0);
			}
			if (drawScale == default)
			{
				drawScale = new Vector2(0, 1);
				if (longHandle)
				{
					drawScale = new Vector2(-0.6f, 1);
				}
				drawScale *= drawScaleFactor;
			}
			Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
			if (State == 1)
			{
				tex = ModAsset.EvilHalbertBarnacle_proj_released.Value;
			}
			Vector2 drawCenter = Projectile.Center - Main.screenPosition;

			SpriteBatchState sBS = GraphicsUtils.GetState(spriteBatch).Value;
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

			DrawVertexByTwoLine(tex, lightColor, diagonal.XY(), diagonal.ZW(), drawCenter + mainAxisDirection * drawScale.X, drawCenter + mainAxisDirection * drawScale.Y);
			if (glowTexture != null)
			{
				DrawVertexByTwoLine(glowTexture, new Color(1f, 1f, 1f, 0), diagonal.XY(), diagonal.ZW(), drawCenter + mainAxisDirection * drawScale.X, drawCenter + mainAxisDirection * drawScale.Y);
			}

			spriteBatch.End();
			spriteBatch.Begin(sBS);
		}
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
	}

	public override void Attack()
	{
		Player player = Main.player[Projectile.owner];

		// Melee time parameter, the faster, the smaller.
		float meleeTime = 1f / player.meleeSpeed;
		if (currantAttackType == 0)
		{
			NoWeapon = false;
			if (timer < 16 * meleeTime)
			{
				NoTrail = true;
				LockPlayerDir(Player);
				float targetRot = -MathHelper.PiOver2 - Projectile.spriteDirection * 2f;
				mainAxisDirection = Vector2.Lerp(mainAxisDirection, Vector2Elipse(162, targetRot, -1.2f), 0.15f);
				mainAxisDirection += Projectile.DirectionFrom(Player.Center) * 3;
				Omega = 0;
				Projectile.rotation = mainAxisDirection.ToRotation();
				RotationTowardScreen = RotationTowardScreen * 0.8f;
			}
			else if (timer < 50 * meleeTime)
			{
				if (timer > 24 * meleeTime)
				{
					NoTrail = false;
				}
				canHit = true;
				if (timer < 32 * meleeTime)
				{
					Omega += 0.02f / meleeTime;
				}
				if (timer > 40 * meleeTime)
				{
					Omega *= 0.85f / MathF.Log(meleeTime * MathHelper.E);
					NoTrail = true;
				}
				Projectile.rotation += Projectile.spriteDirection * Omega;
				mainAxisDirection = Vector2Elipse(162, Projectile.rotation, -1.3f, RotationTowardScreen, 1000);
			}
			if (timer > 56 * meleeTime)
			{
				NextAttackType();
			}
		}
		if (currantAttackType == 1)
		{
			if (timer < 4 * meleeTime)
			{
				NoTrail = true;
				LockPlayerDir(Player);
				mainAxisDirection = mainAxisDirection * 0.9f + Vector2Elipse(162, -MathHelper.PiOver2 + Projectile.rotation, -1.3f, RotationTowardScreen, 1000) * 0.1f;
				Omega *= 0.4f / MathF.Log(meleeTime * MathHelper.E);
				Projectile.rotation += Projectile.spriteDirection * Omega;
				Projectile.rotation = mainAxisDirection.ToRotation();
				RotationTowardScreen = RotationTowardScreen * 0.75f + 0.6f * 0.25f * (-Projectile.spriteDirection);
			}
			else if (timer < 42 * meleeTime)
			{
				if (timer > 12 * meleeTime)
				{
					NoTrail = false;
				}
				canHit = true;
				if (timer < 16 * meleeTime)
				{
					Omega += 0.026f / meleeTime;
				}
				if (timer > 28 * meleeTime)
				{
					Omega *= 0.85f / MathF.Log(meleeTime * MathHelper.E);
					NoTrail = true;
				}
				Projectile.rotation += Projectile.spriteDirection * Omega;
				mainAxisDirection = Vector2Elipse(162, Projectile.rotation, -1.3f, RotationTowardScreen, 1000);
			}
			if (timer > 36 * meleeTime)
			{
				NextAttackType();
			}
		}
		if (currantAttackType == 2)
		{
			if (timer < 4 * meleeTime)
			{
				NoTrail = true;
				LockPlayerDir(Player);
				mainAxisDirection = mainAxisDirection * 0.9f + Vector2Elipse(162, -MathHelper.PiOver2 + Projectile.rotation, -1.3f, RotationTowardScreen, 1000) * 0.1f;
				Omega *= 0.4f / MathF.Log(meleeTime * MathHelper.E);
				Projectile.rotation += Projectile.spriteDirection * Omega;
				Projectile.rotation = mainAxisDirection.ToRotation();
				RotationTowardScreen = RotationTowardScreen * 0.75f - 0.6f * 0.25f * (-Projectile.spriteDirection);
			}
			else if (timer < 42 * meleeTime)
			{
				if (timer > 12 * meleeTime)
				{
					NoTrail = false;
				}
				canHit = true;
				if (timer < 12 * meleeTime)
				{
					Omega -= 0.04f / meleeTime;
				}
				if (timer > 28 * meleeTime)
				{
					Omega *= 0.5f / MathF.Log(meleeTime * MathHelper.E);
				}
				Projectile.rotation += Projectile.spriteDirection * Omega;
				mainAxisDirection = Vector2Elipse(162, Projectile.rotation, -1.3f, RotationTowardScreen, 1000);
			}
			if (timer > 26 * meleeTime)
			{
				NextAttackType();
			}
		}
		if (currantAttackType == 3)
		{
			NoTrail = true;
			NoWeapon = true;
			if (timer == 1)
			{
				TargetVel = Main.MouseWorld - player.MountedCenter;
				TargetVel = TargetVel.NormalizeSafe();
			}
			if (timer % 5 == 1 && timer < 27)
			{
				Vector2 dir = TargetVel.RotatedBy((timer - 11) / 10f);
				Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), player.Center, dir * 5, ModContent.ProjectileType<EvilHalbertBarnacle_proj_Thrust>(), Projectile.damage, Projectile.knockBack, player.whoAmI);
			}
			if (timer > 54 * meleeTime)
			{
				NoWeapon = false;
				NextAttackType();
			}
		}
		if (currantAttackType == 4)
		{
			if (timer < 4 * meleeTime)
			{
				NoWeapon = false;
				LockPlayerDir(Player);
				mainAxisDirection = mainAxisDirection * 0.6f + Vector2Elipse(162, -MathHelper.PiOver2 + Projectile.rotation, 0, RotationTowardScreen, 1000) * 0.4f;
				Omega *= 0.4f / MathF.Log(meleeTime * MathHelper.E);
				Projectile.rotation += Projectile.spriteDirection * Omega;
				Projectile.rotation = mainAxisDirection.ToRotation();
				RotationTowardScreen = RotationTowardScreen * 0.75f - 0.6f * 0.25f * (-Projectile.spriteDirection);
			}
			else if (timer < 42 * meleeTime)
			{
				if (timer > 12 * meleeTime)
				{
					NoTrail = false;
				}
				canHit = true;
				if (timer < 12 * meleeTime)
				{
					Omega -= 0.04f / meleeTime;
				}
				if (timer > 28 * meleeTime)
				{
					Omega *= 0.5f / MathF.Log(meleeTime * MathHelper.E);
				}
				Projectile.rotation += Projectile.spriteDirection * Omega;
				mainAxisDirection = Vector2Elipse(154, Projectile.rotation, 0, RotationTowardScreen, 1000);
			}
			if (timer > 36 * meleeTime)
			{
				NextAttackType();
			}
		}

		if (currantAttackType == 5)
		{
			if (timer < 4 * meleeTime)
			{
				NoTrail = true;
				LockPlayerDir(Player);
				mainAxisDirection = mainAxisDirection * 0.9f + Vector2Elipse(162, -MathHelper.PiOver2 + Projectile.rotation, -0.8f, RotationTowardScreen, 1000) * 0.1f;
				Omega *= 0.4f / MathF.Log(meleeTime * MathHelper.E);
				Projectile.rotation += Projectile.spriteDirection * Omega;
				Projectile.rotation = mainAxisDirection.ToRotation();
				RotationTowardScreen = RotationTowardScreen * 0.75f + 0.6f * 0.25f * (-Projectile.spriteDirection);
			}
			else if (timer < 42 * meleeTime)
			{
				if (timer > 12 * meleeTime)
				{
					NoTrail = false;
				}
				canHit = true;
				if (timer < 12 * meleeTime)
				{
					Omega += 0.04f / meleeTime;
				}
				if (timer > 28 * meleeTime)
				{
					Omega *= 0.5f / MathF.Log(meleeTime * MathHelper.E);
				}
				Projectile.rotation += Projectile.spriteDirection * Omega;
				mainAxisDirection = Vector2Elipse(154, Projectile.rotation, -0.8f, RotationTowardScreen, 1000);
			}
			if (timer > 36 * meleeTime)
			{
				NextAttackType();
			}
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		DrawSelf(Main.spriteBatch, lightColor);
		DrawTrail(lightColor);
		return false;
	}

	public override void DrawTrail(Color color)
	{
		List<Vector2> smoothTrail_current = GraphicsUtils.CatmullRom(slashTrail.ToList()); // 平滑
		var SmoothTrail = new List<Vector2>();
		for (int x = 0; x < smoothTrail_current.Count - 1; x++)
		{
			Vector2 vec = smoothTrail_current[x];

			SmoothTrail.Add(Vector2.Normalize(vec) * (vec.Length() + disFromPlayer));
		}
		if (slashTrail.Count != 0)
		{
			Vector2 vec = slashTrail.ToArray()[slashTrail.Count - 1];

			SmoothTrail.Add(Vector2.Normalize(vec) * (vec.Length() + disFromPlayer));
		}
		Vector2 center = Projectile.Center - Vector2.Normalize(mainAxisDirection) * disFromPlayer;
		int length = SmoothTrail.Count;
		if (length <= 3)
		{
			return;
		}

		Vector2[] trail = SmoothTrail.ToArray();
		var bars = new List<Vertex2D>();
		var bars_Side = new List<Vertex2D>();
		float fadeIndexScale = slashTrail.Count / (float)length;
		for (int i = 0; i < length; i++)
		{
			float factor = i / (length - 1f);
			if (i == length - 1)
			{
				factor = 0;
			}
			float w = TrailAlpha(factor);
			Color drawColor = Lighting.GetColor((center + trail[i] * 0.9f * Projectile.scale).ToTileCoordinates()) * 0.6f;
			drawColor.A = 255;
			int fadeIndex = (int)Math.Clamp(i * fadeIndexScale, 0, DrawTrailFade.Count - 1);
			drawColor *= DrawTrailFade.ToArray()[fadeIndex];
			int invisible = 0;

			if (timer < 24)
			{
				invisible = length;
			}
			if (timer < 44 && timer >= 24)
			{
				invisible = (int)Utils.Lerp(length, 0, MathF.Pow((timer - 24) / 20f, 2));
			}
			if (i < invisible)
			{
				w = 0;
			}
			bars.Add(center + trail[i] * 0.3f * Projectile.scale, drawColor, new Vector3(factor, 0.7f, 0f));
			bars.Add(center + trail[i] * 1.1f * Projectile.scale, drawColor, new Vector3(factor, 0, w));

			bars_Side.Add(center + trail[i] * 0.3f * Projectile.scale, drawColor, new Vector3(factor, 0.8f, 0f));
			bars_Side.Add(center + trail[i] * 1.15f * Projectile.scale, drawColor, new Vector3(factor, 0, w));
		}

		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;

		Effect meleetrail = ModAsset.EvilHalbertBarnacle_proj_MeleeTrail.Value;
		Main.spriteBatch.End();

		Main.spriteBatch.Begin(SpriteSortMode.Immediate, TrailBlendState(), SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone);
		meleetrail.Parameters["uTransform"].SetValue(model * projection);
		meleetrail.CurrentTechnique.Passes["ArcBladeConvertTransparent"].Apply();
		Main.graphics.GraphicsDevice.Textures[0] = ModAsset.Barnacle_Color_Trail.Value;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

		Main.graphics.GraphicsDevice.Textures[0] = ModAsset.Barnacle_Color_Trail2.Value;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars_Side.ToArray(), 0, bars_Side.Count - 2);
		Main.spriteBatch.End();

		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone);
		Main.graphics.GraphicsDevice.Textures[0] = ModAsset.Barnacle_Color_Trail.Value;
		meleetrail.Parameters["tex1"].SetValue(ModContent.Request<Texture2D>(TrailColorTex(), ReLogic.Content.AssetRequestMode.ImmediateLoad).Value);
		meleetrail.CurrentTechnique.Passes["ArcBladeAffectByEnvironmentLight"].Apply();
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		Main.spriteBatch.End();

		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone);
		Main.graphics.GraphicsDevice.Textures[0] = ModAsset.Barnacle_Color_Trail2.Value;
		meleetrail.CurrentTechnique.Passes["ArcBladeAffectByEnvironmentLightPlus"].Apply();
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars_Side.ToArray(), 0, bars_Side.Count - 2);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
	}

	public override void End()
	{
		Player player = Main.player[Projectile.owner];
		TestPlayerDrawer Tplayer = player.GetModPlayer<TestPlayerDrawer>();
		player.legFrame = new Rectangle(0, 0, player.legFrame.Width, player.legFrame.Height);
		player.fullRotation = 0;
		player.legRotation = 0;
		Tplayer.HeadRotation = 0;
		Tplayer.HideLeg = false;
		player.legPosition = Vector2.Zero;
		Projectile.Kill();
		player.GetModPlayer<MEACPlayer>().isUsingMeleeProj = false;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
	}
}

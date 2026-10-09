using Terraria.DataStructures;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.PrimordialJadeWinged_Spear;

public class PrimordialJadeWinged_Spear : MeleeProj
{
	public override void SetDef()
	{
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 30;
		Projectile.extraUpdates = 1;
		Projectile.scale = 1f;
		Projectile.hostile = false;
		Projectile.friendly = true;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.penetrate = -1;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 15;
		Projectile.DamageType = DamageClass.Melee;

		Projectile.width = 80;
		Projectile.height = 80;
		Projectile.tileCollide = false;
		Projectile.friendly = true;
		longHandle = true;
		maxAttackType = 4;
		maxSlashTrailLength = 20;
		shaderType = Commons.MEAC.Enums.MeleeTrailShaderType.ArcBladeTransparentedByZ;
		;
		autoEnd = false;
	}
	public override string TrailShapeTex()
	{
		return Commons.ModAsset.Melee_Mod;
	}
	public override string TrailColorTex()
	{
		return "Everglow/Myth/Misc/Projectiles/Weapon/Melee/PrimordialJadeWinged_Spear/PrimordialJadeWinged_Spear_meleeColor";
	}
	public override float TrailAlpha(float factor)
	{
		return base.TrailAlpha(factor) * 1.15f;
	}
	public override BlendState TrailBlendState()
	{
		return BlendState.NonPremultiplied;
	}
	public override void End()
	{
		Player player = Main.player[Projectile.owner];
		player.legFrame = new Rectangle(0, 0, player.legFrame.Width, player.legFrame.Height);
		player.fullRotation = 0;
		player.legRotation = 0;
		player.legPosition = Vector2.Zero;
		Projectile.Kill();
		player.GetModPlayer<MEACPlayer>().isUsingMeleeProj = false;
	}
	public override void OnSpawn(IEntitySource source)
	{
	}
	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		base.AI();
		TestPlayerDrawer Tplayer = player.GetModPlayer<TestPlayerDrawer>();
		Tplayer.HideLeg = true;
		useSlash = true;
		float timeMul = 1 / player.meleeSpeed;
		if (currantAttackType == 0)
		{
			if (timer < 3 * timeMul)// 前摇
			{
				useSlash = false;
				LockPlayerDir(player);
				float targetRot = -MathHelper.PiOver2 - player.direction * 0.5f;
				mainAxisDirection = Vector2.Lerp(mainAxisDirection, Vector2Elipse(170, targetRot, 2f), 0.7f);
				mainAxisDirection += Projectile.DirectionFrom(player.Center) * 3;
				Projectile.rotation = mainAxisDirection.ToRotation();
			}
			if (timer == (int)(20 * timeMul))
			{
				AttSound(SoundID.Item1);
			}

			if (timer == (int)(30 * timeMul))
			{
				Vector2 v0 = new Vector2(Projectile.spriteDirection * 6, -0.5f);
				Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), player.Center - v0 * 30, v0, ModContent.ProjectileType<PrimordialJadeWinged_Spear_thrust2>(), Projectile.damage, 0, Projectile.owner);
			}
			if (timer > 3 * timeMul && timer < 30 * timeMul)
			{
				canHit = true;
				Projectile.rotation -= Projectile.spriteDirection * 0.25f / timeMul;
				mainAxisDirection = Vector2Elipse(190, Projectile.rotation, 0.6f);
				GenerateVFX();
				GenerateSpark();
			}

			if (timer > 40 * timeMul)
			{
				player.fullRotation = 0;
				player.legRotation = 0;
				NextAttackType();
			}
		}
		if (currantAttackType == 1)
		{
			if (timer < 24 * timeMul)// 前摇
			{
				useSlash = false;
				LockPlayerDir(player);
				float targetRot = -MathHelper.PiOver2 - player.direction * 0.5f;
				mainAxisDirection = Vector2.Lerp(mainAxisDirection, Vector2Elipse(210, targetRot, +1.2f), 0.4f / timeMul);
				mainAxisDirection += Projectile.DirectionFrom(player.Center) * 3;
				Projectile.rotation = mainAxisDirection.ToRotation();
			}
			if (timer == (int)(20 * timeMul))
			{
				AttSound(SoundID.Item1);
			}

			if (timer > 16 * timeMul && timer < 50 * timeMul)
			{
				canHit = true;
				Projectile.rotation -= Projectile.spriteDirection * 0.25f / timeMul;
				mainAxisDirection = Vector2.Lerp(mainAxisDirection, Vector2Elipse(240, Projectile.rotation, -1.2f, 0.3f * Projectile.spriteDirection), 0.4f / timeMul);
				GenerateVFX();
				GenerateSpark();
			}
			if (timer > 50 * timeMul)
			{
				NextAttackType();
			}
		}
		if (currantAttackType == 2)
		{
			if (timer < 24 * timeMul)// 前摇
			{
				useSlash = false;
				LockPlayerDir(player);
				float targetRot = -MathHelper.PiOver2 - player.direction * 2.5f;
				mainAxisDirection = Vector2.Lerp(mainAxisDirection, Vector2Elipse(190, targetRot, 0.4f), 0.4f / timeMul);
				mainAxisDirection += Projectile.DirectionFrom(player.Center) * 3;
				Projectile.rotation = mainAxisDirection.ToRotation();
			}
			if (timer == (int)(20 * timeMul))
			{
				AttSound(SoundID.Item1);
			}

			if (timer > 16 * timeMul && timer < 30 * timeMul)
			{
				canHit = true;
				Projectile.rotation += Projectile.spriteDirection * 0.15f / timeMul;
				mainAxisDirection = Vector2.Lerp(mainAxisDirection, Vector2Elipse(210, Projectile.rotation, 0.4f, 0f), 0.2f / timeMul);
				GenerateVFX();
				GenerateSpark();
			}
			if (timer > 30 * timeMul && timer < 45 * timeMul)
			{
				canHit = true;
				Projectile.rotation += Projectile.spriteDirection * 0.35f / timeMul;
				mainAxisDirection = Vector2.Lerp(mainAxisDirection, Vector2Elipse(210, Projectile.rotation, 0.4f, 0f), 0.4f / timeMul);
				GenerateVFX();
				GenerateSpark();
			}
			if (timer > 55 * timeMul)
			{
				NextAttackType();
			}
		}
		if (currantAttackType == 3)
		{
			if (timer < 24 * timeMul)// 前摇
			{
				useSlash = false;
				LockPlayerDir(player);
				float targetRot = -MathHelper.PiOver2 - player.direction * 2.5f;
				mainAxisDirection = Vector2.Lerp(mainAxisDirection, Vector2Elipse(190, targetRot, 1.27f, -0.3f * Projectile.spriteDirection), 0.4f / timeMul);
				mainAxisDirection += Projectile.DirectionFrom(player.Center) * 3;
				Projectile.rotation = mainAxisDirection.ToRotation();
			}
			if (timer == (int)(20 * timeMul))
			{
				AttSound(SoundID.Item1);
			}

			if (timer == (int)(37 * timeMul))
			{
				AttSound(SoundID.Item1);
			}

			if (timer == (int)(54 * timeMul))
			{
				AttSound(SoundID.Item1);
			}

			if (timer > 16 * timeMul && timer < 57 * timeMul)
			{
				canHit = true;
				Projectile.rotation += Projectile.spriteDirection * 0.29f / timeMul;
				mainAxisDirection = Vector2.Lerp(mainAxisDirection, Vector2Elipse(270, Projectile.rotation, 1.27f, -0.2f * Projectile.spriteDirection), 0.3f / timeMul);
				GenerateVFX();
				GenerateSpark();
			}
			if (timer == (int)(69 * timeMul))
			{
				Vector2 v0 = new Vector2(Projectile.spriteDirection * 6, 0.5f);
				Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), player.Center - v0 * 30, v0, ModContent.ProjectileType<PrimordialJadeWinged_Spear_thrust2>(), Projectile.damage, 0, Projectile.owner);
			}
			if (timer > 83 * timeMul)
			{
				NextAttackType();
			}
		}
		if (currantAttackType == 4)
		{
			if (timer < 24 * timeMul)// 前摇
			{
				useSlash = false;
				// LockPlayerDir(player);
				float targetRot = -MathHelper.PiOver2 - player.direction * 2.5f;
				mainAxisDirection = Vector2.Lerp(mainAxisDirection, Vector2Elipse(190, targetRot, 0, -0.3f * Projectile.spriteDirection), 0.4f / timeMul);
				mainAxisDirection += Projectile.DirectionFrom(player.Center) * 3;
				Projectile.rotation = mainAxisDirection.ToRotation();
			}
			if (timer == (int)(20 * timeMul))
			{
				AttSound(SoundID.Item1);
			}

			if (timer > 16 * timeMul && timer < 50 * timeMul)
			{
				canHit = true;
				Projectile.rotation += Projectile.spriteDirection * ((timer - 16 * timeMul) / 110f) / timeMul / timeMul;
				mainAxisDirection = Vector2.Lerp(mainAxisDirection, Vector2Elipse(200, Projectile.rotation, 0, -0.2f * Projectile.spriteDirection), 0.3f / timeMul);
				GenerateVFX();
				GenerateSpark();
			}
			if (timer > 76 * timeMul)
			{
				NextAttackType();
			}
		}
	}
	public override void OnKill(int timeLeft)
	{
		Player player = Main.player[Projectile.owner];
		player.fullRotation = 0;
	}
	public override void DrawSelf(SpriteBatch spriteBatch, Color lightColor, Vector4 diagonal = default, Vector2 drawScale = default, Texture2D glowTexture = null)
	{
		drawScale = new Vector2(-0.6f, 1.14f);
		base.DrawSelf(spriteBatch, lightColor, diagonal, drawScale, glowTexture);
	}
	public override void DrawTrail(Color color)
	{
		List<Vector2> SmoothTrailX = GraphicsUtils.CatmullRom(slashTrail.ToList()); // 平滑
		var SmoothTrail = new List<Vector2>();
		for (int x = 0; x <= SmoothTrailX.Count - 1; x++)
		{
			SmoothTrail.Add(SmoothTrailX[x]);
		}
		if (slashTrail.Count != 0)
		{
			SmoothTrail.Add(slashTrail.ToArray()[slashTrail.Count - 1]);
		}

		int length = SmoothTrail.Count;
		if (length <= 3)
		{
			return;
		}

		Vector2[] trail = SmoothTrail.ToArray();
		var bars = new List<Vertex2D>();

		for (int i = 0; i < length; i++)
		{
			float factor = i / (length - 1f);
			float w = TrailAlpha(factor);
			Color c0 = Color.White;
			if (i == 0)
			{
				c0 = Color.Transparent;
			}
			bars.Add(new Vertex2D(Projectile.Center + trail[i] * 0.3f * Projectile.scale, c0, new Vector3(factor, 1, 0f)));
			bars.Add(new Vertex2D(Projectile.Center + trail[i] * Projectile.scale, c0, new Vector3(factor, 0, w)));
		}
		bars.Add(new Vertex2D(Projectile.Center + mainAxisDirection * 0.3f * Projectile.scale, Color.White, new Vector3(0, 1, 0f)));
		bars.Add(new Vertex2D(Projectile.Center + mainAxisDirection * Projectile.scale, Color.White, new Vector3(0, 0, 1)));
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, TrailBlendState(), SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone);
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;

		Effect MeleeTrail = Commons.ModAsset.MeleeTrail.Value;
		MeleeTrail.Parameters["uTransform"].SetValue(model * projection);
		Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>(TrailShapeTex(), ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
		// Main.graphics.GraphicsDevice.Textures[1] = ModContent.Request<Texture2D>(TrailColorTex(), ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

		MeleeTrail.Parameters["tex1"].SetValue(ModContent.Request<Texture2D>(TrailColorTex(), ReLogic.Content.AssetRequestMode.ImmediateLoad).Value);
		MeleeTrail.CurrentTechnique.Passes[ShaderTypeName].Apply();

		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
	}
	private void GenerateVFX()
	{
		Player player = Main.player[Projectile.owner];
		int times = (int)Math.Floor(player.meleeSpeed);
		if (Main.rand.NextFloat(0, 1f) < player.meleeSpeed % 1f)
		{
			times += 1;
		}
		for (int x = 0; x < times; x++)
		{
			Vector2 newVec = mainAxisDirection;
			newVec *= player.gravDir;
			Vector2 mainVecLeft = Vector2.Normalize(newVec).RotatedBy(-MathHelper.PiOver2 * player.gravDir);
			var positionVFX = Projectile.Center + mainVecLeft * Main.rand.NextFloat(-30f, 30f) + newVec * Main.rand.NextFloat(0.7f, 1f);

			var filthy = new FilthyLucreFlame_darkDust
			{
				velocity = mainVecLeft * 6 * Projectile.spriteDirection,
				Active = true,
				Visible = true,
				position = positionVFX,
				maxTime = Main.rand.Next(17, 56),
				ai = new float[] { Main.rand.NextFloat(0.1f, 1f), Main.rand.NextFloat(-0.03f, 0.03f), Main.rand.NextFloat(18f, 30f) },
			};
			Ins.VFXManager.Add(filthy);
			var filthy2 = new FilthyLucreFlameDust
			{
				velocity = mainVecLeft * 16 * Projectile.spriteDirection,
				Active = true,
				Visible = true,
				position = positionVFX,
				maxTime = Main.rand.Next(17, 56),
				ai = new float[] { Main.rand.NextFloat(0.1f, 1f), Main.rand.NextFloat(-0.1f, 0.1f), Main.rand.NextFloat(18f, 30f) },
			};
			Ins.VFXManager.Add(filthy2);
		}
	}
	public void GenerateSpark()
	{
		Player player = Main.player[Projectile.owner];
		int times = (int)Math.Floor(player.meleeSpeed);
		if (Main.rand.NextFloat(0, 1f) < player.meleeSpeed % 1f)
		{
			times += 1;
		}
		for (int x = 0; x < times; x++)
		{
			Vector2 newVec = mainAxisDirection;
			newVec *= player.gravDir;
			Vector2 mainVecLeft = Vector2.Normalize(newVec).RotatedBy(-MathHelper.PiOver2 * player.gravDir);
			var positionVFX = Projectile.Center + mainVecLeft * Main.rand.NextFloat(-30f, 30f) + newVec * Main.rand.NextFloat(0.7f, 1f);
			var smog = new FilthyFragileDust
			{
				velocity = mainVecLeft * 16 * Projectile.spriteDirection,
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

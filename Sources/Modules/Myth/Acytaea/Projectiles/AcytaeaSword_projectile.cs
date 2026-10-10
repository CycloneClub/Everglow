using Everglow.Myth.Acytaea.Buffs;
using Everglow.Myth.Acytaea.VFXs;

namespace Everglow.Myth.Acytaea.Projectiles;

public class AcytaeaSword_projectile : MeleeProj
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeProjectiles;

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
		longHandle = false;
		maxAttackType = 0;
		maxSlashTrailLength = 20;
		shaderType = Commons.MEAC.Enums.MeleeTrailShaderType.ArcBladeTransparentedByZ;
		autoEnd = false;
	}

	public override string TrailShapeTex()
	{
		return Commons.ModAsset.Melee_Mod;
	}

	public override string TrailColorTex()
	{
		return "Everglow/Myth/Acytaea/Projectiles/Acytaea_meleeColor";
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
				float targetRot = -MathHelper.PiOver2 + player.direction * 0.5f;
				mainAxisDirection = Vector2.Lerp(mainAxisDirection, Vector2Elipse(170, targetRot, 2f), 0.7f);
				mainAxisDirection += Projectile.DirectionFrom(player.Center) * 3;
				Projectile.rotation = mainAxisDirection.ToRotation();
			}
			if (timer == (int)(20 * timeMul))
			{
				AttSound(SoundID.Item1);
			}

			if (timer > 3 * timeMul && timer < 30 * timeMul)
			{
				canHit = true;
				Projectile.rotation += Projectile.spriteDirection * 0.25f / timeMul;
				mainAxisDirection = Vector2Elipse(190, Projectile.rotation, 0.6f);
				if (timer < 24 * timeMul)
				{
					GenerateVFX();
				}
				else
				{
					if (Main.rand.Next((int)(60 * timeMul)) < (30 * timeMul - timer) * 10)
					{
						GenerateVFX();
					}
				}
			}

			if (timer > 60 * timeMul)
			{
				player.fullRotation = 0;
				player.legRotation = 0;
				NextAttackType();
			}
		}
	}

	private void GenerateVFX()
	{
		Player player = Main.player[Projectile.owner];
		int times = (int)Math.Floor(player.meleeSpeed);
		if (Main.rand.NextFloat(0, 1f) < player.meleeSpeed % 1f)
		{
			times += 1;
		}
		times *= 3;
		for (int x = 0; x < times; x++)
		{
			Vector2 newVec = mainAxisDirection;
			newVec *= player.gravDir;
			Vector2 mainVecLeft = Vector2.Normalize(newVec).RotatedBy(-MathHelper.PiOver2 * player.gravDir);
			var positionVFX = Projectile.Center + mainVecLeft * Main.rand.NextFloat(-30f, 30f) + newVec * Main.rand.NextFloat(0.7f, 0.9f);

			var acytaeaFlame = new AcytaeaFlameDust
			{
				velocity = -mainVecLeft * Main.rand.NextFloat(6f, 12f) * Projectile.spriteDirection,
				Active = true,
				Visible = true,
				position = positionVFX,
				maxTime = Main.rand.Next(14, 16),
				ai = new float[] { Main.rand.NextFloat(0.1f, 1f), Main.rand.NextFloat(-0.04f, 0.04f), Main.rand.NextFloat(18f, 30f) },
			};
			Ins.VFXManager.Add(acytaeaFlame);
		}
	}

	public override void OnKill(int timeLeft)
	{
	}

	public override void DrawSelf(SpriteBatch spriteBatch, Color lightColor, Vector4 diagonal = default, Vector2 drawScale = default, Texture2D glowTexture = null)
	{
		drawScale = new Vector2(-0.1f, 1.02f);
		glowTexture = ModAsset.Acytaea_sword_Item_glow.Value;
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

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		for (int x = 0; x < 25; x++)
		{
			Vector2 newVec = new Vector2(0, Main.rand.NextFloat(4f, 12f)).RotatedByRandom(6.238f);
			var positionVFX = target.Center + newVec * Main.rand.NextFloat(0.7f, 0.9f);

			var acytaeaFlame = new AcytaeaFlameDust
			{
				velocity = newVec,
				Active = true,
				Visible = true,
				position = positionVFX,
				maxTime = Main.rand.Next(14, 16),
				ai = new float[] { Main.rand.NextFloat(0.1f, 1f), Main.rand.NextFloat(-0.04f, 0.04f), Main.rand.NextFloat(18f, 30f) },
			};
			Ins.VFXManager.Add(acytaeaFlame);
		}
		target.AddBuff(ModContent.BuffType<AcytaeaInferno>(), 450);
		base.OnHitNPC(target, hit, damageDone);
	}
}

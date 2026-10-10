using Everglow.Commons.DataStructures;
using Everglow.Myth.Acytaea.Buffs;
using Everglow.Myth.Acytaea.VFXs;
using Terraria.DataStructures;

namespace Everglow.Myth.Acytaea.Projectiles;

public class AcytaeaSword_projectile_TownNPC : MeleeProj
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.TownNPCProjectiles;

	public override string Texture => "Everglow/Myth/Acytaea/Projectiles/AcytaeaSword_projectile";

	public override void SetDef()
	{
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 80;
		Projectile.extraUpdates = 1;
		Projectile.scale = 1f;
		Projectile.hostile = false;
		Projectile.friendly = false;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.penetrate = -1;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 15;
		Projectile.DamageType = DamageClass.Melee;

		Projectile.width = 80;
		Projectile.height = 80;
		longHandle = false;
		maxAttackType = 0;
		maxSlashTrailLength = 20;
		shaderType = Commons.MEAC.Enums.MeleeTrailShaderType.ArcBladeTransparentedByZ;
		ignoreTile = true;
		autoEnd = false;
	}

	public NPC Owner;

	public override void OnSpawn(IEntitySource source)
	{
		int index = (int)Projectile.ai[0];
		if (index >= 0 && index < 200)
		{
			Owner = Main.npc[index];
		}
		else
		{
			Projectile.Kill();
		}
		Projectile.Center = Owner.Center;
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
		Projectile.Kill();
	}

	public override void AI()
	{
		Attack();
		Projectile.Center = Owner.Center;
		Projectile.spriteDirection = -Owner.direction;
		timer++;
		if (!canHit)
		{
			if (!isRightClick)
			{
				bool IsEnd = autoEnd ? !Player.controlUseItem || Player.dead : Player.dead;
				if (IsEnd)
				{
					End();
				}
			}
			else
			{
				bool IsEnd = autoEnd ? !Player.controlUseTile || Player.dead : Player.dead;
				if (IsEnd)
				{
					End();
				}
			}
		}
		if (useSlash)
		{
			slashTrail.Enqueue(mainAxisDirection);
			if (slashTrail.Count > maxSlashTrailLength)
			{
				slashTrail.Dequeue();
			}
		}
		else
		{
			slashTrail.Clear();
		}
		if (canLongLeftClick)
		{
			if (Main.mouseLeft)
			{
				clickTimer++;
			}
			else
			{
				clickTimer = 0;
			}
		}

		useSlash = true;
		if (currantAttackType == 0)
		{
			if (timer < 16)// 前摇
			{
				useSlash = false;
				float targetRot = -MathHelper.PiOver2 - Owner.direction * 0.5f;
				mainAxisDirection = Vector2.Lerp(mainAxisDirection, Vector2Elipse(170, targetRot, 2f), 0.7f);
				Projectile.rotation = mainAxisDirection.ToRotation();
			}
			if (timer == 20)
			{
				Projectile.friendly = true;
				AttSound(SoundID.Item1);
			}

			if (timer >= 16 && timer < 30)
			{
				canHit = true;
				Projectile.rotation += Projectile.spriteDirection * 0.03f;
				mainAxisDirection = Vector2Elipse(190, Projectile.rotation, 1.2f, 0);
			}

			if (timer >= 30 && timer < 40)
			{
				canHit = true;
				Projectile.rotation += Projectile.spriteDirection * 0.2f;
				mainAxisDirection = Vector2Elipse(190, Projectile.rotation, 1.2f, 0);
				GenerateVFX();
			}

			if (timer >= 40 && timer < 50)
			{
				canHit = true;
				Projectile.rotation += Projectile.spriteDirection * 0.4f;
				mainAxisDirection = Vector2Elipse(190, Projectile.rotation, 1.2f, 0);
				GenerateVFX();
			}
			if (timer >= 50 && timer < 60)
			{
				Projectile.friendly = false;
				canHit = true;
				Projectile.rotation += Projectile.spriteDirection * 0.01f;
				mainAxisDirection = Vector2Elipse(190, Projectile.rotation, 1.2f, 0);
			}
			if (timer >= 60 && timer < 80)
			{
				canHit = true;
				Projectile.rotation += Projectile.spriteDirection * 0.005f;
				mainAxisDirection = Vector2Elipse(190, Projectile.rotation, 1.2f, 0);
			}
		}
	}

	private void GenerateVFX()
	{
		int times = 1;
		for (int x = 0; x < times; x++)
		{
			Vector2 newVec = Vector2Elipse(190, Projectile.rotation + x / 3f * 0.25f, 1.2f, 0);

			Vector2 mainVecLeft = Vector2.Normalize(newVec).RotatedBy(MathHelper.PiOver2);
			float size = Main.rand.NextFloat(Main.rand.NextFloat(Main.rand.NextFloat(0.4f, 0.96f), 0.96f), 0.96f);
			var positionVFX = Projectile.Center + newVec * size;

			var acytaeaFlame = new AcytaeaFlameDust
			{
				velocity = mainVecLeft.RotatedByRandom(6.283) * 1.5f,
				Active = true,
				Visible = true,
				position = positionVFX,
				maxTime = Main.rand.Next(14, 16),
				ai = new float[] { Main.rand.NextFloat(0.1f, 1f), 0, 12f * size },
			};
			Ins.VFXManager.Add(acytaeaFlame);
		}
	}

	public override void DrawSelf(SpriteBatch spriteBatch, Color lightColor, Vector4 diagonal = default, Vector2 drawScale = default, Texture2D glowTexture = null)
	{
		drawScale = new Vector2(-0.1f, 1.02f);
		glowTexture = ModAsset.Acytaea_sword_Item_glow.Value;

		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Effect dissolve = Commons.ModAsset.Dissolve.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(0)) * Main.GameViewMatrix.TransformationMatrix;
		float dissolveDuration = (80 - timer) / 20f;
		if (timer < 16)
		{
			dissolveDuration = timer / 16f;
		}
		if (timer < 60 && timer > 16)
		{
			dissolveDuration = 1.2f;
		}
		dissolve.Parameters["uTransform"].SetValue(model * projection);
		dissolve.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_spiderNet.Value);
		dissolve.Parameters["duration"].SetValue(dissolveDuration);
		dissolve.Parameters["uDissolveColor"].SetValue(new Vector4(1f, 0f, 0f, 1f));
		dissolve.Parameters["uNoiseSize"].SetValue(3f);
		dissolve.Parameters["uNoiseXY"].SetValue(new Vector2(Projectile.ai[1], Projectile.ai[2]));
		dissolve.CurrentTechnique.Passes[0].Apply();

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
		Vector2 drawCenter = Projectile.Center - Main.screenPosition;

		DrawVertexByTwoLine(tex, lightColor, diagonal.XY(), diagonal.ZW(), drawCenter + mainAxisDirection * drawScale.X, drawCenter + mainAxisDirection * drawScale.Y);

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
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
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.ZoomMatrix;

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
			Vector2 newVec = new Vector2(0, Main.rand.NextFloat(1f, 2f)).RotatedByRandom(6.238f);
			var positionVFX = target.Center + newVec * Main.rand.NextFloat(0.7f, 0.9f);

			var acytaeaFlame = new AcytaeaFlameDust
			{
				velocity = newVec,
				Active = true,
				Visible = true,
				position = positionVFX,
				maxTime = Main.rand.Next(14, 16),
				ai = new float[] { Main.rand.NextFloat(0.1f, 1f), Main.rand.NextFloat(-0.04f, 0.04f), Main.rand.NextFloat(8f, 12f) },
			};
			Ins.VFXManager.Add(acytaeaFlame);
		}
		target.AddBuff(ModContent.BuffType<AcytaeaInferno>(), 450);
		base.OnHitNPC(target, hit, damageDone);
	}
}

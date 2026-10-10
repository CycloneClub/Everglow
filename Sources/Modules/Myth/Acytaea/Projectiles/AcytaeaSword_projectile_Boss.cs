using Everglow.Commons.MEAC.Enums;
using Everglow.Myth.Acytaea.Buffs;
using Everglow.Myth.Acytaea.VFXs;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;

namespace Everglow.Myth.Acytaea.Projectiles;

public class AcytaeaSword_projectile_Boss : ModProjectile, IWarpProjectile, IBloomProjectile
{
	public override string Texture => "Everglow/Myth/Acytaea/Projectiles/AcytaeaSword_projectile";

	public override void SetDefaults()
	{
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 120;
		Projectile.extraUpdates = 0;
		Projectile.scale = 1f;
		Projectile.hostile = true;
		Projectile.friendly = false;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.penetrate = -1;
		Projectile.DamageType = DamageClass.Melee;

		Projectile.width = 80;
		Projectile.height = 80;
		longHandle = false;
		maxAttackType = 1;
		trailLength = 20;
		shaderType = MeleeTrailShaderType.ArcBladeTransparentedByZ;
		trailVecs = new Queue<Vector2>(trailLength + 1);
	}

	public int attackType = 0;
	public int maxAttackType = 1;
	public Vector2 mainVec;
	public Queue<Vector2> trailVecs;
	public int trailLength = 40;
	public int timer = 0;
	public int OwnerNPC = -1;
	public bool useTrail = true;
	public bool longHandle = false;
	public MeleeTrailShaderType shaderType = MeleeTrailShaderType.ArcBladeAutoTransparent;

	public override void OnSpawn(IEntitySource source)
	{
		int index = (int)Projectile.ai[0];
		if (index >= 0 && index < 200)
		{
			OwnerNPC = (int)Projectile.ai[0];
		}
		else
		{
			Projectile.Kill();
		}
	}

	public override void AI()
	{
		if (OwnerNPC == -1)
		{
			Projectile.Kill();
			return;
		}
		NPC Owner = Main.npc[OwnerNPC];
		if (Owner == null || !Owner.active)
		{
			Projectile.Kill();
			return;
		}
		timer++;
		useTrail = true;
		if (useTrail)
		{
			trailVecs.Enqueue(mainVec);
			if (trailVecs.Count > trailLength)
			{
				trailVecs.Dequeue();
			}
		}
		else// 清空！
		{
			trailVecs.Clear();
		}

		// ProduceWaterRipples(new Vector2(mainVec.Length(), 30));
		float timeMul = 1f;
		if (attackType == 0)
		{
			if (timer < 30 * timeMul)// 前摇
			{
				float targetRot = -MathHelper.PiOver2 - Owner.spriteDirection * 0.5f;
				mainVec = Vector2.Lerp(mainVec, Vector2Elipse(170, targetRot, 2f), 0.7f);
				mainVec += Projectile.DirectionFrom(Owner.Center) * 3;
				Projectile.rotation = mainVec.ToRotation();
			}
			if (timer == (int)(20 * timeMul))
			{
				SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
			}

			if (timer > 33 * timeMul && timer < 60 * timeMul)
			{
				Projectile.extraUpdates = 1;
				Projectile.rotation += Projectile.spriteDirection * 0.25f / timeMul;
				mainVec = Vector2Elipse(190, Projectile.rotation, 0.6f);
				if (timer < 54 * timeMul)
				{
					GenerateVFX();
				}
				else
				{
					if (Main.rand.Next((int)(90 * timeMul)) < (60 * timeMul - timer) * 10)
					{
						GenerateVFX();
					}
				}
			}

			if (timer > 90 * timeMul)
			{
				End();
			}
		}
		Projectile.Center = Owner.Center + mainVec * 0.02f;
		Projectile.spriteDirection = Owner.spriteDirection;
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		DrawTrail(lightColor);
		DrawSelf(Main.spriteBatch, lightColor);
		return false;
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		float point = 0;
		if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center + mainVec * Projectile.scale * (longHandle ? 0.2f : 0.1f), Projectile.Center + mainVec * Projectile.scale, Projectile.height, ref point))
		{
			if (Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, targetHitbox.TopLeft(), targetHitbox.Width, targetHitbox.Height))
			{
				return true;
			}
		}

		return false;
	}

	public void DrawSelf(SpriteBatch spriteBatch, Color lightColor, Vector4 diagonal = default(Vector4), Vector2 drawScale = default(Vector2), Texture2D glowTexture = null)
	{
		if (diagonal == default(Vector4))
		{
			diagonal = new Vector4(0, 1, 1, 0);
		}
		if (drawScale == default(Vector2))
		{
			drawScale = new Vector2(0, 1);
			if (longHandle)
			{
				drawScale = new Vector2(-0.6f, 1);
			}
		}
		Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
		Texture2D tex2 = ModAsset.AcytaeaSword_projectile_highLight.Value;
		Texture2D tex3 = ModAsset.AcytaeaSword_projectile_glow.Value;
		Vector2 drawCenter = Projectile.Center - Main.screenPosition;

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		if (timer < 30)
		{
			DrawVertexByTwoLine(tex2, new Color(255, 255, 255, 0) * (timer / 30f), diagonal.XY(), diagonal.ZW(), drawCenter + mainVec * drawScale.X, drawCenter + mainVec * drawScale.Y);
		}
		else
		{
			DrawVertexByTwoLine(tex2, new Color(255, 255, 255, 0) * ((120 - timer) / 90f), diagonal.XY(), diagonal.ZW(), drawCenter + mainVec * drawScale.X, drawCenter + mainVec * drawScale.Y);
		}
		DrawVertexByTwoLine(tex, lightColor, diagonal.XY(), diagonal.ZW(), drawCenter + mainVec * drawScale.X, drawCenter + mainVec * drawScale.Y);
		if (timer < 30)
		{
			DrawVertexByTwoLine(tex3, new Color(255, 255, 255, 0) * (timer / 30f), diagonal.XY(), diagonal.ZW(), drawCenter + mainVec * drawScale.X, drawCenter + mainVec * drawScale.Y);
		}
		else
		{
			DrawVertexByTwoLine(tex3, new Color(255, 255, 255, 0) * ((120 - timer) / 90f), diagonal.XY(), diagonal.ZW(), drawCenter + mainVec * drawScale.X, drawCenter + mainVec * drawScale.Y);
		}

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
	}

	public void DrawVertexByTwoLine(Texture2D texture, Color drawColor, Vector2 textureCoordStart, Vector2 textureCoordEnd, Vector2 positionStart, Vector2 positionEnd)
	{
		Vector2 coordVector = textureCoordEnd - textureCoordStart;
		coordVector.X *= texture.Width;
		coordVector.Y *= texture.Height;
		float theta = MathF.Atan2(coordVector.Y, coordVector.X);
		Vector2 drawVector = positionEnd - positionStart;

		Vector2 mainVectorI = drawVector.RotatedBy(theta * -Projectile.spriteDirection) * MathF.Cos(theta);
		Vector2 mainVectorJ = drawVector.RotatedBy((theta - MathHelper.PiOver2) * -Projectile.spriteDirection) * MathF.Sin(theta);

		List<Vertex2D> vertex2Ds = new List<Vertex2D>
		{
			new Vertex2D(positionStart, drawColor, new Vector3(textureCoordStart, 0)),
			new Vertex2D(positionStart + mainVectorI, drawColor, new Vector3(textureCoordEnd.X, textureCoordStart.Y, 0)),

			new Vertex2D(positionStart + mainVectorJ, drawColor, new Vector3(textureCoordStart.X, textureCoordEnd.Y, 0)),
			new Vertex2D(positionEnd, drawColor, new Vector3(textureCoordEnd, 0)),
		};
		Main.graphics.GraphicsDevice.Textures[0] = texture;
		Main.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertex2Ds.ToArray(), 0, vertex2Ds.Count - 2);
	}

	public override bool ShouldUpdatePosition()
	{
		return false;
	}

	public override void CutTiles()
	{
		DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
		var cut = new Terraria.Utils.TileActionAttempt(DelegateMethods.CutTiles);
		Vector2 beamStartPos = Projectile.Center;
		Vector2 beamEndPos = beamStartPos + mainVec;
		Utils.PlotTileLine(beamStartPos, beamEndPos, Projectile.width * Projectile.scale, cut);
	}

	public string TrailShapeTex()
	{
		return Commons.ModAsset.Melee_Mod;
	}

	public string TrailColorTex()
	{
		return "Everglow/Myth/Acytaea/Projectiles/Acytaea_meleeColor";
	}

	public virtual float TrailAlpha(float factor)
	{
		float w;
		w = MathHelper.Lerp(0f, 1, factor);
		return w;
	}

	public BlendState TrailBlendState()
	{
		return BlendState.NonPremultiplied;
	}

	public static Vector2 Vector2Elipse(float radius, float rot0, float rot1, float rot2 = 0, float viewZ = 1000)
	{
		Vector3 v = Vector3.Transform(Vector3.UnitX, Matrix.CreateRotationZ(rot0)) * radius;
		v = Vector3.Transform(v, Matrix.CreateRotationX(-rot1));
		if (rot2 != 0)
		{
			v = Vector3.Transform(v, Matrix.CreateRotationZ(-rot2));
		}

		float k = -viewZ / (v.Z - viewZ);
		return k * new Vector2(v.X, v.Y);
	}

	public void DrawBloom()
	{
		DrawTrail(Color.White);
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
		List<Vector2> SmoothTrailX = GraphicsUtils.CatmullRom(trailVecs.ToList()); // 平滑
		var SmoothTrail = new List<Vector2>();
		for (int x = 0; x < SmoothTrailX.Count - 1; x++)
		{
			SmoothTrail.Add(SmoothTrailX[x]);
		}
		if (trailVecs.Count != 0)
		{
			SmoothTrail.Add(trailVecs.ToArray()[trailVecs.Count - 1]);
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
			float w = 0.1f;
			float d = trail[i].ToRotation() + 3.14f + 1.57f;
			if (d > 6.28f)
			{
				d -= 6.28f;
			}

			float dir = d / MathHelper.TwoPi;

			float dir1 = dir;
			if (i > 0)
			{
				float d1 = trail[i - 1].ToRotation() + 3.14f + 1.57f;
				if (d1 > 6.28f)
				{
					d1 -= 6.28f;
				}

				dir1 = d1 / MathHelper.TwoPi;
			}

			if (dir - dir1 > 0.5)
			{
				var midValue = (1 - dir) / (1 - dir + dir1);
				var midPoint = midValue * trail[i] + (1 - midValue) * trail[i - 1];
				var oldFactor = (i - 1) / (length - 1f);
				var midFactor = midValue * factor + (1 - midValue) * oldFactor;
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(0, w, 0, 1), new Vector3(midFactor, 1, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition + midPoint * Projectile.scale * 1.1f, new Color(0, w, 0, 1), new Vector3(midFactor, 0, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(1, w, 0, 1), new Vector3(midFactor, 1, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition + midPoint * Projectile.scale * 1.1f, new Color(1, w, 0, 1), new Vector3(midFactor, 0, 1)));
			}
			if (dir1 - dir > 0.5)
			{
				var midValue = (1 - dir1) / (1 - dir1 + dir);
				var midPoint = midValue * trail[i - 1] + (1 - midValue) * trail[i];
				var oldFactor = (i - 1) / (length - 1f);
				var midFactor = midValue * oldFactor + (1 - midValue) * factor;
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(1, w, 0, 1), new Vector3(midFactor, 1, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition + midPoint * Projectile.scale * 1.1f, new Color(1, w, 0, 1), new Vector3(midFactor, 0, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(0, w, 0, 1), new Vector3(midFactor, 1, 1)));
				bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition + midPoint * Projectile.scale * 1.1f, new Color(0, w, 0, 1), new Vector3(midFactor, 0, 1)));
			}
			bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(dir, w, 0, 1), new Vector3(factor, 1, w)));
			bars.Add(new Vertex2D(Projectile.Center - Main.screenPosition + trail[i] * Projectile.scale * 1.1f, new Color(dir, w, 0, 1), new Vector3(factor, 0, w)));
		}

		spriteBatch.Draw(ModContent.Request<Texture2D>(Commons.ModAsset.Melee_Warp_Mod).Value, bars, PrimitiveType.TriangleStrip);
	}

	public void End()
	{
		Projectile.Kill();
	}

	public override void OnKill(int timeLeft)
	{
		foreach (var proj in Main.projectile)
		{
			if (proj != null && proj.active)
			{
				if (proj.type == ModContent.ProjectileType<AcytaeaSword_following>())
				{
					return;
				}
			}
		}
		if (OwnerNPC == -1)
		{
			return;
		}
		NPC Owner = Main.npc[OwnerNPC];
		if (Owner == null || !Owner.active)
		{
			return;
		}
		Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<AcytaeaSword_following>(), 0, 0f, -1, OwnerNPC);
	}

	private void GenerateVFX()
	{
		int times = 3;
		for (int x = 0; x < times; x++)
		{
			Vector2 newVec = mainVec;
			Vector2 mainVecLeft = Vector2.Normalize(newVec).RotatedBy(-MathHelper.PiOver2);
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
		for (int x = 0; x < times; x++)
		{
			Vector2 newVec = mainVec;
			Vector2 mainVecLeft = Vector2.Normalize(newVec).RotatedBy(-MathHelper.PiOver2);
			var positionVFX = Projectile.Center + mainVecLeft * Main.rand.NextFloat(-30f, 30f) + newVec * Main.rand.NextFloat(0.7f, 0.9f);

			var acytaeaFlame = new AcytaeaSparkDust
			{
				velocity = -mainVecLeft * Main.rand.NextFloat(6f, 28f) * Projectile.spriteDirection,
				Active = true,
				Visible = true,
				position = positionVFX,
				maxTime = Main.rand.Next(14, 36),
				ai = new float[] { Main.rand.NextFloat(0.1f, 1f), Main.rand.NextFloat(-0.04f, 0.04f), Main.rand.NextFloat(8f, 10f) },
			};
			Ins.VFXManager.Add(acytaeaFlame);
		}
	}

	public void DrawTrail(Color color)
	{
		if (trailVecs.Count <= 1)
		{
			return;
		}
		List<Vector2> SmoothTrailX = GraphicsUtils.CatmullRom(trailVecs.ToList()); // 平滑
		var SmoothTrail = new List<Vector2>();
		for (int x = 0; x <= SmoothTrailX.Count - 1; x++)
		{
			SmoothTrail.Add(SmoothTrailX[x]);
		}
		if (trailVecs.Count != 0)
		{
			SmoothTrail.Add(trailVecs.ToArray()[trailVecs.Count - 1]);
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
		bars.Add(new Vertex2D(Projectile.Center + mainVec * 0.3f * Projectile.scale, Color.White, new Vector3(0, 1, 0f)));
		bars.Add(new Vertex2D(Projectile.Center + mainVec * Projectile.scale, Color.White, new Vector3(0, 0, 1)));
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, TrailBlendState(), SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone);
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.ZoomMatrix;

		Effect MeleeTrail = Commons.ModAsset.MeleeTrail.Value;
		MeleeTrail.Parameters["uTransform"].SetValue(model * projection);
		Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>(TrailShapeTex(), ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

		// Main.graphics.GraphicsDevice.Textures[1] = ModContent.Request<Texture2D>(TrailColorTex(), ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
		MeleeTrail.Parameters["tex1"].SetValue(ModContent.Request<Texture2D>(TrailColorTex(), ReLogic.Content.AssetRequestMode.ImmediateLoad).Value);
		MeleeTrail.CurrentTechnique.Passes[shaderType.ToString()].Apply();

		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo info)
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
		base.OnHitPlayer(target, info);
	}
}

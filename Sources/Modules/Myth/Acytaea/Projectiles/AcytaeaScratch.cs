using Everglow.Commons.DataStructures;
using Everglow.Myth.Acytaea.Buffs;
using Everglow.Myth.Acytaea.VFXs;
using Terraria.DataStructures;

namespace Everglow.Myth.Acytaea.Projectiles;

public class AcytaeaScratch : ModProjectile
{
	public override string Texture => "Everglow/Myth/Acytaea/Projectiles/AcytaeaSword_projectile";
	public override void SetDefaults()
	{
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 60;
		Projectile.extraUpdates = 0;
		Projectile.scale = 1f;
		Projectile.hostile = true;
		Projectile.friendly = false;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.penetrate = -1;
		Projectile.DamageType = DamageClass.Melee;

		Projectile.width = 40;
		Projectile.height = 40;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
	}
	public int OwnerNPC = -1;
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
		if (Projectile.timeLeft > 15)
		{
			Projectile.Center = Owner.Center;
			Projectile.spriteDirection = Owner.spriteDirection;
			for (int x = 0; x < Math.Min(Owner.velocity.Length() * 0.2f - 2, Projectile.timeLeft * 0.5f); x++)
			{
				Vector2 newVec = Owner.velocity * 0.1f + new Vector2(0, Main.rand.NextFloat(0.1f, 0.7f)).RotatedByRandom(6.283);
				if (newVec.Length() > 10)
				{
					newVec = newVec / newVec.Length() * 10f;
				}
				var positionVFX = Projectile.Center + Owner.velocity * Main.rand.NextFloat(0f, 1f) + new Vector2(0, Main.rand.NextFloat(0.1f, 15f)).RotatedByRandom(6.283) + new Vector2(0, -10);

				var acytaeaFlame = new AcytaeaSparkDust
				{
					velocity = newVec,
					Active = true,
					Visible = true,
					position = positionVFX,
					maxTime = Main.rand.Next(24, 56),
					ai = new float[] { Main.rand.NextFloat(0.1f, 1f), Main.rand.NextFloat(-0.01f, 0.01f), Main.rand.NextFloat(0.8f, 1.2f) * newVec.Length() },
				};
				Ins.VFXManager.Add(acytaeaFlame);
			}
		}
	}
	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		for (int k = 0; k < Projectile.oldPos.Length; k++)
		{
			Vector2 deltaVector = Projectile.oldPos[k] - Projectile.position;
			Rectangle newProjectileHitBox = projHitbox;
			newProjectileHitBox.X -= (int)deltaVector.X;
			newProjectileHitBox.Y -= (int)deltaVector.Y;
			if (newProjectileHitBox.Intersects(targetHitbox))
			{
				return true;
			}
		}
		return false;
	}
	public override void OnSpawn(IEntitySource source)
	{
		OwnerNPC = (int)Projectile.ai[0];
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
		for (int x = 0; x < 60; x++)
		{
			Vector2 newVec = new Vector2(0, Main.rand.NextFloat(4f, 12f)).RotatedByRandom(6.238f);
			var positionVFX = target.Center + newVec * Main.rand.NextFloat(0.7f, 0.9f);

			var acytaeaSpark = new AcytaeaSparkDust
			{
				velocity = newVec,
				Active = true,
				Visible = true,
				position = positionVFX,
				maxTime = Main.rand.Next(14, 30),
				ai = new float[] { Main.rand.NextFloat(0.1f, 1f), Main.rand.NextFloat(-0.01f, 0.01f), Main.rand.NextFloat(8f, 10f) },
			};
			Ins.VFXManager.Add(acytaeaSpark);
		}
		target.AddBuff(ModContent.BuffType<AcytaeaInferno>(), 450);
		base.OnHitPlayer(target, info);
	}
	public override bool PreDraw(ref Color lightColor)
	{
		return false;
	}
	public override void PostDraw(Color lightColor)
	{
		DrawTrail();
	}
	public virtual void DrawTrail()
	{
		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		DrawDark();
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		DrawLight();
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
	}
	private void DrawLight()
	{
		for (int z = 0; z < 3; z++)
		{
			List<Vector2> oldPosNoneZero = new List<Vector2>();
			for (int x = 0; x < Projectile.oldPos.Length; x++)
			{
				if (x >= 1)
				{
					if (Projectile.oldPos[x] == Projectile.oldPos[x - 1])
					{
						continue;
					}
				}
				if (Projectile.oldPos[x] == Vector2.zeroVector)
				{
					break;
				}
				oldPosNoneZero.Add(Projectile.oldPos[x]);
			}
			if (oldPosNoneZero.Count <= 1)
			{
				return;
			}
			List<Vector2> SmoothTrailX = GraphicsUtils.CatmullRom(oldPosNoneZero.ToArray()); // 平滑
			var SmoothTrail = new List<Vector2>();
			for (int x = 0; x <= SmoothTrailX.Count - 1; x++)
			{
				SmoothTrail.Add(SmoothTrailX[x]);
			}
			SmoothTrail.Add(oldPosNoneZero.Last());

			int length = SmoothTrail.Count;
			if (length <= 3)
				return;
			Vector2[] trail = SmoothTrail.ToArray();

			List<Vertex2D> bars = new List<Vertex2D>();
			Vector2 offset = Vector2.zeroVector;
			if (z == 2)
			{
				offset = new Vector2(18 * Projectile.spriteDirection, -12);
			}
			for (int i = 1; i < length; ++i)
			{
				float width = 12;
				if (Projectile.timeLeft < 20)
				{
					width = Projectile.timeLeft * 1.8f;
				}
				var normalDir = trail[i - 1] - trail[i];
				normalDir = new Vector2(-normalDir.Y, normalDir.X).SafeNormalize(Vector2.Zero);
				var factor = (i - 1) / (float)(length - 3);
				var color = new Color(255, 0, 65, 0) * MathF.Sin(factor * MathF.PI) * MathF.Sin(factor * MathF.PI);
				if (z == 1)
				{
					offset = -normalDir * 24f * Projectile.spriteDirection;
				}
				bars.Add(new Vertex2D(trail[i] - normalDir * width + offset + new Vector2(Projectile.width / 2f), color, new Vector3(factor * 2, 0.5f - Projectile.spriteDirection * 0.5f, factor)));
				bars.Add(new Vertex2D(trail[i] + normalDir * width + offset + new Vector2(Projectile.width / 2f), color, new Vector3(factor * 2, 0.5f + Projectile.spriteDirection * 0.5f, factor)));
			}
			if (bars.Count > 2)
			{
				Texture2D t = ModAsset.Trail_Scratch.Value;
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
				Effect effect = ModAsset.AcytaeaScratchEffect.Value;
				var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
				var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
				effect.Parameters["uTransform"].SetValue(model * projection);
				effect.Parameters["uProcession"].SetValue(0.5f);
				effect.CurrentTechnique.Passes["Test2"].Apply();
				Main.graphics.GraphicsDevice.Textures[0] = t;
				Main.graphics.GraphicsDevice.Textures[1] = ModAsset.Acytaea_meleeColor.Value;
				Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			}
		}
	}
	private void DrawDark()
	{
		for (int z = 0; z < 3; z++)
		{
			List<Vector2> oldPosNoneZero = new List<Vector2>();
			for (int x = 0; x < Projectile.oldPos.Length; x++)
			{
				if (x >= 1)
				{
					if (Projectile.oldPos[x] == Projectile.oldPos[x - 1])
					{
						continue;
					}
				}
				if (Projectile.oldPos[x] == Vector2.zeroVector)
				{
					break;
				}
				oldPosNoneZero.Add(Projectile.oldPos[x]);
			}
			if (oldPosNoneZero.Count <= 1)
			{
				return;
			}
			List<Vector2> SmoothTrailX = GraphicsUtils.CatmullRom(oldPosNoneZero.ToArray()); // 平滑
			var SmoothTrail = new List<Vector2>();
			for (int x = 0; x <= SmoothTrailX.Count - 1; x++)
			{
				SmoothTrail.Add(SmoothTrailX[x]);
			}
			SmoothTrail.Add(oldPosNoneZero.Last());

			int length = SmoothTrail.Count;
			if (length <= 3)
				return;
			Vector2[] trail = SmoothTrail.ToArray();

			List<Vertex2D> bars = new List<Vertex2D>();
			Vector2 offset = Vector2.zeroVector;
			if (z == 2)
			{
				offset = new Vector2(18 * Projectile.spriteDirection, -12);
			}
			var color = Color.White;
			for (int i = 1; i < length; ++i)
			{
				float width = 12;
				if (Projectile.timeLeft < 20)
				{
					width = Projectile.timeLeft * 1.8f;
				}
				var normalDir = trail[i - 1] - trail[i];
				normalDir = new Vector2(-normalDir.Y, normalDir.X).SafeNormalize(Vector2.Zero);

				var factor = (i - 1) / (float)(length - 3);
				if (z == 1)
				{
					offset = -normalDir * 24f * Projectile.spriteDirection;
				}
				bars.Add(new Vertex2D(trail[i] - normalDir * width + offset + new Vector2(Projectile.width / 2f), color, new Vector3(factor, 0.5f - Projectile.spriteDirection * 0.5f, factor)));
				bars.Add(new Vertex2D(trail[i] + normalDir * width + offset + new Vector2(Projectile.width / 2f), color, new Vector3(factor, 0.5f + Projectile.spriteDirection * 0.5f, factor)));

			}

			if (bars.Count > 2)
			{
				Texture2D t = Commons.ModAsset.Trail_black.Value;
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
				Effect effect = ModAsset.AcytaeaScratchEffect.Value;
				var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
				var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
				effect.Parameters["uTransform"].SetValue(model * projection);
				effect.Parameters["uProcession"].SetValue(0.5f);
				effect.CurrentTechnique.Passes["Test"].Apply();
				Main.graphics.GraphicsDevice.Textures[0] = t;
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			}
		}
	}
}

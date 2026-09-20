using Everglow.Commons.DataStructures;
using Everglow.Commons.Graphics;
using Everglow.Commons.Templates.Weapons;
using Everglow.Yggdrasil.KelpCurtain.VFXs;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Magic;

public class EmptyWaterStaff_proj : TrailingProjectile
{
	public override void SetCustomDefaults()
	{
		TrailColor = new Color(0.85f, 0.75f, 0.65f, 0f);
		TrailWidth = 12f;
		TrailTexture = Commons.ModAsset.Trail_8.Value;
		TrailTextureBlack = Commons.ModAsset.Trail_8_black.Value;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;

		Projectile.DamageType = DamageClass.Magic;
		Projectile.width = 20;
		Projectile.height = 20;
		ProjTrailColor.colorList.Add((new Color(15, 231, 255, 0), 0));
		ProjTrailColor.colorList.Add((new Color(63, 181, 255, 0), 0.4f));
		ProjTrailColor.colorList.Add((new Color(69, 112, 255, 0), 0.8f));
		ProjTrailColor.colorList.Add((new Color(36, 46, 76, 0), 1f));
	}

	public GradientColor ProjTrailColor = new();

	public override void AI()
	{
		base.AI();
		if (Main.rand.NextBool(4))
		{
			Vector2 vel = new Vector2(0, Main.rand.NextFloat(0.6f, 1.4f)).RotatedByRandom(MathHelper.TwoPi) + Projectile.velocity;
			var dust = new EmptyWaterStaff_BubbleBreak
			{
				velocity = vel,
				Active = true,
				Visible = true,
				position = Projectile.Center,
				maxTime = Main.rand.Next(60, 90),
				scale = Main.rand.NextFloat(3f, 5f),
				rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(4.0f, 10.93f) },
			};
			Ins.VFXManager.Add(dust);
		}
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		bool safe = true;
		foreach (var proj in Main.projectile)
		{
			if (proj is not null && proj.active && proj.type == ModContent.ProjectileType<EmptyWaterStaff_proj_bubble>())
			{
				EmptyWaterStaff_proj_bubble eWS = proj.ModProjectile as EmptyWaterStaff_proj_bubble;
				if (eWS is not null)
				{
					if (eWS.Target == target)
					{
						safe = false;
						break;
					}
				}
			}
		}
		if (safe)
		{
			Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), target.Center, Vector2.zeroVector, ModContent.ProjectileType<EmptyWaterStaff_proj_bubble>(), Projectile.damage, 0, Projectile.owner, target.whoAmI);
		}
	}

	public override void DestroyEntity()
	{
		for (int i = 0; i < 12; i++)
		{
			Vector2 vel = new Vector2(0, Main.rand.NextFloat(5.6f, 8.4f)).RotatedByRandom(MathHelper.TwoPi);
			var dust = new EmptyWaterStaff_BubbleBreak
			{
				velocity = vel,
				Active = true,
				Visible = true,
				position = Projectile.Center,
				maxTime = Main.rand.Next(60, 120),
				scale = Main.rand.NextFloat(3f, 5f),
				rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(4.0f, 10.93f) },
			};
			Ins.VFXManager.Add(dust);
		}
		base.DestroyEntity();
	}

	public override bool PreDraw(ref Color lightColor)
	{
		DrawTrail();
		if (TimeAfterEntityDestroy <= 0)
		{
			DrawSelf();
		}
		return false;
	}

	public override void DrawSelf()
	{
		var texMain = (Texture2D)ModContent.Request<Texture2D>(Texture);
		var color = new Color(1f, 1f, 1f, 0);
		Main.spriteBatch.Draw(texMain, Projectile.Center - Main.screenPosition - Projectile.velocity, null, color, Projectile.velocity.ToRotation() - MathHelper.PiOver2, texMain.Size() / 2f, 0.6f, SpriteEffects.None, 0);
	}

	public override Vector3 ModifyTrailTextureCoordinate(float factor, float timeValue, float phase, float widthValue)
	{
		float x = factor - timeValue * 0.6f;
		float y = 1;
		float z = widthValue;
		if (phase == 2)
		{
			y = 0;
		}
		if (phase % 2 == 1)
		{
			y = 0.5f;
		}
		return new Vector3(x, y, z);
	}

	public override Color GetTrailColor(int style, Vector2 worldPos, int index, ref float factor, float extraValue0 = 0, float extraValue1 = 0)
	{
		if (style == 1)
		{
			return ProjTrailColor.GetColor(factor);
		}
		else
		{
			return base.GetTrailColor(style, worldPos, index, ref factor, extraValue0, extraValue1);
		}
	}

	//public override void DrawTrail()
	//{
	//	var unSmoothPos = new List<Vector2>();
	//	for (int i = 0; i < Projectile.oldPos.Length; ++i)
	//	{
	//		if (Projectile.oldPos[i] == Vector2.Zero)
	//		{
	//			break;
	//		}

	//		unSmoothPos.Add(Projectile.oldPos[i]);
	//	}
	//	List<Vector2> smoothTrail_current = GraphicsUtils.CatmullRom(unSmoothPos); // 平滑
	//	var SmoothTrail = new List<Vector2>();
	//	for (int x = 0; x < smoothTrail_current.Count - 1; x++)
	//	{
	//		SmoothTrail.Add(smoothTrail_current[x]);
	//	}
	//	if (unSmoothPos.Count != 0)
	//	{
	//		SmoothTrail.Add(unSmoothPos[unSmoothPos.Count - 1]);
	//	}

	//	Vector2 halfSize = new Vector2(Projectile.width, Projectile.height) / 2f;
	//	var bars = new List<Vertex2D>();
	//	var bars2 = new List<Vertex2D>();
	//	var bars3 = new List<Vertex2D>();
	//	for (int i = 1; i < SmoothTrail.Count; ++i)
	//	{
	//		float mulFac = Timer / (float)ProjectileID.Sets.TrailCacheLength[Projectile.type];
	//		if (mulFac > 1f)
	//		{
	//			mulFac = 1f;
	//		}
	//		float factor = i / (float)SmoothTrail.Count * mulFac;
	//		float width = TrailWidthFunction(factor);
	//		float timeValue = (float)Main.time * 0.06f;

	//		Vector2 drawPos = SmoothTrail[i] + halfSize;
	//		Color drawC = ProjTrailColor.GetColor(i / (float)SmoothTrail.Count);
	//		factor *= 1.5f;
	//		bars.Add(new Vertex2D(drawPos + new Vector2(0, 1).RotatedBy(MathHelper.TwoPi * 2f / 3f) * TrailWidth, drawC, new Vector3(-factor * 2 + timeValue, 1, width)));
	//		bars.Add(new Vertex2D(drawPos, drawC, new Vector3(-factor * 2 + timeValue, 0.5f, width)));
	//		bars2.Add(new Vertex2D(drawPos + new Vector2(0, 1).RotatedBy(MathHelper.TwoPi * 1f / 3f) * TrailWidth, drawC, new Vector3(-factor * 2 + timeValue, 0, width)));
	//		bars2.Add(new Vertex2D(drawPos, drawC, new Vector3(-factor * 2 + timeValue, 0.5f, width)));
	//		bars3.Add(new Vertex2D(drawPos + new Vector2(0, 1).RotatedBy(MathHelper.TwoPi * 0f / 3f) * TrailWidth, drawC, new Vector3(-factor * 2 + timeValue, 1, width)));
	//		bars3.Add(new Vertex2D(drawPos, drawC, new Vector3(-factor * 2 + timeValue, 0.5f, width)));
	//	}
	//	SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
	//	Main.spriteBatch.End();
	//	Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
	//	Effect effect = TrailShader;
	//	var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
	//	var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
	//	effect.Parameters["uTransform"].SetValue(model * projection);
	//	effect.CurrentTechnique.Passes[0].Apply();
	//	Main.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
	//	Main.graphics.GraphicsDevice.Textures[0] = TrailTexture;
	//	Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
	//	if (bars.Count > 3)
	//	{
	//		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
	//	}

	//	if (bars2.Count > 3)
	//	{
	//		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars2.ToArray(), 0, bars2.Count - 2);
	//	}

	//	if (bars3.Count > 3)
	//	{
	//		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars3.ToArray(), 0, bars3.Count - 2);
	//	}

	//	Main.spriteBatch.End();
	//	Main.spriteBatch.Begin(sBS);
	//}

	//public override void DrawTrailDark()
	//{
	//	var unSmoothPos = new List<Vector2>();
	//	for (int i = 0; i < Projectile.oldPos.Length; ++i)
	//	{
	//		if (Projectile.oldPos[i] == Vector2.Zero)
	//		{
	//			break;
	//		}

	//		unSmoothPos.Add(Projectile.oldPos[i]);
	//	}
	//	List<Vector2> smoothTrail_current = GraphicsUtils.CatmullRom(unSmoothPos); // 平滑
	//	var SmoothTrail = new List<Vector2>();
	//	for (int x = 0; x < smoothTrail_current.Count - 1; x++)
	//	{
	//		SmoothTrail.Add(smoothTrail_current[x]);
	//	}
	//	if (unSmoothPos.Count != 0)
	//	{
	//		SmoothTrail.Add(unSmoothPos[unSmoothPos.Count - 1]);
	//	}

	//	Vector2 halfSize = new Vector2(Projectile.width, Projectile.height) / 2f;
	//	var bars = new List<Vertex2D>();
	//	var bars2 = new List<Vertex2D>();
	//	var bars3 = new List<Vertex2D>();
	//	for (int i = SmoothTrail.Count - 1; i > 0; --i)
	//	{
	//		float mulFac = Timer / (float)ProjectileID.Sets.TrailCacheLength[Projectile.type];
	//		if (mulFac > 1f)
	//		{
	//			mulFac = 1f;
	//		}
	//		float factor = i / (float)SmoothTrail.Count * mulFac;
	//		float width = TrailWidthFunction(factor);
	//		float timeValue = (float)Main.time * 0.06f;

	//		Vector2 drawPos = SmoothTrail[i] + halfSize;
	//		Color drawC = Color.White;
	//		factor *= 1.5f;
	//		bars.Add(new Vertex2D(drawPos + new Vector2(0, 1).RotatedBy(MathHelper.TwoPi * 2f / 3f) * TrailWidth, drawC, new Vector3(-factor * 2 + timeValue, 1, width)));
	//		bars.Add(new Vertex2D(drawPos, drawC, new Vector3(-factor * 2 + timeValue, 0.5f, width)));
	//		bars2.Add(new Vertex2D(drawPos + new Vector2(0, 1).RotatedBy(MathHelper.TwoPi * 1f / 3f) * TrailWidth, drawC, new Vector3(-factor * 2 + timeValue, 0, width)));
	//		bars2.Add(new Vertex2D(drawPos, drawC, new Vector3(-factor * 2 + timeValue, 0.5f, width)));
	//		bars3.Add(new Vertex2D(drawPos + new Vector2(0, 1).RotatedBy(MathHelper.TwoPi * 0f / 3f) * TrailWidth, drawC, new Vector3(-factor * 2 + timeValue, 1, width)));
	//		bars3.Add(new Vertex2D(drawPos, drawC, new Vector3(-factor * 2 + timeValue, 0.5f, width)));
	//	}
	//	SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
	//	Main.spriteBatch.End();
	//	Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
	//	Effect effect = TrailShader;
	//	var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
	//	var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
	//	effect.Parameters["uTransform"].SetValue(model * projection);
	//	effect.CurrentTechnique.Passes[0].Apply();
	//	Main.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
	//	Main.graphics.GraphicsDevice.Textures[0] = TrailTextureBlack;
	//	Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
	//	if (bars.Count > 3)
	//	{
	//		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
	//	}

	//	if (bars2.Count > 3)
	//	{
	//		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars2.ToArray(), 0, bars2.Count - 2);
	//	}

	//	if (bars3.Count > 3)
	//	{
	//		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars3.ToArray(), 0, bars3.Count - 2);
	//	}

	//	Main.spriteBatch.End();
	//	Main.spriteBatch.Begin(sBS);
	//}
}

using Everglow.Commons.DataStructures;
using Everglow.Commons.MEAC;
using Everglow.Commons.Templates.Weapons.StabbingSwords.VFX;
using Everglow.Commons.Utilities;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;
using Everglow.Commons.VFX.CommonVFXDusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Shaders;

namespace Everglow.EternalResolve.Bosses.Projectiles
{
	public abstract class Rapier_Hostile_Stab : ModProjectile, IWarpProjectile
	{
		public int ItemType;

		/// <summary>
		/// 常规颜色
		/// </summary>
		public Color Color = Color.White;

		/// <summary>
		/// 阴影强度
		/// </summary>
		public float CurrentColorFactor = 0f;

		/// <summary>
		/// 重影深度
		/// </summary>
		public float OldColorFactor = 0f;

		/// <summary>
		/// 重影彩色部分亮度
		/// </summary>
		public float OldLightColorValue = 0f;

		/// <summary>
		/// 重影大小缩变,小于1
		/// </summary>
		public float ScaleMultiplicative_Modifier = 0f;

		/// <summary>
		/// 刀光宽度1
		/// </summary>
		public float AttackEffectWidth = 1f;

		/// <summary>
		/// 重影深度缩变,小于1
		/// </summary>
		public float ShadeMultiplicative_Modifier = 0f;

		/// <summary>
		/// 重影彩色部分亮度缩变,小于1
		/// </summary>
		public float LightColorValueMultiplicative_Modifier = 0f;

		/// <summary>
		/// 表示刺剑攻击长度,标准长度1
		/// </summary>
		public float AttackLength = 1f;

		public override string Texture => Commons.ModAsset.StabbingProjectile_Mod;

		public override void SetDefaults()
		{
			Projectile.width = 10;
			Projectile.height = 24;
			Projectile.netImportant = true;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 80;
			Projectile.extraUpdates = 5;
			Projectile.tileCollide = false;
			Projectile.aiStyle = -1;
		}

		public Vector2 StartCenter = Vector2.zeroVector;
		public Vector2 EndPos = Vector2.zeroVector;
		public int ToKill = 120;

		public override void OnSpawn(IEntitySource source)
		{
			SoundStyle ss = new SoundStyle(Commons.ModAsset.StabbingSwordSound_Mod);
			SoundEngine.PlaySound(ss, Projectile.Center);
			StartCenter = Projectile.Center;

			if (Projectile.ai[1] == 1 || Projectile.ai[1] == 2)
			{
				Projectile.timeLeft = 40;
				Projectile.extraUpdates++;
			}
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			float point = 0;
			Vector2 end = Projectile.Center + Projectile.velocity * 80 * AttackLength;
			if (EndPos != Vector2.zeroVector)
			{
				end = EndPos;
			}
			if (Projectile.timeLeft > 2)
			{
				if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), StartCenter, end, Projectile.width, ref point))
				{
					return true;
				}
			}
			return false;
		}

		public override void AI()
		{
			if (Projectile.timeLeft % 15 == 0 && Projectile.ai[1] != 2)
			{
				StabVFX v = new StabVFX()
				{
					Position = Projectile.Center + Projectile.velocity * AttackLength * 140,
					Velocity = Vector2.Normalize(Projectile.velocity),
					StabEffectColor = Color.Lerp(Color, Color.White, 0.2f),
					Scale = 10,
					MaxTime = 20,
					TimeLeft = 20,
					Alpha = 0.8f,
				};
				Ins.VFXManager.Add(v);
			}
			ProduceWaterRipples(new Vector2(Projectile.velocity.Length(), 30));
			if (Projectile.ai[1] == 0)
			{
				Main.npc[(int)Projectile.ai[0]].spriteDirection = Projectile.spriteDirection;
				Projectile.Center = Main.npc[(int)Projectile.ai[0]].Center + Projectile.velocity * 50 * AttackLength;
			}
			else
			{
				if (Projectile.timeLeft > 2)
				{
					Projectile.Center += Projectile.velocity * 8;
				}
			}

			if (Projectile.timeLeft <= 2)
			{
				ToKill--;
				Projectile.timeLeft = 2;
			}
			if (ToKill >= 120)
			{
				Projectile.rotation = Projectile.velocity.ToRotation();
				Projectile.spriteDirection = Projectile.direction;
			}
			else
			{
				Projectile.extraUpdates = 24;
				if (ToKill < 0)
				{
					Projectile.Kill();
				}
			}
			Vector2 end = Projectile.Center + Projectile.velocity * 100 * AttackLength;
			if (!Collision.CanHitLine(StartCenter, 1, 1, end, 1, 1))
			{
				if (EndPos == Vector2.zeroVector)
				{
					EndPos = end;
					HitTile();
				}
			}
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
					Position = EndPos,
					MaxTime = Main.rand.Next(1, 25),
					Scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(10f, 27.0f)),
					Rotation = Main.rand.NextFloat(6.283f),
					ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.13f, 0.13f) },
				};
				Ins.VFXManager.Add(spark);
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

		public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
		{
			DrawItem(lightColor);
			return false;
		}

		public virtual void DrawItem(Color lightColor)
		{
			int type = ItemType;
			Texture2D itemTexture = TextureAssets.Item[type].Value;
			Main.spriteBatch.Draw(itemTexture, Main.npc[(int)Projectile.ai[0]].Center + Projectile.velocity * 40 - Main.screenPosition, null, lightColor, Projectile.rotation + MathF.PI * 0.25f, itemTexture.Size() / 2f, 1, SpriteEffects.None, 0f);
		}

		public override void PostDraw(Player player, Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
		{
			DrawEffect(lightColor);
		}

		public virtual void DrawEffect(Color lightColor)
		{
			Vector2 normalized = Vector2.Normalize(Projectile.velocity.RotatedBy(Math.PI * 0.5)) * 25 * ToKill / 120f * AttackEffectWidth;
			Vector2 start = StartCenter;
			Vector2 end = Projectile.Center + Projectile.velocity * 100 * AttackLength;
			if (EndPos != Vector2.Zero)
			{
				end = EndPos;
			}
			float value = (Projectile.timeLeft + ToKill) / 200f;
			Vector2 middle = Vector2.Lerp(end, start, MathF.Sqrt(value) * 0.5f);
			float time = (float)(Main.time * 0.03);
			float dark = MathF.Sin(value * MathF.PI) * 4;
			List<Vertex2D> bars = new List<Vertex2D>
			{
				new Vertex2D(start + normalized, new Color(0, 0, 0, 120) * 0.4f * CurrentColorFactor, new Vector3(1 + time, 0, 0)),
				new Vertex2D(start - normalized, new Color(0, 0, 0, 120) * 0.4f * CurrentColorFactor, new Vector3(1 + time, 1, 0)),
				new Vertex2D(middle + normalized, Color.White * 0.4f * dark * CurrentColorFactor, new Vector3(0.5f + time, 0, 0.5f)),
				new Vertex2D(middle - normalized, Color.White * 0.4f * dark * CurrentColorFactor, new Vector3(0.5f + time, 1, 0.5f)),
				new Vertex2D(end + normalized, Color.White * 0.9f * dark * CurrentColorFactor, new Vector3(0f + time, 0, 1)),
				new Vertex2D(end - normalized, Color.White * 0.9f * dark * CurrentColorFactor, new Vector3(0f + time, 1, 1)),
			};
			if (bars.Count >= 3)
			{
				SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
				Effect effect = Commons.ModAsset.StabSwordEffect.Value;
				var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
				var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
				effect.Parameters["uTransform"].SetValue(model * projection);
				effect.Parameters["uProcession"].SetValue(0.21f);
				effect.CurrentTechnique.Passes[0].Apply();
				Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail_black.Value;
				Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(sBS);
			}
			Color alphaColor = Color;
			alphaColor.A = 0;
			alphaColor.R = (byte)(alphaColor.R * lightColor.R / 255f);
			alphaColor.G = (byte)(alphaColor.G * lightColor.G / 255f);
			alphaColor.B = (byte)(alphaColor.B * lightColor.B / 255f);

			normalized = Vector2.Normalize(Projectile.velocity.RotatedBy(Math.PI * 0.5)) * 36 * ToKill / 120f * AttackEffectWidth;
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
				SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
				Effect effect = Commons.ModAsset.StabSwordEffect.Value;
				var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
				var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
				effect.Parameters["uTransform"].SetValue(model * projection);
				effect.Parameters["uProcession"].SetValue(value * 1.1f);
				effect.CurrentTechnique.Passes[0].Apply();
				Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail_1.Value;
				Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(sBS);
			}

			alphaColor.A = 0;
			alphaColor.R = (byte)(565 * lightColor.R / 255f);
			alphaColor.G = (byte)(565 * lightColor.G / 255f);
			alphaColor.B = (byte)(565 * lightColor.B / 255f);
			normalized = Vector2.Normalize(Projectile.velocity.RotatedBy(Math.PI * 0.5)) * 24 * ToKill / 120f * AttackEffectWidth;
			bars = new List<Vertex2D>
			{
				new Vertex2D(start + normalized, new Color(0, 0, 0, 0), new Vector3(1 + time, 0, 0)),
				new Vertex2D(start - normalized, new Color(0, 0, 0, 0), new Vector3(1 + time, 1, 0)),
				new Vertex2D(middle + normalized, alphaColor, new Vector3(0.5f + time, 0, 0.5f)),
				new Vertex2D(middle - normalized, alphaColor, new Vector3(0.5f + time, 1, 0.5f)),
				new Vertex2D(end + normalized, alphaColor, new Vector3(0f + time, 0, 1)),
				new Vertex2D(end - normalized, alphaColor, new Vector3(0f + time, 1, 1)),
			};
			if (bars.Count >= 3)
			{
				SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
				Effect effect = Commons.ModAsset.StabSwordEffect.Value;
				var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
				var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
				effect.Parameters["uTransform"].SetValue(model * projection);
				effect.Parameters["uProcession"].SetValue(value * 0.6f + 0.4f);
				effect.CurrentTechnique.Passes[0].Apply();
				Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail.Value;
				Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(sBS);
			}
		}

		public void DrawWarp(VFXBatch sb)
		{
			Vector2 normalized = Vector2.Normalize(Projectile.velocity.RotatedBy(Math.PI * 0.5)) * 20 * ToKill / 120f * AttackEffectWidth;
			Vector2 start = StartCenter;
			Vector2 end = Projectile.Center + Projectile.velocity * 100 * AttackLength;
			if (EndPos != Vector2.Zero)
			{
				end = EndPos;
			}
			float value = (Projectile.timeLeft + ToKill) / 135f;
			Vector2 middle = Vector2.Lerp(end, start, MathF.Sqrt(value) * 0.5f);
			float time = (float)(Main.time * 0.03);
			Color alphaColor = Color;
			alphaColor.A = 0;
			alphaColor.R = (byte)(((Projectile.velocity.ToRotation() + Math.PI * 3) % MathHelper.TwoPi) / MathHelper.TwoPi * 255);
			alphaColor.G = 12;

			List<Vertex2D> bars = new List<Vertex2D>
			{
				new Vertex2D(start + normalized - Main.screenPosition, new Color(alphaColor.R, 0, 0, 0), new Vector3(1 + time, 0, 0)),
				new Vertex2D(start - normalized - Main.screenPosition, new Color(alphaColor.R, 0, 0, 0), new Vector3(1 + time, 1, 0)),
				new Vertex2D(middle + normalized - Main.screenPosition, alphaColor, new Vector3(0.5f + time, 0, 0.5f)),
				new Vertex2D(middle - normalized - Main.screenPosition, alphaColor, new Vector3(0.5f + time, 1, 0.5f)),
				new Vertex2D(end - Main.screenPosition, alphaColor, new Vector3(0f + time, 0.5f, 1)),
				new Vertex2D(end - Main.screenPosition, alphaColor, new Vector3(0f + time, 0.5f, 1)),
			};
			sb.Draw(Commons.ModAsset.Trail_1.Value, bars, PrimitiveType.TriangleStrip);
		}
	}
}

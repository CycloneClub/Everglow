using Everglow.Commons.Vertex;
using Everglow.SpellAndSkull.Common;
using Everglow.SpellAndSkull.Projectiles.BlackHole.Dusts;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace Everglow.SpellAndSkull.Projectiles.BlackHole;

// [Pipeline(typeof(BlackHolePipeline))]
internal class BlackHole : ModProjectile
{
	public static Projectile proj; // 只能存在一个

	public override void SetDefaults()
	{
		Projectile.width = Projectile.height = 200;
		Projectile.scale = 0;
		Projectile.aiStyle = -1;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 180;
		Projectile.friendly = true;
		Projectile.penetrate = -1;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 30;
		Projectile.DamageType = DamageClass.Magic;
		ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 5000;
	}

	public static bool ProjActive()
	{
		if (proj != null)
		{
			return proj.active && Main.projectile[proj.whoAmI].active && Main.projectile[proj.whoAmI].type == ModContent.ProjectileType<BlackHole>();
		}
		else
		{
			return false;
		}
	}

	public override void OnSpawn(IEntitySource source)
	{
		if (ProjActive() && proj != Projectile)
		{
			Projectile.Kill();
		}
		else
		{
			proj = Projectile;
		}
	}

	public override void AI()
	{
		if (Projectile.timeLeft > 20)
		{
			proj.scale = MathHelper.Lerp(proj.scale, 280 * Projectile.ai[0], 0.1f);
		}
		else
		{
			proj.scale = MathHelper.Lerp(proj.scale, 0, 0.25f);
		}

		if (Main.rand.NextBool(6))// 暗色粒子
		{
			var d = new DarkDust() { position = Projectile.Center + Main.rand.NextVector2Unit() * 30, velocity = Main.rand.NextVector2Unit() * 6, scale = 0.8f, time_max = 30 };
			Ins.VFXManager.Add(d);
		}
		if (Main.rand.NextBool(4))// 光亮粒子
		{
			var c = new Color(0.2f, 0.7f, 1f); // 颜色
			var d = new LightDust() { drawColor = c, position = Projectile.Center + Main.rand.NextVector2Unit() * 30, velocity = Main.rand.NextVector2Unit() * 6, scale = 0.2f, time_max = 30 };
			Ins.VFXManager.Add(d);
		}
		AbsorbMonster();
		ScreenShake();
	}

	private void ScreenShake()
	{
		Player player = Main.player[Projectile.owner];
		float kPlayerCenter = (player.Center - Projectile.Center).Length();
		kPlayerCenter = 100f / (kPlayerCenter + 100f);
		ScreenShaker Gsplayer = Main.player[Projectile.owner].GetModPlayer<ScreenShaker>();
		Gsplayer.FlyCamPosition = new Vector2(0, Projectile.ai[0] * Projectile.scale * kPlayerCenter).RotatedByRandom(6.283);
	}

	private void AbsorbMonster()
	{
		float MinDis = 2550 * Projectile.ai[0];
		float MaxSpeed = 100f * Projectile.ai[0];
		if (Projectile.timeLeft < 20f)
		{
			MaxSpeed = Projectile.timeLeft * 5f;
		}

		foreach (var target in Main.npc)
		{
			if (target.active)
			{
				if (!target.dontTakeDamage && !target.friendly)
				{
					if (target.type == NPCID.TargetDummy)
					{
						continue;
					}

					if (target.velocity.Length() <= 0.001f)
					{
						continue;
					}

					Vector2 ToTarget = target.Center - Projectile.Center;
					float dis = ToTarget.Length();
					if (dis < MinDis && ToTarget != Vector2.Zero)
					{
						float mess = target.width * target.height;
						mess = (float)Math.Sqrt(mess);
						Vector2 Addvel = Vector2.Normalize(ToTarget) / mess / (dis + 10) * 40000f * (target.knockBackResist + 0.3f) * Projectile.ai[0];
						if (!target.noGravity)
						{
							Addvel.Y *= 30f;
						}

						target.velocity -= Addvel;
						float kSpeed = 1f;
						if (dis < 100)
						{
							kSpeed = (dis + 100) / 200f;
						}

						if (target.velocity.Length() > MaxSpeed * kSpeed)
						{
							target.velocity *= MaxSpeed * kSpeed / target.velocity.Length();
						}
					}
				}
			}
		}
		foreach (var target in Main.item)
		{
			if (target.active)
			{
				Vector2 ToTarget = target.Center - Projectile.Center;
				float dis = ToTarget.Length();
				if (dis < MinDis && ToTarget != Vector2.Zero)
				{
					if (dis < 45)
					{
						if (target.type is >= ItemID.CopperCoin and <= ItemID.PlatinumCoin or ItemID.Star or ItemID.Heart)
						{
							target.position = Main.player[Projectile.owner].Center;
						}
					}
					if ((target.position - Main.player[Projectile.owner].Center).Length() < 75)
					{
						continue;
					}

					float mess = target.width * target.height;
					mess = (float)Math.Sqrt(mess);
					Vector2 Addvel = Vector2.Normalize(ToTarget) / mess / (dis + 10) * 20000f * Projectile.ai[0];
					target.velocity -= Addvel;
					float kSpeed = 1f;
					if (dis < 100)
					{
						kSpeed = (dis + 100) / 200f;
					}

					if (target.velocity.Length() > MaxSpeed * kSpeed)
					{
						target.velocity *= MaxSpeed * kSpeed / target.velocity.Length();
					}
				}
			}
		}
		foreach (var target in Main.gore)
		{
			if (target.active)
			{
				Vector2 ToTarget = target.position - Projectile.Center;
				float dis = ToTarget.Length();
				if (dis < MinDis && ToTarget != Vector2.Zero)
				{
					float mess = target.Width * target.Height;
					mess = (float)Math.Sqrt(mess);
					Vector2 Addvel = Vector2.Normalize(ToTarget) / mess / (dis + 10) * 40000f * Projectile.ai[0];
					target.velocity -= Addvel;
					float kSpeed = 1f;
					if (dis < 100)
					{
						kSpeed = (dis + 100) / 200f;
					}

					if (target.velocity.Length() > MaxSpeed * kSpeed)
					{
						target.velocity *= MaxSpeed * kSpeed / target.velocity.Length();
					}

					target.timeLeft -= 24;
				}
			}
		}
		foreach (var target in Main.player)
		{
			if (target.active)
			{
				Vector2 ToTarget = target.position - Projectile.Center;
				float dis = ToTarget.Length();
				if (dis < MinDis * 0.1f && ToTarget != Vector2.Zero)
				{
					float mess = 40;
					mess = (float)Math.Sqrt(mess);
					Vector2 Addvel = Vector2.Normalize(ToTarget) / mess / (dis + 10) * 400f * Projectile.ai[0];
					target.velocity -= Addvel;
					float kSpeed = 1f;
					if (dis < 10)
					{
						kSpeed = (dis + 10) / 20f;
					}

					if (target.velocity.Length() > MaxSpeed * kSpeed)
					{
						target.velocity *= MaxSpeed * kSpeed / target.velocity.Length();
					}
				}
			}
		}
	}

	public static Vector2 Projection(Vector3 vec, Vector2 center)
	{
		float k1 = -1200 / (vec.Z - 1200);
		var v = new Vector2(vec.X, vec.Y);
		return v + (k1 - 1) * (v - center);
	}

	public static void DrawRing(Projectile Projectile, bool front = false)// 分前后两段(由front参数决定)绘制环
	{
		var c = new Color(0.2f, 0.7f, 1f); // 环的颜色

		float time = (float)Main.timeForVisualEffects * 0.02f;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);

		List<Vertex2D> vertices = new();
		(int, int) r = front ? (50, 100) : (0, 50);
		for (int i = r.Item1; i <= r.Item2; i++)
		{
			var v3 = Vector3.Transform(Vector3.UnitX * Projectile.scale * 0.8f, Matrix.CreateRotationY(i * MathHelper.TwoPi / 100));

			Vector2 v2 = Projection(new Vector3(Projectile.Center.X + v3.X - Main.screenPosition.X, Projectile.Center.Y + v3.Y - Main.screenPosition.Y, v3.Z), new Vector2(Main.screenWidth, Main.screenHeight) / 2);
			vertices.Add(new Vertex2D(v2, c * 0.8f, new Vector3(time + i / 50f, 0, 0)));

			v3 *= 2.7f;
			v2 = Projection(new Vector3(Projectile.Center.X + v3.X - Main.screenPosition.X, Projectile.Center.Y + v3.Y - Main.screenPosition.Y, v3.Z), new Vector2(Main.screenWidth, Main.screenHeight) / 2);
			vertices.Add(new Vertex2D(v2, c, new Vector3(time + i / 50f, 1, 0)));
		}
		Main.graphics.GraphicsDevice.Textures[0] = ModAsset.tex3.Value;
		// Effect ef = SpellAndSkullContent.QuickEffect("SpellAndSkull/Projectiles/BlackHole/Colorize");
		// ef.CurrentTechnique.Passes[0].Apply();
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);

		if (Main.gfxQuality == 1)
		{
			c = new Color(0.2f, 0.7f, 1f); // 环的颜色
			time = (float)Main.timeForVisualEffects * 0.03f;
			vertices = new();
			r = front ? (50, 100) : (0, 50);
			for (int i = r.Item1; i <= r.Item2; i++)
			{
				var v3 = Vector3.Transform(Vector3.UnitX * Projectile.scale * 0.8f, Matrix.CreateRotationY(i * MathHelper.TwoPi / 100));
				v3.Y -= 0.009f * Projectile.scale;
				Vector2 v2 = Projection(new Vector3(Projectile.Center.X + v3.X - Main.screenPosition.X, Projectile.Center.Y + v3.Y - Main.screenPosition.Y, v3.Z), new Vector2(Main.screenWidth, Main.screenHeight) / 2);
				vertices.Add(new Vertex2D(v2, c * 0.8f, new Vector3(time + i / 50f, 0, 0)));

				v3 *= MathF.Sin(Projectile.timeLeft / 10f) * 0.2f + 1.9f;
				v2 = Projection(new Vector3(Projectile.Center.X + v3.X - Main.screenPosition.X, Projectile.Center.Y + v3.Y - Main.screenPosition.Y, v3.Z), new Vector2(Main.screenWidth, Main.screenHeight) / 2);
				vertices.Add(new Vertex2D(v2, c, new Vector3(time + i / 50f, 1, 0)));
			}
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_1.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);

			time = (float)Main.timeForVisualEffects * 0.02f;
			vertices = new();
			r = front ? (50, 100) : (0, 50);
			for (int i = r.Item1; i <= r.Item2; i++)
			{
				var v3 = Vector3.Transform(Vector3.UnitX * Projectile.scale * 0.8f, Matrix.CreateRotationY(i * MathHelper.TwoPi / 100));
				v3.Y += 0.009f * Projectile.scale;
				Vector2 v2 = Projection(new Vector3(Projectile.Center.X + v3.X - Main.screenPosition.X, Projectile.Center.Y + v3.Y - Main.screenPosition.Y, v3.Z), new Vector2(Main.screenWidth, Main.screenHeight) / 2);
				vertices.Add(new Vertex2D(v2, c * 0.8f, new Vector3(time, 0, 0)));

				v3 *= MathF.Sin(Projectile.timeLeft / 10f + 3.14f) * 0.3f + 2.4f;
				v2 = Projection(new Vector3(Projectile.Center.X + v3.X - Main.screenPosition.X, Projectile.Center.Y + v3.Y - Main.screenPosition.Y, v3.Z), new Vector2(Main.screenWidth, Main.screenHeight) / 2);
				vertices.Add(new Vertex2D(v2, c, new Vector3(time, 1, 0)));
			}
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_2_thick.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);
		}

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D tex = TextureAssets.MagicPixel.Value;
		Main.spriteBatch.Draw(tex, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), new Color(0, 0, 0, (float)(100f / ((Main.LocalPlayer.Center - Projectile.Center).Length() + 100f)) * Projectile.scale / 60f));
		if (!Main.drawToScreen)
		{
			DrawRing(Projectile);
		}
		else// 低特效
		{
			DrawRing(Projectile);
			tex = ModContent.Request<Texture2D>(Texture).Value;
			Main.spriteBatch.Draw(tex, proj.Center - Main.screenPosition, null, Color.White, 0, tex.Size() / 2, proj.scale / 255f, 0, 0);
			DrawRing(proj, true);
		}
		return false;
	}
}

public class TemporarySys : ModSystem// 暂时用一个ModSystem上滤镜
{
	public override void Load()
	{
		Terraria.Graphics.Effects.On_FilterManager.EndCapture += FilterManager_EndCapture;
	}

	private void FilterManager_EndCapture(Terraria.Graphics.Effects.On_FilterManager.orig_EndCapture orig, Terraria.Graphics.Effects.FilterManager self, RenderTarget2D finalTexture, RenderTarget2D screenTarget1, RenderTarget2D screenTarget2, Color clearColor)
	{
		if (BlackHole.ProjActive())
		{
			Projectile proj = BlackHole.proj;
			var sb = Main.spriteBatch;
			var gd = Main.instance.GraphicsDevice;

			gd.SetRenderTarget(Main.screenTargetSwap);
			gd.Clear(Color.Black);
			sb.Begin();
			sb.Draw(Main.screenTarget, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
			sb.End();

			gd.SetRenderTarget(Main.screenTarget);
			gd.Clear(Color.Black);
			sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			Effect eff = ModAsset.BlackHole_shader.Value;
			var scRes = new Vector2(Main.screenWidth, Main.screenHeight);
			var pos = Vector2.Transform(proj.Center - Main.screenPosition, Main.Transform);
			eff.Parameters["uPosition"].SetValue(pos / scRes);
			eff.Parameters["uRatio"].SetValue(scRes.X / scRes.Y);
			eff.Parameters["uRadius"].SetValue(0.001f * proj.scale * Main.Transform.M11 / (Main.screenWidth / 1920f)); // 乘了一个总缩放系数
			eff.Parameters["uIntensity"].SetValue(3f); // 扭曲程度，可以调节这个值来实现不同效果
			eff.CurrentTechnique.Passes[0].Apply();
			sb.Draw(Main.screenTargetSwap, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);

			sb.End();
			sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);
			// 绘制黑洞以及前半环
			Texture2D tex = ModAsset.BlackHole.Value;
			sb.Draw(tex, proj.Center - Main.screenPosition, null, Color.White, 0, tex.Size() / 2, proj.scale / 255f, 0, 0);
			BlackHole.DrawRing(proj, true);
			sb.End();
		}
		orig(self, finalTexture, screenTarget1, screenTarget2, clearColor);
	}
}

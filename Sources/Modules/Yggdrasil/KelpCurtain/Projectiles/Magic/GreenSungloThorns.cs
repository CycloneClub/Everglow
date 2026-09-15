using Everglow.Commons.VFX.CommonVFXDusts;
using Everglow.Yggdrasil.Common.Elevator.Tiles;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using MathNet.Numerics.Distributions;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Terraria.DataStructures;
using Terraria.GameContent.RGB;
using Terraria.Graphics.CameraModifiers;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Magic;

internal class GreenSungloThorns : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 48;
		Projectile.height = 144;
		Projectile.friendly = true;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 10000;
		Projectile.hostile = false;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.hide = true;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 30;
	}

	internal Vector2[] Position = new Vector2[900];
	internal Vector2[] StartPosition = new Vector2[900];
	internal Vector2[,] OldPosition = new Vector2[900/*编号*/, 60/*位置*/];
	internal Vector2[] Velocity = new Vector2[900];
	internal float[] AI0 = new float[900];
	internal float[] AI1 = new float[900];
	internal int[] TimeLeft = new int[900];
	internal bool[] Active = new bool[900];
	internal bool[] Smaller = new bool[900];

	public override void OnSpawn(IEntitySource source)
	{
		for (int i = 0; i < 6; i++)
		{
			Vector2 v = new Vector2(0, -1.4f).RotatedBy((i - 2.5) / 1.8d);
			ActivateVine(i, Projectile.Center, v, 300, Main.rand.Next(100), Main.rand.NextFloat(0, 2f));
		}
	}

	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
	{
		behindNPCsAndTiles.Add(index);
	}

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];

		Projectile.spriteDirection = player.direction;
		Projectile.velocity *= 0;

		UpdateMoving();
		if (t < 550)
		{
			t += 3.75f;
		}
		if (Projectile.timeLeft <= 2)
		{
			for (int i = 0; i < 900; i++)
			{
				if (TimeLeft[i] > 0)
				{
					Projectile.timeLeft = 2;
					break;
				}
			}
		}
	}

	internal void ActivateVine(int i, Vector2 position, Vector2 velocity, int timeleft = 300, float ai0 = 0, float ai1 = 0, bool smaller = false)
	{
		int Delta = 0;
		while (TimeLeft[i + Delta] > 0 && i + Delta < 840)
		{
			Delta += 60;
		}
		i += Delta;

		StartPosition[i] = Projectile.Center;
		Position[i] = position;
		Velocity[i] = velocity;
		AI0[i] = ai0;
		AI1[i] = ai1;
		TimeLeft[i] = timeleft;
		Smaller[i] = smaller;
		Active[i] = true;
	}

	internal void UpdateMoving()
	{
		for (int i = 0; i < 900; i++)
		{
			if (!Active[i])
			{
				continue;
			}
			TimeLeft[i] -= 1;

			OldPosition[i, 0] = Position[i];
			for (int j = 59; j > 0; j--)
			{
				OldPosition[i, j] = OldPosition[i, j - 1];
			}
			Position[i] += Velocity[i];

			float colorLight = Math.Min(TimeLeft[i] / 100f, 1f);
			if (TimeLeft[i] < 75)
			{
				if (AI0[i] > 50) // 0~100
				{
					Velocity[i] = Velocity[i].RotatedBy(Math.PI / -20f);
					Velocity[i] *= 0.975f;
					Lighting.AddLight(Position[i], colorLight * 0.0f, colorLight * 0.3f, colorLight * 0.0f);
				}
				else
				{
					Velocity[i] = Velocity[i].RotatedBy(Math.PI / 20f);
					Velocity[i] *= 0.975f;
					Lighting.AddLight(Position[i], colorLight * 0.0f, colorLight * 0.3f, colorLight * 0.0f);
				}
			}
			else
			{
				if ((Position[i] - StartPosition[i]).Length() >= 60)
				{
					TimeLeft[i] -= 5;
				}
				AI1[i] += 1 / 30f; // 0.0~2.0
				Velocity[i] = Velocity[i].RotatedBy(Math.PI / 60d * (float)Math.Sin(AI1[i] * Math.PI));
			}
			if (TimeLeft[i] <= 0)
			{
				KillVine(i);
			}
		}
	}

	internal void KillVine(int i)
	{
		StartPosition[i] = Vector2.Zero;
		Position[i] = Vector2.Zero;
		TimeLeft[i] = 0;
		for (int j = 0; j < 60; j++)
		{
			OldPosition[i, j] = Vector2.Zero;
		}
		Velocity[i] = Vector2.Zero;
		AI0[i] = 0;
		AI1[i] = 0;
	}

	public override bool PreDraw(ref Color lightColor)
	{
		DrawVine();
		DrawThorn();
		return false;
	}

	public void DrawVine()
	{
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		for (int i = 0; i < 900; i++)
		{
			if (Position[i] == Vector2.Zero || !Active[i])
			{
				continue;
			}

			var bars = new List<Vertex2D>();
			float colorLight = Math.Min(TimeLeft[i] / 100f, 1f);
			float width = 6;
			if (TimeLeft[i] < 60)
			{
				width = TimeLeft[i] / 10f;
			}

			if (Smaller[i])
			{
				width = 5;
				if (TimeLeft[i] < 60)
				{
					width = TimeLeft[i] / 12f;
				}
			}

			int TrueL = 0;
			for (int j = 1; j < 60; ++j)
			{
				if (OldPosition[i, j] == Vector2.Zero)
				{
					break;
				}
				TrueL++;
			}

			for (int j = 2; j < 60; ++j)
			{
				if (OldPosition[i, j] == Vector2.Zero)
				{
					break;
				}

				var normalDir = OldPosition[i, j - 1] - OldPosition[i, j];
				normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));
				var factor = j / 60f;
				var w = MathHelper.Lerp(1f, 0.05f, factor);
				Lighting.AddLight(OldPosition[i, j], colorLight * 0f * (1 - factor), colorLight * 0.3f * (1 - factor), 0);
				Vector2 DrawPos = Projectile.Center + OldPosition[i, j] - StartPosition[i] + new Vector2(4, 48) - Main.screenPosition;
				var color = new Color(0.01f, 1f, 0.5f, 0f);
				if (Smaller[i])
				{
					color = new Color(0f, 0.4f, 0.5f, 0);
				}

				bars.Add(new Vertex2D(DrawPos + normalDir * width, color, new Vector3(factor + 0.008f, 1, w)));
				bars.Add(new Vertex2D(DrawPos - normalDir * width, color, new Vector3(factor + 0.008f, 0, w)));
			}
			var Vx = new List<Vertex2D>();
			if (bars.Count > 2)
			{
				Vx.Add(bars[0]);
				var vertex = new Vertex2D((bars[0].position + bars[1].position) * 0.5f + (bars[0].position - bars[1].position).RotatedBy(-Math.PI / 2) * 1f, new Color(254, 254, 254, 0), new Vector3(1f, 0.5f, 1));
				Vx.Add(bars[1]);
				Vx.Add(vertex);
				for (int j = 0; j < bars.Count - 2; j += 2)
				{
					Vx.Add(bars[j]);
					Vx.Add(bars[j + 2]);
					Vx.Add(bars[j + 1]);

					Vx.Add(bars[j + 1]);
					Vx.Add(bars[j + 2]);
					Vx.Add(bars[j + 3]);
				}
			}
			if (Vx.Count > 2)
			{
				Texture2D t = ModAsset.VineLine.Value;
				Main.graphics.GraphicsDevice.Textures[0] = t;
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, Vx.ToArray(), 0, Vx.Count / 3);
			}
		}
	}

	private float t = 1f;
	private float a = Main.rand.NextFloat(2.5f, 3.5f);
	private float b = Main.rand.NextFloat(2.5f, 3.5f);

	public void DrawThorn()
	{
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		float Width = 15f;
		var thorn1 = new List<Vertex2D>();
		var thorn2 = new List<Vertex2D>();
		thorn1.Clear();
		thorn2.Clear();
		Main.NewText(t);

		for (float i = 0; i <= t; i++)
		{
			Vector2 pos = new Vector2(MathF.Cos(i * 0.01f * 2 - 3) * (MathF.Sqrt(i * 0.01f) - a) * 0.25f, -i * 0.01f) * 25;
			Vector2 pos2 = new Vector2(MathF.Cos((i + 1) * 0.01f * 2 - 3) * (MathF.Sqrt((i + 1) * 0.01f) - a) * 0.25f, -i * 0.01f - 1) * 25;
			Vector2 normal = MathUtils.NormalizeSafe(pos - pos2).RotatedBy(MathF.PI * 0.5f);
			float y = (float)(-i / 320f);
			float process = MathF.Min(1, (t - i) / t * 5f);
			Width = 15f * process;
			thorn1.Add(new Vertex2D(Projectile.Center + pos + normal * Width - Main.screenPosition + Vector2.UnitY * 48, Color.Green, new Vector3(0, y % 1 + 1, Width)));
			thorn1.Add(new Vertex2D(Projectile.Center + pos - normal * Width - Main.screenPosition + Vector2.UnitY * 48, Color.Green, new Vector3(1, y % 1 + 1, Width)));
		}

		Main.graphics.GraphicsDevice.Textures[0] = ModAsset.GreenSungloThorns.Value;

		if (thorn1.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, thorn1.ToArray(), 0, thorn1.Count - 2);
		}

		for (int i = 0; i <= t; i++)
		{
			Vector2 pos = new Vector2(-MathF.Cos(i * 0.01f * 2 - 3) * (MathF.Sqrt(i * 0.01f) - b) * 0.25f, -i * 0.01f) * 25;
			Vector2 pos2 = new Vector2(-MathF.Cos((i + 1) * 0.01f * 2 - 3) * (MathF.Sqrt((i + 1) * 0.01f) - b) * 0.25f, -i * 0.01f - 1) * 25;
			Vector2 normal = MathUtils.NormalizeSafe(pos - pos2).RotatedBy(MathF.PI * 0.5f);
			float y = (float)(-i / 320f);
			float process = MathF.Min(1, (t - i) / t * 5f);
			Width = 15f * process;
			thorn2.Add(new Vertex2D(Projectile.Center + pos + normal * Width - Main.screenPosition + Vector2.UnitY * 48, Color.White, new Vector3(0, y % 1 + 1, Width)));
			thorn2.Add(new Vertex2D(Projectile.Center + pos - normal * Width - Main.screenPosition + Vector2.UnitY * 48, Color.White, new Vector3(1, y % 1 + 1, Width)));
		}

		Main.graphics.GraphicsDevice.Textures[0] = ModAsset.GreenSungloThorns.Value;

		if (thorn2.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, thorn2.ToArray(), 0, thorn2.Count - 2);
		}

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
	}

	public override void OnKill(int timeLeft)
	{
		for (int g = 0; g < 100; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(2f, 15f)).RotatedByRandom(MathHelper.TwoPi);
			var spark = new JungleSporeDust
			{
				velocity = newVelocity,
				Active = true,
				Visible = true,
				position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) + newVelocity,
				maxTime = Main.rand.Next(137, 245),
				scale = Main.rand.NextFloat(12f, Main.rand.NextFloat(8f, 16f)),
				rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, Main.rand.NextFloat(0.5f, 1.0f)), Main.rand.NextFloat(-0.03f, 0.03f) },
			};
			Ins.VFXManager.Add(spark);
		}
	}
}

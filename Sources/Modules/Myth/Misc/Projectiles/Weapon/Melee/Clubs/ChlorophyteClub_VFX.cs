using Everglow.Commons.DataStructures;
using Everglow.Myth.Common;
using Everglow.Myth.Misc.Dusts;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.Clubs;

public class ChlorophyteClub_VFX : ModProjectile
{
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
		Projectile.tileCollide = false;
		Projectile.timeLeft = 60;
		Player player = Main.player[Projectile.owner];
		for (int i = 0; i < 12; i++)
		{
			Vector2 v = new Vector2(0, Main.rand.NextFloat(1.9f, 3.4f)).RotatedBy(i / 6d * Math.PI) * Projectile.ai[0] * 2.2f;
			v.Y *= player.gravDir;
			ActivateVine(i, player.MountedCenter, v, 300, Main.rand.Next(100), Main.rand.NextFloat(0, 2f));
		}
		for (int i = 0; i < 24 * Projectile.ai[0] * 2.2f; i++)
		{
			Vector2 v = new Vector2(0, Main.rand.NextFloat(2.9f, 2.4f)).RotatedBy(i / 12d * Math.PI) * Projectile.ai[0] * 2.2f;
			Dust.NewDust(Projectile.Center - new Vector2(4), 0, 0, ModContent.DustType<LeafVFX>(), v.X, v.Y, 0, default, Main.rand.NextFloat(1f, 2.4f) * MathF.Sqrt(Projectile.ai[0]));
		}
		SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath.WithPitchOffset(0.3f), Projectile.Center);
	}
	public override void AI()
	{
		Player player = Main.player[Projectile.owner];

		Projectile.velocity *= 0;

		UpdateMoving();
		UpdateMoving();
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
		Player player = Main.player[Projectile.owner];

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
		Player player = Main.player[Projectile.owner];
		for (int i = 0; i < 900; i++)
		{
			if (!Active[i])
				continue;
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
				if (AI0[i] > 60 && AI0[i] <= 85)//0~100
				{
					Velocity[i] = Velocity[i].RotatedBy(Math.PI / -20f);
					Velocity[i] *= 0.975f;
					Lighting.AddLight(Position[i], colorLight * 0.0f, colorLight * 0.3f, colorLight * 0.0f);
				}
				else if (AI0[i] > 80)
				{
					Velocity[i] = Velocity[i].RotatedBy(Math.PI / 20f);
					Velocity[i] *= 0.975f;
					Lighting.AddLight(Position[i], colorLight * 0.0f, colorLight * 0.3f, colorLight * 0.0f);
				}
				else
				{
					AI1[i] += 1 / 30f; //0.0~2.0
					Velocity[i] = Velocity[i].RotatedBy(Math.PI / 60d * (float)Math.Sin(AI1[i] * Math.PI));
					Velocity[i] *= 0.975f;
					Lighting.AddLight(Position[i], colorLight * 0.0f, colorLight * 0.3f, colorLight * 0.0f);
				}
			}
			else
			{
				if ((Position[i] - StartPosition[i]).Length() >= 60)
					TimeLeft[i] -= 5;
				AI1[i] += 1 / 30f; //0.0~2.0
				Velocity[i] = Velocity[i].RotatedBy(Math.PI / 60d * (float)Math.Sin(AI1[i] * Math.PI));
				Lighting.AddLight(Position[i], 0, colorLight * 0.3f, 0);
				if (Main.rand.NextBool(40) && !Smaller[i])
					ActivateVine(i, Position[i] + Projectile.Center - StartPosition[i], Velocity[i], Main.rand.Next(70, 140), Main.rand.Next(100), Main.rand.NextFloat(0, 2f), true);
			}
			if (TimeLeft[i] <= 0)
				KillVine(i);
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
		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		for (int i = 0; i < 4; i++)
		{
			float value = (60 - Projectile.timeLeft) / 30f;
			if (value < 1)
			{
				DrawCircleDark(MathF.Pow(value, 0.3f) * 140 * Projectile.ai[0] * 2.2f, 85 * (1 - value) + 62 * Projectile.ai[0] * 2.2f, (1 - value), Projectile.Center - Main.screenPosition, Main.time / 16f);
				float x = (value - 0.5f) * 2f;
				float mulColor = MathF.Pow(Math.Min(MathF.Cos(MathHelper.PiOver2 * x), 1 - Math.Abs(x)), 2) * 2;
				Color c0 = new Color(0.2f * (1 - value), 1.5f * (1 - value), 0.8f * (1 - value), 0) * mulColor;
				DrawCircle(MathF.Pow(value, 0.3f) * 140 * Projectile.ai[0] * 2.2f, 85 * (1 - value) + 62 * Projectile.ai[0] * 2.2f, c0, Projectile.Center - Main.screenPosition, Main.time / 16f);
			}
			value -= 0.2f;
			if (value is < 1 and > 0)
			{
				float x = (value - 0.5f) * 2f;
				float mulColor = MathF.Pow(Math.Min(MathF.Cos(MathHelper.PiOver2 * x), 1 - Math.Abs(x)), 2) * 5;
				Color c0 = new Color(0.2f * (1 - value), 1f * (1 - value), 0.8f * (1 - value), 0) * mulColor;
				DrawCircleDark(MathF.Pow(value, 0.3f) * 104 * Projectile.ai[0] * 2.2f, 47 * (1 - value) + 32 * Projectile.ai[0] * 2.2f, (1 - value), Projectile.Center - Main.screenPosition, -Main.time / 32f);
				DrawCircle(MathF.Pow(value, 0.3f) * 104 * Projectile.ai[0] * 2.2f, 47 * (1 - value) + 32 * Projectile.ai[0] * 2.2f, c0, Projectile.Center - Main.screenPosition, -Main.time / 32f);
			}
		}
		for (int i = 0; i < 900; i++)
		{
			if (Position[i] == Vector2.Zero || !Active[i])
				continue;

			var bars = new List<Vertex2D>();
			float colorLight = Math.Min(TimeLeft[i] / 100f, 1f);
			float width = 6;
			if (TimeLeft[i] < 60)
				width = TimeLeft[i] / 10f;
			if (Smaller[i])
			{
				width = 5;
				if (TimeLeft[i] < 60)
					width = TimeLeft[i] / 12f;
			}

			int TrueL = 0;
			for (int j = 1; j < 60; ++j)
			{
				if (OldPosition[i, j] == Vector2.Zero)
					break;

				TrueL++;
			}

			for (int j = 2; j < 60; ++j)
			{
				if (OldPosition[i, j] == Vector2.Zero)
					break;

				var normalDir = OldPosition[i, j - 1] - OldPosition[i, j];
				normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));
				var factor = j / 60f;
				var w = MathHelper.Lerp(1f, 0.05f, factor);
				Lighting.AddLight(OldPosition[i, j], colorLight * 0f * (1 - factor), colorLight * 0.3f * (1 - factor), 0);
				Vector2 DrawPos = Projectile.Center + OldPosition[i, j] - StartPosition[i] + new Vector2(4) - Main.screenPosition;
				var color = new Color(0.2f, 1f, 0.7f, 0f);
				if (Smaller[i])
					color = new Color(0.0f, 0.4f, 0.3f, 0);
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
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);

		return false;
	}
	private static void DrawCircle(float radius, float width, Color color, Vector2 center, double addRot = 0)
	{
		var circle = new List<Vertex2D>();
		for (int h = 0; h <= 150; h++)
		{
			circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(h / 150f * MathHelper.TwoPi + addRot), color, new Vector3(1, h / 50f, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(h / 150f * MathHelper.TwoPi + addRot), color, new Vector3(0, h / 50f, 0)));
		}
		if (circle.Count > 0)
		{
			Texture2D t = Commons.ModAsset.Noise_spine.Value;
			Main.graphics.GraphicsDevice.Textures[0] = t;
			Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, circle.ToArray(), 0, circle.Count - 2);
		}
	}
	private void DrawCircleDark(float radius, float width, float alpha, Vector2 center, double addRot = 0)
	{
		Color color = new Color(1f, 1f, 1f, alpha * 1.6f);
		var circle = new List<Vertex2D>();
		for (int h = 0; h <= 150; h++)
		{
			circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(h / 150f * MathHelper.TwoPi + addRot), color, new Vector3(1, h / 50f, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(h / 150f * MathHelper.TwoPi + addRot), color, new Vector3(0, h / 50f, 0)));
		}
		if (circle.Count > 0)
		{
			Texture2D t = Commons.ModAsset.Noise_spine_black.Value;
			Main.graphics.GraphicsDevice.Textures[0] = t;
			Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, circle.ToArray(), 0, circle.Count - 2);
		}
	}
}

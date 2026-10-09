using Everglow.Myth.Common;
using Everglow.Myth.OmniElementItems;
using Terraria.DataStructures;

namespace Everglow.Myth.OmniElementItems.Projectiles;

internal class LilyHarpProj : ModProjectile// , IWarpProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 70;
		Projectile.height = 78;
		Projectile.friendly = false;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 10000;
		Projectile.hostile = false;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
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

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		Projectile.Center = player.Center + new Vector2(player.direction * 4, -12 * player.gravDir);
		Projectile.spriteDirection = player.direction;
		Projectile.velocity *= 0;
		if (player.itemTime % 30 == 1 && player.HeldItem.type == ModContent.ItemType<LilyHarp>())
		{
			for (int i = 0; i < 6; i++)
			{
				Vector2 v = new Vector2(0, -1.4f).RotatedBy((i - 2.5) / 1.8d);
				v.Y *= player.gravDir;
				ActivateVine(i, player.Center, v, 300, Main.rand.Next(100), Main.rand.NextFloat(0, 2f));
			}
		}
		if (player.itemTime > 0 && player.HeldItem.type == ModContent.ItemType<LilyHarp>())
		{
			Projectile.timeLeft = player.itemTime + 60;
		}

		if (player.itemTime > 20)
		{
			player.eyeHelper.BlinkBecausePlayerGotHurt();
		}

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
		Player.CompositeArmStretchAmount PCAS = Player.CompositeArmStretchAmount.Full;
		if (player.itemTime < 50)
		{
			PCAS = Player.CompositeArmStretchAmount.ThreeQuarters;
		}

		if (player.itemTime < 45)
		{
			PCAS = Player.CompositeArmStretchAmount.Quarter;
		}

		if (player.itemTime < 40)
		{
			PCAS = Player.CompositeArmStretchAmount.None;
		}

		player.SetCompositeArmFront(true, PCAS, -player.direction + player.itemTime / 90f * -player.direction);

		if (player.itemTime is >= 25 and <= 65)
		{
			if (player.itemTime % 4 == 0)
			{
				float Rot = (player.itemTime - 45) / 10f;
				Vector2 v0 = new Vector2(0, -10).RotatedBy(Rot * player.direction);
				v0.Y *= player.gravDir;

				Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, v0, ModContent.ProjectileType<LilyHarpNote>(), player.HeldItem.damage, player.HeldItem.knockBack, player.whoAmI, Main.rand.Next(4), -1);
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

		StartPosition[i] = player.Center;
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
				if (AI0[i] > 50)// 0~100
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
				Lighting.AddLight(Position[i], 0, colorLight * 0.3f, 0);
				if (Main.rand.NextBool(40) && !Smaller[i])
				{
					ActivateVine(i, Position[i] + player.Center - StartPosition[i], Velocity[i], Main.rand.Next(70, 140), Main.rand.Next(100), Main.rand.NextFloat(0, 2f), true);
				}
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
		Player player = Main.player[Projectile.owner];

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
				Vector2 DrawPos = player.Center + OldPosition[i, j] - StartPosition[i] + new Vector2(4) - Main.screenPosition;
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
			float value = (player.itemTimeMax - player.itemTime) / (float)player.itemTimeMax * 1.4f;

			if (value < 1)
			{
				DrawCircle(value * 160, 15 * (1 - value) + 3, new Color(0, 0.15f * (1 - value), 0.03f * (1 - value), 0f), player.Center + new Vector2(player.direction * 15, 0) - Main.screenPosition);
			}

			value -= 0.2f;
			if (value is < 1 and > 0)
			{
				DrawCircle(value * 133, 8 * (1 - value) + 3, new Color(0, 0.10f * (1 - value), 0.06f * (1 - value), 0f), player.Center + new Vector2(player.direction * 15, 0) - Main.screenPosition);
			}
		}
		Texture2D tx = ModAsset.LilyHarpProj.Value;
		float AddRot = player.fullRotation;
		SpriteEffects se = SpriteEffects.None;
		if (player.direction == -1)
		{
			se = SpriteEffects.FlipHorizontally;
		}

		if (player.gravDir == -1)
		{
			se = SpriteEffects.FlipVertically;
		}

		if (player.gravDir == -1 && player.direction == -1)
		{
			AddRot = (float)Math.PI - player.fullRotation;
			se = SpriteEffects.None;
		}
		Main.spriteBatch.Draw(tx, Projectile.Center - Main.screenPosition, null, Lighting.GetColor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16), AddRot, tx.Size() / 2f, 1, se, 0);
		;
		return false;
	}

	private static void DrawCircle(float radius, float width, Color color, Vector2 center)
	{
		var circle = new List<Vertex2D>();
		for (int h = 0; h < radius / 2; h++)
		{
			circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(h / radius * Math.PI * 4), color, new Vector3(0.5f, 1, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radius + width).RotatedBy(h / radius * Math.PI * 4), color, new Vector3(0.5f, 0, 0)));
		}
		circle.Add(new Vertex2D(center + new Vector2(0, radius), color, new Vector3(0.5f, 1, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius + width), color, new Vector3(0.5f, 0, 0)));
		if (circle.Count > 0)
		{
			Texture2D t = ModAsset.Wave.Value;
			Main.graphics.GraphicsDevice.Textures[0] = t;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, circle.ToArray(), 0, circle.Count - 2);
		}
	}
	private static void DrawCircle(VFXBatch spriteBatch, float radius, float width, Color color, Vector2 center)
	{
		var circle = new List<Vertex2D>();
		for (int h = 0; h < radius / 2; h++)
		{
			circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(h / radius * Math.PI * 4), color, new Vector3(0.5f, 1, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radius + width).RotatedBy(h / radius * Math.PI * 4), color, new Vector3(0.5f, 0, 0)));
		}
		circle.Add(new Vertex2D(center + new Vector2(0, radius), color, new Vector3(0.5f, 1, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius + width), color, new Vector3(0.5f, 0, 0)));
		if (circle.Count > 0)
		{
			Texture2D t = ModAsset.Wave.Value;
			spriteBatch.Draw(t, circle, PrimitiveType.TriangleStrip);
		}
	}
	public void DrawWarp(VFXBatch spriteBatch)
	{

		Player player = Main.player[Projectile.owner];
		float value = (player.itemTimeMax - player.itemTime) / (float)player.itemTimeMax * 1.4f;
		value -= 0.02f;
		if (value < 1)
		{
			float c0 = 0.3f * (float)Math.Sqrt(1 - value);
			DrawCircle(spriteBatch, value * 160, 30 * (1 - value) + 6, new Color(c0, c0, c0, 0f), player.Center + new Vector2(player.direction * 15, 0) - Main.screenPosition);
		}
		value -= 0.22f;
		if (value is < 1 and > 0)
		{
			float c0 = 0.2f * (float)Math.Sqrt(1 - value);
			DrawCircle(spriteBatch, value * 133, 16 * (1 - value) + 6, new Color(c0, c0, c0, 0f), player.Center + new Vector2(player.direction * 15, 0) - Main.screenPosition);
		}
	}
}

internal class LilyHarpOwner : ModPlayer
{
	public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
	{
		base.ModifyDrawInfo(ref drawInfo);
	}
}

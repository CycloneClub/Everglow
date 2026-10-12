using Terraria.DataStructures;

namespace Everglow.Myth.LanternMoon.Projectiles.Item_Shoot;

public class Firework12Inches : FireworkProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.TypelessProjectiles;

	public override void OnSpawn(IEntitySource source)
	{
		Timer = 0;
		SpawnStyle(Main.rand.Next(13));
	}

	public override void SpawnStyle(int style)
	{
		var axis = new Vector3(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
		float rot = Main.rand.NextFloat(6.283f);
		int starsCount = 200;
		Color color = NormalColor();
		Color color1 = NormalColor();
		while (color1 == color)
		{
			color1 = NormalColor();
		}
		switch (style)
		{
			// 球状
			case 0:
				for (int theta = -starsCount; theta <= starsCount; theta += 10)
				{
					float length = MathF.Cos(theta / (float)starsCount * MathF.PI);
					for (int phi = 0; phi <= length * starsCount; phi += 5)
					{
						Vector3 velocity = new Vector3(MathF.Cos(phi / length / starsCount * MathHelper.TwoPi) * length, MathF.Sin(theta / (float)starsCount * MathF.PI), MathF.Sin(phi / length / starsCount * MathHelper.TwoPi) * length) * 24f * Main.rand.NextFloat(0.95f, 1.05f);
						velocity = RodriguesRotate(velocity, axis, rot);
						var flame = new FlameTrail(-velocity, new List<Vector3>(), velocity, color, Main.rand.NextFloat(0.85f, 1.15f) * 1.4f, true);
						flame.FlameTimer = Main.rand.Next(-20, 0);
						Stars.Add(flame);
					}
				}
				break;

			// 环状
			case 1:
				starsCount = 300;
				for (int theta = -starsCount; theta <= starsCount; theta += 10)
				{
					if (MathF.Abs(theta) < starsCount * 0.05f)
					{
						float length = MathF.Cos(theta / (float)starsCount * MathF.PI);
						for (int phi = 0; phi <= length * starsCount; phi += 5)
						{
							Vector3 velocity = new Vector3(MathF.Cos(phi / length / starsCount * MathHelper.TwoPi) * length, MathF.Sin(theta / (float)starsCount * MathF.PI), MathF.Sin(phi / length / starsCount * MathHelper.TwoPi) * length) * 27f * Main.rand.NextFloat(0.95f, 1.05f);
							velocity = RodriguesRotate(velocity, axis, rot);
							var flame = new FlameTrail(-velocity, new List<Vector3>(), velocity, color1, Main.rand.NextFloat(0.85f, 1.15f) * 1.4f, true);
							flame.FlameTimer = Main.rand.Next(-20, 0);
							Stars.Add(flame);
						}
					}
				}
				starsCount = 150;
				for (int theta = -starsCount; theta < starsCount; theta += 10)
				{
					float length = MathF.Cos(theta / (float)starsCount * MathF.PI);
					for (int phi = 0; phi <= length * starsCount; phi += 5)
					{
						Vector3 velocity = new Vector3(MathF.Cos(phi / length / starsCount * MathHelper.TwoPi) * length, MathF.Sin(theta / (float)starsCount * MathF.PI), MathF.Sin(phi / length / starsCount * MathHelper.TwoPi) * length) * 12f * Main.rand.NextFloat(0.95f, 1.05f);
						velocity = RodriguesRotate(velocity, axis, rot);
						Stars.Add(new FlameTrail(-velocity, new List<Vector3>(), velocity, color, Main.rand.NextFloat(0.85f, 1.15f) * 1.4f, true));
					}
				}
				break;

			// 杂点
			case 2:
				for (int theta = -starsCount; theta <= starsCount; theta += 10)
				{
					float length = MathF.Cos(theta / (float)starsCount * MathF.PI);
					for (int phi = 0; phi <= length * starsCount; phi += 5)
					{
						Vector3 velocity = new Vector3(MathF.Cos(phi / length / starsCount * MathHelper.TwoPi) * length, MathF.Sin(theta / (float)starsCount * MathF.PI), MathF.Sin(phi / length / starsCount * MathHelper.TwoPi) * length) * 24f * Main.rand.NextFloat(0.95f, 1.05f);
						velocity = RodriguesRotate(velocity, axis, rot);
						Stars.Add(new FlameTrail(-velocity, new List<Vector3>(), velocity, Main.rand.NextBool(2) ? color : color1, Main.rand.NextFloat(0.85f, 1.15f) * 1.4f, true));
					}
				}
				break;

			// 三段球
			case 3:
				for (int theta = -starsCount; theta <= starsCount; theta += 10)
				{
					float length = MathF.Cos(theta / (float)starsCount * MathF.PI);
					for (int phi = 0; phi <= length * starsCount; phi += 5)
					{
						Vector3 velocity = new Vector3(MathF.Cos(phi / length / starsCount * MathHelper.TwoPi) * length, MathF.Sin(theta / (float)starsCount * MathF.PI), MathF.Sin(phi / length / starsCount * MathHelper.TwoPi) * length) * 24f * Main.rand.NextFloat(0.95f, 1.05f);
						velocity = RodriguesRotate(velocity, axis, rot);
						Stars.Add(new FlameTrail(-velocity, new List<Vector3>(), velocity, Math.Abs(theta) > starsCount * 0.2f ? color : color1, Main.rand.NextFloat(0.85f, 1.15f) * 1.4f, true));
					}
				}
				break;

			// 金色椰树
			case 4:
				starsCount = 150;
				for (int theta = -starsCount; theta <= starsCount; theta += 10)
				{
					float length = MathF.Cos(theta / (float)starsCount * MathF.PI);
					for (int phi = 0; phi <= length * starsCount; phi += 5)
					{
						Vector3 velocity = new Vector3(MathF.Cos(phi / length / starsCount * MathHelper.TwoPi) * length, MathF.Sin(theta / (float)starsCount * MathF.PI), MathF.Sin(phi / length / starsCount * MathHelper.TwoPi) * length) * 24f * Main.rand.NextFloat(0.95f, 1.05f);
						velocity = RodriguesRotate(velocity, axis, rot);
						var flame = new FlameTrail(-velocity, new List<Vector3>(), velocity, new Color(0, 0, 0, 0), Main.rand.NextFloat(0.85f, 1.15f) * 1f, true);
						flame.TrailStyle = 1;
						flame.FlameTimer = Main.rand.Next(-10, 11);
						flame.ai[0] = 20;
						flame.ai[1] = 4;
						Stars.Add(flame);
					}
				}
				break;

			// 椰树带心
			case 5:
				starsCount = 90;
				for (int theta = -starsCount; theta <= starsCount; theta += 10)
				{
					float length = MathF.Cos(theta / (float)starsCount * MathF.PI);
					for (int phi = 0; phi <= length * starsCount; phi += 5)
					{
						Vector3 velocity = new Vector3(MathF.Cos(phi / length / starsCount * MathHelper.TwoPi) * length, MathF.Sin(theta / (float)starsCount * MathF.PI), MathF.Sin(phi / length / starsCount * MathHelper.TwoPi) * length) * 24f * Main.rand.NextFloat(0.95f, 1.05f);
						velocity = RodriguesRotate(velocity, axis, rot);
						var flame = new FlameTrail(-velocity, new List<Vector3>(), velocity, new Color(0, 0, 0, 0), Main.rand.NextFloat(0.85f, 1.15f) * 1f, true);
						flame.TrailStyle = 1;
						flame.FlameTimer = Main.rand.Next(-10, 11);
						flame.ai[0] = 20;
						Stars.Add(flame);
					}
				}
				starsCount = 100;
				for (int theta = -starsCount; theta <= starsCount; theta += 10)
				{
					float length = MathF.Cos(theta / (float)starsCount * MathF.PI);
					for (int phi = 0; phi <= length * starsCount; phi += 5)
					{
						Vector3 velocity = new Vector3(MathF.Cos(phi / length / starsCount * MathHelper.TwoPi) * length, MathF.Sin(theta / (float)starsCount * MathF.PI), MathF.Sin(phi / length / starsCount * MathHelper.TwoPi) * length) * 9f * Main.rand.NextFloat(0.95f, 1.05f);
						velocity = RodriguesRotate(velocity, axis, rot);
						var flame = new FlameTrail(-velocity, new List<Vector3>(), velocity, color, Main.rand.NextFloat(0.85f, 1.15f) * 1.8f, true);
						flame.FlameTimer = Main.rand.Next(-10, 11);
						flame.ai[0] = 60;
						Stars.Add(flame);
					}
				}
				break;

			// 两半球
			case 6:
				for (int theta = -starsCount + 5; theta < 5; theta += 10)
				{
					float length = MathF.Cos(theta / (float)starsCount * MathF.PI);
					for (int phi = 0; phi <= length * starsCount; phi += 5)
					{
						Vector3 velocity = new Vector3(MathF.Cos(phi / length / starsCount * MathHelper.TwoPi) * length, MathF.Sin(theta / (float)starsCount * MathF.PI), MathF.Sin(phi / length / starsCount * MathHelper.TwoPi) * length) * 24f * Main.rand.NextFloat(0.95f, 1.05f);
						velocity = RodriguesRotate(velocity, axis, rot);
						Stars.Add(new FlameTrail(-velocity, new List<Vector3>(), velocity, color1, Main.rand.NextFloat(0.85f, 1.15f) * 1.4f, true));
					}
				}
				for (int theta = 5; theta <= starsCount; theta += 10)
				{
					float length = MathF.Cos(theta / (float)starsCount * MathF.PI);
					for (int phi = 0; phi <= length * starsCount; phi += 5)
					{
						Vector3 velocity = new Vector3(MathF.Cos(phi / length / starsCount * MathHelper.TwoPi) * length, MathF.Sin(theta / (float)starsCount * MathF.PI), MathF.Sin(phi / length / starsCount * MathHelper.TwoPi) * length) * 24f * Main.rand.NextFloat(0.95f, 1.05f);
						velocity = RodriguesRotate(velocity, axis, rot);
						Stars.Add(new FlameTrail(-velocity, new List<Vector3>(), velocity, color, Main.rand.NextFloat(0.85f, 1.15f) * 1.4f, true));
					}
				}
				break;

			// 多个集束
			case 7:
				starsCount = 100;
				for (int theta = -starsCount; theta <= starsCount; theta += 10)
				{
					float length = MathF.Cos(theta / (float)starsCount * MathF.PI);
					for (int phi = 0; phi <= length * starsCount; phi += 5)
					{
						Vector3 velocity = new Vector3(MathF.Cos(phi / length / starsCount * MathHelper.TwoPi) * length, MathF.Sin(theta / (float)starsCount * MathF.PI), MathF.Sin(phi / length / starsCount * MathHelper.TwoPi) * length) * 27f * Main.rand.NextFloat(0.95f, 1.05f);
						velocity = RodriguesRotate(velocity, axis, rot);
						Stars.Add(new FlameTrail(-velocity, new List<Vector3>(), velocity, color1, Main.rand.NextFloat(0.85f, 1.15f) * 2.4f, true));
						for (int x = 0; x < 3; x++)
						{
							Vector3 newVel = Vector3.Normalize(RodriguesRotate(new Vector3(velocity.Y, -velocity.X, 0), velocity, x / 3f * MathHelper.TwoPi)) * 0.3f * Main.rand.NextFloat(0.85f, 1.15f);
							var flame = new FlameTrail(-velocity, new List<Vector3>(), velocity * 0.4f + newVel, color, Main.rand.NextFloat(0.85f, 1.15f) * 0.8f, true);
							flame.FlameTimer = Main.rand.Next(-10, 0);
							flame.ai[0] = -30;
							Stars.Add(flame);
						}
					}
				}
				break;

			// 金环+核心
			case 8:
				if (style == 8)
				{
					starsCount = 180;
					for (int theta = -starsCount; theta <= starsCount; theta += 10)
					{
						float length = MathF.Cos(theta / (float)starsCount * MathF.PI);
						for (int phi = 0; phi <= length * starsCount; phi += 5)
						{
							Vector3 velocity = new Vector3(MathF.Cos(phi / length / starsCount * MathHelper.TwoPi) * length, MathF.Sin(theta / (float)starsCount * MathF.PI), MathF.Sin(phi / length / starsCount * MathHelper.TwoPi) * length) * 12f * Main.rand.NextFloat(0.95f, 1.05f);
							velocity = RodriguesRotate(velocity, axis, rot);
							Stars.Add(new FlameTrail(-velocity, new List<Vector3>(), velocity, color, Main.rand.NextFloat(0.85f, 1.15f) * 1.4f, true));
						}
					}
					starsCount = 300;
					int theta2 = 0;
					float length2 = MathF.Cos(theta2 / (float)starsCount * MathF.PI);
					for (int phi = 0; phi <= length2 * starsCount; phi += 5)
					{
						Vector3 velocity = new Vector3(MathF.Cos(phi / length2 / starsCount * MathHelper.TwoPi) * length2, MathF.Sin(theta2 / (float)starsCount * MathF.PI), MathF.Sin(phi / length2 / starsCount * MathHelper.TwoPi) * length2) * 27f * Main.rand.NextFloat(0.95f, 1.05f);
						velocity = RodriguesRotate(velocity, axis, rot);
						var flame = new FlameTrail(-velocity, new List<Vector3>(), velocity, new Color(0, 0, 0, 0), Main.rand.NextFloat(0.85f, 1.15f) * 1f, true);
						flame.TrailStyle = 1;
						flame.FlameTimer = Main.rand.Next(-10, 11);
						flame.ai[0] = 20;
						flame.ai[1] = 20;
						Stars.Add(flame);
					}
				}
				break;

			// 金色漏斗
			case 9:
				if (style == 9)
				{
					Color c0 = NormalColor();
					starsCount = 300;
					int theta2 = 0;
					float length2 = MathF.Cos(theta2 / (float)starsCount * MathF.PI);
					for (int phi = 0; phi < length2 * starsCount; phi += 5)
					{
						Vector3 velocity = new Vector3(MathF.Cos(phi / length2 / starsCount * MathHelper.TwoPi) * length2, MathF.Sin(theta2 / (float)starsCount * MathF.PI), MathF.Sin(phi / length2 / starsCount * MathHelper.TwoPi) * length2) * 27f * Main.rand.NextFloat(0.95f, 1.05f);
						velocity = RodriguesRotate(velocity, axis, rot);
						var flame = new FlameTrail(-velocity, new List<Vector3>(), velocity, c0, Main.rand.NextFloat(0.85f, 1.15f) * 1.4f, true);
						flame.FlameTimer = Main.rand.Next(-10, 0);
						flame.ai[0] = 20;
						Stars.Add(flame);
					}
					starsCount = 200;
					for (int theta = -starsCount; theta < starsCount; theta += 10)
					{
						float length = MathF.Cos(theta / (float)starsCount * MathF.PI);
						for (int phi = 0; phi <= length * starsCount; phi += 5)
						{
							Vector3 velocity = new Vector3(MathF.Cos(phi / length / starsCount * MathHelper.TwoPi) * length, MathF.Sin(theta / (float)starsCount * MathF.PI), MathF.Sin(phi / length / starsCount * MathHelper.TwoPi) * length) * 24f * Main.rand.NextFloat(0.95f, 1.05f);
							velocity = RodriguesRotate(velocity, axis, rot);
							if (Math.Abs(theta) > starsCount * 0.24f)
							{
								var flame = new FlameTrail(-velocity, new List<Vector3>(), velocity, new Color(0, 0, 0, 0), Main.rand.NextFloat(0.85f, 1.15f) * 1f, true);
								flame.TrailStyle = 1;
								flame.FlameTimer = Main.rand.Next(-10, 11);
								flame.ai[0] = 20;
								flame.ai[1] = 8;
								Stars.Add(flame);
							}
						}
					}
				}
				break;

			// 双层
			case 10:
				starsCount = 150;
				for (int theta = -starsCount; theta < starsCount; theta += 10)
				{
					float length = MathF.Cos(theta / (float)starsCount * MathF.PI);
					for (int phi = 0; phi <= length * starsCount; phi += 5)
					{
						Vector3 velocity = new Vector3(MathF.Cos(phi / length / starsCount * MathHelper.TwoPi) * length, MathF.Sin(theta / (float)starsCount * MathF.PI), MathF.Sin(phi / length / starsCount * MathHelper.TwoPi) * length) * 12f * Main.rand.NextFloat(0.95f, 1.05f);
						velocity = RodriguesRotate(velocity, axis, rot);
						Stars.Add(new FlameTrail(-velocity, new List<Vector3>(), velocity, color, Main.rand.NextFloat(0.85f, 1.15f) * 1.4f, true));
					}
				}
				starsCount = 150;
				for (int theta = -starsCount; theta < starsCount; theta += 10)
				{
					float length = MathF.Cos(theta / (float)starsCount * MathF.PI);
					for (int phi = 0; phi <= length * starsCount; phi += 5)
					{
						Vector3 velocity = new Vector3(MathF.Cos(phi / length / starsCount * MathHelper.TwoPi) * length, MathF.Sin(theta / (float)starsCount * MathF.PI), MathF.Sin(phi / length / starsCount * MathHelper.TwoPi) * length) * 24f * Main.rand.NextFloat(0.95f, 1.05f);
						velocity = RodriguesRotate(velocity, axis, rot);
						Stars.Add(new FlameTrail(-velocity, new List<Vector3>(), velocity, color1, Main.rand.NextFloat(0.85f, 1.15f) * 1.4f, true));
					}
				}
				break;

			// 球状随机拖尾
			case 11:
				for (int theta = -starsCount; theta <= starsCount; theta += 10)
				{
					float length = MathF.Cos(theta / (float)starsCount * MathF.PI);
					for (int phi = 0; phi <= length * starsCount; phi += 5)
					{
						Vector3 velocity = new Vector3(MathF.Cos(phi / length / starsCount * MathHelper.TwoPi) * length, MathF.Sin(theta / (float)starsCount * MathF.PI), MathF.Sin(phi / length / starsCount * MathHelper.TwoPi) * length) * 24f * Main.rand.NextFloat(0.95f, 1.05f);
						velocity = RodriguesRotate(velocity, axis, rot);
						var flame = new FlameTrail(-velocity, new List<Vector3>(), velocity, color, Main.rand.NextFloat(0.85f, 1.15f) * 1.4f, true);
						flame.FlameTimer = Main.rand.Next(-20, 0);
						if (Main.rand.NextBool(10))
						{
							flame = new FlameTrail(-velocity, new List<Vector3>(), velocity, new Color(0, 0, 0, 0), Main.rand.NextFloat(0.85f, 1.15f) * 1f, true);
							flame.TrailStyle = 1;
							flame.FlameTimer = Main.rand.Next(-10, 11);
							flame.ai[0] = 20;
							flame.ai[1] = 0;
						}
						Stars.Add(flame);
					}
				}
				break;

			// 闪烁实验
			case 12:
				for (int theta = -starsCount; theta <= starsCount; theta += 10)
				{
					float length = MathF.Cos(theta / (float)starsCount * MathF.PI);
					for (int phi = 0; phi <= length * starsCount; phi += 5)
					{
						Vector3 velocity = new Vector3(MathF.Cos(phi / length / starsCount * MathHelper.TwoPi) * length, MathF.Sin(theta / (float)starsCount * MathF.PI), MathF.Sin(phi / length / starsCount * MathHelper.TwoPi) * length) * 24f * Main.rand.NextFloat(0.95f, 1.05f);
						velocity = RodriguesRotate(velocity, axis, rot);
						var flame = new FlameTrail(-velocity, new List<Vector3>(), velocity, color, Main.rand.NextFloat(0.85f, 1.15f) * 1.4f, true);
						flame.FlameTimer = Main.rand.Next(-60, -40);
						flame.ai[3] = Main.rand.Next(20) - 20;
						Stars.Add(flame);
					}
				}
				break;
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D flame = Commons.ModAsset.LightPoint.Value;
		var trailBars0 = new List<Vertex2D>();
		var trailBars1 = new List<Vertex2D>();
		foreach (FlameTrail flameTrail in Stars)
		{
			if (flameTrail.Active)
			{
				Vector3 drawPos3D = flameTrail.Postion;
				drawPos3D.X += (Projectile.Center - Main.screenPosition).X;
				drawPos3D.Y += (Projectile.Center - Main.screenPosition).Y;
				float scale;
				Vector2 v2Pos = Projection2D(drawPos3D, new Vector2(Main.screenWidth, Main.screenHeight) / 2, 1000, out scale);

				Vector3[] trailPos = flameTrail.OldPos.Reverse<Vector3>().ToArray();
				int len = trailPos.Length;
				if (len <= 2)
				{
					continue;
				}

				if (Ins.VisualQuality.High)
				{
					for (int i = 1; i < len; i++)
					{
						if (i % 24 != 1 && i < len - 24 && i > 8)
						{
							continue;
						}
						float scale0;
						Vector3 oldPos3D0 = trailPos[i];
						oldPos3D0.X += (Projectile.Center - Main.screenPosition).X;
						oldPos3D0.Y += (Projectile.Center - Main.screenPosition).Y;
						Vector2 old0 = Projection2D(oldPos3D0, new Vector2(Main.screenWidth, Main.screenHeight) / 2, 1000, out scale0);

						float scale1;
						Vector3 oldPos3D1 = trailPos[i - 1];
						oldPos3D1.X += (Projectile.Center - Main.screenPosition).X;
						oldPos3D1.Y += (Projectile.Center - Main.screenPosition).Y;
						Vector2 old1 = Projection2D(oldPos3D1, new Vector2(Main.screenWidth, Main.screenHeight) / 2, 1000, out scale1);

						Vector2 normal = old0 - old1;
						normal = Vector2.Normalize(normal).RotatedBy(MathHelper.PiOver2);

						Color light = Lighting.GetColor((int)((old0 + Main.screenPosition).X / 16f), (int)((old0 + Main.screenPosition).Y / 16f));
						light.A = 0;
						if (i == 1)
						{
							Vector2 addVel = normal.RotatedBy(-MathHelper.PiOver2) * scale0 * 5 * flameTrail.Scale;
							trailBars0.Add(old0 - addVel + normal * scale0 * 5 * flameTrail.Scale, Color.Transparent, new Vector3(0.5f, 0, 0));
							trailBars0.Add(old0 - addVel - normal * scale0 * 5 * flameTrail.Scale, Color.Transparent, new Vector3(0.5f, 1, 0));
						}
						trailBars0.Add(old0 + normal * scale0 * 5 * flameTrail.Scale * MathF.Log(i * 0.1f + 1), light * MathF.Pow(0.965f, i) * 0.04f, new Vector3(0.5f, 0, 0));
						trailBars0.Add(old0 - normal * scale0 * 5 * flameTrail.Scale * MathF.Log(i * 0.1f + 1), light * MathF.Pow(0.965f, i) * 0.04f, new Vector3(0.5f, 1, 0));
						if (i == len - 1)
						{
							Vector2 addVel = normal.RotatedBy(-MathHelper.PiOver2) * scale0 * 2 * flameTrail.Scale;
							trailBars0.Add(old0 + addVel + normal * scale0 * 5 * flameTrail.Scale, Color.Transparent, new Vector3(0.5f, 0, 0));
							trailBars0.Add(old0 + addVel - normal * scale0 * 5 * flameTrail.Scale, Color.Transparent, new Vector3(0.5f, 1, 0));
						}

						if (flameTrail.TrailStyle == 1)
						{
							float zValue = (TrailLength - i) / (float)TrailLength;
							if (flameTrail.FlameTimer > 60)
							{
								zValue -= (flameTrail.FlameTimer - 60) / 60f;
								if (zValue < 0)
								{
									zValue = 0;
								}
							}
							float width = 1f;
							if (len < 75)
							{
								width *= len / 75f;
							}
							if (Timer - i < flameTrail.ai[1])
							{
								zValue = 0;
							}
							if (Timer - i < flameTrail.ai[1] + 10)
							{
								zValue *= (Timer - i - flameTrail.ai[1]) / 10f;
							}
							if (i == 1)
							{
								Vector2 addVel = normal.RotatedBy(-MathHelper.PiOver2) * scale0 * 5 * flameTrail.Scale;
								trailBars1.Add(old0 - addVel + normal * scale0 * 15 * flameTrail.Scale * MathF.Log(i * 0.1f + 1) * width, Color.Transparent, new Vector3(i / 40f, 0, 1));
								trailBars1.Add(old0 - addVel - normal * scale0 * 15 * flameTrail.Scale * MathF.Log(i * 0.1f + 1) * width, Color.Transparent, new Vector3(i / 40f, 1, 1));
							}
							trailBars1.Add(old0 + normal * scale0 * 15 * flameTrail.Scale * MathF.Log(i * 0.1f + 1) * width, new Color(1f, 0.7f, 0.4f, 0), new Vector3(i / 40f, 0, zValue));
							trailBars1.Add(old0 - normal * scale0 * 15 * flameTrail.Scale * MathF.Log(i * 0.1f + 1) * width, new Color(1f, 0.7f, 0.4f, 0), new Vector3(i / 40f, 1, zValue));
							if (i == len - 1)
							{
								Vector2 addVel = normal.RotatedBy(-MathHelper.PiOver2) * scale0 * 2 * flameTrail.Scale;
								trailBars1.Add(old0 + addVel + normal * scale0 * 15 * flameTrail.Scale * MathF.Log(i * 0.1f + 1) * width, Color.Transparent, new Vector3(i / 40f, 0, 0));
								trailBars1.Add(old0 + addVel - normal * scale0 * 15 * flameTrail.Scale * MathF.Log(i * 0.1f + 1) * width, Color.Transparent, new Vector3(i / 40f, 1, 0));
							}
						}
					}
				}
				if (flameTrail.Color != new Color(0, 0, 0, 0))
				{
					float sizeValue = flame.Width * flameTrail.Scale * scale;
					if (flameTrail.ai[3] != 0 && flameTrail.FlameTimer > flameTrail.ai[3])
					{
						sizeValue *= MathF.Sin(flameTrail.FlameTimer * 0.5f + flameTrail.Postion.Z * 0.1f) + 1;
						flameTrail.ai[2] = sizeValue * 2 - 2;
					}
					float scaleValue2 = sizeValue * flameTrail.Scale * 0.4f;
					if (scaleValue2 > 10f)
					{
						scaleValue2 = 10f;
					}
					if (flameTrail.ai[2] > 0)
					{
						for (int j = 0; j < 3; j++)
						{
							float rot = j * MathHelper.TwoPi / 3f;
							float length = (flameTrail.ai[2] + 1) / 6f;
							if (length > 10)
							{
								length = 10;
							}
							float brightness = flameTrail.ai[2] / 4f;
							if (brightness > 1)
							{
								brightness = 1;
							}
							trailBars0.Add(v2Pos + new Vector2(-length, -0.5f).RotatedBy(rot) * scaleValue2, Color.Transparent, new Vector3(0, 0, 0));
							trailBars0.Add(v2Pos + new Vector2(length, -0.5f).RotatedBy(rot) * scaleValue2, Color.Transparent, new Vector3(1, 0, 0));

							trailBars0.Add(v2Pos + new Vector2(-length, -0.5f).RotatedBy(rot) * scaleValue2, flameTrail.Color * brightness * 0.14f, new Vector3(0, 0f, 0));
							trailBars0.Add(v2Pos + new Vector2(length, -0.5f).RotatedBy(rot) * scaleValue2, flameTrail.Color * brightness * 0.14f, new Vector3(1, 0f, 0));

							trailBars0.Add(v2Pos + new Vector2(-length, 0.5f).RotatedBy(rot) * scaleValue2, flameTrail.Color * brightness * 0.14f, new Vector3(0, 1f, 0));
							trailBars0.Add(v2Pos + new Vector2(length, 0.5f).RotatedBy(rot) * scaleValue2, flameTrail.Color * brightness * 0.14f, new Vector3(1, 1f, 0));

							trailBars0.Add(v2Pos + new Vector2(-length, 0.5f).RotatedBy(rot) * scaleValue2, Color.Transparent, new Vector3(0, 1, 0));
							trailBars0.Add(v2Pos + new Vector2(length, 0.5f).RotatedBy(rot) * scaleValue2, Color.Transparent, new Vector3(1, 1, 0));
						}
						for (int j = 0; j < 3; j++)
						{
							float rot = j * MathHelper.TwoPi / 3f + MathHelper.PiOver2;
							float length = (flameTrail.ai[2] + 1) / 20f;
							if (length > 10)
							{
								length = 10;
							}
							float brightness = flameTrail.ai[2] / 8f;
							if (brightness > 1)
							{
								brightness = 1;
							}
							trailBars0.Add(v2Pos + new Vector2(-length, -0.5f).RotatedBy(rot) * scaleValue2, Color.Transparent, new Vector3(0, 0, 0));
							trailBars0.Add(v2Pos + new Vector2(length, -0.5f).RotatedBy(rot) * scaleValue2, Color.Transparent, new Vector3(1, 0, 0));

							trailBars0.Add(v2Pos + new Vector2(-length, -0.5f).RotatedBy(rot) * scaleValue2, flameTrail.Color * brightness * 0.24f, new Vector3(0, 0f, 0));
							trailBars0.Add(v2Pos + new Vector2(length, -0.5f).RotatedBy(rot) * scaleValue2, flameTrail.Color * brightness * 0.24f, new Vector3(1, 0f, 0));

							trailBars0.Add(v2Pos + new Vector2(-length, 0.5f).RotatedBy(rot) * scaleValue2, flameTrail.Color * brightness * 0.24f, new Vector3(0, 1f, 0));
							trailBars0.Add(v2Pos + new Vector2(length, 0.5f).RotatedBy(rot) * scaleValue2, flameTrail.Color * brightness * 0.24f, new Vector3(1, 1f, 0));

							trailBars0.Add(v2Pos + new Vector2(-length, 0.5f).RotatedBy(rot) * scaleValue2, Color.Transparent, new Vector3(0, 1, 0));
							trailBars0.Add(v2Pos + new Vector2(length, 0.5f).RotatedBy(rot) * scaleValue2, Color.Transparent, new Vector3(1, 1, 0));
						}
					}
					trailBars0.Add(v2Pos + new Vector2(-0.5f, -0.5f) * sizeValue, Color.Transparent, new Vector3(0, 0, 0));
					trailBars0.Add(v2Pos + new Vector2(0.5f, -0.5f) * sizeValue, Color.Transparent, new Vector3(1, 0, 0));

					trailBars0.Add(v2Pos + new Vector2(-0.5f, -0.5f) * sizeValue, flameTrail.Color * 0.4f, new Vector3(0, 0, 0));
					trailBars0.Add(v2Pos + new Vector2(0.5f, -0.5f) * sizeValue, flameTrail.Color * 0.4f, new Vector3(1, 0, 0));

					trailBars0.Add(v2Pos + new Vector2(-0.5f, 0.5f) * sizeValue, flameTrail.Color * 0.4f, new Vector3(0, 1, 0));
					trailBars0.Add(v2Pos + new Vector2(0.5f, 0.5f) * sizeValue, flameTrail.Color * 0.4f, new Vector3(1, 1, 0));

					trailBars0.Add(v2Pos + new Vector2(-0.5f, 0.5f) * sizeValue, Color.Transparent, new Vector3(0, 1, 0));
					trailBars0.Add(v2Pos + new Vector2(0.5f, 0.5f) * sizeValue, Color.Transparent, new Vector3(1, 1, 0));

					trailBars0.Add(v2Pos + new Vector2(-0.5f, -0.5f) * scaleValue2, Color.Transparent, new Vector3(0, 0, 0));
					trailBars0.Add(v2Pos + new Vector2(0.5f, -0.5f) * scaleValue2, Color.Transparent, new Vector3(1, 0, 0));

					trailBars0.Add(v2Pos + new Vector2(-0.5f, -0.5f) * scaleValue2, flameTrail.Color * 6f, new Vector3(0, 0, 0));
					trailBars0.Add(v2Pos + new Vector2(0.5f, -0.5f) * scaleValue2, flameTrail.Color * 6f, new Vector3(1, 0, 0));

					trailBars0.Add(v2Pos + new Vector2(-0.5f, 0.5f) * scaleValue2, flameTrail.Color * 6f, new Vector3(0, 1, 0));
					trailBars0.Add(v2Pos + new Vector2(0.5f, 0.5f) * scaleValue2, flameTrail.Color * 6f, new Vector3(1, 1, 0));

					trailBars0.Add(v2Pos + new Vector2(-0.5f, 0.5f) * scaleValue2, Color.Transparent, new Vector3(0, 1, 0));
					trailBars0.Add(v2Pos + new Vector2(0.5f, 0.5f) * scaleValue2, Color.Transparent, new Vector3(1, 1, 0));
				}
			}
		}
		Main.graphics.GraphicsDevice.Textures[0] = flame;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		if (trailBars0.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, trailBars0.ToArray(), 0, trailBars0.Count - 2);
		}

		if (trailBars1.Count > 3)
		{
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_1.Value;
			Effect effect = ModAsset.FireworkTrailStyle1.Value;
			effect.Parameters["uPerlin"].SetValue(ModAsset.CorruptDustFadePowderII.Value);
			var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
			var model = Matrix.CreateTranslation(new Vector3(0, 0, 0)) * Main.GameViewMatrix.TransformationMatrix;
			effect.Parameters["uTransform"].SetValue(model * projection);
			effect.CurrentTechnique.Passes["Test"].Apply();
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, trailBars1.ToArray(), 0, trailBars1.Count - 2);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			Main.graphics.GraphicsDevice.Textures[0] = flame;
		}
		return false;
	}
}

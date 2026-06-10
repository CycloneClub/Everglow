using System.Runtime.InteropServices;
using Everglow.Commons.DataStructures;
using Terraria.DataStructures;

namespace Everglow.Myth.LanternMoon.Projectiles.Item_Shoot;

public class Firework12Inches : FireworkProjectile
{
	public override void OnSpawn(IEntitySource source)
	{
		Timer = 0;
		SpawnStyle(Main.rand.Next(13));
		MoveSight = false;
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
}

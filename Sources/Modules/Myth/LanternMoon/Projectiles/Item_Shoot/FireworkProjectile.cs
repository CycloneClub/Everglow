using Terraria.DataStructures;
using static Everglow.Myth.LanternMoon.Projectiles.Item_Shoot.FireworkProjectile;

namespace Everglow.Myth.LanternMoon.Projectiles.Item_Shoot;

public abstract class FireworkProjectile : ModProjectile
{
	public override string Texture => "Everglow/Myth/UIImages/VisualTextures/DarkGrey";

	public struct FlameTrail
	{
		public FlameTrail(Vector3 postion, List<Vector3> oldPos, Vector3 velocity, Color color, float scale, bool active)
		{
			Postion = postion;
			OldPos = oldPos;
			Velocity = velocity;
			Color = color;
			Scale = scale;
			Active = active;
		}

		public List<Vector3> OldPos;
		public Vector3 Postion;
		public Vector3 Velocity;
		public Color Color;
		public float Scale;
		public int TrailStyle;
		public int FlameTimer;
		public float[] ai = new float[5];

		// ai[0]:FlameTimer went over 50 + ai[0] will start to diminish.
		// ai[1]:Only when TrailStyle != 0,FlameTimer went over ai[1] will start spark trail.
		// ai[2]:Flicker strength;
		// ai[3]:FlameTimer went over ai[3] will start blinking.
		// ai[4]:Only when ai[4] != 0,FlameTimer went over ai[4] will split.
		public bool Active;
	}

	public List<FlameTrail> Stars;
	public int Timer;
	public int TrailLength = 75;
	public int MaxStyle = 16;
	public bool MoveSight = true;

	public override void SetDefaults()
	{
		Projectile.width = 200;
		Projectile.height = 200;
		Projectile.timeLeft = 600;
		Projectile.aiStyle = -1;
		Projectile.scale = 1;
		Projectile.friendly = true;
		Projectile.tileCollide = false;
		Projectile.penetrate = -1;
		Projectile.ignoreWater = true;
		Stars = new List<FlameTrail>();
		ProjectileID.Sets.DrawScreenCheckFluff[Type] = 10800;
		SetDef();
	}

	public virtual void SetDef()
	{
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		return base.Colliding(projHitbox, targetHitbox);
	}

	public static Vector3 RodriguesRotate(Vector3 origVec, Vector3 axis, float theta)
	{
		if (axis != new Vector3(0, 0, 0))
		{
			axis = Vector3.Normalize(axis);
		}
		else
		{
			axis = new Vector3(0, 0, -1);
		}
		float cos = MathF.Cos(theta);
		return cos * origVec + (1 - cos) * Vector3.Dot(origVec, axis) * axis + MathF.Sin(theta) * Vector3.Cross(origVec, axis);
	}

	public static Vector2 Projection2D(Vector3 vector, Vector2 center, float viewZ, out float scale)
	{
		float value = -viewZ / (vector.Z - viewZ);
		scale = value;
		var v = new Vector2(vector.X, vector.Y);
		return v + (value - 1) * (v - center);
	}

	public override void OnSpawn(IEntitySource source)
	{
		Timer = 0;
		SpawnStyle(Main.rand.Next(MaxStyle)); // Main.rand.Next(MaxStyle)
	}

	public static Color NormalColor()
	{
		Color c0 = Color.White;
		switch (Main.rand.Next(5))
		{
			case 0:
				c0 = new Color(1f, 0.1f, 0.1f, 0);
				break;
			case 1:
				c0 = new Color(0.05f, 1f, 0.18f, 0);
				break;
			case 2:
				c0 = new Color(0.2f, 0.01f, 0.8f, 0);
				break;
			case 3:
				c0 = new Color(0.7f, 0.01f, 0.4f, 0);
				break;
			case 4:
				c0 = new Color(0.9f, 0.4f, 0.05f, 0);
				break;
		}
		return c0;
	}

	public virtual void SpawnStyle(int style)
	{
		Projectile.damage = 3000;
		var axis = new Vector3(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
		float rot = Main.rand.NextFloat(6.283f);
		int starsCount = 90;
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
						Vector3 velocity = new Vector3(MathF.Cos(phi / length / starsCount * MathHelper.TwoPi) * length, MathF.Sin(theta / (float)starsCount * MathF.PI), MathF.Sin(phi / length / starsCount * MathHelper.TwoPi) * length) * 9f * Main.rand.NextFloat(0.95f, 1.05f);
						velocity = RodriguesRotate(velocity, axis, rot);
						var flame = new FlameTrail(-velocity, new List<Vector3>(), velocity, color, Main.rand.NextFloat(0.85f, 1.15f) * 0.7f, true);
						flame.FlameTimer = Main.rand.Next(-70, 0);
						Stars.Add(flame);
					}
				}
				break;

			// 环状
			case 1:
				if (style == 1)
				{
					Color c0 = NormalColor();
					starsCount = 160;
					int theta2 = 0;
					float length2 = MathF.Cos(theta2 / (float)starsCount * MathF.PI);
					for (int phi = 0; phi < length2 * starsCount; phi += 5)
					{
						Vector3 velocity = new Vector3(MathF.Cos(phi / length2 / starsCount * MathHelper.TwoPi) * length2, MathF.Sin(theta2 / (float)starsCount * MathF.PI), MathF.Sin(phi / length2 / starsCount * MathHelper.TwoPi) * length2) * 10f * Main.rand.NextFloat(0.95f, 1.05f);
						velocity = RodriguesRotate(velocity, axis, rot);
						var flame = new FlameTrail(-velocity, new List<Vector3>(), velocity, c0, Main.rand.NextFloat(0.85f, 1.15f) * 0.7f, true);
						flame.FlameTimer = Main.rand.Next(-10, 0);
						flame.ai[0] = 20;
						Stars.Add(flame);
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
						Vector3 velocity = new Vector3(MathF.Cos(phi / length / starsCount * MathHelper.TwoPi) * length, MathF.Sin(theta / (float)starsCount * MathF.PI), MathF.Sin(phi / length / starsCount * MathHelper.TwoPi) * length) * 9f * Main.rand.NextFloat(0.95f, 1.05f);
						velocity = RodriguesRotate(velocity, axis, rot);
						Stars.Add(new FlameTrail(-velocity, new List<Vector3>(), velocity, Main.rand.NextBool(2) ? color : color1, Main.rand.NextFloat(0.85f, 1.15f) * 0.7f, true));
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
						Vector3 velocity = new Vector3(MathF.Cos(phi / length / starsCount * MathHelper.TwoPi) * length, MathF.Sin(theta / (float)starsCount * MathF.PI), MathF.Sin(phi / length / starsCount * MathHelper.TwoPi) * length) * 9f * Main.rand.NextFloat(0.95f, 1.05f);
						velocity = RodriguesRotate(velocity, axis, rot);
						Stars.Add(new FlameTrail(-velocity, new List<Vector3>(), velocity, Math.Abs(theta) > starsCount * 0.2f ? color : color1, Main.rand.NextFloat(0.85f, 1.15f) * 0.7f, true));
					}
				}
				break;

			// 金色椰树
			case 4:
				for (int theta = -starsCount; theta <= starsCount; theta += 10)
				{
					float length = MathF.Cos(theta / (float)starsCount * MathF.PI);
					for (int phi = 0; phi <= length * starsCount; phi += 5)
					{
						Vector3 velocity = new Vector3(MathF.Cos(phi / length / starsCount * MathHelper.TwoPi) * length, MathF.Sin(theta / (float)starsCount * MathF.PI), MathF.Sin(phi / length / starsCount * MathHelper.TwoPi) * length) * 9f * Main.rand.NextFloat(0.95f, 1.05f);
						velocity = RodriguesRotate(velocity, axis, rot);
						var flame = new FlameTrail(-velocity, new List<Vector3>(), velocity, new Color(0, 0, 0, 0), Main.rand.NextFloat(0.85f, 1.15f) * 0.7f, true);
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
				starsCount = 60;
				for (int theta = -starsCount; theta <= starsCount; theta += 10)
				{
					float length = MathF.Cos(theta / (float)starsCount * MathF.PI);
					for (int phi = 0; phi <= length * starsCount; phi += 5)
					{
						Vector3 velocity = new Vector3(MathF.Cos(phi / length / starsCount * MathHelper.TwoPi) * length, MathF.Sin(theta / (float)starsCount * MathF.PI), MathF.Sin(phi / length / starsCount * MathHelper.TwoPi) * length) * 9f * Main.rand.NextFloat(0.95f, 1.05f);
						velocity = RodriguesRotate(velocity, axis, rot);
						var flame = new FlameTrail(-velocity, new List<Vector3>(), velocity, new Color(0, 0, 0, 0), Main.rand.NextFloat(0.85f, 1.15f) * 0.7f, true);
						flame.TrailStyle = 1;
						flame.FlameTimer = Main.rand.Next(-10, 11);
						flame.ai[0] = 20;
						Stars.Add(flame);
					}
				}
				starsCount = 40;
				for (int theta = -starsCount; theta <= starsCount; theta += 10)
				{
					float length = MathF.Cos(theta / (float)starsCount * MathF.PI);
					for (int phi = 0; phi <= length * starsCount; phi += 5)
					{
						Vector3 velocity = new Vector3(MathF.Cos(phi / length / starsCount * MathHelper.TwoPi) * length, MathF.Sin(theta / (float)starsCount * MathF.PI), MathF.Sin(phi / length / starsCount * MathHelper.TwoPi) * length) * 4f * Main.rand.NextFloat(0.95f, 1.05f);
						velocity = RodriguesRotate(velocity, axis, rot);
						var flame = new FlameTrail(-velocity, new List<Vector3>(), velocity, color, Main.rand.NextFloat(0.85f, 1.15f) * 0.9f, true);
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
						Vector3 velocity = new Vector3(MathF.Cos(phi / length / starsCount * MathHelper.TwoPi) * length, MathF.Sin(theta / (float)starsCount * MathF.PI), MathF.Sin(phi / length / starsCount * MathHelper.TwoPi) * length) * 9f * Main.rand.NextFloat(0.95f, 1.05f);
						velocity = RodriguesRotate(velocity, axis, rot);
						Stars.Add(new FlameTrail(-velocity, new List<Vector3>(), velocity, color1, Main.rand.NextFloat(0.85f, 1.15f) * 0.7f, true));
					}
				}
				for (int theta = 5; theta <= starsCount; theta += 10)
				{
					float length = MathF.Cos(theta / (float)starsCount * MathF.PI);
					for (int phi = 0; phi <= length * starsCount; phi += 5)
					{
						Vector3 velocity = new Vector3(MathF.Cos(phi / length / starsCount * MathHelper.TwoPi) * length, MathF.Sin(theta / (float)starsCount * MathF.PI), MathF.Sin(phi / length / starsCount * MathHelper.TwoPi) * length) * 9f * Main.rand.NextFloat(0.95f, 1.05f);
						velocity = RodriguesRotate(velocity, axis, rot);
						Stars.Add(new FlameTrail(-velocity, new List<Vector3>(), velocity, color, Main.rand.NextFloat(0.85f, 1.15f) * 0.7f, true));
					}
				}
				break;

			// 多个集束
			case 7:
				starsCount = 45;
				for (int theta = -starsCount; theta <= starsCount; theta += 10)
				{
					float length = MathF.Cos(theta / (float)starsCount * MathF.PI);
					for (int phi = 0; phi <= length * starsCount; phi += 5)
					{
						Vector3 velocity = new Vector3(MathF.Cos(phi / length / starsCount * MathHelper.TwoPi) * length, MathF.Sin(theta / (float)starsCount * MathF.PI), MathF.Sin(phi / length / starsCount * MathHelper.TwoPi) * length) * 10f * Main.rand.NextFloat(0.95f, 1.05f);
						velocity = RodriguesRotate(velocity, axis, rot);
						Stars.Add(new FlameTrail(-velocity, new List<Vector3>(), velocity, color1, Main.rand.NextFloat(0.85f, 1.15f) * 1f, true));
						for (int x = 0; x < 6; x++)
						{
							Vector3 newVel = Vector3.Normalize(RodriguesRotate(new Vector3(velocity.Y, -velocity.X, 0), velocity, x / 6f * MathHelper.TwoPi)) * 0.3f * Main.rand.NextFloat(0.85f, 1.15f);
							var flame = new FlameTrail(-velocity, new List<Vector3>(), velocity * 0.4f + newVel, color, Main.rand.NextFloat(0.85f, 1.15f) * 0.4f, true);
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
					starsCount = 70;
					for (int theta = -starsCount; theta <= starsCount; theta += 10)
					{
						float length = MathF.Cos(theta / (float)starsCount * MathF.PI);
						for (int phi = 0; phi <= length * starsCount; phi += 5)
						{
							Vector3 velocity = new Vector3(MathF.Cos(phi / length / starsCount * MathHelper.TwoPi) * length, MathF.Sin(theta / (float)starsCount * MathF.PI), MathF.Sin(phi / length / starsCount * MathHelper.TwoPi) * length) * 5f * Main.rand.NextFloat(0.95f, 1.05f);
							velocity = RodriguesRotate(velocity, axis, rot);
							Stars.Add(new FlameTrail(-velocity, new List<Vector3>(), velocity, color, Main.rand.NextFloat(0.85f, 1.15f) * 0.7f, true));
						}
					}
					starsCount = 140;
					int theta2 = 0;
					float length2 = MathF.Cos(theta2 / (float)starsCount * MathF.PI);
					for (int phi = 0; phi < length2 * starsCount; phi += 5)
					{
						Vector3 velocity = new Vector3(MathF.Cos(phi / length2 / starsCount * MathHelper.TwoPi) * length2, MathF.Sin(theta2 / (float)starsCount * MathF.PI), MathF.Sin(phi / length2 / starsCount * MathHelper.TwoPi) * length2) * 10f * Main.rand.NextFloat(0.95f, 1.05f);
						velocity = RodriguesRotate(velocity, axis, rot);
						var flame = new FlameTrail(-velocity, new List<Vector3>(), velocity, new Color(0, 0, 0, 0), Main.rand.NextFloat(0.85f, 1.15f) * 0.7f, true);
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
					starsCount = 120;
					int theta2 = 0;
					float length2 = MathF.Cos(theta2 / (float)starsCount * MathF.PI);
					for (int phi = 0; phi < length2 * starsCount; phi += 5)
					{
						Vector3 velocity = new Vector3(MathF.Cos(phi / length2 / starsCount * MathHelper.TwoPi) * length2, MathF.Sin(theta2 / (float)starsCount * MathF.PI), MathF.Sin(phi / length2 / starsCount * MathHelper.TwoPi) * length2) * 10f * Main.rand.NextFloat(0.95f, 1.05f);
						velocity = RodriguesRotate(velocity, axis, rot);
						var flame = new FlameTrail(-velocity, new List<Vector3>(), velocity, c0, Main.rand.NextFloat(0.85f, 1.15f) * 0.7f, true);
						flame.FlameTimer = Main.rand.Next(-10, 0);
						flame.ai[0] = 20;
						Stars.Add(flame);
					}
					starsCount = 180;
					for (int theta = -starsCount; theta < starsCount; theta += 10)
					{
						float length = MathF.Cos(theta / (float)starsCount * MathF.PI);
						for (int phi = 0; phi <= length * starsCount; phi += 5)
						{
							Vector3 velocity = new Vector3(MathF.Cos(phi / length / starsCount * MathHelper.TwoPi) * length, MathF.Sin(theta / (float)starsCount * MathF.PI), MathF.Sin(phi / length / starsCount * MathHelper.TwoPi) * length) * 9f * Main.rand.NextFloat(0.95f, 1.05f);
							velocity = RodriguesRotate(velocity, axis, rot);
							if (Math.Abs(theta) > starsCount * 0.24f)
							{
								var flame = new FlameTrail(-velocity, new List<Vector3>(), velocity, new Color(0, 0, 0, 0), Main.rand.NextFloat(0.85f, 1.15f) * 0.7f, true);
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
				starsCount = 70;
				for (int theta = -starsCount; theta < starsCount; theta += 10)
				{
					float length = MathF.Cos(theta / (float)starsCount * MathF.PI);
					for (int phi = 0; phi <= length * starsCount; phi += 5)
					{
						Vector3 velocity = new Vector3(MathF.Cos(phi / length / starsCount * MathHelper.TwoPi) * length, MathF.Sin(theta / (float)starsCount * MathF.PI), MathF.Sin(phi / length / starsCount * MathHelper.TwoPi) * length) * 5f * Main.rand.NextFloat(0.95f, 1.05f);
						velocity = RodriguesRotate(velocity, axis, rot);
						Stars.Add(new FlameTrail(-velocity, new List<Vector3>(), velocity, color, Main.rand.NextFloat(0.85f, 1.15f) * 0.7f, true));
					}
				}
				starsCount = 70;
				for (int theta = -starsCount; theta < starsCount; theta += 10)
				{
					float length = MathF.Cos(theta / (float)starsCount * MathF.PI);
					for (int phi = 0; phi <= length * starsCount; phi += 5)
					{
						Vector3 velocity = new Vector3(MathF.Cos(phi / length / starsCount * MathHelper.TwoPi) * length, MathF.Sin(theta / (float)starsCount * MathF.PI), MathF.Sin(phi / length / starsCount * MathHelper.TwoPi) * length) * 9f * Main.rand.NextFloat(0.95f, 1.05f);
						velocity = RodriguesRotate(velocity, axis, rot);
						Stars.Add(new FlameTrail(-velocity, new List<Vector3>(), velocity, color1, Main.rand.NextFloat(0.85f, 1.15f) * 0.7f, true));
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
						Vector3 velocity = new Vector3(MathF.Cos(phi / length / starsCount * MathHelper.TwoPi) * length, MathF.Sin(theta / (float)starsCount * MathF.PI), MathF.Sin(phi / length / starsCount * MathHelper.TwoPi) * length) * 9f * Main.rand.NextFloat(0.95f, 1.05f);
						velocity = RodriguesRotate(velocity, axis, rot);
						var flame = new FlameTrail(-velocity, new List<Vector3>(), velocity, color, Main.rand.NextFloat(0.85f, 1.15f) * 0.7f, true);
						flame.FlameTimer = Main.rand.Next(-20, 0);
						if (Main.rand.NextBool(10))
						{
							flame = new FlameTrail(-velocity, new List<Vector3>(), velocity, new Color(0, 0, 0, 0), Main.rand.NextFloat(0.85f, 1.15f) * 0.7f, true);
							flame.TrailStyle = 1;
							flame.FlameTimer = Main.rand.Next(-10, 11);
							flame.ai[0] = 20;
							flame.ai[1] = 0;
						}
						Stars.Add(flame);
					}
				}
				break;

			// 闪烁球
			case 12:
				for (int theta = -starsCount; theta <= starsCount; theta += 10)
				{
					float length = MathF.Cos(theta / (float)starsCount * MathF.PI);
					for (int phi = 0; phi <= length * starsCount; phi += 5)
					{
						Vector3 velocity = new Vector3(MathF.Cos(phi / length / starsCount * MathHelper.TwoPi) * length, MathF.Sin(theta / (float)starsCount * MathF.PI), MathF.Sin(phi / length / starsCount * MathHelper.TwoPi) * length) * 9f * Main.rand.NextFloat(0.95f, 1.05f);
						velocity = RodriguesRotate(velocity, axis, rot);
						var flame = new FlameTrail(-velocity, new List<Vector3>(), velocity, color, Main.rand.NextFloat(0.85f, 1.15f) * 0.7f, true);
						flame.FlameTimer = Main.rand.Next(-60, -40);
						flame.ai[3] = Main.rand.Next(20) - 20;
						Stars.Add(flame);
					}
				}
				break;

			// 杂环
			case 13:
				if (style == 13)
				{
					starsCount = 140;
					int theta2 = 0;
					float length2 = MathF.Cos(theta2 / (float)starsCount * MathF.PI);
					for (int phi = 0; phi < length2 * starsCount; phi += 5)
					{
						Vector3 velocity = new Vector3(MathF.Cos(phi / length2 / starsCount * MathHelper.TwoPi) * length2, MathF.Sin(theta2 / (float)starsCount * MathF.PI), MathF.Sin(phi / length2 / starsCount * MathHelper.TwoPi) * length2) * 10f * Main.rand.NextFloat(0.95f, 1.05f);
						velocity = RodriguesRotate(velocity, axis, rot);
						var flame = new FlameTrail(-velocity, new List<Vector3>(), velocity, new Color(0, 0, 0, 0), Main.rand.NextFloat(0.85f, 1.15f) * 0.7f, true);
						flame.TrailStyle = 1;
						flame.FlameTimer = Main.rand.Next(-10, 11);
						flame.ai[0] = 20;
						flame.ai[1] = 10;
						if (phi % 10 == 5)
						{
							velocity *= 0.85f;
							flame = new FlameTrail(-velocity, new List<Vector3>(), velocity, color, Main.rand.NextFloat(0.85f, 1.15f) * 0.7f, true);
							flame.FlameTimer = Main.rand.Next(-60, -40);
							flame.ai[3] = Main.rand.Next(20) - 20;
						}
						Stars.Add(flame);
					}
				}
				break;

			// 分裂
			case 14:
				starsCount = 50;
				for (int theta = -starsCount; theta <= starsCount; theta += 10)
				{
					float length = MathF.Cos(theta / (float)starsCount * MathF.PI);
					for (int phi = 0; phi <= length * starsCount; phi += 5)
					{
						Vector3 velocity = new Vector3(MathF.Cos(phi / length / starsCount * MathHelper.TwoPi) * length, MathF.Sin(theta / (float)starsCount * MathF.PI), MathF.Sin(phi / length / starsCount * MathHelper.TwoPi) * length) * 9f * Main.rand.NextFloat(0.95f, 1.05f);
						velocity = RodriguesRotate(velocity, axis, rot);
						var flame = new FlameTrail(-velocity, new List<Vector3>(), velocity, color, Main.rand.NextFloat(0.85f, 1.15f) * 1.2f, true);
						flame.FlameTimer = Main.rand.Next(-20, 20);
						flame.ai[4] = 40;
						Stars.Add(flame);
					}
				}
				break;

			// 棕色椰树
			case 15:
				for (int theta = -starsCount; theta <= starsCount; theta += 10)
				{
					float length = MathF.Cos(theta / (float)starsCount * MathF.PI);
					for (int phi = 0; phi <= length * starsCount; phi += 5)
					{
						Vector3 velocity = new Vector3(MathF.Cos(phi / length / starsCount * MathHelper.TwoPi) * length, MathF.Sin(theta / (float)starsCount * MathF.PI), MathF.Sin(phi / length / starsCount * MathHelper.TwoPi) * length) * 9f * Main.rand.NextFloat(0.95f, 1.05f);
						velocity = RodriguesRotate(velocity, axis, rot);
						var flame = new FlameTrail(-velocity, new List<Vector3>(), velocity, new Color(0, 0, 0, 0), Main.rand.NextFloat(0.85f, 1.15f) * 0.7f, true);
						flame.TrailStyle = 2;
						flame.FlameTimer = Main.rand.Next(-10, 11);
						flame.ai[0] = 20;
						flame.ai[1] = 4;
						Stars.Add(flame);
					}
				}
				break;
		}
	}

	public FlameTrail UpdateFlameTrail(FlameTrail oldFlametrail)
	{
		oldFlametrail.Postion += oldFlametrail.Velocity;
		if (oldFlametrail.ai[4] != 0)
		{
			if (oldFlametrail.FlameTimer == oldFlametrail.ai[4])
			{
				var subProj = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<FireworkSubExplosion>(), 50, 0f, Projectile.owner, 0, 0);
				if (subProj != null)
				{
					var fireworkSubExplosion = subProj.ModProjectile as FireworkSubExplosion;
					if (fireworkSubExplosion != null)
					{
						fireworkSubExplosion.LegacyStyle = 0;
						fireworkSubExplosion.LegacyColor = oldFlametrail.Color;
						fireworkSubExplosion.LegacyVelocity = oldFlametrail.Velocity;
						fireworkSubExplosion.LegacyPosition = oldFlametrail.Postion;
					}
				}
			}
			if (oldFlametrail.FlameTimer > oldFlametrail.ai[4])
			{
				oldFlametrail.Velocity *= 0.2f;
			}
		}

		if (oldFlametrail.FlameTimer > 50 + oldFlametrail.ai[0])
		{
			oldFlametrail.Scale *= 0.96f;
		}
		oldFlametrail.Velocity.Y += 0.03f;
		oldFlametrail.Velocity *= 0.98f;
		oldFlametrail.Velocity.X += Main.windSpeedCurrent * 0.02f / (oldFlametrail.Scale + 1);
		Vector3 drawPos3D = oldFlametrail.Postion;
		drawPos3D.X += (Projectile.Center - Main.screenPosition).X;
		drawPos3D.Y += (Projectile.Center - Main.screenPosition).Y;
		float scale;
		Vector2 v2Pos = Projection2D(drawPos3D, new Vector2(Main.screenWidth, Main.screenHeight) / 2, 1000, out scale);
		float value = scale * oldFlametrail.Scale / 255f;
		if (value > 0.1f / 255f)
		{
			Lighting.AddLight(v2Pos + Main.screenPosition, oldFlametrail.Color.R * value, oldFlametrail.Color.G * value, oldFlametrail.Color.B * value);
		}
		oldFlametrail.OldPos.Add(oldFlametrail.Postion);
		if (oldFlametrail.OldPos.Count > TrailLength)
		{
			oldFlametrail.OldPos.RemoveAt(0);
		}

		if (Ins.VisualQuality.High)
		{
		}
		else
		{
			if (oldFlametrail.TrailStyle == 1)
			{
				var d = Dust.NewDustDirect(v2Pos + Main.screenPosition, 0, 0, DustID.YellowTorch);
				d.scale = scale * 0.6f;
				d.velocity *= 0.3f;
			}
		}

		if (oldFlametrail.TrailStyle == 1)
		{
			if (value > 0.1f / 255f)
			{
				Lighting.AddLight(v2Pos + Main.screenPosition, new Vector3(1f, 0.6f, 0.4f) * value * 255f * 3);
			}
		}
		if (oldFlametrail.Scale < 0.05f)
		{
			oldFlametrail.Active = false;
		}
		oldFlametrail.FlameTimer++;
		return oldFlametrail;
	}

	public virtual void ModifyView()
	{
		Player player = Main.player[Projectile.owner];
		FireworkVisitor fireworkVisitor = player.GetModPlayer<FireworkVisitor>();
		if (fireworkVisitor != null)
		{
			fireworkVisitor.BestFireworkView += (Projectile.Center + new Vector2(0, 200) - player.Center - fireworkVisitor.BestFireworkView) * 0.4f;
		}
	}

	public override void AI()
	{
		Timer++;
		if (MoveSight)
		{
			ModifyView();
		}
		bool allKilled = true;
		for (int i = 0; i < Stars.Count; i++)
		{
			Stars[i] = UpdateFlameTrail(Stars[i]);
			if (Stars[i].Active)
			{
				allKilled = false;
			}
		}
		if (allKilled)
		{
			Projectile.Kill();
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		return false;
	}
}

public class FireworkProjectileDraw : GlobalProjectile
{
	// 全部合批
	public override bool PreDraw(Projectile projectile, Player player, ref Color lightColor)
	{
		Texture2D flame = Commons.ModAsset.LightPoint.Value;
		var trailBars0 = new List<Vertex2D>();
		var trailBars1 = new List<Vertex2D>();
		var trailBars2 = new List<Vertex2D>();
		var trailBars3 = new List<Vertex2D>();
		var trailBars4 = new List<Vertex2D>();
		if (projectile.ModProjectile is FireworkProjectile)
		{
			var fireProj = projectile.ModProjectile as FireworkProjectile;
			if (fireProj != null)
			{
				foreach (FlameTrail flameTrail in fireProj.Stars)
				{
					if (flameTrail.Active)
					{
						Vector3 drawPos3D = flameTrail.Postion;
						drawPos3D.X += (projectile.Center - Main.screenPosition).X;
						drawPos3D.Y += (projectile.Center - Main.screenPosition).Y;
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
								if (i % 12 != 1 && i < len - 24 && i > 8)
								{
									continue;
								}
								float scale0;
								Vector3 oldPos3D0 = trailPos[i];
								oldPos3D0.X += (projectile.Center - Main.screenPosition).X;
								oldPos3D0.Y += (projectile.Center - Main.screenPosition).Y;
								Vector2 old0 = Projection2D(oldPos3D0, new Vector2(Main.screenWidth, Main.screenHeight) / 2, 1000, out scale0);

								float scale1;
								Vector3 oldPos3D1 = trailPos[i - 1];
								oldPos3D1.X += (projectile.Center - Main.screenPosition).X;
								oldPos3D1.Y += (projectile.Center - Main.screenPosition).Y;
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
								trailBars0.Add(old0 + normal * scale0 * 5 * flameTrail.Scale * MathF.Log(i * 0.1f + 1), light * MathF.Pow(0.965f, i) * 0.1f, new Vector3(0.5f, 0, 0));
								trailBars0.Add(old0 - normal * scale0 * 5 * flameTrail.Scale * MathF.Log(i * 0.1f + 1), light * MathF.Pow(0.965f, i) * 0.1f, new Vector3(0.5f, 1, 0));
								if (i == len - 1)
								{
									Vector2 addVel = normal.RotatedBy(-MathHelper.PiOver2) * scale0 * 2 * flameTrail.Scale;
									trailBars0.Add(old0 + addVel + normal * scale0 * 5 * flameTrail.Scale, Color.Transparent, new Vector3(0.5f, 0, 0));
									trailBars0.Add(old0 + addVel - normal * scale0 * 5 * flameTrail.Scale, Color.Transparent, new Vector3(0.5f, 1, 0));
								}

								if (flameTrail.TrailStyle == 1)
								{
									float zValue = (fireProj.TrailLength - i) / (float)fireProj.TrailLength;
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
									if (fireProj.Timer - i < flameTrail.ai[1])
									{
										zValue = 0;
									}
									if (fireProj.Timer - i < flameTrail.ai[1] + 10)
									{
										zValue *= (fireProj.Timer - i - flameTrail.ai[1]) / 10f;
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
								if (flameTrail.TrailStyle == 2)
								{
									float zValue = (fireProj.TrailLength - i) / (float)fireProj.TrailLength;
									float factor = zValue;
									if (flameTrail.FlameTimer > 60)
									{
										zValue -= (flameTrail.FlameTimer - 60) / 60f;
										if (zValue < 0)
										{
											zValue = 0;
										}
									}
									float width = 0.6f;
									if (len < 75)
									{
										width *= len / 75f;
									}
									if (fireProj.Timer - i < flameTrail.ai[1])
									{
										zValue = 0;
									}
									if (fireProj.Timer - i < flameTrail.ai[1] + 10)
									{
										zValue *= (fireProj.Timer - i - flameTrail.ai[1]) / 10f;
									}
									if (i == 1)
									{
										Vector2 addVel = normal.RotatedBy(-MathHelper.PiOver2) * scale0 * 5 * flameTrail.Scale;
										trailBars1.Add(old0 - addVel + normal * scale0 * 15 * flameTrail.Scale * MathF.Log(i * 0.1f + 1) * width, Color.Transparent, new Vector3(i / 40f, 0, 1));
										trailBars1.Add(old0 - addVel - normal * scale0 * 15 * flameTrail.Scale * MathF.Log(i * 0.1f + 1) * width, Color.Transparent, new Vector3(i / 40f, 1, 1));
									}
									var c0 = Color.Lerp(new Color(64, 18, 18, 0), new Color(255, 246, 196, 0), MathF.Pow(factor, 12f));
									trailBars1.Add(old0 + normal * scale0 * 15 * flameTrail.Scale * MathF.Log(i * 0.1f + 1) * width, c0, new Vector3(i / 40f, 0, zValue));
									trailBars1.Add(old0 - normal * scale0 * 15 * flameTrail.Scale * MathF.Log(i * 0.1f + 1) * width, c0, new Vector3(i / 40f, 1, zValue));
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
							float mulLight = (20 + flameTrail.ai[0] - flameTrail.FlameTimer) / 30f;
							mulLight = Math.Clamp(mulLight, 0, 1);
							for (int j = 1; j < 5 * mulLight; j++)
							{
								trailBars0.Add(v2Pos + new Vector2(-0.5f, -0.5f) * sizeValue * j * 2, Color.Transparent, new Vector3(0, 0, 0));
								trailBars0.Add(v2Pos + new Vector2(0.5f, -0.5f) * sizeValue * j * 2, Color.Transparent, new Vector3(1, 0, 0));

								trailBars0.Add(v2Pos + new Vector2(-0.5f, -0.5f) * sizeValue * j * 2, flameTrail.Color * (0.1f / j), new Vector3(0, 0, 0));
								trailBars0.Add(v2Pos + new Vector2(0.5f, -0.5f) * sizeValue * j * 2, flameTrail.Color * (0.1f / j), new Vector3(1, 0, 0));

								trailBars0.Add(v2Pos + new Vector2(-0.5f, 0.5f) * sizeValue * j * 2, flameTrail.Color * (0.1f / j), new Vector3(0, 1, 0));
								trailBars0.Add(v2Pos + new Vector2(0.5f, 0.5f) * sizeValue * j * 2, flameTrail.Color * (0.1f / j), new Vector3(1, 1, 0));

								trailBars0.Add(v2Pos + new Vector2(-0.5f, 0.5f) * sizeValue * j * 2, Color.Transparent, new Vector3(0, 1, 0));
								trailBars0.Add(v2Pos + new Vector2(0.5f, 0.5f) * sizeValue * j * 2, Color.Transparent, new Vector3(1, 1, 0));
							}

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
		return base.PreDraw(projectile, player, ref lightColor);
	}
}

using Everglow.Myth.Common;
using Terraria.GameContent;

namespace Everglow.Myth.TheFirefly.Projectiles;

public class MothMagicArray : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 80;
		Projectile.height = 80;
		Projectile.tileCollide = false; // Makes the minion go through tiles freely
		Projectile.hostile = false;
		Projectile.timeLeft = 21;
		Projectile.friendly = false; // Only controls if it deals damage to enemies on contact (more on that later)
		Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
		Projectile.aiStyle = -1;
		Projectile.DamageType = DamageClass.Summon;
	}

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		int AttackTime = (int)(player.HeldItem.useTime / 0.6);
		Projectile.localAI[0] += 1;
		if (Projectile.timeLeft == 21)
		{
		}
		if (Main.mouseRight && player.statMana >= player.ownedProjectileCounts[ModContent.ProjectileType<GlowMoth>()] && Projectile.timeLeft >= 18)
		{
			if (Projectile.localAI[0] % AttackTime == 0)
			{
				for (int f = 0; f < player.ownedProjectileCounts[ModContent.ProjectileType<GlowMoth>()]; f++)
				{
					var proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Main.rand.NextVector2Unit() * Main.rand.Next(6, 13), ModContent.ProjectileType<PhantomMoth>(), (int)(Projectile.damage * 0.7), 0, Projectile.owner, Main.MouseWorld.X, Main.MouseWorld.Y);
					proj.netUpdate2 = true;
					proj.CritChance = Projectile.CritChance;
				}
			}
			if (Projectile.localAI[0] % (AttackTime / 5) == 0)
				player.statMana -= player.ownedProjectileCounts[ModContent.ProjectileType<GlowMoth>()];
			Projectile.timeLeft = 20;
		}
		else
		{
		}
	}

	public static void DrawDoubleLine(Vector2 StartPos, Vector2 EndPos, Color color1, Color color2)
	{
		float Wid = 1f;
		Vector2 Width = Vector2.Normalize(StartPos - EndPos).RotatedBy(Math.PI / 2d) * Wid;

		var vertex2Ds = new List<Vertex2D>();

		for (int x = 0; x < 3; x++)
		{
			vertex2Ds.Add(new Vertex2D(StartPos + Width + new Vector2(x / 3f).RotatedBy(x), color1, new Vector3(0, 0, 0)));
			vertex2Ds.Add(new Vertex2D(EndPos + Width + new Vector2(x / 3f).RotatedBy(x), color2, new Vector3(0, 0, 0)));
			vertex2Ds.Add(new Vertex2D(StartPos - Width + new Vector2(x / 3f).RotatedBy(x), color1, new Vector3(0, 0, 0)));

			vertex2Ds.Add(new Vertex2D(EndPos + Width + new Vector2(x / 3f).RotatedBy(x), color2, new Vector3(0, 0, 0)));
			vertex2Ds.Add(new Vertex2D(EndPos - Width + new Vector2(x / 3f).RotatedBy(x), color2, new Vector3(0, 0, 0)));
			vertex2Ds.Add(new Vertex2D(StartPos - Width + new Vector2(x / 3f).RotatedBy(x), color1, new Vector3(0, 0, 0)));
		}

		Main.graphics.GraphicsDevice.Textures[0] = TextureAssets.MagicPixel.Value;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertex2Ds.ToArray(), 0, vertex2Ds.Count / 3);
	}

	private float CirR0 = 0;

	public Color GetProjectileAlpha(Color orig)
	{
		var color = new Color(0, 0, 0, 0)
		{
			R = (byte)(orig.R * (255 - Projectile.alpha) / 255f),
			G = (byte)(orig.G * (255 - Projectile.alpha) / 255f),
			B = (byte)(orig.B * (255 - Projectile.alpha) / 255f),
			A = (byte)(orig.A * (255 - Projectile.alpha) / 255f),
		};
		return color;
	}

	private Vector2 OldAimPos = Vector2.Zero;

	public override void PostDraw(Color lightColor)
	{
		Player player = Main.player[Projectile.owner];
		int AttackTime = (int)(player.HeldItem.useTime / 0.6);
		CirR0 += 0.007f;
		float Rad;
		if (Projectile.timeLeft >= 20)
			Rad = Math.Min(Projectile.localAI[0] * 3, 90);
		else
		{
			Rad = Math.Min(Projectile.localAI[0] * 3, 90) * Projectile.timeLeft / 20f;
		}
		Rad = Rad * Rad / 90f;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		var Vx = new List<Vertex2D>();

		Vector2 vf = Projectile.Center - Main.screenPosition;
		var color2 = new Color(0, 120, 255, 0);

		var color3 = new Color(0, 120, 255, 0);
		color3.G = (byte)(color3.G * (Math.Cos(Projectile.localAI[0] / (AttackTime * 0.5) * Math.PI) + 1) / 2d);
		color3.B = (byte)(color3.B * (Math.Cos(Projectile.localAI[0] / (AttackTime * 0.5) * Math.PI) + 1) / 2d);

		color3 = GetProjectileAlpha(color3);

		var color4 = new Color(0, 120, 255, 0);
		color4 = GetProjectileAlpha(color4);

		for (int h = 0; h < 90; h++)
		{
			Vector2 v0 = new Vector2(0, Rad * 0.78f).RotatedBy(h / 45d * Math.PI - CirR0 * 0.3f);
			Vector2 v1 = new Vector2(0, Rad * 0.78f).RotatedBy((h + 1) / 45d * Math.PI - CirR0 * 0.3f);
			Vx.Add(new Vertex2D(vf + v0, new Color(0, 0, 0, 0.1f * Rad / 90f), new Vector3(h / 30f % 1f, 0, 0)));
			Vx.Add(new Vertex2D(vf + v1, new Color(0, 0, 0, 0.1f * Rad / 90f), new Vector3((0.999f + h) / 30f % 1f, 0, 0)));
			Vx.Add(new Vertex2D(vf, new Color(0, 0, 0, 0.9f * Rad / 90f), new Vector3((0.5f + h) / 30f % 1f, 1, 0)));
		}
		Texture2D t = TextureAssets.MagicPixel.Value;
		Main.graphics.GraphicsDevice.Textures[0] = t;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, Vx.ToArray(), 0, Vx.Count / 3);

		//鼠标圈

		t = ModAsset.BlueFlyD.Value;
		Main.spriteBatch.Draw(t, Main.MouseScreen, null, new Color(0.5f, 0.5f, 0.5f, 0), 0, t.Size() / 2f, 0.75f, SpriteEffects.None, 0);

		//攻击位置,此处顺带标记距离小于120的
		if (Projectile.localAI[0] % AttackTime == 0)
		{
			OldAimPos = Main.MouseWorld;
			float distance = 120;
			foreach (NPC target in Main.npc)
			{
				if (target.CanBeChasedBy(Projectile, false) && Collision.CanHit(Projectile.Center, 1, 1, target.Center, 1, 1))
				{
					if ((target.Center - Main.MouseWorld).Length() < distance)
					{
						distance = (target.Center - Main.MouseWorld).Length();
						player.MinionAttackTargetNPC = target.whoAmI;
					}
				}
			}
		}
		if (OldAimPos != Vector2.Zero)
		{
			float k = (AttackTime - Projectile.localAI[0] % AttackTime) / (AttackTime * 0.5f);
			k = Math.Min(k, 1);
			Vector2 v2 = OldAimPos - Main.screenPosition;
			for (int h = 0; h < 90; h++)
			{
				Vector2 v0 = new Vector2(0, Rad * 0.4f * k).RotatedBy(h / 45d * Math.PI - CirR0 * 2.3f);
				Vector2 v1 = new Vector2(0, Rad * 0.4f * k).RotatedBy((h + 1) / 45d * Math.PI - CirR0 * 2.3f);

				Vx.Add(new Vertex2D(v2 + v0, color3, new Vector3(h / 30f % 1f, 0, 0)));
				Vx.Add(new Vertex2D(v2 + v1, color3, new Vector3((1 + h) / 30f % 1f, 0, 0)));
				Vx.Add(new Vertex2D(v2, color3, new Vector3((0.5f + h) / 30f % 1f, 1, 0)));
			}
			t = ModAsset.Circle0.Value;
			Main.graphics.GraphicsDevice.Textures[0] = t;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, Vx.ToArray(), 0, Vx.Count / 3);

			t = ModAsset.BlueFlyD.Value;
			Main.spriteBatch.Draw(t, v2, null, new Color(0.5f * k, 0.5f * k, 0.5f * k, 0f * k), 0, t.Size() / 2f, 0.75f, SpriteEffects.None, 0);
		}

		//花边圈
		for (int h = 0; h < 90; h++)
		{
			Vector2 v0 = new Vector2(0, Rad).RotatedBy(h / 45d * Math.PI - CirR0 * 0.3f);
			Vector2 v1 = new Vector2(0, Rad).RotatedBy((h + 1) / 45d * Math.PI - CirR0 * 0.3f);
			Vx.Add(new Vertex2D(vf + v0, color2, new Vector3(h / 30f % 1f, 0, 0)));
			Vx.Add(new Vertex2D(vf + v1, color2, new Vector3((0.999f + h) / 30f % 1f, 0, 0)));
			Vx.Add(new Vertex2D(vf, color2, new Vector3((0.5f + h) / 30f % 1f, 1, 0)));
		}
		t = ModAsset.Circle0.Value;
		Main.graphics.GraphicsDevice.Textures[0] = t;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, Vx.ToArray(), 0, Vx.Count / 3);

		//噪声圈
		var Vx2 = new List<Vertex2D>();
		for (int h = 0; h < 90; h++)
		{
			Vector2 v0 = new Vector2(0, Rad).RotatedBy(h / 45d * Math.PI + CirR0 * 0.4f);
			Vector2 v1 = new Vector2(0, Rad).RotatedBy((h + 1) / 45d * Math.PI + CirR0 * 0.4f);
			Vx2.Add(new Vertex2D(vf + v0, color4, new Vector3(h / 30f % 1f, 0, 0)));
			Vx2.Add(new Vertex2D(vf + v1, color4, new Vector3((1 + h) / 30f % 1f, 0, 0)));
			Vx2.Add(new Vertex2D(vf, color4, new Vector3((0.5f + h) / 30f % 1f, 1, 0)));
		}
		t = ModAsset.Circle1.Value;
		Main.graphics.GraphicsDevice.Textures[0] = t;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, Vx2.ToArray(), 0, Vx2.Count / 3);

		//内部圈
		var Vx3 = new List<Vertex2D>();
		for (int h = 0; h < 90; h++)
		{
			Vector2 v0 = new Vector2(0, Rad).RotatedBy(h / 45d * Math.PI + CirR0);
			Vector2 v1 = new Vector2(0, Rad).RotatedBy((h + 1) / 45d * Math.PI + CirR0);
			if (h % 2 == 1)
			{
				Vx3.Add(new Vertex2D(vf + v0, color3, new Vector3(h / 30f % 1f, 0, 0)));
				Vx3.Add(new Vertex2D(vf + v1, color3, new Vector3((1 + h) / 30f % 1f, 0, 0)));
				Vx3.Add(new Vertex2D(vf, color3, new Vector3((0.5f + h) / 30f % 1f, 1, 0)));
			}
			else
			{
				Vx3.Add(new Vertex2D(vf + v0, color3, new Vector3((1 + h) / 30f % 1f, 0, 0)));
				Vx3.Add(new Vertex2D(vf + v1, color3, new Vector3(h / 30f % 1f, 0, 0)));
				Vx3.Add(new Vertex2D(vf, color3, new Vector3((0.5f + h) / 30f % 1f, 1, 0)));
			}
		}
		t = ModAsset.Circle2.Value;
		Main.graphics.GraphicsDevice.Textures[0] = t;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, Vx3.ToArray(), 0, Vx3.Count / 3);

		//旋转线
		var color5 = new Color(84, 0, 255, 0);
		for (int h = 0; h < 7; h++)
		{
			Vector2 v0 = new Vector2(0, Rad * 0.78f).RotatedBy(h / 3.5 * Math.PI - CirR0 * 0.9);
			Vector2 v1 = new Vector2(0, Rad * 0.78f).RotatedBy(h / 3.5 * Math.PI - CirR0 * 0.9 + (Projectile.localAI[0] + AttackTime * 0.5) / (AttackTime * 0.5) * Math.PI);
			DrawDoubleLine(vf + v0, vf + (v1 + v0) * 0.5f, new Color(0, 0, 40, 0), color3);
			DrawDoubleLine(vf + (v1 + v0) * 0.5f, vf + v1, color3, new Color(0, 0, 40, 0));
		}

		color5.R = (byte)(color5.R * (Math.Cos(Projectile.localAI[0] / (AttackTime * 0.5) * Math.PI) + 1) / 4d);
		color5.G = (byte)(color5.G * (Math.Cos(Projectile.localAI[0] / (AttackTime * 0.5) * Math.PI) + 1) / 2d);
		color5.B = (byte)(color5.B * (Math.Cos(Projectile.localAI[0] / (AttackTime * 0.5) * Math.PI) + 1) / 2d);

		color5 = GetProjectileAlpha(color5);

		//固定线
		for (int h = 0; h < 7; h++)
		{
			Vector2 v0 = new Vector2(0, Rad * 0.78f).RotatedBy(h / 3.5 * Math.PI - CirR0 * 0.3);
			Vector2 v1 = new Vector2(0, Rad * 0.78f).RotatedBy(h / 3.5 * Math.PI - CirR0 * 0.3 - (4 + ((Projectile.localAI[0] + AttackTime * 0.5) % (AttackTime * 2) > AttackTime ? 0 : 2)) / 7d * Math.PI);
			DrawDoubleLine(vf + v0, vf + (v1 + v0) * 0.5f, new Color(0, 0, 0, 0), color5);
			DrawDoubleLine(vf + (v1 + v0) * 0.5f, vf + v1, color5, new Color(0, 0, 0, 0));
		}
		//对召唤物的连线
		foreach (Projectile p in Main.projectile)
		{
			if (p.active)
			{
				if (p.type == ModContent.ProjectileType<GlowMoth>())
				{
					Vector2 v0 = p.Center - Projectile.Center;
					Vector2 v1 = v0.SafeNormalize(Vector2.Zero);
					if (v0.Length() < 300 && v0.Length() > Rad * 0.78f)
					{
						var color6 = new Color(0, 0, 0, 0);
						float kDis = 300 - v0.Length();
						kDis /= 300f;
						kDis *= Projectile.timeLeft / 20f;
						color6.R = (byte)(color2.R * kDis);
						color6.G = (byte)(color2.G * kDis);
						color6.B = (byte)(color2.B * kDis);

						var color7 = new Color(0, 0, 0, 0);

						kDis = (float)Math.Clamp(Math.Sqrt(kDis) - 0.5f, 0, 1);
						color7.R = (byte)(color2.R * kDis);
						color7.G = (byte)(color2.G * kDis);
						color7.B = (byte)(color2.B * kDis);
						Vector2 Middle = (vf + v1 * Rad * 0.78f + p.Center - Main.screenPosition) * 0.5f;
						DrawDoubleLine(vf + v1 * Rad * 0.78f, Middle, color6, color7);
						DrawDoubleLine(Middle, p.Center - Main.screenPosition, color7, color6);
					}
				}
			}
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		return true;
	}

	private static void DrawCircle(VFXBatch spriteBatch, float radius, float width, Color color, Vector2 center, bool Black = false)
	{
		var circle = new List<Vertex2D>();
		for (int h = 0; h < radius / 2; h += 5)
		{
			circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(h / radius * Math.PI * 4), color, new Vector3(0.5f, 1, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radius + width).RotatedBy(h / radius * Math.PI * 4), color, new Vector3(0.5f, 0, 0)));
		}
		circle.Add(new Vertex2D(center + new Vector2(0, radius), color, new Vector3(0.5f, 1, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius + width), color, new Vector3(0.5f, 0, 0)));
		if (circle.Count > 0)
		{
			Texture2D t = ModAsset.Wave.Value;
			if (Black)
				t = ModAsset.WaveBlack.Value;
			Main.graphics.GraphicsDevice.Textures[0] = t;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, circle.ToArray(), 0, circle.Count - 2);
		}
	}
	private static void DrawCircle(float radius, float width, Color color, Vector2 center, float value0 = 0, float valu1 = 0)
	{
		var circle = new List<Vertex2D>();
		for (int h = 0; h < radius / 2; h++)
		{
			circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(h / radius * Math.PI * 4), color, new Vector3((h / (float)radius * 6 + (float)Main.timeForVisualEffects / 200f) % 1, 1, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radius + width).RotatedBy(h / radius * Math.PI * 4), color, new Vector3((h / (float)radius * 6 + (float)Main.timeForVisualEffects / 200f) % 1, 0, 0)));
		}
		circle.Add(new Vertex2D(center + new Vector2(0, radius), color, new Vector3(0.5f, 1, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius + width), color, new Vector3(0.5f, 0, 0)));
		if (circle.Count > 0)
		{
			Texture2D t = ModAsset.FireLight.Value;
			Main.graphics.GraphicsDevice.Textures[0] = t;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, circle.ToArray(), 0, circle.Count - 2);
		}
	}
}

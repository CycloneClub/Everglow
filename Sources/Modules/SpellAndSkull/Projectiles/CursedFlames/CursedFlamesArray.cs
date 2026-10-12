using Everglow.Commons.MEAC;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;

namespace Everglow.SpellAndSkull.Projectiles.CursedFlames;

public class CursedFlamesArray : ModProjectile, IWarpProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 28;
		Projectile.height = 28;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 10000;
		Projectile.DamageType = DamageClass.Summon;
		Projectile.tileCollide = false;
	}

	public override bool? CanCutTiles()
	{
		return false;
	}

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		Projectile.Center = Projectile.Center * 0.7f + (player.Center + new Vector2(player.direction * 22, 12 * player.gravDir * (float)(0.2 + Math.Sin(Main.timeForVisualEffects / 18d) / 2d))) * 0.3f;
		Projectile.spriteDirection = player.direction;
		Projectile.velocity *= 0;
		if (player.itemTime > 0 && player.HeldItem.type == ItemID.CursedFlames && player.active && !player.dead)
		{
			Projectile.timeLeft = player.itemTime + 60;
			if (timer < 30)
			{
				timer++;
			}
		}
		else
		{
			timer--;
			if (timer < 0)
			{
				Projectile.Kill();
			}
		}
		Player.CompositeArmStretchAmount PCAS = Player.CompositeArmStretchAmount.Full;

		player.SetCompositeArmFront(true, PCAS, (float)(-Math.Sin(Main.timeForVisualEffects / 18d) * 0.6 + 1.2) * -player.direction);
		Vector2 vTOMouse = Main.MouseWorld - player.Center;
		player.SetCompositeArmBack(true, PCAS, (float)(Math.Atan2(vTOMouse.Y, vTOMouse.X) - Math.PI / 2d));
		Projectile.rotation = player.fullRotation;

		ringPos = ringPos * 0.9f + new Vector2(-12 * player.direction, -24 * player.gravDir) * 0.1f;

		// GenerateVFX(12, new Vector2(0, timer * 1.45f).RotatedByRandom(6.283));
	}

	public void GenerateVFX(int Frequency, Vector2 ringPos)
	{
		Player player = Main.player[Projectile.owner];
		float mulVelocity = 1f;
		for (int g = 0; g < Frequency; g++)
		{
			var cf = new CursedFlameDust
			{
				velocity = ringPos * mulVelocity * 0.1f + player.velocity,
				Active = true,
				Visible = true,
				position = player.Center + ringPos + ringPos,
				maxTime = Main.rand.Next(18, 40),
				ai = new float[] { Main.rand.NextFloat(0.1f, 1f), Main.rand.NextFloat(-0.01f, 0.01f), Main.rand.NextFloat(4f, 12f) * ringPos.Length() / 30f },
			};
			Ins.VFXManager.Add(cf);
		}
	}

	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
	{
		behindNPCs.Add(index);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Projectile.hide = false;
		DrawMagicArray(ModAsset.FogTraceShade.Value, new Color(1f, 1f, 1f, 1f));

		DrawMagicArray(Commons.ModAsset.Trail_6.Value, new Color(67, 255, 0, 0));
		return false;
	}

	internal int timer = 0;
	internal Vector2 ringPos = Vector2.Zero;

	public void DrawMagicArray(Texture2D tex, Color c0)
	{
		Player player = Main.player[Projectile.owner];
		Texture2D Water = tex;
		var c1 = new Color(c0.R * 0.39f / 255f, c0.G * 0.39f / 255f, c0.B * 0.39f / 255f, c0.A * 0.39f / 255f);
		DrawTexCircle(timer * 1.6f, 22, c0, player.Center + ringPos - Main.screenPosition, Water, Main.timeForVisualEffects / 12f);
		DrawTexCircle(timer * 1.3f, 32, c1, player.Center + ringPos - Main.screenPosition, Water, -Main.timeForVisualEffects / 36f);

		float timeRot = (float)(Main.timeForVisualEffects / 57d);
		Vector2 Point1 = player.Center + ringPos - Main.screenPosition + new Vector2(0, timer * 1.8f).RotatedBy(Math.PI * 0 + timeRot);
		Vector2 Point2 = player.Center + ringPos - Main.screenPosition + new Vector2(0, timer * 1.8f).RotatedBy(Math.PI * 2 / 4d + timeRot);
		Vector2 Point3 = player.Center + ringPos - Main.screenPosition + new Vector2(0, timer * 1.8f).RotatedBy(Math.PI * 4 / 4d + timeRot);
		Vector2 Point4 = player.Center + ringPos - Main.screenPosition + new Vector2(0, timer * 1.8f).RotatedBy(Math.PI * 6 / 4d + timeRot);

		Vector2 Point5 = player.Center + ringPos - Main.screenPosition + new Vector2(0, timer * 1.8f).RotatedBy(Math.PI * 1 / 4d + timeRot);
		Vector2 Point6 = player.Center + ringPos - Main.screenPosition + new Vector2(0, timer * 1.8f).RotatedBy(Math.PI * 3 / 4d + timeRot);
		Vector2 Point7 = player.Center + ringPos - Main.screenPosition + new Vector2(0, timer * 1.8f).RotatedBy(Math.PI * 5 / 4d + timeRot);
		Vector2 Point8 = player.Center + ringPos - Main.screenPosition + new Vector2(0, timer * 1.8f).RotatedBy(Math.PI * 7 / 4d + timeRot);
		DrawTexLine(Point1, Point6, c1, c1, Water);
		DrawTexLine(Point6, Point4, c1, c1, Water);
		DrawTexLine(Point4, Point5, c1, c1, Water);
		DrawTexLine(Point5, Point3, c1, c1, Water);
		DrawTexLine(Point3, Point8, c1, c1, Water);
		DrawTexLine(Point8, Point2, c1, c1, Water);
		DrawTexLine(Point2, Point7, c1, c1, Water);
		DrawTexLine(Point7, Point1, c1, c1, Water);
	}

	private static void DrawTexCircle(float radius, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
	{
		var circle = new List<Vertex2D>();
		for (int h = 0; h < radius / 2; h++)
		{
			circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(h / radius * Math.PI * 4 + addRot), color, new Vector3(h * 2 / radius, 1, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radius + width + (float)(Math.Sin(Main.timeForVisualEffects / 4 + h / 2d) * 6f)).RotatedBy(h / radius * Math.PI * 4 + addRot), color, new Vector3(h * 2 / radius, 0, 0)));
		}
		circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), color, new Vector3(1, 1, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius + width).RotatedBy(addRot), color, new Vector3(1, 0, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), color, new Vector3(0, 1, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius + width).RotatedBy(addRot), color, new Vector3(0, 0, 0)));

		if (circle.Count > 0)
		{
			Main.graphics.GraphicsDevice.Textures[0] = tex;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, circle.ToArray(), 0, circle.Count - 2);
		}
	}

	private static void DrawTexCircle(VFXBatch sb, float radius, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
	{
		var circle = new List<Vertex2D>();
		for (int h = 0; h < radius / 2; h++)
		{
			circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(h / radius * Math.PI * 4 + addRot), color, new Vector3(h * 2 / radius, 1, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radius + width + (float)(Math.Sin(Main.timeForVisualEffects / 4 + h / 2d) * 6f)).RotatedBy(h / radius * Math.PI * 4 + addRot), color, new Vector3(h * 2 / radius, 0, 0)));
		}
		circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), color, new Vector3(1, 1, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius + width).RotatedBy(addRot), color, new Vector3(1, 0, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), color, new Vector3(0, 1, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius + width).RotatedBy(addRot), color, new Vector3(0, 0, 0)));

		if (circle.Count > 0)
		{
			sb.Draw(tex, circle, PrimitiveType.TriangleStrip);
		}
	}

	public static void DrawTexLine(Vector2 StartPos, Vector2 EndPos, Color color1, Color color2, Texture2D tex)
	{
		float Wid = 3f;
		Vector2 Width = Vector2.Normalize(StartPos - EndPos).RotatedBy(Math.PI / 2d) * Wid;

		var vertex2Ds = new List<Vertex2D>();

		for (int x = 0; x < 3; x++)
		{
			float Value0 = (float)(Main.timeForVisualEffects / 91 + 20) % 1f;
			float Value1 = (float)(Main.timeForVisualEffects / 91 + 20.4) % 1f;

			if (Value1 < Value0)
			{
				float D0 = 1 - Value0;
				Vector2 Delta = EndPos - StartPos;
				vertex2Ds.Add(new Vertex2D(StartPos + Width + new Vector2(x / 3f).RotatedBy(x), color1, new Vector3(Value0, 0, 0)));
				vertex2Ds.Add(new Vertex2D(StartPos + Delta * D0 + Width + new Vector2(x / 3f).RotatedBy(x), color2, new Vector3(1, 0, 0)));
				vertex2Ds.Add(new Vertex2D(StartPos - Width + new Vector2(x / 3f).RotatedBy(x), color1, new Vector3(Value0, 1, 0)));

				vertex2Ds.Add(new Vertex2D(StartPos + Delta * D0 + Width + new Vector2(x / 3f).RotatedBy(x), color2, new Vector3(1, 0, 0)));
				vertex2Ds.Add(new Vertex2D(StartPos + Delta * D0 - Width + new Vector2(x / 3f).RotatedBy(x), color2, new Vector3(1, 1, 0)));
				vertex2Ds.Add(new Vertex2D(StartPos - Width + new Vector2(x / 3f).RotatedBy(x), color1, new Vector3(Value0, 1, 0)));

				vertex2Ds.Add(new Vertex2D(StartPos + Delta * D0 + Width + new Vector2(x / 3f).RotatedBy(x), color1, new Vector3(0, 0, 0)));
				vertex2Ds.Add(new Vertex2D(EndPos + Width + new Vector2(x / 3f).RotatedBy(x), color2, new Vector3(Value1, 0, 0)));
				vertex2Ds.Add(new Vertex2D(StartPos + Delta * D0 - Width + new Vector2(x / 3f).RotatedBy(x), color1, new Vector3(0, 1, 0)));

				vertex2Ds.Add(new Vertex2D(EndPos + Width + new Vector2(x / 3f).RotatedBy(x), color2, new Vector3(Value1, 0, 0)));
				vertex2Ds.Add(new Vertex2D(EndPos - Width + new Vector2(x / 3f).RotatedBy(x), color2, new Vector3(Value1, 1, 0)));
				vertex2Ds.Add(new Vertex2D(StartPos + Delta * D0 - Width + new Vector2(x / 3f).RotatedBy(x), color1, new Vector3(0, 1, 0)));

				continue;
			}

			vertex2Ds.Add(new Vertex2D(StartPos + Width + new Vector2(x / 3f).RotatedBy(x), color1, new Vector3(Value0, 0, 0)));
			vertex2Ds.Add(new Vertex2D(EndPos + Width + new Vector2(x / 3f).RotatedBy(x), color2, new Vector3(Value1, 0, 0)));
			vertex2Ds.Add(new Vertex2D(StartPos - Width + new Vector2(x / 3f).RotatedBy(x), color1, new Vector3(Value0, 1, 0)));

			vertex2Ds.Add(new Vertex2D(EndPos + Width + new Vector2(x / 3f).RotatedBy(x), color2, new Vector3(Value1, 0, 0)));
			vertex2Ds.Add(new Vertex2D(EndPos - Width + new Vector2(x / 3f).RotatedBy(x), color2, new Vector3(Value1, 1, 0)));
			vertex2Ds.Add(new Vertex2D(StartPos - Width + new Vector2(x / 3f).RotatedBy(x), color1, new Vector3(Value0, 1, 0)));
		}

		Main.graphics.GraphicsDevice.Textures[0] = tex;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertex2Ds.ToArray(), 0, vertex2Ds.Count / 3);
	}

	public void DrawWarp(VFXBatch sb)
	{
		Player player = Main.player[Projectile.owner];
		DrawTexCircle(sb, timer * 1.2f, 52, new Color(64, 7, 255, 0), player.Center + ringPos - Main.screenPosition, Commons.ModAsset.Trail_2_thick.Value, Main.timeForVisualEffects / 17);
	}
}

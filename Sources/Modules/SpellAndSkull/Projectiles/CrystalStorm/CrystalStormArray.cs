using Everglow.Commons.Vertex;
using Everglow.SpellAndSkull.Common;

namespace Everglow.SpellAndSkull.Projectiles.CrystalStorm;

internal class CrystalStormArray : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 28;
		Projectile.height = 28;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 10000;
		Projectile.DamageType = DamageClass.Magic;
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
		if (player.itemTime > 0 && player.HeldItem.type == ItemID.CrystalStorm && player.active && !player.dead)
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
	}
	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
	{
		behindNPCs.Add(index);
	}
	public override bool PreDraw(ref Color lightColor)
	{
		Projectile.hide = false;
		DrawMagicArray(ModAsset.CrystalDarkline.Value, new Color(0.6f, 0.6f, 0.6f, 0.6f));

		DrawMagicArray(Commons.ModAsset.Trail_5.Value, new Color(0, 120, 225, 0));


		return false;
	}
	internal int timer = 0;
	internal Vector2 ringPos = Vector2.Zero;
	public void DrawMagicArray(Texture2D tex, Color c0)
	{
		Player player = Main.player[Projectile.owner];
		Texture2D Water = tex;
		Texture2D Crystalline = ModAsset.Crystalline.Value;
		Texture2D CrystalLight = ModAsset.CrystalLight.Value;
		if (tex == ModAsset.CrystalDarkline.Value)
		{
			Crystalline = tex;
			CrystalLight = tex;
		}
		var c1 = new Color(155, 0, 225, 0);
		var c2 = new Color(0, 0, 255, 0);
		DrawTexSquire(timer * 2.88f, 11, c0, player.Center + ringPos - Main.screenPosition, Water, -Main.timeForVisualEffects / 300);
		DrawTexSquire(timer * 3.1f, 24, c2, player.Center + ringPos - Main.screenPosition, Crystalline, -Main.timeForVisualEffects / 300);


		DrawTexSquire(timer * 3.18f, 11, c0, player.Center + ringPos - Main.screenPosition, Water, -Main.timeForVisualEffects / 300 + MathHelper.PiOver4);
		DrawTexSquire(timer * 3.3f, 24, c0, player.Center + ringPos - Main.screenPosition, Crystalline, -Main.timeForVisualEffects / 300 + MathHelper.PiOver4);

		DrawTexCircle(timer * 2.67f, 75, c0, player.Center + ringPos - Main.screenPosition, Crystalline, Main.timeForVisualEffects / 400 + MathHelper.PiOver4);
		DrawTexCircle(timer * 1.3f, 30, c1, player.Center + ringPos - Main.screenPosition, Crystalline, Main.timeForVisualEffects / 400 + MathHelper.PiOver4);

		float timeRot = (float)(Main.timeForVisualEffects / 240d);
		Vector2 Point1 = player.Center + ringPos - Main.screenPosition + new Vector2(0, timer * 1.4f).RotatedBy(Math.PI * 0 + timeRot);
		Vector2 Point2 = player.Center + ringPos - Main.screenPosition + new Vector2(0, timer * 1.4f).RotatedBy(Math.PI * 1 / 4d + timeRot);
		Vector2 Point3 = player.Center + ringPos - Main.screenPosition + new Vector2(0, timer * 1.4f).RotatedBy(Math.PI * 2 / 4d + timeRot);
		Vector2 Point4 = player.Center + ringPos - Main.screenPosition + new Vector2(0, timer * 1.4f).RotatedBy(Math.PI * 3 / 4d + timeRot);
		Vector2 Point5 = player.Center + ringPos - Main.screenPosition + new Vector2(0, timer * 1.4f).RotatedBy(Math.PI * 4 / 4d + timeRot);
		Vector2 Point6 = player.Center + ringPos - Main.screenPosition + new Vector2(0, timer * 1.4f).RotatedBy(Math.PI * 5 / 4d + timeRot);
		Vector2 Point7 = player.Center + ringPos - Main.screenPosition + new Vector2(0, timer * 1.4f).RotatedBy(Math.PI * 6 / 4d + timeRot);
		Vector2 Point8 = player.Center + ringPos - Main.screenPosition + new Vector2(0, timer * 1.4f).RotatedBy(Math.PI * 7 / 4d + timeRot);

		Vector2 Point1_ = player.Center + ringPos - Main.screenPosition + new Vector2(0, timer * 1.4f).RotatedBy(Math.PI * 0 + timeRot + 0.2);
		Vector2 Point2_ = player.Center + ringPos - Main.screenPosition + new Vector2(0, timer * 1.4f).RotatedBy(Math.PI * 1 / 4d + timeRot + 0.2);
		Vector2 Point3_ = player.Center + ringPos - Main.screenPosition + new Vector2(0, timer * 1.4f).RotatedBy(Math.PI * 2 / 4d + timeRot + 0.2);
		Vector2 Point4_ = player.Center + ringPos - Main.screenPosition + new Vector2(0, timer * 1.4f).RotatedBy(Math.PI * 3 / 4d + timeRot + 0.2);
		Vector2 Point5_ = player.Center + ringPos - Main.screenPosition + new Vector2(0, timer * 1.4f).RotatedBy(Math.PI * 4 / 4d + timeRot + 0.2);
		Vector2 Point6_ = player.Center + ringPos - Main.screenPosition + new Vector2(0, timer * 1.4f).RotatedBy(Math.PI * 5 / 4d + timeRot + 0.2);
		Vector2 Point7_ = player.Center + ringPos - Main.screenPosition + new Vector2(0, timer * 1.4f).RotatedBy(Math.PI * 6 / 4d + timeRot + 0.2);
		Vector2 Point8_ = player.Center + ringPos - Main.screenPosition + new Vector2(0, timer * 1.4f).RotatedBy(Math.PI * 7 / 4d + timeRot + 0.2);


		DrawTexLine(Point1_, Point3, c0, c0, CrystalLight, 0.1f);
		DrawTexLine(Point2_, Point4, c0, c0, CrystalLight, 0.4f);
		DrawTexLine(Point3_, Point5, c0, c0, CrystalLight, 0.2f);
		DrawTexLine(Point4_, Point6, c0, c0, CrystalLight, 0.8f);
		DrawTexLine(Point5_, Point7, c0, c0, CrystalLight, 0.5f);
		DrawTexLine(Point6_, Point8, c0, c0, CrystalLight, 0.7f);
		DrawTexLine(Point7_, Point1, c0, c0, CrystalLight, 0.3f);
		DrawTexLine(Point8_, Point2, c0, c0, CrystalLight, 0.1f);

		DrawTexLine(Point1, Point4, c0, c0, CrystalLight, 0.6f);
		DrawTexLine(Point8, Point5, c0, c0, CrystalLight, 0.9f);

		DrawTexLine(Point2, Point7, c0, c0, CrystalLight, 0.1f);
		DrawTexLine(Point3, Point6, c0, c0, CrystalLight, 0.4f);

		DrawTexLine(Point1_, Point4, c0, c0, CrystalLight, 0.2f);
		DrawTexLine(Point8_, Point5, c0, c0, CrystalLight, 0.8f);

		DrawTexLine(Point2_, Point7, c0, c0, CrystalLight, 0.5f);
		DrawTexLine(Point3_, Point6, c0, c0, CrystalLight, 0.7f);

	}
	private static void DrawTexCircle(float radius, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
	{
		var circle = new List<Vertex2D>();
		for (int h = 0; h < radius / 2; h++)
		{
			circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(h / radius * Math.PI * 4 + addRot), color, new Vector3(h * 2 / radius, 1, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(h / radius * Math.PI * 4 + addRot), color, new Vector3(h * 2 / radius, 0, 0)));
		}
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(addRot), color, new Vector3(1, 1, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), color, new Vector3(1, 0, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(addRot), color, new Vector3(0, 1, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), color, new Vector3(0, 0, 0)));
		if (circle.Count > 0)
		{
			Main.graphics.GraphicsDevice.Textures[0] = tex;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, circle.ToArray(), 0, circle.Count - 2);
		}
	}
	private static void DrawTexSquire(float radius, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
	{
		var circle = new List<Vertex2D>();
		for (int h = 0; h < 5; h++)
		{
			float Value0 = (float)(h / 4f + Main.timeForVisualEffects / 191d) % 1f;
			float Value1 = (float)((h + 1) / 4f + Main.timeForVisualEffects / 191d) % 1f;

			circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(Math.PI / 2 * h + addRot), color, new Vector3(Value0, 1, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(Math.PI / 2 * h + addRot), color, new Vector3(Value0, 0, 0)));

			if (Value1 < Value0)
			{
				float D0 = (1 - Value0) * 4;
				Vector2 Delta = new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(Math.PI / 2 * (h + 1) + addRot) - new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(Math.PI / 2 * h + addRot);
				Vector2 DeltaWidth = new Vector2(0, radius).RotatedBy(Math.PI / 2 * (h + 1) + addRot) - new Vector2(0, radius).RotatedBy(Math.PI / 2 * h + addRot);

				if (h < 4)
				{
					circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(Math.PI / 2 * h + addRot) + Delta * D0, color, new Vector3(1, 1, 0)));
					circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(Math.PI / 2 * h + addRot) + DeltaWidth * D0, color, new Vector3(1, 0, 0)));

					circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(Math.PI / 2 * h + addRot) + Delta * D0, color, new Vector3(0, 1, 0)));
					circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(Math.PI / 2 * h + addRot) + DeltaWidth * D0, color, new Vector3(0, 0, 0)));
				}

			}
		}
		if (circle.Count > 0)
		{
			Main.graphics.GraphicsDevice.Textures[0] = tex;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, circle.ToArray(), 0, circle.Count - 2);
		}

	}
	public void DrawTexLine(Vector2 StartPos, Vector2 EndPos, Color color1, Color color2, Texture2D tex, float AddValue = 0)
	{
		float Wid = 24f;
		Vector2 Width = Vector2.Normalize(StartPos - EndPos).RotatedBy(Math.PI / 2d) * Wid;

		var vertex2Ds = new List<Vertex2D>();
		float Value0 = (float)(Main.timeForVisualEffects / 291d + 20 + AddValue) % 1f;
		float Value1 = (float)(Main.timeForVisualEffects / 291d + 20.1 + AddValue) % 1f;
		if (Value1 < Value0)
		{
			float D0 = 1 - Value0;
			Vector2 Delta = EndPos - StartPos;

			vertex2Ds.Add(new Vertex2D(StartPos + Delta * D0 + Width, color2, new Vector3(1, 0, 0)));
			vertex2Ds.Add(new Vertex2D(StartPos + Delta * D0 - Width, color2, new Vector3(1, 1, 0)));

			vertex2Ds.Add(new Vertex2D(StartPos + Width, color1, new Vector3(Value0, 0, 0)));
			vertex2Ds.Add(new Vertex2D(StartPos - Width, color1, new Vector3(Value0, 1, 0)));



			vertex2Ds.Add(new Vertex2D(StartPos + Delta * D0 + Width, color1, new Vector3(0, 0, 0)));
			vertex2Ds.Add(new Vertex2D(StartPos + Delta * D0 - Width, color1, new Vector3(0, 1, 0)));

			vertex2Ds.Add(new Vertex2D(EndPos + Width, color2, new Vector3(Value1, 0, 0)));
			vertex2Ds.Add(new Vertex2D(EndPos - Width, color2, new Vector3(Value1, 1, 0)));
		}
		else
		{
			vertex2Ds.Add(new Vertex2D(StartPos + Width, color1, new Vector3(Value0, 0, 0)));
			vertex2Ds.Add(new Vertex2D(StartPos - Width, color1, new Vector3(Value0, 1, 0)));

			vertex2Ds.Add(new Vertex2D(EndPos + Width, color2, new Vector3(Value1, 0, 0)));
			vertex2Ds.Add(new Vertex2D(EndPos - Width, color2, new Vector3(Value1, 1, 0)));
		}

		Main.graphics.GraphicsDevice.Textures[0] = tex;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertex2Ds.ToArray(), 0, vertex2Ds.Count - 2);
	}
}

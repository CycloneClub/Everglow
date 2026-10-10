using Everglow.Commons.MEAC;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;

namespace Everglow.SpellAndSkull.Projectiles.DemonScythe;

public class DemonScytheArray : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.drawLayer = ProjectileDrawLayerID.BehindNPCs;
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
		if (player.itemTime > 0 && player.HeldItem.type == ItemID.DemonScythe && player.active && !player.dead)
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


	public override bool PreDraw(Player player, ref Color lightColor)
	{
		DrawMagicArray(Commons.ModAsset.Trail_5_black.Value, new Color(1f, 1f, 1f, 1f));
		DrawMagicArray(Commons.ModAsset.Trail_5_black.Value, new Color(1f, 1f, 1f, 1f));
		DrawMagicArray(Commons.ModAsset.Trail_5.Value, new Color(0.4f, 0.0f, 0.8f, 0));
		return false;
	}

	internal int timer = 0;
	internal Vector2 ringPos = Vector2.Zero;

	public void DrawMagicArray(Texture2D tex, Color c0)
	{
		Player player = Main.player[Projectile.owner];
		Texture2D Water = tex;
		var c1 = new Color(c0.R * 0.19f / 255f, c0.G * 0.19f / 255f, c0.B * 0.19f / 255f, c0.A * 0.19f / 255f);
		var c2 = new Color(c0.R * 0.09f / 255f, c0.G * 0.09f / 255f, c0.B * 0.09f / 255f, c0.A * 0.09f / 255f);
		float Size0 = (float)(Math.Sin(Main.timeForVisualEffects / 12) / 7d + 1);
		float Size1 = (float)(Math.Sin((Main.timeForVisualEffects + 40) / 24) / 7d + 1);
		DrawTexCircle(timer * 1.6f * Size0, 25, c0, player.Center + ringPos - Main.screenPosition, Water, Main.timeForVisualEffects / 17);
		DrawTexCircle(timer * 1.5f * Size0, 15, c1, player.Center + ringPos - Main.screenPosition, Water, Main.timeForVisualEffects / 57);
		DrawTexCircle(timer * 1.4f * Size0, 15, c2, player.Center + ringPos - Main.screenPosition, Water, Main.timeForVisualEffects / 227);
		DrawTexMoon(timer * 1.6f * Size0, 25, c0, player.Center + ringPos - Main.screenPosition, ModAsset.BloomLight.Value, Main.timeForVisualEffects / 3);
		DrawTexCircle(timer * 0.8f, 25, c0 * Size1, player.Center + ringPos - Main.screenPosition + new Vector2(0, timer * 0.8f).RotatedBy(-Main.timeForVisualEffects / 36), Water, -Main.timeForVisualEffects / 7);
		DrawTexCircle(timer * 0.7f, 12, c1 * Size1, player.Center + ringPos - Main.screenPosition + new Vector2(0, timer * 0.8f).RotatedBy(-Main.timeForVisualEffects / 36), Water, -Main.timeForVisualEffects / 27);
		DrawTexCircle(timer * 0.63f, 12, c2 * Size1, player.Center + ringPos - Main.screenPosition + new Vector2(0, timer * 0.8f).RotatedBy(-Main.timeForVisualEffects / 36), Water, -Main.timeForVisualEffects / 127);
		DrawTexMoon(timer * 0.8f, 25, c0 * Size1, player.Center + ringPos - Main.screenPosition + new Vector2(0, timer * 0.8f).RotatedBy(-Main.timeForVisualEffects / 36), ModAsset.BloomLight.Value, -Main.timeForVisualEffects / 1.8);
	}

	private static void DrawTexCircle(float radius, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
	{
		var circle = new List<Vertex2D>();
		for (int h = 0; h < radius / 2; h++)
		{
			circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(h / radius * Math.PI * 4 + addRot), color, new Vector3(h * 2 / radius, 1, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radius + width).RotatedBy(h / radius * Math.PI * 4 + addRot), color, new Vector3(h * 2 / radius, 0, 0)));
		}
		circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), color, new Vector3(0.5f, 1, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius + width).RotatedBy(addRot), color, new Vector3(0.5f, 0, 0)));
		if (circle.Count > 0)
		{
			Main.graphics.GraphicsDevice.Textures[0] = tex;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, circle.ToArray(), 0, circle.Count - 2);
		}
	}

	private static void DrawTexMoon(float radius, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
	{
		var circle = new List<Vertex2D>();
		for (int h = 0; h < radius * 5; h++)
		{
			circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(h / radius * Math.PI * 0.27 + addRot), color, new Vector3(h * 0.2f / radius, 1, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radius + width).RotatedBy(h / radius * Math.PI * 0.27 + addRot), color, new Vector3(h * 0.2f / radius, 0, 0)));
		}

		// circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), color, new Vector3(0.5f, 1, 0)));
		// circle.Add(new Vertex2D(center + new Vector2(0, radius + width).RotatedBy(addRot), color, new Vector3(0.5f, 0, 0)));
		if (circle.Count > 0)
		{
			Main.graphics.GraphicsDevice.Textures[0] = tex;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, circle.ToArray(), 0, circle.Count - 2);
		}
	}

	private static void DrawTexMoon(VFXBatch spriteBatch, float radius, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
	{
		var circle = new List<Vertex2D>();
		for (int h = 0; h < radius * 5; h++)
		{
			circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(h / radius * Math.PI * 0.27 + addRot), color, new Vector3(h * 0.2f / radius, 1, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radius + width).RotatedBy(h / radius * Math.PI * 0.27 + addRot), color, new Vector3(h * 0.2f / radius, 0, 0)));
		}

		// circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), color, new Vector3(0.5f, 1, 0)));
		// circle.Add(new Vertex2D(center + new Vector2(0, radius + width).RotatedBy(addRot), color, new Vector3(0.5f, 0, 0)));
		if (circle.Count > 0)
		{
			spriteBatch.Draw(tex, circle, PrimitiveType.TriangleStrip);
		}
	}

	public static void DrawTexLine(Vector2 StartPos, Vector2 EndPos, Color color1, Color color2, Texture2D tex)
	{
		float Wid = 6f;
		Vector2 Width = Vector2.Normalize(StartPos - EndPos).RotatedBy(Math.PI / 2d) * Wid;

		var vertex2Ds = new List<Vertex2D>();

		for (int x = 0; x < 3; x++)
		{
			float Value0 = (float)(Main.timeForVisualEffects / 291d + 20) % 1f;
			float Value1 = (float)(Main.timeForVisualEffects / 291d + 20.03) % 1f;
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
}

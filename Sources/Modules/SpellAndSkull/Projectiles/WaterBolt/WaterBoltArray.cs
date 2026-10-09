using Everglow.Commons.MEAC;
using Everglow.Commons.VFX;
using static Everglow.SpellAndSkull.Common.SpellAndSkullUtils;
namespace Everglow.SpellAndSkull.Projectiles.WaterBolt;

public class WaterBoltArray : ModProjectile, IWarpProjectile
{
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
		if (player.itemTime > 0 && player.HeldItem.type == ItemID.WaterBolt && player.active && !player.dead)
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
		DrawMagicArray(Commons.ModAsset.Trail_5_black.Value, new Color(1f, 1f, 1f, 1f));
		DrawMagicArray(Commons.ModAsset.Trail_5.Value, new Color(0, 0.45f, 1f, 0));
		return false;
	}

	internal int timer = 0;
	internal Vector2 ringPos = Vector2.Zero;

	public void DrawMagicArray(Texture2D tex, Color c0)
	{
		Player player = Main.player[Projectile.owner];
		Texture2D Water = tex;
		var c1 = new Color(c0.R * 0.39f / 255f, c0.G * 0.39f / 255f, c0.B * 0.39f / 255f, c0.A * 0.39f / 255f);
		DrawTexCircle(timer * 1.6f, 22, c0, player.Center + ringPos - Main.screenPosition, Water, Main.timeForVisualEffects / 17);
		DrawTexCircle(timer * 1.3f, 32, c1, player.Center + ringPos - Main.screenPosition, Water, -Main.timeForVisualEffects / 17);

		float timeRot = (float)(Main.timeForVisualEffects / 57d);
		Vector2 Point1 = player.Center + ringPos - Main.screenPosition + new Vector2(0, timer * 1.8f).RotatedBy(Math.PI * 0 + timeRot);
		Vector2 Point2 = player.Center + ringPos - Main.screenPosition + new Vector2(0, timer * 1.8f).RotatedBy(Math.PI * 2 / 3d + timeRot);
		Vector2 Point3 = player.Center + ringPos - Main.screenPosition + new Vector2(0, timer * 1.8f).RotatedBy(Math.PI * 4 / 3d + timeRot);

		Vector2 Point4 = player.Center + ringPos - Main.screenPosition + new Vector2(0, timer * 1.8f).RotatedBy(Math.PI * 1 / 3d + timeRot);
		Vector2 Point5 = player.Center + ringPos - Main.screenPosition + new Vector2(0, timer * 1.8f).RotatedBy(Math.PI * 3 / 3d + timeRot);
		Vector2 Point6 = player.Center + ringPos - Main.screenPosition + new Vector2(0, timer * 1.8f).RotatedBy(Math.PI * 5 / 3d + timeRot);
		DrawTexLine(Point1, Point2, c1, c1, Water);
		DrawTexLine(Point2, Point3, c1, c1, Water);
		DrawTexLine(Point3, Point1, c1, c1, Water);

		DrawTexLine(Point4, Point5, c1, c1, Water);
		DrawTexLine(Point5, Point6, c1, c1, Water);
		DrawTexLine(Point6, Point4, c1, c1, Water);
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
		Player player = Main.player[Projectile.owner];
		DrawTexCircle(spriteBatch, timer * 1.2f, 52, new Color(64, 7, 255, 0), player.Center + ringPos - Main.screenPosition, Commons.ModAsset.Trail_5.Value, Main.timeForVisualEffects / 17);
	}
}

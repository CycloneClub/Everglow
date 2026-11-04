using Everglow.Commons.MEAC;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.SpellAndSkull.Projectiles.GoldenShower;

public class GoldenShowerBook : MagicBookProjectile, IWarpProjectile
{
	public override void SetDef()
	{
		DustType = DustID.Ichor;
		ItemType = ItemID.GoldenShower;

		TexCoordTop = new Vector2(6, 0);
		TexCoordLeft = new Vector2(0, 24);
		TexCoordDown = new Vector2(22, 24);
		TexCoordRight = new Vector2(28, 0);

		effectColor = new Color(255, 175, 0, 0);
	}

	public override void OnSpawn(IEntitySource source)
	{
		FrontTexture = ModAsset.GoldenShower_A.Value;
		PaperTexture = ModAsset.GoldenShower_C.Value;
		BackTexture = ModAsset.GoldenShower_B.Value;
		GlowTexture = ModAsset.GoldenShower_E.Value;
		Player player = Main.player[Projectile.owner];
		for (int d = 0; d < 16; d++)
		{
			Vector2 velocity = new Vector2(0, Main.rand.NextFloat(-16f, -12f)).RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f));
			Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), Projectile.Center + velocity * -2 + new Vector2(0, -20), velocity, ModContent.ProjectileType<GoldenShowerII>(), player.GetWeaponDamage(player.HeldItem) * 2, player.HeldItem.knockBack, player.whoAmI);
		}
		base.OnSpawn(source);
	}

	public override void SpecialAI()
	{
		Player player = Main.player[Projectile.owner];
		constantUsingTime++;

		if (player.itemTime <= 0 || player.HeldItem.type != ItemID.GoldenShower)
		{
			if (timer < 0)
			{
				float rain = Math.Min(constantUsingTime / 6f, 120);
				constantUsingTime = 0;
				SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);

				Projectile.Kill();
				Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<GoldenShowerBomb>(), 1, 0, player.whoAmI, rain / 4f);
			}
		}
		if (player.itemTime == 2 && player.HeldItem.type == ItemType)
		{
			if (Main.mouseRight)
			{
				constantUsingTime += 3;
				player.statMana -= 7;
				for (int d = 0; d < 2; d++)
				{
					Vector2 velocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero).RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * player.HeldItem.shootSpeed * 1.3f;
					Projectile.NewProjectileDirect(player.GetSource_ItemUse(player.HeldItem), Projectile.Center + velocity * -2 + new Vector2(0, -10), velocity, ModContent.ProjectileType<GoldenShowerII>(), player.GetWeaponDamage(player.HeldItem) * 2, player.HeldItem.knockBack, player.whoAmI);
				}
			}
			else
			{
				Vector2 velocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero) * player.HeldItem.shootSpeed * 1.3f;
				Projectile.NewProjectileDirect(player.GetSource_ItemUse(player.HeldItem), Projectile.Center + velocity * -2 + new Vector2(0, -10), velocity, ModContent.ProjectileType<GoldenShowerII>(), player.GetWeaponDamage(player.HeldItem) * 2, player.HeldItem.knockBack, player.whoAmI);
			}
		}
	}

	internal int constantUsingTime = 0;

	public override void SpecialDraw()
	{
		if (timer < 24 && constantUsingTime > 150)
		{
			float tTimer = timer - 6;
			float Rain = Math.Min(constantUsingTime / 6, 120) / 120f;
			float Fade = (24 - tTimer) / 24f;
			if (Fade < 0)
			{
				Fade = 0;
			}

			Rain *= Fade;
			DrawTexCircle(tTimer * 24 * Rain / Fade, 184 * Fade, new Color(Rain, Rain, Rain, Rain), Projectile.Center - Main.screenPosition, ModAsset.DarklineWave.Value, 0);
			DrawTexCircle(tTimer * 24 * Rain / Fade, 184 * Fade, new Color(Rain, Rain * 0.9f, 0, 0), Projectile.Center - Main.screenPosition, ModAsset.LightlineWave.Value, 0);
		}

		if (timer < 12 && constantUsingTime > 150)
		{
			float Rain = Math.Min(constantUsingTime / 6, 120) / 120f;
			float Fade = (12 - timer) / 12f;
			Rain *= Fade;
			DrawTexCircle(timer * 40 * Rain / Fade, 184 * Fade, new Color(Rain, Rain, Rain, Rain), Projectile.Center - Main.screenPosition, ModAsset.DarklineWave.Value, 0);
			DrawTexCircle(timer * 40 * Rain / Fade, 184 * Fade, new Color(Rain, Rain * 0.9f, 0, 0), Projectile.Center - Main.screenPosition, ModAsset.LightlineWave.Value, 0);
		}
	}

	private static void DrawTexCircle(float radius, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
	{
		var circle = new List<Vertex2D>();
		for (int h = 0; h < radius / 2; h++)
		{
			circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(h / radius * Math.PI * 4 + addRot), color, new Vector3(h * 24 / radius % 1, 1, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(h / radius * Math.PI * 4 + addRot), color, new Vector3(h * 24 / radius % 1, 0, 0)));
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

	private static void DrawTexCircle(VFXBatch sb, float radius, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
	{
		var circle = new List<Vertex2D>();
		for (int h = 0; h < radius / 2; h++)
		{
			circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(h / radius * Math.PI * 4 + addRot), color, new Vector3(h * 24 / radius % 1, 1, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(h / radius * Math.PI * 4 + addRot), color, new Vector3(h * 24 / radius % 1, 0, 0)));
		}
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(addRot), color, new Vector3(1, 1, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), color, new Vector3(1, 0, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(addRot), color, new Vector3(0, 1, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), color, new Vector3(0, 0, 0)));
		if (circle.Count > 0)
		{
			sb.Draw(tex, circle, PrimitiveType.TriangleStrip);
		}
	}

	public void DrawWarp(VFXBatch sb)
	{
		if (timer < 24 && constantUsingTime > 150)
		{
			float tTimer = timer - 6;
			float Rain = Math.Min(constantUsingTime / 6, 120) / 120f;
			float Fade = (24 - tTimer) / 24f;
			if (Fade < 0)
			{
				Fade = 0;
			}

			Rain *= Fade;
			DrawTexCircle(sb, tTimer * 24 * Rain / Fade, 184 * Fade, new Color(Rain, Rain * 0.09f, 0, 0), Projectile.Center - Main.screenPosition, ModAsset.LightlineWave.Value, 0);
		}

		if (timer < 22 && constantUsingTime > 150)
		{
			float Rain = Math.Min(constantUsingTime / 6, 120) / 120f;
			float Fade = (22 - timer) / 22f;
			Rain *= Fade;
			DrawTexCircle(sb, timer * 40 * Rain / Fade, 184 * Fade, new Color(Rain, Rain * 0.09f, 0, 0), Projectile.Center - Main.screenPosition, ModAsset.LightlineWave.Value, 0);
		}
	}
}

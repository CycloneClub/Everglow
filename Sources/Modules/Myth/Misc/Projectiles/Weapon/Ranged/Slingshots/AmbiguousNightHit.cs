using Everglow.Commons.Mechanics.EliminateLight;
using Everglow.Commons.Templates.Weapons.Slingshots;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Ranged.Slingshots;

public class AmbiguousNightHit : ModProjectile
{
	public override string Texture => "Everglow/Myth/Misc/Projectiles/Weapon/Ranged/Slingshots/AmbiguousNight";

	public override void SetDefaults()
	{
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 300;
		Projectile.extraUpdates = 6;
		Projectile.tileCollide = false;
		Projectile.DamageType = DamageClass.Ranged;
	}

	public override void AI()
	{
		NPC target = Main.npc[Main.rand.Next(Main.npc.Length)];
		if (!target.dontTakeDamage && target.active)
		{
			if (!target.friendly)
			{
				if ((target.Center - Projectile.position).Length() < 200)
				{
					int x = (int)Main.timeForVisualEffects;
					Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<NormalHit>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 6f, Main.rand.NextFloat(6.283f));
					Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<AmbiguousLine>(), 40, 0, Projectile.owner, x, 0);
					Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), target.Center, Vector2.Zero, ModContent.ProjectileType<NormalHit>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 6f, Main.rand.NextFloat(6.283f));
					Projectile.NewProjectile(Projectile.GetSource_FromAI(), target.Center, Vector2.Zero, ModContent.ProjectileType<AmbiguousLine>(), 40, 0, Projectile.owner, x, 1/*ai1 = 1才绘制*/);
					// ScreenShaker Gsplayer = Main.player[Main.myPlayer].GetModPlayer<ScreenShaker>();
					// Gsplayer.FlyCamPosition = new Vector2(0, 2).RotatedByRandom(6.283);
				}
			}
		}
		if (Projectile.ai[0] > 0.09f)
		{
			float value = Projectile.ai[0] * 100;
			if (Projectile.timeLeft % 20 == 0 && Projectile.timeLeft > 150)
			{
				Vector2 targetCenter = new Vector2(0, Main.rand.NextFloat(0.45f * value, 1.55f * value)).RotatedByRandom(6.283) * 10 + Projectile.Center;
				int x = (int)Main.timeForVisualEffects;
				Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<AmbiguousLine>(), 40, 0, Projectile.owner, x, 0);
				Projectile.NewProjectile(Projectile.GetSource_FromAI(), targetCenter, Vector2.Zero, ModContent.ProjectileType<AmbiguousLine>(), 40, 0, Projectile.owner, x, 1/*ai1 = 1才绘制*/);
				// ScreenShaker Gsplayer = Main.player[Main.myPlayer].GetModPlayer<ScreenShaker>();
				// Gsplayer.FlyCamPosition = new Vector2(0, 2).RotatedByRandom(6.283);
			}
		}
	}

	public override void PostDraw(Player player, Color lightColor)
	{
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		Texture2D shadow = ModAsset.CursedHit.Value;
		Texture2D blackHole = ModAsset.BlackHole_texture.Value;
		Texture2D blue = ModAsset.CorruptLight.Value;
		float dark = Math.Max(Projectile.timeLeft / 300f, 0);

		Main.spriteBatch.Draw(shadow, Projectile.Center - Main.screenPosition, null, Color.White, 0, shadow.Size() / 2f, 8f * Projectile.ai[0] * dark * dark, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(blackHole, Projectile.Center - Main.screenPosition, null, Color.White, 0, blackHole.Size() / 2f, 1.4f * Projectile.ai[0] * dark, SpriteEffects.None, 0);
		Texture2D light = Commons.ModAsset.StarSlash.Value;
		dark = Math.Max((Projectile.timeLeft - 240) / 70f, 0);
		Main.spriteBatch.Draw(blue, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 0), 0, blue.Size() / 2f, 6f * Projectile.ai[0] * dark, SpriteEffects.None, 0);
		if (Projectile.timeLeft % 4 >= 2)
		{
			Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, new Color(0, 5, 255, 0), 0 + Projectile.ai[1], light.Size() / 2f, new Vector2(1f, dark) * Projectile.ai[0] * 6f, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, new Color(0, 5, 255, 0), 1.57f + Projectile.ai[1], light.Size() / 2f, new Vector2(0.5f, dark) * Projectile.ai[0] * 6f, SpriteEffects.None, 0);
		}
		else if (Projectile.timeLeft % 4 == 0)
		{
			Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, new Color(0, 225, 255, 0), 0 + Projectile.ai[1], light.Size() / 2f, new Vector2(0.5f, dark) * Projectile.ai[0] * 6f, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, new Color(0, 225, 255, 0), 1.57f + Projectile.ai[1], light.Size() / 2f, new Vector2(0.3f, dark) * Projectile.ai[0] * 6f, SpriteEffects.None, 0);
		}
		if (Projectile.timeLeft > 290)
		{
			Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), 0 + Projectile.ai[1], light.Size() / 2f, new Vector2(0.4f, dark) * Projectile.ai[0] * 6f, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), 1.57f + Projectile.ai[1], light.Size() / 2f, new Vector2(0.2f, dark) * Projectile.ai[0] * 6f, SpriteEffects.None, 0);
			float lightValue = Projectile.timeLeft - 290;
			Lighting.AddLight((int)(Projectile.Center.X / 16), (int)(Projectile.Center.Y / 16), lightValue * lightValue / 100f, lightValue * lightValue / 100f, lightValue);
		}
		EliminateLightManager.AddCircle(Projectile.Center, Projectile.timeLeft * Projectile.ai[0] * 8f);
		return false;
	}
}

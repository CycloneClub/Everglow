using Everglow.Commons.DataStructures;
using Everglow.Commons.Graphics;
using Everglow.Commons.Templates.Weapons;
using Everglow.Yggdrasil.KelpCurtain.VFXs;
using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Bosses.KingJellyBall;
using Terraria;
using static Terraria.ModLoader.BackupIO;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Magic;

public class GreenSungloSpore : TrailingProjectile
{
	public override void SetCustomDefaults()
	{
		TrailColor = new Color(0.475f, 1f, 0.475f, 0f);
		TrailWidth = 25f;
		TrailTexture = Commons.ModAsset.Trail_2.Value;
		TrailTextureBlack = Commons.ModAsset.Trail_2_black.Value;
		TrailLength = 25;

		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 25;
		Projectile.width = 20;
		Projectile.height = 20;

		Projectile.friendly = false;
		Projectile.hostile = false;
	}

	public override void Behaviors()
	{
		Projectile.velocity.Y += 0.25f;
	}

	public override void OnKill(int timeLeft)
	{
		foreach (Projectile p in Main.projectile)
		{
			if (p.type == ModContent.ProjectileType<GreenSungloThorns>() && p.owner == Projectile.owner)
			{
				p.Kill();
			}
		}

		int tileX = ((int)Projectile.Center.X) / 16;
		int tileY = ((int)Projectile.Center.Y) / 16;
		do
		{
			tileY -= 3;
		}
		while (WorldGen.SolidTile2(tileX, tileY) || WorldGen.SolidTile2(tileX, tileY + 1) || WorldGen.SolidTile2(tileX, tileY + 2));

		for (; tileY < Main.maxTilesY - 10
			&& (Main.tile[tileX, tileY + 3] == null || Main.tile[tileX - 1, tileY + 3] == null || Main.tile[tileX - 1, tileY + 3] == null
			|| !WorldGen.SolidTile2(tileX, tileY + 3) || !WorldGen.SolidTile2(tileX - 1, tileY + 3) || !WorldGen.SolidTile2(tileX - 1, tileY + 3)); tileY++)
		{
		}

		Vector2 pos = new Vector2(tileX * 16, tileY * 16);
		Projectile.NewProjectileDirect(null, pos, Vector2.Zero, ModContent.ProjectileType<GreenSungloThorns>(), Projectile.damage, 0, Projectile.owner);
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		base.ModifyHitNPC(target, ref modifiers);
		modifiers.FinalDamage *= 0;
		modifiers.HideCombatText();
	}

	public override bool PreDraw(ref Color lightColor)
	{
		DrawTrail();
		if (TimeAfterEntityDestroy <= 0)
		{
			var texMain = (Texture2D)ModContent.Request<Texture2D>(Texture);
			Main.spriteBatch.Draw(texMain, Projectile.Center - Main.screenPosition - Projectile.velocity, null, lightColor, Projectile.velocity.ToRotation() + MathHelper.PiOver2 * 0.5f, texMain.Size() / 2f, 0.8f, SpriteEffects.None, 0);
		}
		return false;
	}

	public override void DrawTrail()
	{
		base.DrawTrail();
	}

	public override Vector3 ModifyTrailTextureCoordinate(float factor, float timeValue, float phase, float widthValue)
	{
		float x = factor - timeValue * 0.5f;
		float y = 1;
		float z = widthValue;
		if (phase == 2)
		{
			y = 0;
		}
		if (phase % 2 == 1)
		{
			y = 0.5f;
		}
		return new Vector3(x, y, z);
	}
}

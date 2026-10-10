using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Bosses.SquamousShell;

public class SquamousShell_shade : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.BossProjectiles;

	public int Timer = 0;

	public override void SetDefaults()
	{
		Projectile.timeLeft = 120;
		Projectile.extraUpdates = 8;
		Projectile.aiStyle = -1;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Timer = 0;
	}

	public override void OnSpawn(IEntitySource source) => base.OnSpawn(source);

	public override void AI()
	{
		Timer++;
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		float fade = 1f;
		if (Projectile.timeLeft < 20f)
		{
			fade *= Projectile.timeLeft / 20f;
		}
		if (Timer < 20f)
		{
			fade *= Timer / 20f;
		}
		Texture2D blackTex = ModAsset.SquamousShell_shade.Value;
		Texture2D lightTex = ModAsset.SquamousShell_shade_light.Value;
		Texture2D glowTex = ModAsset.SquamousShell_shade_glow.Value;
		Vector2 totalSize = blackTex.Size() * 0.5f;
		var spdEff = SpriteEffects.None;
		if (Projectile.spriteDirection == -1)
		{
			spdEff = SpriteEffects.FlipVertically;
		}
		Vector2 drawPos = Projectile.Center - Main.screenPosition;
		var lightColorDraw = lightColor;
		lightColorDraw.A *= 0;
		var glowColorDraw = new Color(1f, 1f, 1f, 0);
		Main.EntitySpriteDraw(blackTex, drawPos, null, Color.White * fade, Projectile.rotation, totalSize, Projectile.scale, spdEff, 0);
		Main.EntitySpriteDraw(lightTex, drawPos, null, lightColorDraw * fade, Projectile.rotation, totalSize, Projectile.scale, spdEff, 0);
		Main.EntitySpriteDraw(glowTex, drawPos, null, glowColorDraw * fade, Projectile.rotation, totalSize, Projectile.scale, spdEff, 0);
		return false;
	}
}

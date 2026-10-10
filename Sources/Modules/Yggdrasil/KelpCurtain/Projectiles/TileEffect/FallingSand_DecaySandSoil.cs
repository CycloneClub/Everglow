using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.KelpCurtain.Items.Placeables;
using Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake;
using Microsoft.Xna.Framework;
using Terraria;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.TileEffect;

public class FallingSand_DecaySandSoil : ModProjectile
{
	public override string LocalizationCategory => LocalizationUtils.Categories.RangedProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 16;
		Projectile.height = 16;
		Projectile.friendly = true;
		Projectile.hostile = true;
		Projectile.DamageType = DamageClass.Ranged;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 120000;
		Projectile.extraUpdates = 2;
	}

	public override void AI()
	{
		Projectile.velocity.Y += 0.02f;

		Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

		if (Projectile.velocity.Y > 4f)
		{
			Projectile.velocity.Y = 4f;
		}
		Projectile.frameCounter++;
		if (Projectile.frameCounter >= 7)
		{
			Projectile.frameCounter = 0;
			++Projectile.frame;
			if (Projectile.frame >= 4)
			{
				Projectile.frame = 0;
			}
		}
		Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(4) - new Vector2(8, 8), 16, 16, ModContent.DustType<DecaySandSoil_Dust>());
		dust.velocity.X *= 0.01f;
		dust.noGravity = true;
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		var texMain = (Texture2D)ModContent.Request<Texture2D>(Texture);
		Rectangle frame = new Rectangle(0, Projectile.frame * 24, 16, 24);
		Main.spriteBatch.Draw(texMain, Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation + MathF.PI, frame.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
		return false;
	}

	public override void OnKill(int timeLeft)
	{
		Point projectilePos = Projectile.Center.ToTileCoordinates();
		bool k = WorldGen.PlaceTile(projectilePos.X, projectilePos.Y, ModContent.TileType<DecaySandSoil>());
		if (!k)
		{
			Item.NewItem(WorldGen.GetItemSource_FromTileBreak(projectilePos.X, projectilePos.Y), new Rectangle(projectilePos.X * 16, projectilePos.Y * 16, 16, 16), new Item(ModContent.ItemType<DecaySandSoil_Item>()));
		}
	}
}

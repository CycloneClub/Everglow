using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Microsoft.Xna.Framework;
using Terraria;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Typeless;

public class MysteriousTablet : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.TypelessProjectiles;

	public int TileCollideCounter { get; set; }

	public override void SetDefaults()
	{
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.scale = 1f;
		Projectile.width = 32;
		Projectile.height = 32;
		Projectile.tileCollide = true;
		Projectile.timeLeft = 600;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 15;
	}

	public override void AI()
	{
		Projectile.velocity.Y = Projectile.velocity.Y + 0.4f;
		if (Projectile.velocity.Y > 16f)
		{
			Projectile.velocity.Y = 16f;
		}
		Projectile.rotation += 0.2f * Projectile.direction;

		//// If there is a non projectile entity, use these in any update funtion to make it bounce.
		// Vector2 checkX = Projectile.position + new Vector2(Projectile.velocity.X, 0);
		// if (Collision.SolidCollision(checkX, Projectile.width, Projectile.height))
		// {
		// Projectile.velocity.X *= -0.69f;
		// }
		// Vector2 checkY = Projectile.position + new Vector2(0, Projectile.velocity.Y);
		// if (Collision.SolidCollision(checkY, Projectile.width, Projectile.height))
		// {
		// Projectile.velocity.Y *= -0.69f;
		// }
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		if (++TileCollideCounter >= 5)
		{
			return true;
		}
		if (Projectile.velocity.X != oldVelocity.X)
		{
			Projectile.velocity.X = -oldVelocity.X;
		}

		if (Projectile.velocity.Y != oldVelocity.Y)
		{
			Projectile.velocity.Y = -oldVelocity.Y;
		}

		Projectile.velocity *= 0.69f;
		return false;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		target.value *= 2;
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		var texture = ModContent.Request<Texture2D>(Texture).Value;
		Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
		return false;
	}

	public override void OnKill(int timeLeft)
	{
		for (int x = 0; x < 8; x++)
		{
			var d = Dust.NewDustDirect(Projectile.position - new Vector2(4), Projectile.width, Projectile.height, ModContent.DustType<MysteriousTablet_fragments>(), 0f, 0f, 0, default, 0.7f);
			d.velocity = new Vector2(0, Main.rand.NextFloat(1f, 4f)).RotatedByRandom(6.283);
		}
		Vector2 goreVel = new Vector2(0, Main.rand.NextFloat(0, 5f)).RotatedByRandom(6.28d);
		Gore.NewGore(null, Projectile.Center + goreVel, goreVel, ModContent.Find<ModGore>("Everglow/MysteriousTablet_gore0").Type, 1f);

		goreVel = new Vector2(0, Main.rand.NextFloat(0, 5f)).RotatedByRandom(6.28d);
		Gore.NewGore(null, Projectile.Center + goreVel, goreVel, ModContent.Find<ModGore>("Everglow/MysteriousTablet_gore1").Type, 1f);

		goreVel = new Vector2(0, Main.rand.NextFloat(0, 5f)).RotatedByRandom(6.28d);
		Gore.NewGore(null, Projectile.Center + goreVel, goreVel, ModContent.Find<ModGore>("Everglow/MysteriousTablet_gore2").Type, 1f);

		goreVel = new Vector2(0, Main.rand.NextFloat(0, 5f)).RotatedByRandom(6.28d);
		Gore.NewGore(null, Projectile.Center + goreVel, goreVel, ModContent.Find<ModGore>("Everglow/MysteriousTablet_gore1").Type, 1f);
	}
}

using Everglow.Food.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace Everglow.Food.Projectiles;

internal class CreamChocolateCup_ChocolateBars : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.penetrate = 1;
		Projectile.timeLeft = 180;
		Projectile.tileCollide = true;
		Projectile.DamageType = DamageClass.Magic;
		base.SetDefaults();
	}

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.frame = Main.rand.Next(10);
		base.OnSpawn(source);
	}

	public override void AI()
	{
		Projectile.velocity.Y += 0.15f;
		Projectile.rotation = MathF.Atan2(Projectile.velocity.X, Projectile.velocity.Y) + MathHelper.PiOver4;
		base.AI();
	}

	public override void OnKill(int timeLeft)
	{
		for (int x = 0; x < 15; x++)
		{
			Dust d = Dust.NewDustDirect(Projectile.Center - new Vector2(4) - new Vector2(4f), 8, 8, ModContent.DustType<ChcolateDust>());
			d.velocity = Projectile.velocity * 0.5f + new Vector2(0, Main.rand.NextFloat(3f)).RotatedByRandom(6.283);
			d.rotation = Main.rand.NextFloat(6.283f);
			d.scale = Main.rand.NextFloat(0.8f, 1.6f);
		}
		base.OnKill(timeLeft);
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		Texture2D mainTex = ModAsset.CreamChocolateCup_ChocolateBars.Value;
		Rectangle rectangle = new Rectangle(Projectile.frame % 5 * 20, Projectile.frame > 5 ? 0 : 22, 20, 22);
		Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, rectangle, lightColor, Projectile.rotation, new Vector2(10, 11), 1f, SpriteEffects.None, 0);
		return false;
	}
}

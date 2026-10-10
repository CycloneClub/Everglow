using Everglow.Commons.Mechanics.ElementalDebuff.Debuffs;
using Microsoft.Xna.Framework;
using Terraria;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Magic;

public class DevilHeartStaff_proj_Kill : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override string Texture => Commons.ModAsset.Empty_Mod;

	public override void SetDefaults()
	{
		Projectile.DamageType = DamageClass.Magic;
		Projectile.width = 240;
		Projectile.height = 240;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.timeLeft = 60;
		Projectile.extraUpdates = 3;
		base.SetDefaults();
	}

	public override void AI() => base.AI();

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		return (targetHitbox.Center() - projHitbox.Center()).Length() < 110f;
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		var color = new Color(255, 255, 255, 0);
		Texture2D star = Commons.ModAsset.StarSlash.Value;
		float width = 3f * Projectile.timeLeft / 60f;
		Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, color, MathHelper.PiOver2 + Projectile.rotation, star.Size() / 2f, new Vector2(width, 1.8f), SpriteEffects.None, 0);
		Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, star.Size() / 2f, new Vector2(width, 1.1f), SpriteEffects.None, 0);
		return false;
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		target.AddElementalDebuffBuildUp(Main.player[Projectile.owner], NecrosisDebuff.ID, Projectile.damage * 3);
		target.AddBuff(BuffID.Confused, 180);
		base.ModifyHitNPC(target, ref modifiers);
	}
}

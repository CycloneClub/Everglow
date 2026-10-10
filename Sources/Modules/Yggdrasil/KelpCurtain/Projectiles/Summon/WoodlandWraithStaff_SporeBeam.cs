using Everglow.Commons.DataStructures;
using Everglow.Commons.Templates.Weapons;
using Everglow.Yggdrasil.KelpCurtain.Dusts;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Summon;

public class WoodlandWraithStaff_SporeBeam : TrailingProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.SummonProjectiles;

	public override void SetCustomDefaults()
	{
		TrailColor = new Color(0.9f, 0.9f, 0.95f, 0f);
		TrailWidth = 12f;
		TrailTexture = Commons.ModAsset.Trail_8.Value;
		TrailTextureBlack = Commons.ModAsset.Trail_8_black.Value;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
		Projectile.width = 6;
		Projectile.height = 6;
	}

	public override void Behaviors()
	{
		if (TimeAfterEntityDestroy < 0)
		{
			Projectile.velocity.Y += 0.15f;
		}
		Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(4) + new Vector2(0, Main.rand.NextFloat(2f, 8f)).RotatedByRandom(MathHelper.TwoPi), 0, 0, ModContent.DustType<WoodlandWraithStaff_Spore2>());
		dust.velocity = Projectile.velocity * Main.rand.NextFloat(0.3f, 0.7f);
		dust.scale = Main.rand.NextFloat(0.3f, 0.7f);
		if (TimeAfterEntityDestroy > 0 && TimeAfterEntityDestroy < 17)
		{
			Projectile.friendly = false;
		}
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		if (TimeAfterEntityDestroy >= 17)
		{
			return (targetHitbox.Center() - Projectile.Center).Length() < 300;
		}
		return base.Colliding(projHitbox, targetHitbox);
	}

	public override void DestroyEntityEffect()
	{
		for (int k = 0; k < 60; k++)
		{
			Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(4), 0, 0, ModContent.DustType<WoodlandWraithStaff_Spore2>());
			dust.velocity = new Vector2(0, Main.rand.Next(3, 15)).RotatedByRandom(MathHelper.TwoPi);
		}
		Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<WoodlandWraithStaff_SporeZone>(), Projectile.damage, 0, Projectile.owner);
		Projectile.velocity = Projectile.oldVelocity;
	}

	public override void DrawSelf()
	{
		var texMain = (Texture2D)ModContent.Request<Texture2D>(Texture);
		Main.spriteBatch.Draw(texMain, Projectile.Center - Main.screenPosition - Projectile.velocity, null, Lighting.GetColor(Projectile.Center.ToTileCoordinates()), Projectile.velocity.ToRotation() - MathHelper.PiOver2, texMain.Size() / 2f, 0.6f, SpriteEffects.None, 0);
	}
}

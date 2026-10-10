using Everglow.Commons.Templates.Weapons;
using Terraria.DataStructures;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee;

public class Glow_Fall : TrailingProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeProjectiles;

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.hide = true;
		Projectile.ai[2] = Main.MouseWorld.Y;
	}

	public override void SetCustomDefaults()
	{
		TrailColor = new Color(0, 0.7f, 1f, 0);
		TrailTextureBlack = Commons.ModAsset.Trail_black.Value;
		SelfLuminous = true;
		TrailLength = 50;
		TrailWidth = 20f;
		Projectile.extraUpdates = 2;
		Projectile.friendly = true;
		Projectile.penetrate = -1;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 200;
		Projectile.tileCollide = false;
	}

	public override void Behaviors()
	{
		Projectile.rotation = MathF.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathHelper.PiOver4;
		if (TimeAfterEntityDestroy < 0)
		{
			for (int t = 0; t < 4; t++)
			{
				Dust d0 = Dust.NewDustDirect(Projectile.Center - Projectile.velocity * Main.rand.NextFloat(1f) - new Vector2(4), 8, 8, DustID.MushroomSpray);
				d0.scale *= Main.rand.NextFloat(0.4f, 1f);
				d0.velocity = Projectile.velocity * Main.rand.NextFloat(0.01f, 0.04f);
			}
		}
		if (TimeAfterEntityDestroy < 80 && TimeAfterEntityDestroy > 0)
		{
			TrailColor = Color.Lerp(new Color(0, 0.7f, 1f, 0), Color.Transparent, 1 - TimeAfterEntityDestroy / 80f);
		}
		if (Projectile.Center.Y > Projectile.ai[2])
		{
			Projectile.tileCollide = true;
		}
	}

	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
	{
		behindNPCsAndTiles.Add(index);
		base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
	}

	public override void DrawSelf()
	{
		base.DrawSelf();
	}

	public override bool PreDraw(ref Color lightColor)
	{
		return base.PreDraw(ref lightColor);
	}

	public override void DrawTrail()
	{
		base.DrawTrail();
	}

	public override void DestroyEntityEffect()
	{
		TimeAfterEntityDestroy = ProjectileID.Sets.TrailCacheLength[Projectile.type];
		Projectile.velocity = Projectile.oldVelocity;
		Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + Projectile.velocity, Vector2.Zero, ModContent.ProjectileType<Glow_Fall_Explosion>(), 0, 0, Projectile.owner, Main.rand.NextFloat(MathHelper.TwoPi));
	}
}

using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.TownNPCs;

public class Georg_Hook_Thunder_Mark : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.TownNPCProjectiles;

	public int Timer = 0;

	public override string Texture => Commons.ModAsset.Empty_Mod;

	public override void SetDefaults()
	{
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 60;
		Projectile.tileCollide = false;
		Projectile.scale = 0;
		Timer = 0;
	}

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.scale = 0;
		base.OnSpawn(source);
	}

	public override void AI()
	{
		Projectile.scale += 0.05f;
		Timer++;
	}

	public override void OnKill(int timeLeft)
	{
		Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<Georg_Hook_Thunder>(), Projectile.damage, Projectile.knockBack, Main.myPlayer, Projectile.ai[0]);
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		Texture2D tex = ModAsset.Georg_Hook_Thunder.Value;
		Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 0), 0, tex.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
		return false;
	}
}

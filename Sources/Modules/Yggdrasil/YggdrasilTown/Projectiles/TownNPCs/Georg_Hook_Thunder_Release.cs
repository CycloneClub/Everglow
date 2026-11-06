using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.TownNPCs;

public class Georg_Hook_Thunder_Release : ModProjectile
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
		Projectile.timeLeft = 150;
		Projectile.tileCollide = false;
		Projectile.extraUpdates = 3;
		Timer = 0;
	}

	public override void OnSpawn(IEntitySource source)
	{
		base.OnSpawn(source);
	}

	public override void AI()
	{
		Timer++;
		if (Timer % 15 == 0 && Timer >= 90)
		{
			var addPos = new Vector2(MathF.Sin(Timer) * 540, 0);
			Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + addPos, Vector2.Zero, ModContent.ProjectileType<Georg_Hook_Thunder_Mark>(), Projectile.damage, Projectile.knockBack, Projectile.owner, addPos.X / 720f + Main.rand.NextFloat(-0.3f, 0.3f));
		}
	}
}

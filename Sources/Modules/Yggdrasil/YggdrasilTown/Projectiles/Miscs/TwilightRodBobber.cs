using Microsoft.Xna.Framework;
using Terraria;
namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Miscs;

public class TwilightRodBobber : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MiscsProjectiles;

	public static Color FishingLineColor => new(201, 201, 201, 120);

	public override void SetDefaults()
	{
		Projectile.width = 14;
		Projectile.height = 14;
		Projectile.aiStyle = ProjAIStyleID.Bobber;
		Projectile.bobber = true;
		Projectile.penetrate = -1;
		Projectile.CloneDefaults(ProjectileID.BobberWooden);
	}

	public override void AI()
	{
		if (!Main.dedServ) // Same as 'NetUtils.NotServer'
		{
			Lighting.AddLight(Projectile.Center, new Vector3(0.2f, 0.5f, 1f));
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */ => base.PreDraw(player, ref lightColor);
}

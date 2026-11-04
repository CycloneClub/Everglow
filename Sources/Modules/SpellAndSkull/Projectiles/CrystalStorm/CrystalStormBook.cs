namespace Everglow.SpellAndSkull.Projectiles.CrystalStorm;

internal class CrystalStormBook : MagicBookProjectile
{
	public override void SetDef()
	{
		DustType = DustID.BlueTorch;
		DustTypeII = DustID.GemSapphire;
		ItemType = ItemID.CrystalStorm;
		effectColor = new Color(32, 103, 169, 100);
	}

	private int times = 0;

	public override void SpecialAI()
	{
		Player player = Main.player[Projectile.owner];

		if (player.itemTime == 2 && player.HeldItem.type == ItemType)
		{
			Vector2 velocity;
			for (int f = 0; f < Main.rand.Next(2, 4); f++)
			{
				velocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero).RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * player.HeldItem.shootSpeed;
				Projectile.NewProjectileDirect(player.GetSource_ItemUse(player.HeldItem), Projectile.Center + velocity * -1, velocity * 1.9f, ModContent.ProjectileType<CrystalStormII>(), player.GetWeaponDamage(player.HeldItem), player.HeldItem.knockBack, player.whoAmI);
			}
			if (times % 33 == 12)
			{
				velocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero) * player.HeldItem.shootSpeed;
				var p1 = Projectile.NewProjectileDirect(player.GetSource_ItemUse(player.HeldItem), Projectile.Center + velocity * -1, velocity * 0.6f, ModContent.ProjectileType<LargeCrystal>(), (int)(player.GetWeaponDamage(player.HeldItem) * 2.8), player.HeldItem.knockBack, player.whoAmI);
				p1.CritChance += player.GetWeaponCrit(player.HeldItem) * 2;
			}
			times++;
			if (times > 33)
			{
				times = 0;
			}
		}

		// string pathBase = "SpellAndSkull/Textures/";
		// FrontTexPath = pathBase + "CrystalStorm_A";
		// PaperTexPath = pathBase + "CrystalStorm_C";
		// BackTexPath = pathBase + "CrystalStorm_B";
	}
}

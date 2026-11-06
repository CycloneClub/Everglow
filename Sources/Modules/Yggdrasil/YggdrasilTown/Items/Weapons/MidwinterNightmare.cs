using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Summon.FaelanternProj;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons;

public class MidwinterNightmare : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.SummonWeapons;

	public override void SetDefaults()
	{
		Item.width = 16;
		Item.height = 16;

		Item.DamageType = DamageClass.Summon;
		Item.damage = 22;
		Item.mana = 14;
		Item.knockBack = 0.5f;

		Item.useTime = Item.useAnimation = 34;
		Item.useStyle = ItemUseStyleID.HoldUp;

		Item.autoReuse = false;
		Item.noMelee = true;

		Item.value = 20000;
		Item.rare = ItemRarityID.Orange;

		Item.shoot = ModContent.ProjectileType<FaelanternProj>();
		Item.shootSpeed = 0;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		int tileX = (int)(Main.mouseX + Main.screenPosition.X) / 16;
		int tileY = (int)(Main.mouseY + Main.screenPosition.Y) / 16;
		if (player.gravDir == -1f)
		{
			tileY = (int)(Main.screenPosition.Y + Main.screenHeight - Main.mouseY) / 16;
		}

		do
		{
			tileY -= 3;
		}
		while (WorldGen.SolidTile2(tileX, tileY) || WorldGen.SolidTile2(tileX, tileY + 1) || WorldGen.SolidTile2(tileX, tileY + 2));

		for (; tileY < Main.maxTilesY - 10
			&& (Main.tile[tileX, tileY + 3] == null || Main.tile[tileX - 1, tileY + 3] == null || Main.tile[tileX - 1, tileY + 3] == null
			|| !WorldGen.SolidTile2(tileX, tileY + 3) || !WorldGen.SolidTile2(tileX - 1, tileY + 3) || !WorldGen.SolidTile2(tileX - 1, tileY + 3)); tileY++)
		{
		}

		Vector2 pos = new Vector2(Main.mouseX + Main.screenPosition.X, tileY * 16 + 8);
		int p = Projectile.NewProjectile(source, pos, Vector2.Zero, type, damage, knockback, player.whoAmI);
		Main.projectile[p].originalDamage = damage;
		player.UpdateMaxTurrets();

		return false;
	}
}

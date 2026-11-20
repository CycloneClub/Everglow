namespace Everglow.Myth.Misc.Items.Ammos;

public class CloudBall : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedWeapons;

	public override void SetStaticDefaults()
	{
		Item.ResearchUnlockCount = 99;
	}

	public override void SetDefaults()
	{
		Item.damage = 3;
		Item.DamageType = DamageClass.Ranged;
		Item.width = 14;
		Item.height = 14;
		Item.useTime = 20;
		Item.useAnimation = 20;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.noUseGraphic = true;
		Item.noMelee = true;
		Item.knockBack = 1;
		Item.value = 0;
		Item.rare = ItemRarityID.White;
		Item.autoReuse = true;
		Item.shoot = ModContent.ProjectileType<Projectiles.CloudBall>();
		Item.shootSpeed = 4;
		Item.crit = 2;
		Item.ammo = Item.type;
		Item.maxStack = Item.CommonMaxStack;
		Item.consumable = true;
	}

	public override void AddRecipes()
	{
		CreateRecipe(10)
			.AddIngredient(ItemID.Cloud)
			.AddTile(TileID.SkyMill)
			.Register();
	}
}

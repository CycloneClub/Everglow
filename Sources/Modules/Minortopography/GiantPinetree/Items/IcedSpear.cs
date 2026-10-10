namespace Everglow.Minortopography.GiantPinetree.Items;

public class IcedSpear : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedWeapons;

	public override void SetDefaults()
	{
		Item.useStyle = ItemUseStyleID.Swing;
		Item.width = 50;
		Item.height = 54;
		Item.useAnimation = 16;
		Item.useTime = 16;
		Item.knockBack = 3f;
		Item.damage = 9;
		Item.rare = ItemRarityID.White;
		Item.UseSound = SoundID.Item1;
		Item.value = 36;
		Item.autoReuse = false;
		Item.DamageType = DamageClass.Ranged;
		Item.consumable = true;
		Item.maxStack = 9999;
		Item.noMelee = true;
		Item.noUseGraphic = true;
		Item.channel = true;

		Item.shoot = ModContent.ProjectileType<Projectiles.IcedSpear>();
	}

	public override void AddRecipes()
	{
		CreateRecipe(8)
			.AddIngredient(ItemID.BorealWood, 2)
			.AddIngredient(ItemID.IceBlock, 1)
			.AddTile(TileID.IceBlock)
			.Register();
	}
}

using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Tools;

public class EldamberPickaxe : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Tools;

	public override void SetDefaults()
	{
		Item.width = 42;
		Item.height = 46;
		Item.damage = 10;
		Item.DamageType = DamageClass.Melee;
		Item.useTime = Item.useAnimation = 10;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.UseSound = SoundID.Item1;
		Item.autoReuse = true;
		Item.useTurn = true;
		Item.maxStack = 1;
		Item.knockBack = 4f;
		Item.rare = ItemRarityID.Blue;
		Item.value = Item.sellPrice(silver: 20);
		Item.pick = 75;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient<YggdrasilAmber>(12);
		recipe.AddIngredient<CrystalNucleusOfJellyKing>(4);
		recipe.AddTile(TileID.Anvils);
		recipe.Register();
	}
}

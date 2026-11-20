using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Tools;

public class TwilightCrystalPickaxe : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Tools;

	public override void SetDefaults()
	{
		Item.width = 30;
		Item.height = 30;
		Item.damage = 28;
		Item.DamageType = DamageClass.Melee;
		Item.useTime = Item.useAnimation = 25;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.UseSound = SoundID.Item1;
		Item.autoReuse = true;
		Item.useTurn = true;
		Item.maxStack = 1;
		Item.knockBack = 2f;
		Item.rare = ItemRarityID.Orange;
		Item.value = Item.buyPrice(gold: 3);
		Item.pick = 90;
		Item.tileBoost = 1;
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient<TwilightCrystal>(12)
			.AddTile(TileID.Anvils)
			.Register();
	}
}

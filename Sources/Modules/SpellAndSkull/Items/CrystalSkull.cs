using Terraria.DataStructures;

namespace Everglow.SpellAndSkull.Items;

public class CrystalSkull : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override void SetStaticDefaults()
	{
		Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 32));
	}

	public override void SetDefaults()
	{
		Item.width = 30;
		Item.height = 32;
		Item.value = 11000;
		Item.accessory = true;
		Item.rare = ItemRarityID.LightRed;
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		player.GetModPlayer<GlobalItems.MagicBookPlayer>().MagicBookLevel = 1;
		base.UpdateAccessory(player, hideVisual);
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.Bone, 30)
			.AddIngredient(ItemID.Diamond, 3)
			.AddIngredient(ItemID.FallenStar, 5)
			.AddTile(TileID.Anvils)
			.Register();
	}
}

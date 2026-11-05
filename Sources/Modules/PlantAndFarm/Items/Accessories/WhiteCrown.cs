using Everglow.Commons.Mechanics.PlayerBonuses;
using Everglow.PlantAndFarm.Items.Materials;

namespace Everglow.PlantAndFarm.Items.Accessories;

public class WhiteCrown : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Flower of Hope");
		// DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "希望之花");
		// Tooltip.SetDefault("Increases running speed by (max Hp * 0.2%)\n'A plain white flower, seems to mourn for someone'");
		// Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "每50生命上限+10%移速\n'一枝素白的郁金香，像是在悼念某人'");
	}

	public override void SetDefaults()
	{
		Item.width = 36;
		Item.height = 38;
		Item.value = 3920;
		Item.accessory = true;
		Item.rare = ItemRarityID.Orange;
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		player.GetModPlayer<StatBonusPlayer>().WhiteCrown = true;
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ModContent.ItemType<WindMoveSeed>(), 15)
			.AddIngredient(ModContent.ItemType<WhiteTulip>(), 24)
			.AddTile(TileID.LivingLoom)
			.Register();
	}
}

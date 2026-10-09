using Everglow.PlantAndFarm.Items.Materials;

namespace Everglow.PlantAndFarm.Items.Accessories;

public class OrangeStick : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Pennisetum Arantius");
		// DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "坚橙狼尾");
		// Tooltip.SetDefault("After struck, grants 5 defence and immunity to knockback for 3s\n'Different from most grasses, it stands fast when wind blows fiercely'");
		// Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "受击后3秒获得5防御力和击退免疫\n'与大多数草不同,它在狂风中坚挺而不折腰'");
	}

	public override void SetDefaults()
	{
		Item.width = 10;
		Item.height = 26;
		Item.value = 2985;
		Item.accessory = true;
		Item.rare = 3;
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		// MythPlayer.OrangeStick = 2;
		// if (MythPlayer.OrangeStickCool > 0)
		// {
		player.statDefense += 5;
		player.noKnockback = true;
		// }
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ModContent.ItemType<WindMoveSeed>(), 15)
			.AddIngredient(ModContent.ItemType<OrangeSausage>(), 24)
			.AddTile(304)
			.Register();
	}
}

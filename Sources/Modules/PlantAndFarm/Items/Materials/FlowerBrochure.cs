namespace Everglow.PlantAndFarm.Items.Materials;

public class FlowerBrochure : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Materials;

	public override void SetDefaults()
	{
		Item.width = 24;
		Item.height = 30;
		Item.maxStack = 1;
		Item.value = 10000;
		Item.rare = ItemRarityID.Blue;
		Item.material = true;
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		player.GetModPlayer<PAFPlayer>().FlowerBrochure = !hideVisual;
	}
}

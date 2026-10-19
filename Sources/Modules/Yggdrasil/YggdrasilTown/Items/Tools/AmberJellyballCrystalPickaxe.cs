namespace Everglow.Yggdrasil.YggdrasilTown.Items.Tools;

public class AmberJellyballCrystalPickaxe : ModItem
{
	public override void SetDefaults()
	{
		Item.height = 46;
		Item.width = 42;
		Item.useTime = Item.useAnimation = 10;
		Item.pick = 75;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.UseSound = SoundID.Item1;
		Item.rare = ItemRarityID.Blue;
		Item.value = Item.buyPrice(0, 0, 0, 0);
		Item.autoReuse = true;
		Item.useTurn = true;
	}
}

using Everglow.Myth.TheFirefly.WorldGeneration;
using SubworldLibrary;

namespace Everglow.Myth.TheFirefly.Items;

public class FireflyImpression : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 20;
		Item.maxStack = 1;
		Item.useTurn = true;
		Item.autoReuse = true;
		Item.useAnimation = 15;
		Item.useTime = 7;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.consumable = false;
	}
	public override bool? UseItem(Player player)
	{
		if (player.itemAnimation == player.itemAnimationMax)
		{
			if (SubworldSystem.IsActive<MothWorld>())
			{
				SubworldSystem.Exit();
			}
			else
			{
				if (!SubworldSystem.Enter<MothWorld>())
				{
					Main.NewText("Fail!");
				}
			}
		}
		return base.UseItem(player);
	}
}

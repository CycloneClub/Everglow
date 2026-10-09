using Everglow.Myth.TheTusk.WorldGeneration;
using SubworldLibrary;

namespace Everglow.Myth.TheTusk.Items;

public class TuskMirror : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 30;
		Item.height = 30;
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
			if (SubworldSystem.IsActive<TuskWorld>())
				SubworldSystem.Exit();
			else
			{
				if (!SubworldSystem.Enter<TuskWorld>())
				{
					Main.NewText("Fail!");
				}
			}
		}
		return base.UseItem(player);
	}
}

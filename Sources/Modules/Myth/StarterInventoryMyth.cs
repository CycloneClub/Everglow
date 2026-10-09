using Everglow.Myth.OmniElementItems;
using static Steamworks.SteamUser;

namespace Everglow.Myth;

public class StarterInventoryMyth : ModPlayer
{
	public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath)
	{
		// if (mediumCoreDeath)
		// {
		//    return new[]
		//    {
		//        new Item(ItemID.HealingPotion)
		//    };
		// }
		if (Main.LocalPlayer.name.Equals("Omnielement", StringComparison.OrdinalIgnoreCase) || Main.LocalPlayer.name.Equals("万象元素", StringComparison.OrdinalIgnoreCase))
		{
			return new[]
			{
			new Item(ModContent.ItemType<LilyHarp>()),
			};
		}
		return new[]
		{
		new Item(ItemID.CopperCoin, 0), // stack of 0 means you don't start with it. This code is needed.
        };
	}
}

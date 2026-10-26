using Everglow.Myth.Misc.Items.Accessories;
using Everglow.Myth.Misc.Items.Weapons.Slingshots;
using Terraria.GameContent.ItemDropRules;

namespace Everglow.Myth.Common;

public class MythContentItemLoot : GlobalItem
{
	public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
	{
		if (item.type == ItemID.KingSlimeBossBag)
		{
			itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<GelSlingshot>(), 2));
		}
		if (item.type == ItemID.FishronBossBag)
		{
			itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<DukeTooth>(), 3));
		}
	}
}

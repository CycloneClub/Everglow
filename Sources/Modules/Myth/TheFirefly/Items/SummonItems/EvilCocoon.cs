using Everglow.Myth.TheFirefly.NPCs.Bosses;
using Terraria;

namespace Everglow.Myth.TheFirefly.Items.SummonItems;

public class EvilCocoon : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.SummonItems;

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 32;
		Item.useAnimation = 45;
		Item.useTime = 60;
		Item.useStyle = ItemUseStyleID.HoldUp;
		Item.maxStack = 999;
		Item.consumable = true;
	}

	public override void ModifyTooltips(List<TooltipLine> list)
	{
	}

	public override void Update(WorldItem item, ref float gravity, ref float maxFallSpeed)
	{
	}

	public override bool CanUseItem(Player player)
	{
		if (NPC.CountNPCS(ModContent.NPCType<CorruptMoth>()) < 1)
		{
			NPC.NewNPC(null, (int)Main.MouseWorld.X, (int)Main.MouseWorld.Y + 50, ModContent.NPCType<EvilPack>(), 0, 0f, 0f, 0f, 0f, 255);
			Item.stack--;
			return true;
		}
		return false;
	}
}

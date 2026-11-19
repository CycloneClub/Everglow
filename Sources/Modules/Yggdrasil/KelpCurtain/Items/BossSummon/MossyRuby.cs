using Everglow.Yggdrasil.KelpCurtain.NPCs.VampireMat;

namespace Everglow.Yggdrasil.KelpCurtain.Items.BossSummon;

public class MossyRuby : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.SummonItems;

	public override void SetDefaults()
	{
		Item.consumable = true;
		Item.width = 22;
		Item.height = 34;
		Item.value = 13000;
		Item.useTime = 12;
		Item.useAnimation = 12;
		Item.noMelee = true;
		Item.maxStack = Item.CommonMaxStack;
		Item.useTurn = true;
		Item.UseSound = SoundID.Roar;
		Item.useStyle = ItemUseStyleID.Swing;
	}

	public override bool? UseItem(Player player)
	{
		if (!CanUseItem(player))
		{
			return false;
		}
		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			var npc = NPC.NewNPCDirect(Item.GetNPCSource_FromThis(), player.Center + new Vector2(player.direction * 1800, 0), ModContent.NPCType<VampireMat>());
			npc.velocity.X = -player.direction * 12;
			npc.netUpdate = true;
		}
		return true;
	}

	public override bool CanUseItem(Player player)
	{
		return !NPC.AnyNPCs(ModContent.NPCType<VampireMat>());
	}
}

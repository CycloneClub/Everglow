using Everglow.Myth.TheFirefly.NPCs.Bosses;

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
		Item.UseSound = SoundID.Roar;
		Item.maxStack = 999;
		Item.consumable = true;
	}

	public override void ModifyTooltips(List<TooltipLine> list)
	{
	}

	public override void Update(ref float gravity, ref float maxFallSpeed)
	{
	}

	public override bool CanUseItem(Player player)
	{
		return !NPC.AnyNPCs(ModContent.NPCType<CorruptMoth>());
	}

	public override bool? UseItem(Player player)
	{
		if (!CanUseItem(player))
		{
			return false;
		}
		if (Main.netMode == NetmodeID.SinglePlayer)
		{
			NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<CorruptMoth>());
		}
		else if (Main.netMode == NetmodeID.MultiplayerClient && player.whoAmI == Main.myPlayer)
		{
			NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: ModContent.NPCType<CorruptMoth>());
		}
		return true;
	}
}

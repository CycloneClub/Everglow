using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.UI;

namespace Everglow.Commons.Mechanics.Quest.PlayerSide.Tests;

public class TestOpenQuestPanelWithSelectQuestItem : ModItem
{
	public override string Texture => ModAsset.Point_Mod;

	public override bool IsLoadingEnabled(Mod mod)
	{
#if DEBUG
		return true;
#else
		return false;
#endif
	}

	public override void SetDefaults()
	{
		Item.useStyle = ItemUseStyleID.Swing;
		Item.noUseGraphic = true;
	}

	public override bool? UseItem(Player player)
	{
		if (Main.dedServ || player.whoAmI != Main.myPlayer || !player.active)
		{
			return false;
		}

		// Snapshot this entry's Available quests before Actions activates objectives and raises events.
		var system = ModContent.GetInstance<PlayerQuestSystem>();
		var candidates = system.Manager.Quests
			.Where(quest => quest.State == PlayerQuestState.Available && quest.Source == QuestSourceBase.Default)
			.Select(quest => new QuestIdentity(QuestSide.Player, quest.Name, quest.InstanceId))
			.ToArray();
		foreach (QuestIdentity identity in candidates)
		{
			system.Actions.TryExecute(new QuestAction(identity, QuestActionType.Accept));
		}

		QuestContainer.Instance.ShowWithQuest(nameof(OpenPanelQuestTest));
		return true;
	}

	public override void ModifyTooltips(List<TooltipLine> tooltips)
	{
		tooltips.Add(new TooltipLine(Mod, "AcceptDefaultQuests", "Accept all available default-source DEBUG quests, then open OpenPanelQuestTest."));
	}
}

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

		QuestContainer.Instance.ShowWithQuest(nameof(OpenPanelQuestTest));
		return true;
	}
}

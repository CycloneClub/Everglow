using Everglow.Commons.Mechanics.Mission.UI;

namespace Everglow.Commons.Mechanics.Mission.PlayerSide.Tests;

public class TestOpenMissionPanelWithSelectMissionItem : ModItem
{
	public override string Texture => ModAsset.Point_Mod;

	public override void SetDefaults()
	{
		Item.useStyle = ItemUseStyleID.Swing;
		Item.noUseGraphic = true;
	}

	public override bool? UseItem(Player player)
	{
		MissionContainer.Instance.ShowWithMission(new OpenPanelMissionTest().Name);
		return true;
	}
}

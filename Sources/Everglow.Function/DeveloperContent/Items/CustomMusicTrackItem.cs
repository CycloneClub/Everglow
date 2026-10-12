using Everglow.Commons.DeveloperContent.VFXs;
using Everglow.Commons.Mechanics;

namespace Everglow.Commons.DeveloperContent.Items;

public class CustomMusicTrackItem : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Tools;

	public VisualizedMusicTrack Visual { get; private set; } = null;

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 20;
		Item.useTurn = true;
		Item.useAnimation = 4;
		Item.useTime = 4;
		Item.autoReuse = false;
		Item.useStyle = ItemUseStyleID.HoldUp;
	}

	public override bool CanUseItem(Player player)
	{
		MusicHelper.EnableSoundTrackVisualization = !MusicHelper.EnableSoundTrackVisualization;
		if (MusicHelper.EnableSoundTrackVisualization)
		{
			Main.NewText("Enable Sound Track Visualization.");
		}
		else
		{
			Main.NewText("Disable Sound Track Visualization.");
		}
		return true;
	}
}

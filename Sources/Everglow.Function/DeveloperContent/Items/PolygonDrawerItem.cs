using Everglow.Commons.DeveloperContent.VFXs;

namespace Everglow.Commons.DeveloperContent.Items;

/// <summary>
/// Create line-connected points in world, help developers to visualize polygon.
/// </summary>
public class PolygonDrawerItem : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Tools;

	public PolygonDrawerVisual Visual { get; private set; } = null;

	public override void SetDefaults()
	{
		Item.width = 16;
		Item.height = 16;
		Item.value = 0;
		Item.rare = ItemRarityID.White;
	}

	public override void HoldItem(Player player)
	{
		if (Visual is not null && Visual.Active)
		{
			return;
		}

		var helper = new PolygonDrawerVisual()
		{
			Owner = player,
		};

		Ins.VFXManager.Add(helper);

		// Save after adding for ensuring the helper visual can be added correctly instead of stucking this item in bug.
		Visual = helper;
	}
}

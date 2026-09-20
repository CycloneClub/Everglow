using Everglow.Commons.DeveloperContent.VFXs;

namespace Everglow.Commons.DeveloperContent.Items;

/// <summary>
/// Create line-connected points in world, help developers to visualize polygon.
/// </summary>
public class TileToolBox : ModItem
{
	public TileToolBoxInterface Visual { get; private set; } = null;

	public override void SetDefaults()
	{
		Item.width = 16;
		Item.height = 16;
		Item.value = 0;
		Item.rare = ItemRarityID.White;
	}

	public override void HoldItem(Player player)
	{
		if (Main.mouseLeft && Main.mouseLeftRelease)
		{
			if (Visual is not null && Visual.Active)
			{
				return;
			}
			var helper = new TileToolBoxInterface()
			{
				Owner = player,
			};
			Ins.VFXManager.Add(helper);
			Visual = helper;
		}
	}
}

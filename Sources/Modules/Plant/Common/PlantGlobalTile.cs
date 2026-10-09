using Everglow.Plant.Items.Weapons.Melee;

namespace Everglow.Plant.Common;

public class PlantGlobalTile : GlobalTile
{
	public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		if (type == 484)
		{
			if (PlantModSystem.RollingCactusHitCount < 100)
			{
				PlantModSystem.RollingCactusHitCount++;
				if (PlantModSystem.RollingCactusHitCount == 100)
				{
					Item.NewItem(Entity.GetSource_None(), new Vector2(i, j) * 16f, ModContent.ItemType<CactusBall>());
				}
			}
		}
	}
}

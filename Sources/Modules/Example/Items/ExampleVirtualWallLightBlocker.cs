using Everglow.Commons.Mechanics.EliminateLight;

namespace Everglow.Example.Items;

public class ExampleVirtualWallLightBlocker : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Tools;

	public override void HoldItem(Player player)
	{
		int range = 48;
		if (Main.mouseLeft)
		{
			List<Vector2> polygon = new List<Vector2>();
			for (int i = 0; i < 10; i++)
			{
				float mulRange = 1f;
				if (i % 2 == 0)
				{
					mulRange = 0.5f;
				}
				polygon.Add(Main.MouseWorld + new Vector2(0, -range * 16 * mulRange).RotatedBy(i / 10f * MathHelper.TwoPi + Main.GlobalTimeWrappedHourly));
			}
			EliminateLightManager.AddPolygon(polygon);
		}
	}
}

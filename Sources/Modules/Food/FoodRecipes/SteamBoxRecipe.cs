using Everglow.Food.Items.Ingredients;
using Everglow.Food.Items.ModFood;

namespace Everglow.Food.FoodRecipes;

public class SteamBoxRecipe : FoodRecipes
{
	public SteamBoxRecipe()
	{
		CookingUnitWithOrderMenu = new List<CookingUnitWithOrder>
		{
			// 小笼包
			{
				new CookingUnitWithOrder(
					ModContent.ItemType<XiaoLongBao>(),
					1,
					[ModContent.ItemType<RawXiaoLongBao>()],
					[ModContent.ItemType<RawXiaoLongBao>()],
					[ModContent.ItemType<RawXiaoLongBao>()],
					[ModContent.ItemType<RawXiaoLongBao>()],
					[ModContent.ItemType<RawXiaoLongBao>()],
					[ModContent.ItemType<RawXiaoLongBao>()],
					[ItemID.BottledWater])
			},
		};

		CookingUnitMenu = new List<CookingUnit>
		{
		};
	}
}

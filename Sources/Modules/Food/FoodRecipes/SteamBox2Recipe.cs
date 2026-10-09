using Everglow.Food.Items.Ingredients;
using Everglow.Food.Items.ModFood;

namespace Everglow.Food.FoodRecipes;

public class SteamBox2Recipe : FoodRecipes
{
	public SteamBox2Recipe()
	{
		CookingUnitWithOrderMenu = new List<CookingUnitWithOrder>
		{
			// 小笼包*2
			{
				new CookingUnitWithOrder(
					ModContent.ItemType<XiaoLongBao>(),
					2,
					[ModContent.ItemType<RawXiaoLongBao>()],
					[ModContent.ItemType<RawXiaoLongBao>()],
					[ModContent.ItemType<RawXiaoLongBao>()],
					[ModContent.ItemType<RawXiaoLongBao>()],
					[ModContent.ItemType<RawXiaoLongBao>()],
					[ModContent.ItemType<RawXiaoLongBao>()],
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

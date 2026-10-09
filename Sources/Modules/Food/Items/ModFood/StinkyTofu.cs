using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Everglow.Food.Buffs.ModFoodBuffs;
using Everglow.Food.FoodUtilities;

namespace Everglow.Food.Items.ModFood;

public class StinkyTofu : FoodBase
{
	public override FoodInfo FoodInfo
	{
		get
		{
			return new FoodInfo()
			{
				Satiety = 15,
				BuffType = ModContent.BuffType<StinkyTofuBuff>(),
				BuffTime = new FoodDuration(5, 0, 0),
				Name = "StinkyTofuBuff",
			};
		}
	}

	public override void SetStaticDefaults()
	{
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;

		Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));

		ItemID.Sets.FoodParticleColors[Item.type] = new Color[3]
		{
			new Color(51, 38, 61),
			new Color(130, 24, 29),
			new Color(8, 6, 10),
		};

		ItemID.Sets.IsFood[Type] = true;
	}

	public override void SetDefaults()
	{
		Item.DefaultToFood(22, 22, BuffID.WellFed3, 57600);
		Item.value = Item.buyPrice(0, 3);
		Item.rare = ItemRarityID.Blue;
	}
}

using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Everglow.Food.Buffs.ModFoodBuffs;
using Everglow.Food.FoodUtilities;

namespace Everglow.Food.Items.ModFood;

public class Watermelon : FoodBase
{
	public override FoodInfo FoodInfo
	{
		get
		{
			return new FoodInfo()
			{
				Satiety = 20,
				BuffType = ModContent.BuffType<WatermelonBuff>(),
				BuffTime = new FoodDuration(8, 0, 0),
				Name = "WatermelonBuff",
			};
		}
	}
	public override void SetStaticDefaults()
	{
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;

		Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));

		ItemID.Sets.FoodParticleColors[Item.type] = new Color[3] {
			new Color(108, 150, 12),
			new Color(46, 84, 0),
			new Color(255, 94, 81),
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

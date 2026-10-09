using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Everglow.Food.Buffs.ModFoodBuffs;
using Everglow.Food.FoodUtilities;

namespace Everglow.Food.Items.ModFood;

public class KiwiIceCream : FoodBase
{
	public override FoodInfo FoodInfo
	{
		get
		{
			return new FoodInfo()
			{
				Satiety = 10,
				BuffType = ModContent.BuffType<KiwiIceCreamBuff>(),
				BuffTime = new FoodDuration(4, 0, 0),
				Name = "KiwiIceCreamBuff",
			};
		}
	}
	public override void SetStaticDefaults()
	{
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;

		Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));

		ItemID.Sets.FoodParticleColors[Item.type] = new Color[3]
		{
			new Color(86, 120, 19),
			new Color(206, 139, 162),
			new Color(165, 158, 152),
		};

		ItemID.Sets.IsFood[Type] = true;
	}

	public override void SetDefaults()
	{
		Item.DefaultToFood(22, 22, BuffID.WellFed3, 57600);
		Item.value = Item.buyPrice(0, 3);
		Item.rare = ItemRarityID.Blue;
	}


	public override bool ConsumeItem(Player player)
	{
		return true;
	}
}

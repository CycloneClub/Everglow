using Everglow.Food.Buffs.ModDrinkBuffs;
using Everglow.Food.FoodUtilities;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;

namespace Everglow.Food.Items.ModDrink;

public class Sunrise : DrinkBase
{
	public override DrinkInfo DrinkInfo
	{
		get
		{
			return new DrinkInfo()
			{
				Thirsty = false,
				BuffType = ModContent.BuffType<SunriseBuff>(),
				BuffTime = new FoodDuration(0, 10, 0),
				Name = "SunriseBuff",
			};
		}
	}

	public override void SetStaticDefaults()
	{
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;
		Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));

		ItemID.Sets.FoodParticleColors[Item.type] = new Color[3]
		{
			new Color(255, 180, 7),
			new Color(255, 131, 48),
			new Color(255, 38, 30),
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

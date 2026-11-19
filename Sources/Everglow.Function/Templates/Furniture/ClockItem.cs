using Terraria.GameContent.Creative;

namespace Everglow.Commons.Templates.Furniture;

/// <summary>
/// 钟物品模板
/// </summary>
public abstract class ClockItem : ModItem
{
	public override string LocalizationCategory => Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetStaticDefaults()
	{
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
	}

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 20;
		Item.value = 300;
		Item.maxStack = Item.CommonMaxStack;
		Item.useAnimation = 14;
		Item.UseSound = SoundID.Dig;
	}

	public static float GetHourHandRotation()
	{
		double timeInSecond = Main.time + 16200;
		if (!Main.dayTime)
		{
			timeInSecond = Main.time + 70200;
			if (timeInSecond > 86400)
			{
				timeInSecond -= 86400;
			}
		}
		return (float)(timeInSecond / 43200 * MathHelper.TwoPi);
	}

	public static float GetMinuteHandRotation()
	{
		double timeInSecond = Main.time + 16200;
		if (!Main.dayTime)
		{
			timeInSecond = Main.time + 70200;
			if (timeInSecond > 86400)
			{
				timeInSecond -= 86400;
			}
		}
		return (float)(timeInSecond / 3600 * MathHelper.TwoPi);
	}

	public static float GetSecondHandRotation()
	{
		double timeInSecond = Main.time + 16200;
		if (!Main.dayTime)
		{
			timeInSecond = Main.time + 70200;
			if (timeInSecond > 86400)
			{
				timeInSecond -= 86400;
			}
		}
		return (float)(timeInSecond / 60 * MathHelper.TwoPi);
	}
}

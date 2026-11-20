using Terraria.GameContent.Creative;

namespace Everglow.Commons.Templates.Furniture;

/// <summary>
/// 吊灯物品模板
/// </summary>
public abstract class ChandelierItem : ModItem
{
	public override string LocalizationCategory => Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetStaticDefaults()
	{
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
	}

	public override void SetDefaults()
	{
		Item.width = 26;
		Item.height = 26;
		Item.value = 3000;
		Item.maxStack = Item.CommonMaxStack;
		Item.useAnimation = 14;
		Item.UseSound = SoundID.Dig;
	}
}

namespace Everglow.Myth.TheFirefly.Items;

public class GlowingFirefly : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Fishing;

	public override void SetStaticDefaults()
	{
	}

	public override void SetDefaults()
	{
		Item.width = 32;
		Item.height = 22;
		Item.maxStack = 999;
		Item.bait = 42;
	}
}

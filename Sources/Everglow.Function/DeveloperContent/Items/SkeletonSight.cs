namespace Everglow.Commons.DeveloperContent.Items;

internal class SkeletonSight : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Miscs;

	public override void SetDefaults()
	{
		Item.width = 10;
		Item.height = 10;
	}
}

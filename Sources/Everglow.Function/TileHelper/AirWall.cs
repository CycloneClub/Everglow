namespace Everglow.Commons.TileHelper;

public class AirWall : ModWall
{
	public override void SetStaticDefaults()
	{
		AddMapEntry(Color.White);
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		return false;
	}
}

public class AirWallItem : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableWall(ModContent.WallType<AirWall>());
		Item.consumable = false;
		Item.maxStack = 1;
	}
}

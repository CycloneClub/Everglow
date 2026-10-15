
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
		base.PreDraw(i, j, spriteBatch);
	}
}

public class AirWallItem : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.useTime = 5;
		Item.useAnimation = 5;
		Item.createWall = ModContent.WallType<AirWall>();
		Item.useStyle = ItemUseStyleID.Swing;
		Item.autoReuse = true;
	}
}

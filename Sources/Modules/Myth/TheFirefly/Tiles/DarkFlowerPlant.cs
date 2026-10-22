using Everglow.Commons.Utilities;
using Terraria.ObjectData;

namespace Everglow.Myth.TheFirefly.Tiles;

public class DarkFlowerPlant : ModTile
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Placeables;

	public override string Texture => Commons.ModAsset.White_Mod;

	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileCut[Type] = true;
		Main.tileNoAttach[Type] = true;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.CoordinateHeights = new[] { 16 };
		TileObjectData.newTile.AnchorValidTiles = new[] { ModContent.TileType<DarkCocoon>() };
		TileObjectData.addTile(Type);
		RegisterItemDrop(ModContent.ItemType<Misc.Items.Weapons.DarkFlower>());
		AddMapEntry(new Color(100, 80, 160), ModContent.GetInstance<Misc.Items.Weapons.DarkFlower>().DisplayName);
		HitSound = SoundID.Grass;
		DustType = DustID.SpookyWood;
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		if (!Main.dedServ)
		{
			Vector2 offset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
			Vector2 position = new Vector2(i * 16, j * 16) - Main.screenPosition + offset;
			spriteBatch.Draw(Commons.ModAsset.White.Value, new Rectangle((int)position.X + 4, (int)position.Y, 8, 16), Lighting.GetColor(i, j));
		}
		return false;
	}
}

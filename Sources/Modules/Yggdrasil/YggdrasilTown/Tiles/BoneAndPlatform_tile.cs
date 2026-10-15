using Everglow.Commons.VFX.Scene;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

public class BoneAndPlatform_tile : ModTile, ISceneTile
{
	public override void SetStaticDefaults()
	{
		// Properties
		Main.tileLighted[Type] = true;
		Main.tileFrameImportant[Type] = true;
		Main.tileSolidTop[Type] = true;
		Main.tileSolid[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileTable[Type] = true;
		Main.tileLavaDeath[Type] = true;
		TileID.Sets.Platforms[Type] = true;
		TileID.Sets.DisableSmartCursor[Type] = true;

		AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);
		AddMapEntry(new Color(0, 14, 175));

		DustType = DustID.DynastyWood;
		AdjTiles = new int[] { TileID.Platforms };

		// Placement
		TileObjectData.newTile.CoordinateHeights = new[] { 16 };
		TileObjectData.newTile.CoordinateWidth = 16;
		TileObjectData.newTile.CoordinatePadding = 2;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.StyleMultiplier = 27;
		TileObjectData.newTile.StyleWrapLimit = 27;
		TileObjectData.newTile.UsesCustomCanPlace = false;
		TileObjectData.newTile.LavaDeath = true;
		TileObjectData.addTile(Type);

		LocalizedText name = CreateMapEntryName();
		AddMapEntry(new Color(69, 36, 78), name);
	}

	public void AddScene(int i, int j)
	{
		Tile tile = Main.tile[i, j];
		if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
		{
			BoneAndPlatform_background scene = new BoneAndPlatform_background { Position = new Vector2(i, j - 14) * 16, Active = true, OriginTilePos = new Point(i, j), OriginTileType = ModContent.TileType<BoneAndPlatform_tile>() };
			Ins.VFXManager.Add(scene);
			BoneAndPlatform_foreground scene2 = new BoneAndPlatform_foreground { Position = new Vector2(i, j - 14) * 16, Active = true, OriginTilePos = new Point(i, j), OriginTileType = ModContent.TileType<BoneAndPlatform_tile>() };
			Ins.VFXManager.Add(scene2);
		}
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		return false;
	}
}

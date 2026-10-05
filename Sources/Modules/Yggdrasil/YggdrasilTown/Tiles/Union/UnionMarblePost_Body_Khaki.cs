using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.Union;

public class UnionMarblePost_Body_Khaki : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileLavaDeath[Type] = false;
		Main.tileNoAttach[Type] = false;
		Main.tileWaterDeath[Type] = false;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
		TileObjectData.newTile.Height = 1;
		TileObjectData.newTile.Width = 3;

		TileObjectData.newTile.AnchorAlternateTiles = new[] { Type, ModContent.TileType<UnionMarblePost_Bottom_Khaki>() | ModContent.TileType<UnionMarblePost_Body_Khaki>() | ModContent.TileType<UnionMarblePost_Top_Khaki>() };

		TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.None, 0, 0);
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
		TileObjectData.newTile.CoordinateHeights = new[] { 16 };
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.LavaDeath = false;
		TileObjectData.newTile.Origin = new Point16(1, 0);

		// TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
		// TileObjectData.newAlternate.AnchorBottom = new AnchorData(AnchorType.AlternateTile, TileObjectData.newTile.Width, 0);
		// TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.None, 0, 0);
		// TileObjectData.addAlternate(0);

		// TileObjectData.newTile.AnchorAlternateTiles = new[] { ModContent.TileType<UnionMarblePost_Bottom_Khaki>() | ModContent.TileType<UnionMarblePost_Body_Khaki>() | ModContent.TileType<UnionMarblePost_Top_Khaki>() };
		// TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.AlternateTile, TileObjectData.newTile.Width, 0);
		// TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
		TileObjectData.addTile(Type);
		DustType = ModContent.DustType<UnionMarblePost_Dust_Khaki>();
		AddMapEntry(new Color(226, 202, 181));
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		Color lightColor = Lighting.GetColor(i, j);
		Tile tile = Main.tile[i, j];
		var zero = new Vector2(Main.offScreenRange, Main.offScreenRange);

		if (Main.drawToScreen)
		{
			zero = Vector2.Zero;
		}
		Rectangle drawFrame = new Rectangle(54, 0, 2, 16);
		if (tile.TileFrameX == 0)
		{
			spriteBatch.Draw(ModAsset.UnionMarblePost_Body_Khaki.Value, new Vector2(i, j) * 16 - Main.screenPosition + zero + new Vector2(-2, 0), drawFrame, lightColor, 0, Vector2.zeroVector, 1, SpriteEffects.None, 0);
		}
		if (tile.TileFrameX == 36)
		{
			spriteBatch.Draw(ModAsset.UnionMarblePost_Body_Khaki.Value, new Vector2(i, j) * 16 - Main.screenPosition + zero + new Vector2(16, 0), drawFrame, lightColor, 0, Vector2.zeroVector, 1, SpriteEffects.None, 0);
		}
	}
}

using Everglow.Commons.TileHelper;
using Everglow.Yggdrasil.Common.Elevator.Tiles;
using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.GravelStore;

public class BakeryVase : ModTile, ITileFluentlyDrawn
{
	public override void SetStaticDefaults()
	{
		Main.tileTable[Type] = true;
		Main.tileFrameImportant[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;
		TileID.Sets.HasOutlines[Type] = true;

		DustType = ModContent.DustType<LampWood_Dust>();
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidWithTop | AnchorType.Table, 1, 0);
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.addTile(Type);

		AddMapEntry(new Color(69, 36, 78));
	}

	public override void NumDust(int i, int j, bool fail, ref int num)
	{
		num = 0;
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		TileFluentDrawManager.AddFluentPoint(this, i, j);
		return base.PreDraw(i, j, spriteBatch);
	}

	public void FluentDraw(Vector2 screenPosition, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		Tile tile = Main.tile[pos];
		var tileData = TileObjectData.GetTileData(tile.type, 0);
		if (!TileDrawing.IsVisible(tile) || tileData is null)
		{
			return;
		}

		if (tile.TileFrameY == 0)
		{
			Vector2 offset = new Vector2(-2, -4);
			Rectangle frame = new Rectangle(0, 16, 14, 20);
			if (tile.TileFrameX == 18)
			{
				frame = new Rectangle(16, 16, 14, 20);
				offset = new Vector2(2, -4);
			}
			if (tile.TileFrameX == 36)
			{
				frame = new Rectangle(32, 22, 6, 14);
				offset = new Vector2(-2, -4);
			}
			Texture2D tex = tileDrawing.GetTileDrawTexture(tile, pos.X, pos.Y);

			float windCycle = 0;
			if (tileDrawing.InAPlaceWithWind(pos.X, pos.Y, 1, 1))
			{
				windCycle = tileDrawing.GetWindCycle(pos.X, pos.Y, tileDrawing._sunflowerWindCounter);
			}

			// Wind push rotation
			int totalPushTime = 30;
			float pushForcePerFrame = 1.26f;
			float highestWindGridPushComplex = tileDrawing.GetHighestWindGridPushComplex(pos.X, pos.Y, 1, 1, totalPushTime, pushForcePerFrame, 3, swapLoopDir: true);
			windCycle += highestWindGridPushComplex;
			float rotation = windCycle * 0.4f;

			spriteBatch.Draw(tex, pos.ToWorldCoordinates() - Main.screenPosition + offset, frame, Lighting.GetColor(pos), rotation, new Vector2(frame.Width * 0.5f, frame.Height), 1, SpriteEffects.None, 0);
		}
	}
}

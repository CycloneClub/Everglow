using System;
using Everglow.Commons.TileHelper;
using Everglow.Myth.TheFirefly.Dusts;
using ReLogic.Content;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Everglow.Myth.TheFirefly.Tiles.Furnitures;

public class GlowWoodChandelier : ModTile, ITileFluentlyDrawn, ITileFlameData
{
	private Asset<Texture2D> flameTexture;

	public override void SetStaticDefaults()
	{
		Main.tileFlame[Type] = true;
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileSolid[Type] = false;
		Main.tileNoFail[Type] = true;
		TileID.Sets.HasOutlines[Type] = true;

		TileID.Sets.RoomNeeds.CountsAsTorch[Type] = true;

		DustType = ModContent.DustType<BlueGlow>();
		AdjTiles = new int[] { TileID.Chandeliers };

		// Placement - Standard Chandelier Setup Below
		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
		TileObjectData.newTile.Origin = new Point16(1, 0);
		TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 1, 1);
		TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
		TileObjectData.newTile.LavaDeath = true;
		TileObjectData.newTile.StyleWrapLimit = 37;
		TileObjectData.newTile.StyleHorizontal = false;
		TileObjectData.newTile.StyleLineSkip = 2;
		TileObjectData.newTile.DrawYOffset = -2;
		TileObjectData.addTile(Type);

		if (!Main.dedServ)
		{
			flameTexture = ModContent.Request<Texture2D>("Everglow/Myth/TheFirefly/Tiles/Furnitures/GlowWoodChandelier_Flame");
		}

		LocalizedText name = CreateMapEntryName();
		AddMapEntry(new Color(69, 36, 78), name);
	}

	public override void NumDust(int i, int j, bool fail, ref int num)
	{
		num = 0;
	}

	public override void HitWire(int i, int j)
	{
		FurnitureUtils.LightHitwire(i, j, Type, 3, 3);
	}

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		var tile = Main.tile[i, j];
		if (tile.TileFrameX < 54)
		{
			r = 0.1f;
			g = 0.9f;
			b = 1f;
		}
		else
		{
			r = 0f;
			g = 0f;
			b = 0f;
		}
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		TileFluentDrawManager.AddFluentPoint(this, i, j);
		return false;
	}

	public void FluentDraw(Vector2 screenPosition, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		FurnitureUtils.Chandelier3x3FluentDraw(screenPosition, pos, spriteBatch, tileDrawing);
	}

	public TileDrawing.TileFlameData GetTileFlameData(int tileX, int tileY, int type, int tileFrameY) =>
		new TileDrawing.TileFlameData()
		{
			flameCount = 7,
			flameTexture = flameTexture.Value,
			flameRangeXMin = -10,
			flameRangeXMax = 11,
			flameRangeMultX = 0.15f,
			flameRangeYMin = -10,
			flameRangeYMax = 1,
			flameRangeMultY = 0.35f,
			flameColor = new Color(30, 30, 30, 0),
		};
}

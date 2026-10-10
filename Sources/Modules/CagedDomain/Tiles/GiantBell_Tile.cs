using Everglow.Commons.TileHelper;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.ObjectData;

namespace Everglow.CagedDomain.Tiles;

public class GiantBell_Tile : ModTile, ITileFluentlyDrawn
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileSolid[Type] = false;
		Main.tileNoFail[Type] = true;

		TileID.Sets.RoomNeeds.CountsAsTorch[Type] = true;

		DustType = DustID.Gold;

		// Placement
		HitSound = new SoundStyle("Everglow/CagedDomain/Sounds/GiantBell");
		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
		TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
		TileObjectData.newTile.AnchorBottom = default;
		TileObjectData.newTile.Height = 7;
		TileObjectData.newTile.Width = 5;
		TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16, 16, 16, 16, 16 };
		TileObjectData.newTile.Origin = new Point16(1, 0);
		TileObjectData.addTile(Type);

		AddMapEntry(new Color(225, 195, 78));
	}

	public override bool RightClick(int i, int j)
	{
		SoundEngine.PlaySound(HitSound, new Vector2(i, j) * 16);
		return base.RightClick(i, j);
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var tile = Main.tile[i, j];
		if (tile.TileFrameX == 36 && tile.TileFrameY == 0)
		{
			TileFluentDrawManager.AddFluentPoint(this, i, j);
		}
		return false;
	}

	public override void PlaceInWorld(int i, int j, Item item)
	{
		SoundEngine.PlaySound(HitSound, new Vector2(i, j) * 16);
		base.PlaceInWorld(i, j, item);
	}

	public void FluentDraw(Vector2 screenPosition, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		var drawCenterPos = pos.ToWorldCoordinates(autoAddY: 0) - screenPosition;
		DrawGiantBellPiece(0.01f, 0, pos + new Point(-1, 0), pos + new Point(-1, 1), drawCenterPos, spriteBatch, tileDrawing);
	}

	/// <summary>
	/// 绘制撞钟
	/// </summary>
	/// <param name="swayCoefficient">撞钟的摇摆系数</param>
	/// <param name="offsetX">绘制偏移</param>
	/// <param name="tilePos">用于进行摇晃和风速判定的物块的坐标</param>
	/// <param name="paintPos">用于应用漆的物块的坐标</param>
	/// <param name="drawCenterPos">绘制中心的坐标</param>
	/// <param name="spriteBatch">合批绘制</param>
	/// <param name="tileDrawing">原版TileDrawing类的实例，有很多好用的方法</param>
	private void DrawGiantBellPiece(float swayCoefficient, int offsetX, Point tilePos, Point paintPos, Vector2 drawCenterPos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		// 回声涂料
		if (!TileDrawing.IsVisible(Main.tile[paintPos]))
		{
			return;
		}

		var tile = Main.tile[tilePos];
		ushort type = tile.TileType;
		int paint = Main.tile[paintPos].TileColor;
		Texture2D tex = PaintedTextureSystem.TryGetPaintedTexture(ModAsset.Tiles_GiantBell_Path, type, 0, paint, tileDrawing);
		tex ??= ModAsset.Tiles_GiantBell.Value;
		var frame = new Rectangle(0, 0, 80, 106);

		int sizeX = 3;
		int sizeY = 3;

		float windCycle = 0;
		if (tileDrawing.InAPlaceWithWind(tilePos.X, tilePos.Y, sizeX, sizeY))
		{
			windCycle = tileDrawing.GetWindCycle(tilePos.X, tilePos.Y, tileDrawing._sunflowerWindCounter);
		}

		int totalPushTime = 80;
		float pushForcePerFrame = 1.26f;
		float highestWindGridPushComplex = tileDrawing.GetHighestWindGridPushComplex(tilePos.X, tilePos.Y, sizeX, sizeY, totalPushTime, pushForcePerFrame, 3, swapLoopDir: true);
		windCycle += highestWindGridPushComplex;

		// 支持发光涂料
		Color tileLight = Lighting.GetColor(tilePos);
		tileDrawing.DrawAnimatedTile_AdjustForVisionChangers(tilePos.X, tilePos.Y, tile, type, 0, 0, ref tileLight, tileDrawing._rand.NextBool(4));
		tileLight = tileDrawing.DrawTiles_GetLightOverride(tilePos.Y, tilePos.X, tile, type, 0, 0, tileLight);

		float rotation = -windCycle * swayCoefficient;
		var origin = new Vector2(40, 0);
		var tileSpriteEffect = SpriteEffects.None;
		spriteBatch.Draw(tex, drawCenterPos + new Vector2(offsetX, -2), frame, tileLight, rotation, origin, 1f, tileSpriteEffect, 0f);
	}
}

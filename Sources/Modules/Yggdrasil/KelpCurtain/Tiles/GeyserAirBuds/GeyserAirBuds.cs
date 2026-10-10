using Everglow.Commons.TileHelper;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.KelpCurtain.Items.Placeables;
using Everglow.Yggdrasil.WorldGeneration;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.GeyserAirBuds;

public class GeyserAirBuds : ModTile, ITileFluentlyDrawn // 继承ITileFluentlyDrawn实现可旋转动画绘制
{
	public override void SetStaticDefaults()
	{
		MinPick = 150;
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileCut[Type] = false;
		RegisterItemDrop(ModContent.ItemType<GeyserAirBudsItem>(), 1);

		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.UsesCustomCanPlace = true;
		TileObjectData.newTile.CoordinateHeights = [16, 18];
		TileObjectData.newTile.LavaDeath = false;
		TileObjectData.newTile.Origin = new Point16(0, 1);
		TileObjectData.newSubTile.AnchorBottom = new(Terraria.Enums.AnchorType.SolidTile, 3, 0);

		// 这个和 PlaceInWorld重复了，所以有两个tileEntity
		// TileObjectData.newTile.HookPostPlaceMyPlayer = ModContent.GetInstance<GeyserAirBudsEntity>().Generic_HookPostPlaceMyPlayer;
		TileObjectData.addTile(Type);
		DustType = ModContent.DustType<GeyserBudDust_Blue>();
		AnimationFrameHeight = 96;
		AddMapEntry(new Color(89, 177, 255));
	}

	public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
	{
		return base.TileFrame(i, j, ref resetFrame, ref noBreak);
	}

	public override bool CreateDust(int i, int j, ref int type)
	{
		return true;
	}

	public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		Tile tile = Main.tile[i, j];
		if (tile.TileFrameX == 0 && tile.TileFrameY == 0 && !tile.HasTile)
		{
			Vector2 center = new Point(i, j).ToWorldCoordinates() + new Vector2(8);
			for (int k = 0; k < 60; k++)
			{
				Vector2 pos = new Vector2(0, MathF.Sqrt(Main.rand.NextFloat()) * 100);
				if (pos.Y > 50)
				{
					pos.Y = Main.rand.NextFloat(-50, 50);
				}
				pos -= new Vector2(4);
				int type = ModContent.DustType<GeyserBudDust_Blue>();
				if (Main.rand.NextBool(8))
				{
					type = ModContent.DustType<GeyserBudDust_Red>();
				}
				Dust.NewDust(pos + center, 0, 0, type);
			}
		}
	}

	public override void PlaceInWorld(int i, int j, Item item)
	{
		Tile tile = Main.tile[i, j];
		if (tile.TileFrameX == 0 && tile.TileFrameY == 18)
		{
			ModTileEntity.PlaceEntityNet(i + 1, j, ModContent.TileEntityType<GeyserAirBudsEntity>());
			TileEntity.ByPosition.TryGetValue(new Point16(i + 1, j), out TileEntity entity);
			if (entity is GeyserAirBudsEntity geyser)
			{
				geyser.StartFrame = 6;
			}
		}
		base.PlaceInWorld(i, j, item);
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		Tile tile = Main.tile[i, j];

		int relX = tile.TileFrameX / 18;
		int relY = tile.TileFrameY / 18;

		if (tile.TileFrameY == 18 && tile.TileFrameX == 0)
		{
			TileFluentDrawManager.AddFluentPoint(this, i, j);
		}
		return false;
	}

	public void ResetAsCoolingGeyserAirBudsPlatform(int i, int j)
	{
		for (int x = 0; x < 2; x++)
		{
			for (int y = 0; y < 2; y++)
			{
				Tile tile = TileUtils.SafeGetTile(i + x, j + y);
				if (tile.TileType == Type)
				{
					tile.TileType = (ushort)ModContent.TileType<GeyserAirBudsPlatform>();
					tile.TileFrameX = (short)(x * 18);
					tile.TileFrameY = (short)(y * 18);
					tile.HasTile = true;
				}
			}
		}
	}

	public void FluentDraw(Vector2 screenPosition, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		var tile = Main.tile[pos];
		var drawCenterPos = pos.ToWorldCoordinates(autoAddY: 16) - screenPosition;

		ushort type = tile.TileType;
		if (type != Type)
		{
			return;
		}

		// 回声涂料
		if (!TileDrawing.IsVisible(tile))
		{
			return;
		}

		int paint = Main.tile[pos].TileColor;
		Texture2D tex = PaintedTextureSystem.TryGetPaintedTexture(ModAsset.GeyserAirBuds_Untrimmed_Path, type, 0, paint, tileDrawing);
		tex ??= ModAsset.GeyserAirBuds_Untrimmed.Value;

		float windCycle = 0;
		if (tileDrawing.InAPlaceWithWind(pos.X, pos.Y - 1, 2, 2))
		{
			windCycle = tileDrawing.GetWindCycle(pos.X, pos.Y - 1, tileDrawing._sunflowerWindCounter);
		}
		float swayStrength = 0.05f;
		int totalPushTime = 80;
		float pushForcePerFrame = 1.26f;
		float highestWindGridPushComplex = tileDrawing.GetHighestWindGridPushComplex(pos.X, pos.Y - 1, 2, 2, totalPushTime, pushForcePerFrame, 3, swapLoopDir: true);
		windCycle += highestWindGridPushComplex;
		float rotation = windCycle * swayStrength;

		// 颜色
		Color tileLight = Lighting.GetColor(pos);
		tileDrawing.DrawAnimatedTile_AdjustForVisionChangers(pos.X, pos.Y, tile, type, 0, 0, ref tileLight, tileDrawing._rand.NextBool(4));
		tileLight = tileDrawing.DrawTiles_GetLightOverride(pos.Y, pos.X, tile, type, 0, 0, tileLight);

		var origin = new Vector2(48, 96);
		var tileSpriteEffect = SpriteEffects.None;

		int frameYOffset = 0;
		float frameValue = 0;
		if (TileEntity.ByPosition.TryGetValue(new Point16(pos.X + 1, pos.Y), out TileEntity entity) && entity is GeyserAirBudsEntity geyser)
		{
			frameYOffset = geyser.GetFrame() * AnimationFrameHeight; // 获取状态帧
			frameValue = (geyser.GetFrame() - 6) / 11f;
		}
		Rectangle frame = new Rectangle(0, frameYOffset, 96, AnimationFrameHeight);
		spriteBatch.Draw(tex, drawCenterPos + new Vector2(8, 6), frame, tileLight, rotation, origin, 1f, tileSpriteEffect, 0f);
		float lerpValue = MathF.Pow(frameValue, 0.25f);
		Vector3 lightColor = Vector3.Lerp(new Vector3(0.21f, 0.15f, 0.52f), new Vector3(0.21f, 0.45f, 0.32f), lerpValue);
		Lighting.AddLight(pos.ToWorldCoordinates() + new Vector2(8, 6), lightColor);
		Lighting.AddLight(pos.ToWorldCoordinates() + new Vector2(-8, 6), lightColor);
	}

	public override void KillMultiTile(int i, int j, int frameX, int frameY)
	{
		ModContent.GetInstance<GeyserAirBudsEntity>().Kill(i, j);
		Item.NewItem(new EntitySource_TileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 16), ModContent.ItemType<GeyserAirBudsItem>());
	}
}

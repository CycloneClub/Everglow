using Everglow.Commons.TileHelper;
using Everglow.Commons.VFX.Scene;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.KelpCurtain.Items.Placeables;
using Everglow.Yggdrasil.WorldGeneration;
using Terraria.DataStructures;
using Terraria.ObjectData;
using static Everglow.Yggdrasil.WorldGeneration.YggdrasilWorldGeneration;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake;

public class GiantGhostClawBarnacleCollideTile : ShapeDataTile, ISceneTile
{
	public static int[,] PixelHideForeground;

	public override void Load()
	{
		base.Load();
		var imageData = ImageReader.Read<SixLabors.ImageSharp.PixelFormats.Rgb24>(ShapePath);
		imageData.ProcessPixelRows(accessor =>
		{
			PixelHideForeground = new int[accessor.Width, accessor.Height];
			TotalWidth = accessor.Width;
			TotalHeight = accessor.Height;
			for (int y = 0; y < accessor.Height; y++)
			{
				var pixelRow = accessor.GetRowSpan(y);
				for (int x = 0; x < pixelRow.Length; x++)
				{
					ref var pixel = ref pixelRow[x];
					PixelHideForeground[x, y] = pixel.B;
				}
			}
		});
	}

	public override void SetStaticDefaults()
	{
		CustomItemType = ModContent.ItemType<GiantGhostClawBarnacle_Item>();
		DustType = ModContent.DustType<SharpBarnacle_Dust>();
		TotalWidth = 19;
		TotalHeight = 23;
		MinPick = int.MaxValue;

		Main.tileFrameImportant[Type] = true;
		Main.tileLavaDeath[Type] = false;
		Main.tileSolid[Type] = true;
		Main.tileWaterDeath[Type] = false;
		Main.tileBlendAll[Type] = false;
		Main.tileBlockLight[Type] = false;

		TileObjectData.newTile.WaterPlacement = Terraria.Enums.LiquidPlacement.Allowed;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Width = 19;
		TileObjectData.newTile.Height = 23;
		TileObjectData.newTile.UsesCustomCanPlace = true;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.CoordinatePadding = 2;
		TileObjectData.newTile.CoordinateWidth = 16;
		TileObjectData.newTile.CoordinateHeights = new int[23];
		Array.Fill(TileObjectData.newTile.CoordinateHeights, 16);
		TileObjectData.newTile.LavaDeath = false;
		TileObjectData.newTile.Origin = new(0, 12);
		TileObjectData.newTile.AnchorTop = new AnchorData(0, 0, 0);
		TileObjectData.newTile.AnchorBottom = new AnchorData(0, 0, 0);
		TileObjectData.addTile(Type);

		AddMapEntry(new Color(101, 99, 107));
	}

	public override bool CanExplode(int i, int j)
	{
		return false;
	}

	public override bool CanKillTile(int i, int j, ref bool blockDamaged)
	{
		return false;
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		return false;
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		base.PostDraw(i, j, spriteBatch);
	}

	public override void NearbyEffects(int i, int j, bool closer)
	{
		Tile tile = Main.tile[i, j];
		tile.LiquidType = LiquidID.Water;
		tile.LiquidAmount = 255;
		if (tile.TileFrameX == 0 && tile.TileFrameY == 36)
		{
			if (YggdrasilWorld.CanEnterTheGiantGhoseClawBarnacle)
			{
				for (int x = 0; x < TotalWidth; x++)
				{
					for (int y = 0; y < TotalHeight; y++)
					{
						var checkTile = TileUtils.SafeGetTile(x + i, y + j);
						if (checkTile.TileType == ModContent.TileType<GiantGhostClawBarnacleDoorTile>())
						{
							checkTile.HasTile = false;
							checkTile.LiquidType = LiquidID.Water;
							checkTile.LiquidAmount = 255;
						}
					}
				}
			}
		}
		base.NearbyEffects(i, j, closer);
	}

	public override void PlaceOriginAtTopLeft(int x, int y)
	{
		if (x > Main.maxTilesX - TotalWidth || x < 0 || y > Main.maxTilesY - TotalHeight || y < 0)
		{
			return;
		}

		for (int i = 0; i < TotalWidth; i++)
		{
			for (int j = 0; j < TotalHeight; j++)
			{
				if (PixelHasTile[i, j] >= 200)
				{
					ushort type = Type;
					if (j > 2 && j < 20 && i <= 2)
					{
						type = (ushort)ModContent.TileType<GiantGhostClawBarnacleDoorTile>();
					}
					Tile tile = Main.tile[x + i, y + j];
					tile.TileType = type;
					tile.TileFrameX = (short)(i * 18);
					tile.TileFrameY = (short)(j * 18);
					tile.HasTile = true;
				}
			}
		}
	}

	public void AddScene(int i, int j)
	{
		Tile tile = TileUtils.SafeGetTile(i, j);
		if (tile.TileFrameX == 0 && tile.TileFrameY == 36)
		{
			GiantGhostClawBarnacle_VFX gGCBV = new GiantGhostClawBarnacle_VFX { Position = new Vector2(i, j - 2) * 16, Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = Type };
			Ins.VFXManager.Add(gGCBV);
			GiantGhostClawBarnacle_Background gGCBB = new GiantGhostClawBarnacle_Background { Position = new Vector2(i, j - 2) * 16, Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = Type };
			Ins.VFXManager.Add(gGCBB);
		}
	}
}

using Everglow.Commons.TileHelper;
using Everglow.Commons.VFX.Scene;
using Everglow.Yggdrasil.YggdrasilTown.Items.Tools.Developer;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.FurnaceTiles;

public class GiantFurnace : ShapeDataTile, ISceneTile
{
	public override void SetStaticDefaults()
	{
		CustomItemType = ModContent.ItemType<GiantFurnacePlaceItem>();
		DustType = DustID.Iron;
		TotalWidth = 47;
		TotalHeight = 60;
		MinPick = int.MaxValue;
		Main.tileSolid[Type] = false;
		Main.tileFrameImportant[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileNoAttach[Type] = false;
		Main.tileBlendAll[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);

		TileObjectData.newTile.Width = 47;
		TileObjectData.newTile.Height = 60;
		TileObjectData.newTile.UsesCustomCanPlace = true;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.CoordinatePadding = 2;
		TileObjectData.newTile.CoordinateWidth = 16;
		TileObjectData.newTile.CoordinateHeights = new int[60];
		Array.Fill(TileObjectData.newTile.CoordinateHeights, 16);
		TileObjectData.newTile.CoordinateHeights[^1] = 18;
		TileObjectData.newTile.LavaDeath = false;
		TileObjectData.newTile.Origin = new(24, 60);
		TileObjectData.newTile.AnchorTop = new AnchorData(0, 0, 0);
		TileObjectData.newTile.AnchorBottom = new AnchorData(0, 0, 0);
		TileObjectData.addTile(Type);

		AddMapEntry(new Color(255, 137, 68));
	}

	public void AddScene(int i, int j)
	{
		Tile tile = Main.tile[i, j];
		//if (tile.TileFrameX == 468 && tile.TileFrameY == 702)
		//{
		//	var scene = new GiantFurnace_Body { Position = new Vector2(i, j) * 16 + new Vector2(-226, -640), Active = true, OriginTilePos = new Point(i, j), OriginTileType = Type };
		//	Ins.VFXManager.Add(scene);
		//}
		if (tile.TileFrameX == 468 && tile.TileFrameY == 306)
		{
			var scene = new GiantFurnace_LavaWindow { Position = new Vector2(i, j) * 16 + new Vector2(-46, 14), Active = true, OriginTilePos = new Point(i, j), OriginTileType = Type };
			Ins.VFXManager.Add(scene);
		}
	}

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		Tile tile = Main.tile[i, j];
		if (tile.TileFrameX is >= 414 and < 540)
		{
			if (tile.TileFrameY is >= 360 and < 450)
			{
				r = 1;
				g = 0.7f;
				b = 0.2f;
			}
		}
		r *= 2;
		g *= 2;
		b *= 2;
	}

	public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		if (fail)
		{
			return;
		}
		if (!MultiItem)
		{
			CustomDropItem(i, j);
		}
		SoundEngine.PlaySound(HitSound, new Vector2(i * 16, j * 16));
	}

	public override void PlaceOriginAtBottomLeft(int x, int y)
	{
		if (x > Main.maxTilesX - TotalWidth || x < 0 || y > Main.maxTilesY || y - TotalHeight < 0)
		{
			return;
		}

		for (int i = 0; i < TotalWidth; i++)
		{
			for (int j = 0; j < TotalHeight; j++)
			{
				if (PixelHasTile[i, TotalHeight - j - 1] >= 200)
				{
					Tile tile = Main.tile[x + i, y - j];
					tile.TileType = Type;
					tile.TileFrameX = (short)(i * 18);
					tile.TileFrameY = (short)((TotalHeight - j - 1) * 18);
					tile.HasTile = true;
					if (tile.TileFrameY == 144 && tile.TileFrameX >= 126 && tile.TileFrameX <= 828 && !(tile.TileFrameX is >= 648 and <= 684))
					{
						tile.TileType = (ushort)ModContent.TileType<GiantFurnace_Block>();
						tile.TileFrameX = (short)(i * 18);
						tile.TileFrameY = (short)((TotalHeight - j - 1) * 18);
						tile.HasTile = true;
					}
					if (tile.TileFrameY == 522 && tile.TileFrameX >= 360 && tile.TileFrameX <= 720 && !(tile.TileFrameX is >= 648 and <= 684))
					{
						tile.TileType = (ushort)ModContent.TileType<GiantFurnace_Block>();
						tile.TileFrameX = (short)(i * 18);
						tile.TileFrameY = (short)((TotalHeight - j - 1) * 18);
						tile.HasTile = true;
					}
					if (tile.TileFrameY == 144 && (tile.TileFrameX is >= 648 and <= 684))
					{
						tile.TileType = (ushort)ModContent.TileType<GiantFurnace_Platform>();
						tile.TileFrameX = (short)(i * 18);
						tile.TileFrameY = (short)((TotalHeight - j - 1) * 18);
						tile.HasTile = true;
					}
					if (tile.TileFrameY >= 162 && (tile.TileFrameX is >= 648 and <= 684))
					{
						tile.TileType = (ushort)ModContent.TileType<GiantFurnace_Platform>();
						tile.TileFrameX = (short)(i * 18);
						tile.TileFrameY = (short)((TotalHeight - j - 1) * 18);
						tile.HasTile = true;
					}
				}
			}
		}
	}

	public override bool CanExplode(int i, int j) => false;

	public override bool CanKillTile(int i, int j, ref bool blockDamaged) => false;
}

using Everglow.Commons.Enums;
using Everglow.Commons.Utilities;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;
using Everglow.Commons.VFX.Pipelines;
using Terraria;
using Terraria.ObjectData;

namespace Everglow.Commons.DeveloperContent.Items;

/// <summary>
/// Visulalize the data of mouse-covered-tile.
/// </summary>
public class TileDataReader : ModItem
{
	public bool EnableResidentEffect = false;

	public override void SetDefaults()
	{
		Item.width = 16;
		Item.height = 16;
		Item.value = 0;
		Item.rare = ItemRarityID.White;
	}

	public override void HoldItem(Player player)
	{
		int i = Main.MouseWorld.ToTileCoordinates().X;
		int j = Main.MouseWorld.ToTileCoordinates().Y;
		if (!TileDataReaderSystem.OwnerPlayerWhoAmI.Contains(player.whoAmI))
		{
			TileDataReaderSystem.OwnerPlayerWhoAmI.Add(player.whoAmI);
			var vfx = new TileDataReaderSystem { FixPoint = new Point(i, j), Active = true, Visible = true, EverLasting = EnableResidentEffect };
			Ins.VFXManager.Add(vfx);
		}

		// Right click to enable resident tile reader effect.
		if (Main.mouseRight && Main.mouseRightRelease && !Main.mapFullscreen)
		{
			EnableResidentEffect = !EnableResidentEffect;
			CombatText.NewText(player.Hitbox, Color.White, (EnableResidentEffect ? "Enable" : "Disable") + "everlasting tile reading effect.");
			if (TileDataReaderSystem.OwnerPlayerWhoAmI.Contains(player.whoAmI))
			{
				TileDataReaderSystem.OwnerPlayerWhoAmI.Remove(player.whoAmI);
			}
		}
	}
}

[Pipeline(typeof(WCSPipeline))]
public class TileDataReaderSystem : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PreDrawFilter;

	public Texture2D Texture;
	public Point FixPoint;
	public static List<int> OwnerPlayerWhoAmI = new List<int>();
	public List<Point> ContinueTiles = new List<Point>();
	public List<Point> SurfaceTiles = new List<Point>();
	public List<Point> CheckLiquidTiles = new List<Point>();
	public Point OldTilePos = new Point(0, 0);
	public bool EverLasting = false;
	public int MaxContinueCount = 625;

	public override void OnSpawn()
	{
		Texture = ModAsset.TileBlock.Value;
	}

	public override void Update()
	{
		FixPoint = Main.MouseWorld.ToTileCoordinates();
		int i = FixPoint.X;
		int j = FixPoint.Y;
		Player player = Main.LocalPlayer;
		if (!OwnerPlayerWhoAmI.Contains(player.whoAmI))
		{
			Active = false;
			return;
		}
		if (i < 20 || i > Main.maxTilesX - 20)
		{
			if (j < 20 || j > Main.maxTilesY - 20)
			{
				Active = false;
				OwnerPlayerWhoAmI.Remove(player.whoAmI);
				return;
			}
		}

		if (player.HeldItem.type != ModContent.ItemType<TileDataReader>() && !EverLasting)
		{
			Active = false;
			OwnerPlayerWhoAmI.Remove(player.whoAmI);
			return;
		}
		if (OldTilePos != new Point(i, j))
		{
			UpdateContinueTiles(i, j);
		}
		OldTilePos = new Point(i, j);
		base.Update();
	}

	public override void Draw()
	{
		int i = FixPoint.X;
		int j = FixPoint.Y;
		Player player = Main.LocalPlayer;
		if (i < 20 || i > Main.maxTilesX - 20 || j < 20 || j > Main.maxTilesY - 20)
		{
			Active = false;
			OwnerPlayerWhoAmI.Remove(player.whoAmI);
			return;
		}
		Tile tile = TileUtils.SafeGetTile(i, j);
		int colorType = ItemRarityID.White;
		Color drawColor = Color.White;
		if (!tile.HasTile)
		{
			colorType = ItemRarityID.Gray;
			drawColor = Color.Gray;
		}
		DrawBlockBound(i, j, drawColor);
		if (tile.HasTile)
		{
			if (ContinueTiles.Count < MaxContinueCount)
			{
				var drawContinueColor = new Color(0.12f, 0.24f, 0.4f, 0);
				foreach (var check_tile in ContinueTiles)
				{
					DrawBlockBound(check_tile.X, check_tile.Y, drawContinueColor);
				}
			}
			if (SurfaceTiles.Count < MaxContinueCount)
			{
				var drawSurfaceTilesColor = new Color(0.8f, 0.03f, 0.06f, 0);
				foreach (var check_tile in SurfaceTiles)
				{
					DrawBlockBound(check_tile.X, check_tile.Y, drawSurfaceTilesColor);
				}
			}
		}
		if (CheckLiquidTiles.Count < MaxContinueCount)
		{
			var drawContinueColor = new Color(0.0f, 0.0f, 0.6f, 0);
			foreach (var check_tile in CheckLiquidTiles)
			{
				DrawBlockBound(check_tile.X, check_tile.Y, drawContinueColor);
			}
		}
		string datas = GetDatas(i, j);
		Main.instance.MouseText(datas, colorType);
	}

	public string GetDatas(int i, int j)
	{
		Tile tile = TileUtils.SafeGetTile(i, j);

		string datas = "\nCoordinate: [" + i + ", " + j + "]";
		datas += "\nHasTile: " + tile.HasTile;
		datas += "\nType :" + tile.TileType;
		//datas += "\nPaint :" + tile.BlockColorAndCoating().Invisible;
		if (tile.HasTile)
		{
			datas += " " + TileID.Search.GetName(tile.TileType);
			datas += "\nFrame : [" + tile.TileFrameX + ", " + tile.TileFrameY + "]";
			if (ContinueTiles.Count < MaxContinueCount)
			{
				datas += "\nContinue Tiles : " + ContinueTiles.Count;
			}
			if (SurfaceTiles.Count < MaxContinueCount)
			{
				datas += "\nSurface Tiles : " + SurfaceTiles.Count;
			}
			int multiStyle = TileObjectData.GetTileStyle(tile);
			if (multiStyle >= 0)
			{
				datas += "\nMultiTile Style: " + multiStyle;
			}
		}
		if (!tile.HasTile)
		{
			if (CheckLiquidTiles.Count <= 900)
			{
				datas += "\nCan Fill Liquid Blocks: " + CheckLiquidTiles.Count;
			}
		}
		//float waterLine;
		//Collision.GetWaterLine(i, j, out waterLine);
		//datas += "\n" + waterLine;

		if (tile.WallType > WallID.None)
		{
			datas += "\nWallType :" + tile.WallType;
			datas += " " + WallID.Search.GetName(tile.WallType);
		}

		// datas += "\nSlope: " + tile.Slope;
		datas += "\nColliding: " + Collision.IsWorldPointSolid(Main.MouseWorld);
		return datas;
	}

	/// <summary>
	/// When there is an isolated tile area(<=MaxContinueCount tiles), you can check the number of continue tiles.
	/// </summary>
	/// <returns></returns>
	public void UpdateContinueTiles(int i, int j)
	{
		Tile tile = TileUtils.SafeGetTile(i, j);
		if (tile.HasTile)
		{
			SurfaceTiles = TileUtils.BFSSurface(new Point(i, j), 625);
			ContinueTiles = TileUtils.BFSContinueTile(new Point(i, j), false, 625);
		}
		else
		{
			CheckLiquidTiles = TileUtils.BFSGetCanFillLiquidTiles(i, j);
		}
	}

	public void DrawBlockBound(int i, int j, Color color)
	{
		Vector2 pos = new Vector2(i, j) * 16;
		var bars = new List<Vertex2D>()
		{
			new Vertex2D(pos, color, new Vector3(0, 0, 0)),
			new Vertex2D(pos + new Vector2(16, 0), color, new Vector3(1, 0, 0)),
			new Vertex2D(pos + new Vector2(0, 16), color, new Vector3(0, 1, 0)),

			new Vertex2D(pos + new Vector2(0, 16), color, new Vector3(0, 1, 0)),
			new Vertex2D(pos + new Vector2(16, 0), color, new Vector3(1, 0, 0)),
			new Vertex2D(pos + new Vector2(16), color, new Vector3(1, 1, 0)),
		};

		Ins.Batch.Draw(Texture, bars, PrimitiveType.TriangleList);
	}
}

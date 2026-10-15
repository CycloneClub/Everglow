using Everglow.Commons.Enums;
using Everglow.Commons.VFX;
using Everglow.Commons.VFX.Pipelines;

namespace Everglow.Commons.DeveloperContent.Items;

[Pipeline(typeof(WCSPipeline))]
public class TileFliperSystemVFX : Visual
{
	public Point FixPoint;

	public Point ControlPoint;

	public Player Owner;

	public Item TileFliperItem = null;

	public Rectangle FlipArea()
	{
		int minX = Math.Min(FixPoint.X, ControlPoint.X);
		int maxX = Math.Max(FixPoint.X, ControlPoint.X);
		int minY = Math.Min(FixPoint.Y, ControlPoint.Y);
		int maxY = Math.Max(FixPoint.Y, ControlPoint.Y);
		return new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
	}

	public override CodeLayer DrawLayer => CodeLayer.PostDrawTiles;

	public override void Update()
	{
		if (Owner.HeldItem.type == ModContent.ItemType<TileFliper>())
		{
			if (TileFliperItem is null)
			{
				TileFliperItem = Owner.HeldItem;
			}
			TileFliper tf = TileFliperItem.ModItem as TileFliper;
			if (tf != null)
			{
				tf.EnableFlipSystem = true;
			}
		}
		else
		{
			Kill();
			return;
		}

		if (Main.mouseLeft && Main.mouseLeftRelease)
		{
			FixPoint = Main.MouseWorld.ToTileCoordinates();
		}
		if (Main.mouseLeft)
		{
			ControlPoint = Main.MouseWorld.ToTileCoordinates();
		}
		if (Main.mouseRight && Main.mouseRightRelease)
		{
			FlipTheTileMap();
		}
	}

	public void FlipTheTileMap()
	{
		Rectangle flip = FlipArea();
		for (int i = 0; i < flip.Width / 2; i++)
		{
			for (int j = 0; j < flip.Height; j++)
			{
				var tile0 = Main.tile[i + flip.X, j + flip.Y];
				var tile1 = Main.tile[flip.X + flip.Width - i - 1, j + flip.Y];
				(tile0.TileType, tile1.TileType) = (tile1.TileType, tile0.TileType);
				(tile0.HasTile, tile1.HasTile) = (tile1.HasTile, tile0.HasTile);
				(tile0.WallType, tile1.WallType) = (tile1.WallType, tile0.WallType);
				(tile0.Slope, tile1.Slope) = (tile1.Slope, tile0.Slope);
				ReverseSlope(i + flip.X, j + flip.Y);
				ReverseSlope(flip.X + flip.Width - i - 1, j + flip.Y);
				(tile0.LiquidType, tile1.LiquidType) = (tile1.LiquidType, tile0.LiquidType);
				(tile0.LiquidAmount, tile1.LiquidAmount) = (tile1.LiquidAmount, tile0.LiquidAmount);
				(tile0.TileColor, tile1.TileColor) = (tile1.TileColor, tile0.TileColor);
				(tile0.WallColor, tile1.WallColor) = (tile1.WallColor, tile0.WallColor);
				(tile0.IsHalfBlock, tile1.IsHalfBlock) = (tile1.IsHalfBlock, tile0.IsHalfBlock);
				(tile0.TileFrameX, tile1.TileFrameX) = (tile1.TileFrameX, tile0.TileFrameX);
				(tile0.TileFrameY, tile1.TileFrameY) = (tile1.TileFrameY, tile0.TileFrameY);
				(tile0.HasActuator, tile1.HasActuator) = (tile1.HasActuator, tile0.HasActuator);
				(tile0.IsActuated, tile1.IsActuated) = (tile1.IsActuated, tile0.IsActuated);
			}
		}
		for (int i = 0; i < flip.Width; i++)
		{
			for (int j = 0; j < flip.Height; j++)
			{
				WorldGen.SquareTileFrame(i + flip.X, j + flip.Y);
				WorldGen.SquareWallFrame(i + flip.X, j + flip.Y);
			}
		}
	}

	public void ReverseSlope(int i, int j)
	{
		Tile tile = Main.tile[i, j];
		if (tile.Slope == SlopeType.SlopeDownLeft)
		{
			tile.Slope = SlopeType.SlopeDownRight;
			return;
		}
		if (tile.Slope == SlopeType.SlopeUpLeft)
		{
			tile.Slope = SlopeType.SlopeUpRight;
			return;
		}
		if (tile.Slope == SlopeType.SlopeDownRight)
		{
			tile.Slope = SlopeType.SlopeDownLeft;
			return;
		}
		if (tile.Slope == SlopeType.SlopeUpRight)
		{
			tile.Slope = SlopeType.SlopeUpLeft;
			return;
		}
	}

	public override void Kill()
	{
		if (TileFliperItem != null)
		{
			TileFliper tf = TileFliperItem.ModItem as TileFliper;
			if (tf != null)
			{
				tf.EnableFlipSystem = false;
			}
		}
		base.Kill();
	}

	public override void Draw()
	{
		Rectangle drawTileArea = FlipArea();
		var startPoint = new Point(drawTileArea.X, drawTileArea.Y);
		Texture2D tex = ModAsset.TileBlock4x4.Value;
		Color drawColor = new Color(0.4f, 0.2f, 1f, 0.3f);
		for (int i = 0; i < drawTileArea.Width; i++)
		{
			for (int j = 0; j < drawTileArea.Height; j++)
			{
				Vector2 drawPos = (startPoint + new Point(i, j)).ToWorldCoordinates();
				Rectangle drawFrame = new Rectangle(16, 16, 16, 16);
				if (i == 0)
				{
					drawFrame.X = 0;
				}
				if (j == 0)
				{
					drawFrame.Y = 0;
				}
				if (i == drawTileArea.Width - 1)
				{
					drawFrame.X = 32;
				}
				if (j == drawTileArea.Height - 1)
				{
					drawFrame.Y = 32;
				}
				if (i == drawTileArea.Width - 1 && i == 0)
				{
					drawFrame.X = 48;
				}
				if (j == drawTileArea.Height - 1 && j == 0)
				{
					drawFrame.Y = 48;
				}
				Ins.Batch.Draw(tex, drawPos, drawFrame, drawColor, 0, new Vector2(8), 1f, SpriteEffects.None);
			}
		}
	}
}

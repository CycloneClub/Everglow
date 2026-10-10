using Everglow.Commons.DataStructures;
using static Everglow.Yggdrasil.WorldGeneration.MainWorldGeneration_Yggdrasil;
using static Everglow.Yggdrasil.WorldGeneration.YggdrasilWorldGeneration;

namespace Everglow.Yggdrasil.Common.Tiles;

public class CheckPylonPoint : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Tools;

	public Point CheckPos = new Point(0, 0);

	public override void SetDefaults()
	{
		Item.width = 16;
		Item.height = 16;
	}

	public override void HoldItem(Player player)
	{
		if (Main.mouseLeft && Main.mouseLeftRelease)
		{
			int pointX = (int)(WorldGen.genRand.Next(80, 160) * (WorldGen.genRand.Next(2) - 0.5f) * 2 - 20 + Main.maxTilesX / 2);
			int pointY = 160;
			int counts = 0;
			while (!IsTileSmooth(new Point(pointX, pointY)) || !AreaNoChest(pointX, pointX + 16, pointY - 13, pointY + 3))
			{
				pointX = (int)(WorldGen.genRand.Next(80, 240) * (WorldGen.genRand.Next(2) - 0.5f) * 2 - 20 + Main.maxTilesX / 2);
				for (int y = 160; y < Main.maxTilesY / 3; y++)
				{
					if (TileUtils.SafeGetTile(pointX, y).HasTile && TileUtils.SafeGetTile(pointX, y).TileType != TileID.Trees)
					{
						if (TileUtils.SafeGetTile(pointX, y - 1).HasTile)
						{
							y -= 2;
						}
						pointY = y;
						break;
					}
				}
				counts++;
				if (counts > 150)
				{
					break;
				}
			}
			CheckPos = new Point(pointX, pointY);
			Main.NewText(CheckPos);
			Main.NewText(counts, Color.Yellow);
		}
	}

	public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
	{
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Texture2D square = Commons.ModAsset.TileBlock.Value;
		for (int i = 0; i < 16; i++)
		{
			for (int j = 0; j < 16; j++)
			{
				Main.spriteBatch.Draw(square, (CheckPos + new Point(i, j - 13)).ToWorldCoordinates() - new Vector2(8) - Main.screenPosition, null, new Color(1f, 0.1f, 0.1f, 1f), 0, Vector2.zeroVector, 1f, SpriteEffects.None, 0);
			}
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
	}
}

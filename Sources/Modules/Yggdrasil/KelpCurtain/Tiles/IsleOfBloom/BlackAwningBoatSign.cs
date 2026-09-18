using Everglow.Commons.CustomTiles;
using Everglow.Yggdrasil.KelpCurtain.CustomTiles;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.IsleOfBloom;

public class BlackAwningBoatSign : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileLavaDeath[Type] = true;
		Main.tileWaterDeath[Type] = false;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
		TileObjectData.newTile.Height = 3;
		TileObjectData.newTile.Width = 3;
		TileObjectData.newTile.CoordinateHeights = new int[]
		{
			16,
			16,
			18,
		};
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.LavaDeath = true;
		TileObjectData.addTile(Type);
		DustType = ModContent.DustType<AlgaeExtractor_Dust>();
		AddMapEntry(new Color(131, 158, 154));
	}

	public override bool RightClick(int i, int j)
	{
		var tile = Main.tile[i, j];
		float offset_x = -tile.TileFrameX / 18f * 16f - 16;
		if (tile.TileFrameX > 36)
		{
			offset_x = (108 - tile.TileFrameX) / 18f * 16f + 16;
		}
		float offset_y = -tile.TileFrameY / 18f * 16f;
		Vector2 summonPos = new Vector2(i, j) * 16f + new Vector2(offset_x, offset_y);
		int x = (int)(summonPos.X / 16);
		int y = (int)(summonPos.Y / 16);
		var checkWaterTile = Main.tile[x, y];
		float waterLine = 0;
		if (checkWaterTile.LiquidAmount <= 0)
		{
			for (int h = 0; h < 40; h++)
			{
				Collision.GetWaterLine(x, y + h, out waterLine);
				if (waterLine != 0)
				{
					summonPos.Y = waterLine;
					break;
				}
			}
		}
		else
		{
			for (int h = 0; h < 40; h++)
			{
				Collision.GetWaterLine(x, y - h, out waterLine);
				if (waterLine != 0)
				{
					summonPos.Y = waterLine;
					break;
				}
			}
		}

		summonPos -= new Vector2(46, 20);
		ColliderManager.Instance.Add<BlackAwningBoat>(summonPos);
		for (int l = 0; l < 16; l++)
		{
			int type;
			switch (Main.rand.Next(3))
			{
				case 0:
					type = GoreID.ChimneySmoke1;
					break;
				case 1:
					type = GoreID.ChimneySmoke2;
					break;
				case 2:
					type = GoreID.ChimneySmoke3;
					break;
				default:
					type = GoreID.ChimneySmoke1;
					break;
			}

			var gore = Gore.NewGorePerfect(summonPos + new Vector2(Main.rand.NextFloat(92), Main.rand.NextFloat(20)) - new Vector2(17, 18), new Vector2(0, Main.rand.NextFloat(1f)).RotatedByRandom(MathHelper.TwoPi), type);
			gore.timeLeft = Main.rand.Next(60, 120);
		}
		return base.RightClick(i, j);
	}

	public override bool CanExplode(int i, int j) => false;

	public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
	{
		noBreak = true;
		return base.TileFrame(i, j, ref resetFrame, ref noBreak);
	}
}

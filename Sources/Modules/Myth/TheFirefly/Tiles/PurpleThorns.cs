namespace Everglow.Myth.TheFirefly.Tiles;

public class PurpleThorns : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileCut[Type] = true;
		TileID.Sets.TouchDamageDestroyTile[Type] = true;
		TileID.Sets.TouchDamageImmediate[Type] = 12;
		DustType = DustID.SpookyWood;
		HitSound = SoundID.Grass;
		AddMapEntry(new Color(35, 9, 35));
	}

	public override bool IsTileDangerous(int i, int j, Player player)
	{
		if (player.HasBuff(BuffID.Dangersense))
		{
			return true;
		}
		return false;
	}

	public override void RandomUpdate(int i, int j, bool underground)
	{
		int nearCount = 0;
		for (int x = -5; x < 6; x++)
		{
			for (int y = -5; y < 6; y++)
			{
				if (Main.tile[i + x, j + y].TileType == Type)
				{
					nearCount++;
					if (nearCount >= 14)
					{
						return;
					}
				}
			}
		}
		(int, int) nextPos = (i, j - 1);
		switch (Main.rand.Next(4))
		{
			case 0:
				break;
			case 1:
				nextPos = (i + 1, j);
				break;
			case 2:
				nextPos = (i - 1, j);
				break;
			case 3:
				nextPos = (i, j + 1);
				break;
		}
		var nextTile = Main.tile[nextPos.Item1, nextPos.Item2];

		var nextTileLeft = Main.tile[nextPos.Item1 - 1, nextPos.Item2];

		var nextTileUp = Main.tile[nextPos.Item1, nextPos.Item2 - 1];

		var nextTileRight = Main.tile[nextPos.Item1 + 1, nextPos.Item2];

		var nextTileDown = Main.tile[nextPos.Item1, nextPos.Item2 + 1];

		var nextTileLeftUp = Main.tile[nextPos.Item1 - 1, nextPos.Item2 - 1];

		var nextTileRightUp = Main.tile[nextPos.Item1 + 1, nextPos.Item2 - 1];

		var nextTileRightDown = Main.tile[nextPos.Item1 + 1, nextPos.Item2 + 1];

		var nextTileLeftDown = Main.tile[nextPos.Item1 - 1, nextPos.Item2 + 1];

		if (nextTileLeftUp.HasTile && nextTileLeft.HasTile && nextTileUp.HasTile)
		{
			return;
		}
		if (nextTileLeftDown.HasTile && nextTileLeft.HasTile && nextTileDown.HasTile)
		{
			return;
		}
		if (nextTileRightDown.HasTile && nextTileRight.HasTile && nextTileDown.HasTile)
		{
			return;
		}
		if (nextTileRightUp.HasTile && nextTileRight.HasTile && nextTileUp.HasTile)
		{
			return;
		}
		if (!nextTile.HasTile && nextTile.LiquidAmount == 0)
		{
			WorldGen.PlaceTile(nextPos.Item1, nextPos.Item2, Type);
		}
	}

	public override void NearbyEffects(int i, int j, bool closer)
	{
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		base.PostDraw(i, j, spriteBatch);
	}
}

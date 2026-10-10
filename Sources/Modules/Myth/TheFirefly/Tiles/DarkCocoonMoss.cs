namespace Everglow.Myth.TheFirefly.Tiles;

public class DarkCocoonMoss : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileMergeDirt[Type] = true;
		Main.tileBlockLight[Type] = true;
		Main.tileMerge[Type][ModContent.TileType<DarkCocoon>()] = true;
		Main.tileMerge[Type][ModContent.TileType<DarkCocoonSpecial>()] = true;
		MinPick = 175;
		DustType = DustID.SpookyWood;
		AddMapEntry(new Color(35, 49, 122));
	}

	public override IEnumerable<Item> GetItemDrops(int i, int j)
	{
		yield return new Item(ModContent.ItemType<Items.DarkCocoon>());
	}

	public override void RandomUpdate(int i, int j, bool underground)
	{
		Tile t0 = Main.tile[i, j];
		Tile t1 = Main.tile[i, j - 1];
		Tile t2 = Main.tile[i, j + 1];
		Tile t3 = Main.tile[i + 1, j];
		Tile t4 = Main.tile[i - 1, j];
		if (Main.rand.NextBool(2))// 黑萤苔藓
		{
			switch (Main.rand.Next(4))
			{
				case 0:
					if (t0.Slope == SlopeType.Solid && !t2.HasTile)
					{
						t2.TileType = (ushort)ModContent.TileType<FireflyMoss>();
						t2.HasTile = true;
						t2.TileFrameY = (short)(Main.rand.Next(3, 6) * 18);
					}
					break;
				case 1:
					if (t0.Slope == SlopeType.Solid && !t4.HasTile)
					{
						t4.TileType = (ushort)ModContent.TileType<FireflyMoss>();
						t4.HasTile = true;
						t4.TileFrameY = (short)(Main.rand.Next(9, 12) * 18);
					}
					break;
				case 2:
					if (t0.Slope == SlopeType.Solid && !t3.HasTile)
					{
						t3.TileType = (ushort)ModContent.TileType<FireflyMoss>();
						t3.HasTile = true;
						t3.TileFrameY = (short)(Main.rand.Next(6, 9) * 18);
					}
					break;
				case 3:
					if (t0.Slope == SlopeType.Solid && !t1.HasTile)
					{
						t1.TileType = (ushort)ModContent.TileType<FireflyMoss>();
						t1.HasTile = true;
						t1.TileFrameY = (short)(Main.rand.Next(0, 3) * 18);
					}
					break;
			}
		}
		else
		{
			int count = CheckDarkBeside(i, j);
			if (count <= 16)
			{
				switch (Main.rand.Next(4))
				{
					case 0:
						if (CheckDarkCocoon(t1))
						{
							t1.TileType = Type;
							t1.HasTile = true;
						}
						break;
					case 1:
						if (CheckDarkCocoon(t2))
						{
							t2.TileType = Type;
							t2.HasTile = true;
						}
						break;
					case 2:
						if (CheckDarkCocoon(t3))
						{
							t3.TileType = Type;
							t3.HasTile = true;
						}
						break;
					case 3:
						if (CheckDarkCocoon(t4))
						{
							t4.TileType = Type;
							t4.HasTile = true;
						}
						break;
				}
			}
		}
	}

	// 判定周围有多少个同样的块
	private int CheckDarkBeside(int i, int j)
	{
		int count = 0;
		for (int x = -6; x < 7; x++)
		{
			for (int y = -6; y < 7; y++)
			{
				var tileCheck = Main.tile[i + x, j + y];
				if (CheckDarkCocoon(tileCheck))
				{
					count++;
				}
			}
		}
		return count;
	}

	private bool CheckDarkCocoon(Tile tile)
	{
		if (tile.HasTile)
		{
			if (tile.TileType == Type)
			{
				return true;
			}
		}
		return false;
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var tile = Main.tile[i, j];
		var zero = new Vector2(Main.offScreenRange, Main.offScreenRange);

		if (Main.drawToScreen)
		{
			zero = Vector2.Zero;
		}

		Texture2D tex = ModAsset.DarkCocoonMossGlow.Value;

		spriteBatch.Draw(tex, new Vector2(i * 16, j * 16) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), new Color(55, 55, 55, 0), 0, new Vector2(0), 1, SpriteEffects.None, 0);

		base.PostDraw(i, j, spriteBatch);
	}

	public override bool CanExplode(int i, int j)
	{
		return false;
	}
}

namespace Everglow.PlantAndFarm.Common.Systems;

public class FlowerAutoMultiply : GlobalTile
{
	public override void RandomUpdate(int i, int j, int type)
	{
		Vector2[] Types =
		 { new Vector2(1, 10),
		 new Vector2(1, 11),
		 new Vector2(2, 13),
		 new Vector2(2, 18),
		 new Vector2(3, 22),
		 new Vector2(3, 23),
		 new Vector2(3, 21),
		 new Vector2(4, 25),
		 new Vector2(4, 26),
		 new Vector2(4, 24),
		 new Vector2(5, 28),
		 new Vector2(5, 29),
		 new Vector2(5, 27),
		 new Vector2(6, 31),
		 new Vector2(6, 32),
		 new Vector2(6, 30),
		 new Vector2(7, 34),
		 new Vector2(7, 35),
		 new Vector2(7, 33),
		 new Vector2(8, 37),
		 new Vector2(8, 38),
		 new Vector2(8, 36),
		 new Vector2(9, 40),
		 new Vector2(9, 41),
		 new Vector2(9, 39),
		 new Vector2(10, 43),
		 new Vector2(10, 44),
		 new Vector2(10, 42), };
		Tile tile = Main.tile[i, j];
		if (tile.TileType == 3)
		{
			int X = tile.TileFrameX / 18;
			for (int u = 1; u < 11; u++)
			{
				int id = Array.IndexOf(Types, new Vector2(u, X));
				if (id != -1)
				{
					for (int x = -4; x < 5; x++)
					{
						for (int y = -2; y < 3; y++)
						{
							if (!((Tile)Main.tile[i + x, j + y]).HasTile && Main.tile[i + x, j + y + 1].TileType == 2 && Main.tile[i + x, j + y + 1].Slope == SlopeType.Solid && !(Main.tile[i + x, j + y].LiquidAmount > 1))
							{
								int TYPE = 0;
								if (u >= 3)
								{
									TYPE = u * 3 + Main.rand.Next(3) + 12;
								}

								if (u == 2)
								{
									TYPE = Main.rand.NextBool() ? 13 : 18;
								}

								if (u == 1)
								{
									TYPE = Main.rand.NextBool() ? 10 : 11;
								}

								Main.tile[i + x, j + y].TileType = 3;
								Main.tile[i + x, j + y].TileFrameX = (short)(TYPE * 18);
								((Tile)Main.tile[i + x, j + y]).HasTile = true;
							}
						}
					}
				}
			}
		}
	}
	public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		/*bool Lootf = false;
            Player player = Main.LocalPlayer;
            for (int t = 0; t < 58; t++)
            {
                if (player.inventory[t].type == ModContent.ItemType<Items.Flowers.FlowerBrochure>() && player.inventory[t].stack > 0)
                {
                    Lootf = true;
                    break;
                }
            }
            if (Lootf)
            {
                if (Main.tile[i, j].TileType == 73 || Main.tile[i, j].TileType == 3)
                {
                    if (Main.tile[i, j].TileFrameY == 0)
                    {
                        if (Main.tile[i, j].TileFrameX >= 21 * 18 && Main.tile[i, j].TileFrameX <= 23 * 18)
                        {
                            Item.NewItem(null, new Vector2(i * 16, j * 16), ModContent.ItemType<Items.Flowers.RedFlame>());
                        }
                        if (Main.tile[i, j].TileFrameX >= 24 * 18 && Main.tile[i, j].TileFrameX <= 26 * 18)
                        {
                            Item.NewItem(null, new Vector2(i * 16, j * 16), ModContent.ItemType<Items.Flowers.GoldCup>());
                        }
                        if (Main.tile[i, j].TileFrameX >= 27 * 18 && Main.tile[i, j].TileFrameX <= 29 * 18)
                        {
                            Item.NewItem(null, new Vector2(i * 16, j * 16), ModContent.ItemType<Items.Flowers.ShallowNight>());
                        }
                        if (Main.tile[i, j].TileFrameX >= 30 * 18 && Main.tile[i, j].TileFrameX <= 32 * 18)
                        {
                            Item.NewItem(null, new Vector2(i * 16, j * 16), ModContent.ItemType<Items.Flowers.PurplePhantom>());
                        }
                        if (Main.tile[i, j].TileFrameX >= 33 * 18 && Main.tile[i, j].TileFrameX <= 35 * 18)
                        {
                            Item.NewItem(null, new Vector2(i * 16, j * 16), ModContent.ItemType<Items.Flowers.HotPinkTulip>());
                        }
                        if (Main.tile[i, j].TileFrameX >= 36 * 18 && Main.tile[i, j].TileFrameX <= 38 * 18)
                        {
                            Item.NewItem(null, new Vector2(i * 16, j * 16), ModContent.ItemType<Items.Flowers.WhiteTulip>());
                        }
                        if (Main.tile[i, j].TileFrameX >= 39 * 18 && Main.tile[i, j].TileFrameX <= 41 * 18)
                        {
                            Item.NewItem(null, new Vector2(i * 16, j * 16), ModContent.ItemType<Items.Flowers.OrangeTulip>());
                        }
                        if (Main.tile[i, j].TileFrameX >= 42 * 18 && Main.tile[i, j].TileFrameX <= 44 * 18)
                        {
                            Item.NewItem(null, new Vector2(i * 16, j * 16), ModContent.ItemType<Items.Flowers.LightPurpleBalls>());
                        }
                        if (Main.tile[i, j].TileFrameX == 13 * 18 || Main.tile[i, j].TileFrameX == 18 * 18)
                        {
                            Item.NewItem(null, new Vector2(i * 16, j * 16), ModContent.ItemType<Items.Flowers.GoldWhip>());
                        }
                        if (Main.tile[i, j].TileFrameX == 20 * 18)
                        {
                            Item.NewItem(null, new Vector2(i * 16, j * 16), ModContent.ItemType<Items.Flowers.CyanHyacinth>());
                        }
                        if (Main.tile[i, j].TileFrameX == 6 * 18)
                        {
                            Item.NewItem(null, new Vector2(i * 16, j * 16), ModContent.ItemType<Items.Flowers.PurpleTail>());
                        }
                        if (Main.tile[i, j].TileFrameX == 7 * 18)
                        {
                            Item.NewItem(null, new Vector2(i * 16, j * 16), ModContent.ItemType<Items.Flowers.DarkPoppy>());
                        }
                        if (Main.tile[i, j].TileFrameX == 9 * 18)
                        {
                            Item.NewItem(null, new Vector2(i * 16, j * 16), ModContent.ItemType<Items.Flowers.BlueFreeze>());
                        }
                        if (Main.tile[i, j].TileFrameX == 10 * 18 || Main.tile[i, j].TileFrameX == 11 * 18)
                        {
                            Item.NewItem(null, new Vector2(i * 16, j * 16), ModContent.ItemType<Items.Flowers.LightChrysanthemum>());
                        }
                        if (Main.tile[i, j].TileFrameX == 12 * 18)
                        {
                            Item.NewItem(null, new Vector2(i * 16, j * 16), ModContent.ItemType<Items.Flowers.Lavender>());
                        }
                        if (Main.tile[i, j].TileFrameX == 14 * 18)
                        {
                            Item.NewItem(null, new Vector2(i * 16, j * 16), ModContent.ItemType<Items.Flowers.WhiteStar>());
                        }
                        if (Main.tile[i, j].TileFrameX == 15 * 18)
                        {
                            Item.NewItem(null, new Vector2(i * 16, j * 16), ModContent.ItemType<Items.Flowers.SilverClock>());
                        }
                        if (Main.tile[i, j].TileFrameX == 16 * 18)
                        {
                            Item.NewItem(null, new Vector2(i * 16, j * 16), ModContent.ItemType<Items.Flowers.BluePedal>());
                        }
                        if (Main.tile[i, j].TileFrameX == 17 * 18)
                        {
                            Item.NewItem(null, new Vector2(i * 16, j * 16), ModContent.ItemType<Items.Flowers.PinkSun>());
                        }
                        if (Main.tile[i, j].TileFrameX == 19 * 18)
                        {
                            Item.NewItem(null, new Vector2(i * 16, j * 16), ModContent.ItemType<Items.Flowers.OrangeSausage>());
                        }
                    }
                }
            }*/
	}
}

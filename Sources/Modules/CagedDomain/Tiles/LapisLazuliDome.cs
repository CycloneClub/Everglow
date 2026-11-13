using Everglow.CagedDomain.Dusts;
using Everglow.CagedDomain.Items;
using Everglow.Commons.TileHelper;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ObjectData;

namespace Everglow.CagedDomain.Tiles
{
	public class LapisLazuliDome : ShapeDataTile
	{
		public override void SetStaticDefaults()
		{
			CustomItemType = ModContent.ItemType<LapisLazuliDome_Item>();
			DustType = ModContent.DustType<LapisLazuliDome_Dust>();
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;

			TileObjectData.newTile.Width = 16;
			TileObjectData.newTile.Height = 7;
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinateHeights = new int[7];
			Array.Fill(TileObjectData.newTile.CoordinateHeights, 16);
			TileObjectData.newTile.CoordinateHeights[^1] = 20;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.Origin = new(8, 6);
			TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.None, 0, 0);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
			TileObjectData.addTile(Type);

			AddMapEntry(new Color(15, 80, 137));
		}

		public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
		{
			if (fail)
			{
				return;
			}
			var thisTile = Main.tile[i, j];
			int x0 = i - thisTile.TileFrameX / 18;
			int y0 = j - thisTile.TileFrameY / 18;
			int times = 1;
			for (int x = 0; x < TotalWidth; x++)
			{
				for (int y = 0; y < TotalHeight; y++)
				{
					var tile = Main.tile[x0 + x, y0 + y];
					if (tile.TileFrameX == x * 18 && tile.TileFrameY == y * 18)
					{
						if (tile.TileType == Type && PixelHasTile[x, y] >= 200)
						{
							times++;
							tile.HasTile = false;
							for (int f = 0; f < 2; f++)
							{
								Dust.NewDust(new Vector2(x0 + x, y0 + y) * 16, 16, 16, DustType, 0, 0, 0, default, 1);
							}
							if (MultiItem)
							{
								CustomDropItem(x0 + x, y0 + y);
							}
						}
					}
				}
			}
			if (!MultiItem)
			{
				CustomDropItem(i, j);
			}
			SoundEngine.PlaySound(HitSound, new Vector2(i * 16, j * 16));
		}
	}
}

using Everglow.CagedDomain.Dusts;
using Everglow.CagedDomain.Items;
using Everglow.Commons.TileHelper;
using Everglow.Commons.Utilities;
using Terraria.Audio;
using Terraria.ObjectData;

namespace Everglow.CagedDomain.Tiles
{
	public class BronzeLamp : ShapeDataTile
	{
		public override void SetStaticDefaults()
		{
			CustomItemType = ModContent.ItemType<BronzeLamp_Item>();
			DustType = ModContent.DustType<BronzeLamp_Dust>();
			Main.tileFrameImportant[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileNoAttach[Type] = true;

			AdjTiles = new int[] { TileID.Lamps };

			// TileObjectData.newSubTile.CopyFrom(TileObjectData.Style3x4);
			TileObjectData.newSubTile.Width = 3;
			TileObjectData.newSubTile.Height = 12;
			TileObjectData.newSubTile.UsesCustomCanPlace = true;
			TileObjectData.newSubTile.StyleHorizontal = true;
			TileObjectData.newSubTile.CoordinatePadding = 2;
			TileObjectData.newSubTile.CoordinateWidth = 16;
			TileObjectData.newSubTile.CoordinateHeights = new int[12];
			Array.Fill(TileObjectData.newSubTile.CoordinateHeights, 16);
			TileObjectData.newSubTile.CoordinateHeights[^1] = 20;
			TileObjectData.newSubTile.LavaDeath = false;
			TileObjectData.newSubTile.Origin = new(1, 11);
			TileObjectData.newSubTile.AnchorBottom = new(Terraria.Enums.AnchorType.SolidTile, 3, 0);
			TileObjectData.addSubTile(1);

			// TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
			TileObjectData.newTile.Width = 3;
			TileObjectData.newTile.Height = 12;
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinateHeights = new int[12];
			Array.Fill(TileObjectData.newTile.CoordinateHeights, 16);
			TileObjectData.newTile.CoordinateHeights[^1] = 20;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.Origin = new(1, 11);
			TileObjectData.newTile.AnchorBottom = new(Terraria.Enums.AnchorType.SolidTile, 3, 0);
			TileObjectData.addTile(Type);

			AddMapEntry(Color.Gold);
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			Tile tile = Main.tile[i, j];
			if (tile.TileFrameX == 18)
			{
				if (tile.TileFrameY == 18)
				{
					r = 1;
					g = 0.5f;
					b = 0.2f;
				}
				else if (tile.TileFrameY == 36)
				{
					r = 1;
					g = 0.5f;
					b = 0.2f;
				}
			}
			else
			{
				r = g = b = 0;
			}
			r *= 3;
			g *= 3;
			b *= 3;
		}

		public override void HitWire(int i, int j)
		{
			FurnitureUtils.LightHitwire(i, j, Type, 3, 12);
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
							bool glassDust = false;
							if (tile.TileFrameY == 18)
							{
								glassDust = true;
							}
							else if (tile.TileFrameY == 36)
							{
								glassDust = true;
							}
							int max = glassDust ? 5 : 1;
							for (int a = 0; a < max; a++)
							{
								Dust dust = Dust.NewDustDirect(new Vector2(x0 + x, y0 + y) * 16, 16, 16, DustType, 0, 0, 0, default, 1);
								dust.frame = new Rectangle(glassDust ? 10 : 0, Main.rand.Next(3) * 10, 8, 8);
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

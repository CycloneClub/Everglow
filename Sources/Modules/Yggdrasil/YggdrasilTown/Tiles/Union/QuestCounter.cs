using Everglow.Commons.Templates.Furniture;
using Everglow.Commons.TileHelper;
using Everglow.Commons.VFX.Scene;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.Union;

public class QuestCounter : ShapeDataTile, ISceneTile
{
	public override void SetStaticDefaults()
	{
		CustomItemType = ModContent.ItemType<QuestCounter_Item>();
		DustType = DustID.DynastyWood;
		TotalWidth = 22;
		TotalHeight = 13;

		Main.tileFrameImportant[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileNoAttach[Type] = false;
		Main.tileBlendAll[Type] = false;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Width = 22;
		TileObjectData.newTile.Height = 13;
		TileObjectData.newTile.UsesCustomCanPlace = true;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.CoordinatePadding = 2;
		TileObjectData.newTile.CoordinateWidth = 16;
		TileObjectData.newTile.CoordinateHeights = new int[13];
		Array.Fill(TileObjectData.newTile.CoordinateHeights, 16);
		TileObjectData.newTile.CoordinateHeights[^1] = 18;
		TileObjectData.newTile.LavaDeath = false;
		TileObjectData.newTile.Origin = new(11, 11);
		TileObjectData.newTile.AnchorTop = new AnchorData(0, 0, 0);
		TileObjectData.newTile.AnchorBottom = new AnchorData(0, 0, 0);
		TileObjectData.addTile(Type);

		AddMapEntry(new Color(89, 44, 20));
	}

	public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
	{
		resetFrame = false;
		return false;
	}

	public override bool CanExplode(int i, int j)
	{
		return false;
	}

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		Tile tile = Main.tile[i, j];
		if (tile.TileFrameX is >= 162 and < 234)
		{
			if (tile.TileFrameY == 90)
			{
				r = 1.6f;
				g = 1f;
				b = 0.9f;
			}
		}
	}

	public override void HitWire(int i, int j)
	{
		FurnitureUtils.LightHitwire(i, j, Type, 22, 13);
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
						for (int a = 0; a < 5; a++)
						{
							Dust dust = Dust.NewDustDirect(new Vector2(x0 + x, y0 + y) * 16, 16, 16, DustType, 0, 0, 0, default, 1);
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

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		Tile tile = Main.tile[i, j];
		var zero = new Vector2(Main.offScreenRange);
		if (Main.drawToScreen)
		{
			zero = Vector2.Zero;
		}
		var texture = ModContent.Request<Texture2D>(Texture).Value;
		if (tile.TileFrameX == 216 && tile.TileFrameY == 90)
		{
			spriteBatch.Draw(texture, new Vector2(i, j) * 16 - Main.screenPosition + zero + new Vector2(-42, -6), new Rectangle(0, 236, 52, 14), new Color(1f, 1f, 1f, 0) * 0.3f, 0, Vector2.zeroVector, 1, SpriteEffects.None, 0);
		}
		if (tile.TileFrameX % 396 == 306 && tile.TileFrameY == 90)
		{
			float hourRot = ClockItem.GetHourHandRotation();
			float minuteRot = ClockItem.GetMinuteHandRotation();
			spriteBatch.Draw(texture, new Vector2(i, j) * 16 - Main.screenPosition + zero + new Vector2(-4, 4), new Rectangle(18, 112, 10, 1), Lighting.GetColor(i, j), minuteRot, new Vector2(0, 0.5f), 1, SpriteEffects.None, 0);
			spriteBatch.Draw(texture, new Vector2(i, j) * 16 - Main.screenPosition + zero + new Vector2(-4, 4), new Rectangle(18, 112, 6, 2), Lighting.GetColor(i, j), hourRot, new Vector2(0, 1), 1, SpriteEffects.None, 0);
		}
	}

	public void AddScene(int i, int j)
	{
		var tile = Main.tile[i, j];
		if (tile.TileFrameX == 198 && tile.TileFrameY == 162)
		{
			QuestCounter_Consultant qCC = new QuestCounter_Consultant()
			{
				Active = true,
				Visible = true,
				Texture = ModAsset.QuestCounter_Consultant.Value,
				OriginTilePos = new Point(i, j),
				OriginTileType = Type,
				Direction = 1,
				Position = new Point(i, j).ToWorldCoordinates(),
			};
			Ins.VFXManager.Add(qCC);
		}
	}
}

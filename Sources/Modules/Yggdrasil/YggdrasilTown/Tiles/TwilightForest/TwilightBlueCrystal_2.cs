using Everglow.Commons.VFX.Scene;
using Everglow.Yggdrasil.YggdrasilTown.Dusts.TwilightForest;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest;

public class TwilightBlueCrystal_2 : ModTile, ISceneTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileLavaDeath[Type] = false;

		Main.tileWaterDeath[Type] = false;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Height = 3;
		TileObjectData.newTile.Width = 3;
		TileObjectData.newTile.CoordinateHeights = new int[]
		{
			16,
			16,
			18,
		};
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.LavaDeath = false;
		TileObjectData.addTile(Type);
		AddMapEntry(new Color(40, 80, 148));
		DustType = ModContent.DustType<TwilightCrystalDust>();
	}

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		r = 0.07f;
		g = 0.35f;
		b = 0.45f;
		base.ModifyLight(i, j, ref r, ref g, ref b);
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
	}

	public void AddScene(int i, int j)
	{
		Tile tile = Main.tile[i, j];
		if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
		{
			TwilightBlueCrystal_2_Mirror mirror = new TwilightBlueCrystal_2_Mirror { Position = new Vector2(i, j) * 16 + new Vector2(4, 8), Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = Type, Texture = ModAsset.TwilightBlueCrystal_2_Mirror.Value };
			Ins.VFXManager.Add(mirror);
		}
		if (tile.TileFrameX == 54 && tile.TileFrameY == 0)
		{
			TwilightBlueCrystal_2_Mirror mirror = new TwilightBlueCrystal_2_Mirror { Position = new Vector2(i, j) * 16 + new Vector2(8, 8), Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = Type, FlipH = true, Texture = ModAsset.TwilightBlueCrystal_2_Mirror_flipH.Value };
			Ins.VFXManager.Add(mirror);
		}
	}
}

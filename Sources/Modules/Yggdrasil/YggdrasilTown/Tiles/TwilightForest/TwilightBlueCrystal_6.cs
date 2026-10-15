using Everglow.Commons.TileHelper;
using Everglow.Commons.VFX.Scene;
using Everglow.Yggdrasil.YggdrasilTown.Dusts.TwilightForest;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest;

public class TwilightBlueCrystal_6 : ShapeDataTile, ISceneTile
{
	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();
		Main.tileLighted[Type] = true;
		AddMapEntry(new Color(40, 80, 148));
		DustType = ModContent.DustType<TwilightCrystalDust>();
	}
	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		r = 0.1f;
		g = 0.42f;
		b = 0.42f;
		base.ModifyLight(i, j, ref r, ref g, ref b);
	}
	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
	}
	public void AddScene(int i, int j)
	{
		Tile tile = Main.tile[i, j];
		if (tile.TileFrameX == 36 && tile.TileFrameY == 36)
		{
			TwilightBlueCrystal_6_Mirror mirror = new TwilightBlueCrystal_6_Mirror { Position = new Vector2(i, j) * 16 + new Vector2(-18, -28), Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = Type };
			Ins.VFXManager.Add(mirror);
		}
	}
}

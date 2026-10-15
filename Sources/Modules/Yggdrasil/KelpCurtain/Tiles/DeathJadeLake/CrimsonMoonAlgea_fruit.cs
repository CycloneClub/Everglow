using Everglow.Commons.TileHelper;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.KelpCurtain.Items.Materials;
using Terraria.GameContent.Drawing;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake;

public class CrimsonMoonAlgea_fruit : ModTile
{
	public override void PostSetDefaults()
	{
		Main.tileFrameImportant[Type] = false;
		Main.tileNoAttach[Type] = true;
		Main.tileCut[Type] = true;
		Main.tileLavaDeath[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Height = 1;
		TileObjectData.newTile.Width = 1;
		TileObjectData.newTile.CoordinateWidth = 16;
		TileObjectData.addTile(Type);

		DustType = ModContent.DustType<CrimsonMoonAlgea_fruitDust>();

		AddMapEntry(new Color(255, 195, 153));
		HitSound = SoundID.Grass;
	}

	public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
	{
		Tile bottomTile = TileUtils.SafeGetTile(i, j + 1);
		Tile tile = Main.tile[i, j];
		if (tile.HasTile && tile.TileType == Type)
		{
			if (!bottomTile.HasTile || (bottomTile.TileType != ModContent.TileType<CrimsonMoonAlgea>() && !Main.tileSolid[bottomTile.type] && !Main.tileSolidTop[bottomTile.type]))
			{
				WorldGen.KillTile(i, j, false, false, true);
			}
		}
		return base.TileFrame(i, j, ref resetFrame, ref noBreak);
	}

	public override bool CreateDust(int i, int j, ref int type)
	{
		for (int k = 0; k < 20; k++)
		{
			Vector2 pos = new Point(i, j).ToWorldCoordinates();
			Vector2 vel = new Vector2(0, -MathF.Sqrt(Main.rand.NextFloat())).RotatedBy(Main.rand.NextFloat(-2.2f, 2.2f)) * 12;
			Dust d = Dust.NewDustPerfect(pos + vel, ModContent.DustType<CrimsonMoonAlgea_fruitDust>());
			d.velocity = vel;
			d.noGravity = true;
		}
		return false;
	}

	public override IEnumerable<Item> GetItemDrops(int i, int j)
	{
		yield return new Item(ModContent.ItemType<CrimsonMoonSap>());
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		return false;
	}
}

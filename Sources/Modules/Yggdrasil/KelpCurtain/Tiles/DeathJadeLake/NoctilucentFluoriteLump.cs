using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Terraria.DataStructures;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake;

public class NoctilucentFluoriteLump : ModTile
{
	public override void PostSetDefaults()
	{
		Main.tileFrameImportant[Type] = false;
		Main.tileNoAttach[Type] = true;
		Main.tileCut[Type] = false;
		Main.tileLavaDeath[Type] = true;
		Main.tileLighted[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.AnchorBottom = AnchorData.Empty;

		TileObjectData.newTile.CopyFrom(TileObjectData.newTile);
		TileObjectData.newTile.Origin = Point16.Zero;
		TileObjectData.newTile.AnchorWall = true;
		TileObjectData.addTile(Type);
		DustType = ModContent.DustType<NoctilucentFluoriteLump_Dust>();

		AddMapEntry(new Color(186, 242, 244));
		HitSound = SoundID.Shatter;
	}

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		r = 0.7f;
		g = 0.95f;
		b = 0.96f;
		base.ModifyLight(i, j, ref r, ref g, ref b);
	}

	public override void NearbyEffects(int i, int j, bool closer) => base.NearbyEffects(i, j, closer);

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		Tile tile = Main.tile[i, j];
		Vector2 zero = new Vector2(Main.offScreenRange);
		if (Main.drawToScreen)
		{
			zero = Vector2.zeroVector;
		}
		Texture2D tex = ModAsset.NoctilucentFluoriteLump.Value;
		Texture2D glowTex = ModAsset.NoctilucentFluoriteLump_Glow.Value;
		spriteBatch.Draw(tex, new Point(i, j).ToWorldCoordinates() - Main.screenPosition + zero, new Rectangle(0, 0, 20, 20), new Color(1f, 1f, 1f, 1f), 0, new Vector2(10), 1f, SpriteEffects.None, 0);
		spriteBatch.Draw(glowTex, new Point(i, j).ToWorldCoordinates() - Main.screenPosition + zero, new Rectangle(0, 0, 80, 80), new Color(1f, 1f, 1f, 0), 0, new Vector2(40), 1f, SpriteEffects.None, 0);
		return false;
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		base.PostDraw(i, j, spriteBatch);
	}
}

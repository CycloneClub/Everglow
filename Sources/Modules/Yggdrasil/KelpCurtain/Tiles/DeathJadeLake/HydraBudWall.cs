using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Terraria.DataStructures;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake;

public class HydraBudWall : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileLavaDeath[Type] = true;
		Main.tileWaterDeath[Type] = false;
		Main.tileLighted[Type] = true;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Height = 1;
		TileObjectData.newTile.Width = 1;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.LavaDeath = true;
		TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
		TileObjectData.newTile.AnchorWall = true;
		TileObjectData.addTile(Type);
		DustType = ModContent.DustType<FluorescentHydraWoodDust>();
		AddMapEntry(new Color(0, 195, 255));
	}

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		r = 0.7f;
		g = 1f;
		b = 1.1f;
		base.ModifyLight(i, j, ref r, ref g, ref b);
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		Rectangle frame = new Rectangle(4, 26, 14, 12);
		switch (TileUtils.GetFixedRandomNumber(i, j, 3))
		{
			case 0:
				frame = new Rectangle(4, 26, 14, 12);
				break;
			case 1:
				frame = new Rectangle(28, 20, 22, 22);
				break;
			case 2:
				frame = new Rectangle(60, 4, 54, 48);
				break;
		}
		Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
		Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
		Vector2 pos = new Vector2(i * 16, j * 16) + zero - Main.screenPosition;
		spriteBatch.Draw(tex, pos, frame, Lighting.GetColor(i, j), TileUtils.GetFixedRandomNumber(i, j, 1024) / 1024f * MathHelper.TwoPi, frame.Size() * 0.5f, 1f, SpriteEffects.None, 0);
		return false;
	}
}

using Everglow.Commons.VFX.Scene;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake.IRProbe;

public class IRProbe_Normal : ModTile, ISceneTile
{
	public override void PostSetDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = false;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.StyleMultiplier = 5; // Since each style has 5 placement styles, we set this to 5.
		TileObjectData.newTile.AnchorBottom = AnchorData.Empty; // Clear out existing bottom anchor inherited from Style1x1 temporarily so that we don't have to set it to empty in each of the alternates.

		// To reduce code repetition, we'll use the same AnchorData value multiple times. This works because the tile is as tall as it is wide.
		var SolidOrSolidSideAnchor1TilesLong = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 1, 0);

		TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
		TileObjectData.newAlternate.Origin = Point16.Zero;
		TileObjectData.newAlternate.AnchorTop = SolidOrSolidSideAnchor1TilesLong;
		TileObjectData.addAlternate(1);

		TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
		TileObjectData.newAlternate.Origin = Point16.Zero;
		TileObjectData.newAlternate.AnchorLeft = SolidOrSolidSideAnchor1TilesLong;
		TileObjectData.addAlternate(2);

		TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
		TileObjectData.newAlternate.Origin = Point16.Zero;
		TileObjectData.newAlternate.AnchorRight = SolidOrSolidSideAnchor1TilesLong;
		TileObjectData.addAlternate(3);

		TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
		TileObjectData.newAlternate.Origin = Point16.Zero;
		TileObjectData.newAlternate.AnchorWall = true;
		TileObjectData.addAlternate(4);

		// Finally, we restore the default AnchorBottom, the extra AnchorTypes here allow placing on tables, platforms, and other tiles.
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.Table | AnchorType.SolidSide, 1, 0);
		TileObjectData.addTile(Type);
		DustType = ModContent.DustType<IRProbe_Normal_Dust>();

		AddMapEntry(new Color(99, 25, 16));
		HitSound = SoundID.Shatter;
	}

	public void AddScene(int i, int j)
	{
		var tile = Main.tile[i, j];
		int style = tile.TileFrameX / 18;
		bool noneRot = false;
		float rot = 0;
		switch (style)
		{
			case 0:
				rot = 0;
				break;
			case 1:
				rot = MathHelper.Pi;
				break;
			case 2:
				rot = MathHelper.PiOver2;
				break;
			case 3:
				rot = MathHelper.PiOver2 * 3;
				break;
			case 4:
				rot = 0;
				noneRot = true;
				break;
		}
		var laser = new IRProbe_Normal_Laser { Position = new Vector2(i, j) * 16, Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = Type };
		laser.Style = 0;
		laser.NoneRotation = noneRot;
		laser.StartRotation = rot;
		Ins.VFXManager.Add(laser);
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var tile = Main.tile[i, j];
		int style = tile.TileFrameX / 18;
		var frame = new Rectangle(24, 0, 24, 24);
		switch (style)
		{
			case 0:
				frame = new Rectangle(24, 0, 24, 24);
				break;
			case 1:
				frame = new Rectangle(24, 48, 24, 24);
				break;
			case 2:
				frame = new Rectangle(48, 24, 24, 24);
				break;
			case 3:
				frame = new Rectangle(0, 24, 24, 24);
				break;
			case 4:
				frame = new Rectangle(24, 24, 24, 24);
				break;
		}

		var zero = new Vector2(Main.offScreenRange);
		if (Main.drawToScreen)
		{
			zero = Vector2.zeroVector;
		}
		Texture2D tex = ModAsset.IRProbe_Normal.Value;
		spriteBatch.Draw(tex, new Point(i, j).ToWorldCoordinates() - Main.screenPosition + zero, frame, Lighting.GetColor(i, j), 0, frame.Size() * 0.5f, 1, SpriteEffects.None, 0);
		return false;
	}
}

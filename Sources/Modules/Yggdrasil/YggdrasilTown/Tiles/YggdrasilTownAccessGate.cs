using Everglow.Commons.VFX.Scene;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

public class YggdrasilTownAccessGate : ModTile, ISceneTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileMergeDirt[Type] = true;
		Main.tileBlockLight[Type] = false;
		AddMapEntry(Color.Black);
		MinPick = int.MaxValue;
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = 0;

	public override bool CanKillTile(int i, int j, ref bool blockDamaged) => false;

	public override bool CanExplode(int i, int j) => false;

	public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
	{
		noBreak = true;
		resetFrame = false;
		return base.TileFrame(i, j, ref resetFrame, ref noBreak);
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		Texture2D tex = ModAsset.YggdrasilTownAccessGate.Value;
		Vector2 pos = new Point(i, j).ToWorldCoordinates() + new Vector2(0, -8);
		Vector2 zero = new Vector2(Main.offScreenRange);
		if (Main.drawToScreen)
		{
			zero = Vector2.zeroVector;
		}
		pos += zero;
		Rectangle frame = new Rectangle(30, 0, 28, 6);
		spriteBatch.Draw(tex, pos - Main.screenPosition, frame, Lighting.GetColor(pos.ToTileCoordinates()), 0, new Vector2(frame.Width * 0.5f, 0), 1f, SpriteEffects.None, 0);
		return false;
	}

	public void AddScene(int i, int j)
	{
		YggdrasilTownAccessGate_VFX yTAGV = new YggdrasilTownAccessGate_VFX()
		{
			Active = true,
			Visible = true,
			Texture = ModAsset.YggdrasilTownAccessGate_VFX.Value,
			DecayTimeMax = 30,
			DecayTimer = 30,
			OriginTilePos = new Point(i, j),
			OriginTileType = Type,
			Direction = 1,
			Position = new Point(i, j).ToWorldCoordinates(),
		};
		Ins.VFXManager.Add(yTAGV);
	}
}

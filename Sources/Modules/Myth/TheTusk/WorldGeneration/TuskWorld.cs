using SubworldLibrary;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace Everglow.Myth.TheTusk.WorldGeneration;

internal class TuskWorld : Subworld
{
	public override int Width => 400;
	public override int Height => 300;
	public override bool NormalUpdates => true;
	public override bool ShouldSave => false;
	public override List<GenPass> Tasks => new List<GenPass>()
	{
		new TuskGen.SubWorldTuskLandGenPass(),
	};
	public override void DrawMenu(GameTime gameTime)
	{
		Texture2D MenuUG2SBG = ModAsset.Lightning.Value;
		Main.spriteBatch.Draw(MenuUG2SBG, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), new Rectangle(0, 0, MenuUG2SBG.Width, MenuUG2SBG.Height), Color.White);
		base.DrawMenu(gameTime);
	}
	public override void OnEnter()
	{
		SubworldSystem.hideUnderworld = true;
	}
	public override void OnLoad()
	{
		Main.worldSurface = 250;
		Main.rockLayer = 300;
		GenVars.waterLine = 260;
	}
}

using Everglow.Commons.TileHelper;
using Everglow.Commons.Utilities.BackgroundHelper;
using Everglow.Commons.VFX.Scene;
using Everglow.Yggdrasil.KelpCurtain.Background;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.KelpCurtain.VFXs;
using MonoMod.Core.Platforms;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.IsleOfBloom;

public class IsleOfBloom_CaveRay : ModTile, ISceneTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileLavaDeath[Type] = false;
		Main.tileWaterDeath[Type] = false;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Height = 1;
		TileObjectData.newTile.Width = 1;
		TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
		TileObjectData.newTile.AnchorWall = true;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.LavaDeath = false;
		TileObjectData.addTile(Type);
		AddMapEntry(new Color(36, 56, 23));
	}

	public override void NearbyEffects(int i, int j, bool closer)
	{
		base.NearbyEffects(i, j, closer);
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		return false;
	}

	public void AddScene(int i, int j)
	{
		IsleOfBloom_CaveRay_VFX ray = new IsleOfBloom_CaveRay_VFX();
		ray.Active = true;
		ray.Visible = true;
		ray.OriginTilePos = new Point(i, j);
		ray.Position = new Point(i, j).ToWorldCoordinates();
		ray.OriginTileType = Type;
		Ins.VFXManager.Add(ray);
	}
}

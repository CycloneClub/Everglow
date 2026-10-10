using Everglow.Commons.Templates.Pylon;
using Everglow.Myth.TheFirefly.Dusts;
using Terraria.GameContent;
using Terraria.Map;

namespace Everglow.Myth.TheFirefly.Pylon;

public class FireflyPylon : EverglowPylonBase<FireflyPylonTileEntity>
{
	public override int DropItemType => ModContent.ItemType<FireflyPylonItem>();

	public override void PostSetDefaults() => AddMapEntry(new Color(85, 62, 255));

	public override bool ValidTeleportCheck_NPCCount(TeleportPylonInfo pylonInfo, int defaultNecessaryNPCCount) => true;

	public override bool ValidTeleportCheck_BiomeRequirements(TeleportPylonInfo pylonInfo, SceneMetrics sceneData) => true;

	public override void ValidTeleportCheck_DestinationPostCheck(TeleportPylonInfo destinationPylonInfo, ref bool destinationPylonValid, ref string errorKey)
	{
		if (destinationPylonInfo.ModPylon is not FireflyPylon)
		{
			destinationPylonValid = false;
			errorKey = "Mods.Everglow.ErrorMessage.FireflyPylon_DestinationOnly";
		}
		base.ValidTeleportCheck_DestinationPostCheck(destinationPylonInfo, ref destinationPylonValid, ref errorKey);
	}

	public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
	{
		DrawModPylon(spriteBatch, i, j, CrystalTexture, CrystalHighlightTexture, new Vector2(0, DefaultVerticalOffset), new Color(5, 0, 55, 30), new Color(255, 0, 155, 20), CrystalVerticalFrameCount, true, ModContent.DustType<FireflyPylonDust>());
	}

	public override void DrawMapIcon(ref MapOverlayDrawContext context, ref string mouseOverText, TeleportPylonInfo pylonInfo, bool isNearPylon, Color drawColor, float deselectedScale, float selectedScale)
	{
		if (!PylonSystem.Instance.shabbyPylonEnable)
		{
			return;
		}

		base.DrawMapIcon(ref context, ref mouseOverText, pylonInfo, isNearPylon, drawColor, deselectedScale, selectedScale);
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var tile = Main.tile[i, j];
		var zero = new Vector2(Main.offScreenRange, Main.offScreenRange);

		if (Main.drawToScreen)
		{
			zero = Vector2.Zero;
		}

		Texture2D tex = ModAsset.FireflyPylonGlow.Value;

		spriteBatch.Draw(tex, new Vector2(i * 16, j * 16) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), new Color(1f, 1f, 1f, 0), 0, new Vector2(0), 1, SpriteEffects.None, 0);
	}
}

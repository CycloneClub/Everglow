using Everglow.Commons.Templates.Pylon;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Map;

namespace Everglow.Myth.TheFirefly.Pylon;

public class ShabbyPylon : EverglowPylonBase<ShabbyPylonTileEntity>
{
	public float AscensionTimer = 0;
	public const float MaxAscensionTime = 100;
	public int AnimationTimer = 0;
	public const float MaxTime = 100f;

	public override int DropItemType => ModContent.ItemType<ShabbyPylonItem>();

	public override void PostSetDefaults()
	{
		DustType = DustID.Lead;
		AddMapEntry(new Color(105, 113, 105));
	}

	public override bool ValidTeleportCheck_NPCCount(TeleportPylonInfo pylonInfo, int defaultNecessaryNPCCount) => true;

	public override bool ValidTeleportCheck_BiomeRequirements(TeleportPylonInfo pylonInfo, SceneMetrics sceneData) => true; // Vector2.Distance(Main.LocalPlayer.Center, ModContent.GetInstance<ShabbyPylonTileEntity>().Position.ToVector2() + new Vector2(24, 32)) <= 80;

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
		float offset;
		Color shadowColor;
		bool animation;
		if (PylonSystem.Instance.shabbyPylonEnable)
		{
			AscensionTimer = MathUtils.Approach(AscensionTimer, MaxAscensionTime, 1);
			float factor = AscensionTimer / MaxAscensionTime;
			offset = DefaultVerticalOffset * factor;
			shadowColor = Color.White * 0.1f * factor;
			animation = true;
		}
		else
		{
			offset = 0;
			shadowColor = Color.Transparent;
			animation = false;
		}

		DrawModPylon(spriteBatch, i, j, CrystalTexture, CrystalHighlightTexture, new Vector2(0, offset), shadowColor, Color.Gray, CrystalVerticalFrameCount, animation);
	}

	public override void DrawMapIcon(ref MapOverlayDrawContext context, ref string mouseOverText, TeleportPylonInfo pylonInfo, bool isNearPylon, Color drawColor, float deselectedScale, float selectedScale)
	{
		if (!PylonSystem.Instance.shabbyPylonEnable)
		{
			return;
		}

		if (Main.mapFullscreen && PylonSystem.Instance.firstEnableAnimation)
		{
			var firefly = TileEntity.ByPosition.FirstOrDefault(pair => pair.Value is FireflyPylonTileEntity);
			var target = firefly.Key.ToWorldCoordinates() - Main.ScreenSize.ToVector2() / 2;
			AnimationTimer++;

			// TODO 这里直接改screenPosition没用，需要一个改屏幕位置和阻止玩家操作的轮子
			Main.screenPosition = (AnimationTimer / MaxTime).Lerp(Main.screenPosition, target);
			if (AnimationTimer >= MaxTime)
			{
				PylonSystem.Instance.firstEnableAnimation = false;
			}

			return;
		}

		base.DrawMapIcon(ref context, ref mouseOverText, pylonInfo, isNearPylon, drawColor, deselectedScale, selectedScale);
	}
}

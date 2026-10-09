using System.Reflection;
using Everglow.AssetReplace.UIReplace.Core;
using Everglow.Commons.Modules;
using MonoMod.Cil;
using Terraria.GameContent;

namespace Everglow.AssetReplace;

public class ItemTooltipDrawModule : IModule
{
	public ItemTooltipDrawModule()
	{
		Code = GetType().Assembly;
	}
	public string Name => "Tooltip Drawing Modify";

	public Assembly Code { get; }

	public bool Condition => true;

	public void Load()
	{
		IL_Main.MouseText_DrawItemTooltip += PatchTooltipDrawing;
		// For "fake item"
		// Why not use "On_Main.MouseText_DrawItemTooltip"? Because that will cause a "Common Language Runtime Detected..." error
		On_Main.DrawPendingMouseText += On_Main_MouseTextInner;
	}

	private void On_Main_MouseTextInner(On_Main.orig_DrawPendingMouseText orig)
	{
		if (IsFakeItem)
		{
			var invBack13 = TextureAssets.InventoryBack13;
			TextureAssets.InventoryBack13 = UIReplaceModule.TerrariaAssets.InventoryBacks[12]; // not a typo
			orig.Invoke();
			TextureAssets.InventoryBack13 = invBack13;
			return;
		}

		orig.Invoke();
	}

	private void PatchTooltipDrawing(ILContext il)
	{
		var c = new ILCursor(il);
		if (!c.TryGotoNext(MoveType.After, i => i.MatchLdsfld<Main>(nameof(Main.SettingsEnabled_OpaqueBoxBehindTooltips))))
		{
			return;
		}

		c.EmitDelegate<Func<bool, bool>>((returnValue) =>
		{
			if (IsFakeItem)
			{
				return returnValue;
			}

			return ModContent.GetInstance<AssetReplaceConfig>().TextureReplace == TextureReplaceMode.Terraria && returnValue;
		});
	}

	// Check if the item is a "fake item". If it is, don't apply the changes
	// The "fake item" is used in "UICommon.TooltipMouseText" to draw mouse text with border
	// TODO: We should support custom text borders applied to "fake item"
	//       But "GlobalItem.PreDrawTooltip" doesn't get called for "fake item" so there is no proper solution for now
	private static bool IsFakeItem => Main.HoverItem.type is ItemID.IronPickaxe && Main.HoverItem.scale is 0 && Main.HoverItem.rare is 0 && Main.HoverItem.value is -1;

	public void Unload()
	{

	}
}

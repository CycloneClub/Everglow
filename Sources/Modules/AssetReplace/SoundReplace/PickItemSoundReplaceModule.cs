using System.Reflection;
using Everglow.Commons.Interfaces;
using Everglow.Commons.Modules;
using Microsoft.Xna.Framework.Audio;
using Terraria.Audio;

namespace Everglow.AssetReplace.SoundReplace;

internal class PickItemSoundReplaceModule : IModule
{
	public PickItemSoundReplaceModule()
	{
		Code = GetType().Assembly;
	}

	public string Name => "Pick Item Sound Modify";

	public Assembly Code { get; }

	public bool Condition => true;

	public void Load()
	{
		_playOriginalSound = true;
		Terraria.UI.On_ItemSlot.LeftClick_ItemArray_int_int += PatchLeftClick;
		Terraria.UI.On_ItemSlot.RightClick_ItemArray_int_int += PatchRightClick;
		On_SoundEngine.PlaySound_int_int_int_int_float_float += PatchLegacyIDPlaySound;
	}

	private void PatchRightClick(Terraria.UI.On_ItemSlot.orig_RightClick_ItemArray_int_int orig, Item[] inv, int context, int slot)
	{
		_playOriginalSound = true;
		if (ModContent.GetInstance<AssetReplaceConfig>().ItemPickSoundReplace && Main.mouseRight && Main.stackSplit <= 1)
		{
			SoundStyle? customSoundStyle = null;

			IModifyItemPickSound.Invoke(inv[slot], context, false, ref customSoundStyle, ref _playOriginalSound);

			if (customSoundStyle.HasValue)
			{
				SoundEngine.PlaySound(customSoundStyle.Value);
			}

			orig.Invoke(inv, context, slot);
			return;
		}
		orig.Invoke(inv, context, slot);
	}

	private void PatchLeftClick(Terraria.UI.On_ItemSlot.orig_LeftClick_ItemArray_int_int orig, Item[] inv, int context, int slot)
	{
		_playOriginalSound = true;
		if (ModContent.GetInstance<AssetReplaceConfig>().ItemPickSoundReplace && Main.mouseLeft && Main.mouseLeftRelease)
		{
			SoundStyle? customSoundStyle = null;

			if (!Main.mouseItem.IsAir)
			{
				IModifyItemPickSound.Invoke(Main.mouseItem, context, true, ref customSoundStyle, ref _playOriginalSound);
			}
			else if (!inv[slot].IsAir)
			{
				IModifyItemPickSound.Invoke(inv[slot], context, false, ref customSoundStyle, ref _playOriginalSound);
			}

			if (customSoundStyle.HasValue)
			{
				SoundEngine.PlaySound(customSoundStyle.Value);
			}

			orig.Invoke(inv, context, slot);
			return;
		}
		orig.Invoke(inv, context, slot);
	}

	private SoundEffectInstance PatchLegacyIDPlaySound(On_SoundEngine.orig_PlaySound_int_int_int_int_float_float orig, int type, int x, int y, int Style, float volumeScale, float pitchOffset)
	{
		if (_playOriginalSound)
		{
			return orig.Invoke(type, x, y, Style, volumeScale, pitchOffset);
		}

		return null;
	}

	private static bool _playOriginalSound = false;

	public void Unload()
	{
	}
}

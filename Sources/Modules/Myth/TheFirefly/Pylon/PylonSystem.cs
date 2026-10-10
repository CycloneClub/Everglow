using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace Everglow.Myth.TheFirefly.Pylon;

internal class PylonSystem : ModSystem
{
	public static PylonSystem Instance => ModContent.GetInstance<PylonSystem>();

	public const string DataName = "pylon";
	public bool shabbyPylonEnable = false;
	public bool firstEnableAnimation = false;

	public override void SaveWorldData(TagCompound tag)
	{
		tag[DataName] = (byte)new BitsByte(shabbyPylonEnable, firstEnableAnimation);
	}

	public override void LoadWorldData(TagCompound tag)
	{
		BitsByte bits = tag.GetByte(DataName);
		shabbyPylonEnable = bits[0];
		firstEnableAnimation = bits[1];
	}

	public override void NetSend(BinaryWriter writer)
	{
		writer.Write(shabbyPylonEnable);
	}

	public override void NetReceive(BinaryReader reader)
	{
		shabbyPylonEnable = reader.ReadBoolean();
	}
}

internal class PylonValidCheck : GlobalPylon
{
	public override void PostValidTeleportCheck(TeleportPylonInfo destinationPylonInfo, TeleportPylonInfo nearbyPylonInfo, ref bool destinationPylonValid, ref bool validNearbyPylonFound, ref string errorKey)
	{
		if (nearbyPylonInfo.ModPylon is FireflyPylon or ShabbyPylon)
		{
			if (!PylonSystem.Instance.shabbyPylonEnable)
			{
				errorKey = Language.GetTextValue("Mods.Everglow.Common.NotActivated");
				return;
			}

			if (destinationPylonInfo.ModPylon is FireflyPylon or ShabbyPylon)
			{
				validNearbyPylonFound = true;
				destinationPylonValid = true;
			}
			else
			{
				errorKey = Language.GetTextValue("Mods.Everglow.PylonSystem.FireflyPylonLimit");
				destinationPylonValid = false;
			}
		}
	}
}

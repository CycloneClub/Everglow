using Everglow.Commons.Netcode.Abstracts;
using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.YggdrasilTown.Items.PermanentBoosters;

namespace Everglow.Yggdrasil.Netcode;

public class PermanentBoostPacket : IPacket
{
	public int playerId = -1;
	public int consumedAntiHeavenSicknessPill = 0;
	public int consumedJadeGlazeFruit = 0;
	public int consumedLampBorerHoney = 0;
	public int consumedSquamousCore = 0;
	public int consumedMagicOrganExtract = 0;
	public int consumedActiveMagicSpringPowder = 0;

	public void Receive(BinaryReader reader, int whoAmI)
	{
		playerId = reader.ReadInt32();
		consumedAntiHeavenSicknessPill = reader.ReadInt32();
		consumedJadeGlazeFruit = reader.ReadInt32();
		consumedLampBorerHoney = reader.ReadInt32();
		consumedSquamousCore = reader.ReadInt32();
		consumedMagicOrganExtract = reader.ReadInt32();
		consumedActiveMagicSpringPowder = reader.ReadInt32();
	}

	public void Send(BinaryWriter writer)
	{
		writer.Write(playerId);
		writer.Write(consumedAntiHeavenSicknessPill);
		writer.Write(consumedJadeGlazeFruit);
		writer.Write(consumedLampBorerHoney);
		writer.Write(consumedSquamousCore);
		writer.Write(consumedMagicOrganExtract);
		writer.Write(consumedActiveMagicSpringPowder);
	}

	[HandlePacket(typeof(PermanentBoostPacket))]
	public class PermanentBoostPacketHandler : IPacketHandler
	{
		public void Handle(IPacket packet, int whoAmI)
		{
			var packetData = (PermanentBoostPacket)packet;
			int playerIndex = Main.netMode == NetmodeID.Server ? whoAmI : packetData.playerId;
			if (playerIndex < 0 || playerIndex >= Main.maxPlayers)
			{
				return;
			}
			var mp = Main.player[playerIndex].GetModPlayer<YggdrasilPlayer>();

			mp.ConsumedAntiHeavenSicknessPill = packetData.consumedAntiHeavenSicknessPill;
			mp.ConsumedJadeGlazeFruit = packetData.consumedJadeGlazeFruit;
			mp.ConsumedLampBorerHoney = packetData.consumedLampBorerHoney;
			mp.ConsumedSquamousCore = packetData.consumedSquamousCore;
			mp.ConsumedMagicOrganExtract = Math.Clamp(packetData.consumedMagicOrganExtract, 0, MagicOrganExtract.MaxUse);
			mp.ConsumedActiveMagicSpringPowder = Math.Clamp(packetData.consumedActiveMagicSpringPowder, 0, ActiveMagicSpringPowder.MaxUse);

			if (Main.netMode == NetmodeID.Server)
			{
				mp.SyncPlayer(toWho: -1, fromWho: playerIndex, newPlayer: false);
			}
		}
	}
}

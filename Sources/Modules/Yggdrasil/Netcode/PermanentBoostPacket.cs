using Everglow.Commons.Netcode.Abstracts;
using Everglow.Yggdrasil.Common;

namespace Everglow.Yggdrasil.Netcode;

public class PermanentBoostPacket : IPacket
{
	public int consumedAntiHeavenSicknessPill = 0;
	public int consumedJadeGlazeFruit = 0;
	public int consumedLampBorerHoney = 0;
	public int consumedSquamousCore = 0;

	public void Receive(BinaryReader reader, int whoAmI)
	{
		consumedAntiHeavenSicknessPill = reader.ReadInt32();
		consumedJadeGlazeFruit = reader.ReadInt32();
		consumedLampBorerHoney = reader.ReadInt32();
		consumedSquamousCore = reader.ReadInt32();
	}

	public void Send(BinaryWriter writer)
	{
		writer.Write(consumedAntiHeavenSicknessPill);
		writer.Write(consumedJadeGlazeFruit);
		writer.Write(consumedLampBorerHoney);
		writer.Write(consumedSquamousCore);
	}

	[HandlePacket(typeof(PermanentBoostPacket))]
	public class PermanentBoostPacketHandler : IPacketHandler
	{
		public void Handle(IPacket packet, int whoAmI)
		{
			var packetData = (PermanentBoostPacket)packet;
			var mp = Main.player[whoAmI].GetModPlayer<YggdrasilPlayer>();

			mp.ConsumedAntiHeavenSicknessPill = packetData.consumedAntiHeavenSicknessPill;
			mp.ConsumedJadeGlazeFruit = packetData.consumedJadeGlazeFruit;
			mp.ConsumedLampBorerHoney = packetData.consumedLampBorerHoney;
			mp.ConsumedSquamousCore = packetData.consumedSquamousCore;
		}
	}
}

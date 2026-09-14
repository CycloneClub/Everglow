using Everglow.Commons.Netcode.Abstracts;
using Everglow.Yggdrasil.KelpCurtain.NPCs.VampireMat;

namespace Everglow.Yggdrasil.Netcode;

public class VampireMatHitPacket : IPacket
{
	private int damage;

	public VampireMatHitPacket(int damage)
	{
		this.damage = damage;
	}

	public void Receive(BinaryReader reader, int whoAmI)
	{
		damage = reader.ReadInt32();
	}

	public void Send(BinaryWriter writer)
	{
		writer.Write(damage);
	}

	[HandlePacket(typeof(VampireMatHitPacket))]
	public class VampireMatHitPacketHandler : IPacketHandler
	{
		public void Handle(IPacket packet, int whoAmI)
		{
			var packetData = (VampireMatHitPacket)packet;
			VampireMat.VampireHitCommonEffect(Main.player[whoAmI], packetData.damage);
		}
	}
}

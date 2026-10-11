using Everglow.Commons.Netcode.Abstracts;
using Everglow.Yggdrasil.YggdrasilTown.Tiles;

namespace Everglow.Yggdrasil.Netcode;

public class SquamousShellSummonPacket : IPacket
{
	public int TileX;
	public int TileY;
	public bool Spawned;

	public void Send(BinaryWriter writer)
	{
		writer.Write(TileX);
		writer.Write(TileY);
		writer.Write(Spawned);
	}

	public void Receive(BinaryReader reader, int whoAmI)
	{
		TileX = reader.ReadInt32();
		TileY = reader.ReadInt32();
		Spawned = reader.ReadBoolean();
	}

	[HandlePacket(typeof(SquamousShellSummonPacket))]
	public class Handler : IPacketHandler
	{
		public void Handle(IPacket packet, int whoAmI)
		{
			var request = (SquamousShellSummonPacket)packet;
			if (!WorldGen.InWorld(request.TileX, request.TileY))
			{
				return;
			}
			var seal = ModContent.GetInstance<SquamousShellSeal>();
			if (Main.netMode == NetmodeID.Server && !request.Spawned)
			{
				if (seal.TrySummon(request.TileX, request.TileY, whoAmI))
				{
					request.Spawned = true;
					ModIns.PacketResolver.Send(request);
				}
			}
			else if (Main.netMode == NetmodeID.MultiplayerClient && request.Spawned)
			{
				seal.ApplyPostSummonEffect();
			}
		}
	}
}

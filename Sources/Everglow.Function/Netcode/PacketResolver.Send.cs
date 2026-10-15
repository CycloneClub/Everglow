using Everglow.Commons.Netcode.Abstracts;
using Everglow.Commons.Utilities;
using SubworldLibrary;

namespace Everglow.Commons.Netcode;

public partial class PacketResolver
{
	private ModPacket GetPacket()
	{
		return _mod.GetPacket();
	}

	private void Send(IPacket packet, RouteDestination destination, int toClient = -1, int ignoreClient = -1)
	{
		if (NetUtils.IsSingle)
		{
			return;
		}

		var sourcePlayer = NetUtils.IsServer ? ignoreClient : Main.myPlayer;
		var data = SerializePacket(packet, destination, sourcePlayer);

		var modPacket = GetPacket();
		modPacket.Write(data);
		modPacket.Send(toClient, ignoreClient);
	}

	/// <summary>
	/// Sends a packet instance to the specified targets.
	/// </summary>
	/// <param name="packet">The packet to send.</param>
	/// <param name="toClient">Target client ID, or -1 for all clients.</param>
	/// <param name="ignoreClient">Client ID to ignore, or -1 to ignore none.</param>
	public void Send(IPacket packet, int toClient = -1, int ignoreClient = -1)
	{
		Send(packet, RouteDestination.WorldOnly, toClient, ignoreClient);
	}

	/// <summary>
	/// Sends a packet with automatic target determination based on the fromServer flag.
	/// <br/> Wrapper around <see cref="Send(IPacket, int, int)"/>.
	/// </summary>
	/// <param name="packet">The packet to send.</param>
	/// <param name="fromServer">If true, sends to all clients except the specified player. If false, sends to all clients.</param>
	/// <param name="player">The player to potentially ignore when fromServer is true.</param>
	public void Send(IPacket packet, bool fromServer, Player player)
	{
		if (fromServer)
		{
			Send(packet, -1, player.whoAmI);
		}
		else
		{
			Send(packet);
		}
	}

	/// <summary>
	/// Routes a packet according to the specified destination.
	/// </summary>
	public void Route(IPacket packet, RouteDestination destination)
	{
		if (NetUtils.IsSingle)
		{
			return;
		}

		Debug.Assert(destination is not RouteDestination.WorldOnly, "Use Send() to send world only packets.");

		switch (destination)
		{
			case RouteDestination.MainServer:
				{
					if (NetUtils.IsMainClient)
					{
						// Main client -> Main world
						Send(packet, RouteDestination.MainServer);
					}
					else if (NetUtils.IsSubServer)
					{
						// Sub world -> Main world
						var data = SerializePacket(packet, RouteDestination.MainServer, -1);
						SubworldSystem.SendToMainServer(_mod, data);
					}
					else if (NetUtils.IsSubClient)
					{
						// Sub client -> Sub world -> Main world
						Send(packet, RouteDestination.MainServer);
					}
				}
				break;
			case RouteDestination.AllDownstream:
				{
					Debug.Assert(NetUtils.IsMainServer, "All downstream can only be sent from main world server!");

					// Send to main clients
					Send(packet, RouteDestination.AllDownstream);

					// Send to all sub servers
					var data = SerializePacket(packet, RouteDestination.AllDownstream, -1);
					SubworldSystem.SendToAllSubservers(_mod, data);
				}
				break;
		}
	}
}

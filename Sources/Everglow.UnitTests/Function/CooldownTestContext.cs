using System.Reflection;
using System.Runtime.CompilerServices;
using Everglow.Commons;
using Everglow.Commons.Mechanics.Cooldown;
using Everglow.Commons.Netcode;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Everglow.UnitTests.Function;

internal sealed class CooldownTestContext : IDisposable
{
	private readonly List<Action> restore = [];
	private readonly List<ModPlayer> players = [];

	public Player Player { get; }
	public EverglowPlayer Cooldowns { get; }

	public CooldownTestContext()
	{
		Program.SavePath = string.Empty;
		int netMode = Main.netMode;
		bool dedicatedServer = Main.dedServ;
		var resolver = ModIns.PacketResolver;
		restore.Add(() => { Main.netMode = netMode; Main.dedServ = dedicatedServer; ModIns.PacketResolver = resolver; });
		Main.netMode = NetmodeID.SinglePlayer;
		Main.dedServ = false;
		// Single-player Send returns before accessing the networking dependencies.
		ModIns.PacketResolver = (PacketResolver)RuntimeHelpers.GetUninitializedObject(typeof(PacketResolver));
		foreach (string name in new[] { "registry", "nameToNetID", "nameToType", "nextNetID" })
		{
			var field = typeof(CooldownRegistry).GetField(name, BindingFlags.NonPublic | BindingFlags.Static)!;
			object? value = field.GetValue(null);
			restore.Add(() => field.SetValue(null, value));
		}
		new CooldownRegistry().Load();
		Player = new Player { active = true };
		Cooldowns = Attach<EverglowPlayer>();
	}

	public T Attach<T>()
		where T : ModPlayer, new()
	{
		var content = typeof(ContentInstance<T>).GetProperty("Instance")!;
		object? previous = content.GetValue(null);
		restore.Add(() => content.SetValue(null, previous));
		var modPlayer = (T)new T().NewInstance(Player);
		typeof(ModPlayer).GetProperty("Index")!.SetValue(modPlayer, (ushort)players.Count);
		players.Add(modPlayer);
		content.SetValue(null, modPlayer);
		typeof(Player).GetField("modPlayers", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(Player, players.ToArray());
		return modPlayer;
	}

	public void Register(string id, Type type)
	{
		var types = (Dictionary<string, Type>)typeof(CooldownRegistry).GetField("nameToType", BindingFlags.NonPublic | BindingFlags.Static)!.GetValue(null)!;
		if (types.TryAdd(id, type))
		{
			CooldownRegistry.Register(id);
		}
	}

	public void Dispose()
	{
		for (int i = restore.Count - 1; i >= 0; i--)
		{
			restore[i]();
		}
	}
}

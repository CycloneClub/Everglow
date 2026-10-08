using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Events;

public class EventSystem : ModSystem
{
	private static readonly List<ModEvent> actives = [];

	private static readonly Dictionary<string, ModEvent> registry = new(StringComparer.Ordinal);

	public override void Load()
	{
		if (!Main.dedServ)
		{
			On_Main.DrawInvasionProgress += DrawInvasionProgress;
		}
	}

	private static void DrawInvasionProgress(On_Main.orig_DrawInvasionProgress orig)
	{
		if (!DrawInvasionProgress_Everglow())
		{
			orig();
		}
	}

	private static bool DrawInvasionProgress_Everglow()
	{
		foreach (ModEvent e in actives)
		{
			if (e.IsBackground)
			{
				continue;
			}
			e.Draw(Main.spriteBatch);
			return true;
		}

		return false;
	}

	private static void ReSortActives()
	{
		actives.Sort((e1, e2) => -e1.SortRank.CompareTo(e2.SortRank));
	}

	internal static void Register(ModEvent e)
	{
		ModTypeLookup<ModEvent>.Register(e);
		registry.Add(e.FullName, e);
	}

	public static bool Activate(ModEvent e, params object[] args)
	{
		if (!e.Active && CanChange(e) && e.CanActivate(args))
		{
			actives.Add(e);
			ReSortActives();
			e.Active = true;
			e.OnActivate(args);
			if (e.Networked && Main.netMode == NetmodeID.Server)
			{
				NetMessage.SendData(MessageID.WorldData);
			}
			return true;
		}
		return false;
	}

	public static bool Activate<T>(params object[] args)
		where T : ModEvent
	{
		ModEvent e;
		return (e = ModContent.GetInstance<T>()) is not null && Activate(e, args);
	}

	public static bool Deactivate(ModEvent e, params object[] args)
	{
		if (CanChange(e) && e.CanDeactivate(args))
		{
			if (actives.Remove(e))
			{
				e.Active = false;
				e.OnDeactivate(args);
				if (e.Networked && Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.WorldData);
				}
				return true;
			}
			return false;
		}
		return false;
	}

	public static bool Deactivate<T>(params object[] args)
		where T : ModEvent
	{
		ModEvent e;
		return (e = ModContent.GetInstance<T>()) is not null && Deactivate(e, args);
	}

	private static bool CanChange(ModEvent e) => !e.Networked
		|| (Main.netMode != NetmodeID.MultiplayerClient
			&& registry.TryGetValue(e.FullName, out var registeredEvent) && ReferenceEquals(e, registeredEvent));

	internal static void NotifyNPCKilled(NPC npc)
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}
		foreach (ModEvent e in actives.ToArray())
		{
			if (e.Active)
			{
				e.OnNPCKilled(npc);
			}
		}
	}

	public override void PostUpdateEverything()
	{
		foreach (ModEvent e in actives.ToArray())
		{
			if (!e.Active)
			{
				continue;
			}
			if (e.Networked && Main.netMode == NetmodeID.MultiplayerClient)
			{
				e.PostUpdateEverythingClient();
			}
			else
			{
				e.PostUpdateEverything();
			}
		}
	}

	public override void ClearWorld()
	{
		foreach (ModEvent e in registry.Values.Concat(actives).Distinct())
		{
			e.ClearWorld();
		}
		actives.Clear();
	}

	public override void OnWorldUnload() => ClearWorld();

	public override void Unload()
	{
		ClearWorld();
		registry.Clear();
	}

	public override void SaveWorldData(TagCompound tag)
	{
		foreach (ModEvent e in registry.Values)
		{
			TagCompound subtag = new();
			e.SaveWorldData(subtag);
			tag[e.FullName] = subtag;
		}
		int Count = actives.Count;
		tag[nameof(Count)] = Count;
		for (int i = 0; i < Count; i++)
		{
			ModEvent e = actives[i];
			tag[$"{nameof(actives)}_{i}.FullName"] = e.FullName;
			tag[$"{nameof(actives)}_{i}.DefName"] = e.DefName;
			TagCompound subtag = new();
			if (registry.TryGetValue(e.FullName, out ModEvent oe) && oe != e)
			{
				e.SaveWorldData(subtag);
			}
			tag[$"{nameof(actives)}_{i}.Tag"] = subtag;
		}
	}

	public override void LoadWorldData(TagCompound tag)
	{
		ClearWorld();
		foreach (ModEvent e in registry.Values)
		{
			if (tag.TryGet(e.FullName, out TagCompound subtag))
			{
				e.LoadWorldData(e.FullName, subtag);
			}
		}
		int Count;
		if (tag.TryGet(nameof(Count), out Count) && Count > 0)
		{
			for (int i = 0; i < Count; i++)
			{
				if (tag.TryGet($"{nameof(actives)}_{i}.FullName", out string fullName) && tag.TryGet($"{nameof(actives)}_{i}.DefName", out string defName))
				{
					if (registry.TryGetValue(fullName, out ModEvent e))
					{
						if (e.FullName != defName)
						{
							if (e.Networked)
							{
								continue;
							}

							e = e.Clone();
							tag.TryGet($"{nameof(actives)}_{i}.Tag", out TagCompound subtag);
							subtag ??= new();
							e.LoadWorldData(defName, subtag);
						}
						e.Active = true;
						actives.Add(e);
					}
				}
			}
			ReSortActives();
		}
	}

	public override void NetSend(BinaryWriter writer)
	{
		var events = registry.Values.Where(e => e.Networked).ToArray();
		writer.Write(events.Length);
		foreach (ModEvent e in events)
		{
			writer.Write(e.FullName);
			writer.Write(e.Active);
			using var stream = new MemoryStream();
			using var eventWriter = new BinaryWriter(stream);
			e.NetSend(eventWriter);
			byte[] data = stream.ToArray();
			writer.Write(data.Length);
			writer.Write(data);
		}
	}

	public override void NetReceive(BinaryReader reader)
	{
		int count = reader.ReadInt32();
		if (count < 0 || count > 1024)
		{
			throw new InvalidDataException("Invalid event count.");
		}
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			foreach (ModEvent e in registry.Values.Where(e => e.Networked))
			{
				actives.Remove(e);
				e.ClearWorld();
			}
		}
		for (int i = 0; i < count; i++)
		{
			string fullName = reader.ReadString();
			bool active = reader.ReadBoolean();
			int length = reader.ReadInt32();
			if (length < 0 || length > 32768)
			{
				throw new InvalidDataException("Invalid event payload length.");
			}
			byte[] data = reader.ReadBytes(length);
			if (data.Length != length)
			{
				throw new EndOfStreamException();
			}
			if (Main.netMode != NetmodeID.MultiplayerClient
				|| !registry.TryGetValue(fullName, out var e) || !e.Networked)
			{
				continue;
			}
			using var stream = new MemoryStream(data);
			using var eventReader = new BinaryReader(stream);
			e.NetReceive(eventReader);
			e.Active = active;
			if (e.Active)
			{
				actives.Add(e);
			}
		}
		ReSortActives();
	}
}

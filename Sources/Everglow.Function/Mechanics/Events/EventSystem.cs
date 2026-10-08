using Everglow.Commons.Utilities;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace Everglow.Commons.Mechanics.Events;

public class EventSystem : ModSystem
{
	private const string LayerName = "Everglow/ModEvent";
	private const string VanillaInvasionLayerName = "Vanilla: Invasion Progress Bars";
	private const string VanillaMinimapLayerName = "Vanilla: Map / Minimap";

	private static List<ModEvent> actives = [];

	private static LegacyGameInterfaceLayer layer_HasInvasion = new(LayerName, delegate
	{
		if (!DrawInvasionProgress_Everglow())
		{
			DrawInvasionProgress_Vanilla();
		}
		if (Main.HealthBarDrawSettings != 0)
		{
			Main.BigBossProgressBar.Draw(Main.spriteBatch);
		}
		return true;
	});

	private static LegacyGameInterfaceLayer layer_NoInvasion = new(LayerName, delegate
	{
		DrawInvasionProgress_Everglow();

		return true;
	});

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

	private static void DrawInvasionProgress_Vanilla()
	{
		var sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin();
		Main.DrawInvasionProgress();
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
	}

	private static void ReSortActives()
	{
		actives.Sort((e1, e2) => -e1.SortRank.CompareTo(e2.SortRank));
	}

	internal static void Register(ModEvent e)
	{
		ModTypeLookup<ModEvent>.Register(e);
	}

	public static bool Activate(ModEvent e, params object[] args)
	{
		if (e.CanActivate(args))
		{
			actives.Add(e);
			ReSortActives();
			e.Active = true;
			e.OnActivate(args);
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
		if (e.CanDeactivate(args))
		{
			if (actives.Remove(e))
			{
				e.Active = false;
				e.OnDeactivate(args);
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

	public override void PostUpdateEverything()
	{
		foreach (ModEvent e in actives)
		{
			e.PostUpdateEverything();
		}
	}

	public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
	{
		int index = layers.FindIndex(layer => layer.Name == VanillaInvasionLayerName);
		if (index != -1)
		{
			layers.RemoveAt(index);
			layers.Insert(index, layer_HasInvasion);
			return;
		}
		index = layers.FindIndex(layer => layer.Name == VanillaMinimapLayerName);
		if (index != -1)
		{
			layers.Insert(index, layer_NoInvasion);
		}
	}

	public override void SaveWorldData(TagCompound tag)
	{
		foreach (ModEvent e in ModContent.GetContent<ModEvent>())
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
			if (ModContent.TryFind(e.FullName, out ModEvent oe) && oe != e)
			{
				e.SaveWorldData(subtag);
			}
			tag[$"{nameof(actives)}_{i}.Tag"] = subtag;
		}
	}

	public override void LoadWorldData(TagCompound tag)
	{
		foreach (ModEvent e in ModContent.GetContent<ModEvent>())
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
					if (ModContent.TryFind(fullName, out ModEvent e))
					{
						if (e.FullName != defName)
						{
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
}

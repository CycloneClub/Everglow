using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Events;

public abstract class ModEvent : ModType
{
	public sealed override void Register()
	{
		EventSystem.Register(this);
	}

	public float SortRank = 1;

	public bool Active { get; internal set; }

	public virtual string DefName => FullName;

	public virtual bool IsBackground => false;

	/// <summary>
	/// Opts a registered, single-instance event into current-world server authority.
	/// Legacy events keep their existing update and activation behavior until migrated.
	/// TODO: Remove Networked once legacy events, including multi-instance events, use server authority and world synchronization.
	/// </summary>
	public virtual bool Networked => false;

	public virtual void PostUpdateEverythingClient()
	{
	}

	public virtual void NetSend(BinaryWriter writer)
	{
	}

	public virtual void NetReceive(BinaryReader reader)
	{
	}

	/// <summary>Clear world state without running gameplay completion hooks.</summary>
	public virtual void ClearWorld()
	{
		Active = false;
	}

	public virtual void PostUpdateEverything()
	{
	}

	public virtual bool CanActivate(params object[] args)
	{
		return !Active;
	}

	public virtual bool CanDeactivate(params object[] args)
	{
		return Active;
	}

	public virtual void OnActivate(params object[] args)
	{
	}

	public virtual void OnDeactivate(params object[] args)
	{
	}

	/// <summary>Called for NPC kills on single player or the server while this event is active.</summary>
	public virtual void OnNPCKilled(NPC npc)
	{
	}

	public virtual void Draw(SpriteBatch sprite)
	{
	}

	public virtual void SaveWorldData(TagCompound tag)
	{
	}

	public virtual void LoadWorldData(string defName, TagCompound tag)
	{
	}

	public virtual ModEvent Clone()
	{
		return (ModEvent)MemberwiseClone();
	}
}

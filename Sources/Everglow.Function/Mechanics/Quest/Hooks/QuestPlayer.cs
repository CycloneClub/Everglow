using Everglow.Commons.Mechanics.Quest.PlayerSide;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;
using Everglow.Commons.Utilities;
using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Quest.Hooks;

public class QuestPlayer : ModPlayer
{
	/// <summary>
	/// Called on local client only.
	/// </summary>
	public static event Action<Item> OnPickupEvent;

	/// <summary>
	/// Called on local client only.
	/// </summary>
	public static event Action<Player, Item> OnDropSelectedItemEvent;

	private PlayerQuestManagerData questManagerData;

	/// <summary>
	/// Indicate to apply player quest info into <see cref="PlayerQuestManager"/>. Defaults to <c>false</c>.
	/// <br/>Set to <c>true</c> after data applying, to <c>false</c> after player loading.
	/// </summary>
	private bool questManagerDataInitialized = false;

	public override void Load()
	{
		On_Player.DropSelectedItem_int_refItem += On_Player_DropSelectedItem_int_refItem;
	}

	public override void OnEnterWorld()
	{
		if (!questManagerDataInitialized) // Prevent load being called when OnEnterWorld is called by subworldlibrary
		{
			PlayerQuestManager.Instance.ApplyData(questManagerData);
			questManagerDataInitialized = true;
		}
	}

	public override void SaveData(TagCompound tag)
	{
		PlayerQuestManager.Instance.SaveData(tag);
	}

	public override void LoadData(TagCompound tag)
	{
		questManagerData = PlayerQuestManager.Instance.LoadData(tag);
		questManagerDataInitialized = false;
	}

	public override bool OnPickup(Item item)
	{
		OnPickupEvent?.Invoke(item);
		return true;
	}

	private void On_Player_DropSelectedItem_int_refItem(On_Player.orig_DropSelectedItem_int_refItem orig, Player self, int slot, ref Item theItemWeDrop)
	{
		orig(self, slot, ref theItemWeDrop);
		OnDropSelectedItemEvent?.Invoke(self, theItemWeDrop);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		if (!NetUtils.IsServer && Player.whoAmI == Main.myPlayer && !target.active)
		{
			QuestGlobalNPC.TriggerOnKillNPCEvent(target);
		}
	}
}

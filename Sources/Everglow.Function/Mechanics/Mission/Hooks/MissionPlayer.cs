using Everglow.Commons.Mechanics.Mission.PlayerSide;
using Everglow.Commons.Mechanics.Mission.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Mission.PlayerSide;
using Everglow.Commons.Mechanics.Mission.PlayerSide.Tests;
using Everglow.Commons.Utilities;
using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Mission.Hooks;

public class MissionPlayer : ModPlayer
{
	/// <summary>
	/// Called on local client only.
	/// </summary>
	public static event Action<Item> OnPickupEvent;

	/// <summary>
	/// Called on local client only.
	/// </summary>
	public static event Action<Player, Item> OnDropSelectedItemEvent;

	private PlayerMissionManagerData missionManagerData;

	/// <summary>
	/// Indicate to apply player mission info into <see cref="PlayerMissionManager"/>. Defaults to <c>false</c>.
	/// <br/>Set to <c>true</c> after data applying, to <c>false</c> after player loading.
	/// </summary>
	private bool missionManagerDataInitialized = false;

	public override void Load()
	{
		On_Player.DropSelectedItem_int_refItem += On_Player_DropSelectedItem_int_refItem;
	}

	public override void OnEnterWorld()
	{
		if (!missionManagerDataInitialized) // Prevent load being called when OnEnterWorld is called by subworldlibrary
		{
			PlayerMissionManager.ApplyData(missionManagerData);
			missionManagerDataInitialized = true;
		}

#if DEBUG
		if (!PlayerMissionManager.HasMission<PlayerMissionBase>())
		{
			PlayerMissionManager.AddMission(new KillNPCMissionTest(), PlayerMissionState.Available);
			PlayerMissionManager.AddMission(new ParallelMissionTest(), PlayerMissionState.Available);
			PlayerMissionManager.AddMission(new MissionObjectivesTest(), PlayerMissionState.Available);
			PlayerMissionManager.AddMission(new OpenPanelMissionTest(), PlayerMissionState.Available);
			PlayerMissionManager.AddMission(new BranchingMissionTest(), PlayerMissionState.Available);
			PlayerMissionManager.AddMission(new NoneMission1(), PlayerMissionState.Available);
			PlayerMissionManager.AddMission(new NoneMission2(), PlayerMissionState.Available);
			PlayerMissionManager.AddMission(new NoneMission3(), PlayerMissionState.Available);
			PlayerMissionManager.AddMission(new NoneMission4(), PlayerMissionState.Available);
			PlayerMissionManager.AddMission(new NoneMission5(), PlayerMissionState.Available);
			PlayerMissionManager.AddMission(new NoneMission6(), PlayerMissionState.Available);
			PlayerMissionManager.AddMission(new MissionTimerTest(), PlayerMissionState.Available);
			PlayerMissionManager.AddMission(new MissionIconTest(), PlayerMissionState.Available);
			PlayerMissionManager.AddMission(new GiveItemMissionTest(), PlayerMissionState.Available);
			PlayerMissionManager.AddMission(new ExploreMissionTest(), PlayerMissionState.Available);
		}
#endif
	}

	public override void SaveData(TagCompound tag)
	{
		PlayerMissionManager.SaveData(tag);
	}

	public override void LoadData(TagCompound tag)
	{
		missionManagerData = PlayerMissionManager.LoadData(tag);
		missionManagerDataInitialized = false;
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
			MissionGlobalNPC.TriggerOnKillNPCEvent(target);
		}
	}
}

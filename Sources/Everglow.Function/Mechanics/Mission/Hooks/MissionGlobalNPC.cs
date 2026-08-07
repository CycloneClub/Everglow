using Everglow.Commons.Mechanics.Mission.PlayerSide;
using Everglow.Commons.Mechanics.Mission.PlayerSide;
using Everglow.Commons.Mechanics.Mission.PlayerSide.Objectives;
using Everglow.Commons.Mechanics.Mission.WorldSide;
using Everglow.Commons.Mechanics.Mission.WorldSide.Objectives;

namespace Everglow.Commons.Mechanics.Mission.Hooks;

public class MissionGlobalNPC : GlobalNPC
{
	public static event Action<NPC> OnKillNPCEvent;

	public static event Action<NPC> OnNPCKilled;

	public static void TriggerOnKillNPCEvent(NPC npc)
	{
		OnKillNPCEvent.Invoke(npc);
	}

	public override bool SpecialOnKill(NPC npc)
	{
		OnNPCKilled?.Invoke(npc);

		return base.SpecialOnKill(npc);
	}

	public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		var killTypes = GetKillNPCTypes();
		if (killTypes.Contains(npc.type))
		{
			Texture2D tex = ModAsset.MissionExclamationMark.Value;
			float scale = (1f + (float)Math.Sin(Main.timeForVisualEffects * 0.24f) * 0.14f) * 0.16f;
			Color color = new Color(1f, 0.7f, 0.5f, 1f);
			spriteBatch.Draw(tex, new Vector2(npc.Center.X, npc.Center.Y - 36) - Main.screenPosition, null, color, 0f, tex.Size() / 2, scale, SpriteEffects.None, 0f);
		}

		var talkTypes = GetTalkNPCTypes();
		if (talkTypes.Contains(npc.type))
		{
			Texture2D tex = ModAsset.MissionExclamationMark.Value;
			float scale = (1f + (float)Math.Sin(Main.timeForVisualEffects * 0.24f) * 0.14f) * 0.16f;
			Color color = new Color(0.5f, 0.8f, 1f, 1f);
			spriteBatch.Draw(tex, new Vector2(npc.Center.X, npc.Center.Y - 36) - Main.screenPosition, null, color, 0f, tex.Size() / 2, scale, SpriteEffects.None, 0f);
		}
	}

	public static IEnumerable<int> GetTalkNPCTypes()
	{
		var missions = PlayerMissionManager.GetMissionPool(PlayerMissionState.Accepted);

		var playerSideNPCs = missions
			.SelectMany(mission => mission.Objectives.ActiveObjectives)
			.Select(o =>
			{
				if (o is TalkNPCObjective talkObjective)
				{
					return talkObjective.NPCType;
				}
				else if (o is GiveItemObjective giveObjective)
				{
					return giveObjective.NPCType;
				}
				else
				{
					return NPCID.None;
				}
			});

		var worldSideNPCs = WorldMissionManager.Instance.ActiveMissions
			.SelectMany(m => m.ActiveObjectives)
			.Select(o =>
			{
				if (o is WorldTalkObjective talkObjective)
				{
					return talkObjective.NPCType;
				}
				else if (o is WorldGiveObjective giveObjective)
				{
					return giveObjective.NPCType;
				}
				else
				{
					return NPCID.None;
				}
			});
		return playerSideNPCs.Concat(worldSideNPCs).Distinct();
	}

	public static IEnumerable<int> GetKillNPCTypes()
	{
		var missions = PlayerMissionManager.GetMissionPool(PlayerMissionState.Accepted);

		var playerSideNPCs = missions
			.SelectMany(mission => mission.Objectives.ActiveObjectives)
			.OfType<KillNPCObjective>()
			.SelectMany(killObjective => killObjective.NPCTypes);

		var worldSideNPCs = WorldMissionManager.Instance.ActiveMissions
			.SelectMany(m => m.ActiveObjectives)
			.Select(o =>
			{
				if (o is WorldKillNPCObjective killObjective)
				{
					return killObjective.NPCType;
				}
				else
				{
					return NPCID.None;
				}
			}).Distinct();

		return playerSideNPCs.Concat(worldSideNPCs).Distinct();
	}

}

using System.Reflection;
using Everglow.Commons.Mechanics.Miscs;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria.DataStructures;

namespace Everglow.Commons.Mechanics;

public class EverglowGlobalNPC : GlobalNPC
{
	public override bool InstancePerEntity => true;

	internal int ExpectedDotDamage { get; set; }

	public override void ResetEffects(NPC npc)
	{
		ExpectedDotDamage = 0;
	}

	private static void NPC_ScaleStats_ByDifficulty(On_NPC.orig_ScaleStats_ByDifficulty orig, NPC npc)
	{
		orig(npc);
		// This hook also covers Journey and continuous difficulty below Expert.
		Type type = npc.ModNPC?.GetType();
		if (type != null && type.GetCustomAttribute<NoGameModeScaleAttribute>() != null)
		{
			NPCID.Sets.DontDoHardmodeScaling[npc.type] = true;
			npc.lifeMax = (int)(npc.lifeMax / GameDifficultyData.EnemyMaxLifeMultiplier.Sample(npc.difficulty));
			npc.damage = (int)(npc.damage / GameDifficultyData.EnemyDamageMultiplier.Sample(npc.difficulty));
			// 1.4.5 no longer applies a generic difficulty multiplier to defense.
			npc.value = (int)(npc.value / GameDifficultyData.EnemyMoneyDropMultiplier.Sample(npc.difficulty));
			npc.knockBackResist /= GameDifficultyData.KnockbackToEnemiesMultiplier.Sample(npc.difficulty);
		}
	}

	public override void Load()
	{
		On_NPC.ScaleStats_ByDifficulty += NPC_ScaleStats_ByDifficulty;
		IL_NPC.UpdateNPC_BuffApplyDOTs += MergeExpectedDotDamage;
	}

	public override void Unload()
	{
		On_NPC.ScaleStats_ByDifficulty -= NPC_ScaleStats_ByDifficulty;
		IL_NPC.UpdateNPC_BuffApplyDOTs -= MergeExpectedDotDamage;
	}

	private delegate void MergeDotTally(NPC npc, ref NPC.DOTTally tally);

	private static void MergeExpectedDotDamage(ILContext il)
	{
		// Vanilla now batches DOT damage itself. Merge our threshold into its tally after mod hooks.
		var tally = il.Body.Variables.FirstOrDefault(variable => variable.VariableType.FullName == "Terraria.NPC/DOTTally");
		var cursor = new ILCursor(il);
		if (tally is null || !cursor.TryGotoNext(MoveType.After, instruction => instruction.MatchCall(typeof(NPCLoader), nameof(NPCLoader.UpdateLifeRegen))))
		{
			throw new InvalidOperationException("Cannot find the 1.4.5 NPC DOT tally/update hook. Check the tModLoader version.");
		}
		cursor.Emit(OpCodes.Ldarg_0);
		cursor.Emit(OpCodes.Ldloca, tally);
		cursor.EmitDelegate<MergeDotTally>((NPC npc, ref NPC.DOTTally dotTally) =>
		{
			dotTally.ExpectedDamagePerSecond = Math.Max(dotTally.ExpectedDamagePerSecond, npc.GetGlobalNPC<EverglowGlobalNPC>().ExpectedDotDamage);
		});
	}
}

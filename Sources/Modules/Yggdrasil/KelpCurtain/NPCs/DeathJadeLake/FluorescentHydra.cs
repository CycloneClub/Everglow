using Everglow.Yggdrasil.Common;
using SubworldLibrary;

namespace Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake;

/// <summary>
/// 荧光水螅（Fluorescent Hydra），亡碧湖（Death Jade Lake）的荧光水螅体。
/// Design row: 荧光水螅, heading block id CvAGdz87Son7TQxKZY8c9Dt5nke.
/// The committed design snapshot's section for this row carries a heading and nothing else - no stats
/// table, no behaviour text and no drop - so this class is an identity-only shell under D-45: it loads,
/// registers, is layer-gated and carries a precise blocker instead of invented behaviour. Nothing in it
/// is design-derived except the row's name and its region.
/// Approved artwork is missing from the repository, so the class requests the shared
/// Commons.ModAsset.White_Mod fallback (D-48). The DeathJadeLake region subfolder is what tML's default
/// (namespace-derived) texture resolution expects, so when the art arrives the D-49 migration is adding
/// FluorescentHydra.png beside this .cs and deleting the Texture override; the runtime asset path is
/// Everglow/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/FluorescentHydra.png. The row's artwork and
/// behaviour blockers live in 03-BIOLOGY.json and are consolidated by plan 04-09.
/// The pre-existing Dusts/FluorescentHydraWoodDust is an unrelated wood-tile dust and is NOT this
/// creature's dust.
/// </summary>
public class FluorescentHydra : ModNPC
{
	// No HJSON key is created for this class; localization stays deferred (D-20).
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.NPCs;

	// Approved artwork is missing from the repository (D-48). The shared fallback keeps the class
	// loadable, and the beside-.png the phase's gate asserts arrives with the D-49 migration.
	public override string Texture => Commons.ModAsset.White_Mod;

	public override void SetStaticDefaults()
	{
		// No approved sprite exists, so there is nothing to slice: the whole texture is one frame.
		Main.npcFrameCount[NPC.type] = 1;
		NPCSpawnManager.RegisterNPC(Type);
	}

	public override void SetDefaults()
	{
		// D-45/D-54: every value below is this phase's conservative default pending the design. The
		// design row carries a heading only, so it supplies no hit box, no 生命, no 伤害, no 防御 and no
		// 钱币 cell, and no behaviour is invented from the row's name.
		//
		// The engine's own aiStyle 0 is the vanilla "No AI" style: the creature does not move and only
		// faces the player, so this shell needs no AI() body of its own.
		NPC.aiStyle = 0;

		// No sprite exists to measure, so a conservative mid-sized sessile-polyp box is used.
		NPC.width = 32;
		NPC.height = 40;

		NPC.lifeMax = 60;
		NPC.life = 60;
		NPC.damage = 20;
		NPC.defense = 4;

		// The design supplies no 钱币 cell, so no bounty is invented.
		NPC.value = 0;

		// The design supplies no 稀有度 cell. NPC.rarity is the engine's only NPC rarity field and its
		// own default is White, so the empty cell is recorded here (the Phase 3 NPC.rare -> NPC.rarity
		// correction, 04-DEVIATIONS.md section 10).
		NPC.rarity = ItemRarityID.White;

		// The engine's own default contact and death sounds are kept as the conservative default.
		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;

		// The design claims no 可被捕捉 text for this row, so no catch item is set and no critter flag is
		// registered for this creature (D-58).
		NPC.catchItem = 0;

		SpawnModBiomes = [ModContent.GetInstance<KelpCurtainBiome>().Type];
	}

	/// <summary>
	/// 荧光水螅 spawns only inside Yggdrasil's Kelp Curtain layer (BIO-06).
	/// <c>NPCSpawnManager.EditSpawnPool</c> returns early outside the subworld, so this per-creature gate
	/// is the real isolation, and <see cref="KelpCurtainBiome.IsKelpCurtainLayer"/> is the server-safe
	/// layer predicate because the hook runs in single player or on the server only, where the client
	/// camera is zero (D-52/D-55).
	/// The design says nothing about land or water for this row, so the shell returns the neutral water
	/// weight band and invents no positional condition (D-45/D-54).
	/// </summary>
	/// <param name="spawnInfo">The engine's spawn context.</param>
	/// <returns>The conservative water weight, or <c>0f</c> outside the design's context.</returns>
	public override float SpawnChance(NPCSpawnInfo spawnInfo)
	{
		if (!SubworldSystem.IsActive<YggdrasilWorld>() || !KelpCurtainBiome.IsKelpCurtainLayer(spawnInfo.Player))
		{
			return 0f;
		}

		return KelpCurtainSpawnConditions.WaterWeight;
	}
}

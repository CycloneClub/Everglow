using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.KelpCurtain.Items.Weapons;
using SubworldLibrary;
using Terraria.GameContent.ItemDropRules;

namespace Everglow.Yggdrasil.KelpCurtain.NPCs.SpinyMossCourt;

/// <summary>
/// 枯木活化士兵（犬）（Animated Witherbark Hound）——刺苔庭园（Spiny Moss Court）王庭的冲撞兽。
/// Design row: 枯木活化士兵, region h1 block YCYwdeYPEomoMVxotFOcE8gZnUh (刺苔庭园),
/// heading block id S4Jadlq0colTtbx8UagceDb7nCq, stats table NNe6dSQqxoxXllxT9PvcVmApn1c,
/// behaviour block ids BYhvdnXwrogK3Jx1z35cwgS7nhg / MGYedPrhioUFFdxP5XrcMpnxnMd,
/// drop block id KzhDde2Q7o3Te6xUDPsclRNqnCW.
/// <para>
/// This is a <b>sibling class</b> of the row whose <c>internal_name</c> is <c>AnimatedWitherbarkSoldier</c>
/// (the 近战 variant): the design gives 普通（犬）its own stat row (生命 75 / 伤害 20 / 防御 2 / 击退抗性 70 /
/// 免疫 中毒 / 1银20铜) and its own drop, and <c>SetDefaults</c> runs once per NPC type, so a stat variant must
/// never be faked by mutating <c>NPC.lifeMax</c> at runtime (OQ2, 04-DEVIATIONS.md section 2).
/// </para>
/// <para>
/// 犬类敌怪会高速来回冲撞玩家: the hound alternates a high-speed charge with a short recovery, turning around
/// after every charge so it comes back at the prey from the other side. Its 中立 default and its provocation
/// reading are the same as its siblings'.
/// </para>
/// <para>
/// Approved artwork is missing from the repository, so the class requests the shared
/// <c>Commons.ModAsset.White_Mod</c> fallback. The <c>SpinyMossCourt</c> region subfolder is what tML's
/// default (namespace-derived) texture resolution expects, so the D-49 migration is adding
/// <c>AnimatedWitherbarkHound.png</c> beside this .cs and deleting the <c>Texture</c> override.
/// </para>
/// <para>
/// D-46 BLOCKER - the Spiny Moss Court morale/command system does not exist, so the design's 意志高涨 bonus
/// (防御力+4, 攻击+35%, 移速+15%) and the 干涸心脏 morale-gated drop are NOT implemented. The morale seam is the
/// named members below, which a later phase raises without reworking this AI (04-DEVIATIONS.md sections 7, 13).
/// </para>
/// </summary>
public class AnimatedWitherbarkHound : ModNPC
{
	/// <summary>
	/// 在默认情况下是中立状态: the proximity half of the provocation reading. The design names no number, so a
	/// conservative courtyard-guard radius is used (D-54 default, 04-DEVIATIONS.md section 10).
	/// </summary>
	private const float ProvokeRange = 10f * 16f;

	/// <summary>漫无目的地游荡: the slow patrol pace used while the creature is still 中立 (D-54 default).</summary>
	private const float WanderSpeed = 1.1f;

	/// <summary>
	/// 高速来回冲撞玩家: the charge speed. The design's 速度 cell reads 快 for this variant, so the charge is the
	/// fastest movement of the four variants (D-54 default).
	/// </summary>
	private const float ChargeSpeed = 7.5f;

	/// <summary>
	/// How long a charge lasts before the hound breaks off, so 来回 turns into a real back-and-forth rather than
	/// one endless dash (D-54 default).
	/// </summary>
	private const int ChargeFrames = 45;

	/// <summary>The recovery window between two charges, during which the hound turns around (D-54 default).</summary>
	private const int RecoverFrames = 60;

	/// <summary>The slow pace of the recovery window (D-54 default).</summary>
	private const float RecoverSpeed = 1.4f;

	/// <summary>The distance at which the provoked state falls back to 中立 (D-54 default).</summary>
	private const float LeashRange = 70f * 16f;

	/// <summary>
	/// The three states of the design's charge: the 中立 patrol, the 高速冲锋, and the short recovery in which the
	/// hound turns around for the next pass.
	/// </summary>
	private enum AnimatedWitherbarkHoundState
	{
		Patrolling = 0,
		Charging = 1,
		Recovering = 2,
	}

	/// <summary>Named wrapper over <c>NPC.ai[0]</c> so no bare numeric index is scattered through the class.</summary>
	private AnimatedWitherbarkHoundState State
	{
		get => (AnimatedWitherbarkHoundState)(int)NPC.ai[0];
		set => NPC.ai[0] = (int)value;
	}

	/// <summary>Named wrapper over <c>NPC.localAI[0]</c>: the remaining frames of the current charge/recovery.</summary>
	private int StateTimer
	{
		get => (int)NPC.localAI[0];
		set => NPC.localAI[0] = value;
	}

	/// <summary>
	/// The neutral flag, kept in a named wrapper over <c>NPC.localAI[1]</c> (1 once provoked, 0 while the
	/// design's 中立 default still holds).
	/// </summary>
	private int ProvokedFlag
	{
		get => (int)NPC.localAI[1];
		set => NPC.localAI[1] = value;
	}

	/// <summary>
	/// 意志高涨 防御力+4: the morale/command seam (D-46). The system that would raise this does not exist, so
	/// it stays at its neutral value (0) and a later phase raises it without reworking this AI.
	/// </summary>
	public float MoraleDefenseBonus;

	/// <summary>意志高涨 攻击+35%: the damage multiplier a later morale system sets. The neutral value is 1.</summary>
	public float MoraleDamageScale = 1f;

	/// <summary>意志高涨 移速+15%: the speed multiplier a later morale system sets. The neutral value is 1.</summary>
	public float MoraleSpeedScale = 1f;

	/// <summary>
	/// A named neutral-state member so a later morale/command system can flip the creature and read the
	/// design's 中立 default (D-46).
	/// </summary>
	public bool IsNeutral => ProvokedFlag == 0;

	// No HJSON key is created for this class; localization stays deferred (D-20).
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.NPCs;

	// Approved artwork is missing from the repository (D-48). The shared fallback keeps the class loadable,
	// and the beside-.png this class's region folder expects arrives with the D-49 migration.
	public override string Texture => Commons.ModAsset.White_Mod;

	public override void SetStaticDefaults()
	{
		// No approved sprite exists, so there is nothing to slice: the whole texture is one frame.
		Main.npcFrameCount[NPC.type] = 1;
		NPCSpawnManager.RegisterNPC(Type);

		// 免疫 中毒 (the design row's 免疫 cell for this variant): the poisoned debuff is the only immunity.
		NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
	}

	public override void SetDefaults()
	{
		// The design's charge-and-recover cadence has no vanilla aiStyle analogue that can stay neutral, so the
		// class owns its AI() (the documented case where a local AI() is warranted, D-31).
		NPC.aiStyle = -1;

		// No sprite exists to measure, so a conservative quadruped hound box is used (D-54 default).
		NPC.width = 40;
		NPC.height = 26;

		NPC.lifeMax = 75; // 生命 75
		NPC.life = 75;
		NPC.damage = 20; // 伤害 20
		NPC.defDamage = 20;
		NPC.defense = 2; // 防御 2
		NPC.defDefense = 2;
		NPC.knockBackResist = 0.3f; // 击退抗性 70 -> 1 - 0.70
		NPC.value = 120; // 钱币（铜） 1银20铜 = 120 copper, a 1:1 transpose (NPC.value is copper)

		// Conservative default: the design's 稀有度 cell is empty and 普通 is its 类型 cell, so no design
		// rarity exists. NPC.rarity is the engine's only NPC rarity field and defaults to White, so this
		// records the empty cell (the Phase 3 NPC.rare -> NPC.rarity correction, section 10).
		NPC.rarity = ItemRarityID.White;

		// A hound is a ground quadruped: it keeps the engine's gravity and tile collision.
		NPC.noGravity = false;
		NPC.noTileCollide = false;

		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;

		// 枯木活化士兵 is not 可被捕捉, so no catch item, no Main.npcCatchable and no CountsAsCritter.
		NPC.catchItem = 0;

		// Deterministic starting values, because SetDefaults runs on every side: no Main.rand is consumed here,
		// so the server and its clients agree before the first sync arrives.
		StateTimer = 0;
		ProvokedFlag = 0;

		SpawnModBiomes = [ModContent.GetInstance<KelpCurtainBiome>().Type];
	}

	/// <summary>
	/// The provoked-stat re-assertion and the morale seam. 意志高涨 (D-46) is a blocker, so the morale members
	/// keep their neutral values and this is a no-op today; it exists so a later morale/command system can raise
	/// defence and damage without this class being reworked, and so the defence is restored from the bases on
	/// every side (<c>NPC.defense</c> is not part of the NPC net message, the <c>MossyThornTurtle</c> precedent).
	/// </summary>
	public override void PostAI()
	{
		NPC.defense = NPC.defDefense + (int)MoraleDefenseBonus;
		NPC.damage = (int)(NPC.defDamage * MoraleDamageScale);
	}

	/// <summary>
	/// The design's charge cadence, and the 中立 default: while unprovoked the hound only patrols, so it never
	/// charges before the design's own 中立 is broken.
	/// </summary>
	public override void AI()
	{
		if (IsNeutral)
		{
			UpdatePatrolling();
			UpdateFacing();
			return;
		}

		switch (State)
		{
			case AnimatedWitherbarkHoundState.Charging:
				UpdateCharging();
				break;
			case AnimatedWitherbarkHoundState.Recovering:
				UpdateRecovering();
				break;
			default:
				UpdateSeeking();
				break;
		}

		UpdateFacing();
	}

	/// <summary>
	/// The design row's 减伤 cell is empty (stats table NNe6dSQqxoxXllxT9PvcVmApn1c), so no
	/// <c>FinalDamage</c> scaling is applied here. The omission is the design's, not the plan's.
	/// </summary>
	/// <param name="modifiers">The hit modifiers being resolved.</param>
	public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
	{
	}

	/// <summary>
	/// Damage taken is the primary provocation reading, so a non-lethal hit breaks the design's 中立. The
	/// transition is written on the authoritative side only, with an <c>NPC.netUpdate</c> (D-55).
	/// </summary>
	/// <param name="hit">The resolved hit.</param>
	public override void HitEffect(NPC.HitInfo hit)
	{
		if (NPC.life <= 0)
		{
			// Death dust only, and only on a client: the dedicated server has no graphics access (D-35/D-55).
			if (!Main.dedServ)
			{
				for (int i = 0; i < 6; i++)
				{
					int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<WitherWoodDust>(), 2 * hit.HitDirection, -2f);
					if (Main.rand.NextBool())
					{
						Main.dust[dust].noGravity = true;
						Main.dust[dust].scale = 1.2f * NPC.scale;
					}
					else
					{
						Main.dust[dust].scale = 0.7f * NPC.scale;
					}
				}
			}

			return;
		}

		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			ProvokedFlag = 1;
			State = AnimatedWitherbarkHoundState.Charging;
			StateTimer = ChargeFrames;
			NPC.netUpdate = true;
		}
	}

	/// <summary>
	/// The idle patrol of the design's 中立 default: a slow walk that reverses at a wall or a ledge, with no
	/// player targeting at all until the design's 中立 is broken.
	/// </summary>
	private void UpdatePatrolling()
	{
		if (TryGetPlayer(out _, out float distance) && distance <= ProvokeRange)
		{
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				ProvokedFlag = 1;
				State = AnimatedWitherbarkHoundState.Charging;
				StateTimer = ChargeFrames;
				NPC.netUpdate = true;
			}

			return;
		}

		NPC.velocity.X = WanderSpeed * NPC.direction;

		if (NPC.collideX || !HasFloorAhead())
		{
			NPC.direction *= -1;
			NPC.velocity.X = WanderSpeed * NPC.direction;
		}
	}

	/// <summary>
	/// The provoked state's opening step: pick the charge heading toward the prey once, then hand over to
	/// <see cref="UpdateCharging"/>.
	/// </summary>
	private void UpdateSeeking()
	{
		if (!TryGetPlayer(out Player target, out float distance) || distance > LeashRange)
		{
			EnterNeutral();
			return;
		}

		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		NPC.direction = target.Center.X >= NPC.Center.X ? 1 : -1;
		State = AnimatedWitherbarkHoundState.Charging;
		StateTimer = ChargeFrames;
		NPC.netUpdate = true;
	}

	/// <summary>
	/// 高速来回冲撞玩家: the high-speed dash. The heading is committed when the charge begins (so the hound runs
	/// past the prey instead of homing mid-dash), and the dash ends against a wall, on a charge timer, or when the
	/// prey leaves the leash.
	/// </summary>
	private void UpdateCharging()
	{
		if (!TryGetPlayer(out _, out float distance) || distance > LeashRange)
		{
			EnterNeutral();
			return;
		}

		NPC.velocity.X = ChargeSpeed * MoraleSpeedScale * NPC.direction;
		if (NPC.velocity.Y == 0f && NPC.collideX)
		{
			NPC.velocity.Y = -5.5f;
		}

		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		StateTimer--;
		if (StateTimer <= 0 || NPC.collideX)
		{
			State = AnimatedWitherbarkHoundState.Recovering;
			StateTimer = RecoverFrames;
			NPC.netUpdate = true;
		}
	}

	/// <summary>
	/// The recovery between two dashes: the hound slows, turns around (which is what makes the attack 来回), and
	/// goes back to the seek step.
	/// </summary>
	private void UpdateRecovering()
	{
		NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, RecoverSpeed * MoraleSpeedScale * NPC.direction, 0.12f);

		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		StateTimer--;
		if (StateTimer <= 0)
		{
			NPC.direction *= -1;
			State = AnimatedWitherbarkHoundState.Patrolling;
			StateTimer = 0;
			NPC.netUpdate = true;
		}
	}

	/// <summary>True when the tile just below and ahead of the leading edge is solid, or the world edge is reached.</summary>
	/// <returns>True when the creature can keep walking.</returns>
	private bool HasFloorAhead()
	{
		int checkTileX = (int)((NPC.Center.X + (NPC.width / 2f + 8f) * NPC.direction) / 16f);
		int checkTileY = (int)((NPC.Bottom.Y + 8f) / 16f);
		if (!WorldGen.InWorld(checkTileX, checkTileY, 1))
		{
			return true;
		}

		return Main.tile[checkTileX, checkTileY].HasTile;
	}

	/// <summary>The nearest live player, used only by the provocation test and the charge.</summary>
	/// <param name="player">The player the creature is aware of, or <c>null</c>.</param>
	/// <param name="distance">The distance to that player, or <see cref="float.MaxValue"/>.</param>
	/// <returns>True when a live player exists.</returns>
	private bool TryGetPlayer(out Player player, out float distance)
	{
		player = null;
		distance = float.MaxValue;
		NPC.TargetClosest(false);
		if (NPC.target < 0 || NPC.target >= Main.maxPlayers)
		{
			return false;
		}

		Player candidate = Main.player[NPC.target];
		if (candidate is null || !candidate.active || candidate.dead)
		{
			return false;
		}

		player = candidate;
		distance = Vector2.Distance(NPC.Center, candidate.Center);
		return true;
	}

	/// <summary>Keeps the facing in step with the direction of travel (the arriving art reads this).</summary>
	private void UpdateFacing()
	{
		if (MathF.Abs(NPC.velocity.X) > 0.05f)
		{
			NPC.direction = NPC.velocity.X > 0f ? 1 : -1;
		}

		NPC.spriteDirection = NPC.direction;
	}

	/// <summary>The return to 中立 once the prey is out of leash, written authoritatively (D-55).</summary>
	private void EnterNeutral()
	{
		if (Main.netMode == NetmodeID.MultiplayerClient || IsNeutral)
		{
			return;
		}

		ProvokedFlag = 0;
		State = AnimatedWitherbarkHoundState.Patrolling;
		StateTimer = 0;
		NPC.netUpdate = true;
	}

	/// <summary>
	/// The courtyard unit's land spawn gate. The design gives no per-variant weight and makes the 近战 variant the
	/// most common one, so this variant returns the phase's conservative land weight. Only ever inside Yggdrasil
	/// and inside the Kelp Curtain layer (BIO-06): <c>NPCSpawnManager.EditSpawnPool</c> returns early outside the
	/// subworld, so this per-creature gate is the real isolation, and
	/// <see cref="KelpCurtainBiome.IsKelpCurtainLayer"/> is the server-safe layer predicate because the hook runs
	/// in single player or on the server only, where the client camera is zero (D-52/D-55). The design's 刺苔庭园
	/// is an above-water courtyard, so the land condition is its own 地表 reading (Pitfall 6).
	/// </summary>
	/// <param name="spawnInfo">The engine's spawn context.</param>
	/// <returns>The conservative land weight, or <c>0f</c> outside the design's context.</returns>
	public override float SpawnChance(NPCSpawnInfo spawnInfo)
	{
		if (!SubworldSystem.IsActive<YggdrasilWorld>() || !KelpCurtainBiome.IsKelpCurtainLayer(spawnInfo.Player))
		{
			return 0f;
		}

		if (!KelpCurtainSpawnConditions.IsDryLand(spawnInfo))
		{
			return 0f;
		}

		return KelpCurtainSpawnConditions.LandWeight;
	}

	/// <summary>
	/// 犬类掉落1枯木碎块，9%（1/11）概率掉落1 活化之犬召唤杖: this is the only variant of the four that wires a drop,
	/// and the 9% reciprocal is the denominator 11. <c>ActivatedDogStaff</c> was implemented in Phase 1 (D-57), so
	/// the type resolves. The design's other drops for this row - 枯木碎块 (1 for the hound, 1~2 for the others)
	/// and the morale-gated 干涸心脏 - have no ModItem in the repository, so no rule is written and no item type is
	/// referenced for them (D-58, Pitfall 2); the blockers live in 04-DEVIATIONS.md section 6 and in the biology
	/// matrix.
	/// </summary>
	/// <param name="npcLoot">The loot table being built.</param>
	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
		npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ActivatedDogStaff>(), 11, 1, 1));
	}
}

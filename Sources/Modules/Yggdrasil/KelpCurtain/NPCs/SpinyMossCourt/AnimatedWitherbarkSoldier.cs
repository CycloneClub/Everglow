using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using SubworldLibrary;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;

namespace Everglow.Yggdrasil.KelpCurtain.NPCs.SpinyMossCourt;

/// <summary>
/// 枯木活化士兵（Animated Witherbark Soldier）——刺苔庭园（Spiny Moss Court）王庭的基础近战单位。
/// Design row: 枯木活化士兵, region h1 block YCYwdeYPEomoMVxotFOcE8gZnUh (刺苔庭园),
/// heading block id S4Jadlq0colTtbx8UagceDb7nCq, stats table NNe6dSQqxoxXllxT9PvcVmApn1c,
/// behaviour block ids BYhvdnXwrogK3Jx1z35cwgS7nhg / MGYedPrhioUFFdxP5XrcMpnxnMd,
/// drop block id KzhDde2Q7o3Te6xUDPsclRNqnCW.
/// <para>
/// This class is the row's own <c>internal_name</c>: it implements the 普通（近战）stat row
/// (生命 80 / 伤害 22 / 防御 4 / 击退抗性 20 / 免疫 中毒 / 2银). The three sibling stat rows are the separate
/// classes <c>AnimatedWitherbarkSoldierRanged</c> (远程), <c>AnimatedWitherbarkSoldierSpell</c> (法术) and
/// <c>AnimatedWitherbarkHound</c> (犬), because <c>SetDefaults</c> runs once per NPC type and a stat variant
/// must never be faked by mutating <c>NPC.lifeMax</c> at runtime (OQ2, 04-DEVIATIONS.md section 2).
/// </para>
/// <para>
/// 庭院的基础敌怪单位, 在默认情况下是中立状态: the design's 中立 default is a real state here - the
/// creature keeps its own patrol until it is provoked by damage taken or by a player entering
/// <see cref="ProvokeRange"/>. The design's 近战敌怪会像原版的地牢骷髅一样尝试近战 is mirrored by the local
/// fighter AI below (walk toward the prey, jump at a wall/ledge or when the prey is above, deal contact
/// damage). The vanilla fighter <c>aiStyle</c> is deliberately NOT cloned: it always acquires a player
/// target, which would make the design's neutral default inexpressible - the approach is what is mirrored,
/// not the engine's targeting. See 04-DEVIATIONS.md section 10 for the recorded defaults.
/// </para>
/// <para>
/// Approved artwork is missing from the repository, so the class requests the shared
/// <c>Commons.ModAsset.White_Mod</c> fallback. The <c>SpinyMossCourt</c> region subfolder is what tML's
/// default (namespace-derived) texture resolution expects, so the D-49 migration is adding
/// <c>AnimatedWitherbarkSoldier.png</c> beside this .cs and deleting the <c>Texture</c> override; the runtime
/// asset path is Everglow/Yggdrasil/KelpCurtain/NPCs/SpinyMossCourt/AnimatedWitherbarkSoldier.png.
/// </para>
/// <para>
/// D-46 BLOCKER - the Spiny Moss Court morale/command system does not exist: the design's 意志高涨 bonus
/// (防御力+4, 攻击+35%, 移速+15%), the 干涸心脏 morale-gated drop and the break-on-commander-death rule are
/// NOT implemented. The morale seam is exposed as the named members below so a later phase raises defence,
/// damage and speed without reworking this AI (04-DEVIATIONS.md sections 7 and 13).
/// </para>
/// </summary>
public class AnimatedWitherbarkSoldier : ModNPC
{
	/// <summary>
	/// 在默认情况下是中立状态: the proximity half of the provocation reading. The design names no number,
	/// so a conservative courtyard-guard radius is used (D-54 default, 04-DEVIATIONS.md section 10).
	/// </summary>
	private const float ProvokeRange = 10f * 16f;

	/// <summary>漫无目的地游荡: the slow patrol pace used while the creature is still 中立 (D-54 default).</summary>
	private const float WanderSpeed = 0.9f;

	/// <summary>
	/// 近战敌怪会像原版的地牢骷髅一样尝试近战: the chase pace of the provoked state, above the patrol
	/// pace. The design's 速度 cell reads 较快 for this variant (D-54 default).
	/// </summary>
	private const float ChaseSpeed = 2.2f;

	/// <summary>The wall/ledge/above-prey jump, mirroring the vanilla dungeon skeleton (D-54 default).</summary>
	private const float JumpSpeed = 6.4f;

	/// <summary>
	/// The distance at which the provoked state falls back to 中立, so a player who leaves cannot leave the
	/// whole courtyard permanently aggroed (D-54 default).
	/// </summary>
	private const float LeashRange = 60f * 16f;

	/// <summary>How often the jump may fire, so the fighter AI does not jitter against a wall (D-54 default).</summary>
	private const int JumpCooldownFrames = 45;

	/// <summary>
	/// The two states of the design's 中立 unit: the default patrol and the provoked melee chase.
	/// </summary>
	private enum AnimatedWitherbarkSoldierState
	{
		Neutral = 0,
		Hostile = 1,
	}

	/// <summary>Named wrapper over <c>NPC.ai[0]</c> so no bare numeric index is scattered through the class.</summary>
	private AnimatedWitherbarkSoldierState State
	{
		get => (AnimatedWitherbarkSoldierState)(int)NPC.ai[0];
		set => NPC.ai[0] = (int)value;
	}

	/// <summary>Named wrapper over <c>NPC.localAI[0]</c>: the frames left before the next jump.</summary>
	private int JumpCooldown
	{
		get => (int)NPC.localAI[0];
		set => NPC.localAI[0] = value;
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
	/// A named neutral-state member (not a bare <c>NPC.ai[0]</c> read) so a later morale/command system can
	/// flip the creature and read the design's 中立 default (D-46).
	/// </summary>
	public bool IsNeutral => State == AnimatedWitherbarkSoldierState.Neutral;

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

		// 免疫 中毒 (the design row's 免疫 cell): the poisoned debuff is the only immunity of this variant.
		NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
	}

	public override void SetDefaults()
	{
		// The design's neutral-patrol-then-melee-chase behaviour has no vanilla aiStyle analogue that can
		// stay neutral, so the class owns its AI() (the documented case where a local AI() is warranted, D-31).
		NPC.aiStyle = -1;

		// No sprite exists to measure, so a conservative humanoid soldier box is used (D-54 default).
		NPC.width = 30;
		NPC.height = 46;

		NPC.lifeMax = 80; // 生命 80
		NPC.life = 80;
		NPC.damage = 22; // 伤害 22
		NPC.defDamage = 22;
		NPC.defense = 4; // 防御 4
		NPC.defDefense = 4;
		NPC.knockBackResist = 0.8f; // 击退抗性 20 -> 1 - 0.20
		NPC.value = 200; // 钱币（铜） 2银 = 200 copper, a 1:1 transpose (NPC.value is copper)

		// Conservative default: the design's 稀有度 cell is empty and 普通 is its 类型 cell, so no design
		// rarity exists. NPC.rarity is the engine's only NPC rarity field and defaults to White, so this
		// records the empty cell (the Phase 3 NPC.rare -> NPC.rarity correction, section 10).
		NPC.rarity = ItemRarityID.White;

		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;

		// 枯木活化士兵 is not 可被捕捉, so no catch item, no Main.npcCatchable and no CountsAsCritter.
		NPC.catchItem = 0;

		// Deterministic starting timer, because SetDefaults runs on every side: no Main.rand is consumed here,
		// so the server and its clients agree before the first sync arrives.
		JumpCooldown = 0;

		SpawnModBiomes = [ModContent.GetInstance<KelpCurtainBiome>().Type];
	}

	/// <summary>
	/// The patrol heading is drawn once on the authoritative side so every side begins from the same value
	/// (the engine's own NPC message carries <c>NPC.direction</c>).
	/// </summary>
	/// <param name="source">The entity source the spawn came from.</param>
	public override void OnSpawn(IEntitySource source)
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		NPC.direction = Main.rand.NextBool() ? 1 : -1;
		NPC.spriteDirection = NPC.direction;
		NPC.netUpdate = true;
	}

	/// <summary>
	/// The provoked-stat re-assertion and the morale seam. 意志高涨 (D-46) is a blocker, so the morale
	/// members keep their neutral values and this is a no-op today; it exists so a later morale/command
	/// system can raise defence and damage without this class being reworked, and so the design's
	/// state-dependent defence is restored from the bases on every side (<c>NPC.defense</c> is not part of
	/// the NPC net message, the <c>MossyThornTurtle</c> precedent).
	/// </summary>
	public override void PostAI()
	{
		NPC.defense = NPC.defDefense + (int)MoraleDefenseBonus;
		NPC.damage = (int)(NPC.defDamage * MoraleDamageScale);
	}

	/// <summary>
	/// 中立 until provoked. The patrol walks at <see cref="WanderSpeed"/> and turns at a wall or a ledge;
	/// provocation comes from damage taken (see <see cref="HitEffect"/>) or from a player inside
	/// <see cref="ProvokeRange"/>.
	/// </summary>
	public override void AI()
	{
		if (State == AnimatedWitherbarkSoldierState.Neutral)
		{
			UpdateNeutral();
		}
		else
		{
			UpdateHostile();
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
	/// Damage taken is the primary provocation reading (the design's 中立 is not described as permanent), so
	/// a non-lethal hit moves the creature to the provoked state. The transition is written on the
	/// authoritative side only, with an <c>NPC.netUpdate</c> (D-55).
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
			EnterHostile();
		}
	}

	/// <summary>
	/// The idle patrol of the design's 中立 default: a slow walk that reverses at a wall or a ledge, with no
	/// player targeting at all.
	/// </summary>
	private void UpdateNeutral()
	{
		if (TryGetPlayer(out _, out float distance) && distance <= ProvokeRange)
		{
			EnterHostile();
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
	/// 近战敌怪会像原版的地牢骷髅一样尝试近战: close on the prey at <see cref="ChaseSpeed"/> (scaled by the
	/// morale seam), jump at an obstruction or when the prey is above, and deal contact damage.
	/// </summary>
	private void UpdateHostile()
	{
		if (!TryGetPlayer(out Player target, out float distance) || distance > LeashRange)
		{
			EnterNeutral();
			return;
		}

		float direction = target.Center.X >= NPC.Center.X ? 1f : -1f;
		NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, direction * ChaseSpeed * MoraleSpeedScale, 0.08f);

		if (Main.netMode != NetmodeID.MultiplayerClient && JumpCooldown > 0)
		{
			JumpCooldown--;
		}

		// The vanilla dungeon skeleton jumps at a wall, at a ledge and when its prey is above it.
		bool preyAbove = target.Center.Y < NPC.Center.Y - 48f;
		if (JumpCooldown <= 0 && NPC.velocity.Y == 0f && (NPC.collideX || !HasFloorAhead() || preyAbove))
		{
			NPC.velocity.Y = -JumpSpeed;
			JumpCooldown = JumpCooldownFrames;
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

	/// <summary>The nearest live player, used only by the provocation test and the provoked chase.</summary>
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

	/// <summary>The provoked transition, written only on the authoritative side with an <c>NPC.netUpdate</c> (D-55).</summary>
	private void EnterHostile()
	{
		if (Main.netMode == NetmodeID.MultiplayerClient || State == AnimatedWitherbarkSoldierState.Hostile)
		{
			return;
		}

		State = AnimatedWitherbarkSoldierState.Hostile;
		NPC.netUpdate = true;
	}

	/// <summary>The return to 中立 once the prey is out of leash, written authoritatively (D-55).</summary>
	private void EnterNeutral()
	{
		if (Main.netMode == NetmodeID.MultiplayerClient || State == AnimatedWitherbarkSoldierState.Neutral)
		{
			return;
		}

		State = AnimatedWitherbarkSoldierState.Neutral;
		NPC.netUpdate = true;
	}

	/// <summary>
	/// 庭院的基础敌怪单位, 也拥有最高的刷新权重: this variant carries the phase's highest land weight (the top of
	/// the D-54 land band) because the design makes it the courtyard's most common unit, and the design's other
	/// three variants return lower land weights. Only ever inside Yggdrasil and inside the Kelp Curtain layer
	/// (BIO-06): <c>NPCSpawnManager.EditSpawnPool</c> returns early outside the subworld, so this per-creature
	/// gate is the real isolation, and <see cref="KelpCurtainBiome.IsKelpCurtainLayer"/> is the server-safe layer
	/// predicate because the hook runs in single player or on the server only, where the client camera is zero
	/// (D-52/D-55). The design's 刺苔庭园 is an above-water courtyard, so the land condition is its own 地表
	/// reading (Pitfall 6).
	/// </summary>
	/// <param name="spawnInfo">The engine's spawn context.</param>
	/// <returns>The highest land weight, or <c>0f</c> outside the design's context.</returns>
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

		return 1.5f;
	}

	/// <summary>
	/// 除了犬，均掉落1~2 枯木碎块 ... 任何枯木士兵在意志高涨期间死亡，都会有50%概率掉落1 干涸心脏: neither 枯木碎块
	/// nor 干涸心脏 has a ModItem in the repository, so no rule is written and no item type is referenced here
	/// (D-58, Pitfall 2): a <c>ModContent.ItemType&lt;X&gt;()</c> for an absent <c>X</c> would fail the whole mod
	/// build. Only the 犬 variant wires a drop. The blockers live in 04-DEVIATIONS.md section 6 and in the
	/// biology matrix instead.
	/// </summary>
	/// <param name="npcLoot">The loot table being built.</param>
	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
	}
}

using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.KelpCurtain.Projectiles.Enemies;
using SubworldLibrary;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.NPCs.SpinyMossCourt;

/// <summary>
/// 枯木活化士兵（远程）（Animated Witherbark Soldier, Ranged）——刺苔庭园（Spiny Moss Court）王庭的投石单位。
/// Design row: 枯木活化士兵, region h1 block YCYwdeYPEomoMVxotFOcE8gZnUh (刺苔庭园),
/// heading block id S4Jadlq0colTtbx8UagceDb7nCq, stats table NNe6dSQqxoxXllxT9PvcVmApn1c,
/// behaviour block ids BYhvdnXwrogK3Jx1z35cwgS7nhg / MGYedPrhioUFFdxP5XrcMpnxnMd,
/// drop block id KzhDde2Q7o3Te6xUDPsclRNqnCW.
/// <para>
/// This is a <b>sibling class</b> of the row whose <c>internal_name</c> is <c>AnimatedWitherbarkSoldier</c>
/// (the 近战 variant): the design gives 普通（远程）its own stat row (生命 60 / 伤害 35 / 防御 0 / 击退抗性 40 /
/// 免疫 中毒、困惑 / 1银50铜), and <c>SetDefaults</c> runs once per NPC type, so a stat variant must never be
/// faked by mutating <c>NPC.lifeMax</c> at runtime (OQ2, 04-DEVIATIONS.md section 2).
/// </para>
/// <para>
/// 远程敌怪会向玩家投掷巨石，每次需要从地面拾取巨石，经过120帧后才能投出（缓慢的捡石头动画），在此期间无法移动，
/// 每次攻击后会尝试和玩家保持8格左右的距离，持续200帧，随后才会进行下一轮攻击. The cycle is one enum over
/// <c>NPC.ai[0]</c>: seek, pick up for <see cref="PickupFrames"/> frames without moving, throw
/// <c>AnimatedWitherbarkSoldier_Boulder</c>, then hold about <see cref="PreferredRangeTiles"/> tiles from the
/// prey for <see cref="RetreatFrames"/> frames. The repository has no ground-boulder entity, so the design's
/// 从地面拾取巨石 is represented by the immobilised crouch at the throwing position (recorded as a conservative
/// reading, not a new entity; 04-DEVIATIONS.md section 10).
/// </para>
/// <para>
/// Approved artwork is missing from the repository, so the class requests the shared
/// <c>Commons.ModAsset.White_Mod</c> fallback. The <c>SpinyMossCourt</c> region subfolder is what tML's
/// default (namespace-derived) texture resolution expects, so the D-49 migration is adding
/// <c>AnimatedWitherbarkSoldierRanged.png</c> beside this .cs and deleting the <c>Texture</c> override.
/// </para>
/// <para>
/// D-46 BLOCKER - the Spiny Moss Court morale/command system does not exist, so the design's 意志高涨 bonus
/// (防御力+4, 攻击+35%, 移速+15%) and the 干涸心脏 morale-gated drop are NOT implemented. The morale seam is the
/// named members below, which a later phase raises without reworking this AI (04-DEVIATIONS.md sections 7, 13).
/// </para>
/// </summary>
public class AnimatedWitherbarkSoldierRanged : ModNPC
{
	/// <summary>
	/// 在默认情况下是中立状态: the proximity half of the provocation reading. The design names no number, so a
	/// conservative courtyard-guard radius is used (D-54 default, 04-DEVIATIONS.md section 10).
	/// </summary>
	private const float ProvokeRange = 10f * 16f;

	/// <summary>漫无目的地游荡: the slow patrol pace used while the creature is still 中立 (D-54 default).</summary>
	private const float WanderSpeed = 0.9f;

	/// <summary>The pace used while walking toward and away from the prey (D-54 default).</summary>
	private const float WalkSpeed = 1.5f;

	/// <summary>
	/// 每次需要从地面拾取巨石，经过120帧后才能投出: exactly 120 frames of immobility, then the throw.
	/// </summary>
	private const int PickupFrames = 120;

	/// <summary>
	/// 每次攻击后会尝试和玩家保持8格左右的距离，持续200帧: exactly 200 frames of spacing, then the next round.
	/// </summary>
	private const int RetreatFrames = 200;

	/// <summary>每次攻击后会尝试和玩家保持8格左右的距离: about eight tiles, the design's own number.</summary>
	private const float PreferredRangeTiles = 8f;

	/// <summary>
	/// The spacing window's tolerance around <see cref="PreferredRangeTiles"/>, so the creature does not
	/// oscillate on the boundary (D-54 default).
	/// </summary>
	private const float PreferredRangeSlackTiles = 1.5f;

	/// <summary>The range at which the creature stops walking and begins the 捡石头 crouch (D-54 default).</summary>
	private const float PickupRange = 16f * 16f;

	/// <summary>投掷巨石: the boulder's launch speed (D-54 default; the design gives no number).</summary>
	private const float BoulderSpeed = 8.5f;

	/// <summary>The distance at which the provoked state falls back to 中立 (D-54 default).</summary>
	private const float LeashRange = 70f * 16f;

	/// <summary>
	/// The three states of the design's ranged cycle: 从地面拾取巨石, the 120-frame immobilised crouch, and the
	/// 200-frame spacing window that follows the throw.
	/// </summary>
	private enum AnimatedWitherbarkSoldierRangedState
	{
		SeekingBoulder = 0,
		PickingUp = 1,
		Retreating = 2,
	}

	/// <summary>Named wrapper over <c>NPC.ai[0]</c> so no bare numeric index is scattered through the class.</summary>
	private AnimatedWitherbarkSoldierRangedState State
	{
		get => (AnimatedWitherbarkSoldierRangedState)(int)NPC.ai[0];
		set => NPC.ai[0] = (int)value;
	}

	/// <summary>Named wrapper over <c>NPC.localAI[0]</c>: the frames left in the current state's timer.</summary>
	private int StateTimer
	{
		get => (int)NPC.localAI[0];
		set => NPC.localAI[0] = value;
	}

	/// <summary>
	/// The provocation flag, kept in a named wrapper over the synced <c>NPC.ai[1]</c> (1 once the design's
	/// 中立 has been broken, 0 while it still holds), because <c>NPC.ai[0]</c> is already the attack cycle.
	/// It must ride the synced array: every side reads it through <see cref="IsNeutral"/> to choose its AI
	/// branch, and <c>NPC.localAI[]</c> never reaches a client (D-55).
	/// </summary>
	private int ProvokedFlag
	{
		get => (int)NPC.ai[1];
		set => NPC.ai[1] = value;
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
	/// A named neutral-state member (the design's 中立 default, exposed so a later morale/command system can
	/// read and flip it, D-46).
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

		// 免疫 中毒、困惑 (the design row's 免疫 cell for this variant): exactly these two immunities.
		NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
		NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
	}

	public override void SetDefaults()
	{
		// The design's seek / crouch / throw / space-apart cycle has no vanilla aiStyle analogue, so the class
		// owns its AI() (the documented case where a local AI() is warranted, D-31).
		NPC.aiStyle = -1;

		// No sprite exists to measure, so a conservative humanoid soldier box is used (D-54 default).
		NPC.width = 30;
		NPC.height = 46;

		NPC.lifeMax = 60; // 生命 60
		NPC.life = 60;
		NPC.damage = 35; // 伤害 35
		NPC.defDamage = 35;
		NPC.defense = 0; // 防御 0
		NPC.defDefense = 0;
		NPC.knockBackResist = 0.6f; // 击退抗性 40 -> 1 - 0.40
		NPC.value = 150; // 钱币（铜） 1银50铜 = 150 copper, a 1:1 transpose (NPC.value is copper)

		// Conservative default: the design's 稀有度 cell is empty and 普通 is its 类型 cell, so no design
		// rarity exists. NPC.rarity is the engine's only NPC rarity field and defaults to White, so this
		// records the empty cell (the Phase 3 NPC.rare -> NPC.rarity correction, section 10).
		NPC.rarity = ItemRarityID.White;

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
	/// The design's ranged cycle, and the 中立 default: while unprovoked the creature only patrols, so the
	/// attack cycle never begins before the design's own 中立 is broken.
	/// </summary>
	public override void AI()
	{
		if (IsNeutral)
		{
			UpdateNeutral();
			UpdateFacing();
			return;
		}

		switch (State)
		{
			case AnimatedWitherbarkSoldierRangedState.PickingUp:
				UpdatePickingUp();
				break;
			case AnimatedWitherbarkSoldierRangedState.Retreating:
				UpdateRetreating();
				break;
			default:
				UpdateSeekingBoulder();
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
			State = AnimatedWitherbarkSoldierRangedState.SeekingBoulder;
			StateTimer = 0;
			NPC.netUpdate = true;
		}
	}

	/// <summary>
	/// The idle patrol of the design's 中立 default: a slow walk that reverses at a wall or a ledge, with no
	/// player targeting at all until the design's 中立 is broken.
	/// </summary>
	private void UpdateNeutral()
	{
		if (TryGetPlayer(out _, out float distance) && distance <= ProvokeRange)
		{
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				ProvokedFlag = 1;
				State = AnimatedWitherbarkSoldierRangedState.SeekingBoulder;
				StateTimer = 0;
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
	/// 从地面拾取巨石: close on the prey until the design's throwing distance, then hand over to the
	/// immobilised crouch.
	/// </summary>
	private void UpdateSeekingBoulder()
	{
		if (!TryGetPlayer(out Player target, out float distance) || distance > LeashRange)
		{
			EnterNeutral();
			return;
		}

		if (distance <= PickupRange)
		{
			EnterPickingUp();
			return;
		}

		float direction = target.Center.X >= NPC.Center.X ? 1f : -1f;
		NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, direction * WalkSpeed * MoraleSpeedScale, 0.08f);
		if (NPC.velocity.Y == 0f && NPC.collideX)
		{
			NPC.velocity.Y = -6f;
		}
	}

	/// <summary>
	/// 经过120帧后才能投出（缓慢的捡石头动画），在此期间无法移动: the creature plants itself, holds still for
	/// <see cref="PickupFrames"/> frames and then throws. The timer runs on the authoritative side only, and the
	/// throw is the last tick of the window (D-55).
	/// </summary>
	private void UpdatePickingUp()
	{
		NPC.velocity.X = 0f;

		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		if (StateTimer < PickupFrames)
		{
			StateTimer++;
			return;
		}

		ThrowBoulder();
		State = AnimatedWitherbarkSoldierRangedState.Retreating;
		StateTimer = 0;
		NPC.netUpdate = true;
	}

	/// <summary>
	/// 每次攻击后会尝试和玩家保持8格左右的距离，持续200帧: hold about eight tiles for
	/// <see cref="RetreatFrames"/> frames (walking away when too close, closing when too far), then begin the
	/// next round.
	/// </summary>
	private void UpdateRetreating()
	{
		if (!TryGetPlayer(out Player target, out float distance) || distance > LeashRange)
		{
			EnterNeutral();
			return;
		}

		float preferred = PreferredRangeTiles * 16f;
		float slack = PreferredRangeSlackTiles * 16f;
		float direction = 0f;
		if (distance < preferred - slack)
		{
			direction = NPC.Center.X >= target.Center.X ? 1f : -1f;
		}
		else if (distance > preferred + slack)
		{
			direction = target.Center.X >= NPC.Center.X ? 1f : -1f;
		}

		NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, direction * WalkSpeed * MoraleSpeedScale, 0.08f);

		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		StateTimer++;
		if (StateTimer >= RetreatFrames)
		{
			State = AnimatedWitherbarkSoldierRangedState.SeekingBoulder;
			StateTimer = 0;
			NPC.netUpdate = true;
		}
	}

	/// <summary>
	/// 向玩家投掷巨石: spawns one <c>AnimatedWitherbarkSoldier_Boulder</c> from the creature's own AI source, on
	/// the authoritative side only, so the damage credit belongs to the creature and the boulder is never
	/// duplicated per client (D-55).
	/// </summary>
	private void ThrowBoulder()
	{
		Vector2 direction = Vector2.UnitX * NPC.direction;
		if (NPC.target >= 0 && NPC.target < Main.maxPlayers)
		{
			Player target = Main.player[NPC.target];
			if (target.active && !target.dead)
			{
				direction = (target.Center - NPC.Center).SafeNormalize(direction);
			}
		}

		// A slight upward lob so the boulder arcs onto the prey instead of dropping short.
		Vector2 velocity = direction * BoulderSpeed;
		velocity.Y -= 3f;
		Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, ModContent.ProjectileType<AnimatedWitherbarkSoldier_Boulder>(), NPC.damage, 0f);
		NPC.netUpdate = true;
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

	/// <summary>The nearest live player, used only by the provocation test and the attack cycle.</summary>
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

	/// <summary>The transition into the 捡石头 crouch, written authoritatively with an <c>NPC.netUpdate</c> (D-55).</summary>
	private void EnterPickingUp()
	{
		if (Main.netMode == NetmodeID.MultiplayerClient || State == AnimatedWitherbarkSoldierRangedState.PickingUp)
		{
			return;
		}

		State = AnimatedWitherbarkSoldierRangedState.PickingUp;
		StateTimer = 0;
		NPC.velocity.X = 0f;
		NPC.netUpdate = true;
	}

	/// <summary>The return to 中立 once the prey is out of leash, written authoritatively (D-55).</summary>
	private void EnterNeutral()
	{
		if (Main.netMode == NetmodeID.MultiplayerClient || IsNeutral)
		{
			return;
		}

		ProvokedFlag = 0;
		State = AnimatedWitherbarkSoldierRangedState.SeekingBoulder;
		StateTimer = 0;
		NPC.netUpdate = true;
	}

	/// <summary>
	/// The courtyard unit's land spawn gate. The design makes the 近战 variant the most common one, so this
	/// variant returns a lower land weight (D-54 default; the design gives no per-variant weight). Only ever
	/// inside Yggdrasil and inside the Kelp Curtain layer (BIO-06): <c>NPCSpawnManager.EditSpawnPool</c> returns
	/// early outside the subworld, so this per-creature gate is the real isolation, and
	/// <see cref="KelpCurtainBiome.IsKelpCurtainLayer"/> is the server-safe layer predicate because the hook runs
	/// in single player or on the server only, where the client camera is zero (D-52/D-55). The design's 刺苔庭园
	/// is an above-water courtyard, so the land condition is its own 地表 reading (Pitfall 6).
	/// </summary>
	/// <param name="spawnInfo">The engine's spawn context.</param>
	/// <returns>The land weight, or <c>0f</c> outside the design's context.</returns>
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

		return 1f;
	}

	/// <summary>
	/// 除了犬，均掉落1~2 枯木碎块 ... 干涸心脏: neither 枯木碎块 nor 干涸心脏 has a ModItem in the repository, so
	/// no rule is written and no item type is referenced here (D-58, Pitfall 2): a
	/// <c>ModContent.ItemType&lt;X&gt;()</c> for an absent <c>X</c> would fail the whole mod build. Only the 犬
	/// variant wires a drop. The blockers live in 04-DEVIATIONS.md section 6 and in the biology matrix instead.
	/// </summary>
	/// <param name="npcLoot">The loot table being built.</param>
	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
	}
}

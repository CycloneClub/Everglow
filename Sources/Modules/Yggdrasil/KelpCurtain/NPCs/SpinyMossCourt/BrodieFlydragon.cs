using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using SubworldLibrary;

namespace Everglow.Yggdrasil.KelpCurtain.NPCs.SpinyMossCourt;

/// <summary>
/// 布罗迪蝇蜓（Brodie Flydragon）——刺苔庭园（Spiny Moss Court）与森雨幽谷（Valley of Lush and Moist）的飞行敌怪（标准）.
/// Design row: 布罗迪蝇蜓, region h1 blocks YCYwdeYPEomoMVxotFOcE8gZnUh (刺苔庭园) and
/// WxatdnICsojUQoxS6VXcXeYgnwd (森雨幽谷), heading block id BAQKdvFlvoy7wJxCglZcUWGCnOh,
/// stats table V9zXdPuSdoQD7bxe6IVcdzqrncf, behaviour block ids KLvadaAVTodoIWxPu5kcN4Spnsg /
/// Pf95dbVTFodZTMx5qJSc34ZPnbd.
/// <para>
/// This class is the row's own <c>internal_name</c>: it implements the 普通（标准）stat row
/// (生命 40 / 伤害 25 / 防御 5 / 击退抗性 50 / 免疫 中毒 / 20 copper). The 小 row is the separate class
/// <c>SmallBrodieFlydragon</c>, because <c>SetDefaults</c> runs once per NPC type and a stat variant must never be
/// faked by mutating <c>NPC.lifeMax</c> at runtime (OQ2, 04-DEVIATIONS.md section 2).
/// </para>
/// <para>
/// 会在刺苔庭园与森雨幽谷刷新，小型的飞行敌怪 ... 主动靠近玩家并且进行近战攻击: the creature hovers over the courtyard,
/// pursues the player once they enter its range and presses into them for contact damage. 自然刷新只会刷新标准大小
/// 的蝇蜓 - this standard class is the one the design's natural spawns use, so it carries the higher spawn weight.
/// </para>
/// <para>
/// D-46 BLOCKER - the 森雨幽谷 Valley egg system does not exist: there is no egg NPC and no egg-breaking spawn path,
/// so this plan creates neither and neither class relies on one. The small variant above is spawned by this class's
/// own separate natural weight instead - a recorded deferral, never an approximation. The design's split of its
/// habitat between 刺苔庭园 and 森雨幽谷 is the separately recorded Phase 5-6 region gap (D-52/D-53): no coordinate
/// range or other region stand-in is written into <see cref="SpawnChance"/> (04-DEVIATIONS.md sections 5, 7, 13).
/// </para>
/// <para>
/// Approved artwork is missing from the repository, so the class requests the shared
/// <c>Commons.ModAsset.White_Mod</c> fallback. The <c>SpinyMossCourt</c> region subfolder is what tML's default
/// (namespace-derived) texture resolution expects, so the D-49 migration is adding <c>BrodieFlydragon.png</c>
/// beside this .cs and deleting the <c>Texture</c> override.
/// </para>
/// </summary>
public class BrodieFlydragon : ModNPC
{
	/// <summary>主动靠近玩家: the range at which the hover gives way to the pursuit (D-54 default).</summary>
	private const float AggroRange = 40f * 16f;

	/// <summary>The lazy open-air drift speed of the hovering state (D-54 default).</summary>
	private const float HoverSpeed = 2f;

	/// <summary>主动靠近玩家: the pursuit speed (D-54 default; the design gives no number).</summary>
	private const float PursueSpeed = 3.6f;

	/// <summary>The short press into the player that turns the pursuit into a 近战攻击 (D-54 default).</summary>
	private const float LungeSpeed = 5.5f;

	/// <summary>进行近战攻击: the range at which the creature commits to its melee press (D-54 default).</summary>
	private const float MeleeRange = 2.6f * 16f;

	/// <summary>How long the creature keeps circling in one direction before turning (D-54 default).</summary>
	private const int HoverCycleFrames = 150;

	/// <summary>How long the melee press lasts before the creature backs out and resumes the pursuit (D-54 default).</summary>
	private const int LungeFrames = 30;

	/// <summary>How long the pursuit may go on without reaching the player before the creature disengages (D-54 default).</summary>
	private const int PursueGiveUpFrames = 300;

	/// <summary>The vertical bob amplitude of the hover, in pixels per tick (D-54 default).</summary>
	private const float HoverBobSpeed = 0.45f;

	/// <summary>
	/// The three states of the design's flying melee creature: the open-air hover, 主动靠近玩家 and the 近战攻击 press.
	/// </summary>
	private enum BrodieFlydragonState
	{
		Hovering = 0,
		Pursuing = 1,
		Attacking = 2,
	}

	/// <summary>Named wrapper over <c>NPC.ai[0]</c> so no bare numeric index is scattered through the class.</summary>
	private BrodieFlydragonState State
	{
		get => (BrodieFlydragonState)(int)NPC.ai[0];
		set => NPC.ai[0] = (int)value;
	}

	/// <summary>Named wrapper over <c>NPC.localAI[0]</c>: the frames spent in the current hover/pursuit cycle.</summary>
	private int HoverTimer
	{
		get => (int)NPC.localAI[0];
		set => NPC.localAI[0] = value;
	}

	/// <summary>Named wrapper over <c>NPC.localAI[1]</c>: the frames spent in the current pursuit.</summary>
	private int PursueTimer
	{
		get => (int)NPC.localAI[1];
		set => NPC.localAI[1] = value;
	}

	/// <summary>Named wrapper over <c>NPC.localAI[2]</c>: the frames spent in the current melee press.</summary>
	private int AttackTimer
	{
		get => (int)NPC.localAI[2];
		set => NPC.localAI[2] = value;
	}

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

		// 免疫 中毒 (the design row's 免疫 cell for the 标准 row): the poisoned debuff is the only immunity.
		NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
	}

	public override void SetDefaults()
	{
		// The design's hover/pursue/melee-press cycle has no vanilla aiStyle analogue, so the class owns its AI()
		// (the documented case where a local AI() is warranted, D-31).
		NPC.aiStyle = -1;

		// No sprite exists to measure, so a conservative flying box is used (D-54 default).
		NPC.width = 44;
		NPC.height = 32;

		NPC.lifeMax = 40; // 生命 40
		NPC.life = 40;
		NPC.damage = 25; // 伤害 25
		NPC.defDamage = 25;
		NPC.defense = 5; // 防御 5
		NPC.defDefense = 5;
		NPC.knockBackResist = 0.5f; // 击退抗性 50 -> 1 - 0.50
		NPC.value = 20; // 钱币（铜） 20 copper, a 1:1 transpose (NPC.value is copper)

		// Conservative default: the design's 稀有度 cell is empty and 普通 is its 类型 cell, so no design rarity
		// exists. NPC.rarity is the engine's only NPC rarity field and defaults to White, so this records the
		// empty cell (the Phase 3 NPC.rare -> NPC.rarity correction, section 10).
		NPC.rarity = ItemRarityID.White;

		// 小型的飞行敌怪: an open-air flier, so gravity is off. Tile collision stays on (the VerdantRods
		// precedent in this same region) so it cannot fly through the courtyard's terrain.
		NPC.noGravity = true;
		NPC.noTileCollide = false;

		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;

		// 布罗迪蝇蜓 is not 可被捕捉, so no catch item, no Main.npcCatchable and no CountsAsCritter.
		NPC.catchItem = 0;

		// Deterministic starting values, because SetDefaults runs on every side: no Main.rand is consumed here, so
		// the server and its clients agree before the first sync arrives.
		HoverTimer = 0;
		PursueTimer = 0;
		AttackTimer = 0;

		SpawnModBiomes = [ModContent.GetInstance<KelpCurtainBiome>().Type];
	}

	/// <summary>
	/// The hover / pursue / melee press loop. Every state transition is written on the authoritative side only,
	/// with an <c>NPC.netUpdate</c> (D-55).
	/// </summary>
	public override void AI()
	{
		switch (State)
		{
			case BrodieFlydragonState.Pursuing:
				UpdatePursuing();
				break;
			case BrodieFlydragonState.Attacking:
				UpdateAttacking();
				break;
			default:
				UpdateHovering();
				break;
		}

		UpdateFacing();
	}

	/// <summary>
	/// The design row's 减伤 cell is empty (stats table V9zXdPuSdoQD7bxe6IVcdzqrncf), so no <c>FinalDamage</c>
	/// scaling is applied here. The omission is the design's, not the plan's.
	/// </summary>
	/// <param name="modifiers">The hit modifiers being resolved.</param>
	public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
	{
	}

	/// <summary>Death dust, guarded so the dedicated server never touches graphics (D-35/D-55).</summary>
	/// <param name="hit">The resolved hit.</param>
	public override void HitEffect(NPC.HitInfo hit)
	{
		if (NPC.life > 0 || Main.dedServ)
		{
			return;
		}

		for (int i = 0; i < 6; i++)
		{
			int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<WitherWoodDust>(), 2 * hit.HitDirection, -2f);
			Main.dust[dust].noGravity = true;
			Main.dust[dust].scale = Main.rand.NextFloat(0.7f, 1.2f);
		}
	}

	/// <summary>
	/// The open-air hover: a lazy patrol with a vertical bob that turns after
	/// <see cref="HoverCycleFrames"/> or at a wall, and hands over to the pursuit as soon as a player is inside
	/// <see cref="AggroRange"/>.
	/// </summary>
	private void UpdateHovering()
	{
		if (TryGetPlayer(out _, out float distance) && distance <= AggroRange)
		{
			EnterPursuing();
			return;
		}

		NPC.velocity.X = HoverSpeed * NPC.direction;
		NPC.velocity.Y = MathF.Sin((float)Main.time * 0.05f + NPC.whoAmI) * HoverBobSpeed;

		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			HoverTimer++;
			if (HoverTimer >= HoverCycleFrames || NPC.collideX)
			{
				NPC.direction *= -1;
				HoverTimer = 0;
				NPC.netUpdate = true;
			}
		}
	}

	/// <summary>
	/// 主动靠近玩家: fly at the player until the melee range is reached (the press), or until the pursuit has
	/// gone on for <see cref="PursueGiveUpFrames"/> without a hit, in which case the creature disengages.
	/// </summary>
	private void UpdatePursuing()
	{
		if (!TryGetPlayer(out Player target, out float distance))
		{
			EnterHovering();
			return;
		}

		if (distance <= MeleeRange)
		{
			EnterAttacking();
			return;
		}

		Vector2 direction = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX * NPC.direction);
		NPC.velocity = Vector2.Lerp(NPC.velocity, direction * PursueSpeed, 0.06f);

		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		PursueTimer++;
		if (PursueTimer >= PursueGiveUpFrames)
		{
			EnterHovering();
		}
	}

	/// <summary>
	/// 进行近战攻击: press into the player for <see cref="LungeFrames"/> so the creature's contact damage lands,
	/// then back out into the pursuit.
	/// </summary>
	private void UpdateAttacking()
	{
		if (!TryGetPlayer(out Player target, out _))
		{
			EnterHovering();
			return;
		}

		Vector2 direction = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX * NPC.direction);
		NPC.velocity = Vector2.Lerp(NPC.velocity, direction * LungeSpeed, 0.10f);

		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		AttackTimer++;
		if (AttackTimer >= LungeFrames)
		{
			EnterPursuing();
		}
	}

	/// <summary>The nearest live player, used only by the aggro test and the pursuit (D-55: never a camera value).</summary>
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

	/// <summary>主动靠近玩家, written only on the authoritative side with an <c>NPC.netUpdate</c> (D-55).</summary>
	private void EnterPursuing()
	{
		if (Main.netMode == NetmodeID.MultiplayerClient || State == BrodieFlydragonState.Pursuing)
		{
			return;
		}

		State = BrodieFlydragonState.Pursuing;
		PursueTimer = 0;
		NPC.netUpdate = true;
	}

	/// <summary>进行近战攻击, written only on the authoritative side with an <c>NPC.netUpdate</c> (D-55).</summary>
	private void EnterAttacking()
	{
		if (Main.netMode == NetmodeID.MultiplayerClient || State == BrodieFlydragonState.Attacking)
		{
			return;
		}

		State = BrodieFlydragonState.Attacking;
		AttackTimer = 0;
		NPC.netUpdate = true;
	}

	/// <summary>The return to the open-air hover once the pursuit is given up, written authoritatively (D-55).</summary>
	private void EnterHovering()
	{
		if (Main.netMode == NetmodeID.MultiplayerClient || State == BrodieFlydragonState.Hovering)
		{
			return;
		}

		State = BrodieFlydragonState.Hovering;
		HoverTimer = 0;
		PursueTimer = 0;
		NPC.netUpdate = true;
	}

	/// <summary>
	/// 自然刷新只会刷新标准大小的蝇蜓: this standard class carries the higher of the two flydragon weights, so the
	/// design's natural spawns are standard-sized. The small variant is reachable through its own lower conservative
	/// weight instead, because the 森雨幽谷 egg route the design names is an unimplemented D-46 system. Only ever
	/// inside Yggdrasil and inside the Kelp Curtain layer (BIO-06): <c>NPCSpawnManager.EditSpawnPool</c> returns early
	/// outside the subworld, so this per-creature gate is the real isolation, and
	/// <see cref="KelpCurtainBiome.IsKelpCurtainLayer"/> is the server-safe layer predicate because the hook runs in
	/// single player or on the server only, where the client camera is zero (D-52/D-55). The design's
	/// 刺苔庭园 / 森雨幽谷 split is a Phase 5-6 region blocker and is never approximated with a coordinate range
	/// (D-53).
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
	/// The design row names no drop at all (the stats table V9zXdPuSdoQD7bxe6IVcdzqrncf has no drop block, and the
	/// matrix row's <c>drop_block_id</c> is empty), so the table stays empty and no item type is referenced - the
	/// <c>VerdantRods</c> precedent for a row whose design names no drop.
	/// </summary>
	/// <param name="npcLoot">The loot table being built.</param>
	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
	}
}

using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.KelpCurtain.Projectiles.Enemies;
using SubworldLibrary;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;

namespace Everglow.Yggdrasil.KelpCurtain.NPCs.ValleyOfLushAndMoist;

/// <summary>
/// 阿萨辛覆盘子（Assassin Raspberry），森雨幽谷（Valley of Lush and Moist）的伏击者。
/// Design row: 阿萨辛覆盘子, heading block id TjICdnhRvowaSdxcAcXchNGDnco,
/// behavior block PqdMdnpwQomTTIxDt3PcdyGDnNd (静止的敌对生物，玩家在距离恰好为4~8格时，会伸出并向着玩家随机散射
/// 4~6颗地刺，少于4格无法攻击但是不会缩回去，超过8格后重新缩回地下), the design's 免疫 中毒、困惑 and
/// 击退抗性 免疫击退 cells, 生命 80 / 伤害 30 / 防御 20（被动）4（攻击） / 钱币（铜）1银 = 100.
/// <para>
/// <b>Scope (D-46).</b> The trigger, the 8-tile extend and the 4~8-tile attack window are built in full.
/// The design's 伪装 - a raspberry that is indistinguishable from scenery while it is buried - belongs to
/// the Valley disguised-hazard visual system, which does not exist in this repository (04-DEVIATIONS.md
/// section 7 lists it as an unimplemented system). That presentation is therefore a D-46 blocker naming
/// that system and this file, and no disguise trick is faked here: a half-built alpha trick would be
/// indistinguishable from a rendering bug.
/// </para>
/// <para>
/// Approved artwork (贴图) is missing from the repository, so the class requests the shared
/// Commons.ModAsset.White_Mod fallback. When the art arrives the D-49 migration is adding
/// AssassinRaspberry.png beside this .cs and deleting the Texture override (D-48/D-49); the runtime asset
/// path is Everglow/Yggdrasil/KelpCurtain/NPCs/ValleyOfLushAndMoist/AssassinRaspberry.png.
/// </para>
/// </summary>
public class AssassinRaspberry : ModNPC
{
	/// <summary>超过8格后重新缩回地下: beyond this range the ambusher retracts and stays buried.</summary>
	private const float ExtendTiles = 8f;

	/// <summary>玩家在距离恰好为4~8格时: the closest range at which the ambusher can still attack.</summary>
	private const float AttackMinTiles = 4f;

	/// <summary>Pixels per tile, so a world-space distance can be compared with the design's tile window.</summary>
	private const float PixelsPerTile = 16f;

	/// <summary>散射4~6颗地刺: the volley's minimum spike count.</summary>
	private const int MinSpikes = 4;

	/// <summary>散射4~6颗地刺: the volley's maximum spike count.</summary>
	private const int MaxSpikes = 6;

	/// <summary>
	/// The volley cadence. The design gives no interval, so a conservative three-second cadence is used
	/// and recorded as a D-54 default (04-DEVIATIONS.md section 10 covers the register).
	/// </summary>
	private const int VolleyIntervalFrames = 180;

	/// <summary>向着玩家随机散射: the cone the spikes are scattered across (D-54 default).</summary>
	private const float MaxScatterRadians = MathHelper.Pi / 6f;

	/// <summary>The spike's launch speed, in pixels per tick (D-54 default).</summary>
	private const float SpikeSpeed = 8f;

	/// <summary>The lift that makes the spike emerge from the ground and arc rather than skid (D-54 default).</summary>
	private const float SpikeLift = 2.5f;

	/// <summary>
	/// The two local states of the design row: <see cref="Hidden"/> is 缩回地下 (buried, passive defence 20)
	/// and <see cref="Extended"/> is 伸出 (attack-capable, defence 4). No vanilla <c>aiStyle</c> provides a
	/// distance-windowed ambusher, so the class owns its <c>AI()</c> (D-31).
	/// </summary>
	private enum AssassinRaspberryState
	{
		Hidden = 0,
		Extended = 1,
	}

	/// <summary>Named wrapper over <c>NPC.ai[0]</c> so no bare numeric index is scattered through the class.</summary>
	private AssassinRaspberryState State
	{
		get => (AssassinRaspberryState)(int)NPC.ai[0];
		set => NPC.ai[0] = (int)value;
	}

	/// <summary>
	/// Named wrapper over <c>NPC.localAI[0]</c>: the frames since the last scattered volley, so no bare
	/// numeric index is scattered through the class.
	/// </summary>
	private int VolleyTimer
	{
		get => (int)NPC.localAI[0];
		set => NPC.localAI[0] = (int)value;
	}

	/// <summary>
	/// The point the stationary ambusher came to rest at, captured on the authoritative side, so 缩回地下
	/// returns it to its own spawn point instead of to wherever a shove left it (D-55).
	/// </summary>
	public Vector2 AnchorPosition;

	// No HJSON key is created for this class; localization stays deferred (D-20).
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.NPCs;

	// Approved artwork is missing from the repository (D-48). The shared fallback keeps the class
	// loadable, and the phase's gate asserts the override until the art arrives.
	public override string Texture => Commons.ModAsset.White_Mod;

	public override void SetStaticDefaults()
	{
		// No approved sprite exists, so there is nothing to slice: the whole texture is one frame.
		Main.npcFrameCount[NPC.type] = 1;
		NPCSpawnManager.RegisterNPC(Type);

		// 免疫 中毒、困惑 (the design row's 免疫 cell): exactly these two immunities.
		NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
		NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
	}

	public override void SetDefaults()
	{
		// The distance-windowed ambush has no vanilla aiStyle analogue, so the class owns its AI().
		NPC.aiStyle = -1;

		// No sprite exists to measure, so a conservative mid-sized ground box is used and is the first
		// value to revisit when the art arrives (D-54).
		NPC.width = 44;
		NPC.height = 48;

		NPC.lifeMax = 80; // 生命 80
		NPC.life = 80;
		NPC.damage = 30; // 伤害 30
		NPC.defDamage = 30;
		NPC.defense = 20; // 防御 20（被动，缩回地下）
		NPC.defDefense = 20;
		NPC.knockBackResist = 0f; // 击退抗性 免疫击退
		NPC.value = 100; // 钱币（铜） 1银 = 100 copper, a 1:1 transpose (NPC.value is copper)

		// Conservative default: the design's 稀有度 cell is empty, and NPC.rarity is the engine's only
		// NPC rarity field (the NPC.rare -> NPC.rarity correction, 04-DEVIATIONS.md section 10).
		NPC.rarity = ItemRarityID.White;

		NPC.friendly = false;
		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;

		// Not 可被捕捉, so no catch item, no Main.npcCatchable and no CountsAsCritter.
		NPC.catchItem = 0;

		SpawnModBiomes = [ModContent.GetInstance<KelpCurtainBiome>().Type];
	}

	public override void OnSpawn(IEntitySource source)
	{
		AnchorPosition = NPC.Center;
	}

	/// <summary>
	/// The design's distance window: the ambusher extends once the prey is inside 8 tiles and retracts once
	/// it leaves, it cannot attack below 4 tiles but does not retract there, and it scatters its spikes only
	/// inside the exact 4~8 tile band. Every authoritative write happens on the server or in single player
	/// (D-55); the distance is computed from the synced <c>NPC.Center</c> and the target's centre, never from
	/// a screen or camera value.
	/// </summary>
	public override void AI()
	{
		NPC.TargetClosest(false);
		Player target = null;
		if (NPC.target >= 0 && NPC.target < Main.maxPlayers)
		{
			target = Main.player[NPC.target];
		}

		bool hasTarget = target is not null && target.active && !target.dead;
		float distance = hasTarget ? Vector2.Distance(NPC.Center, target.Center) : float.MaxValue;
		float extendRange = ExtendTiles * PixelsPerTile;

		if (hasTarget && distance <= extendRange)
		{
			// 玩家在距离恰好为4~8格时,会伸出: inside the extend range it emerges and stays out.
			EnterState(AssassinRaspberryState.Extended);
		}
		else
		{
			// 超过8格后重新缩回地下 (and a lost target counts as beyond every range).
			EnterState(AssassinRaspberryState.Hidden);
		}

		if (State == AssassinRaspberryState.Hidden)
		{
			// Buried and stationary: no horizontal drift, and the rest position becomes the anchor the
			// retract path returns to.
			NPC.velocity.X = 0f;
			if (NPC.velocity.Y == 0f && NPC.collideY)
			{
				AnchorPosition = NPC.Center;
			}

			VolleyTimer = 0;
			return;
		}

		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			UpdateVolley(hasTarget, target, distance, extendRange);
		}
	}

	/// <summary>
	/// 玩家在距离恰好为4~8格时...随机散射4~6颗地刺: the design's attack window, run on the authoritative side
	/// only. 少于4格无法攻击但是不会缩回去, so a player closer than 4 tiles silences the ambusher without
	/// retracting it, and a player beyond 8 tiles has already driven it back underground.
	/// </summary>
	/// <param name="hasTarget">Whether a live player target exists.</param>
	/// <param name="target">The player target, or <c>null</c>.</param>
	/// <param name="distance">The distance to that target.</param>
	/// <param name="extendRange">The <see cref="ExtendTiles"/> range in pixels.</param>
	private void UpdateVolley(bool hasTarget, Player target, float distance, float extendRange)
	{
		if (!hasTarget || target is null || distance < AttackMinTiles * PixelsPerTile || distance > extendRange)
		{
			return;
		}

		VolleyTimer++;
		if (VolleyTimer < VolleyIntervalFrames)
		{
			return;
		}

		VolleyTimer = 0;
		FireSpikeScatter(target.Center);
		NPC.netUpdate = true;
	}

	/// <summary>
	/// The single state-transition helper (the <c>GuppyConch</c> pattern): the design's defence switch -
	/// 防御 20（被动）/ 4（攻击） - is applied here and nowhere else, so no exit path can leave the attacking
	/// value stuck when the ambusher goes back to 缩回地下. Written on the authoritative side only, with an
	/// <c>NPC.netUpdate</c> (D-55).
	/// </summary>
	/// <param name="nextState">The state to enter.</param>
	private void EnterState(AssassinRaspberryState nextState)
	{
		if (State == nextState)
		{
			return;
		}

		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			State = nextState;
			NPC.defense = nextState == AssassinRaspberryState.Extended ? 4 : 20;

			if (nextState == AssassinRaspberryState.Hidden)
			{
				// 缩回地下: the stationary creature returns to its own anchor.
				NPC.Center = AnchorPosition;
				NPC.velocity = Vector2.Zero;
				VolleyTimer = 0;
			}

			NPC.netUpdate = true;
		}
	}

	/// <summary>
	/// 向着玩家随机散射4~6颗地刺: the ambusher's only attack. The spikes leave the ground at the creature's
	/// feet and are scattered at random within a cone toward the prey (the design says 随机散射, not a fan).
	/// Called only from the authoritative side (D-55).
	/// </summary>
	/// <param name="targetCenter">The prey's centre, used as the scatter's aim.</param>
	private void FireSpikeScatter(Vector2 targetCenter)
	{
		int spikeCount = Main.rand.Next(MinSpikes, MaxSpikes + 1);
		Vector2 origin = NPC.Bottom;
		Vector2 aim = (targetCenter - origin).SafeNormalize(Vector2.UnitX);
		for (int i = 0; i < spikeCount; i++)
		{
			float scatter = Main.rand.NextFloat(-MaxScatterRadians, MaxScatterRadians);
			Vector2 velocity = (aim.RotatedBy(scatter) * SpikeSpeed) - (Vector2.UnitY * SpikeLift);
			Projectile.NewProjectile(NPC.GetSource_FromAI(), origin, velocity, ModContent.ProjectileType<AssassinRaspberry_Spike>(), NPC.damage, 0f);
		}
	}

	/// <summary>
	/// The hit and death dust. Client-only work, so the whole body sits behind the dedicated-server guard
	/// (D-35/D-55) and reuses an existing Kelp Curtain dust rather than creating a new dust class (D-51).
	/// </summary>
	/// <param name="hit">The resolved hit.</param>
	public override void HitEffect(NPC.HitInfo hit)
	{
		if (Main.dedServ)
		{
			return;
		}

		int dustCount = NPC.life <= 0 ? 6 : 2;
		for (int i = 0; i < dustCount; i++)
		{
			int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<SucculentHerbDust>(), 2 * hit.HitDirection, -2f);
			Main.dust[dust].noGravity = true;
			Main.dust[dust].scale = Main.rand.NextFloat(0.7f, 1.2f);
		}
	}

	/// <summary>
	/// Subworld-only spawning (BIO-06). <c>NPCSpawnManager.EditSpawnPool</c> returns early outside
	/// Yggdrasil, so this per-creature gate is the real isolation, and
	/// <see cref="KelpCurtainBiome.IsKelpCurtainLayer"/> is the server-safe layer predicate because the
	/// hook runs in single player or on the server only, where the client camera is zero (D-52/D-55).
	/// The ambusher is a ground creature, so the shared dry-land condition is used and the design supplies
	/// no weight (the conservative land band, D-54). The 森雨幽谷 region refinement is a Phases 5-6
	/// predicate gap (04-DEVIATIONS.md section 5).
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
	/// The design row names no drop, so the table is deliberately empty with no type reference
	/// (the VerdantRods precedent, D-57/D-58).
	/// </summary>
	/// <param name="npcLoot">The loot table being built.</param>
	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
	}
}

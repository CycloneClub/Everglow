using Everglow.Commons.Mechanics.Miscs;
using Everglow.Commons.Templates.Enemies;
using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.KelpCurtain.Projectiles.Enemies;
using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using SubworldLibrary;
using Terraria.GameContent.ItemDropRules;

namespace Everglow.Yggdrasil.KelpCurtain.NPCs.ValleyOfLushAndMoist;

/// <summary>
/// 红针洋辣子（Red Needle Caterpillar），森雨幽谷（Valley of Lush and Moist）的针刺毛虫。
/// Design row: 红针洋辣子, heading block id XrNWdCklJoFPUyxEtANcQ9tWnNe,
/// behavior blocks Ofr1dFMe4ot9tLxq6hicZfLxnzh (AI同树皮刺毛虫) and
/// G1ModIH6no6tFXxeuLCcZ8HYnGc (与玩家距离不低于4格时，每过180帧会像尖刺史莱姆一样在头部发射4~6尖刺，
/// 且命中后有25%概率会造成中毒20秒，37.5%概率造成中毒10秒), drop 掉落同树皮刺毛虫.
/// <para>
/// AI同树皮刺毛虫: the AI is the repository's existing segmented-worm template
/// <see cref="Caterpillar"/>, mirrored from <c>YggdrasilTown/NPCs/BarkSpicyCaterpillar.cs</c>, so this
/// class is a stat and behaviour overlay on that template rather than a re-derived worm.
/// </para>
/// <para>
/// 掉落同树皮刺毛虫: the drop is the pre-existing implemented
/// <c>YggdrasilTown/Items/Materials/CaterpillarJuice.cs</c>, wired with exactly the rule
/// <c>BarkSpicyCaterpillar</c> uses (the OQ1 WIRE decision, 04-DEVIATIONS.md section 6.1). No new item,
/// buff, dust, gore or binary asset is created (D-48/D-57).
/// </para>
/// <para>
/// Approved artwork (贴图) is missing from the repository, so the class requests the shared
/// Commons.ModAsset.White_Mod fallback. 04-DEVIATIONS.md section 5 records 红针洋辣子's Valley region
/// split (森雨幽谷 vs 刺苔庭园) as a Phases 5-6 predicate gap, so the spawn gate stays the server-safe
/// subworld + layer + dry-land triple (D-52/D-53/D-55).
/// </para>
/// </summary>
[NoGameModeScale]
public class RedNeedleCaterpillar : Caterpillar
{
	/// <summary>与玩家距离不低于4格时: the volley's minimum firing range, in tiles.</summary>
	private const float MinVolleyRangeTiles = 4f;

	/// <summary>Pixels per tile, so a world-space distance can be compared with the design's tile ranges.</summary>
	private const float PixelsPerTile = 16f;

	/// <summary>每过180帧: the volley interval, in frames.</summary>
	private const int VolleyIntervalFrames = 180;

	/// <summary>发射4~6尖刺: the volley's minimum spike count.</summary>
	private const int MinSpikes = 4;

	/// <summary>发射4~6尖刺: the volley's maximum spike count.</summary>
	private const int MaxSpikes = 6;

	/// <summary>伤害 20（近战）15（远程）: the needle's ranged damage.</summary>
	private const int SpikeDamage = 15;

	/// <summary>
	/// 像尖刺史莱姆一样: the cone the needles fan across. The design fixes the same 4~6 count the vanilla
	/// spike slime uses, so its own spread is the conservative default here (D-54).
	/// </summary>
	private const float MaxSpreadRadians = MathHelper.Pi / 9f;

	/// <summary>The needle's launch speed, in pixels per tick (D-54 default).</summary>
	private const float SpikeSpeed = 7f;

	/// <summary>25%概率会造成中毒20秒: 20 s = 1200 ticks, rolled 1 in 4.</summary>
	private const int PoisonLongTicks = 1200;

	/// <summary>The 25% long-poison denominator.</summary>
	private const int PoisonLongChanceDenominator = 4;

	/// <summary>37.5%概率造成中毒10秒: 10 s = 600 ticks, rolled 3 in 8.</summary>
	private const int PoisonShortTicks = 600;

	/// <summary>The 37.5% short-poison numerator (3 of 8).</summary>
	private const int PoisonShortChanceNumerator = 3;

	/// <summary>The 37.5% short-poison denominator (3 of 8).</summary>
	private const int PoisonShortChanceDenominator = 8;

	/// <summary>
	/// Named wrapper over <c>NPC.localAI[0]</c>: the frames since the last volley, so no bare numeric
	/// index is scattered through the class.
	/// </summary>
	private int VolleyTimer
	{
		get => (int)NPC.localAI[0];
		set => NPC.localAI[0] = (int)value;
	}

	// No HJSON key is created for this class; localization stays deferred (D-20).
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.NPCs;

	// Approved artwork is missing from the repository (D-48). The shared fallback keeps the class
	// loadable, and the phase's gate asserts the override until the art arrives.
	public override string Texture => Commons.ModAsset.White_Mod;

	// The template's PreDraw slices the texture as texture.Width / 3, so the fallback must still divide
	// into three usable frames: White_Mod is 256x256, so each frame is 85x256 and the template's own
	// GetDrawFrame stays valid. No GetDrawFrame override is needed.

	public override void SetStaticDefaults()
	{
		// No approved sprite exists, so there is nothing to slice: the whole texture is one frame.
		Main.npcFrameCount[NPC.type] = 1;
		NPCSpawnManager.RegisterNPC(Type);

		// 免疫 中毒 (the design row's 免疫 cell): the poisoned debuff is the only immunity.
		NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
	}

	public override void SetDefaults()
	{
		base.SetDefaults();

		// The segmented-worm geometry. No approved sprite exists to measure, so the precedent's own
		// conservative values are used and are the first thing to revisit when the art arrives (D-54).
		SegmentBehavioralSize = 10;
		SegmentHitBoxSize = 30;
		SegmentCount = 10;
		AnimationSpeed = 2;

		// The template's own HitEffect emits DustType with no dedicated-server guard, so -1 disables it
		// and this class emits the same dust from its own guarded HitEffect instead (D-35/T-04-46).
		DustType = -1;

		// 生命 60 / 伤害 20 / 防御 4 / 钱币（铜） 80铜. [NoGameModeScale] keeps the design's own values from
		// being scaled a second time, so no mode branch touches them.
		NPC.lifeMax = 60;
		NPC.life = 60;
		NPC.damage = 20;
		NPC.defDamage = 20;
		NPC.defense = 4;
		NPC.defDefense = 4;
		NPC.value = 80;

		// 击退抗性 -12/-8/4 maps onto the BarkSpicyCaterpillar precedent's negative progression: a
		// negative resistance is valid in this repository and needs no clamp (04-DEVIATIONS.md section 10).
		NPC.knockBackResist = -0.12f;
		if (Main.expertMode)
		{
			NPC.knockBackResist = -0.08f;
		}
		if (Main.masterMode)
		{
			NPC.knockBackResist = -0.04f;
		}

		// Conservative default: the design's 稀有度 cell is empty, and NPC.rarity is the engine's only
		// NPC rarity field (the NPC.rare -> NPC.rarity correction, 04-DEVIATIONS.md section 10).
		NPC.rarity = ItemRarityID.White;

		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;
		SpawnModBiomes = [ModContent.GetInstance<KelpCurtainBiome>().Type];
	}

	/// <summary>
	/// The template's coroutine worm AI plus the design's needle volley. The volley is spawned once, on
	/// the authoritative side, so clients never duplicate it (D-55).
	/// </summary>
	public override void AI()
	{
		base.AI();

		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			UpdateVolley();
		}
	}

	/// <summary>
	/// 与玩家距离不低于4格时，每过180帧会像尖刺史莱姆一样在头部发射4~6尖刺: the design's volley, run on the
	/// authoritative side only. The origin is the synced <c>NPC.Center</c>, never
	/// <c>Segments[0].SelfPosition</c>: the template's segment simulation consumes <c>Main.rand</c> on every
	/// side and is never synchronised, so a segment-derived origin would be per-side state and the needles
	/// would not leave the head the client is rendering (D-55). The distance is computed from the synced
	/// <c>NPC.Center</c> and the target's centre, never from a screen or camera value.
	/// </summary>
	private void UpdateVolley()
	{
		NPC.TargetClosest(false);
		if (NPC.target < 0 || NPC.target >= Main.maxPlayers)
		{
			return;
		}

		Player target = Main.player[NPC.target];
		if (!target.active || target.dead)
		{
			return;
		}

		// 在头部: the design's head is a per-side segment offset with no synced slot, so the volley is
		// anchored on the synced centre instead; both sides then agree on the origin (D-55).
		Vector2 origin = NPC.Center;
		if (Vector2.Distance(origin, target.Center) < MinVolleyRangeTiles * PixelsPerTile)
		{
			// 与玩家距离不低于4格时才能发射: closer than four tiles there is no volley at all.
			return;
		}

		VolleyTimer++;
		if (VolleyTimer < VolleyIntervalFrames)
		{
			return;
		}

		VolleyTimer = 0;
		FireVolley(origin, target.Center);
		NPC.netUpdate = true;
	}

	/// <summary>
	/// 在头部发射4~6尖刺: one volley of the design's needles, fanned across <see cref="MaxSpreadRadians"/>
	/// in the design's spiky-slime pattern. Called only from the authoritative side (D-55).
	/// </summary>
	/// <param name="origin">The volley's world origin, the synced <c>NPC.Center</c>.</param>
	/// <param name="targetCenter">The prey's centre, used as the volley's aim.</param>
	private void FireVolley(Vector2 origin, Vector2 targetCenter)
	{
		int spikeCount = Main.rand.Next(MinSpikes, MaxSpikes + 1);
		Vector2 aim = (targetCenter - origin).SafeNormalize(Vector2.UnitX);
		for (int i = 0; i < spikeCount; i++)
		{
			float spread = spikeCount <= 1 ? 0f : MathHelper.Lerp(-MaxSpreadRadians, MaxSpreadRadians, i / (float)(spikeCount - 1));
			Projectile.NewProjectile(NPC.GetSource_FromAI(), origin, aim.RotatedBy(spread) * SpikeSpeed, ModContent.ProjectileType<RedNeedleCaterpillar_Spike>(), SpikeDamage, 0f);
		}
	}

	/// <summary>
	/// 命中后有25%概率会造成中毒20秒，37.5%概率造成中毒10秒: the design's contact split, rolled as the literal
	/// probabilities (1 in 4, then 3 in 8 of the remainder). tML documents <c>ModNPC.OnHitPlayer</c> as
	/// running on the local client only, so this is deliberately NOT wrapped in a multiplayer-client
	/// guard; a guard would make the design's split dead code in multiplayer (the 叶飞棍 precedent).
	/// </summary>
	/// <param name="target">The player the worm touched.</param>
	/// <param name="hurtInfo">The resolved hit.</param>
	public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
	{
		if (Main.rand.NextBool(PoisonLongChanceDenominator))
		{
			target.AddBuff(BuffID.Poisoned, PoisonLongTicks);
			return;
		}

		if (Main.rand.Next(PoisonShortChanceDenominator) < PoisonShortChanceNumerator)
		{
			target.AddBuff(BuffID.Poisoned, PoisonShortTicks);
		}
	}

	/// <summary>
	/// The hit and death dust the template would otherwise emit unguarded through its <c>DustType</c>
	/// path. Client-only work, so the whole body sits behind the dedicated-server guard (D-35/D-55) and
	/// reuses an existing Kelp Curtain dust rather than creating a new dust class (D-51).
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
			int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<KelpMoss_dust>(), 2 * hit.HitDirection, -2f);
			Main.dust[dust].noGravity = true;
			Main.dust[dust].scale = Main.rand.NextFloat(0.7f, 1.2f);
		}
	}

	/// <summary>
	/// Subworld-only spawning (BIO-06). <c>NPCSpawnManager.EditSpawnPool</c> returns early outside
	/// Yggdrasil, so this per-creature gate is the real isolation, and
	/// <see cref="KelpCurtainBiome.IsKelpCurtainLayer"/> is the server-safe layer predicate because the
	/// hook runs in single player or on the server only, where the client camera is zero (D-52/D-55).
	/// 红针洋辣子 is a land crawler, so the shared dry-land condition is used and the design supplies no
	/// weight (the conservative land band, D-54).
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
	/// 掉落同树皮刺毛虫: the pre-existing implemented <c>CaterpillarJuice</c>, wired with exactly the rule
	/// <c>BarkSpicyCaterpillar.cs</c> uses. This is the OQ1 WIRE decision - the reference is compile-safe
	/// and creates no new item scope (04-DEVIATIONS.md section 6.1) - which is why the phase's gate
	/// resolves item types over the whole Sources/Modules/Yggdrasil tree.
	/// </summary>
	/// <param name="npcLoot">The loot table being built.</param>
	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
		npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CaterpillarJuice>(), 1, 1, 2));
	}
}

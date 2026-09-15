using Everglow.Commons.Coroutines;
using Everglow.Commons.Templates.Enemies;
using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.YggdrasilTown.Biomes;
using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;

namespace Everglow.Yggdrasil.YggdrasilTown.NPCs;

public class CarapaceCaterpillar : Caterpillar
{
	public override void SetDefaults()
	{
		base.SetDefaults();
		SegmentBehavioralSize = 16;
		SegmentHitBoxSize = 30;
		SegmentCount = 10;
		AnimationSpeed = 3;
		NPC.knockBackResist = 0.05f;
		NPC.lifeMax = 55;
		NPC.damage = 10;
		NPC.defense = 15;
		NPC.value = 20;
		NPC.HitSound = SoundID.NPCHit2;
		NPC.DeathSound = SoundID.NPCDeath1;
	}

	public override void OnSpawn(IEntitySource source)
	{
		Segments = new List<Segment>();
		int startDir = 1;
		if (Main.rand.NextBool(2))
		{
			startDir = -1;
		}
		for (int i = 0; i < SegmentCount; i++)
		{
			Segment segment = new Segment();
			segment.Style = 2;
			if (i == 0)
			{
				segment.Style = 0;
			}
			if (i == 1)
			{
				segment.Style = 1;
			}
			if (i == SegmentCount - 1)
			{
				segment.Style = 3;
			}
			segment.SelfPosition = new Vector2((i - (SegmentCount / 2f - 1)) * SegmentBehavioralSize * startDir, 0);
			segment.Normal = new Vector2(0, -1 * startDir);
			segment.SelfDirection = startDir;
			Segments.Add(segment);
		}
		NPC.velocity *= 0;
		Toughness = MaxToughness;
		_caterpillarCoroutine.StartCoroutine(new Coroutine(Falling()));
	}

	public override Rectangle GetDrawFrame(int Style)
	{
		Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
		int height = texture.Height;
		if (Style == 0)
		{
			return new Rectangle(0, 0, 36, height);
		}
		if (Style == 1)
		{
			return new Rectangle(38, 0, 22, height);
		}
		if (Style == 2)
		{
			return new Rectangle(62, 0, 32, height);
		}
		if (Style == 3)
		{
			return new Rectangle(96, 0, 30, height);
		}
		return base.GetDrawFrame(Style);
	}

	public override void SetStaticDefaults()
	{
		NPCSpawnManager.RegisterNPC(Type);
	}

	public override float SpawnChance(NPCSpawnInfo spawnInfo)
	{
		YggdrasilTownBiome YggdrasilTownBiome = ModContent.GetInstance<YggdrasilTownBiome>();
		if (!YggdrasilTownBiome.IsBiomeActive(Main.LocalPlayer))
		{
			return 0f;
		}

		return 8f;
	}

	public override void HitEffect(NPC.HitInfo hit)
	{
		for (int j = 0; j < Segments.Count; j++)
		{
			Vector2 pos = NPC.Center + Segments[j].SelfPosition;
			Dust.NewDustDirect(pos - new Vector2(SegmentHitBoxSize / 2), SegmentHitBoxSize / 2, SegmentHitBoxSize / 2, ModContent.DustType<CrimsonBlood>());
		}
	}

	public override bool PreKill()
	{
		for (int j = 0; j < Segments.Count; j++)
		{
			Vector2 v0 = new Vector2(0, Main.rand.NextFloat(0, 6f)).RotatedByRandom(MathHelper.TwoPi);
			int type = ModContent.Find<ModGore>("Everglow/CarapaceCaterpillar_gore2").Type;
			if (j == 0)
			{
				type = ModContent.Find<ModGore>("Everglow/CarapaceCaterpillar_gore0").Type;
			}
			if (j == 1)
			{
				type = ModContent.Find<ModGore>("Everglow/CarapaceCaterpillar_gore1").Type;
			}
			if (j == Segments.Count - 1)
			{
				type = ModContent.Find<ModGore>("Everglow/CarapaceCaterpillar_gore3").Type;
			}
			Gore.NewGore(NPC.GetSource_Death(), NPC.Center + Segments[j].SelfPosition, v0, type, NPC.scale);
		}
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < Segments.Count; j++)
			{
				Vector2 pos = NPC.Center + Segments[j].SelfPosition;
				Dust.NewDustDirect(pos - new Vector2(SegmentHitBoxSize / 2), SegmentHitBoxSize / 2, SegmentHitBoxSize / 2, ModContent.DustType<CrimsonBlood>());
			}
		}
		return base.PreKill();
	}

	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
		npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CaterpillarJuice>(), 1, 2, 4));
	}
}

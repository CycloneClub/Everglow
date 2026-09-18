using Everglow.Commons.Mechanics.Miscs;
using Everglow.Commons.Templates.Enemies;
using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.YggdrasilTown.Dusts;

namespace Everglow.Yggdrasil.YggdrasilTown.NPCs;

/// <summary>
/// 这种棕色毛虫仅作为模板供参考不会自然生成
/// </summary>
[NoGameModeScale]
public class BrownCaterpillar : Caterpillar
{
	public override void SetStaticDefaults()
	{
		NPCSpawnManager.RegisterNPC(Type);
	}

	public override void SetDefaults()
	{
		DustType = ModContent.DustType<VerdantBlood>();
	}

	public override float SpawnChance(NPCSpawnInfo spawnInfo)
	{
		return 0f;
	}

	public override bool PreKill()
	{
		for (int j = 0; j < Segments.Count; j++)
		{
			Vector2 v0 = new Vector2(0, Main.rand.NextFloat(0, 6f)).RotatedByRandom(MathHelper.TwoPi);
			int type = ModContent.Find<ModGore>("Everglow/BrownCaterpillar_gore1").Type;
			if (j == 0)
			{
				type = ModContent.Find<ModGore>("Everglow/BrownCaterpillar_gore0").Type;
			}
			if (j == Segments.Count - 1)
			{
				type = ModContent.Find<ModGore>("Everglow/BrownCaterpillar_gore2").Type;
			}
			Gore.NewGore(NPC.GetSource_Death(), NPC.Center + Segments[j].SelfPosition, v0, type, NPC.scale);
			if (Main.rand.NextBool(2))
			{
				v0 = new Vector2(0, Main.rand.NextFloat(0, 6f)).RotatedByRandom(MathHelper.TwoPi);
				type = ModContent.Find<ModGore>("Everglow/BrownCaterpillar_gore3").Type;
				Gore.NewGore(NPC.GetSource_Death(), NPC.Center + Segments[j].SelfPosition, v0, type, NPC.scale);
			}
		}
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < Segments.Count; j++)
			{
				Vector2 pos = NPC.Center + Segments[j].SelfPosition;
				Dust.NewDustDirect(pos - new Vector2(20), 20, 20, ModContent.DustType<VerdantBlood>());
			}
		}
		return base.PreKill();
	}
}

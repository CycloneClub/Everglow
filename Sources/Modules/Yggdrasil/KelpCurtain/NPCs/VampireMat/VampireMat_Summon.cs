using Everglow.Yggdrasil.KelpCurtain.Biomes;
using Everglow.Yggdrasil.KelpCurtain.VFXs.VampireMat;
using Everglow.Yggdrasil.WorldGeneration;

namespace Everglow.Yggdrasil.KelpCurtain.NPCs.VampireMat;

public class VampireMat_Summon : ModPlayer
{
	public static int VampireMatSummonCD = 0;

	public override void PostUpdate()
	{
		if (VampireMatSummonCD > 0)
		{
			VampireMatSummonCD--;
		}
		base.PostUpdate();
	}

	public override void OnHurt(Player.HurtInfo info)
	{
		if (VampireMatSummonCD <= 0 && Player.InModBiome<DeathJadeLakeBiome>())
		{
			if (NPC.CountNPCS(ModContent.NPCType<VampireMat>()) <= 0)
			{
				Vector2 center = KelpCurtainGeneration.VampireMatCaveCenter;
				Vector2 toCenter = center - Player.Center;
				float dis = toCenter.Length();
				if (dis < 880)
				{
					NPC.NewNPCDirect(Player.GetSource_FromAI(), center + toCenter.NormalizeSafe() * 800, ModContent.NPCType<VampireMat>());
					Main.NewText("Strange stirrings in the pitch-black depths send a chill down your spine.", Color.Purple);
					var screenEffectVFX = new ScreenScaringEffect()
					{
						Active = true,
						Visible = true,
						Timer = 0,
						MaxTime = 120,
					};
					Ins.VFXManager.Add(screenEffectVFX);
				}
			}
		}
		base.OnHurt(info);
	}
}

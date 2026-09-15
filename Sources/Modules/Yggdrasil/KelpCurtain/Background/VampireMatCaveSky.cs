using Everglow.Commons.Mechanics.EliminateLight;
using Everglow.Commons.Utilities.BackgroundHelper;
using Everglow.Yggdrasil.KelpCurtain.NPCs.VampireMat;
using Everglow.Yggdrasil.WorldGeneration;

namespace Everglow.Yggdrasil.KelpCurtain.Background;

public class VampireMatCaveSky : BackgroundSlideBase
{
	public override void SetDefaults()
	{
		base.SetDefaults();
		Texture = ModAsset.VampireMatCaveSky.Value;
		Distance = 2.8f;
		UseColorStyle = 2;
		LayerPriority = 2;
		Shader = Effects.XWrap_YClamp_Shader;
	}

	public override void Draw()
	{
		Main.spriteBatch.Draw(Texture, WorldAnchor - Main.screenPosition, null, Color.White, 0, Texture.Size() * 0.5f, new Vector2(2f, 2f), SpriteEffects.None, 0);
		EliminateLightManager.AddCircle(WorldAnchor, 65 * 16);
		foreach (var npc in Main.npc)
		{
			if (npc is not null && npc.active && npc.type == ModContent.NPCType<VampireMat>())
			{
				VampireMat mat = npc.ModNPC as VampireMat;
				if (mat is not null)
				{
					if (mat.DiveAtBackground)
					{
						VampireMat.DrawSelf(npc, mat, Main.spriteBatch, Lighting.GetColor(npc.Center.ToTileCoordinates()));
					}
				}
			}
		}
	}

	public override bool CanActive()
	{
		return (Main.LocalPlayer.Center - KelpCurtainGeneration.VampireMatCaveCenter).Length() < new Vector2(Main.screenWidth, Main.screenHeight).Length() / 2f + 60 * 16;
	}
}

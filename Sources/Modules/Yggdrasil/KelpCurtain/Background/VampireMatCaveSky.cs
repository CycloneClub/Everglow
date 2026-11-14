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

	/// <summary>
	/// The cave sky is an entirely opaque sprite drawn at scale 2 after the lake layers.
	/// Its anchor is assigned from VampireMatCaveCenter when the slide is registered.
	/// </summary>
	internal static Vector4 GetLakeOcclusionBounds()
	{
		if (Main.dedServ || !YggdrasilWorld.InYggdrasil)
		{
			return Vector4.Zero;
		}
		var backgrounds = ModContent.GetInstance<BackgroundSystem>();
		if (!backgrounds.HasBgSlide(typeof(VampireMatCaveSky).FullName))
		{
			return Vector4.Zero;
		}
		Vector2 center = KelpCurtainGeneration.VampireMatCaveCenter;
		Vector2 halfSize = ModAsset.VampireMatCaveSky.Value.Size();
		return new Vector4(center.X - halfSize.X, center.Y - halfSize.Y, center.X + halfSize.X, center.Y + halfSize.Y);
	}

	/// <summary>
	/// Subtract the opaque cave from a horizontal background row (left, top, right, bottom).
	/// Keep rows crossing its vertical edges intact to avoid changing UVs or introducing edge seams.
	/// The output span must hold two rectangles.
	/// </summary>
	public static int ClipBackgroundRow(Vector4 row, Vector4 hidden, Span<Vector4> visible)
	{
		if (row.Z <= row.X || row.W <= row.Y)
		{
			return 0;
		}
		if (hidden.Z <= hidden.X || hidden.W <= hidden.Y
			|| row.Y < hidden.Y || row.W > hidden.W || row.Z <= hidden.X || row.X >= hidden.Z)
		{
			visible[0] = row;
			return 1;
		}

		int count = 0;
		if (row.X < hidden.X)
		{
			visible[count++] = new Vector4(row.X, row.Y, hidden.X, row.W);
		}
		if (row.Z > hidden.Z)
		{
			visible[count++] = new Vector4(hidden.Z, row.Y, row.Z, row.W);
		}
		return count;
	}

	public override bool CanActive()
	{
		return (Main.LocalPlayer.Center - KelpCurtainGeneration.VampireMatCaveCenter).Length() < new Vector2(Main.screenWidth, Main.screenHeight).Length() / 2f + 60 * 16;
	}
}

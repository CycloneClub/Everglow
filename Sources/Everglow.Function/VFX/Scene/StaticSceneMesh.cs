using Everglow.Commons.Vertex;

namespace Everglow.Commons.VFX.Scene;

/// <summary>
/// A cached static scene mesh. Positions and UVs are built only once; colors can be refreshed separately at a lower frequency.
/// Designed for room scenes whose world geometry never changes, to avoid rebuilding thousands of vertices per frame.
/// </summary>
public class StaticSceneMesh
{
	public Vertex2D[] Vertices;

	/// <summary>
	/// Frames elapsed since the last color refresh. Managed by the caller.
	/// </summary>
	public int ColorRefreshTimer;

	/// <summary>
	/// Build the whole mesh once, with the same layout as <see cref="SceneUtils.DrawMultiSceneTowardBottom" />.
	/// </summary>
	public void Build(int i, int j, Texture2D texture, bool flipH, float colorFactors = 1)
	{
		var bars = new List<Vertex2D>(texture.Width / 16 * (texture.Height / 16) * 6);
		SceneUtils.DrawMultiSceneTowardBottom(i, j, texture, bars, flipH, colorFactors);
		Vertices = bars.ToArray();
	}

	/// <summary>
	/// Recompute vertex colors from current lighting. Positions and UVs stay untouched.
	/// </summary>
	public void RefreshColors(float colorFactors = 1)
	{
		if (Vertices is null)
		{
			return;
		}
		for (int k = 0; k < Vertices.Length; k++)
		{
			Vertices[k].color = Lighting.GetColor(Vertices[k].position.ToTileCoordinates()) * colorFactors;
		}
	}
}

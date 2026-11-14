using System.Reflection;
using Everglow.Commons.Vertex;
using Everglow.Yggdrasil.KelpCurtain.VFXs.VampireMat;
using Microsoft.Xna.Framework;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
public class VampireMatScreenRenderingTests
{
	[TestMethod]
	public void OverlayQuadsKeepOffsetsFadeAndUnclampedRepetitionGain()
	{
		var method = typeof(ScreenScaringEffect).GetMethod("BuildOverlayVertices", BindingFlags.Static | BindingFlags.Public);
		Assert.IsNotNull(method, "The repeated overlay should use two reusable quads.");
		var build = method.CreateDelegate<BuildOverlay>();
		Span<Vertex2D> vertices = stackalloc Vertex2D[8];
		build(vertices, new Vector2(1920, 1080), 12, 0.5f);

		Assert.AreEqual(new Vector2(12, 0), vertices[0].position);
		Assert.AreEqual(new Vector2(1932, 1080), vertices[3].position);
		Assert.AreEqual(new Vector2(-12, 0), vertices[4].position);
		Assert.AreEqual(new Vector2(1908, 1080), vertices[7].position);
		Assert.AreEqual(new Color(102, 0, 0, 0), vertices[0].color);
		Assert.AreEqual(new Color(0, 51, 0, 0), vertices[4].color);
		Assert.AreEqual(new Vector3(0, 0, 5), vertices[0].texCoord);
		Assert.AreEqual(new Vector3(1, 1, 5), vertices[7].texCoord);

		build(vertices, new Vector2(800, 600), -7, 0);
		Assert.AreEqual(new Vector2(-7, 0), vertices[0].position);
		Assert.AreEqual(new Vector2(7, 0), vertices[4].position);
		Assert.AreEqual(Color.Transparent, vertices[0].color);
		Assert.AreEqual(Color.Transparent, vertices[4].color);
	}

	private delegate void BuildOverlay(Span<Vertex2D> vertices, Vector2 size, float offset, float fade);
}

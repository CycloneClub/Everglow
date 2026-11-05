using Microsoft.Xna.Framework;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
public class VampireMatTerrainPerformanceTests
{
	[TestMethod]
	public void CaveOcclusionRemovesCoveredRowsAndPreservesOutsideGeometry()
	{
		ClipRow clip = Everglow.Yggdrasil.KelpCurtain.Background.VampireMatCaveSky.ClipBackgroundRow;
		Span<Vector4> visible = stackalloc Vector4[2];
		var hidden = new Vector4(100, 100, 2100, 2100);
		Assert.AreEqual(0, clip(new Vector4(200, 200, 2000, 216), hidden, visible));
		Assert.AreEqual(2, clip(new Vector4(0, 200, 2200, 216), hidden, visible));
		Assert.AreEqual(new Vector4(0, 200, 100, 216), visible[0]);
		Assert.AreEqual(new Vector4(2100, 200, 2200, 216), visible[1]);

		var boundaryRow = new Vector4(0, 90, 2200, 106);
		Assert.AreEqual(1, clip(boundaryRow, hidden, visible));
		Assert.AreEqual(boundaryRow, visible[0], "A row crossing the top edge must remain visible.");
		Assert.AreEqual(1, clip(boundaryRow, Vector4.Zero, visible));
		Assert.AreEqual(boundaryRow, visible[0], "Without a cave sky the original row must be preserved.");
		Assert.AreEqual(1, clip(new Vector4(2000, 200, 2200, 216), hidden, visible));
		Assert.AreEqual(new Vector4(2100, 200, 2200, 216), visible[0]);
		Assert.AreEqual(0, clip(new Vector4(200, 200, 100, 216), hidden, visible));
	}

	private delegate int ClipRow(Vector4 row, Vector4 hidden, Span<Vector4> visible);
}

using Everglow.Commons.Utilities;
using Terraria.ID;

namespace Everglow.UnitTests.Function;

[TestClass]
[DoNotParallelize]
public class AssetUtilsTest
{
	private bool _originalDedServ;

	[TestInitialize]
	public void Initialize()
	{
		Terraria.Program.SavePath = string.Empty;
		_originalDedServ = Terraria.Main.dedServ;
		Terraria.Main.dedServ = true;
	}

	[TestCleanup]
	public void Cleanup() => Terraria.Main.dedServ = _originalDedServ;

	[TestMethod]
	public void LoadVanillaItemTexture_OnDedicatedServer_ReturnsWithoutThrowing()
	{
		AssetUtils.LoadVanillaItemTexture(ItemID.GoldBar);
	}

	[TestMethod]
	public void LoadVanillaNPCTexture_OnDedicatedServer_ReturnsWithoutThrowing()
	{
		AssetUtils.LoadVanillaNPCTexture(NPCID.Guide);
	}
}

using System.Reflection;
using Everglow.Yggdrasil.Mechanics.TownCrowd;
using Microsoft.Xna.Framework;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
public class CrowdLifeRulesTests
{
	private static Type Rules
	{
		get
		{
			var type = typeof(TownCrowdSystem).Assembly.GetType("Everglow.Yggdrasil.Mechanics.TownCrowd.CrowdLifeRules");
			Assert.IsNotNull(type);
			return type;
		}
	}

	[TestMethod]
	public void InitialIntentIsIndependentOfStreetOrPylonAppearance()
	{
		var choose = Rules.GetMethod("ChooseIntent")!;
		Assert.AreEqual("ReturnHome", choose.Invoke(null, [false, false, 0, 100, 0])!.ToString());
		Assert.AreEqual("VisitTown", choose.Invoke(null, [true, false, 0, 100, 0])!.ToString());
		Assert.AreEqual("EnterGuild", choose.Invoke(null, [true, false, 0, 0, 100])!.ToString());
		Assert.AreEqual("EnterGuild", choose.Invoke(null, [false, false, 0, 0, 100])!.ToString());
		Assert.AreEqual("VisitTown", choose.Invoke(null, [false, true, 0, 0, 100])!.ToString());
	}

	[TestMethod]
	public void GuildTriggerRequiresWholeBodyInsideOpening()
	{
		var contains = Rules.GetMethod("InsideGuildOpening")!;
		Assert.IsTrue((bool)contains.Invoke(null, [new Point(10, 20), new Vector2(216, 448)])!);
		Assert.IsFalse((bool)contains.Invoke(null, [new Point(10, 20), new Vector2(160, 448)])!);
		Assert.IsFalse((bool)contains.Invoke(null, [new Point(10, 20), new Vector2(216, 480)])!);
	}

	[TestMethod]
	public void ZoomedCameraIncludesExactlyItsWorldSpaceExtent()
	{
		var camera = CrowdLifeRules.CameraBounds(new Vector2(1000, 2000), new Vector2(800, 600),
			Matrix.CreateTranslation(-400, -300, 0) * Matrix.CreateScale(2, 2, 1) * Matrix.CreateTranslation(400, 300, 0));
		Assert.AreEqual(new Rectangle(1200, 2150, 400, 300), camera);
		var regions = new CrowdRegions();
		Assert.IsTrue(regions.Observe(camera, 0).Length > 0);
		Assert.AreEqual(0, regions.Observe(camera, 1).Length);
		Assert.IsTrue(regions.Observe(new Rectangle(20000, 20000, 400, 300), 2).Length > 0);
		Assert.AreEqual(0, regions.Observe(camera, 3).Length);
	}

	[TestMethod]
	public void RegionExposureRemembersShortRevisitsAndBoundsMemory()
	{
		var type = typeof(TownCrowdSystem).Assembly.GetType("Everglow.Yggdrasil.Mechanics.TownCrowd.CrowdRegions");
		Assert.IsNotNull(type);
		var regions = Activator.CreateInstance(type)!;
		var observe = type.GetMethod("Observe")!;
		Rectangle[] Observe(Rectangle view, long clock) => (Rectangle[])observe.Invoke(regions, [view, clock])!;
		var first = new Rectangle(20, 20, 100, 100);
		Assert.AreEqual(1, Observe(first, 0).Length);
		Assert.AreEqual(0, Observe(first, 1).Length);
		Assert.AreEqual(1, Observe(new Rectangle(800, 20, 100, 100), 2).Length);
		Assert.AreEqual(0, Observe(first, 3).Length);
		for (int i = 0; i < 400; i++)
		{
			Observe(new Rectangle(i * 768, 400, 100, 100), i + 4);
		}
		Assert.IsTrue((int)type.GetProperty("Count")!.GetValue(regions)! <= 128);
		type.GetMethod("Clear")!.Invoke(regions, null);
		Assert.AreEqual(1, Observe(first, 500).Length);
	}
}

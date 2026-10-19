using Everglow.Yggdrasil.Mechanics.TownCrowd;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
public class CrowdAvoidanceTests
{
	private static Type StateType
	{
		get
		{
			var type = typeof(TownCrowdSystem).Assembly.GetType("Everglow.Yggdrasil.Mechanics.TownCrowd.CrowdAvoidance");
			Assert.IsNotNull(type);
			return type;
		}
	}

	[TestMethod]
	public void RecoveryNeedsThreeContinuousSafeSeconds()
	{
		var type = StateType;
		var state = Activator.CreateInstance(type)!;
		bool Tick(bool danger, bool safe) => (bool)type.GetMethod("Update")!.Invoke(state, [danger, safe, true, 300])!;
		bool Alarmed() => (bool)type.GetProperty("Alarmed")!.GetValue(state)!;
		Tick(true, false);
		Assert.IsTrue(Alarmed());
		for (int i = 0; i < 179; i++) { Tick(false, true); }
		Assert.IsTrue(Alarmed());
		Tick(false, false);
		for (int i = 0; i < 179; i++) { Tick(false, true); }
		Assert.IsTrue(Alarmed());
		Tick(false, true);
		Assert.IsFalse(Alarmed());
	}

	[TestMethod]
	public void ReturningOnscreenResetsRetirementAndSafeOffscreenStillRetires()
	{
		var type = StateType;
		var state = Activator.CreateInstance(type)!;
		bool Tick(bool danger, bool safe, bool visible) => (bool)type.GetMethod("Update")!.Invoke(state, [danger, safe, visible, 300])!;
		Tick(true, false, true);
		for (int i = 0; i < 299; i++) { Assert.IsFalse(Tick(false, true, false)); }
		Assert.IsFalse(Tick(false, false, true));
		Tick(true, false, true);
		for (int i = 0; i < 299; i++) { Assert.IsFalse(Tick(false, true, false)); }
		Assert.IsTrue(Tick(false, true, false));
	}
}

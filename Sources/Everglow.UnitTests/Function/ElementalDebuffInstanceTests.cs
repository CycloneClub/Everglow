using System.Reflection;
using Everglow.Commons.Mechanics.ElementalDebuff;
using Everglow.Commons.Mechanics.ElementalDebuff.Debuffs;
using Terraria;

namespace Everglow.UnitTests.Function;

[TestClass]
[DoNotParallelize]
public class ElementalDebuffInstanceTests
{
	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
	}

	[TestMethod]
	public void Threshold_SharedSourcesTriggerOnceAndDiscardOverflow()
	{
		var handler = new RecordingBurn();
		var instance = CreateInstance(handler);
		var target = new NPC { dontTakeDamage = true };
		instance.AddBuildUp(500, 0);
		instance.Update(target);
		Assert.IsFalse(instance.Proc);
		instance.AddBuildUp(500, 1);
		Assert.IsFalse(instance.Proc);
		instance.AddBuildUp(500, 2);
		instance.Update(target);
		Assert.IsTrue(instance.Proc);
		Assert.AreEqual(0, instance.BuildUp);
		Assert.AreEqual(2, instance.ProccedBy);
		Assert.AreEqual(1, handler.Triggers);
		Assert.IsFalse(instance.AddBuildUp(1000, 3));
		Assert.AreEqual(2, instance.ProccedBy);
	}

	[TestMethod]
	public void Lockout_RejectsBuildUpWithoutRefreshingAndExpiresOnLastUpdate()
	{
		var handler = new RecordingBurn();
		var instance = CreateInstance(handler);
		var target = new NPC { dontTakeDamage = true };
		instance.AddBuildUp(handler.BuildUpMax, 0);
		instance.Update(target);
		for (int tick = 1; tick < handler.Duration; tick++)
		{
			Assert.IsFalse(instance.AddBuildUp(500, 1));
			instance.Update(target);
			Assert.IsTrue(instance.Proc);
			Assert.AreEqual(handler.Duration - tick, instance.TimeLeft);
		}
		instance.Update(target);
		Assert.IsFalse(instance.Proc);
		Assert.AreEqual(0, instance.BuildUp);
		Assert.AreEqual(0, instance.TimeLeft);
		Assert.IsTrue(instance.AddBuildUp(handler.BuildUpMax, 1));
		instance.Update(target);
		Assert.AreEqual(2, handler.Triggers);
		Assert.AreEqual(1, instance.ProccedBy);
	}

	[TestMethod]
	[DataRow(0.5f, 0f, 250)]
	[DataRow(0.5f, -1f, 250)]
	[DataRow(0.5f, 0.25f, 375)]
	[DataRow(0.5f, 2f, 500)]
	[DataRow(2f, 0f, 0)]
	public void Resistance_PenetrationIsClampedAndCannotAmplifyRawBuildUp(float resistance, float penetration, int expected)
	{
		var handler = new RecordingBurn();
		handler.SetOverride(new ElementalDebuffOverride(ElementalResistance: resistance));
		var instance = CreateInstance(handler);
		instance.AddBuildUp(500, 7, penetration);
		Assert.AreEqual(expected, instance.BuildUp);
		Assert.AreEqual(expected > 0 ? 7 : -1, instance.LastInteraction);
		instance.ResetEffects();
		Assert.AreEqual(expected, instance.BuildUp);
	}

	[TestMethod]
	public void Corrosion_DotUsesRegenUnitsAndStopsWithProc()
	{
		var handler = new RecordingCorrosion();
		var instance = CreateInstance(handler);
		var target = new NPC { dontTakeDamage = true };
		instance.AddBuildUp(handler.BuildUpMax, 0);
		instance.Update(target);
		instance.ApplyDot(target);
		Assert.AreEqual(0, target.lifeRegen);
		target.dontTakeDamage = false;
		instance.ApplyDot(target);
		Assert.AreEqual(-handler.DotDamage, target.lifeRegen);
		instance.Reset();
		target.lifeRegen = 0;
		instance.ApplyDot(target);
		Assert.AreEqual(0, target.lifeRegen);
	}

	private static ElementalDebuffInstance CreateInstance(ElementalDebuffHandler handler)
	{
		var instance = new ElementalDebuffInstance();
		typeof(ElementalDebuffInstance).GetProperty(nameof(ElementalDebuffInstance.Handler), BindingFlags.Public | BindingFlags.Instance)!.SetValue(instance, handler);
		return instance;
	}

	// Avoid loading visual assets, buffs or projectile pools in the standalone test host.
	private sealed class RecordingBurn : BurnDebuff
	{
		public int Triggers { get; private set; }

		public override void PostProc(NPC npc) => Triggers++;
	}

	private sealed class RecordingCorrosion : CorrosionDebuff
	{
		public override void PostProc(NPC npc)
		{
		}
	}
}

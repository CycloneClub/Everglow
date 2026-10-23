using Everglow.Commons.Mechanics.Cooldown;
using Everglow.Commons.Utilities;
using Microsoft.Xna.Framework.Graphics;

namespace Everglow.UnitTests.Function;

[TestClass]
[DoNotParallelize]
public class CooldownLifecycleTests
{
	private CooldownTestContext context = null!;

	[TestInitialize]
	public void Initialize()
	{
		context = new CooldownTestContext();
		context.Register(ProbeCooldown.ID, typeof(ProbeCooldown));
	}

	[TestCleanup]
	public void Cleanup() => context.Dispose();

	[TestMethod]
	public void ExplicitUpdate_RespectsPauseExpiresOnceAndUsesTheRegisteredState()
	{
		context.Player.AddCooldown(ProbeCooldown.ID, 2);
		var state = context.Cooldowns.cooldowns[ProbeCooldown.ID];
		var handler = (ProbeCooldown)state.cooldown;
		handler.Paused = true;
		Tick();
		Assert.AreEqual(2, state.timeLeft);
		Assert.AreEqual(1, handler.Updates);
		handler.Paused = false;
		Tick();
		Assert.AreEqual(1, state.timeLeft);
		Assert.IsTrue(context.Player.HasCooldown(ProbeCooldown.ID));
		Tick();
		Assert.IsFalse(context.Player.HasCooldown(ProbeCooldown.ID));
		Assert.AreEqual(1, handler.Completions);
		Tick();
		Assert.AreEqual(1, handler.Completions);
	}

	[TestMethod]
	public void DuplicateStart_OverwriteFalseKeepsRemainingTimeAndOverwriteTrueRestarts()
	{
		context.Player.AddCooldown(ProbeCooldown.ID, 5);
		Tick();
		context.Player.AddCooldown(ProbeCooldown.ID, 9, overwrite: false);
		Assert.AreEqual(4, context.Cooldowns.cooldowns[ProbeCooldown.ID].timeLeft);
		context.Player.AddCooldown(ProbeCooldown.ID, 9);
		Assert.AreEqual(9, context.Cooldowns.cooldowns[ProbeCooldown.ID].timeLeft);
	}

	[TestMethod]
	public void PlayerCooldowns_AreIndependentForTheSameType()
	{
		context.Player.AddCooldown(ProbeCooldown.ID, 5);
		using var other = new CooldownTestContext();
		other.Register(ProbeCooldown.ID, typeof(ProbeCooldown));
		Assert.IsFalse(other.Player.HasCooldown(ProbeCooldown.ID));
		other.Player.AddCooldown(ProbeCooldown.ID, 9);
		Tick();
		Assert.AreEqual(4, context.Player.GetCooldown(ProbeCooldown.ID).timeLeft);
		Assert.AreEqual(9, other.Player.GetCooldown(ProbeCooldown.ID).timeLeft);
	}

	[TestMethod]
	public void HiddenCooldown_DoesNotReadGraphicsOrChangeHoveredIcon()
	{
		context.Player.AddCooldown(ProbeCooldown.ID, 5);
		string hovered = CooldownSystem.DrawCooldownIcon(null!, context.Player, "existing", ProbeCooldown.ID, default, default);
		Assert.AreEqual("existing", hovered);
	}

	[TestMethod]
	public void AutomaticUpdate_StillUsesExistingDefaultPhase()
	{
		context.Player.AddCooldown(ProbeCooldown.ID, 1);
		var handler = (ProbeCooldown)context.Cooldowns.cooldowns[ProbeCooldown.ID].cooldown;
		context.Cooldowns.PostUpdateMiscEffects();
		Assert.AreEqual(1, handler.Completions);
		Assert.IsFalse(context.Player.HasCooldown(ProbeCooldown.ID));
	}

	private void Tick() => context.Player.TickCooldown(ProbeCooldown.ID);

	public sealed class ProbeCooldown : CooldownBase
	{
		public static new string ID => nameof(ProbeCooldown);
		public override string TypeID => ID;
		public bool Paused { get; set; }
		public int Updates { get; private set; }
		public int Completions { get; private set; }
		public override bool CanTickDown => !Paused;
		public override bool IsVisible => false;
		public override Texture2D Texture => null!;
		public override void Update() => Updates++;
		public override void OnCompleted() => Completions++;
	}
}

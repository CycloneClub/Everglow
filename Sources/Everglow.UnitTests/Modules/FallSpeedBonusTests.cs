using System.Reflection;
using Everglow.Commons.Mechanics.PlayerBonuses;
using Everglow.Commons.Templates.Weapons.Clubs;
using Everglow.Commons.Utilities;
using Everglow.Food.Buffs;
using Everglow.Food.Buffs.VanillaFoodBuffs;
using Everglow.UnitTests.Function;
using Terraria;
using Terraria.ModLoader;

namespace Everglow.UnitTests.Modules;

[TestClass]
[DoNotParallelize]
public class FallSpeedBonusTests
{
	private CooldownTestContext context = null!;
	private StatBonusPlayer bonuses = null!;
	private Player[] previousPlayers = null!;
	private Projectile[] previousProjectiles = null!;
	private readonly List<Action> restoreContent = [];
	private float criticalDamage;
	private float addCritDamage;

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		criticalDamage = FoodBuffModPlayer.CriticalDamage;
		addCritDamage = FoodBuffModPlayer.AddCritDamage;
		previousPlayers = Main.player;
		previousProjectiles = Main.projectile;
		context = new CooldownTestContext();
		bonuses = context.Attach<StatBonusPlayer>();
		context.Attach<FoodBuffModPlayer>();
		context.Player.whoAmI = 0;
		context.Player.GetAttackSpeed(DamageClass.Melee) = 1f;
		Main.player = [context.Player];
		Main.projectile = [];
		BeginFrame();
	}

	[TestCleanup]
	public void Cleanup()
	{
		Main.player = previousPlayers;
		Main.projectile = previousProjectiles;
		for (int i = restoreContent.Count - 1; i >= 0; i--)
		{
			restoreContent[i]();
		}
		FoodBuffModPlayer.CriticalDamage = criticalDamage;
		FoodBuffModPlayer.AddCritDamage = addCritDamage;
		context.Dispose();
	}

	[TestMethod]
	public void FoodCapsTakeMinimumInEveryOrder()
	{
		ModBuff[] foods = [new CookedMarshmallowBuff(), new GrilledSquirrelBuff(), new RoastedDuckBuff()];
		float minimum = foods.Min(food => FoodCap(food));
		foreach (int[] order in new[] { new[] { 0, 1, 2 }, new[] { 0, 2, 1 }, new[] { 1, 0, 2 }, new[] { 1, 2, 0 }, new[] { 2, 0, 1 }, new[] { 2, 1, 0 } })
		{
			BeginFrame();
			context.Player.GetModPlayer<FoodBuffModPlayer>().ResetEffects();
			foreach (int food in order)
			{
				Apply(foods[food]);
			}

			bonuses.PostUpdateEquips();
			Assert.AreEqual(minimum, context.Player.maxFallSpeed);
		}
	}

	[TestMethod]
	public void FoodCapsUseFixedNormalBaseline()
	{
		foreach (ModBuff food in new ModBuff[] { new CookedMarshmallowBuff(), new GrilledSquirrelBuff(), new RoastedDuckBuff() })
		{
			float fromHigh = FoodCap(food);
			BeginFrame();
			context.Player.GetModPlayer<FoodBuffModPlayer>().ResetEffects();
			context.Player.maxFallSpeed = 80f;
			Apply(food);
			bonuses.PostUpdateEquips();
			Assert.AreEqual(fromHigh, context.Player.maxFallSpeed);
			Assert.IsTrue(fromHigh < 35.01f);
		}
	}

	[TestMethod]
	[DataRow(1.25f, 1f)]
	[DataRow(1.25f, -1f)]
	[DataRow(5.01f, 1f)]
	[DataRow(35.01f, -1f)]
	public void CapPreservesLowerPhysicsLimitAndDoesNotChangeGravityOrVelocity(float baseline, float gravityDirection)
	{
		float cap = FoodCap(new CookedMarshmallowBuff());
		BeginFrame();
		context.Player.GetModPlayer<FoodBuffModPlayer>().ResetEffects();
		context.Player.maxFallSpeed = baseline;
		context.Player.gravity = 0.1f;
		context.Player.gravDir = gravityDirection;
		context.Player.velocity.Y = 12 * gravityDirection;
		Apply(new CookedMarshmallowBuff());
		bonuses.PostUpdateEquips();
		Assert.AreEqual(Math.Min(baseline, cap), context.Player.maxFallSpeed);
		Assert.AreEqual(0.1f, context.Player.gravity);
		Assert.AreEqual(gravityDirection, context.Player.gravDir);
		Assert.AreEqual(12 * gravityDirection, context.Player.velocity.Y);
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void FrogLegBonusSettlesBeforeFoodCap(bool reverse)
	{
		float marshmallow = FoodCap(new CookedMarshmallowBuff());
		BeginFrame();
		context.Player.GetModPlayer<FoodBuffModPlayer>().ResetEffects();
		ModBuff[] foods = [new SauteedFrogLegsBuff(), new CookedMarshmallowBuff()];
		foreach (var food in reverse ? foods.Reverse() : foods)
		{
			Apply(food);
		}
		bonuses.PostUpdateEquips();
		Assert.AreEqual(marshmallow, context.Player.maxFallSpeed);
		BeginFrame();
		context.Player.GetModPlayer<FoodBuffModPlayer>().ResetEffects();
		Apply(new SauteedFrogLegsBuff());
		float immediate = context.Player.maxFallSpeed;
		Assert.IsTrue(immediate > marshmallow, "The native bonus applies immediately without settlement.");
		bonuses.PostUpdateEquips();
		Assert.AreEqual(immediate, context.Player.maxFallSpeed, 0.00001f);
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void ActionOverrideOutranksCapsWithoutAccumulatingAndExpiresNextFrame(bool reverse)
	{
		float cap = FoodCap(new CookedMarshmallowBuff());
		BeginFrame();
		context.Player.GetModPlayer<FoodBuffModPlayer>().ResetEffects();
		Action[] contributions =
		[
			() => context.Player.maxFallSpeed += 5f,
			() => Apply(new CookedMarshmallowBuff()),
			() => context.Player.AddStat(StatBonus.MaxFallSpeedForSpecialBehavior, 8000f),
			() => context.Player.AddStat(StatBonus.MaxFallSpeedForSpecialBehavior, 10000f),
			() => context.Player.AddStat(StatBonus.MaxFallSpeedForSpecialBehavior, 10000f),
		];
		foreach (var contribute in reverse ? contributions.Reverse() : contributions)
		{
			contribute();
		}
		bonuses.PostUpdateEquips();
		Assert.AreEqual(10000f, context.Player.maxFallSpeed);
		BeginFrame();
		Apply(new CookedMarshmallowBuff());
		bonuses.PostUpdateEquips();
		Assert.AreEqual(cap, context.Player.maxFallSpeed);
		BeginFrame();
		bonuses.PostUpdateEquips();
		Assert.AreEqual(10.01f, context.Player.maxFallSpeed);
	}

	[TestMethod]
	[DataRow(false, 0, 24, 0, false)]
	[DataRow(true, 0, 10, 0, true)]
	[DataRow(true, 0, 26, 0, true)]
	[DataRow(true, 0, 30, 1, true)]
	[DataRow(true, 0, 30, 2, true)]
	[DataRow(true, 1, 26, 0, false)]
	public void ClubUsesExistingOwnerRegistrationThroughoutActionAndRestoresCapOnKill(bool spawned, int owner, int timer, int attack, bool overridden)
	{
		float cap = FoodCap(new CookedMarshmallowBuff());
		BeginFrame();
		context.Player.GetModPlayer<FoodBuffModPlayer>().ResetEffects();
		var owners = (List<int>)typeof(ClubProjSmash).GetField("ownSmashClubPlayers", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
		int[] previousOwners = owners.ToArray();
		owners.Clear();
		try
		{
			Main.player = [context.Player, new Player { active = true, whoAmI = 1 }];
			var clubPlayer = context.Attach<ClubProjSmashPlayer>();
			var projectile = AttachProjectile(new TestClubSmash(), true, owner);
			var club = (TestClubSmash)projectile.ModProjectile;
			club.timer = timer;
			club.currantAttackType = attack;
			if (spawned)
			{
				club.OnSpawn(null!);
			}
			Apply(new CookedMarshmallowBuff());
			clubPlayer.UpdateEquips();
			bonuses.PostUpdateEquips();
			if (overridden)
			{
				Assert.IsTrue(context.Player.maxFallSpeed > cap);
			}
			else
			{
				Assert.AreEqual(cap, context.Player.maxFallSpeed);
			}
			club.OnKill(0);
			BeginFrame();
			Apply(new CookedMarshmallowBuff());
			clubPlayer.UpdateEquips();
			bonuses.PostUpdateEquips();
			Assert.AreEqual(cap, context.Player.maxFallSpeed);
		}
		finally
		{
			owners.Clear();
			owners.AddRange(previousOwners);
		}
	}

	[TestMethod]
	[DataRow(0, 0, false)]
	[DataRow(1, 0, true)]
	[DataRow(0, 1, true)]
	[DataRow(1, 1, true)]
	public void SpearUsesOriginalOwnedCountsAndRestoresCapWhenBothDisappear(int normalCount, int downCount, bool overridden)
	{
		float cap = FoodCap(new CookedMarshmallowBuff());
		BeginFrame();
		context.Player.GetModPlayer<FoodBuffModPlayer>().ResetEffects();
		var myth = AttachMythPlayer();
		context.Player.ownedProjectileCounts[1] = normalCount;
		context.Player.ownedProjectileCounts[2] = downCount;
		Apply(new CookedMarshmallowBuff());
		myth.UpdateEquips();
		bonuses.PostUpdateEquips();
		if (overridden)
		{
			Assert.IsTrue(context.Player.maxFallSpeed > cap);
		}
		else
		{
			Assert.AreEqual(cap, context.Player.maxFallSpeed);
		}
		BeginFrame();
		context.Player.ownedProjectileCounts[1] = 0;
		context.Player.ownedProjectileCounts[2] = 0;
		Apply(new CookedMarshmallowBuff());
		myth.UpdateEquips();
		bonuses.PostUpdateEquips();
		Assert.AreEqual(cap, context.Player.maxFallSpeed);
	}

	[TestMethod]
	[DataRow(0)]
	[DataRow(1)]
	public void SilverBonusesApplyTogetherInUpdateEquipsWithoutRepeatingInMiscEffects(int silverBuff)
	{
		float cap = FoodCap(new CookedMarshmallowBuff());
		BeginFrame();
		context.Player.GetModPlayer<FoodBuffModPlayer>().ResetEffects();
		var myth = AttachMythPlayer();
		myth.GetType().GetField("SilverBuff")!.SetValue(myth, silverBuff);
		context.Player.maxRunSpeed = 3f;
		context.Player.wingAccRunSpeed = 4f;
		context.Player.GetDamage(DamageClass.Generic) = StatModifier.Default + 0.2f;
		myth.UpdateEquips();
		var first = Snapshot();
		bonuses.PostUpdateEquips();
		myth.PostUpdateMiscEffects();
		var second = Snapshot();
		Assert.AreEqual(first.Fall, second.Fall, 0.00001f);
		Assert.AreEqual(first.Run, second.Run, 0.00001f);
		Assert.AreEqual(first.Wing, second.Wing, 0.00001f);
		Assert.AreEqual(first.Damage, second.Damage, 0.00001f);
		if (silverBuff > 0)
		{
			Assert.IsTrue(first.Fall > 10.01f);
			Assert.IsTrue(first.Run > 3f);
			Assert.IsTrue(first.Wing > 4f);
			Assert.IsTrue(first.Damage > 1.2f);
		}
		else
		{
			Assert.AreEqual(10.01f, first.Fall, 0.00001f);
			Assert.AreEqual(3f, first.Run, 0.00001f);
			Assert.AreEqual(4f, first.Wing, 0.00001f);
			Assert.AreEqual(1.2f, first.Damage, 0.00001f);
		}

		BeginFrame();
		Apply(new CookedMarshmallowBuff());
		myth.UpdateEquips();
		bonuses.PostUpdateEquips();
		myth.PostUpdateMiscEffects();
		Assert.AreEqual(cap, context.Player.maxFallSpeed);

		(float Fall, float Run, float Wing, float Damage) Snapshot() =>
			(context.Player.maxFallSpeed, context.Player.maxRunSpeed, context.Player.wingAccRunSpeed, context.Player.GetDamage(DamageClass.Generic).ApplyTo(1f));
	}

	private void BeginFrame()
	{
		bonuses.ResetEffects();
		context.Player.maxFallSpeed = 10.01f;
	}

	private float FoodCap(ModBuff food)
	{
		BeginFrame();
		context.Player.GetModPlayer<FoodBuffModPlayer>().ResetEffects();
		context.Player.maxFallSpeed = 35.01f;
		Apply(food);
		bonuses.PostUpdateEquips();
		return context.Player.maxFallSpeed;
	}

	private void Apply(ModBuff buff)
	{
		int index = 0;
		buff.Update(context.Player, ref index);
	}

	private static Projectile AttachProjectile(ModProjectile prototype, bool active, int owner)
	{
		var projectile = new Projectile { active = active, owner = owner };
		var instance = prototype.NewInstance(projectile);
		typeof(Projectile).GetProperty(nameof(Projectile.ModProjectile))!.SetValue(projectile, instance);
		Main.projectile = [projectile];
		return projectile;
	}

	private ModPlayer AttachMythPlayer()
	{
		var assembly = MythAssembly();
		string spearName = "Everglow.Myth.Misc.Projectiles.Weapon.Melee.PrimordialJadeWinged_Spear.PrimordialJadeWinged_Spear";
		for (int i = 0; i < 2; i++)
		{
			var type = assembly.GetType(spearName + (i == 0 ? string.Empty : "Down"), true)!;
			var content = typeof(ContentInstance<>).MakeGenericType(type).GetProperty("Instance")!;
			object? previous = content.GetValue(null);
			restoreContent.Add(() => content.SetValue(null, previous));
			var prototype = (ModProjectile)Activator.CreateInstance(type)!;
			content.SetValue(null, prototype.NewInstance(new Projectile { type = i + 1 }));
		}
		return (ModPlayer)typeof(CooldownTestContext).GetMethod(nameof(CooldownTestContext.Attach))!
			.MakeGenericMethod(assembly.GetType("Everglow.Myth.Common.MythContentPlayer", true)!)
			.Invoke(context, null)!;
	}

	private static Assembly MythAssembly()
	{
		string configuration = new DirectoryInfo(AppContext.BaseDirectory).Parent!.Name;
		string path = Path.GetFullPath(Path.Combine(
			AppContext.BaseDirectory,
			"..", "..", "..", "..", "Modules", "Myth", "bin", configuration, "net8.0", "Everglow.Myth.dll"));
		return Assembly.LoadFrom(path);
	}

	private sealed class TestClubSmash : ClubProjSmash
	{
	}
}

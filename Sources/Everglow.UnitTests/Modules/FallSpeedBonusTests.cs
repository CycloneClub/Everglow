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

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
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
		context.Dispose();
	}

	[TestMethod]
	[DataRow(0, 1, 2)]
	[DataRow(0, 2, 1)]
	[DataRow(1, 0, 2)]
	[DataRow(1, 2, 0)]
	[DataRow(2, 0, 1)]
	[DataRow(2, 1, 0)]
	public void FoodCapsTakeMinimumInEveryOrder(int first, int second, int third)
	{
		ModBuff[] foods = [new CookedMarshmallowBuff(), new GrilledSquirrelBuff(), new RoastedDuckBuff()];
		Apply(foods[first]);
		Apply(foods[second]);
		Apply(foods[third]);
		bonuses.PostUpdateEquips();
		Assert.AreEqual(2.5f, context.Player.maxFallSpeed);
	}

	[TestMethod]
	[DataRow(0, 2.5f)]
	[DataRow(1, 7.5f)]
	[DataRow(2, 5f)]
	public void FoodCapsUseFixedNormalBaseline(int food, float expected)
	{
		ModBuff[] foods = [new CookedMarshmallowBuff(), new GrilledSquirrelBuff(), new RoastedDuckBuff()];
		context.Player.maxFallSpeed = 35.01f;
		Apply(foods[food]);
		bonuses.PostUpdateEquips();
		Assert.AreEqual(expected, context.Player.maxFallSpeed);
	}

	[TestMethod]
	[DataRow(1.25f, 1f)]
	[DataRow(1.25f, -1f)]
	[DataRow(5.01f, 1f)]
	[DataRow(35.01f, -1f)]
	public void CapPreservesLowerPhysicsLimitAndDoesNotChangeGravityOrVelocity(float baseline, float gravityDirection)
	{
		context.Player.maxFallSpeed = baseline;
		context.Player.gravity = 0.1f;
		context.Player.gravDir = gravityDirection;
		context.Player.velocity.Y = 12 * gravityDirection;
		Apply(new CookedMarshmallowBuff());
		bonuses.PostUpdateEquips();
		Assert.AreEqual(Math.Min(baseline, 2.5f), context.Player.maxFallSpeed);
		Assert.AreEqual(0.1f, context.Player.gravity);
		Assert.AreEqual(gravityDirection, context.Player.gravDir);
		Assert.AreEqual(12 * gravityDirection, context.Player.velocity.Y);
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void FrogLegBonusSettlesBeforeFoodCap(bool reverse)
	{
		ModBuff[] foods = [new SauteedFrogLegsBuff(), new CookedMarshmallowBuff()];
		foreach (var food in reverse ? foods.Reverse() : foods)
		{
			Apply(food);
		}
		bonuses.PostUpdateEquips();
		Assert.AreEqual(2.5f, context.Player.maxFallSpeed);
		BeginFrame();
		Apply(new SauteedFrogLegsBuff());
		Assert.AreEqual(15.01f, context.Player.maxFallSpeed, 0.00001f, "The native bonus applies immediately without settlement.");
		bonuses.PostUpdateEquips();
		Assert.AreEqual(15.01f, context.Player.maxFallSpeed, 0.00001f);
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void ActionOverrideOutranksCapsWithoutAccumulatingAndExpiresNextFrame(bool reverse)
	{
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
		Assert.AreEqual(2.5f, context.Player.maxFallSpeed);
		BeginFrame();
		bonuses.PostUpdateEquips();
		Assert.AreEqual(10.01f, context.Player.maxFallSpeed);
	}

	[TestMethod]
	[DataRow(false, 0, 24, 0, 2.5f)]
	[DataRow(true, 0, 10, 0, 10000f)]
	[DataRow(true, 0, 26, 0, 10000f)]
	[DataRow(true, 0, 30, 1, 10000f)]
	[DataRow(true, 0, 30, 2, 10000f)]
	[DataRow(true, 1, 26, 0, 2.5f)]
	public void ClubUsesExistingOwnerRegistrationThroughoutActionAndRestoresCapOnKill(bool spawned, int owner, int timer, int attack, float expected)
	{
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
			Assert.AreEqual(expected, context.Player.maxFallSpeed);
			club.OnKill(0);
			BeginFrame();
			Apply(new CookedMarshmallowBuff());
			clubPlayer.UpdateEquips();
			bonuses.PostUpdateEquips();
			Assert.AreEqual(2.5f, context.Player.maxFallSpeed);
		}
		finally
		{
			owners.Clear();
			owners.AddRange(previousOwners);
		}
	}

	[TestMethod]
	[DataRow(0, 0, 2.5f)]
	[DataRow(1, 0, 10000f)]
	[DataRow(0, 1, 10000f)]
	[DataRow(1, 1, 10000f)]
	public void SpearUsesOriginalOwnedCountsAndRestoresCapWhenBothDisappear(int normalCount, int downCount, float expected)
	{
		var myth = AttachMythPlayer();
		context.Player.ownedProjectileCounts[1] = normalCount;
		context.Player.ownedProjectileCounts[2] = downCount;
		Apply(new CookedMarshmallowBuff());
		myth.UpdateEquips();
		bonuses.PostUpdateEquips();
		Assert.AreEqual(expected, context.Player.maxFallSpeed);
		BeginFrame();
		context.Player.ownedProjectileCounts[1] = 0;
		context.Player.ownedProjectileCounts[2] = 0;
		Apply(new CookedMarshmallowBuff());
		myth.UpdateEquips();
		bonuses.PostUpdateEquips();
		Assert.AreEqual(2.5f, context.Player.maxFallSpeed);
	}

	[TestMethod]
	[DataRow(0)]
	[DataRow(1)]
	public void SilverBonusesApplyTogetherInUpdateEquipsWithoutRepeatingInMiscEffects(int silverBuff)
	{
		var myth = AttachMythPlayer();
		myth.GetType().GetField("SilverBuff")!.SetValue(myth, silverBuff);
		context.Player.maxRunSpeed = 3f;
		context.Player.wingAccRunSpeed = 4f;
		context.Player.GetDamage(DamageClass.Generic) = StatModifier.Default + 0.2f;
		myth.UpdateEquips();
		AssertBonuses();
		bonuses.PostUpdateEquips();
		myth.PostUpdateMiscEffects();
		AssertBonuses();
		BeginFrame();
		Apply(new CookedMarshmallowBuff());
		myth.UpdateEquips();
		bonuses.PostUpdateEquips();
		myth.PostUpdateMiscEffects();
		Assert.AreEqual(2.5f, context.Player.maxFallSpeed);

		void AssertBonuses()
		{
			Assert.AreEqual(silverBuff > 0 ? 11.51f : 10.01f, context.Player.maxFallSpeed, 0.00001f);
			Assert.AreEqual(silverBuff > 0 ? 3.05f : 3f, context.Player.maxRunSpeed, 0.00001f);
			Assert.AreEqual(silverBuff > 0 ? 4.05f : 4f, context.Player.wingAccRunSpeed, 0.00001f);
			Assert.AreEqual(silverBuff > 0 ? 1.25f : 1.2f, context.Player.GetDamage(DamageClass.Generic).ApplyTo(1f), 0.00001f);
		}
	}

	private void BeginFrame()
	{
		bonuses.ResetEffects();
		context.Player.maxFallSpeed = 10.01f;
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
			var type = assembly.GetType(spearName + (i == 0 ? "" : "Down"), true)!;
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
		string path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory,
			"..", "..", "..", "..", "Modules", "Myth", "bin", configuration, "net8.0", "Everglow.Myth.dll"));
		return Assembly.LoadFrom(path);
	}

	private sealed class TestClubSmash : ClubProjSmash
	{
	}
}

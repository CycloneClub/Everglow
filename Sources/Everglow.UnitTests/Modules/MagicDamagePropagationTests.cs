using System.Reflection;
using Everglow.SpellAndSkull.GlobalItems;
using Everglow.SpellAndSkull.Projectiles;
using Everglow.SpellAndSkull.Projectiles.BookofSkulls;
using Everglow.SpellAndSkull.Projectiles.CrystalStorm;
using Everglow.SpellAndSkull.Projectiles.DemonScythe;
using Everglow.SpellAndSkull.Projectiles.RazorbladeTyphoon;
using Everglow.SpellAndSkull.Projectiles.LunarFlare;
using Everglow.UnitTests.Function;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Everglow.UnitTests.Modules;

[TestClass]
[DoNotParallelize]
public class MagicDamagePropagationTests
{
	private static readonly MethodInfo ApplyStatsFromSource = typeof(Projectile).GetMethod("ApplyStatsFromSource", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!;
	private CooldownTestContext context = null!;
	private int childBaseCrit;
	private int childSpawnCritBonus;
	private Player[] originalPlayers = null!;
	private UnifiedRandom originalRandom = null!;
	private readonly List<Action> restoreContent = [];
	private readonly List<Projectile> shots = [];

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		context = new CooldownTestContext();
		originalPlayers = Main.player;
		originalRandom = Main.rand;
		Main.rand = new UnifiedRandom(1);
		Main.player = [context.Player];
		Main.dedServ = true; // Skip native audio without loading a client.
		context.Player.whoAmI = 0;
		context.Player.inventory[0] = new Item { type = ItemID.WaterBolt, stack = 1, damage = 100, DamageType = DamageClass.Magic };
		context.Player.selectedItem = 0;
		context.Player.GetDamage(DamageClass.Generic) = StatModifier.Default + 0.25f;
		context.Player.GetDamage(DamageClass.Magic) = StatModifier.Default + 0.5f;
		On_Projectile.NewProjectileDirect += CaptureDirect;
		On_Projectile.NewProjectile_IEntitySource_Vector2_Vector2_int_int_float_int_float_float_float += Capture;
		On_Collision.SolidCollision_Vector2_int_int += NoSolidCollision;
	}

	[TestCleanup]
	public void Cleanup()
	{
		On_Projectile.NewProjectileDirect -= CaptureDirect;
		On_Projectile.NewProjectile_IEntitySource_Vector2_Vector2_int_int_float_int_float_float_float -= Capture;
		On_Collision.SolidCollision_Vector2_int_int -= NoSolidCollision;
		for (int i = restoreContent.Count - 1; i >= 0; i--)
		{
			restoreContent[i]();
		}
		Main.player = originalPlayers;
		Main.rand = originalRandom;
		context.Dispose();
	}

	[TestMethod]
	[DataRow(1f, 175)]
	[DataRow(2f, 350)]
	public void OrdinaryBookCombinesGenericAndMagicDamageOnce(float multiplier, int expected)
	{
		var book = (TestBook)new TestBook().NewInstance(new Projectile { owner = 0 });
		book.ItemType = ItemID.WaterBolt;
		book.ProjType = ProjectileID.WaterBolt;
		book.MulDamage = multiplier;
		context.Player.itemTimeMax = 20;
		context.Player.itemTime = 18;
		book.AI();
		Assert.AreEqual(1, shots.Count);
		Assert.AreEqual(expected, shots[0].damage);
	}

	[TestMethod]
	public void LunarFlareKeepsItsMultiplierAfterNativeDamageBonuses()
	{
		SetContent(typeof(LunarFlareII), new LunarFlareII().NewInstance(new Projectile { type = ProjectileID.LunarFlare }));
		var book = (LunarFlareBook)new LunarFlareBook().NewInstance(new Projectile { owner = 0 });
		book.SetDef();
		context.Player.HeldItem.type = ItemID.LunarFlareBook;
		context.Player.itemTimeMax = 20;
		context.Player.itemTime = 18;
		book.SpecialAI();
		Assert.AreEqual(4, shots.Count);
		Assert.IsTrue(shots.All(shot => shot.damage == 437));
	}

	[TestMethod]
	[DataRow(false, 0, 0)]
	[DataRow(true, 0, 0)]
	[DataRow(false, 5, 0)]
	[DataRow(true, 5, 0)]
	[DataRow(false, 0, 3)]
	[DataRow(true, 0, 3)]
	public void BoneSpikesKeepParentStatsAndChildCritBonusesAfterWeaponSwitch(bool pvp, int baseCrit, int spawnCritBonus)
	{
		childBaseCrit = baseCrit;
		childSpawnCritBonus = spawnCritBonus;
		SetContent(typeof(BoneSpike), new BoneSpike().NewInstance(new Projectile { type = ProjectileID.WoodenArrowFriendly }));
		context.Attach<MagicBookPlayer>();
		var parent = new Projectile { owner = 0, damage = 175, knockBack = 3f, CritChance = 17, penetrate = 5 };
		var skull = (SkullII)new SkullII().NewInstance(parent);
		context.Player.HeldItem.damage = 1000;
		if (pvp)
		{
			var modifiers = new Player.HurtModifiers();
			skull.ModifyHitPlayer(new Player(), ref modifiers);
		}
		else
		{
			skull.OnHitNPC(new NPC(), new NPC.HitInfo(), 1);
		}
		Assert.AreEqual(3, shots.Count);
		Assert.IsTrue(shots.All(shot => shot.damage == 87 && shot.knockBack == 3f && shot.CritChance == 17 + baseCrit + spawnCritBonus));
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void BookShotsUseWeaponCritAndPreserveChildContributions(bool lunar)
	{
		ConfigureCrit();
		MagicBookProjectile book;
		if (lunar)
		{
			SetContent(typeof(LunarFlareII), new LunarFlareII().NewInstance(new Projectile { type = ProjectileID.LunarFlare }));
			book = (LunarFlareBook)new LunarFlareBook().NewInstance(new Projectile { owner = 0, CritChance = 99 });
			book.SetDef();
		}
		else
		{
			book = (TestBook)new TestBook().NewInstance(new Projectile { owner = 0, CritChance = 99 });
			book.ItemType = ItemID.WaterBolt;
			book.ProjType = ProjectileID.WaterBolt;
		}
		context.Player.HeldItem.type = book.ItemType;
		context.Player.itemTimeMax = 20;
		context.Player.itemTime = 18;
		if (lunar)
		{
			book.SpecialAI();
		}
		else
		{
			book.AI();
		}
		Assert.IsTrue(shots.Count > 0);
		Assert.IsTrue(shots.All(shot => shot.CritChance == 25));
		Assert.IsTrue(shots.All(shot => shot.OriginalCritChance == 12));
	}

	[TestMethod]
	public void LargeCrystalTriplesWeaponCritWithoutReplacingChildBonuses()
	{
		ConfigureCrit();
		SetContent(typeof(CrystalStormII), new CrystalStormII().NewInstance(new Projectile { type = ProjectileID.CrystalStorm }));
		SetContent(typeof(LargeCrystal), new LargeCrystal().NewInstance(new Projectile { type = ProjectileID.WoodenArrowFriendly }));
		var book = CreateBook("Everglow.SpellAndSkull.Projectiles.CrystalStorm.CrystalStormBook", new Projectile { owner = 0 });
		book.SetDef();
		book.GetType().GetField("times", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(book, 12);
		context.Player.HeldItem.type = ItemID.CrystalStorm;
		context.Player.itemTime = 2;
		book.SpecialAI();
		Assert.AreEqual(59, shots.Single(shot => shot.type == ProjectileID.WoodenArrowFriendly).CritChance);
		Assert.IsTrue(shots.Where(shot => shot.type == ProjectileID.CrystalStorm).All(shot => shot.CritChance == 25));
	}

	[TestMethod]
	public void TyphoonAftereffectKeepsParentCritAfterWeaponSwitch()
	{
		ConfigureCrit();
		SetContent(typeof(HurricaneMask), new HurricaneMask().NewInstance(new Projectile { type = ProjectileID.WoodenArrowFriendly }));
		var book = CreateBook(
			"Everglow.SpellAndSkull.Projectiles.RazorbladeTyphoon.RazorbladeTyphoonBook",
			new Projectile { owner = 0, damage = 175, CritChance = 17 });
		book.GetType().GetField("ConstantUsingTime", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(book, 720f);
		context.Player.HeldItem.crit = 99;
		book.OnKill(0);
		Assert.AreEqual(1, shots.Count);
		Assert.AreEqual(25, shots[0].CritChance);
	}

	[TestMethod]
	public void ScytheDeathFragmentScalesInheritedCritWithoutReplacingChildBonuses()
	{
		ConfigureCrit();
		SetContent(typeof(DemoHit), new DemoHit().NewInstance(new Projectile { type = ProjectileID.Bullet }));
		var crackType = typeof(MagicBookProjectile).Assembly.GetType("Everglow.SpellAndSkull.Projectiles.DemonScythe.DemonScythePlusCrack", true)!;
		SetContent(crackType, ((ModProjectile)Activator.CreateInstance(crackType)!).NewInstance(new Projectile { type = ProjectileID.WoodenArrowFriendly }));
		var scythe = (DemonScythePlus)new DemonScythePlus().NewInstance(new Projectile
		{
			owner = 0, damage = 175, CritChance = 17, velocity = new Vector2(40, 0),
		});
		scythe.OnKill(0);
		var cracks = shots.Where(shot => shot.type == ProjectileID.WoodenArrowFriendly).ToArray();
		Assert.IsTrue(cracks.Length > 0);
		Assert.IsTrue(cracks.All(shot => shot.CritChance == 10)); // floor(17 * (2 / 1.3) / 10) + 5 + 3
	}

	private void ConfigureCrit()
	{
		context.Player.HeldItem.crit = 7;
		context.Player.GetCritChance(DamageClass.Generic) = 4;
		context.Player.GetCritChance(DamageClass.Magic) = 6;
		childBaseCrit = 5;
		childSpawnCritBonus = 3;
	}

	private static MagicBookProjectile CreateBook(string name, Projectile projectile)
	{
		var type = typeof(MagicBookProjectile).Assembly.GetType(name, true)!;
		return (MagicBookProjectile)((ModProjectile)Activator.CreateInstance(type)!).NewInstance(projectile);
	}

	private void SetContent(Type type, object value)
	{
		var instance = typeof(ContentInstance<>).MakeGenericType(type).GetProperty("Instance")!;
		var previous = instance.GetValue(null);
		restoreContent.Add(() => instance.SetValue(null, previous));
		instance.SetValue(null, value);
	}

	private Projectile CaptureDirect(On_Projectile.orig_NewProjectileDirect orig, IEntitySource source, Vector2 position, Vector2 velocity, int type, int damage, float knockback, int owner, float ai0, float ai1, float ai2)
	{
		return CaptureWithNativeInheritance(source, type, damage, knockback, owner);
	}

	private int Capture(On_Projectile.orig_NewProjectile_IEntitySource_Vector2_Vector2_int_int_float_int_float_float_float orig, IEntitySource source, Vector2 position, Vector2 velocity, int type, int damage, float knockback, int owner, float ai0, float ai1, float ai2)
	{
		CaptureWithNativeInheritance(source, type, damage, knockback, owner);
		return 0;
	}

	private Projectile CaptureWithNativeInheritance(IEntitySource source, int type, int damage, float knockback, int owner)
	{
		var shot = new Projectile { type = type, damage = damage, knockBack = knockback, owner = owner, CritChance = childBaseCrit };
		// Run the actual native source inheritance while omitting world insertion and graphics.
		ApplyStatsFromSource.Invoke(shot, [source]);
		// OnSpawn runs after inheritance and may contribute additional crit.
		shot.CritChance += childSpawnCritBonus;
		shots.Add(shot);
		return shot;
	}

	private bool NoSolidCollision(On_Collision.orig_SolidCollision_Vector2_int_int orig, Vector2 position, int width, int height) => false;

	private sealed class TestBook : MagicBookProjectile
	{
	}
}

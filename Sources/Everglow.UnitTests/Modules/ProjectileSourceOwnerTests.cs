using System.Reflection;
using Everglow.Food.Items.Weapons;
using Everglow.Commons.VFX.CommonVFXDusts;
using Everglow.Commons.VFX.CommonVFXDusts.Misc;
using Everglow.UnitTests.Function;
using Everglow.Yggdrasil.YggdrasilTown.Items.Weapons;
using Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.LightSeeker;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Everglow.UnitTests.Modules;

[TestClass]
[DoNotParallelize]
public class ProjectileSourceOwnerTests
{
	private CooldownTestContext context = null!;
	private Player[] players = null!;
	private UnifiedRandom random = null!;
	private int localPlayer;
	private readonly List<Action> restore = [];
	private IEntitySource? capturedSource;
	private Projectile? capturedProjectile;

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		context = new CooldownTestContext();
		players = Main.player;
		random = Main.rand;
		localPlayer = Main.myPlayer;
		Main.myPlayer = 0;
		context.Player.whoAmI = 1;
		Main.player = [new Player { active = true, whoAmI = 0 }, context.Player];
		Main.rand = new UnifiedRandom(1);
		On_Projectile.NewProjectileDirect += CaptureDirect;
		On_Projectile.NewProjectile_IEntitySource_Vector2_Vector2_int_int_float_int_float_float_float += Capture;
	}

	[TestCleanup]
	public void Cleanup()
	{
		On_Projectile.NewProjectileDirect -= CaptureDirect;
		On_Projectile.NewProjectile_IEntitySource_Vector2_Vector2_int_int_float_int_float_float_float -= Capture;
		for (int i = restore.Count - 1; i >= 0; i--)
		{
			restore[i]();
		}
		Main.player = players;
		Main.rand = random;
		Main.myPlayer = localPlayer;
		context.Dispose();
	}

	[TestMethod]
	[DataRow(typeof(LightBeamStaff))]
	[DataRow(typeof(ThermoprobeStaff))]
	[DataRow(typeof(SmeltingStaff))]
	[DataRow(typeof(FryingPanItem))]
	public void ShootKeepsWeaponAmmoContextAndActualPlayer(Type itemType)
	{
		var item = (ModItem)((ModItem)Activator.CreateInstance(itemType)!).NewInstance(new Item { type = ItemID.StoneBlock });
		item.Item.damage = 23;
		item.Item.crit = 7;
		item.Item.ArmorPenetration = 9;
		item.Item.DamageType = DamageClass.Magic;
		context.Player.GetCritChance(DamageClass.Magic) = 11;
		var source = new EntitySource_ItemUse_WithAmmo(context.Player, item.Item, ItemID.WoodenArrow, "SourceOwnerTest");
		item.Shoot(context.Player, source, Vector2.Zero, Vector2.UnitX, ProjectileID.WoodenArrowFriendly, 40, 2);
		Assert.AreSame(source, capturedSource);
		Assert.AreEqual(1, capturedProjectile!.owner);
		Assert.AreEqual(context.Player.GetWeaponCrit(item.Item), capturedProjectile.CritChance);
		Assert.AreEqual(context.Player.GetWeaponArmorPenetration(item.Item), capturedProjectile.ArmorPenetration);
		Assert.AreEqual(23, capturedProjectile.originalDamage);
	}

	[TestMethod]
	public void AccessoryProjectileBelongsToWearerInsteadOfLocalPlayer()
	{
		var module = LoadModule("Minortopography");
		var spriteType = module.GetType("Everglow.Minortopography.GiantPinetree.Projectiles.PineSprite", true)!;
		RegisterProjectile(spriteType);
		var item = (ModItem)Activator.CreateInstance(module.GetType("Everglow.Minortopography.GiantPinetree.Items.SnowPineLockBox", true)!)!;
		item.UpdateAccessory(context.Player, false);
		Assert.AreEqual(1, capturedProjectile!.owner);
		Assert.AreSame(context.Player, ((EntitySource_Parent)capturedSource!).Entity);
	}

	[TestMethod]
	public void HitChildKeepsParentStatsAndVictimAfterWeaponSwitch()
	{
		var module = LoadModule("Myth");
		RegisterProjectile(module.GetType("Everglow.Myth.TheTusk.Projectiles.Weapon.TuskSlash", true)!);
		var parent = new Projectile { owner = 1, damage = 80, CritChance = 31, ArmorPenetration = 17 };
		var knife = (ModProjectile)((ModProjectile)Activator.CreateInstance(module.GetType("Everglow.Myth.TheTusk.Projectiles.Weapon.ToothKnife", true)!)!).NewInstance(parent);
		context.Player.HeldItem.crit = 99;
		var target = new NPC();
		knife.OnHitNPC(target, new NPC.HitInfo(), 10);
		Assert.IsInstanceOfType<EntitySource_OnHit>(capturedSource);
		Assert.AreSame(parent, ((EntitySource_Parent)capturedSource!).Entity);
		Assert.AreSame(target, ((EntitySource_OnHit)capturedSource!).Victim);
		Assert.AreEqual(1, capturedProjectile!.owner);
		Assert.AreEqual(31, capturedProjectile.CritChance);
		Assert.AreEqual(17, capturedProjectile.ArmorPenetration);
	}

	[TestMethod]
	public void CrystalVisualKeepsOriginatingAttackInsteadOfLocalPlayer()
	{
		RegisterProjectile(typeof(HolyCrystalProjectile));
		var parent = new Projectile { owner = 1, CritChance = 31, ArmorPenetration = 17 };
		var source = parent.GetSource_Death();
		var crystal = new HolyCrystal
		{
			Source = source,
			Owner = parent.owner,
			Position = new Vector2(100),
			Velocity = Vector2.UnitX,
			MaxTime = 80,
			ai = [0, 0, 20],
		};
		crystal.Update();
		Assert.AreSame(source, capturedSource);
		Assert.AreEqual(1, capturedProjectile!.owner);
		Assert.AreEqual(31, capturedProjectile.CritChance);
		Assert.AreEqual(17, capturedProjectile.ArmorPenetration);
	}

	[TestMethod]
	public void EnemyAttackDoesNotBelongToItsPlayerTarget()
	{
		RegisterProjectile(typeof(Everglow.Yggdrasil.YggdrasilTown.Projectiles.Enemies.BloodSpell));
		var npc = new NPC { target = 1 };
		npc.ai[0] = 29;
		var caster = (ModNPC)new Everglow.Yggdrasil.YggdrasilTown.NPCs.CrimsonSpell().NewInstance(npc);
		On_NPC.hook_TargetClosest keepTarget = (orig, self, faceTarget) => { };
		On_NPC.TargetClosest += keepTarget;
		try
		{
			caster.AI();
		}
		finally
		{
			On_NPC.TargetClosest -= keepTarget;
		}
		Assert.AreSame(npc, ((EntitySource_Parent)capturedSource!).Entity);
		Assert.AreEqual(Main.maxPlayers, capturedProjectile!.owner);
		Assert.AreEqual(1, npc.target);
	}

	private void RegisterProjectile(Type type)
	{
		var instance = ((ModProjectile)Activator.CreateInstance(type)!).NewInstance(new Projectile { type = ProjectileID.WoodenArrowFriendly });
		var property = typeof(ContentInstance<>).MakeGenericType(type).GetProperty("Instance")!;
		var previous = property.GetValue(null);
		restore.Add(() => property.SetValue(null, previous));
		property.SetValue(null, instance);
	}

	private static Assembly LoadModule(string name)
	{
		string configuration = new DirectoryInfo(AppContext.BaseDirectory).Parent!.Name;
		return Assembly.LoadFrom(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Modules", name, "bin", configuration, "net8.0", $"Everglow.{name}.dll")));
	}

	private Projectile CaptureDirect(On_Projectile.orig_NewProjectileDirect orig, IEntitySource source, Vector2 position, Vector2 velocity, int type, int damage, float knockback, int owner, float ai0, float ai1, float ai2)
		=> Record(source, type, damage, owner);

	private int Capture(On_Projectile.orig_NewProjectile_IEntitySource_Vector2_Vector2_int_int_float_int_float_float_float orig, IEntitySource source, Vector2 position, Vector2 velocity, int type, int damage, float knockback, int owner, float ai0, float ai1, float ai2)
	{
		Record(source, type, damage, owner);
		return 0;
	}

	private Projectile Record(IEntitySource source, int type, int damage, int owner)
	{
		capturedSource = source;
		capturedProjectile = new Projectile { type = type, damage = damage, owner = owner, DamageType = DamageClass.Magic };
		typeof(Projectile).GetMethod("ApplyStatsFromSource", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
			.Invoke(capturedProjectile, [source]);
		return capturedProjectile;
	}
}

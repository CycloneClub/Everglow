using System.Reflection;
using Everglow.Food.Buffs.VanillaFoodBuffs;
using Everglow.UnitTests.Function;
using Terraria;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Everglow.UnitTests.Modules;

[TestClass]
[DoNotParallelize]
public class FoodAndCrownRegressionTests
{
	[TestInitialize]
	public void Initialize() => Program.SavePath = string.Empty;

	[TestMethod]
	public void CookedFishAddsFourMagicCritPointsAndFortyMana()
	{
		using var context = new CooldownTestContext();
		var player = context.Player;
		player.GetCritChance(DamageClass.Magic) = 7;
		player.GetCritChance(DamageClass.Generic) = 5;
		player.statManaMax2 = 200;
		int index = 0;
		new CookedFishBuff().Update(player, ref index);
		Assert.AreEqual(11f, player.GetCritChance(DamageClass.Magic));
		Assert.AreEqual(5f, player.GetCritChance(DamageClass.Generic));
		Assert.AreEqual(240, player.statManaMax2);
	}

	[TestMethod]
	[DataRow(1, false)]
	[DataRow(2, false)]
	[DataRow(3, false)]
	[DataRow(1, true)]
	[DataRow(2, true)]
	[DataRow(3, true)]
	public void CrownExpiresAfterSevenSecondsAndCanTriggerAgainAfterThirtySeconds(int effect, bool projectile)
	{
		using var context = new CooldownTestContext();
		var assembly = LoadPlantAndFarm();
		var crown = (ModItem)Activator.CreateInstance(assembly.GetType("Everglow.PlantAndFarm.Items.Accessories.ThreeColorCrown", true)!)!;
		var playerType = assembly.GetType("Everglow.PlantAndFarm.Items.Materials.PAFPlayer", true)!;
		var effects = (ModPlayer)typeof(CooldownTestContext).GetMethod(nameof(CooldownTestContext.Attach))!
			.MakeGenericMethod(playerType).Invoke(context, null)!;
		var originalRandom = Main.rand;
		bool originalPoison = Main.debuff[BuffID.Poisoned];
		try
		{
			Main.rand = new CrownRandom(effect);
			Main.debuff[BuffID.Poisoned] = true;
			Frame(0);
			Hit();
			for (int frame = 0; frame < 420; frame++)
			{
				Frame(effect);
				Hit(); // Repeated hits must not refresh the active effect or its cooldown.
			}
			for (int frame = 0; frame < 1380; frame++)
			{
				Frame(0);
				if (frame < 1379)
				{
					Hit();
				}
			}
			Hit();
			Frame(effect);
		}
		finally
		{
			Main.rand = originalRandom;
			Main.debuff[BuffID.Poisoned] = originalPoison;
		}

		void Frame(int expectedEffect)
		{
			var player = context.Player;
			effects.ResetEffects();
			player.GetDamage(DamageClass.Generic) = StatModifier.Default + 0.1f;
			player.GetCritChance(DamageClass.Generic) = 5;
			player.statManaMax2 = 200;
			Array.Clear(player.buffImmune);
			crown.UpdateAccessory(player, false);
			Assert.AreEqual(expectedEffect == 1 ? 1.32f : 1.1f, player.GetDamage(DamageClass.Generic).ApplyTo(1), 0.00001f);
			Assert.AreEqual(expectedEffect == 1 ? 16f : 5f, player.GetCritChance(DamageClass.Generic));
			Assert.AreEqual(expectedEffect == 3 ? 500 : 200, player.statManaMax2);
			Assert.AreEqual(expectedEffect == 2, player.buffImmune[BuffID.Poisoned]);
		}

		void Hit()
		{
			if (projectile)
			{
				effects.OnHitNPCWithProj(new Projectile(), new NPC(), new NPC.HitInfo(), 1);
			}
			else
			{
				effects.OnHitNPCWithItem(new Item(), new NPC(), new NPC.HitInfo(), 1);
			}
		}
	}

	[TestMethod]
	public void PearlIncludesGenericDamageInItsMagicBonus()
	{
		using var context = new CooldownTestContext();
		var player = context.Player;
		player.GetDamage(DamageClass.Generic) = StatModifier.Default + 0.25f;
		player.GetDamage(DamageClass.Magic) = StatModifier.Default + 0.5f;
		var damages = new List<int>();
		string configuration = new DirectoryInfo(AppContext.BaseDirectory).Parent!.Name;
		string path = Path.GetFullPath(Path.Combine(
			AppContext.BaseDirectory,
			"..", "..", "..", "..", "Modules", "Minortopography", "bin", configuration, "net8.0", "Everglow.Minortopography.dll"));
		var assembly = Assembly.LoadFrom(path);
		var itemType = assembly.GetType("Everglow.Minortopography.GiantPinetree.Items.PearlOfCyan", true)!;
		var playerType = assembly.GetType("Everglow.Minortopography.GiantPinetree.Items.PearlOfCyanPlayer", true)!;
		var accessory = (ModItem)((ModItem)Activator.CreateInstance(itemType)!).NewInstance(new Item { type = ItemID.StoneBlock });
		var instance = typeof(ContentInstance<>).MakeGenericType(itemType).GetProperty("Instance")!;
		var originalItem = instance.GetValue(null);
		var originalRandom = Main.rand;
		var effects = (ModPlayer)typeof(CooldownTestContext).GetMethod(nameof(CooldownTestContext.Attach))!
			.MakeGenericMethod(playerType).Invoke(context, null)!;
		context.Player.armor[3] = accessory.Item;
		accessory.UpdateAccessory(context.Player, false);
		try
		{
			instance.SetValue(null, accessory);
			Main.rand = new UnifiedRandom(1);
			On_Projectile.NewProjectile_IEntitySource_Vector2_Vector2_int_int_float_int_float_float_float += Capture;
			// Keep spawn positions outside world bounds so no tile data is needed.
			effects.OnHitNPC(new NPC { position = new Vector2(-10000, -10000) }, new NPC.HitInfo { Crit = true }, 1);
			Assert.IsTrue(damages.Count is >= 3 and <= 5);
			Assert.IsTrue(damages.All(damage => damage == 7));
		}
		finally
		{
			On_Projectile.NewProjectile_IEntitySource_Vector2_Vector2_int_int_float_int_float_float_float -= Capture;
			instance.SetValue(null, originalItem);
			Main.rand = originalRandom;
		}

		int Capture(On_Projectile.orig_NewProjectile_IEntitySource_Vector2_Vector2_int_int_float_int_float_float_float orig, IEntitySource source, Vector2 position, Vector2 velocity, int type, int damage, float knockback, int owner, float ai0, float ai1, float ai2)
		{
			damages.Add(damage);
			return 0;
		}
	}

	private static Assembly LoadPlantAndFarm()
	{
		string configuration = new DirectoryInfo(AppContext.BaseDirectory).Parent!.Name;
		string path = Path.GetFullPath(Path.Combine(
			AppContext.BaseDirectory,
			"..", "..", "..", "..", "Modules", "PlantAndFarm", "bin", configuration, "net8.0", "Everglow.PlantAndFarm.dll"));
		return Assembly.LoadFrom(path);
	}

	private sealed class CrownRandom(int effect) : UnifiedRandom
	{
		public override int Next(int minValue, int maxValue) => effect;
	}
}

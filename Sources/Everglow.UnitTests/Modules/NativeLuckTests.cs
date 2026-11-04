using System.Reflection;
using Everglow.Food.Buffs.VanillaDrinkBuffs;
using Everglow.Food.Buffs.VanillaFoodBuffs;
using Everglow.UnitTests.Function;
using Terraria;
using Terraria.ModLoader;

namespace Everglow.UnitTests.Modules;

[TestClass]
[DoNotParallelize]
public class NativeLuckTests
{
	private CooldownTestContext context = null!;

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		context = new CooldownTestContext();
	}

	[TestCleanup]
	public void Cleanup() => context.Dispose();

	[TestMethod]
	[DataRow(-0.2f)]
	[DataRow(0f)]
	[DataRow(0.4f)]
	public void GrapesAddFixedLuckThroughNativeRecalculation(float equipmentLuck)
	{
		var player = context.Player;
		player.equipmentBasedLuckBonus = equipmentLuck;
		RecalculateLuck(player);
		float originalLuck = player.luck;
		int index = 0;
		new GrapesBuff().Update(player, ref index);
		Assert.AreEqual(originalLuck, player.luck);
		Assert.AreEqual(equipmentLuck + 0.1f, player.equipmentBasedLuckBonus, 0.000001f);
		RecalculateLuck(player);
		Assert.AreEqual(originalLuck + 0.1f, player.luck, 0.000001f);
		RecalculateLuck(player);
		Assert.AreEqual(originalLuck + 0.1f, player.luck, 0.000001f);
	}

	[TestMethod]
	[DataRow(0, 1, 2)]
	[DataRow(0, 2, 1)]
	[DataRow(1, 0, 2)]
	[DataRow(1, 2, 0)]
	[DataRow(2, 0, 1)]
	[DataRow(2, 1, 0)]
	public void OrdinaryLuckBonusesStackAdditivelyInEveryBuffOrder(int first, int second, int third)
	{
		var player = context.Player;
		RecalculateLuck(player);
		float originalLuck = player.luck;
		string configuration = new DirectoryInfo(AppContext.BaseDirectory).Parent!.Name;
		string path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory,
			"..", "..", "..", "..", "Modules", "Myth", "bin", configuration, "net8.0", "Everglow.Myth.dll"));
		var curseType = Assembly.LoadFrom(path).GetType("Everglow.Myth.LanternMoon.Buffs.UnfortuneCurse", throwOnError: true)!;
		ModBuff[] buffs = [new GrapesBuff(), new GrapeJuiceBuff(), (ModBuff)Activator.CreateInstance(curseType)!];
		int index = 0;
		buffs[first].Update(player, ref index);
		buffs[second].Update(player, ref index);
		buffs[third].Update(player, ref index);
		Assert.AreEqual(originalLuck, player.luck);
		Assert.AreEqual(9900.1f, player.equipmentBasedLuckBonus, 0.001f);
		RecalculateLuck(player);
		Assert.AreEqual(originalLuck + 9900.1f, player.luck, 0.001f);
	}

	private static void RecalculateLuck(Player player) => typeof(Player)
		.GetMethod("RecalculateLuck", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!
		.Invoke(player, null);
}

using System.Reflection;
using Everglow.Yggdrasil;
using Everglow.Yggdrasil.Common;
using SubworldLibrary;
using Terraria;
using Terraria.ModLoader;

namespace Everglow.UnitTests.Modules.Yggdrasil.Common;

[TestClass]
[DoNotParallelize]
public class YggdrasilPlayerTests
{
	private static readonly FieldInfo CurrentSubworldField = typeof(SubworldSystem).GetField("current", BindingFlags.NonPublic | BindingFlags.Static)!;

	private Subworld? originalSubworld;
	private Player player = null!;
	private YggdrasilPlayer modPlayer = null!;

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		originalSubworld = (Subworld?)CurrentSubworldField.GetValue(null);
		CurrentSubworldField.SetValue(null, null);

		player = new Player();
		modPlayer = (YggdrasilPlayer)new YggdrasilPlayer().NewInstance(player);
	}

	[TestCleanup]
	public void Cleanup()
	{
		CurrentSubworldField.SetValue(null, originalSubworld);
	}

	[TestMethod]
	public void ModifyMaxStats_InYggdrasil_ReplacesVanillaGrowthAndAddsPermanentBoosts()
	{
		player.ConsumedLifeCrystals = 15;
		player.ConsumedLifeFruit = 20;
		player.ConsumedManaCrystals = 9;
		SetPermanentLifeBoosts();
		CurrentSubworldField.SetValue(null, new YggdrasilWorld());

		modPlayer.ModifyMaxStats(out var health, out var mana);

		Assert.AreEqual(-237f, health.Base);
		Assert.AreEqual(-144f, mana.Base);
		Assert.AreEqual(263, (int)health.ApplyTo(500f));
		Assert.AreEqual(56, (int)mana.ApplyTo(200f));
	}

	[TestMethod]
	public void ModifyMaxStats_OutsideYggdrasil_KeepsVanillaGrowthAndAddsPermanentBoosts()
	{
		player.ConsumedLifeCrystals = 15;
		player.ConsumedLifeFruit = 20;
		player.ConsumedManaCrystals = 9;
		SetPermanentLifeBoosts();

		modPlayer.ModifyMaxStats(out var health, out var mana);

		Assert.AreEqual(83f, health.Base);
		Assert.AreEqual(0f, mana.Base);
		Assert.AreEqual(583, (int)health.ApplyTo(500f));
		Assert.AreEqual(200, (int)mana.ApplyTo(200f));
	}

	[TestMethod]
	public void ModifyMaxStats_ComposesAsBaseWithOtherModifiersRegardlessOfOrder()
	{
		player.ConsumedLifeCrystals = 1;
		CurrentSubworldField.SetValue(null, new YggdrasilWorld());

		modPlayer.ModifyMaxStats(out var health, out _);
		var otherMod = new StatModifier(additive: 1.5f, multiplicative: 2f, flat: 7f, @base: 10f);
		var yggdrasilFirst = health.CombineWith(otherMod);
		var otherModFirst = otherMod.CombineWith(health);

		Assert.AreEqual(yggdrasilFirst, otherModFirst);
		Assert.AreEqual(349, (int)yggdrasilFirst.ApplyTo(120f));
	}

	private void SetPermanentLifeBoosts()
	{
		modPlayer.ConsumedAntiHeavenSicknessPill = 5;
		modPlayer.ConsumedJadeGlazeFruit = 2;
		modPlayer.ConsumedSquamousCore = 1;
		modPlayer.ConsumedLampBorerHoney = 3;
	}
}

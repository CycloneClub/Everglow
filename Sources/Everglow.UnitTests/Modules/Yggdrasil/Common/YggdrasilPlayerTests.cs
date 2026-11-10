using System.Reflection;
using Everglow.Yggdrasil;
using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.Netcode;
using Terraria.ModLoader.IO;
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

		modPlayer.ModifyMaxStats(out var insideHealth, out var insideMana);
		CurrentSubworldField.SetValue(null, null);
		modPlayer.ModifyMaxStats(out var outsideHealth, out var outsideMana);

		Assert.IsTrue(insideHealth.Base < outsideHealth.Base);
		Assert.IsTrue(insideHealth.ApplyTo(500f) < outsideHealth.ApplyTo(500f));
		Assert.IsTrue(outsideHealth.ApplyTo(500f) > 500f);
		Assert.IsTrue(insideMana.ApplyTo(200f) < 200f);
		Assert.AreEqual(200, (int)outsideMana.ApplyTo(200f));
	}

	[TestMethod]
	public void ModifyMaxStats_OutsideYggdrasil_KeepsVanillaGrowthAndAddsPermanentBoosts()
	{
		player.ConsumedLifeCrystals = 15;
		player.ConsumedLifeFruit = 20;
		player.ConsumedManaCrystals = 9;
		SetPermanentLifeBoosts();

		modPlayer.ModifyMaxStats(out var health, out var mana);

		Assert.IsTrue(health.ApplyTo(500f) > 500f);
		Assert.AreEqual(0f, mana.Base);
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
	}

	[TestMethod]
	public void ManaCrystalGrowthInYggdrasilUsesIntegralSteps()
	{
		CurrentSubworldField.SetValue(null, new YggdrasilWorld());
		float previous = ManaAfterCrystals(0);
		Assert.AreEqual(20f, previous, 0.001f);
		for (int crystals = 1; crystals <= 9; crystals++)
		{
			float current = ManaAfterCrystals(crystals);
			Assert.AreEqual(MathF.Round(current), current, 0.001f);
			Assert.IsTrue(current >= previous);
			previous = current;
		}

		Assert.IsTrue(ManaAfterCrystals(9) < 20 + 9 * 20);
	}

	private float ManaAfterCrystals(int crystals)
	{
		player.ConsumedManaCrystals = crystals;
		modPlayer.ModifyMaxStats(out _, out var mana);
		return mana.ApplyTo(20 + crystals * 20);
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void ManaBoostersHaveIndependentCapsAndSurviveSaveAndClone(bool springFirst)
	{
		player.ConsumedManaCrystals = 9;
		CurrentSubworldField.SetValue(null, new YggdrasilWorld());
		int springs = 0;
		int extracts = 0;
		if (springFirst)
		{
			while (modPlayer.UseActivatedManaSpringPowder())
			{
				springs++;
			}

			Assert.IsTrue(springs > 0);
			Assert.IsFalse(modPlayer.UseActivatedManaSpringPowder());
		}

		while (modPlayer.UseMagicOrganExtract())
		{
			extracts++;
		}

		Assert.IsTrue(extracts > 0);
		Assert.IsFalse(modPlayer.UseMagicOrganExtract());
		modPlayer.ModifyMaxStats(out _, out var firstStage);
		int partialMana = (int)firstStage.ApplyTo(200);
		if (!springFirst)
		{
			while (modPlayer.UseActivatedManaSpringPowder())
			{
				springs++;
			}

			Assert.IsTrue(springs > 0);
		}

		Assert.IsFalse(modPlayer.UseActivatedManaSpringPowder());
		var tag = new TagCompound();
		modPlayer.SaveData(tag);
		var restored = (YggdrasilPlayer)new YggdrasilPlayer().NewInstance(player);
		restored.LoadData(tag);
		Assert.IsFalse(restored.UseMagicOrganExtract());
		Assert.IsFalse(restored.UseActivatedManaSpringPowder());
		restored.ModifyMaxStats(out _, out var complete);
		int completeMana = (int)complete.ApplyTo(200);
		if (springFirst)
		{
			Assert.AreEqual(partialMana, completeMana);
		}
		else
		{
			Assert.IsTrue(completeMana > partialMana);
		}

		var clone = (YggdrasilPlayer)new YggdrasilPlayer().NewInstance(player);
		restored.CopyClientState(clone);
		Assert.AreEqual(extracts, clone.ConsumedMagicOrganExtract);
		Assert.AreEqual(springs, clone.ConsumedActivatedManaSpringPowder);
		CurrentSubworldField.SetValue(null, null);
		restored.ModifyMaxStats(out _, out var outside);
		Assert.IsTrue((int)outside.ApplyTo(200) > completeMana);
	}

	[TestMethod]
	public void OldSaveDoesNotGrantManaBoostsAndInvalidCountsAreClamped()
	{
		var probe = (YggdrasilPlayer)new YggdrasilPlayer().NewInstance(new Player());
		int springCap = 0;
		while (probe.UseActivatedManaSpringPowder())
		{
			springCap++;
		}

		Assert.IsTrue(springCap > 0);
		modPlayer.ConsumedMagicOrganExtract = 1;
		modPlayer.ConsumedActivatedManaSpringPowder = 1;
		modPlayer.LoadData(new TagCompound());
		Assert.AreEqual(0, modPlayer.ConsumedMagicOrganExtract);
		Assert.AreEqual(0, modPlayer.ConsumedActivatedManaSpringPowder);
		modPlayer.LoadData(new TagCompound
		{
			[nameof(YggdrasilPlayer.ConsumedMagicOrganExtract)] = -1,
			[nameof(YggdrasilPlayer.ConsumedActivatedManaSpringPowder)] = 99,
		});
		Assert.AreEqual(0, modPlayer.ConsumedMagicOrganExtract);
		Assert.AreEqual(springCap, modPlayer.ConsumedActivatedManaSpringPowder);
	}

	[TestMethod]
	public void PermanentBoostPacketRoundTripsBothManaCountsAndExistingLifeCounts()
	{
		var packet = new PermanentBoostPacket
		{
			playerId = 7,
			consumedAntiHeavenSicknessPill = 1,
			consumedJadeGlazeFruit = 2,
			consumedLampBorerHoney = 3,
			consumedSquamousCore = 4,
			consumedMagicOrganExtract = 5,
			consumedActivatedManaSpringPowder = 2,
		};
		using var stream = new MemoryStream();
		using (var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true))
		{
			packet.Send(writer);
		}
		stream.Position = 0;
		using var reader = new BinaryReader(stream);
		var received = new PermanentBoostPacket();
		received.Receive(reader, 0);
		Assert.AreEqual(stream.Length, stream.Position);
		Assert.AreEqual(7, received.playerId);
		Assert.AreEqual(1, received.consumedAntiHeavenSicknessPill);
		Assert.AreEqual(2, received.consumedJadeGlazeFruit);
		Assert.AreEqual(3, received.consumedLampBorerHoney);
		Assert.AreEqual(4, received.consumedSquamousCore);
		Assert.AreEqual(5, received.consumedMagicOrganExtract);
		Assert.AreEqual(2, received.consumedActivatedManaSpringPowder);
	}

	[TestMethod]
	public void EitherManaCountChangeTriggersTheExistingSynchronizationPath()
	{
		var tracked = new TrackingYggdrasilPlayer();
		var copy = new YggdrasilPlayer();
		tracked.SendClientChanges(copy);
		Assert.AreEqual(0, tracked.SyncCalls);
		Assert.IsTrue(tracked.UseMagicOrganExtract());
		tracked.SendClientChanges(copy);
		Assert.AreEqual(1, tracked.SyncCalls);
		tracked.CopyClientState(copy);
		tracked.SendClientChanges(copy);
		Assert.AreEqual(1, tracked.SyncCalls);
		Assert.IsTrue(tracked.UseActivatedManaSpringPowder());
		tracked.SendClientChanges(copy);
		Assert.AreEqual(2, tracked.SyncCalls);
	}

	[TestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void PermanentBoostHandlerUsesOwnerAndRelaysOnlyOnServer(bool server)
	{
		const int PlayerIndex = 7;
		int oldMode = Main.netMode;
		var oldPlayer = Main.player[PlayerIndex];
		var instanceProperty = typeof(ContentInstance<YggdrasilPlayer>).GetProperty("Instance")!;
		var oldInstance = instanceProperty.GetValue(null);
		var tracked = (TrackingYggdrasilPlayer)new TrackingYggdrasilPlayer().NewInstance(player);
		try
		{
			Main.netMode = server ? Terraria.ID.NetmodeID.Server : Terraria.ID.NetmodeID.MultiplayerClient;
			player.whoAmI = PlayerIndex;
			Main.player[PlayerIndex] = player;
			instanceProperty.SetValue(null, tracked);
			typeof(Player).GetField("modPlayers", BindingFlags.Instance | BindingFlags.NonPublic)!
				.SetValue(player, new ModPlayer[] { tracked });
			new PermanentBoostPacket.PermanentBoostPacketHandler().Handle(
				new PermanentBoostPacket
				{
					playerId = server ? PlayerIndex + 1 : PlayerIndex,
					consumedMagicOrganExtract = 99,
					consumedActivatedManaSpringPowder = 99,
				}, server ? PlayerIndex : 255);
			Assert.AreEqual(6, tracked.ConsumedMagicOrganExtract);
			Assert.AreEqual(4, tracked.ConsumedActivatedManaSpringPowder);
			Assert.AreEqual(server ? 1 : 0, tracked.SyncCalls);
			if (server)
			{
				Assert.AreEqual(PlayerIndex, tracked.LastFromWho);
			}
		}
		finally
		{
			Main.netMode = oldMode;
			Main.player[PlayerIndex] = oldPlayer;
			instanceProperty.SetValue(null, oldInstance);
		}
	}

	[TestMethod]
	[DataRow(Terraria.ID.NetmodeID.Server, -1, 7)]
	[DataRow(Terraria.ID.NetmodeID.Server, 255, 7)]
	[DataRow(Terraria.ID.NetmodeID.MultiplayerClient, 255, -1)]
	[DataRow(Terraria.ID.NetmodeID.MultiplayerClient, 255, 255)]
	public void PermanentBoostHandlerRejectsInvalidPlayerIndices(int mode, int sender, int owner)
	{
		int oldMode = Main.netMode;
		try
		{
			Main.netMode = mode;
			new PermanentBoostPacket.PermanentBoostPacketHandler().Handle(new PermanentBoostPacket { playerId = owner }, sender);
		}
		finally
		{
			Main.netMode = oldMode;
		}
	}

	private sealed class TrackingYggdrasilPlayer : YggdrasilPlayer
	{
		public int SyncCalls { get; private set; }

		public int LastFromWho { get; private set; }

		public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
		{
			SyncCalls++;
			LastFromWho = fromWho;
		}
	}

	private void SetPermanentLifeBoosts()
	{
		modPlayer.ConsumedAntiHeavenSicknessPill = 5;
		modPlayer.ConsumedJadeGlazeFruit = 2;
		modPlayer.ConsumedSquamousCore = 1;
		modPlayer.ConsumedLampBorerHoney = 3;
	}
}

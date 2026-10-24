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

		modPlayer.ModifyMaxStats(out var health, out var mana);

		Assert.AreEqual(-237f, health.Base);
		Assert.AreEqual(-150f, mana.Base);
		Assert.AreEqual(263, (int)health.ApplyTo(500f));
		Assert.AreEqual(50, (int)mana.ApplyTo(200f));
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

	[TestMethod]
	[DataRow(0, 20)]
	[DataRow(1, 23)]
	[DataRow(2, 26)]
	[DataRow(3, 30)]
	[DataRow(4, 33)]
	[DataRow(5, 36)]
	[DataRow(6, 40)]
	[DataRow(7, 43)]
	[DataRow(8, 46)]
	[DataRow(9, 50)]
	public void ManaCrystalGrowthInYggdrasilUsesIntegralSteps(int crystals, int expectedMana)
	{
		player.ConsumedManaCrystals = crystals;
		CurrentSubworldField.SetValue(null, new YggdrasilWorld());
		modPlayer.ModifyMaxStats(out _, out var mana);
		Assert.AreEqual(expectedMana, (int)mana.ApplyTo(20 + crystals * 20));
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void ManaBoostersHaveIndependentCapsAndSurviveSaveAndClone(bool springFirst)
	{
		player.ConsumedManaCrystals = 9;
		CurrentSubworldField.SetValue(null, new YggdrasilWorld());
		if (springFirst)
		{
			for (int i = 0; i < 4; i++)
			{
				Assert.IsTrue(modPlayer.UseManaSpringElixir());
			}
			Assert.IsFalse(modPlayer.UseManaSpringElixir());
		}
		for (int i = 0; i < 6; i++)
		{
			Assert.IsTrue(modPlayer.UseInsectManaEssence());
		}
		Assert.IsFalse(modPlayer.UseInsectManaEssence());
		modPlayer.ModifyMaxStats(out _, out var firstStage);
		Assert.AreEqual(springFirst ? 100 : 80, (int)firstStage.ApplyTo(200));
		if (!springFirst)
		{
			for (int i = 0; i < 4; i++)
			{
				Assert.IsTrue(modPlayer.UseManaSpringElixir());
			}
		}
		Assert.IsFalse(modPlayer.UseManaSpringElixir());
		var tag = new TagCompound();
		modPlayer.SaveData(tag);
		var restored = (YggdrasilPlayer)new YggdrasilPlayer().NewInstance(player);
		restored.LoadData(tag);
		Assert.IsFalse(restored.UseInsectManaEssence());
		Assert.IsFalse(restored.UseManaSpringElixir());
		restored.ModifyMaxStats(out _, out var complete);
		Assert.AreEqual(100, (int)complete.ApplyTo(200));
		var clone = (YggdrasilPlayer)new YggdrasilPlayer().NewInstance(player);
		restored.CopyClientState(clone);
		Assert.AreEqual(6, clone.ConsumedInsectManaEssence);
		Assert.AreEqual(4, clone.ConsumedManaSpringElixir);
		CurrentSubworldField.SetValue(null, null);
		restored.ModifyMaxStats(out _, out var outside);
		Assert.AreEqual(250, (int)outside.ApplyTo(200));
	}

	[TestMethod]
	public void OldSaveDoesNotGrantManaBoostsAndInvalidCountsAreClamped()
	{
		modPlayer.ConsumedInsectManaEssence = 6;
		modPlayer.ConsumedManaSpringElixir = 4;
		modPlayer.LoadData(new TagCompound());
		Assert.AreEqual(0, modPlayer.ConsumedInsectManaEssence);
		Assert.AreEqual(0, modPlayer.ConsumedManaSpringElixir);
		modPlayer.LoadData(new TagCompound
		{
			[nameof(YggdrasilPlayer.ConsumedInsectManaEssence)] = -1,
			[nameof(YggdrasilPlayer.ConsumedManaSpringElixir)] = 99,
		});
		Assert.AreEqual(0, modPlayer.ConsumedInsectManaEssence);
		Assert.AreEqual(4, modPlayer.ConsumedManaSpringElixir);
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
			consumedInsectManaEssence = 5,
			consumedManaSpringElixir = 2,
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
		Assert.AreEqual(5, received.consumedInsectManaEssence);
		Assert.AreEqual(2, received.consumedManaSpringElixir);
	}

	[TestMethod]
	public void EitherManaCountChangeTriggersTheExistingSynchronizationPath()
	{
		var tracked = new TrackingYggdrasilPlayer();
		var copy = new YggdrasilPlayer();
		tracked.SendClientChanges(copy);
		Assert.AreEqual(0, tracked.SyncCalls);
		Assert.IsTrue(tracked.UseInsectManaEssence());
		tracked.SendClientChanges(copy);
		Assert.AreEqual(1, tracked.SyncCalls);
		tracked.CopyClientState(copy);
		tracked.SendClientChanges(copy);
		Assert.AreEqual(1, tracked.SyncCalls);
		Assert.IsTrue(tracked.UseManaSpringElixir());
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
					consumedInsectManaEssence = 99,
					consumedManaSpringElixir = 99,
				}, server ? PlayerIndex : 255);
			Assert.AreEqual(6, tracked.ConsumedInsectManaEssence);
			Assert.AreEqual(4, tracked.ConsumedManaSpringElixir);
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

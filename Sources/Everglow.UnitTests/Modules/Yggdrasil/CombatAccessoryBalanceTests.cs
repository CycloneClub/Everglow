using Everglow.Commons.Mechanics.PlayerBonuses;
using Everglow.UnitTests.Function;
using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.KelpCurtain.Buffs;
using Everglow.Yggdrasil.KelpCurtain.Items.Accessories;
using Everglow.Yggdrasil.YggdrasilTown.Items.Accessories.Furnace;
using Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
[DoNotParallelize]
public class CombatAccessoryBalanceTests
{
	[TestInitialize]
	public void Initialize() => Program.SavePath = string.Empty;

	[TestMethod]
	public void MoltenCoreAddsTheSameEnduranceToAnyExistingValue()
	{
		float? added = null;
		foreach (float existingReduction in new[] { 0f, 0.2f, 0.5f })
		{
			using var context = new CooldownTestContext();
			var moltenCore = context.Attach<StatBonusPlayer>();
			moltenCore.ResetEffects();
			var player = context.Player;
			player.endurance = existingReduction;
			player.buffType[0] = BuffID.OnFire;
			player.buffTime[0] = 60;
			new MoltenCore().UpdateAccessory(player, false);
			var modifiers = new Player.HurtModifiers();
			modifiers.FinalDamage *= Math.Max(1 - player.endurance, 0); // Stand in for native endurance handling.
			moltenCore.ModifyHurt(ref modifiers);
			float step = player.endurance - existingReduction;
			added ??= step;
			Assert.AreEqual(added.Value, step, 0.000001f);
			Assert.IsTrue(step > 0f);
			Assert.AreEqual(1f - player.endurance, modifiers.FinalDamage.ApplyTo(1), 0.00001f);
		}
	}

	[TestMethod]
	public void MoltenCoreDoesNotApplyItsBonusesWithoutAFlameDebuff()
	{
		using var context = new CooldownTestContext();
		var moltenCore = context.Attach<StatBonusPlayer>();
		moltenCore.ResetEffects();
		var player = context.Player;
		player.endurance = 0.2f;
		player.moveSpeed = 1f;
		new MoltenCore().UpdateAccessory(player, false);
		var modifiers = new Player.HurtModifiers();
		moltenCore.ModifyHurt(ref modifiers);
		Assert.AreEqual(1f, modifiers.FinalDamage.ApplyTo(1));
		Assert.AreEqual(0.2f, player.endurance);
		Assert.AreEqual(1f, player.moveSpeed);
	}

	[TestMethod]
	public void MoltenCoreAddsNativeEnduranceForEachFlameDebuff()
	{
		float? endurance = null;
		foreach (int fireBuff in new[] { BuffID.OnFire, BuffID.OnFire3, BuffID.CursedInferno })
		{
			using var context = new CooldownTestContext();
			context.Player.buffType[0] = fireBuff;
			context.Player.buffTime[0] = 60;
			context.Player.endurance = 0.2f;
			new MoltenCore().UpdateAccessory(context.Player, false);
			endurance ??= context.Player.endurance;
			Assert.AreEqual(endurance.Value, context.Player.endurance, 0.000001f);
			Assert.IsTrue(context.Player.endurance > 0.2f);
		}
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void HeartAndFowlbeastAddVulnerabilityButMultiplyReduction(bool fractured)
	{
		using var context = new CooldownTestContext();
		var instance = typeof(ContentInstance<FractureDebuff>).GetProperty("Instance")!;
		var previous = instance.GetValue(null);
		var fracture = new FractureDebuff();
		typeof(ModBuff).GetProperty("Type")!.SetValue(fracture, BuffID.BrokenArmor);
		instance.SetValue(null, fracture);
		try
		{
			var heart = context.Attach<HardenedWitherbarkHeart.HardenedWitherbarkHeartPlayer>();
			var fowlbeast = context.Attach<EmptyFowlbeast.EmptyFowlbeastPlayer>();
			heart.Enable = fowlbeast.Enable = true;
			context.Player.buffType[1] = BuffID.OnFire;
			context.Player.buffTime[1] = 60;
			new MoltenCore().UpdateAccessory(context.Player, false);
			if (fractured)
			{
				context.Player.buffType[0] = BuffID.BrokenArmor;
				context.Player.buffTime[0] = 60;
			}
			var npc = new NPC();
			int[] savedTypes = (int[])context.Player.buffType.Clone();
			int[] savedTimes = (int[])context.Player.buffTime.Clone();
			float Hit(bool reverse)
			{
				savedTypes.CopyTo(context.Player.buffType, 0);
				savedTimes.CopyTo(context.Player.buffTime, 0);
				var modifiers = new Player.HurtModifiers();
				if (reverse)
				{
					fowlbeast.ModifyHitByNPC(npc, ref modifiers);
					heart.ModifyHitByNPC(npc, ref modifiers);
				}
				else
				{
					heart.ModifyHitByNPC(npc, ref modifiers);
					fowlbeast.ModifyHitByNPC(npc, ref modifiers);
				}

				return modifiers.FinalDamage.ApplyTo(100);
			}

			float forward = Hit(false);
			float backward = Hit(true);
			Assert.AreEqual(forward, backward, 0.0001f);
			Assert.IsTrue(context.Player.HasBuff<FractureDebuff>());
			float endured = forward * Math.Max(1 - context.Player.endurance, 0);
			Assert.IsTrue(endured < forward);
			if (!fractured)
			{
				Assert.AreEqual(HardenedWitherbarkHeart.CooldownDuration, context.Player.buffTime[context.Player.FindBuffIndex(BuffID.BrokenArmor)]);
			}

			var nextHit = new Player.HurtModifiers();
			heart.ModifyHitByNPC(npc, ref nextHit);
			fowlbeast.ModifyHitByNPC(npc, ref nextHit);
			float followUp = nextHit.FinalDamage.ApplyTo(100);
			if (fractured)
			{
				Assert.AreEqual(forward, followUp, 0.0001f);
			}
			else
			{
				Assert.IsTrue(followUp > forward);
			}
		}
		finally
		{
			instance.SetValue(null, previous);
		}
	}

	[TestMethod]
	[DataRow(true, true)]
	[DataRow(false, true)]
	[DataRow(true, false)]
	public void RedAlgaeAddsVulnerabilityOnlyWithItsExistingBuffAndTileCondition(bool debuffed, bool touchingAlgae)
	{
		using var context = new CooldownTestContext();
		var buffInstance = typeof(ContentInstance<RedAlgaeDebuff>).GetProperty("Instance")!;
		var tileInstance = typeof(ContentInstance<JadeLakeBloodVineAlgea>).GetProperty("Instance")!;
		var previousBuff = buffInstance.GetValue(null);
		var previousTile = tileInstance.GetValue(null);
		bool previousSolid = Main.tileSolid[TileID.Spikes];
		var types = Main.tile.GetData<TileTypeData>();
		var states = Main.tile.GetData<TileWallWireStateData>();
		var savedTiles = new List<(int Index, TileTypeData Type, TileWallWireStateData State)>();
		try
		{
			var buff = new RedAlgaeDebuff();
			typeof(ModBuff).GetProperty("Type")!.SetValue(buff, BuffID.Poisoned);
			buffInstance.SetValue(null, buff);
			var algae = new JadeLakeBloodVineAlgea();
			typeof(ModTile).GetProperty("Type")!.SetValue(algae, (ushort)TileID.Spikes);
			tileInstance.SetValue(null, algae);
			// Borrow existing tile storage; do not allocate a Tilemap or replace global arrays.
			for (int x = 8; x < 16; x++)
			{
				for (int y = 8; y < 18; y++)
				{
					int index = x * Main.tile.Height + y;
					savedTiles.Add((index, types[index], states[index]));
					types[index] = default;
					states[index] = default;
				}
			}
			Main.tileSolid[TileID.Spikes] = true;
			var player = context.Player;
			player.position = new Vector2(160, 160);
			if (touchingAlgae)
			{
				var tile = Main.tile[10, 11];
				tile.HasTile = true;
				tile.TileType = TileID.Spikes;
			}
			if (debuffed)
			{
				player.buffType[0] = BuffID.Poisoned;
				player.buffTime[0] = 60;
			}
			Assert.AreEqual(touchingAlgae ? TileID.Spikes : -1, player.GetHurtTile().type);
			var yggdrasil = context.Attach<YggdrasilPlayer>();
			var fowlbeast = context.Attach<EmptyFowlbeast.EmptyFowlbeastPlayer>();
			fowlbeast.Enable = true;
			float Hit(bool reverse)
			{
				var modifiers = new Player.HurtModifiers();
				if (reverse)
				{
					yggdrasil.ModifyHurt(ref modifiers);
					fowlbeast.ModifyHitByNPC(new NPC(), ref modifiers);
				}
				else
				{
					fowlbeast.ModifyHitByNPC(new NPC(), ref modifiers);
					yggdrasil.ModifyHurt(ref modifiers);
				}

				Assert.AreEqual(1f, modifiers.FinalDamage.Multiplicative);
				return modifiers.FinalDamage.ApplyTo(100);
			}

			float forward = Hit(false);
			Assert.AreEqual(forward, Hit(true), 0.0001f);
			var fowlOnly = new Player.HurtModifiers();
			fowlbeast.ModifyHitByNPC(new NPC(), ref fowlOnly);
			if (debuffed && touchingAlgae)
			{
				Assert.IsTrue(forward > fowlOnly.FinalDamage.ApplyTo(100));
			}
			else
			{
				Assert.AreEqual(fowlOnly.FinalDamage.ApplyTo(100), forward, 0.0001f);
			}
		}
		finally
		{
			foreach (var tile in savedTiles)
			{
				types[tile.Index] = tile.Type;
				states[tile.Index] = tile.State;
			}
			Main.tileSolid[TileID.Spikes] = previousSolid;
			buffInstance.SetValue(null, previousBuff);
			tileInstance.SetValue(null, previousTile);
		}
	}
}

using Everglow.Commons.Mechanics.Cooldown;
using Everglow.Commons.Mechanics.ElementalDebuff;
using Everglow.Commons.Utilities;
using Terraria.GameInput;
using Terraria.ModLoader.IO;

namespace Everglow.Commons;

public partial class EverglowPlayer : ModPlayer
{
	public const string PlayerDataCooldownsKey = "everglowCooldowns";

	#region Fields

	/// <summary>
	/// The chance to consume mana on shoot. Multiply this value with the consume mana chance.
	/// </summary>
	public float ammoCost = 1f;

	/// <summary>
	/// Elemental penetration of each elemental debuff types.
	/// </summary>
	public readonly ElementalPenetration elementalPenetration = new();

	/// <summary>
	/// The <see cref="CooldownInstance"/>s of all <see cref="CooldownBase"/>s this player has active.
	/// <br/> Only the cooldowns that are currently active will be stored here.
	/// <br/> <see cref="PlayerUtils.AddCooldown"/>, <see cref="PlayerUtils.HasCooldown(Player, string)"/>, and <see cref="PlayerUtils.ClearCooldown"/> should be used to manipulate player buffs.
	/// </summary>
	public Dictionary<string, CooldownInstance> cooldowns = [];

	#endregion

	public override void OnEnterWorld()
	{
		// =================== Testing code ===============
		// Player.AddCooldown(TestCooldown.ID, 60 * 50000);
		// Player.AddCooldown(TestCooldown2.ID, 60 * 5000);
		// Player.AddCooldown(TestCooldown3.ID, 60 * 500);
		// Player.AddCooldown(TestCooldown4.ID, 60 * 50);
		foreach (var cd in cooldowns.Values)
		{
			SyncCooldownAddition(Main.dedServ, cd);
		}
	}

	public override void ResetEffects()
	{
		ammoCost = 1f;

		elementalPenetration.ResetEffects();
	}

	public override void PreUpdate()
	{
		// Syncing mouse controls. Usage: Set listen flags to true, then wait for the next update to sync.
		if (Main.myPlayer == Player.whoAmI)
		{
			mouseRight = PlayerInput.Triggers.Current.MouseRight;
			mouseWorld = Main.MouseWorld;

			if (listenMouseRight && mouseRight != oldMouseRight)
			{
				oldMouseRight = mouseRight;
				syncMouseControls = true;
				listenMouseRight = false;
			}

			if (listenMouseWorld && Vector2.Distance(mouseWorld, oldMouseWorld) > MousePositionSyncDiff)
			{
				oldMouseWorld = mouseWorld;
				syncMouseControls = true;
				listenMouseWorld = false;
			}

			if (listenMouseRotation && Math.Abs((mouseWorld - Player.MountedCenter).ToRotation() - (oldMouseWorld - Player.MountedCenter).ToRotation()) > MouseRotationSyncDiff)
			{
				oldMouseWorld = mouseWorld;
				syncMouseControls = true;
				listenMouseRotation = false;
			}
		}
	}

	public override void PostUpdateMiscEffects()
	{
		// Mouse controls
		if (syncMouseControls && NetUtils.IsClient)
		{
			SyncMousePosition(Main.dedServ);
			syncMouseControls = false;
		}

		// Cooldowns
		var expiredCooldowns = new List<string>();
		foreach (var (key, cdInstance) in cooldowns)
		{
			var cdBase = cdInstance.cooldown;
			if (cdBase.CanTickDown)
			{
				cdInstance.timeLeft--;
			}

			cdBase.Update();
			if (cdInstance.timeLeft <= 0)
			{
				cdBase.OnCompleted();
				expiredCooldowns.Add(key);
			}
		}

		foreach (var key in expiredCooldowns)
		{
			cooldowns.Remove(key);
		}

		if (cooldowns.Count > 0)
		{
			SyncCooldownRemoval(Main.dedServ, expiredCooldowns);
		}
	}

	public override void UpdateDead()
	{
		if (cooldowns.Count > 0)
		{
			var removedCooldowns = new List<string>();
			foreach (var (key, cdInstance) in cooldowns)
			{
				if (cdInstance.cooldown.PersistentCooldown)
				{
					continue;
				}

				removedCooldowns.Add(key);
			}

			if (removedCooldowns.Count > 0)
			{
				foreach (var key in removedCooldowns)
				{
					cooldowns.Remove(key);
				}

				SyncCooldownFullUpdate(Main.dedServ);
			}
		}
	}

	public override void LoadData(TagCompound tag)
	{
		// Cooldowns
		try
		{
			if (tag.TryGet<TagCompound>(PlayerDataCooldownsKey, out var cooldownTag))
			{
				foreach (var (id, data) in cooldownTag)
				{
					if (data is not TagCompound tagData)
					{
						continue;
					}

					var instance = CooldownInstance.Load(tagData, id, Player);
					if (instance is null)
					{
						continue;
					}

					cooldowns.Add(instance.cooldown.TypeID, instance);
				}
			}
		}
		catch (IOException)
		{
		}
	}

	public override void SaveData(TagCompound tag)
	{
		var list = new TagCompound();
		foreach (var (id, instance) in cooldowns.Where(cd => !cd.Value.cooldown.CooldownNoSave))
		{
			list.Add(id, instance.Save());
		}

		tag[PlayerDataCooldownsKey] = list;
	}
}

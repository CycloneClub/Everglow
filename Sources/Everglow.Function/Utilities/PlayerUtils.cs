using Everglow.Commons.Mechanics.BiomesText;
using Everglow.Commons.Mechanics.Cooldown;
using Everglow.Commons.Mechanics.ElementalDebuff;
using Terraria.GameContent.Events;

namespace Everglow.Commons.Utilities;

public static class PlayerUtils
{
	public static List<string> GetDevPlayerNames() =>
	[
		/*Mr. Skirt | DXTST*/
		"Mr. Skirt", "Mr Skirt", "DXTsT", "CXUtk",
		/*Dirty Octopus | 脏渔歌*/
		"Dirty Octopus", "脏渔歌",
		/*u_silver | 新萌の绿草*/
		"u_silver", "usilver", "新萌の绿草", "Uin",
		/*ju_zhang | 太阳照常升起*/
		"ju_zhang", "太阳照常升起", "ju zhang",
		/*ye_you | 夜谷紫幽*/
		"ye_you", "ye you", "dsfgasdfg", "Solaestas", "夜谷紫幽",
		/*Slime1024 | 凝胶 | TheGelatum*/
		"Slime1024", "Slime_1024", "凝胶", "TheGelatum",
		/*Omnielement | 万象元素*/
		"Omnielement", "Element Of All", "万象元素",
		/*yiyang223*/
		"yiyang223", "yiyang",
		/*Lacewing*/
		"Lacewing", "lyc-lacewing",
		/*JSDA Ling*/
		"JSDA Ling", "JDSA Ling",
		/*Jack Lyh*/
		"Jack Lyh",
		/*Cyrillya | crapsky223*/
		"Cyril", "Cyrillya", "crapsky223",
		/*FelixYang777*/
		"FelixYang777", "Felix Yang",
		/*Colin Weiss*/
		"Colin Weiss", "ColinWeiss",
		/*SilverMoon | 942293328*/
		"SilverMoon", "942293328", "Silverymoon",
		/*Setnour6*/
		"Setnour6",
		/*Nomis*/
		"Nomis", "NomisPrime",
		/*DomesticFoxcy*/
		"DomesticFoxcy", "DomesticFoxy",
		/*Plantare*/
		"Plantare", "世纪小花",
		/*Drawn_Lemon | Cloudea*/
		"Drawn_Lemon", "DrawnLemon", "Drawn",
		/* Others */
		"青枫", "陌林", "京墨", "鸭子ceo",
	];

	#region Generic

	public static EverglowPlayer Everglow(this Player player) =>
		player.GetModPlayer<EverglowPlayer>();

	public static bool TryGetActivePlayer(int whoAmI, out Player player)
	{
		player = null!;
		if (whoAmI < 0 || whoAmI >= Main.maxPlayers)
		{
			return false;
		}

		player = Main.player[whoAmI];
		return player is not null && player.active;
	}

	/// <summary>
	/// Get mouse position of player.
	/// <br/>Tips: Call <see cref="ListenMouseWorld"/> or <see cref="ListenMouseRotation"/> on every frame if u wanna use this.
	/// </summary>
	/// <param name="player"></param>
	/// <returns></returns>
	public static Vector2 MouseWorld(this Player player) =>
		player.Everglow().mouseWorld;

	/// <summary>
	/// Get mouse right of player
	/// <br/>Tips: Call <see cref="ListenMouseRight"/> on every frame if u wanna use this.
	/// </summary>
	/// <param name="player"></param>
	/// <returns></returns>
	public static bool MouseRight(this Player player) =>
		player.Everglow().mouseRight;

	public static void ListenMouseWorld(this Player player)
	{
		if (Main.myPlayer == player.whoAmI)
		{
			player.Everglow().listenMouseWorld = true;
		}
	}

	public static void ListenMouseRotation(this Player player)
	{
		if (Main.myPlayer == player.whoAmI)
		{
			player.Everglow().listenMouseRotation = true;
		}
	}

	public static void ListenMouseRight(this Player player)
	{
		if (Main.myPlayer == player.whoAmI)
		{
			player.Everglow().listenMouseRight = true;
		}
	}

	#endregion

	#region Player Animation

	/// <summary>
	/// Sets the player's arm position to align with the mouse position, with an optional offset.
	/// </summary>
	/// <param name="player">The player instance to modify.</param>
	/// <param name="offset">An optional float value to adjust the rotation by. Defaults to 0f.</param>
	/// <returns>The modified player instance.</returns>
	public static Player SetArmToFitMousePosition(this Player player, float offset = 0f)
	{
		var rotation = (Main.MouseWorld - player.MountedCenter).ToRotation() * player.gravDir + offset * player.direction - MathHelper.PiOver2;

		// Set the player's composite arm to be in front, fully stretched, and rotated to the calculated rotation.
		player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);

		return player;
	}

	#endregion

	#region Vanilla Stats

	/// <summary>
	/// Determines if the player is in specified vanilla biome.
	/// <br/>Get biome keys from <see cref="VanillaBiomes"/>.
	/// </summary>
	/// <param name="player"></param>
	/// <param name="biomeKey"></param>
	/// <returns></returns>
	public static bool InVanillaBiome(this Player player, string biomeKey)
	{
		return VanillaBiomes.VanillaBiomeIndex.TryGetValue(biomeKey, out var biome) ? biome.Condition(player) : false;
	}

	/// <summary>
	/// Heal life then show life text.
	/// </summary>
	/// <param name="player"></param>
	/// <param name="life"></param>
	public static void HealLife(this Player player, int life, bool showActualHeal = false)
	{
		int lifeCanHeal = player.statLifeMax2 - player.statLife;
		if (lifeCanHeal <= 0)
		{
			return;
		}

		int lifeToHeal = lifeCanHeal >= life ? life : lifeCanHeal;
		player.statLife += lifeToHeal;

		if (showActualHeal)
		{
			CombatText.NewText(player.getRect(), CombatText.HealLife, lifeToHeal, dramatic: true, dot: false);
		}
		else
		{
			CombatText.NewText(player.getRect(), CombatText.HealLife, life, dramatic: true, dot: false);
		}
	}

	/// <summary>
	/// Heal mana then show mana text.
	/// </summary>
	/// <param name="player"></param>
	/// <param name="mana"></param>
	public static void HealMana(this Player player, int mana, bool showActualHeal = false)
	{
		int manaCanHeal = player.statManaMax2 - player.statMana;
		if (manaCanHeal <= 0)
		{
			return;
		}

		int manaToHeal = manaCanHeal >= mana ? mana : manaCanHeal;
		player.statMana += manaToHeal;

		if (showActualHeal)
		{
			CombatText.NewText(player.getRect(), CombatText.HealMana, manaToHeal, dramatic: true, dot: false);
		}
		else
		{
			CombatText.NewText(player.getRect(), CombatText.HealMana, mana, dramatic: true, dot: false);
		}
	}

	/// <summary>
	/// Calculate <see cref="Player.slotsMinions"/> manually, should only be used in <see cref="ModItem.Shoot(Player, Terraria.DataStructures.EntitySource_ItemUse_WithAmmo, Vector2, Vector2, int, int, float)"/>
	/// </summary>
	/// <param name="player"></param>
	/// <returns></returns>
	public static float GetSlotsMinions(this Player player) => Main.projectile.Where(x => x.owner == player.whoAmI && x.active && x.minion).Sum(x => x.minionSlots);

	/// <summary>
	/// Check if the player is in pillar zone
	/// </summary>
	/// <param name="player"></param>
	/// <returns></returns>
	public static bool InCelestialPillarZone(this Player player) => player.ZoneTowerStardust || player.ZoneTowerSolar || player.ZoneTowerVortex || player.ZoneTowerNebula;

	/// <summary>
	/// Check if the player is in event
	/// </summary>
	/// <param name="player"></param>
	/// <param name="checkBloodMoon"></param>
	/// <returns></returns>
	public static bool AnyEvent(this Player player, bool checkBloodMoon = false)
	{
		if (Main.invasionType > InvasionID.None && Main.invasionProgressNearInvasion)
		{
			return true;
		}
		if (player.InCelestialPillarZone())
		{
			return true;
		}
		if (DD2Event.Ongoing && player.ZoneOldOneArmy)
		{
			return true;
		}
		if ((player.ZoneOverworldHeight || player.ZoneSkyHeight) && (Main.eclipse || Main.pumpkinMoon || Main.snowMoon))
		{
			return true;
		}
		if ((player.ZoneOverworldHeight || player.ZoneSkyHeight) && Main.bloodMoon && checkBloodMoon)
		{
			return true;
		}
		return false;
	}

	/// <summary>
	/// Get global consume ammo chance of player.
	/// </summary>
	/// <param name="player"></param>
	/// <returns></returns>
	public static float GetConsumeAmmoChance(this Player player)
	{
		var cost = player.GetModPlayer<EverglowPlayer>().ammoCost;
		if (player.ammoBox)
		{
			cost *= 0.8f;
		}
		if (player.ammoPotion)
		{
			cost *= 0.8f;
		}
		if (player.ammoCost80)
		{
			cost *= 0.8f;
		}
		if (player.ammoCost75)
		{
			cost *= 0.75f;
		}

		return cost;
	}

	#endregion

	#region Cooldown

	public static void AddCooldown(this Player player, string id, int timeToAdd, bool overwrite = true)
	{
		if (!player.HasCooldown(id) || overwrite)
		{
			var modP = player.GetModPlayer<EverglowPlayer>();
			var instance = new CooldownInstance(player, CooldownRegistry.GetNet(id), timeToAdd);
			modP.cooldowns[id] = instance;
			modP.SyncCooldownAddition(Main.dedServ, instance);
		}
	}

	public static bool HasCooldown(this Player player, Predicate<CooldownBase> predicate) =>
		player.GetModPlayer<EverglowPlayer>().cooldowns.Any(cd => predicate(cd.Value.cooldown));

	public static bool HasCooldown(this Player player, string id) =>
		player.HasCooldown(cd => cd.TypeID == id);

	public static bool HasCooldown<TCooldownBase>(this Player player) =>
		player.HasCooldown(cd => cd is TCooldownBase);

	public static void ClearCooldown(this Player player, string id)
	{
		var mp = player.GetModPlayer<EverglowPlayer>();
		if (mp.cooldowns.Remove(id))
		{
			mp.SyncCooldownRemoval(Main.dedServ, [id]);
		}
	}

	#endregion

	#region Elemental Debuff

	public static ElementalPenetration GetElementalPenetration(this Player player) =>
		player.GetModPlayer<EverglowPlayer>().elementalPenetration;

	public static ref StatModifier GetElementalPenetration(this Player player, string type) =>
		ref player.GetModPlayer<EverglowPlayer>().elementalPenetration[type];

	#endregion
}

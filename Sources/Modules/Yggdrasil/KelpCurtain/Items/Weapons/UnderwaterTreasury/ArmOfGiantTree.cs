using Everglow.Yggdrasil.KelpCurtain;
using Everglow.Yggdrasil.Netcode;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Weapons.UnderwaterTreasury;

/// <summary>
/// 巨树之臂 - designed melee smash weapon.
/// Holding the attack charges up to 2.5 s: a full charge deals 200% damage plus a
/// 100% shockwave, while a partial charge scales linearly from 75% up to that maximum.
/// Right-click performs an ordinary swing, and the 30-frame retract animation after a
/// smash locks further attacks.
/// </summary>
public class ArmOfGiantTree : ModItem
{
	/// <summary>2.5 seconds at 60 ticks per second.</summary>
	public const int MaxChargeFrames = 150;

	/// <summary>Radius of the full-charge shockwave, in pixels.</summary>
	private const float ShockwaveRadius = 200f;

	public override string LocalizationCategory => LocalizationUtils.Categories.MeleeWeapons;

	public override void SetDefaults()
	{
		Item.width = 64;
		Item.height = 62;
		Item.damage = 55;
		Item.DamageType = DamageClass.Melee;
		Item.knockBack = 7.25f;
		Item.useTime = 45;
		Item.useAnimation = 45;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.autoReuse = true;
		Item.reuseDelay = 30;
		Item.UseSound = SoundID.Item1;
		Item.rare = ItemRarityID.Orange;
		Item.value = Item.buyPrice(gold: 2);
	}

	public override void HoldItem(Player player)
	{
		var mp = player.GetModPlayer<KelpCurtainPlayer>();

		// Per-stack isolation: two same-type stacks share Type but occupy different
		// player.selectedItem slots, so any slot change resets the accumulator and
		// adopts the newly held stack's slot.
		if (player.selectedItem != mp.ArmOfGiantTreeChargedSlot)
		{
			mp.ArmOfGiantTreeCharge = 0;
			mp.ArmOfGiantTreeChargedSlot = player.selectedItem;
		}

		if (player.controlUseTile)
		{
			// Right-click is the ordinary swing; it does not charge.
			mp.ArmOfGiantTreeCharge = 0;
			Item.useTime = 24;
			Item.useAnimation = 24;
			Item.reuseDelay = 0;
			return;
		}

		Item.useTime = 45;
		Item.useAnimation = 45;
		Item.reuseDelay = 30;

		if (player.controlUseItem && player.itemAnimation <= 0 && player.itemTime <= 0)
		{
			mp.ArmOfGiantTreeCharge = System.Math.Min(mp.ArmOfGiantTreeCharge + 1, MaxChargeFrames);
		}
	}

	public override bool AltFunctionUse(Player player) => true;

	public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
	{
		if (player.altFunctionUse != 2)
		{
			// WR-02: the 0.75x..2x charge scaling applies only to the charged left-click;
			// the right-click ordinary swing keeps its unscaled base damage.
			float charge = player.GetModPlayer<KelpCurtainPlayer>().ArmOfGiantTreeCharge / (float)MaxChargeFrames;
			damage *= MathHelper.Lerp(0.75f, 2f, charge);
		}
	}

	public override bool? UseItem(Player player)
	{
		var mp = player.GetModPlayer<KelpCurtainPlayer>();
		bool fullChargeSmash = player.altFunctionUse != 2 && mp.ArmOfGiantTreeCharge >= MaxChargeFrames;

		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			if (fullChargeSmash)
			{
				// WR-01: the owning client only signals the release; the server applies
				// the authoritative shockwave through the packet handler.
				ModIns.PacketResolver.Send(
					new ArmOfGiantTreeChargePacket()
					{
						Charge = mp.ArmOfGiantTreeCharge,
						ReleaseSmash = true,
					}, toClient: -1, ignoreClient: Main.myPlayer);
			}
		}
		else if (Main.netMode == NetmodeID.SinglePlayer)
		{
			if (fullChargeSmash)
			{
				ApplyShockwave(player);
			}
		}

		// On a dedicated server UseItem does nothing: the ReleaseSmash handler owns the
		// authoritative application, so there is no double application.
		mp.ArmOfGiantTreeCharge = 0;
		return true;
	}

	/// <summary>
	/// Applies the full-charge shockwave around <paramref name="player"/>.
	/// Static-safe: it reads damage and knockback from the player's held item
	/// (100% of the held base damage, IN-02 unchanged) rather than the shared ModItem
	/// instance, so it can run from the packet handler on the authoritative side.
	/// </summary>
	public static void ApplyShockwave(Player player)
	{
		Item heldItem = player.HeldItem;
		int shockDamage = System.Math.Max(1, heldItem.damage);
		for (int i = 0; i < Main.maxNPCs; i++)
		{
			NPC npc = Main.npc[i];
			if (!npc.active || npc.friendly || npc.dontTakeDamage)
			{
				continue;
			}
			if (Vector2.Distance(npc.Center, player.Center) > ShockwaveRadius + npc.width * 0.5f)
			{
				continue;
			}
			int direction = npc.Center.X >= player.Center.X ? 1 : -1;
			npc.SimpleStrikeNPC(shockDamage, direction, false, heldItem.knockBack, DamageClass.Melee);
			npc.netUpdate = true;
		}
	}

	/// <summary>
	/// True when <paramref name="player"/> is holding this item; used by the packet
	/// handler to reject a spoofed release from a sender who does not hold it.
	/// </summary>
	public static bool IsHeldBy(Player player) => player.HeldItem.type == ModContent.ItemType<ArmOfGiantTree>();
}

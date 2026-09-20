using Everglow.Yggdrasil.KelpCurtain;
using Everglow.Yggdrasil.Netcode;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Weapons.UnderwaterTreasury;

public class ArmOfGiantTree : ModItem
{
	public const int MaxChargeFrames = 150;

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

		if (player.selectedItem != mp.ArmOfGiantTreeChargedSlot)
		{
			mp.ArmOfGiantTreeCharge = 0;
			mp.ArmOfGiantTreeChargedSlot = player.selectedItem;
		}

		if (player.controlUseTile)
		{
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

		mp.ArmOfGiantTreeCharge = 0;
		return true;
	}

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

	public static bool IsHeldBy(Player player) => player.HeldItem.type == ModContent.ItemType<ArmOfGiantTree>();
}

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

	/// <summary>Ticks accumulated while the left mouse button is held.</summary>
	public int ChargeTimer;

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
		if (player.HeldItem.type != Type)
		{
			ChargeTimer = 0;
			return;
		}

		if (player.controlUseTile)
		{
			// Right-click is the ordinary swing; it does not charge.
			ChargeTimer = 0;
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
			ChargeTimer = System.Math.Min(ChargeTimer + 1, MaxChargeFrames);
		}
	}

	public override bool AltFunctionUse(Player player) => true;

	public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
	{
		float charge = ChargeTimer / (float)MaxChargeFrames;
		damage *= MathHelper.Lerp(0.75f, 2f, charge);
	}

	public override bool? UseItem(Player player)
	{
		if (Main.myPlayer == player.whoAmI && player.altFunctionUse != 2 && ChargeTimer >= MaxChargeFrames)
		{
			// 100% shockwave: damage and knock back nearby enemies around the smash.
			int shockDamage = System.Math.Max(1, Item.damage);
			const float shockRadius = 200f;
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC npc = Main.npc[i];
				if (!npc.active || npc.friendly || npc.dontTakeDamage)
				{
					continue;
				}
				if (Vector2.Distance(npc.Center, player.Center) > shockRadius + npc.width * 0.5f)
				{
					continue;
				}
				int direction = npc.Center.X >= player.Center.X ? 1 : -1;
				npc.SimpleStrikeNPC(shockDamage, direction, false, Item.knockBack, DamageClass.Melee);
			}
		}
		ChargeTimer = 0;
		return true;
	}
}

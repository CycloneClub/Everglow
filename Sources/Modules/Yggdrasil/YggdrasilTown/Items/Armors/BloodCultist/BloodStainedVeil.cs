using Everglow.Yggdrasil.YggdrasilTown.Buffs;
using Terraria.GameContent.Creative;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Armors.BloodCultist;

[AutoloadEquip(EquipType.Head)]
public class BloodStainedVeil : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Armor;

	public override void SetStaticDefaults()
	{
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
	}

	public override void SetDefaults()
	{
		Item.width = 28;
		Item.height = 26;
		Item.value = 2500;
		Item.rare = ItemRarityID.Green;
		Item.defense = 2;
	}

	public override bool IsArmorSet(Item head, Item body, Item legs)
	{
		return body.type == ModContent.ItemType<BloodDrenchedRobe>();
	}

	public override void UpdateArmorSet(Player player)
	{
		player.GetModPlayer<BloodCultistPlayer>().hasBloodCultistSet = true;
	}

	public override void UpdateEquip(Player player)
	{
		player.GetDamage(DamageClass.Magic) += 0.08f;
	}

	public class BloodCultistPlayer : ModPlayer
	{
		public bool hasBloodCultistSet;

		public override void ResetEffects()
		{
			hasBloodCultistSet = false;
		}

		public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (hasBloodCultistSet && hit.DamageType == DamageClass.Magic)
			{
				Player.AddBuff(ModContent.BuffType<BloodCultistBuff>(), 4 * 60);
				if (Main.rand.NextFloat() < 0.03f)
				{
					Player.HealMana(6);
				}
			}
		}
	}
}

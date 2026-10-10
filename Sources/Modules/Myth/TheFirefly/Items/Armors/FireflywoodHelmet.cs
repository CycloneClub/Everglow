using Everglow.Myth.TheFirefly.Items.Materials;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.Localization;

namespace Everglow.Myth.TheFirefly.Items.Armors
{
	[AutoloadEquip(EquipType.Head)]
	public class FireflywoodHelmet : ModItem
	{
		public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Armor;

		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

			// If your head equipment should draw hair while drawn, use one of the following:
			// ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false; // Don't draw the head at all. Used by Space Creature Mask
			// ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true; // Draw hair as if a hat was covering the top. Used by Wizards Hat
			// ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true; // Draw all hair as normal. Used by Mime Mask, Sunglasses
			// ArmorIDs.Head.Sets.DrawBackHair[Item.headSlot] = true;
			// ArmorIDs.Head.Sets.DrawsBackHairWithoutHeadgear[Item.headSlot] = true;
		}

		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 26;
			Item.value = 5300;
			Item.rare = ItemRarityID.White;
			Item.defense = 5;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<FireflywoodBreastplate>() && legs.type == ModContent.ItemType<FireflywoodLeggings>();
		}

		public override void UpdateArmorSet(Player player)
		{
			player.GetDamage(DamageClass.Generic) += 0.2f;
		}

		public override void UpdateEquip(Player player)
		{
			player.magicCrit += 2;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient<GlowWood>(30);
			recipe.AddIngredient<BlackStarShrub>(10);
			recipe.AddIngredient<GlowingPetal>(5);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}

	internal sealed class FireflywoodArmorSetSystem : ModSystem
	{
		private ArmorSetBonus armorSetBonus;

		public override void PostSetupContent()
		{
			armorSetBonus = new ArmorSetBonus
			{
				Head = ModContent.ItemType<FireflywoodHelmet>(),
				Body = ModContent.ItemType<FireflywoodBreastplate>(),
				Legs = ModContent.ItemType<FireflywoodLeggings>(),
				Effect = ModContent.GetInstance<FireflywoodHelmet>().UpdateArmorSet,
				Description = Language.GetText("Mods.Everglow.Items.Armor.FireflywoodHelmet.SetBonus"),
				PrimaryPart = ArmorSetBonus.PartType.Head,
			};
			ArmorSetBonuses.All.Add(armorSetBonus);
			ArmorSetBonuses.BuildLookup();
		}

		public override void Unload()
		{
			if (armorSetBonus is not null)
			{
				ArmorSetBonuses.All.Remove(armorSetBonus);
				armorSetBonus = null;
			}
		}
	}
}

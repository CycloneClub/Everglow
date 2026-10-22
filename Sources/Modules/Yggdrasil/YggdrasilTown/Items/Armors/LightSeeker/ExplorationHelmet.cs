using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Everglow.Yggdrasil.Common;
using Terraria.GameContent.Creative;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Armors.LightSeeker;

[AutoloadEquip(EquipType.Head)]
public class ExplorationHelmet : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Armor;

	public override void SetStaticDefaults()
	{
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
	}

	public override void SetDefaults()
	{
		Item.width = 28;
		Item.height = 26;
		Item.value = 2500;
		Item.rare = ItemRarityID.Green;
		Item.defense = 1;
	}

	public override bool IsArmorSet(Item head, Item body, Item legs)
	{
		return body.type == ModContent.ItemType<ConcentratingJacket>() && legs.type == ModContent.ItemType<ShadowlessBoots>();
	}

	public override void UpdateArmorSet(Player player)
	{
		player.GetModPlayer<YggdrasilPlayer>().lightSeekerRangedSet = true;
		player.GetModPlayer<EverglowPlayer>().ammoCost *= 1 - 0.1f; // +10% chancce not consume ammo
		player.GetDamage(DamageClass.Ranged) += 0.05f;
	}

	public override void UpdateEquip(Player player)
	{
		Lighting.AddLight(player.Center, Vector3.one * 0.5f);

		player.GetCritChance(DamageClass.Ranged) += 4f;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient<LightBulbOvule>(1);
		recipe.AddIngredient(ItemID.Leather, 4);
		recipe.AddTile(TileID.WorkBenches);
		recipe.Register();
	}
}

public class LightSeekerGNPC : GlobalNPC
{
	public override Color? GetAlpha(NPC npc, Color drawColor)
	{
		if (NetUtils.NotServer && Main.LocalPlayer.GetModPlayer<YggdrasilPlayer>().lightSeekerRangedSet)
		{
			if (npc.CanBeChasedBy() && (npc.noTileCollide || Collision.CanHitLine(npc.Center, 1, 1, Main.LocalPlayer.Center, 1, 1)))
			{
				Vector2 vec1 = Main.MouseWorld - Main.LocalPlayer.Center;
				Vector2 vec2 = npc.Center - Main.LocalPlayer.Center;
				float alpha = (float)Math.Acos(Vector2.Dot(vec1, vec2) / (vec1.Length() * vec2.Length())); // 计算角度
				alpha = 1 - alpha;
				alpha = MathHelper.Clamp(alpha, 0, 1f);
				Color baseColor = drawColor;
				return Color.Lerp(baseColor, Color.White, alpha);
			}
		}
		return base.GetAlpha(npc, drawColor);
	}
}

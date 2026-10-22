using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Armors.Rock;

[AutoloadEquip(EquipType.Head)]
public class RockHelmet : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Armor;

	public const int MeleeCritChanceBonus = 5;

	public const int DistanceRequirementPerDefense = 333;

	public override void SetDefaults()
	{
		Item.width = 28;
		Item.height = 26;
		Item.value = Item.buyPrice(silver: 95);
		Item.rare = ItemRarityID.Green;
		Item.defense = 6;
	}

	public override void SetStaticDefaults()
	{
		ArmorIDs.Head.Sets.IsTallHat[Item.headSlot] = true;
	}

	public override void UpdateEquip(Player player)
	{
		player.GetCritChance(DamageClass.Melee) += MeleeCritChanceBonus;
		player.GetModPlayer<RockArmorSetPlayer>().DrawRockHelmet = true;
	}

	public override void UpdateVanitySet(Player player)
	{
		player.GetModPlayer<RockArmorSetPlayer>().DrawRockHelmet = true;
	}

	public override bool IsArmorSet(Item head, Item body, Item legs)
	{
		return body.type == ModContent.ItemType<RockPlateMail>() && legs.type == ModContent.ItemType<RockGreaves>();
	}

	public override void UpdateArmorSet(Player player)
	{
		player.GetModPlayer<RockArmorSetPlayer>().RockArmorSetEnable = true;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient<TwilightCrystal>(12);
		recipe.AddIngredient<GiantBeetleAntennae>(6);
		recipe.AddTile(TileID.Anvils);
		recipe.Register();
	}
}

public class RockArmorSetPlayer : ModPlayer
{
	public bool RockArmorSetEnable { get; set; } = false;

	public bool DrawRockHelmet { get; set; } = false;

	private int MoveDistance { get; set; } = 0;

	public int DefenseBonus => MoveDistance / RockHelmet.DistanceRequirementPerDefense <= 12
			? MoveDistance / RockHelmet.DistanceRequirementPerDefense
			: 12;

	public override void ResetEffects()
	{
		RockArmorSetEnable = false;
	}

	public override void PostUpdate()
	{
		if (RockArmorSetEnable)
		{
			MoveDistance += (int)(Player.position - Player.oldPosition).Length();
			Player.statDefense += DefenseBonus;
		}
	}

	public override void OnHurt(Player.HurtInfo info)
	{
		MoveDistance = 0;
	}

	/// <summary>
	/// A interim repair code for a over-width helmet.
	/// With a tremble bug when player acclerating.
	/// </summary>
	/// <param name="drawInfo"></param>
	public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
	{
		if (Player.head > 0 && TextureAssets.ArmorHead[Player.head].Value == ModAsset.RockHelmet_Head.Value)
		{
			drawInfo.helmetOffset = new Vector2(4, 0);
			Rectangle helmetFrame = Player.bodyFrame;
			helmetFrame.Width = 58;
			Vector2 drawPos = drawInfo.Position - Main.screenPosition;
			float offsetX = drawInfo.drawPlayer.bodyFrame.Width / 2f + drawInfo.drawPlayer.width / 2f;
			float offsetY = drawInfo.drawPlayer.height - drawInfo.drawPlayer.bodyFrame.Height + 4f;
			if (drawInfo.playerEffect == SpriteEffects.None)
			{
				offsetX -= helmetFrame.Width - drawInfo.drawPlayer.bodyFrame.Width;
				drawInfo.helmetOffset = new Vector2(-4, 0);
			}
			drawPos += new Vector2(-offsetX, offsetY);
			drawPos += drawInfo.helmetOffset + drawInfo.drawPlayer.headPosition + drawInfo.headVect;
			Texture2D tex = TextureAssets.ArmorHead[Player.head].Value;
			drawInfo.DrawDataCache.Add(new DrawData(tex, drawPos, helmetFrame, drawInfo.colorArmorHead, drawInfo.drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect));
		}
		base.ModifyDrawInfo(ref drawInfo);
	}
}

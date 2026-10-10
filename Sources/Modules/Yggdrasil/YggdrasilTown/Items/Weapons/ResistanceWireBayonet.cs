using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons;

public class ResistanceWireBayonet : StabbingSwordItem
{
	// TODO:翻译
	// 持续通过电流充能，并且逐渐红热，过程中伤害逐渐提高到180%。但是击中敌人损耗能量。
	// 对敌人造成点燃，对同一个目标初次造成260%伤害。
	// 对于开放性伤口，烧红的铁棍可以充当应急抗生素

	/// <summary>
	/// 0-100, increse automatically.
	/// </summary>
	public float Power = 0;

	public override void SetDefaults()
	{
		Item.damage = 22;
		Item.knockBack = 1.5f;
		Item.rare = ItemRarityID.Green;
		Item.value = Item.sellPrice(0, 2, 0, 0);
		Item.shoot = ModContent.ProjectileType<ResistanceWireBayonet_proj>();
		PowerfulStabDamageFlat = 4f;
		PowerfulStabProj = ModContent.ProjectileType<ResistanceWireBayonet_proj_stab>();
		base.SetDefaults();
	}

	public override bool AltFunctionUse(Player player)
	{
		if (CurrentPowerfulStabCD > 0)
		{
			return false;
		}

		if (!player.GetModPlayer<StabbingSwordStaminaPlayer>().CheckStamina(StaminaCost * 45))
		{
			return false;
		}

		foreach (Projectile proj in Main.projectile)
		{
			if (proj.owner == player.whoAmI && proj.timeLeft > 1 && proj.type == PowerfulStabProj)
			{
				return false;
			}
		}
		if (player.statLife <= 15)
		{
			return false;
		}
		return true;
	}

	public override void Update(WorldItem item, ref float gravity, ref float maxFallSpeed) => base.Update(item, ref gravity, ref maxFallSpeed);

	public override void UpdateInventory(Player player)
	{
		if (Power < 100)
		{
			Power += 0.2f;
		}
		else
		{
			Power = 100;
		}
		base.UpdateInventory(player);
	}

	public override bool PreDrawInWorld(WorldItem worldItem, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
	{
		return base.PreDrawInWorld(worldItem, spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);
	}

	public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
	{
		if (Power >= 100)
		{
			return true;
		}
		Texture2D coldItem = ModAsset.ResistanceWireBayonet_cold.Value;
		spriteBatch.Draw(coldItem, position, frame, drawColor, 0, origin, scale, SpriteEffects.None, 0);
		Texture2D glow = ModAsset.ResistanceWireBayonet_glow.Value;
		float value = Power / 100f;
		Color glowColor = new Color(value, value, value, 0);
		spriteBatch.Draw(glow, position, frame, glowColor, 0, origin, scale, SpriteEffects.None, 0);
		return false;
	}
}

using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Myth.OmniElementItems;

public class LilyHarp : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicWeapons;

	public override void SetStaticDefaults()
	{
	}

	public override void SetDefaults()
	{
		Item.damage = 137;
		Item.mana = 6;
		Item.width = 50;
		Item.height = 50;
		Item.useTime = 72;
		Item.useAnimation = 72;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.noMelee = true;
		Item.knockBack = 2.25f;
		Item.value = 2100;
		Item.rare = ItemRarityID.Lime;
		Item.autoReuse = true;
		Item.DamageType = DamageClass.Magic;
		Item.noUseGraphic = true;
		Item.shoot = ModContent.ProjectileType<Projectiles.LilyHarpProj>();
		Item.shootSpeed = 0.1f;
	}

	private int SoundStyle = 0;

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		switch (SoundStyle)
		{
			case 0:
				SoundEngine.PlaySound(new SoundStyle("Everglow/Myth/Sounds/LilyHarpCmaj7"), player.Center);
				break;

			case 1:
				SoundEngine.PlaySound(new SoundStyle("Everglow/Myth/Sounds/LilyHarpFmaj7"), player.Center);
				break;

			case 2:
				SoundEngine.PlaySound(new SoundStyle("Everglow/Myth/Sounds/LilyHarpDmaj7"), player.Center);
				break;

			case 3:
				SoundEngine.PlaySound(new SoundStyle("Everglow/Myth/Sounds/LilyHarpGmaj7"), player.Center);
				break;
		}

		SoundStyle++;
		if (SoundStyle >= 4)
		{
			SoundStyle = 0;
		}

		if (player.ownedProjectileCounts[Item.shoot] > 0)
		{
			return false;
		}

		return true;
	}

	public override bool CanUseItem(Player player)
	{
		if (base.CanUseItem(player))
		{
			if (Main.myPlayer == player.whoAmI)
			{
				if (player.altFunctionUse == 2)
				{
					return false;
				}
			}
		}
		return base.CanUseItem(player);
	}

	public override bool AltFunctionUse(Player player)
	{
		return true;
	}
}

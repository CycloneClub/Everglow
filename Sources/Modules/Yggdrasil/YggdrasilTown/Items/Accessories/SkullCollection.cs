using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Accessories;

public class SkullCollection : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public const int ItemFrames = 7;

	public const int DetectRange = 96;
	public const int BuffDuration = 180;
	public const int BuffCooldownMax = 120;
	public const int FinalManaRegenBonus = 5;

	public int BuffCooldown { get; set; }

	public override void SetStaticDefaults()
	{
		Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, ItemFrames));
	}

	public override void SetDefaults()
	{
		Item.accessory = true;
		Item.width = 30;
		Item.height = 35;

		Item.rare = ItemRarityID.Orange;
		Item.value = Item.buyPrice(platinum: 0, gold: 2);
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		if (--BuffCooldown < 0)
		{
			BuffCooldown = 0;
		}

		foreach (var npc in Main.ActiveNPCs)
		{
			if (npc.friendly)
			{
				continue;
			}

			// If there's any enemy within 6 blocks / 96 pixels around the player
			if (Vector2.Distance(npc.Center, player.Center) <= DetectRange)
			{
				// 1. Add a 3s Mana Regeneration buff to the caster, then this effect goes to a 2s cooldown
				// ========================================================================================
				if (BuffCooldown <= 0)
				{
					player.AddBuff(BuffID.ManaRegeneration, BuffDuration);
					BuffCooldown = BuffCooldownMax;
					for (int k = 0; k < 20; k++)
					{
						Dust d = Dust.NewDustDirect(player.position, player.width, player.height, ModContent.DustType<SkullCollectionDust>());
						d.scale *= 2;
						d.velocity.Y -= 3;
					}
				}

				// 2. +5 mana regeneration rate
				// ============================
				player.manaRegenBonus += 5;
				break;
			}
		}
	}

	public override bool PreDrawInWorld(WorldItem worldItem, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
	{
		Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
		var frame = texture.Frame(verticalFrames: ItemFrames, frameY: ((int)Main.time / 5) % ItemFrames);
		spriteBatch.Draw(texture, worldItem.Center - Main.screenPosition, frame, lightColor, 0, new Vector2(texture.Width, texture.Height / ItemFrames) / 2, 1f, SpriteEffects.None, 0);

		return false;
	}
}

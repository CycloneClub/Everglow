using Everglow.Myth.LanternMoon.Items.Weapons;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.GameContent.Creative;

namespace Everglow.Myth.LanternMoon.Items;

public class TreasureBag_LanternGhostKing : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.TreasureBags;

	public override void SetStaticDefaults()
	{
		ItemID.Sets.BossBag[Type] = true;
		ItemID.Sets.PreHardmodeLikeBossBag[Type] = true;

		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
	}

	public override void SetDefaults()
	{
		Item.maxStack = Item.CommonMaxStack;
		Item.consumable = true;
		Item.width = 24;
		Item.height = 24;
		Item.rare = ItemRarityID.Purple;
		Item.expert = true;
	}

	public override bool CanRightClick()
	{
		return true;
	}

	public override void RightClick(Player player)
	{
		switch (Main.rand.Next(6))
		{
			case 0:
				player.QuickSpawnItem(null, ModContent.ItemType<GildingRevolver>(), 1);
				break;
			case 1:
				player.QuickSpawnItem(null, ModContent.ItemType<GoldenLotusStaff>(), 1);
				break;
			case 2:
				player.QuickSpawnItem(null, ModContent.ItemType<KeroseneLanternFlameThrower>(), 1);
				break;
			case 3:
				player.QuickSpawnItem(null, ModContent.ItemType<LanternSword>(), 1);
				break;
			case 4:
				player.QuickSpawnItem(null, ModContent.ItemType<LanternYoyo>(), 1);
				break;
			case 5:
				player.QuickSpawnItem(null, ModContent.ItemType<MillionLightStaff>(), 1);
				break;
		}
		player.QuickSpawnItem(null, ItemID.GoldCoin, 25);
		base.RightClick(player);
	}

	// Below is code for the visuals
	public override Color? GetAlpha(Color lightColor)
	{
		// Makes sure the dropped bag is always visible
		return Color.Lerp(lightColor, Color.White, 0.4f);
	}

	public override void PostUpdate(WorldItem item)
	{
		// Spawn some light and dust when dropped in the world
		Lighting.AddLight(item.Center, Color.White.ToVector3() * 0.4f);

		if (item.timeSinceItemSpawned % 12 == 0)
		{
			Vector2 center = item.Center + new Vector2(0f, item.height * -0.1f);

			// This creates a randomly rotated vector of length 1, which gets it's components multiplied by the parameters
			Vector2 direction = Main.rand.NextVector2CircularEdge(item.width * 0.6f, item.height * 0.6f);
			float distance = 0.3f + Main.rand.NextFloat() * 0.5f;
			var velocity = new Vector2(0f, -Main.rand.NextFloat() * 0.3f - 1.5f);

			var dust = Dust.NewDustPerfect(center + direction * distance, DustID.SilverFlame, velocity);
			dust.scale = 0.5f;
			dust.fadeIn = 1.1f;
			dust.noGravity = true;
			dust.noLight = true;
			dust.alpha = 0;
		}
	}

	public override bool PreDrawInWorld(WorldItem item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
	{
		// Draw the periodic glow effect behind the item when dropped in the world (hence PreDrawInWorld)
		Texture2D texture = TextureAssets.Item[item.type].Value;

		Rectangle frame;

		if (Main.itemAnimations[item.type] != null)
		{
			// In case this item is animated, this picks the correct frame
			frame = Main.itemAnimations[item.type].GetFrame(texture, Main.itemFrameCounter[whoAmI]);
		}
		else
		{
			frame = texture.Frame();
		}

		Vector2 frameOrigin = frame.Size() / 2f;
		var offset = new Vector2(item.width / 2 - frameOrigin.X, item.height - frame.Height);
		Vector2 drawPos = item.position - Main.screenPosition + frameOrigin + offset;

		float time = Main.GlobalTimeWrappedHourly;
		float timer = item.timeSinceItemSpawned / 240f + time * 0.04f;

		time %= 4f;
		time /= 2f;

		if (time >= 1f)
		{
			time = 2f - time;
		}

		time = time * 0.5f + 0.5f;

		for (float i = 0f; i < 1f; i += 0.25f)
		{
			float radians = (i + timer) * MathHelper.TwoPi;

			spriteBatch.Draw(texture, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, frame, new Color(90, 70, 255, 50), rotation, frameOrigin, scale, SpriteEffects.None, 0);
		}

		for (float i = 0f; i < 1f; i += 0.34f)
		{
			float radians = (i + timer) * MathHelper.TwoPi;

			spriteBatch.Draw(texture, drawPos + new Vector2(0f, 4f).RotatedBy(radians) * time, frame, new Color(140, 120, 255, 77), rotation, frameOrigin, scale, SpriteEffects.None, 0);
		}

		return true;
	}
}

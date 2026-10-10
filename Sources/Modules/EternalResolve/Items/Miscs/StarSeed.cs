using Everglow.EternalResolve.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Everglow.EternalResolve.Items.Miscs;

public class StarSeed : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Miscs;

	public override void SetDefaults()
	{
		Item.width = 16;
		Item.height = 14;
	}

	public override bool OnPickup(WorldItem item, Player player)
	{
		Item.stack = 0;
		player.AddBuff(ModContent.BuffType<StarCrack>(), 114514 * 60);
		CombatText.NewText(player.Hitbox, Color.Yellow, "★", true);
		return true;
	}

	public override bool PreDrawInWorld(WorldItem item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
	{
		Texture2D texture = ModAsset.StarSeed.Value;
		for (int i = 0; i < 5; i++)
		{
			Vector2 v0 = new Vector2(0, 7).RotatedBy(i / 2.5 * Math.PI + Main.timeForVisualEffects * 0.06);
			spriteBatch.Draw(texture, item.Center - Main.screenPosition + v0, null, Color.HotPink * 0.3f, 0, texture.Size() / 2f, Item.scale, SpriteEffects.None, 0);
		}
		spriteBatch.Draw(texture, item.Center - Main.screenPosition, null, Color.Yellow, 0, texture.Size() / 2f, Item.scale, SpriteEffects.None, 0);
		return false;
	}
}

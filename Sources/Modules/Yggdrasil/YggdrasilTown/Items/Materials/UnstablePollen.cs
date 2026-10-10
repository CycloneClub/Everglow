using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Materials;

public class UnstablePollen : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Materials;

	public override void SetStaticDefaults()
	{
		Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 7));
	}

	public override void SetDefaults()
	{
		Item.width = 24;
		Item.height = 24;
		Item.value = 90;
		Item.maxStack = Item.CommonMaxStack;
	}

	public override void Update(WorldItem worldItem, ref float gravity, ref float maxFallSpeed)
	{
		float timeValue = (float)(Main.time * 0.13f + worldItem.whoAmI) % 7;
		Vector3 color = new Vector3(0.4f, 0.3f, 0) * (MathF.Sin(timeValue) * 0.25f + 1.1f);
		Lighting.AddLight(worldItem.Center, color);

		base.Update(worldItem, ref gravity, ref maxFallSpeed);
	}

	public override bool PreDrawInWorld(WorldItem worldItem, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
	{
		Texture2D texture2D = ModAsset.UnstablePollen.Value;
		Texture2D textureGlow = ModAsset.UnstablePollen_glow.Value;
		int frameCount = (int)(Main.time * 0.33f + MathF.Sin(whoAmI) * 5) % 7;
		var frame = new Rectangle(0, 30 * frameCount, 30, 30);
		spriteBatch.Draw(texture2D, worldItem.Center - Main.screenPosition, frame, lightColor, rotation, new Vector2(15f), scale, SpriteEffects.None, 0);
		spriteBatch.Draw(textureGlow, worldItem.Center - Main.screenPosition, frame, new Color(1f, 1f, 1f, 0), rotation, new Vector2(15f), scale, SpriteEffects.None, 0);
		return false;
	}
}

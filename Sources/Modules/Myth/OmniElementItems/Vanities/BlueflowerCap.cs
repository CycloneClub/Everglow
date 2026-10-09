namespace Everglow.Myth.OmniElementItems.Vanities;

[AutoloadEquip(EquipType.Face)]
public class BlueflowerCap : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override void SetStaticDefaults()
	{
	}

	public override void SetDefaults()
	{
		Item.width = 18;
		Item.height = 18;
		Item.value = Item.buyPrice(0, 1, 0, 0);
		Item.rare = ItemRarityID.Green;
		Item.vanity = true;
		Item.accessory = true;
	}

	// public override void Load()
	// {
	// //On.Terraria.Main.DrawPlayers_AfterProjectiles += DrawHat;
	// On_Main.DrawPlayers_BehindNPCs += DrawDress;
	// }

	// private static void DrawDress(On_Main.orig_DrawPlayers_BehindNPCs orig, Main self)
	// {
	// orig(self);
	// Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

	// Main.spriteBatch.End();
	// }

	public static float[] PlayerShake = new float[256];
	public static int Adding = 0;

	// private static void DrawHat(On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
	// {
	// orig(self);
	// Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
	// for (int d = 0; d < Main.player.Length; d++)
	// {
	// for (int e = 0; e < Main.player[d].armor.Length; e++)
	// {
	// if (Main.player[d] != null)
	// {
	// if ((Main.player[d].velocity - Main.player[d].oldVelocity).Length() > 7)
	// {
	// if (Adding % 60 == 0)
	// {
	// }
	// }
	// PlayerShake[d] *= 0.98f;
	// }
	// if (Main.player[d].armor[e].type == ModContent.ItemType<BlueflowerCap>())
	// {
	// Texture2D Cap = ModAsset.BlueflowerCap_Cap.Value;
	// Color ligc = Lighting.GetColor((int)(Main.player[d].Center.X / 16f), (int)(Main.player[d].Center.Y / 16f) - 2);
	// Color plC = Main.player[d].GetImmuneAlpha(ligc, -1.5f);
	// Main.spriteBatch.Draw(Cap, Main.player[d].MountedCenter - Main.screenPosition + new Vector2(0, Main.player[d].height - 16) + new Vector2(0, -Main.player[d].height).RotatedBy(Main.player[d].headRotation + Main.player[d].fullRotation) + new Vector2(0, 0), null, plC, Main.player[d].headRotation + Main.player[d].fullRotation + PlayerShake[d], Cap.Size() / 2f, 1f, Main.player[d].direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
	// }
	// }
	// }
	// Main.spriteBatch.End();
	// }
}

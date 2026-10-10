using Terraria;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Accessories.Furnace;

public class HotAirBalloon : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public const int JumpSpeedBoost = 2;

	public override void SetStaticDefaults()
	{
		Item.SetNameOverride("Hot Air-balloon");
	}

	public override void SetDefaults()
	{
		Item.width = 44;
		Item.height = 46;
		Item.accessory = true;
		Item.hasVanityEffects = true;
		Item.rare = ItemRarityID.Blue;
		Item.value = Item.buyPrice(gold: 1, silver: 50);
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		// 1. Enable Jump Boost
		// ====================
		player.jumpBoost = true;

		// 2. Increase Jump Speed
		// ====================================
		player.jumpSpeedBoost += JumpSpeedBoost;

		// 3. Enable Fire Dust VFX
		// =======================
		player.GetModPlayer<HotAirBalloonPlayer>().HotAirBalloonEnable = true;
		Lighting.AddLight(player.Center + new Vector2(20 * player.direction, -30), new Vector3(0.7f, 0.35f, 0));
	}
}

internal class HotAirBalloonPlayer : ModPlayer
{
	public bool HotAirBalloonEnable = false;

	public override void ResetEffects()
	{
		HotAirBalloonEnable = false;
	}

	// 3. Enable Fire Dust VFX
	// =======================
	public override void PostUpdateMiscEffects()
	{
		if (HotAirBalloonEnable)
		{
			// If the player is moving
			if (Player.velocity.Length() <= 1E-05f)
			{
				return;
			}

			// Draw dust
			if (Main.rand.NextBool(1))
			{
				Dust dust = Dust.NewDustDirect(Player.position - new Vector2(2), Player.width + 4, Player.height + 4, DustID.Torch, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default(Color), 1.6f);
				dust.noGravity = true;
				dust.velocity *= 1.8f;
				dust.velocity.Y -= 0.5f;
			}
		}
	}

	public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
	{
		if (HotAirBalloonEnable && drawInfo.drawPlayer.balloon <= 0)
		{
			DrawData item;
			DrawData item_glow;
			Texture2D ballonTexture = ModAsset.HotAirBalloon_Balloon.Value;

			int num = (FocusHelper.IsSelectedApplication/* tModPorter Suggestion: Also consider FocusHelper.AllowInputProcessing, FocusHelper.GameplayActive, FocusHelper.UpdateVisualEffects, or others */ && (!Main.ingameOptionsWindow || !Main.autoPause)) ? (DateTime.Now.Millisecond % 800 / 200) : 0;
			Vector2 vector = Main.OffsetsPlayerOffhand[drawInfo.drawPlayer.bodyFrame.Y / 56];
			if (drawInfo.drawPlayer.direction != 1)
			{
				vector.X = drawInfo.drawPlayer.width - vector.X;
			}

			if (drawInfo.drawPlayer.gravDir != 1f)
			{
				vector.Y -= drawInfo.drawPlayer.height;
			}

			Vector2 vector2 = new Vector2(0f, 8f) + new Vector2((drawInfo.drawPlayer.direction - 1) * 2, 6f);
			Vector2 vector3 = drawInfo.Position - Main.screenPosition + vector * new Vector2(1f, drawInfo.drawPlayer.gravDir) + new Vector2(0f, drawInfo.drawPlayer.height - drawInfo.drawPlayer.bodyFrame.Height) + vector2;
			vector3 = vector3.Floor();
			SpriteEffects sEffect = drawInfo.drawPlayer.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			item = new DrawData(ballonTexture, vector3, new Rectangle(0, ballonTexture.Height / 4 * num, ballonTexture.Width / 2, ballonTexture.Height / 4), drawInfo.colorArmorBody, drawInfo.drawPlayer.bodyRotation, new Vector2(26 + drawInfo.drawPlayer.direction * 4, 28f + drawInfo.drawPlayer.gravDir * 6f), 1f, sEffect);
			item.shader = drawInfo.cBalloon;
			item_glow = new DrawData(ballonTexture, vector3, new Rectangle(ballonTexture.Width / 2, ballonTexture.Height / 4 * num, ballonTexture.Width / 2, ballonTexture.Height / 4), new Color(1f, 1f, 1f, 0), drawInfo.drawPlayer.bodyRotation, new Vector2(26 + drawInfo.drawPlayer.direction * 4, 28f + drawInfo.drawPlayer.gravDir * 6f), 1f, sEffect);
			item_glow.shader = drawInfo.cBalloon;
			drawInfo.DrawDataCache.Add(item);
			drawInfo.DrawDataCache.Add(item_glow);
		}
	}
}

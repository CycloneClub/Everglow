using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Melee;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.Localization;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Tools;

public class MiningPowerPickaxe : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Tools;

	public const int Pick = 59;
	public const int PickTileMax = 8;
	public const int SearchTileMax = 50;
	public const int SearchTileRange = 24 * 16; // 24 Blocks Range
	public const int ChargeMax = 400;
	public const int ChargeCost = 5;

	public int Charge { get; set; } = 0;

	public float ChargeProgress => Charge / (float)ChargeMax;

	private string ChargeProgressText => $"{Charge}/{ChargeMax}";

	private float chargeBarAlpha = 0;

	private bool enableChargeBar = false;

	public override void SetDefaults()
	{
		Item.width = 56;
		Item.height = 50;

		Item.pick = Pick;
		Item.tileBoost = 1;

		Item.DamageType = DamageClass.Melee;
		Item.damage = 28;
		Item.knockBack = 2f;

		Item.useStyle = ItemUseStyleID.Swing;
		Item.useAnimation = 5;
		Item.useTime = 25;
		Item.autoReuse = true;
		Item.useTurn = true;

		Item.noUseGraphic = true;
		Item.channel = true;
		Item.noMelee = true;

		Item.shoot = ModContent.ProjectileType<MiningPowerPickaxe_Proj>();
		Item.shootSpeed = 10;

		Item.rare = ItemRarityID.Orange;
		Item.value = Item.buyPrice(gold: 3);
	}

	public override void ModifyTooltips(List<TooltipLine> tooltips)
	{
		tooltips.Add(new TooltipLine(Mod, "Charge", Language.GetTextValue($"Charge: {ChargeProgressText}"))
		{
			OverrideColor = Color.LimeGreen,
		});
	}

	public override void HoldItem(Player player)
	{
		enableChargeBar = false;
		if (player.whoAmI == Main.myPlayer)
		{
			player.ListenMouseWorld();

			// When mouse right is held.
			if (PlayerInput.Triggers.Current.MouseRight)
			{
				// Charge the pickaxe.
				if (++Charge > ChargeMax)
				{
					Charge = ChargeMax;
				}
				enableChargeBar = true;
			}

			if (player.ownedProjectileCounts[Item.shoot] > 0)
			{
				foreach (var proj in Main.projectile)
				{
					if (proj.active && proj.owner == player.whoAmI && proj.type == Item.shoot)
					{
						MiningPowerPickaxe_Proj mPPP = proj.ModProjectile as MiningPowerPickaxe_Proj;
						if (mPPP.TargetTiles.Count > 0 && player.controlUseItem)
						{
							enableChargeBar = true;
						}
					}
				}
			}
		}
	}

	public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] == 0;

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		var tileType = Main.tile[Player.tileTargetX, Player.tileTargetY].type;
		Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, ai2: tileType);
		return false;
	}

	public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
	{
		// Draw charge text under player.
		if (enableChargeBar)
		{
			if (chargeBarAlpha < 1)
			{
				chargeBarAlpha += 0.05f;
			}
		}
		else
		{
			if (chargeBarAlpha > 0)
			{
				chargeBarAlpha -= 0.05f;
			}
		}
		if (chargeBarAlpha > 0)
		{
			DrawChargeBar(spriteBatch);
		}
	}

	private void DrawChargeBar(SpriteBatch spriteBatch)
	{
		// var stringSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, ChargeProgressText, Vector2.One);
		// var drawPos = Main.LocalPlayer.Bottom - Main.screenPosition;
		// var textColor = new Color(
		// MathHelper.Lerp(1f, Color.LimeGreen.R / 255f * (0.7f + ChargeProgress * 0.3f), ChargeProgress),
		// Color.LimeGreen.G / 255f * (0.9f + ChargeProgress * 0.2f),
		// Color.LimeGreen.B / 255f)
		// * (0.9f + 0.1f * MathF.Sin((float)Main.timeForVisualEffects * 0.04f));
		var sBS = GraphicsUtils.GetState(spriteBatch).Value;
		spriteBatch.End();
		spriteBatch.Begin(sBS);

		spriteBatch.transformMatrix = Main.GameViewMatrix.ZoomMatrix;

		// spriteBatch.DrawString(FontAssets.MouseText.Value, ChargeProgressText, drawPos, textColor, 0, new Vector2(stringSize.X * 0.5f, -stringSize.Y * 0.5f), 1f, SpriteEffects.None, 0);
		DrawBar();

		spriteBatch.End();
		spriteBatch.Begin(sBS);
	}

	private void DrawBar()
	{
		float powerProgress = (float)Charge / ChargeMax;
		var progressTexture = Commons.ModAsset.White.Value;
		var drawPos = Main.LocalPlayer.Bottom + new Vector2(0, Main.LocalPlayer.gfxOffY) - Main.screenPosition;
		var progressPosition = drawPos/* + Owner.gravDir * new Vector2(0, 36)*/;

		var envLight = Lighting.GetColor((drawPos + Main.screenPosition).ToTileCoordinates());
		envLight.A = 180;
		var frameColor = new Color(0.05f, 0.05f, 0.08f, 0.9f);
		var frameColor2 = new Color(0.15f, 0.25f, 0.38f, 0.4f);
		frameColor = Color.Lerp(frameColor, envLight, 0.3f);
		frameColor2 = Color.Lerp(frameColor2, envLight, 0.3f);
		Vector2 frameScale = new Vector2(2f, 0.4f) * 0.05f;
		Vector2 frameScale2 = new Vector2(1.8f, 0.2f) * 0.05f;

		var lineColor = new Color(
			MathHelper.Lerp(1f, Color.LimeGreen.R / 255f * (0.7f + ChargeProgress * 0.3f), ChargeProgress),
			Color.LimeGreen.G / 255f * (0.9f + ChargeProgress * 0.2f),
			Color.LimeGreen.B / 255f)
			* (0.9f + 0.1f * MathF.Sin((float)Main.timeForVisualEffects * 0.04f));
		lineColor = Color.Lerp(lineColor, envLight, 0.3f);
		var lineColorInner = lineColor * 0.4f;
		lineColorInner.A = 210;
		lineColor *= chargeBarAlpha;
		lineColorInner *= chargeBarAlpha;
		frameColor *= chargeBarAlpha;
		frameColor2 *= chargeBarAlpha;

		Vector2 lineScaleOuter = new Vector2(2.2f * powerProgress + 0.2f, 0.7f) * 0.05f;
		Vector2 lineScale = new Vector2(2.2f * powerProgress, 0.5f) * 0.05f;
		Vector2 lineScale2 = new Vector2(2.2f * powerProgress - 0.2f, 0.3f) * 0.05f;
		var linePositionOffset = new Vector2(-2.2f * (1 - powerProgress) * progressTexture.Width * 0.025f, 0);

		Main.spriteBatch.Draw(progressTexture, progressPosition, null, frameColor, 0, progressTexture.Size() / 2, frameScale, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(progressTexture, progressPosition, null, frameColor2, 0, progressTexture.Size() / 2f, frameScale2, SpriteEffects.None, 0);

		Main.spriteBatch.Draw(progressTexture, progressPosition + linePositionOffset, null, frameColor, 0, progressTexture.Size() / 2, lineScaleOuter, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(progressTexture, progressPosition + linePositionOffset, null, lineColor, 0, progressTexture.Size() / 2, lineScale, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(progressTexture, progressPosition + linePositionOffset, null, lineColorInner, 0, progressTexture.Size() / 2, lineScale2, SpriteEffects.None, 0);
	}
}

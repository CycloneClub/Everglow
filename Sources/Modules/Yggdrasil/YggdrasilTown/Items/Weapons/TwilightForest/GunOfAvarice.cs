using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Ranged;
using ReLogic.Graphics;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.TwilightForest;

public class GunOfAvarice : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.RangedWeapons;

	public override void SetDefaults()
	{
		Item.width = 62;
		Item.height = 32;
		Item.scale = 0.75f;

		Item.DamageType = DamageClass.Ranged;
		Item.damage = 16;
		Item.knockBack = 1.5f;
		Item.crit = 2;
		Item.noMelee = true;

		Item.useTime = 27;
		Item.useAnimation = 27;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.autoReuse = false;

		Item.rare = ItemRarityID.Orange;
		Item.value = Item.buyPrice(gold: 10);

		Item.shoot = ProjectileID.Bullet;
		Item.shootSpeed = 12f;
		Item.useAmmo = AmmoID.Bullet;
	}

	private const int MagazineCapacity = 14;

	public const int AutoReloadDuration = 90;
	public const float AutoReloadSuccessChance = 0.8f;
	public const float AutoReloadFailureDamageRatio = 2.8f;

	public const int ManualReloadDuration = 30;

	public const float DamageBonusPerLevel = 0.25f;
	public const int MaxLevel = 10;

	public const int TargetValueBonusInCopper = 75;

	private int AmmoAmount { get; set; } = MagazineCapacity;

	private int Level { get; set; } = 1;

	private int OldLevel { get; set; } = 1;

	public int LevelChangeTimer = 0;

	public override bool CanUseItem(Player player) =>
		player.ownedProjectileCounts[ModContent.ProjectileType<GunOfAvariceAutoReload>()] <= 0 &&
		player.ownedProjectileCounts[ModContent.ProjectileType<GunOfAvariceManualReload>()] <= 0;

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		if (player.altFunctionUse != 2)
		{
			Item.noUseGraphic = false;
			SoundEngine.PlaySound(SoundID.Item41);
			AmmoAmount--;

			// A offset of new Vector2(0, -5) is calculate by texture and HoldoutOffset().
			Projectile.NewProjectile(source, position + new Vector2(0, -5), velocity, type, (int)(damage * (1 + DamageBonusPerLevel * Level)), knockback);

			if (AmmoAmount <= 0)
			{
				// disable graphic when shifting magazine.
				Item.noUseGraphic = true;
				int result = 0;
				if (Main.rand.NextFloat() >= AutoReloadSuccessChance)
				{
					result = 1;
					OldLevel = Level;
					Level = 1;
					LevelChangeTimer = 120;
				}
				else
				{
					LevelChangeTimer = 120;
					OldLevel = Level;
					Level = Level + 1 > MaxLevel ? MaxLevel : Level + 1;
				}

				// save damage value for hurting player while fail.
				Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<GunOfAvariceAutoReload>(), (int)(Item.damage * (1 + DamageBonusPerLevel * OldLevel) * 2.8f), 0, player.whoAmI, ai0: result, ai1: Level);
				AmmoAmount = MagazineCapacity;
			}
		}
		else if (AmmoAmount < MagazineCapacity)
		{
			// disable graphic when shifting magazine.
			Item.noUseGraphic = true;
			Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<GunOfAvariceManualReload>(), 0, 0, player.whoAmI, ai1: Level);
			AmmoAmount = MagazineCapacity;
			OldLevel = Level;
			Level = 1;
			LevelChangeTimer = 120;
		}
		return false;
	}

	public override bool CanConsumeAmmo(Item ammo, Player player) => player.altFunctionUse != 2;

	public override bool AltFunctionUse(Player player) => true;

	public override void ModifyTooltips(List<TooltipLine> tooltips)
	{
		tooltips.Add(new TooltipLine(Mod, "GunOfAvariceAmmo", $"Current Ammo: {AmmoAmount}/{MagazineCapacity}"));
		tooltips.Add(new TooltipLine(Mod, "GunOfAvariceLevel", $"Current Ammo: {Level}"));
	}

	public override void UpdateInventory(Player player)
	{
		if (LevelChangeTimer > 0)
		{
			LevelChangeTimer--;
		}
		else
		{
			LevelChangeTimer = 0;
		}
		base.UpdateInventory(player);
	}

	public override Vector2? HoldoutOffset()
	{
		return new Vector2(-16, -4);
	}

	/// <summary>
	/// Draw UI
	/// </summary>
	/// <param name="spriteBatch"></param>
	/// <param name="position"></param>
	/// <param name="frame"></param>
	/// <param name="drawColor"></param>
	/// <param name="itemColor"></param>
	/// <param name="origin"></param>
	/// <param name="scale"></param>
	public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
	{
		Player player = Main.LocalPlayer;
		if (Item != Main.LocalPlayer.HeldItem)
		{
			return;
		}

		SpriteBatchState sBS = spriteBatch.GetState().Value;
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		// Draw rest bullets in magazine.
		Vector2 drawCenter = player.Center - Main.screenPosition + new Vector2(0, -36);
		Texture2D bulletSlot = Commons.ModAsset.TileBlock.Value;
		float unitWidth = 5;
		if (LevelChangeTimer <= 0)
		{
			Color ammoColor = Color.White;
			if (AmmoAmount == 1)
			{
				ammoColor = Color.Lerp(new Color(1f, 0.3f, 0.3f, 1f), new Color(0.7f, 1f, 1f, 1f), (MathF.Sin((float)Main.timeForVisualEffects * 0.09f) + 1) * 0.5f);
			}
			Vector2 generalCenter = drawCenter + new Vector2(unitWidth * (-AmmoAmount * 0.5f), 0);
			for (int i = 0; i < AmmoAmount; i++)
			{
				spriteBatch.Draw(bulletSlot, generalCenter + new Vector2(unitWidth * i, 0) + new Vector2(10, 0), null, ammoColor, 0, bulletSlot.Size() * 0.5f, new Vector2(0.25f, 0.85f), SpriteEffects.None, 0);
			}
			spriteBatch.Draw(bulletSlot, generalCenter + new Vector2(0, 0), null, ammoColor * 0.5f, 0, bulletSlot.Size() * 0.5f, 1, SpriteEffects.None, 0);
			Vector2 textSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, Level.ToString(), Vector2.One);
			spriteBatch.DrawString(FontAssets.MouseText.Value, Level.ToString(), generalCenter + new Vector2(0, 4), ammoColor, 0, textSize * 0.5f, 0.8f, SpriteEffects.None, 0);
		}

		// Draw text when level change.
		Effect dissolve = Commons.ModAsset.Dissolve.Value;
		float dissolveDuration = Math.Clamp(LevelChangeTimer / 30f - 0.2f, -0.2f, 1);
		dissolve.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_spiderNet.Value);
		dissolve.Parameters["duration"].SetValue(dissolveDuration);
		dissolve.Parameters["uDissolveColor"].SetValue(new Vector4(1f, 1f, 1f, 1f));
		dissolve.Parameters["uNoiseSize"].SetValue(4f);
		dissolve.Parameters["uNoiseXY"].SetValue(new Vector2(0.44f, 0.36f));
		dissolve.CurrentTechnique.Passes["OnlyPixel"].Apply();

		// shift the matrix to world position.
		Color textColor = Color.White;
		string text = "Level: " + Level;
		if (LevelChangeTimer > 80)
		{
			text = "Level: " + OldLevel;
			if (OldLevel == Level && Level == 10)
			{
				text = "Max Level";
			}
		}
		float drawScale = 1f;
		if (LevelChangeTimer is > 60 and < 90)
		{
			if (OldLevel < Level || Level >= 10)
			{
				drawScale = MathF.Pow(Math.Clamp(Math.Abs((LevelChangeTimer - 80f) / 10f * 2), 0, 1), 0.3f);
			}
		}
		if (LevelChangeTimer is > 70 and < 90)
		{
			if (OldLevel >= Level && Level < 10)
			{
				textColor = Color.Lerp(Color.White, Color.Transparent, MathF.Pow(Math.Clamp(Math.Abs((LevelChangeTimer - 80f) / 10f * 2), 0, 1), 0.3f));
			}
		}
		if (LevelChangeTimer is > 80)
		{
			if (OldLevel >= Level && Level < 10)
			{
				Texture2D line = Commons.ModAsset.Trail_7.Value;
				var frameOfLine = new Rectangle(0, 60, 256, 136);
				float timeValue = MathF.Pow(Math.Clamp((LevelChangeTimer - 80) / 40f - 0.3f, 0, 1), 3);
				spriteBatch.Draw(line, drawCenter + new Vector2(-timeValue * 40, 10), frameOfLine, textColor, 0, frameOfLine.Size() * 0.5f, new Vector2(0.25f * (1 - timeValue), 0.01f), SpriteEffects.None, 0);
			}
		}
		Vector2 stringSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, text, Vector2.One);
		spriteBatch.DrawString(FontAssets.MouseText.Value, text, drawCenter, textColor, 0, new Vector2(stringSize.X * 0.5f, 0), drawScale, SpriteEffects.None, 0);

		spriteBatch.End();
		spriteBatch.Begin(sBS);
	}
}

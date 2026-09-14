using Everglow.Commons.Graphics;
using ReLogic.Graphics;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.UI.Chat;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Weapons;

public class RazorbeakBow : ModItem
{
	public const int BaseDelay = 30;
	public const int ProjectileCountPerUse = 3;

	public override string LocalizationCategory => LocalizationUtils.Categories.RangedWeapons;

	public override void SetDefaults()
	{
		Item.width = 38;
		Item.height = 80;

		Item.ranged = true;
		Item.damage = 11;
		Item.crit = 4;
		Item.knockBack = 3f;
		Item.scale = 0.9f;

		Item.useStyle = ItemUseStyleID.Shoot;
		Item.autoReuse = true;
		Item.noMelee = true;

		Item.useTime = 12;
		Item.useAnimation = 30;
		Item.useLimitPerAnimation = ProjectileCountPerUse;
		Item.reuseDelay = BaseDelay;

		Item.useAmmo = AmmoID.Arrow;
		Item.consumeAmmoOnFirstShotOnly = true;
		Item.shoot = ProjectileID.WoodenArrowFriendly;
		Item.shootSpeed = 12f;

		Item.rare = ItemRarityID.Green;
		Item.value = Item.buyPrice(gold: 5);
	}

	public override float UseTimeMultiplier(Player player)
	{
		int stack = player.GetModPlayer<RazorbeakBowPlayer>().RazorbeakBowEffectStack;
		return stack > 0 ? 1f / (1 + stack * RazorbeakBowPlayer.RazorbeakBowAttackSpeedBonusPerStack) : 1f;
	}

	public override float UseAnimationMultiplier(Player player) => UseTimeMultiplier(player);

	public override void HoldItem(Player player) => Item.reuseDelay = (int)(BaseDelay * UseTimeMultiplier(player));

	public override Vector2? HoldoutOffset() => new Vector2(-8, -4);

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		var modPlayer = player.GetModPlayer<RazorbeakBowPlayer>();

		// Initialize group info before the first shot in group.
		if (player.ItemAnimationJustStarted)
		{
			modPlayer.GroupIndex++;
			modPlayer.RazorbeakHitInfo.TryAdd(modPlayer.GroupIndex, (0, false));
		}

		// Mark the group as done before the last shot in group.
		bool isFinalShot = player.itemAnimation <= CombinedHooks.TotalUseTime(Item.useTime, player, Item) // not enough time to shoot again
			|| (Item.useLimitPerAnimation != null && player.ItemUsesThisAnimation == Item.useLimitPerAnimation); // this shot hits the limit
		if (isFinalShot && modPlayer.RazorbeakHitInfo.TryGetValue(modPlayer.GroupIndex, out var value))
		{
			modPlayer.RazorbeakHitInfo[modPlayer.GroupIndex] = (value.Count, true); // Mark the group as done
		}

		var proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
		SoundEngine.PlaySound(Item.UseSound = SoundID.Item5, position);

		// Set global projectile properties
		var gProj = proj.GetGlobalProjectile<RazorbeakGlobalProjectile>();
		gProj.ShotByRazorbeakBow = true;
		gProj.GroupIndex = modPlayer.GroupIndex;

		return false;
	}

	public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
	{
		if (Main.LocalPlayer.HeldItem.ModItem != this)
		{
			return;
		}

		// Draw text
		var modPlayer = Main.LocalPlayer.GetModPlayer<RazorbeakBowPlayer>();
		var progress = modPlayer.RazorbeakBowEffectStack / (float)RazorbeakBowPlayer.RazorbeakBowEffectMaxStack;

		var text = $"{1 + modPlayer.RazorbeakBowEffectStack * RazorbeakBowPlayer.RazorbeakBowAttackSpeedBonusPerStack}x";
		var textSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, text, Vector2.One);
		var textPos = Main.LocalPlayer.Bottom - Main.screenPosition;
		var textColor = new Color(
			Color.LimeGreen.G / 255f * (0.9f + progress * 0.2f),
			MathHelper.Lerp(1f, Color.LimeGreen.R / 255f * (0.7f + progress * 0.3f), progress),
			Color.LimeGreen.B / 255f)
			* (0.9f + 0.1f * MathF.Sin((float)Main.timeForVisualEffects * 0.04f));
		var textScale = 1f + 0.05f * modPlayer.RazorbeakBowEffectStack;

		var sBS = GraphicsUtils.GetState(spriteBatch).Value;
		spriteBatch.End();
		spriteBatch.Begin(sBS);

		spriteBatch.transformMatrix = Main.GameViewMatrix.ZoomMatrix;
		spriteBatch.DrawString(FontAssets.MouseText.Value, text, textPos, textColor, 0, new Vector2(textSize.X * 0.5f, -textSize.Y * 0.5f), textScale, SpriteEffects.None, 0);

		// Draw timer
		float timer = modPlayer.RazorbeakBowTimer / (float)RazorbeakBowPlayer.RazorbeakBowEffectDuration;
		if (timer > 0)
		{
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
			ValueBarHelper.DrawCircleValueBar(spriteBatch, textPos + new Vector2(0, 50f * textScale), 1 - timer, Color.Black, Color.White, 0.08f);
		}

		spriteBatch.End();
		spriteBatch.Begin(sBS);
	}

	public class RazorbeakBowPlayer : ModPlayer
	{
		public const int RazorbeakBowEffectMaxStack = 8;
		public const int RazorbeakBowEffectDuration = 180;
		public const int RazorbeakBowEffectHitRequirement = 3;
		public const float RazorbeakBowAttackSpeedBonusPerStack = 0.25f;

		public int RazorbeakBowEffectStack { get; set; } = 0;

		public int RazorbeakBowTimer { get; set; } = 0;

		public int GroupIndex { get; set; } = 0;

		public Dictionary<int, (int Count, bool Done)> RazorbeakHitInfo { get; } = [];

		public override void PostUpdate()
		{
			// Reset attack speed bonus if the player stop hitting targets for 3 seconds.
			if (RazorbeakBowTimer <= 0)
			{
				RazorbeakBowEffectStack = 0;
			}
			else
			{
				RazorbeakBowTimer--;
			}
		}
	}

	public class RazorbeakGlobalProjectile : GlobalProjectile
	{
		public override bool InstancePerEntity => true;

		public bool ShotByRazorbeakBow { get; set; } = false;

		public bool RazorbeakBowEffectTriggered { get; set; } = false;

		public int GroupIndex { get; set; }

		public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (ShotByRazorbeakBow && !RazorbeakBowEffectTriggered)
			{
				var modPlayer = Main.player[projectile.owner].GetModPlayer<RazorbeakBowPlayer>();

				// Reset hit effect timer.
				modPlayer.RazorbeakBowTimer = RazorbeakBowPlayer.RazorbeakBowEffectDuration;

				// Process effect counter.
				if (modPlayer.RazorbeakHitInfo.TryGetValue(GroupIndex, out var value))
				{
					RazorbeakBowEffectTriggered = true;

					var count = value.Count + 1;
					if (count < RazorbeakBowPlayer.RazorbeakBowEffectHitRequirement)
					{
						modPlayer.RazorbeakHitInfo[GroupIndex] = (count, value.Done);
					}
					else
					{
						modPlayer.RazorbeakBowEffectStack = Math.Min(modPlayer.RazorbeakBowEffectStack + 1, RazorbeakBowPlayer.RazorbeakBowEffectMaxStack);
						modPlayer.RazorbeakHitInfo.Remove(GroupIndex); // Remove element immediately ensuring stack won't triggered twice by a single group.
					}
				}
			}
		}

		public override void OnKill(Projectile projectile, int timeLeft)
		{
			if (ShotByRazorbeakBow)
			{
				// Remove useless group index from the player's RazorbeakHitInfo。
				var modPlayer = Main.player[projectile.owner].GetModPlayer<RazorbeakBowPlayer>();
				if (!modPlayer.RazorbeakHitInfo.TryGetValue(GroupIndex, out var hitInfo) || !hitInfo.Done)
				{
					return;
				}

				if (!Main.projectile.Any(proj =>
					proj.active
					&& proj.owner == projectile.owner
					&& proj.type == projectile.type
					&& proj.whoAmI != projectile.whoAmI
					&& proj.GetGlobalProjectile<RazorbeakGlobalProjectile>().GroupIndex == GroupIndex))
				{
					modPlayer.RazorbeakHitInfo.Remove(GroupIndex);
				}
			}
		}
	}
}

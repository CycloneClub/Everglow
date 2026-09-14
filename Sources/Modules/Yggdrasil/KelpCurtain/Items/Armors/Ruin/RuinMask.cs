using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.KelpCurtain.Buffs;
using Everglow.Yggdrasil.KelpCurtain.Items.Weapons.Ruin;
using Everglow.Yggdrasil.KelpCurtain.Projectiles.Summon;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.Graphics.Shaders;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Armors.Ruin;

[AutoloadEquip(EquipType.Head)]
public class RuinMask : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Armor;

	public override void SetStaticDefaults()
	{
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
	}

	public override void SetDefaults()
	{
		Item.width = 18;
		Item.height = 18;

		Item.defense = 1;

		Item.value = Item.buyPrice(gold: 1);
		Item.rare = ItemRarityID.Gray;
	}

	public override void UpdateEquip(Player player)
	{
		player.maxMinions += 1;
		player.statManaMax2 += 40;
	}

	public override bool IsArmorSet(Item head, Item body, Item legs)
	{
		return body.type == ModContent.ItemType<RuinMagicRobe>() && legs.type == ModContent.ItemType<RuinLeggings>();
	}

	public override void UpdateArmorSet(Player player)
	{
		player.maxMinions += 1;
		player.GetModPlayer<RuinSetPlayer>().RuinSetEnable = true;
		player.GetModPlayer<YggdrasilPlayer>().ruinSet = true;
	}

	public class RuinSetPlayer : ModPlayer
	{
		public const int BuffDuration = 30 * 60;
		public const int CooldownDuration = 120 * 60;

		public bool RuinSetEnable { get; set; } = false;

		public bool RuinSetBuffActive => RuinSetEnable && Player.HasBuff<RuinSetBuff>();

		public override void ResetEffects()
		{
			RuinSetEnable = false;
		}

		public override void FrameEffects()
		{
			if (RuinSetEnable)
			{
				if (Player.HeldItem.type == ModContent.ItemType<WoodlandWraithStaff>())
				{
					Player.armorEffectDrawShadow = true;
					Player.armorEffectDrawOutlines = Player.HasBuff<RuinSetBuff>();
				}
			}
		}

		public override bool CanUseItem(Item item) => Player.ownedProjectileCounts[ModContent.ProjectileType<WoodlandWraithStaff_SetAnimation>()] <= 0;

		public override void PostUpdate()
		{
			// Constraint for vanities, accessories and miscs effects that can change player visible equipments.
			if (Player.face == -1
				&& Player.head == EquipLoader.GetEquipSlot(Mod, nameof(RuinMask), EquipType.Head)
				&& Player.body == EquipLoader.GetEquipSlot(Mod, nameof(RuinMagicRobe), EquipType.Body)
				&& Player.legs == EquipLoader.GetEquipSlot(Mod, nameof(RuinLeggings), EquipType.Legs))
			{
				GenerateEyeGlowDust(Player);
			}
		}

		/// <summary>
		/// Generate dusts as eye glow. Same as vanilla accessory <see cref="ItemID.Yoraiz0rDarkness"/>.
		/// <para/> This method is copied from <see cref="Player.Yoraiz0rEye"/> with some modifications.
		/// </summary>
		private static void GenerateEyeGlowDust(Player player)
		{
			int offsetsPlayerHeadgearIndex = player.bodyFrame.Y / 56;
			if (offsetsPlayerHeadgearIndex >= Main.OffsetsPlayerHeadgear.Length)
			{
				offsetsPlayerHeadgearIndex = 0;
			}
			var frameOffset = Main.OffsetsPlayerHeadgear[offsetsPlayerHeadgearIndex];
			var dustMaskOffset = new Vector2(-1f, 1.5f);
			Vector2 vector = frameOffset + dustMaskOffset;
			vector *= player.Directions;

			Vector2 vector2 = new Vector2(player.width / 2, player.height / 2) + vector + (player.MountedCenter - player.Center);
			player.sitting.GetSittingOffsetInfo(player, out var posOffset, out var seatAdjustment);
			vector2 += posOffset + new Vector2(0f, seatAdjustment);

			if (player.mount.Active && player.mount.Type == 52)
			{
				vector2.X += 14f * (float)player.direction;
				vector2.Y -= 2f * player.gravDir;
			}

			float y = -11.5f * player.gravDir;
			Vector2 vector3 = new Vector2(3 * player.direction - ((player.direction == 1) ? 1 : 0), y) + Vector2.UnitY * player.gfxOffY + vector2;
			Vector2 vector4 = new Vector2(3 * player.shadowDirection[1] - ((player.direction == 1) ? 1 : 0), y) + vector2;
			Vector2 vector5 = Vector2.Zero;
			if (player.mount.Active && player.mount.Cart)
			{
				int num2 = Math.Sign(player.velocity.X);
				if (num2 == 0)
				{
					num2 = player.direction;
				}

				vector5 = new Vector2(MathHelper.Lerp(0f, -8f, player.fullRotation / ((float)Math.PI / 4f)), MathHelper.Lerp(0f, 2f, Math.Abs(player.fullRotation / ((float)Math.PI / 4f)))).RotatedBy(player.fullRotation);
				if (num2 == Math.Sign(player.fullRotation))
				{
					vector5 *= MathHelper.Lerp(1f, 0.6f, Math.Abs(player.fullRotation / ((float)Math.PI / 4f)));
				}
			}

			if (player.fullRotation != 0f)
			{
				vector3 = vector3.RotatedBy(player.fullRotation, player.fullRotationOrigin);
				vector4 = vector4.RotatedBy(player.fullRotation, player.fullRotationOrigin);
			}

			float num3 = 0f;
			Vector2 vector6 = player.position + vector3 + vector5;
			Vector2 vector7 = player.oldPosition + vector4 + vector5;
			vector7.Y -= num3 / 2f;
			vector6.Y -= num3 / 2f;

			DelegateMethods.v3_1 = Main.hslToRgb(Main.rgbToHsl(player.eyeColor).X, 1f, 0.5f).ToVector3() * 0.5f * 1f;
			if (player.velocity != Vector2.Zero)
			{
				Utils.PlotTileLine(player.Center, player.Center + player.velocity * 2f, 4f, DelegateMethods.CastLightOpen);
			}
			else
			{
				Utils.PlotTileLine(player.Left, player.Right, 4f, DelegateMethods.CastLightOpen);
			}

			int dustCount = (int)Vector2.Distance(vector6, vector7) / 3 + 1;
			if (Vector2.Distance(vector6, vector7) % 3f != 0f)
			{
				dustCount++;
			}

			// Generate dusts.
			for (int i = 1; i <= dustCount; i++)
			{
				Dust obj = Main.dust[Dust.NewDust(player.Center, 0, 0, DustID.TheDestroyer)];
				obj.position = Vector2.Lerp(vector7, vector6, i / (float)dustCount);
				obj.noGravity = true;
				obj.velocity = Vector2.Zero;
				obj.scale = 0.8f;
				obj.shader = GameShaders.Armor.GetSecondaryShader(player.cYorai, player);
			}
		}
	}
}

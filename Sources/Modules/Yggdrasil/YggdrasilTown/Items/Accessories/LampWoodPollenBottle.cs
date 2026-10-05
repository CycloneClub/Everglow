using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria;
using Terraria.Audio;
using Terraria.Enums;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Accessories
{
	// Showcases a more complicated extra jump, where the player can jump mid-air with it three (3) times
	public class LampWoodPollenBottle : ModItem
	{
		public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

		public override void SetDefaults()
		{
			Item.DefaultToAccessory(20, 26);
			Item.SetShopValues(ItemRarityColor.Orange3, Item.buyPrice(gold: 2));
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetJumpState<LampWoodPollenBottleJump>().Enable();
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			// Find the line that contains the dummy string from the localization text
			int index = tooltips.FindIndex(static line => line.Text.Contains("<JUMPS>"));
			if (index >= 0)
			{
				// ... and then replace it
				ref string text = ref tooltips[index].Text;
				text = text.Replace("<JUMPS>", $"{Main.LocalPlayer.GetModPlayer<LampWoodPollenBottleJumpPlayer>().jumpsRemaining}");
			}
		}
	}

	public class LampWoodPollenBottleJump : ExtraJump
	{
		public override Position GetDefaultPosition() => new After(BlizzardInABottle);

		public override float GetDurationMultiplier(Player player)
		{
			// Each successive jump has weaker power
			return player.GetModPlayer<LampWoodPollenBottleJumpPlayer>().jumpsRemaining switch
			{
				1 => 1.3f,
				2 => 0.8f,
				_ => 0f,
			};
		}

		public override void OnRefreshed(Player player)
		{
			// Reset the jump counter
			player.GetModPlayer<LampWoodPollenBottleJumpPlayer>().jumpsRemaining = 2;
		}

		public override void OnStarted(Player player, ref bool playSound)
		{
			// Get the jump counter
			ref int jumps = ref player.GetModPlayer<LampWoodPollenBottleJumpPlayer>().jumpsRemaining;

			if (jumps == 2)
			{
				const int numDusts = 30;
				for (int i = 0; i < numDusts; i++)
				{
					VisualDust(player);
				}
			}
			else
			{
				const int numDusts = 24;
				for (int i = 0; i < numDusts; i++)
				{
					VisualDust(player);
				}
			}

			// Play a different sound
			playSound = false;

			float pitch = jumps switch
			{
				1 => 0.5f,
				2 => 0.1f,
				_ => 0,
			};

			SoundEngine.PlaySound(SoundID.Item8 with { Pitch = pitch, PitchVariance = 0.04f }, player.Bottom);

			// Decrement the jump counter
			jumps--;

			// Allow the jump to be used again while the jump counter is > 0
			if (jumps > 0)
			{
				player.GetJumpState(this).Available = true;
			}
		}

		public void VisualDust(Player player, int count = 3)
		{
			int offsetY = player.height;
			if (player.gravDir == -1f)
			{
				offsetY = 0;
			}

			offsetY -= 16;
			Vector2 center = player.Top + new Vector2(0, offsetY);
			for (int i = 0; i < count / 3; i++)
			{
				(float sin, float cos) = MathF.SinCos(Main.rand.NextFloat(MathHelper.TwoPi));

				float amplitudeX = cos * (player.width - 4) / 2f;
				float amplitudeY = sin * 3;
				Vector2 newVelocity = new Vector2(amplitudeX * 5, amplitudeY * 0.2f + Main.rand.NextFloat(6, 15)) * 0.2f;
				var somg = new LampWoodPollenPurpleDust
				{
					Velocity = newVelocity,
					Active = true,
					Visible = true,
					Position = center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
					MaxTime = Main.rand.Next(60, 75),
					Scale = Main.rand.NextFloat(50f, 155f),
					Rotation = Main.rand.NextFloat(6.283f),
					ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
				};
				Ins.VFXManager.Add(somg);
			}
			for (int i = 0; i < count / 3; i++)
			{
				(float sin, float cos) = MathF.SinCos(Main.rand.NextFloat(MathHelper.TwoPi));

				float amplitudeX = cos * (player.width - 4) / 2f;
				float amplitudeY = sin * 3;
				var dust = Dust.NewDustPerfect(center + new Vector2(amplitudeX, amplitudeY), ModContent.DustType<LampWood_Dust_fluorescent_appear>(), -player.velocity * 0.2f, Scale: 1f);
				dust.alpha = 0;
				dust.rotation = MathF.Pow(Main.rand.NextFloat(0.2f, 0.9f), 3) * 3f;
				dust.velocity += new Vector2(amplitudeX * 3, amplitudeY * 0.2f + Main.rand.NextFloat(2, 21)) * 0.2f;
			}
		}
	}

	public class LampWoodPollenBottleJumpPlayer : ModPlayer
	{
		public int jumpsRemaining;
	}
}

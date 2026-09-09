using Everglow.Commons.UI;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.FurnaceTiles;
using Everglow.Yggdrasil.YggdrasilTown.UI;
using MonoMod.Cil;
using ReLogic.Graphics;
using Terraria.GameContent;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace Everglow.Yggdrasil.YggdrasilTown;

public class YggdrasilTownFurnaceSystem : ModSystem
{
	public static Player CurrentPlayer;

	public static int CurrentScore = 0;

	public static int CurrentEnergy = 0;

	public static List<int> MeltingAnimationTimer = new List<int>();

	public static int EnergtMax = 100000;

	public static int PlayerDropInFloorScaleTimer = 0;

	public static int PlayerLeaveFloorScaleTimer = 0;

	public static bool EnquiryMeltingDown = false;

	public static bool MeltingDownButtonClicked = false;

	public static bool FurnaceScoreShopOpen = false;

	public override void Load()
	{
		base.Load();
		UISystem.Instance.PostDrawChestUI += DrawFurnaceMeltingChest;
	}

	public void DrawFurnaceMeltingChest(UISystem system, SpriteBatch spriteBatch)
	{
		Player player = Main.LocalPlayer;
		if (player.chest >= 0)
		{
			int i = Main.chest[player.chest].x;
			int j = Main.chest[player.chest].y;
			var tile = TileUtils.SafeGetTile(i, j);
			if (tile.TileType == ModContent.TileType<FurnaceMeltingChest>())
			{
				// Point panel
				Draw9Piece_ChatBack(spriteBatch, new Rectangle(70, 426, 340, 40), new Color(1f, 0.3f, 0.2f, 0.6f));
				Chest chest = Main.chest[player.chest];
				int totalValue = 0;
				for (int k = 0; k < chest.item.Length; k++)
				{
					Item item = chest.item[k];
					if (item != null)
					{
						float itemValue = 1 + item.value / (100 + MathF.Sqrt(item.value * 10));
						int rare = Math.Min(10, item.rare);
						float rareValue = 6f - (rare - 10) * (rare - 10) / 20f;
						int value = (int)(rareValue * itemValue * item.stack);
						totalValue += value;
					}
				}
				if (EnquiryMeltingDown)
				{
					Utils.DrawBorderStringFourWay(spriteBatch,FontAssets.MouseText.Value, "Clear the chest and get points?", 86, 436, Color.White, Color.Black, Vector2.zeroVector);
				}
				else
				{
					Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.MouseText.Value, "Furnace Points: " + totalValue, 86, 436, Color.White, Color.Black, Vector2.zeroVector);
				}

				// Meltdown button
				var iconFrame = new Rectangle(0, 0, 14, 24);
				var buttonColor = new Color(1f, 0f, 0f, 0.75f);
				bool mouseOver = false;
				if (CurrentPlayer is null)
				{
					buttonColor = new Color(0.25f, 0.25f, 0.25f, 0.5f);
					EnquiryMeltingDown = false;
					MeltingDownButtonClicked = false;
				}
				else
				{
					if (new Rectangle(410, 426, 40, 40).Contains(Main.MouseScreen.ToPoint()))
					{
						buttonColor = new Color(1f, 0.1f, 0.35f, 0.95f);
						Main.instance.MouseText("Meltdown", ItemRarityID.Red);
						mouseOver = true;
						if (Main.mouseLeft && Main.mouseLeftRelease)
						{
							MeltingDownButtonClicked = !MeltingDownButtonClicked;
							EnquiryMeltingDown = MeltingDownButtonClicked;
						}
					}
				}
				Draw9Piece_ChatBack(spriteBatch, new Rectangle(410, 426, 40, 40), buttonColor);
				if (mouseOver || MeltingDownButtonClicked)
				{
					Draw9Piece_ChatBack_Highlight(spriteBatch, new Rectangle(410, 426, 40, 40));
					iconFrame = new Rectangle(16, 0, 14, 24);
				}
				Texture2D icon = ModAsset.FurnaceMeltingChest_MeltingIcon.Value;
				spriteBatch.Draw(icon, new Vector2(430, 446), iconFrame, Color.White, 0, iconFrame.Size() * 0.5f, 1f, SpriteEffects.None, 0);

				// Confirmation panel
				Rectangle yes_button = new Rectangle(20, 258, 46, 82);
				Rectangle no_button = new Rectangle(20, 342, 46, 82);
				Color yesColor = new Color(0.25f, 0.25f, 0.25f, 0.5f);
				Color noColor = new Color(0.25f, 0.25f, 0.25f, 0.5f);
				bool mouseOverYes = false;
				bool mouseOverNo = false;
				Color yesTextColor = new Color(0.2f, 0.2f, 0.2f, 1f);
				Color noTextColor = new Color(0.2f, 0.2f, 0.2f, 1f);
				if (EnquiryMeltingDown)
				{
					yesTextColor = new Color(0.45f, 0.45f, 0.45f, 1f);
					noTextColor = new Color(0.45f, 0.45f, 0.45f, 1f);
					yesColor = new Color(1f, 0f, 0f, 0.75f);
					if (yes_button.Contains(Main.MouseScreen.ToPoint()))
					{
						mouseOverYes = true;
						if (Main.mouseLeft && Main.mouseLeftRelease)
						{
							player.chest = -1;
							MeltingButton.MeltDown(i, j);
						}
					}

					noColor = new Color(0.6f, 0.6f, 0.6f, 0.75f);
					if (no_button.Contains(Main.MouseScreen.ToPoint()))
					{
						mouseOverNo = true;
						if (Main.mouseLeft && Main.mouseLeftRelease)
						{
							MeltingDownButtonClicked = !MeltingDownButtonClicked;
							EnquiryMeltingDown = MeltingDownButtonClicked;
						}
					}
				}
				Draw9Piece_ChatBack(spriteBatch, yes_button, yesColor);
				Draw9Piece_ChatBack(spriteBatch, no_button, noColor);
				if (mouseOverYes)
				{
					yesTextColor = new Color(1f, 1f, 1f, 1f);
					Draw9Piece_ChatBack_Highlight(spriteBatch, yes_button);
				}
				if (mouseOverNo)
				{
					noTextColor = new Color(1f, 1f, 1f, 1f);
					Draw9Piece_ChatBack_Highlight(spriteBatch, no_button);
				}
				Vector2 yesPos = yes_button.Center() - new Vector2(12, 12);
				Vector2 noPos = no_button.Center() - new Vector2(12, 12);
				Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.MouseText.Value, "Yes", yesPos.X, yesPos.Y, yesTextColor,Color.Black,Vector2.zeroVector);
				Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.MouseText.Value, "No", noPos.X, noPos.Y, noTextColor, Color.Black, Vector2.zeroVector);
			}
		}
	}

	public void Draw9Piece_ChatBack(SpriteBatch spriteBatch, Rectangle destinationBox, Color drawColor)
	{
		Texture2D tex = TextureAssets.ChatBack.Value;
		spriteBatch.Draw(tex, new Rectangle(destinationBox.X, destinationBox.Y, 8, 8), new Rectangle(0, 0, 8, 8), drawColor);
		spriteBatch.Draw(tex, new Rectangle(destinationBox.X + 8, destinationBox.Y, destinationBox.Width - 16, 8), new Rectangle(8, 0, 2, 8), drawColor);
		spriteBatch.Draw(tex, new Rectangle(destinationBox.X + destinationBox.Width - 8, destinationBox.Y, 8, 8), new Rectangle(tex.Width - 8, 0, 8, 8), drawColor);

		spriteBatch.Draw(tex, new Rectangle(destinationBox.X, destinationBox.Y + 8, 8, destinationBox.Height - 16), new Rectangle(0, 8, 8, 2), drawColor);
		spriteBatch.Draw(tex, new Rectangle(destinationBox.X + 8, destinationBox.Y + 8, destinationBox.Width - 16, destinationBox.Height - 16), new Rectangle(8, 8, 2, 2), drawColor);
		spriteBatch.Draw(tex, new Rectangle(destinationBox.X + destinationBox.Width - 8, destinationBox.Y + 8, 8, destinationBox.Height - 16), new Rectangle(tex.Width - 8, 8, 8, 2), drawColor);

		spriteBatch.Draw(tex, new Rectangle(destinationBox.X, destinationBox.Y + destinationBox.Height - 8, 8, 8), new Rectangle(0, tex.Height - 8, 8, 8), drawColor);
		spriteBatch.Draw(tex, new Rectangle(destinationBox.X + 8, destinationBox.Y + destinationBox.Height - 8, destinationBox.Width - 16, 8), new Rectangle(8, tex.Height - 8, 2, 8), drawColor);
		spriteBatch.Draw(tex, new Rectangle(destinationBox.X + destinationBox.Width - 8, destinationBox.Y + destinationBox.Height - 8, 8, 8), new Rectangle(tex.Width - 8, tex.Height - 8, 8, 8), drawColor);
	}

	public void Draw9Piece_ChatBack_Highlight(SpriteBatch spriteBatch, Rectangle destinationBox)
	{
		Color drawColor = Color.White;
		Texture2D tex = Commons.ModAsset.Vanilla_Chat_Back_Highlight.Value;
		spriteBatch.Draw(tex, new Rectangle(destinationBox.X, destinationBox.Y, 8, 8), new Rectangle(0, 0, 8, 8), drawColor);
		spriteBatch.Draw(tex, new Rectangle(destinationBox.X + 8, destinationBox.Y, destinationBox.Width - 16, 8), new Rectangle(8, 0, 2, 8), drawColor);
		spriteBatch.Draw(tex, new Rectangle(destinationBox.X + destinationBox.Width - 8, destinationBox.Y, 8, 8), new Rectangle(tex.Width - 8, 0, 8, 8), drawColor);

		spriteBatch.Draw(tex, new Rectangle(destinationBox.X, destinationBox.Y + 8, 8, destinationBox.Height - 16), new Rectangle(0, 8, 8, 2), drawColor);
		spriteBatch.Draw(tex, new Rectangle(destinationBox.X + destinationBox.Width - 8, destinationBox.Y + 8, 8, destinationBox.Height - 16), new Rectangle(tex.Width - 8, 8, 8, 2), drawColor);

		spriteBatch.Draw(tex, new Rectangle(destinationBox.X, destinationBox.Y + destinationBox.Height - 8, 8, 8), new Rectangle(0, tex.Height - 8, 8, 8), drawColor);
		spriteBatch.Draw(tex, new Rectangle(destinationBox.X + 8, destinationBox.Y + destinationBox.Height - 8, destinationBox.Width - 16, 8), new Rectangle(8, tex.Height - 8, 2, 8), drawColor);
		spriteBatch.Draw(tex, new Rectangle(destinationBox.X + destinationBox.Width - 8, destinationBox.Y + destinationBox.Height - 8, 8, 8), new Rectangle(tex.Width - 8, tex.Height - 8, 8, 8), drawColor);
	}

	public override void PostUpdateEverything()
	{
		if (CurrentPlayer != null)
		{
			FurnacePlayer fPlayer = CurrentPlayer.GetModPlayer<FurnacePlayer>();
			CurrentScore = fPlayer.TotalFurnaceScore;
		}
		if (CurrentEnergy >= 1000)
		{
			CurrentEnergy--;
		}
		else
		{
			CurrentEnergy = 75000;
		}
		if (CurrentEnergy > EnergtMax)
		{
			CurrentEnergy = EnergtMax;
		}
		if (PlayerDropInFloorScaleTimer > 0)
		{
			PlayerDropInFloorScaleTimer--;
		}
		if (PlayerLeaveFloorScaleTimer > 0)
		{
			PlayerLeaveFloorScaleTimer--;
		}

		for (int k = MeltingAnimationTimer.Count - 1; k >= 0; k--)
		{
			if (MeltingAnimationTimer[k] > 0)
			{
				MeltingAnimationTimer[k]--;
			}
			else
			{
				MeltingAnimationTimer.RemoveAt(k);
			}
		}
		base.PostUpdateEverything();
	}
}

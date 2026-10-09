using Everglow.Food.Items.ModFood;
using ReLogic.Graphics;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.UI;
using Terraria.UI.Chat;
using static Everglow.Food.UI.StoveUIManager;

namespace Everglow.Yggdrasil.YggdrasilTown.Kitchen.KitchenSystem;

public class KitchenSystemUI : GameInterfaceLayer
{
	public static List<FoodRequestPanel> FoodRequests = new List<FoodRequestPanel>();
	public static List<ScoreChange> ScoreChangeList = new List<ScoreChange>();

	public KitchenSystemUI(string name, InterfaceScaleType scaleType)
		: base(name, scaleType)
	{
	}

	public static int Score = 0;
	public static int TotalScore = 0;
	public static int TargetScore = 0;
	public static int Level = 1;
	public static int MaxTime = 7200;
	public static int Timer = 7200;
	public static int OldTargetScore;
	public static int StartInterfaceAnimationTimer = 30;
	public static Vector2 MainPanelOrigin;
	public static Vector2 MainPanelOriginMinimized;
	public static Vector2 PauseContinuePanelOffset;
	public static Vector2 DragPanelOffset;
	public static Vector2 RestartPanelOffset;
	public static Vector2 MaximizePanelOffset;
	public static Vector2 StartNewGamePanelOffset;
	public static Vector2 AccountPanelOffset;
	public Vector2 DragStartMousePos;
	public Vector2 DragStartPanelPos;
	public static bool Pause = false;
	public static bool Started = false;
	public static bool IsMouseOverPauseContinue;
	public static bool IsMouseOverDrag;
	public static bool IsMouseOverRestart;
	public static bool IsMouseOverMaximize;
	public static bool IsMouseOverStartNewGame;
	public static bool IsMouseOverAccount;
	public static bool Maximized;
	public static bool IsDragging;
	public static bool Failed;

	public static void Reset()
	{
		Restart();
		MainPanelOrigin = new Vector2(530, Main.screenHeight - 300);
		MainPanelOriginMinimized = new Vector2(330, Main.screenHeight - 300);
		Failed = false;
	}

	public static void Restart()
	{
		FoodRequests = new List<FoodRequestPanel>();
		ScoreChangeList = new List<ScoreChange>();
		Score = 0;
		TotalScore = 0;
		Pause = true;
		Started = false;
		MaxTime = 7200;
		Timer = MaxTime;
		Level = 1;
		StartInterfaceAnimationTimer = 30;
		OldTargetScore = TargetScore;
		TargetScore = GetTargetScore();
	}

	public static void Fail()
	{
		Restart();
		Failed = true;
	}

	public static void NextLevel()
	{
		if (Level < 6)
		{
			FoodRequests = new List<FoodRequestPanel>();
			ScoreChangeList = new List<ScoreChange>();
			TotalScore += Score;
			Score = 0;
			Pause = true;
			Started = false;
			MaxTime = 7200;
			Timer = MaxTime;
			++Level;
			StartInterfaceAnimationTimer = 30;
			TargetScore = GetTargetScore();
		}
	}

	public override bool DrawSelf()
	{
		if (!Main.gamePaused)
		{
			Update();
		}

		// Main panel
		if (Maximized)
		{
			Draw9Pieces(MainPanelOrigin, 400, 160, new Color(0.3f, 0.2f, 0.2f), 0);
		}

		// foodRequsets panel
		if (FoodRequests.Count > 0)
		{
			foreach (var foodRequest in FoodRequests)
			{
				foodRequest.Draw();
			}
		}

		// Start interface
		if (StartInterfaceAnimationTimer < 30)
		{
			DrawStartInterface();
		}

		// Score bar
		DrawScoreBar();
		for (int index = ScoreChangeList.Count - 1; index >= 0; index--)
		{
			ScoreChange scoreChange = ScoreChangeList[index];
			scoreChange.Draw();
		}

		// pause panel
		Color defaulePanel = new Color(0.3f, 0.2f, 0.2f);
		float scalePause = 2f;
		if (IsMouseOverPauseContinue)
		{
			defaulePanel = new Color(0.1f, 0.05f, 0.04f);
			if (!Maximized)
			{
				scalePause = 3f;
			}
		}
		if (Maximized)
		{
			Draw9Pieces(PauseContinuePanelOffset, 24, 24, defaulePanel, 0);
		}
		Texture2D icons = ModAsset.FoodRequestUIPanelIcons.Value;
		Rectangle pauseFrame = new Rectangle(0, 0, 13, 13);
		Rectangle continueFrame = new Rectangle(0, 13, 13, 13);
		Main.spriteBatch.Draw(icons, PauseContinuePanelOffset, Pause ? continueFrame : pauseFrame, Color.AliceBlue, 0, pauseFrame.Size() * 0.5f, scalePause, SpriteEffects.None, 0);

		// drag panel
		defaulePanel = new Color(0.3f, 0.2f, 0.2f);
		float scaleDrag = 2f;
		if (IsMouseOverDrag)
		{
			defaulePanel = new Color(0.1f, 0.05f, 0.04f);
			if (!Maximized)
			{
				scaleDrag = 3f;
			}
		}
		Rectangle dragFrame = new Rectangle(0, 39, 13, 13);
		if (Maximized)
		{
			Draw9Pieces(DragPanelOffset, 24, 24, defaulePanel, 0);
		}
		else
		{
			dragFrame = new Rectangle(13, 39, 13, 13);
		}
		Main.spriteBatch.Draw(icons, DragPanelOffset, dragFrame, Color.AliceBlue, 0, dragFrame.Size() * 0.5f, scaleDrag, SpriteEffects.None, 0);

		// restart panel
		defaulePanel = new Color(0.3f, 0.2f, 0.2f);
		float scaleRestart = 2f;
		if (IsMouseOverRestart)
		{
			defaulePanel = new Color(0.1f, 0.05f, 0.04f);
			if (!Maximized)
			{
				scaleRestart = 3f;
			}
		}
		if (Maximized)
		{
			Draw9Pieces(RestartPanelOffset, 24, 24, defaulePanel, 0);
		}
		Rectangle resetFrame = new Rectangle(0, 26, 13, 13);
		Main.spriteBatch.Draw(icons, RestartPanelOffset, resetFrame, Color.AliceBlue, 0, resetFrame.Size() * 0.5f, scaleRestart, SpriteEffects.None, 0);

		// maximize panel
		defaulePanel = new Color(0.3f, 0.2f, 0.2f);
		float scaleMaximize = 2f;
		if (IsMouseOverMaximize)
		{
			defaulePanel = new Color(0.1f, 0.05f, 0.04f);
			if (!Maximized)
			{
				scaleMaximize = 3f;
			}
		}
		if (Maximized)
		{
			Draw9Pieces(MaximizePanelOffset, 24, 24, defaulePanel, 0);
		}
		Rectangle maximizeFrame = new Rectangle(13, 0, 13, 13);
		Rectangle minimizeFrame = new Rectangle(13, 13, 13, 13);
		Main.spriteBatch.Draw(icons, MaximizePanelOffset, Maximized ? minimizeFrame : maximizeFrame, Color.AliceBlue, 0, minimizeFrame.Size() * 0.5f, scaleMaximize, SpriteEffects.None, 0);
		return true;
	}

	public void DrawStartInterface()
	{
		Color defauleIcon = new Color(0.3f, 1f, 0f);
		Color defaulePanel = new Color(0.3f, 0.2f, 0.2f);
		if (IsMouseOverStartNewGame)
		{
			defauleIcon = new Color(0.6f, 1f, 0.3f);
			defaulePanel = new Color(0.1f, 0.05f, 0.04f);
		}
		float startAnimationValue = MathF.Pow(StartInterfaceAnimationTimer / 30f, 3);
		defaulePanel *= 1 - startAnimationValue;
		defauleIcon *= 1 - startAnimationValue;
		if (Maximized)
		{
			StartNewGamePanelOffset = MainPanelOrigin + new Vector2(0, -startAnimationValue * 80);
			if (Level is > 1 and <= 5)
			{
				StartNewGamePanelOffset = MainPanelOrigin + new Vector2(-80, -startAnimationValue * 80);
			}
		}

		// Minimized
		else
		{
			StartNewGamePanelOffset = MainPanelOriginMinimized + new Vector2(0, -startAnimationValue * 80);
			if (Level is > 1 and <= 5)
			{
				StartNewGamePanelOffset = MainPanelOriginMinimized + new Vector2(-30, -startAnimationValue * 80);
			}
		}

		// Start Green Triangle
		if (Maximized)
		{
			if (Level < 6)
			{
				Draw9Pieces(StartNewGamePanelOffset, 48, 48, defaulePanel, 0);
				Texture2D startNewGame = ModAsset.StartNewGame.Value;
				if (Failed)
				{
					startNewGame = ModAsset.StartNewGame_Fail.Value;
				}
				Main.spriteBatch.Draw(startNewGame, StartNewGamePanelOffset, null, defauleIcon, 0, startNewGame.Size() * 0.5f, 1f, SpriteEffects.None, 0);
			}
		}
		else
		{
			if (Level < 6)
			{
				Draw9Pieces(StartNewGamePanelOffset, 20, 20, defaulePanel, 0);
				Texture2D startNewGame = ModAsset.StartNewGame.Value;
				if (Failed)
				{
					startNewGame = ModAsset.StartNewGame_Fail.Value;
				}
				Main.spriteBatch.Draw(startNewGame, StartNewGamePanelOffset, null, defauleIcon, 0, startNewGame.Size() * 0.5f, 0.5f, SpriteEffects.None, 0);
			}
		}
		Vector2 StartTextOffset = MainPanelOrigin + new Vector2(0, startAnimationValue * 80);
		if (!Maximized)
		{
			StartTextOffset = MainPanelOriginMinimized + new Vector2(0, startAnimationValue * 40 + 40);
		}
		Color textColor = new Color(1f, 1f, 0.5f) * (1 - startAnimationValue);
		string textStart = "Click to start a cooking...";
		if (Level is > 1 and <= 5)
		{
			textStart = "Click green triangle to start next cooking level...";
		}
		Vector2 textSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, textStart, Vector2.One);
		if (Level < 6)
		{
			if (Maximized)
			{
				Main.spriteBatch.DrawString(FontAssets.MouseText.Value, textStart, StartTextOffset + new Vector2(0, 70), textColor, 0, textSize * 0.5f, 1f, SpriteEffects.None, 0);
				string textTarget = "Target profit : " + GetTargetScore();
				textSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, textTarget, Vector2.One);
				Main.spriteBatch.DrawString(FontAssets.MouseText.Value, textTarget, StartTextOffset + new Vector2(0, 100), textColor, 0, textSize * 0.5f, 1f, SpriteEffects.None, 0);
			}
			else
			{
				Main.spriteBatch.DrawString(FontAssets.MouseText.Value, textStart, MainPanelOriginMinimized + new Vector2(0, 50), textColor, 0, textSize * 0.5f, 1f, SpriteEffects.None, 0);
			}
		}
		else
		{
			string textEnd = "You have finished the max-level-game. Now you can withdraw your bonus.";
			textSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, textEnd, Vector2.One);
			Main.spriteBatch.DrawString(FontAssets.MouseText.Value, textEnd, StartTextOffset + new Vector2(0, 70), textColor, 0, textSize * 0.5f, 1f, SpriteEffects.None, 0);
		}
		if (Failed)
		{
			string textFail = "Fail!";
			textSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, textFail, Vector2.One);
			textColor = new Color(0.3f, 0.4f, 0.3f) * (1 - startAnimationValue);
			Main.spriteBatch.DrawString(FontAssets.MouseText.Value, textFail, StartTextOffset + new Vector2(0, -120), textColor, 0, textSize * 0.5f, 2f, SpriteEffects.None, 0);
		}

		if (Level is > 1 and <= 6)
		{
			// Account Money
			defauleIcon = new Color(0.5f, 0.4f, 0f);
			if (Level == 6)
			{
				defauleIcon = new Color(0.8f, 0.72f, 0f);
			}
			defaulePanel = new Color(0.3f, 0.2f, 0.2f);
			if (IsMouseOverAccount)
			{
				defauleIcon = new Color(0.9f, 0.8f, 0.3f);
				defaulePanel = new Color(0.1f, 0.05f, 0.04f);
			}
			AccountPanelOffset = MainPanelOrigin + new Vector2(80, -startAnimationValue * 80);
			if (Level == 6)
			{
				AccountPanelOffset = MainPanelOrigin + new Vector2(0, -startAnimationValue * 80);
			}
			if (!Maximized)
			{
				AccountPanelOffset = MainPanelOriginMinimized + new Vector2(30, -startAnimationValue * 80);
				if (Level == 6)
				{
					AccountPanelOffset = MainPanelOriginMinimized + new Vector2(0, -startAnimationValue * 80);
				}
			}
			Texture2D Account = ModAsset.WithdrawAccount.Value;
			if (Maximized)
			{
				Draw9Pieces(AccountPanelOffset, 48, 48, defaulePanel, 0);
				Main.spriteBatch.Draw(Account, AccountPanelOffset, null, defauleIcon, 0, Account.Size() * 0.5f, 1f, SpriteEffects.None, 0);
			}
			else
			{
				Draw9Pieces(AccountPanelOffset, 20, 20, defaulePanel, 0);
				Main.spriteBatch.Draw(Account, AccountPanelOffset, null, defauleIcon, 0, Account.Size() * 0.5f, 0.5f, SpriteEffects.None, 0);
			}

			string textAcount = "Total profit : " + TotalScore + "(x" + (Level - 1) + ")";
			textSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, textAcount, Vector2.One);
			Main.spriteBatch.DrawString(FontAssets.MouseText.Value, textAcount, StartTextOffset + new Vector2(0, 130), textColor, 0, textSize * 0.5f, 1f, SpriteEffects.None, 0);

			string textSuccess = "Success!";
			textSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, textSuccess, Vector2.One);
			if (Maximized)
			{
				Main.spriteBatch.DrawString(FontAssets.MouseText.Value, textSuccess, StartTextOffset + new Vector2(0, -110), textColor, 0, textSize * 0.5f, 2f, SpriteEffects.None, 0);
			}
			else
			{
				Main.spriteBatch.DrawString(FontAssets.MouseText.Value, textSuccess, StartTextOffset + new Vector2(0, -110), textColor, 0, textSize * 0.5f, 1.5f, SpriteEffects.None, 0);
			}
			string textStars = string.Empty;
			int startCount = GetStarCount();
			for (int i = 0; i < startCount; i++)
			{
				textStars += "⭐";
			}

			textSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, textStars, Vector2.One);
			if (Maximized)
			{
				Main.spriteBatch.DrawString(FontAssets.MouseText.Value, textStars, StartTextOffset + new Vector2(0, -70), textColor, 0, textSize * 0.5f, 2f, SpriteEffects.None, 0);
			}
			else
			{
				Main.spriteBatch.DrawString(FontAssets.MouseText.Value, textStars, StartTextOffset + new Vector2(0, -70), textColor, 0, textSize * 0.5f, 1.5f, SpriteEffects.None, 0);
			}
		}
	}

	public void Update()
	{
		GetOffsets();
		UpdateFoodRequest();
		UpdateScore();
		PanelCollisonCheck();
		CheckPosValid();
		CheckGameTimer();
	}

	public void CheckGameTimer()
	{
		if (Started)
		{
			Timer--;
			if (Timer <= 0)
			{
				Timer = 0;
				foreach (var foodRequest in FoodRequests)
				{
					foodRequest.KillAtTimeOut();
				}
				if (Score > TargetScore)
				{
					NextLevel();
				}
				else
				{
					Fail();
				}
			}
			if (StartInterfaceAnimationTimer < 30)
			{
				StartInterfaceAnimationTimer++;
			}
			else
			{
				StartInterfaceAnimationTimer = 30;
			}
		}
		else
		{
			if (StartInterfaceAnimationTimer > 0)
			{
				StartInterfaceAnimationTimer--;
			}
			else
			{
				StartInterfaceAnimationTimer = 0;
			}
		}
	}

	public static int GetTargetScore()
	{
		switch (Level)
		{
			case 0:
				return 60;
			case 1:
				return 2500;
			case 2:
				return 5400;
			case 3:
				return 9000;
			case 4:
				return 13400;
			case 5:
				return 17500;
		}

		return -1;
	}

	public void CheckPosValid()
	{
		if (MainPanelOrigin.X > Main.screenWidth - 200)
		{
			MainPanelOrigin.X = Main.screenWidth - 200;
		}
		if (MainPanelOrigin.X < 200)
		{
			MainPanelOrigin.X = 200;
		}
		if (MainPanelOrigin.Y > Main.screenHeight - 100)
		{
			MainPanelOrigin.Y = Main.screenHeight - 100;
		}
		if (MainPanelOrigin.Y < 100)
		{
			MainPanelOrigin.Y = 100;
		}
	}

	public void PanelCollisonCheck()
	{
		// Pause to taking new orders
		Rectangle pauseBox = new Rectangle((int)PauseContinuePanelOffset.X - 20, (int)PauseContinuePanelOffset.Y - 20, 40, 40);
		if (pauseBox.Contains((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y))
		{
			if (!IsMouseOverPauseContinue)
			{
				SoundEngine.PlaySound(SoundID.MenuTick);
			}
			IsMouseOverPauseContinue = true;
			if (Main.mouseLeft && Main.mouseLeftRelease)
			{
				SoundEngine.PlaySound(SoundID.MenuClose);
				Pause = !Pause;
				Started = true;
			}
			string text = "Stop receving orders";
			if (Pause)
			{
				text = "Continue to receve orders";
				if (!Started)
				{
					text = "Start";
				}
			}
			Main.instance.MouseText(text, ItemRarityID.White);
		}
		else
		{
			IsMouseOverPauseContinue = false;
		}

		// Drag the main panel
		Rectangle dragBox = new Rectangle((int)DragPanelOffset.X - 20, (int)DragPanelOffset.Y - 20, 40, 40);
		if (dragBox.Contains((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y))
		{
			if (!IsMouseOverDrag)
			{
				SoundEngine.PlaySound(SoundID.MenuTick);
			}
			IsMouseOverDrag = true;
			if (Main.mouseLeft && !IsDragging)
			{
				SoundEngine.PlaySound(SoundID.MenuClose);
				IsDragging = true;
				DragStartMousePos = Main.MouseScreen;
				if (Maximized)
				{
					DragStartPanelPos = MainPanelOrigin;
				}
				else
				{
					DragStartPanelPos = MainPanelOriginMinimized;
				}
			}
			if (Main.mouseLeftRelease && IsDragging)
			{
				SoundEngine.PlaySound(SoundID.MenuTick);
				IsDragging = false;
			}
			Main.instance.MouseText("Drag", ItemRarityID.White);
		}
		else
		{
			IsMouseOverDrag = false;
		}

		// Restart a new game
		Rectangle restartBox = new Rectangle((int)RestartPanelOffset.X - 20, (int)RestartPanelOffset.Y - 20, 40, 40);
		if (restartBox.Contains((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y))
		{
			if (!IsMouseOverRestart)
			{
				SoundEngine.PlaySound(SoundID.MenuTick);
			}
			IsMouseOverRestart = true;
			if (Main.mouseLeft && Main.mouseLeftRelease)
			{
				SoundEngine.PlaySound(SoundID.MenuClose);
				if (TotalScore * (Level - 1) > 0)
				{
					Item.NewItem(null, Main.LocalPlayer.Center, ItemID.CopperCoin, TotalScore * (Level - 1));
				}

				Restart();
			}
			Main.instance.MouseText("Restart", ItemRarityID.White);
		}
		else
		{
			IsMouseOverRestart = false;
		}

		// Maxmize main panel
		Rectangle maximizeBox = new Rectangle((int)MaximizePanelOffset.X - 20, (int)MaximizePanelOffset.Y - 20, 40, 40);
		if (maximizeBox.Contains((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y))
		{
			if (!IsMouseOverMaximize)
			{
				SoundEngine.PlaySound(SoundID.MenuTick);
			}
			IsMouseOverMaximize = true;
			if (Main.mouseLeft && Main.mouseLeftRelease)
			{
				SoundEngine.PlaySound(SoundID.MenuClose);
				Maximized = !Maximized;
			}
			string text = "Maximize";
			if (Maximized)
			{
				text = "Minimize";
			}
			Main.instance.MouseText(text, ItemRarityID.White);
		}
		else
		{
			IsMouseOverMaximize = false;
		}

		// StartNewGame panel
		if (StartInterfaceAnimationTimer < 30 && Level < 6)
		{
			Rectangle startNewGameBox = new Rectangle((int)StartNewGamePanelOffset.X - 48, (int)StartNewGamePanelOffset.Y - 48, 96, 96);
			if (!Maximized)
			{
				startNewGameBox = new Rectangle((int)StartNewGamePanelOffset.X - 20, (int)StartNewGamePanelOffset.Y - 20, 40, 40);
			}
			if (startNewGameBox.Contains((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y))
			{
				if (!IsMouseOverStartNewGame)
				{
					SoundEngine.PlaySound(SoundID.MenuTick);
				}
				IsMouseOverStartNewGame = true;
				if (Main.mouseLeft && Main.mouseLeftRelease)
				{
					SoundEngine.PlaySound(SoundID.MenuClose);
					Started = true;
					Pause = false;
					Failed = false;
				}
				Main.instance.MouseText("Start", ItemRarityID.White);
			}
			else
			{
				IsMouseOverStartNewGame = false;
			}
		}

		// Account panel
		if (StartInterfaceAnimationTimer < 30 && Level >= 2)
		{
			Rectangle accountBox = new Rectangle((int)AccountPanelOffset.X - 48, (int)AccountPanelOffset.Y - 48, 96, 96);
			if (!Maximized)
			{
				accountBox = new Rectangle((int)AccountPanelOffset.X - 20, (int)AccountPanelOffset.Y - 20, 40, 40);
			}
			if (accountBox.Contains((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y))
			{
				if (!IsMouseOverAccount)
				{
					SoundEngine.PlaySound(SoundID.MenuTick);
				}
				IsMouseOverAccount = true;
				if (Main.mouseLeft && Main.mouseLeftRelease)
				{
					SoundEngine.PlaySound(SoundID.MenuClose);
					Item.NewItem(null, Main.LocalPlayer.Center, ItemID.CopperCoin, TotalScore * (Level - 1));
					Restart();
				}
				Main.instance.MouseText("Account", ItemRarityID.White);
			}
			else
			{
				IsMouseOverAccount = false;
			}
		}
	}

	public void GetOffsets()
	{
		if (IsDragging)
		{
			if (Maximized)
			{
				MainPanelOrigin = DragStartPanelPos + Main.MouseScreen - DragStartMousePos;
			}
			else
			{
				MainPanelOriginMinimized.X = (DragStartPanelPos + Main.MouseScreen - DragStartMousePos).X;
			}
		}

		MainPanelOriginMinimized.Y = Main.screenHeight - 100;

		PauseContinuePanelOffset = MainPanelOrigin + new Vector2(340, 200);
		DragPanelOffset = MainPanelOrigin + new Vector2(280, 200);
		RestartPanelOffset = MainPanelOrigin + new Vector2(220, 200);
		MaximizePanelOffset = MainPanelOrigin + new Vector2(160, 200);
		if (!Maximized)
		{
			PauseContinuePanelOffset = MainPanelOriginMinimized + new Vector2(200, -100);
			DragPanelOffset = MainPanelOriginMinimized + new Vector2(160, -100);
			RestartPanelOffset = MainPanelOriginMinimized + new Vector2(120, -100);
			MaximizePanelOffset = MainPanelOriginMinimized + new Vector2(80, -100);
		}
	}

	public void DrawScoreBar()
	{
		if (Maximized)
		{
			Draw9Pieces(MainPanelOrigin + new Vector2(0, 220), 400, 60, new Color(0.3f, 0.2f, 0.2f), 0);

			// Value display
			Main.spriteBatch.DrawString(FontAssets.MouseText.Value, "Profit: " + Score.ToString(), MainPanelOrigin + new Vector2(-370, 180), new Color(1f, 1f, 0.5f));
			Main.spriteBatch.DrawString(FontAssets.MouseText.Value, "Target Profit: " + TargetScore.ToString(), MainPanelOrigin + new Vector2(-370, 210), new Color(1f, 1f, 0.5f));
			if (Timer < 1200 && Timer != 0)
			{
				Main.spriteBatch.DrawString(FontAssets.MouseText.Value, "Relest Time: " + (int)(Timer / 60f), MainPanelOrigin + new Vector2(-370, 240), Color.Lerp(new Color(1f, 1f, 0.5f), new Color(1f, 0f, 0f), MathF.Sin((float)Main.timeForVisualEffects * 0.21f) * 0.5f + 0.5f));
			}
			else
			{
				Main.spriteBatch.DrawString(FontAssets.MouseText.Value, "Relest Time: " + (int)(Timer / 60f), MainPanelOrigin + new Vector2(-370, 240), new Color(1f, 1f, 0.5f));
			}

			Main.spriteBatch.DrawString(FontAssets.MouseText.Value, "Level: " + Level, MainPanelOrigin + new Vector2(-150, 210), new Color(1f, 1f, 0.5f));
		}
		else
		{
			string text = Score + " / " + TargetScore;
			Vector2 textSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, text, Vector2.One);
			Main.spriteBatch.DrawString(FontAssets.MouseText.Value, text, MainPanelOriginMinimized + new Vector2(0, 70), new Color(1f, 1f, 0.5f), 0, textSize * 0.5f, 1f, SpriteEffects.None, 0);

			Draw9Pieces(MainPanelOriginMinimized + new Vector2(0, 50), 246, 8, new Color(0.3f, 0.3f, 0.6f), 0);
			Draw9Pieces(MainPanelOriginMinimized + new Vector2(0, 50), 244, 6, new Color(0f, 0f, 0f), 0);
			float timeValue = Timer / (float)MaxTime;

			Color cTime = Color.Lerp(new Color(1f, 0f, 0f), new Color(0f, 1f, 0f), timeValue) * 0.9f * (StartInterfaceAnimationTimer / 30f);
			Draw9Pieces(MainPanelOriginMinimized + new Vector2(-244 * (1 - timeValue), 50), 244 * timeValue, 6, cTime, 0);
		}
	}

	public void UpdateFoodRequest()
	{
		// New order
		if (!Started)
		{
			return;
		}
		if (!Pause && FoodRequests.Count < 5 && Main.rand.NextBool(30))
		{
			Vector3 value = GetRandomFoodType();
			if (value.Y < Timer + 1700)
			{
				FoodRequestPanel foodRequestPanel = new FoodRequestPanel((int)value.X, (int)value.Y, (int)value.Z);
				foodRequestPanel.AnchorPos = new Vector2(310, 0);
				FoodRequests.Add(foodRequestPanel);
			}
		}
		for (int index = FoodRequests.Count - 1; index >= 0; index--)
		{
			FoodRequestPanel foodRequest = FoodRequests[index];
			foodRequest.Update(index);
			if (!foodRequest.Active && foodRequest.Alpha >= 1f)
			{
				FoodRequests.Remove(foodRequest);
			}
		}
		FoodRequests.Sort((panel1, panel2) => panel1.TimeLeft.CompareTo(panel2.TimeLeft));

		// Iterate through the sorted list and execute CheckFinish() for each panel
		foreach (var panel in FoodRequests)
		{
			panel.CheckFinish();
		}
	}

	public int GetStarCount()
	{
		if (TotalScore >= OldTargetScore && TotalScore < OldTargetScore * 1.5)
		{
			return 1;
		}
		if (TotalScore >= OldTargetScore * 1.5 && TotalScore < OldTargetScore * 2.1)
		{
			return 2;
		}
		if (TotalScore >= OldTargetScore * 2.1 && TotalScore < OldTargetScore * 3.3)
		{
			return 3;
		}
		if (TotalScore >= OldTargetScore * 3.3 && TotalScore < OldTargetScore * 6)
		{
			return 4;
		}
		if (TotalScore >= OldTargetScore * 6 && TotalScore < OldTargetScore * 18)
		{
			return 5;
		}
		if (TotalScore >= OldTargetScore * 18 && TotalScore < OldTargetScore * 72)
		{
			return 6;
		}
		if (TotalScore >= OldTargetScore * 72)
		{
			return 7;
		}
		return 0;
	}

	public Vector3 GetRandomFoodType()
	{
		switch (Main.rand.Next(3))
		{
			case 0:
				return new Vector3(ModContent.ItemType<MapoTofu>(), 1800, 150);
			case 1:
				return new Vector3(ModContent.ItemType<YuxiangEggplant>(), 1800, 190);
			case 2:
				return new Vector3(ModContent.ItemType<BoiledBullfrog>(), 1800, 260);
		}
		return new Vector3(0);
	}

	public static void AddScore(int Value)
	{
		Score += Value;
		ScoreChangeList.Add(new ScoreChange(Value));
	}

	public void UpdateScore()
	{
		for (int index = ScoreChangeList.Count - 1; index >= 0; index--)
		{
			ScoreChange scoreChange = ScoreChangeList[index];
			scoreChange.Update();
			if (scoreChange.TimeLeft <= 0f)
			{
				ScoreChangeList.Remove(scoreChange);
			}
		}
	}

	/// <summary>
	/// Width is half width, height as well.
	/// </summary>
	/// <param name="anchorCenter"></param>
	/// <param name="width"></param>
	/// <param name="height"></param>
	/// <param name="color"></param>
	/// <param name="alpha">The fade-out amount; the drawing color is multiplied by one minus this value.</param>
	/// <param name="texture">The nine-piece panel texture, or null to use FoodRequestUIPanel.</param>
	public static void Draw9Pieces(Vector2 anchorCenter, float width, float height, Color color, float alpha, Texture2D texture = default)
	{
		color *= 1 - alpha;
		var bars = new List<Vertex2D>();

		if (width > 10 && height > 10)
		{
			AddRectangleBars(bars, anchorCenter + new Vector2(-width, -height), anchorCenter + new Vector2(-width, -height) + new Vector2(10, 10), new Vector2(0, 0), new Vector2(0.2f), color);
			AddRectangleBars(bars, anchorCenter + new Vector2(-width, -height) + new Vector2(10, 0), anchorCenter + new Vector2(width, -height) + new Vector2(-10, 10), new Vector2(0.5f, 0), new Vector2(0.5f, 0.2f), color);
			AddRectangleBars(bars, anchorCenter + new Vector2(width, -height) + new Vector2(-10, 0), anchorCenter + new Vector2(width, -height) + new Vector2(0, 10), new Vector2(0.8f, 0), new Vector2(1f, 0.2f), color);

			AddRectangleBars(bars, anchorCenter + new Vector2(-width, -height) + new Vector2(0, 10), anchorCenter + new Vector2(-width, height) + new Vector2(10, -10), new Vector2(0, 0.2f), new Vector2(0.2f, 0.8f), color);
			AddRectangleBars(bars, anchorCenter + new Vector2(-width, -height) + new Vector2(10, 10), anchorCenter + new Vector2(width, height) + new Vector2(-10, -10), new Vector2(0.5f, 0.2f), new Vector2(0.5f, 0.8f), color);
			AddRectangleBars(bars, anchorCenter + new Vector2(width, -height) + new Vector2(-10, 10), anchorCenter + new Vector2(width, height) + new Vector2(0, -10), new Vector2(0.8f, 0.2f), new Vector2(1f, 0.8f), color);

			AddRectangleBars(bars, anchorCenter + new Vector2(-width, height) + new Vector2(0, -10), anchorCenter + new Vector2(-width, height) + new Vector2(10, 0), new Vector2(0f, 0.8f), new Vector2(0.2f, 1f), color);
			AddRectangleBars(bars, anchorCenter + new Vector2(-width, height) + new Vector2(10, -10), anchorCenter + new Vector2(width, height) + new Vector2(-10, 0), new Vector2(0.5f, 0.8f), new Vector2(0.5f, 1f), color);
			AddRectangleBars(bars, anchorCenter + new Vector2(width, height) + new Vector2(-10, -10), anchorCenter + new Vector2(width, height) + new Vector2(0, 0), new Vector2(0.8f, 0.8f), new Vector2(1f, 1f), color);
		}
		else if (width < 10 && height > 10)
		{
			AddRectangleBars(bars, anchorCenter + new Vector2(-width, -height), anchorCenter + new Vector2(-width, -height) + new Vector2(width, 10), new Vector2(0, 0), new Vector2(0.2f), color);
			AddRectangleBars(bars, anchorCenter + new Vector2(width, -height) + new Vector2(-width, 0), anchorCenter + new Vector2(width, -height) + new Vector2(0, 10), new Vector2(0.8f, 0), new Vector2(1f, 0.2f), color);

			AddRectangleBars(bars, anchorCenter + new Vector2(-width, -height) + new Vector2(0, 10), anchorCenter + new Vector2(-width, height) + new Vector2(width, -10), new Vector2(0, 0.4f), new Vector2(0.2f, 0.6f), color);
			AddRectangleBars(bars, anchorCenter + new Vector2(width, -height) + new Vector2(-width, 10), anchorCenter + new Vector2(width, height) + new Vector2(0, -10), new Vector2(0.8f, 0.2f), new Vector2(1f, 0.8f), color);

			AddRectangleBars(bars, anchorCenter + new Vector2(-width, height) + new Vector2(0, -10), anchorCenter + new Vector2(-width, height) + new Vector2(width, 0), new Vector2(0f, 0.8f), new Vector2(0.2f, 1f), color);
			AddRectangleBars(bars, anchorCenter + new Vector2(width, height) + new Vector2(-width, -10), anchorCenter + new Vector2(width, height) + new Vector2(0, 0), new Vector2(0.8f, 0.8f), new Vector2(1f, 1f), color);
		}
		else if (width > 10 && height < 10)
		{
			AddRectangleBars(bars, anchorCenter + new Vector2(-width, -height), anchorCenter + new Vector2(-width, -height) + new Vector2(10, height), new Vector2(0, 0), new Vector2(0.2f), color);
			AddRectangleBars(bars, anchorCenter + new Vector2(-width, -height) + new Vector2(10, 0), anchorCenter + new Vector2(width, -height) + new Vector2(-10, height), new Vector2(0.5f, 0), new Vector2(0.5f, 0.2f), color);
			AddRectangleBars(bars, anchorCenter + new Vector2(width, -height) + new Vector2(-10, 0), anchorCenter + new Vector2(width, -height) + new Vector2(0, height), new Vector2(0.8f, 0), new Vector2(1f, 0.2f), color);

			AddRectangleBars(bars, anchorCenter + new Vector2(-width, height) + new Vector2(0, -height), anchorCenter + new Vector2(-width, height) + new Vector2(10, 0), new Vector2(0f, 0.8f), new Vector2(0.2f, 1f), color);
			AddRectangleBars(bars, anchorCenter + new Vector2(-width, height) + new Vector2(10, -height), anchorCenter + new Vector2(width, height) + new Vector2(-10, 0), new Vector2(0.5f, 0.8f), new Vector2(0.5f, 1f), color);
			AddRectangleBars(bars, anchorCenter + new Vector2(width, height) + new Vector2(-10, -height), anchorCenter + new Vector2(width, height) + new Vector2(0, 0), new Vector2(0.8f, 0.8f), new Vector2(1f, 1f), color);
		}
		else
		{
			AddRectangleBars(bars, anchorCenter + new Vector2(-width, -height), anchorCenter + new Vector2(-width, -height) + new Vector2(width, height), new Vector2(0, 0), new Vector2(0.2f), color);
			AddRectangleBars(bars, anchorCenter + new Vector2(width, -height) + new Vector2(-width, 0), anchorCenter + new Vector2(width, -height) + new Vector2(0, height), new Vector2(0.8f, 0), new Vector2(1f, 0.2f), color);

			AddRectangleBars(bars, anchorCenter + new Vector2(-width, height) + new Vector2(0, -height), anchorCenter + new Vector2(-width, height) + new Vector2(width, 0), new Vector2(0f, 0.8f), new Vector2(0.2f, 1f), color);
			AddRectangleBars(bars, anchorCenter + new Vector2(width, height) + new Vector2(-width, -height), anchorCenter + new Vector2(width, height) + new Vector2(0, 0), new Vector2(0.8f, 0.8f), new Vector2(1f, 1f), color);
		}
		if (texture == default)
		{
			texture = ModAsset.FoodRequestUIPanel.Value;
		}
		Main.graphics.graphicsDevice.Textures[0] = texture;
		Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, bars.ToArray(), 0, bars.Count / 3);
	}
}

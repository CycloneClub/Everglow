using Everglow.Commons.DataStructures;
using Terraria.GameInput;
using Terraria.Localization;

namespace Everglow.Example.Items;

/// <summary>复用刺剑调参面板的滑条素材；参数仅影响本客户端的镜墙预览。</summary>
public class ExampleMirrorConfigUI : ModSystem
{
	private const int PanelWidth = 360;
	private const int PanelHeight = 222;
	private const int TrackLeft = 132;
	private const int TrackWidth = 150;
	private const int Idle = -1;
	private const int DragPanel = -2;

	private Vector2 panelPosition;
	private Vector2 dragOffset;
	private bool positionInitialized;
	private bool wasMouseDown;
	private int dragging = Idle;

	public float OffsetX { get; private set; } = 12;

	public float OffsetY { get; private set; }

	public float ReflectionScale { get; private set; } = 1;

	public bool FlipHorizontally { get; private set; } = true;

	private static bool IsVisible => !Main.dedServ && !Main.gameMenu && !Main.mapFullscreen
		&& Main.LocalPlayer.active && !Main.LocalPlayer.dead && Main.LocalPlayer.HeldItem.ModItem is ExampleMirrorItem;

	// UpdateUI 中 Main.mouseX / screenWidth 已被 SetZoom_UI 缩放，取原始数据避免二次缩放。
	private static Vector2 MousePosition => Vector2.Transform(
		new Vector2(PlayerInput.MouseInfo.X, PlayerInput.MouseInfo.Y), Matrix.Invert(Main.UIScaleMatrix));

	private Rectangle PanelBounds
	{
		get
		{
			ClampPanelPosition();
			return new Rectangle((int)panelPosition.X, (int)panelPosition.Y, PanelWidth, PanelHeight);
		}
	}

	public bool CapturesMouse => IsVisible && (dragging != Idle || PanelBounds.Contains(MousePosition.ToPoint()));

	public override void UpdateUI(GameTime gameTime)
	{
		if (Main.dedServ)
		{
			return;
		}
		bool mouseDown = Main.mouseLeft;
		if (!IsVisible)
		{
			dragging = Idle;
			wasMouseDown = mouseDown;
			return;
		}

		Rectangle panel = PanelBounds;
		Vector2 mouse = MousePosition;
		if (CapturesMouse)
		{
			Main.LocalPlayer.mouseInterface = true;
		}
		if (mouseDown && !wasMouseDown)
		{
			if (new Rectangle(panel.X, panel.Y, PanelWidth, 34).Contains(mouse.ToPoint()))
			{
				dragging = DragPanel;
				dragOffset = mouse - panelPosition;
			}
			else if (ToggleBounds(panel).Contains(mouse.ToPoint()))
			{
				FlipHorizontally = !FlipHorizontally;
			}
			else
			{
				for (int row = 0; row < 3; row++)
				{
					Rectangle track = TrackBounds(panel, row);
					track.Inflate(6, 8);
					if (track.Contains(mouse.ToPoint()))
					{
						dragging = row;
						break;
					}
				}
			}
		}

		if (mouseDown && dragging == DragPanel)
		{
			panelPosition = mouse - dragOffset;
			ClampPanelPosition();
		}
		else if (mouseDown && dragging >= 0)
		{
			float progress = Math.Clamp((mouse.X - panel.X - TrackLeft) / TrackWidth, 0, 1);
			switch (dragging)
			{
				case 0:
					OffsetX = MathF.Round(MathHelper.Lerp(-100, 100, progress));
					break;
				case 1:
					OffsetY = MathF.Round(MathHelper.Lerp(-30, 30, progress));
					break;
				case 2:
					ReflectionScale = MathF.Round(MathHelper.Lerp(0.5f, 1.5f, progress), 2);
					break;
			}
		}
		if (!mouseDown)
		{
			dragging = Idle;
		}
		wasMouseDown = mouseDown;
	}

	public override void PostDrawInterface(SpriteBatch spriteBatch)
	{
		if (!IsVisible)
		{
			return;
		}
		Rectangle panel = PanelBounds;
		using var scope = (SpriteBatchState.Defered with { TransformMatrix = Main.UIScaleMatrix }).BeginScope(spriteBatch);
		Texture2D pixel = Commons.ModAsset.White.Value;
		spriteBatch.Draw(pixel, panel, new Color(18, 24, 34, 225));
		spriteBatch.Draw(pixel, new Rectangle(panel.X, panel.Y, PanelWidth, 34), new Color(40, 57, 76, 245));
		Utils.DrawBorderString(spriteBatch, Text("Title"), new Vector2(panel.X + 14, panel.Y + 8), Color.White, 0.85f);
		DrawSlider(spriteBatch, panel, 0, Text("OffsetX"), OffsetX, -100, 100, 12, OffsetX.ToString("0"));
		DrawSlider(spriteBatch, panel, 1, Text("OffsetY"), OffsetY, -30, 30, 0, OffsetY.ToString("0"));
		DrawSlider(spriteBatch, panel, 2, Text("Scale"), ReflectionScale, 0.5f, 1.5f, 1, ReflectionScale.ToString("0.00"));

		Rectangle toggle = ToggleBounds(panel);
		Color toggleColor = toggle.Contains(MousePosition.ToPoint()) ? new Color(65, 85, 108) : new Color(38, 51, 67);
		spriteBatch.Draw(pixel, toggle, toggleColor);
		spriteBatch.Draw(pixel, new Rectangle(toggle.X + 8, toggle.Y + 6, 16, 16), Color.Silver);
		spriteBatch.Draw(pixel, new Rectangle(toggle.X + 11, toggle.Y + 9, 10, 10), FlipHorizontally ? Color.LightSkyBlue : new Color(30, 40, 52));
		Utils.DrawBorderString(spriteBatch, Text("Flip"), new Vector2(toggle.X + 34, toggle.Y + 4), Color.White, 0.75f);
		Utils.DrawBorderString(spriteBatch, Text(FlipHorizontally ? "On" : "Off"), new Vector2(toggle.Right - 48, toggle.Y + 4), Color.White, 0.75f);
	}

	private void DrawSlider(SpriteBatch batch, Rectangle panel, int row, string label, float value, float min, float max, float defaultValue, string valueText)
	{
		Rectangle track = TrackBounds(panel, row);
		Texture2D block = Commons.ModAsset.SlideBlock.Value;
		Texture2D defaultCut = Commons.ModAsset.SlideDefaultCut.Value;
		batch.Draw(Commons.ModAsset.SlideTrack_black.Value, track, Color.White);
		float defaultX = track.X + (defaultValue - min) / (max - min) * TrackWidth;
		batch.Draw(defaultCut, new Vector2(defaultX, track.Center.Y), null, Color.SandyBrown, 0, defaultCut.Size() * 0.5f, 1, SpriteEffects.None, 0);
		float blockX = track.X + (value - min) / (max - min) * TrackWidth;
		batch.Draw(block, new Vector2(blockX, track.Center.Y), null, dragging == row ? Color.LightSkyBlue : Color.White,
			0, block.Size() * 0.5f, 1, SpriteEffects.None, 0);
		Utils.DrawBorderString(batch, label, new Vector2(panel.X + 16, track.Y - 7), Color.White, 0.75f);
		Utils.DrawBorderString(batch, valueText, new Vector2(track.Right + 12, track.Y - 7), Color.White, 0.75f);
	}

	private void ClampPanelPosition()
	{
		Vector2 size = Vector2.Transform(PlayerInput.OriginalScreenSize, Matrix.Invert(Main.UIScaleMatrix));
		if (!positionInitialized)
		{
			panelPosition = new Vector2(size.X - PanelWidth - 24, 260);
			positionInitialized = true;
		}
		panelPosition.X = Math.Clamp(panelPosition.X, 8, Math.Max(8, size.X - PanelWidth - 8));
		panelPosition.Y = Math.Clamp(panelPosition.Y, 8, Math.Max(8, size.Y - PanelHeight - 8));
	}

	private static Rectangle TrackBounds(Rectangle panel, int row) => new(panel.X + TrackLeft, panel.Y + 55 + row * 42, TrackWidth, 8);

	private static Rectangle ToggleBounds(Rectangle panel) => new(panel.X + 16, panel.Y + 180, PanelWidth - 32, 28);

	private static string Text(string key) => Language.GetTextValue(ExampleMirrorItem.TextKey + "Panel." + key);
}

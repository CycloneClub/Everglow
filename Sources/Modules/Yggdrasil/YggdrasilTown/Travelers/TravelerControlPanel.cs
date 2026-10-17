using Everglow.Commons.DataStructures;
using Everglow.Commons.Utilities;
using Everglow.Yggdrasil.YggdrasilTown.Items.Tools.Developer;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.Localization;

namespace Everglow.Yggdrasil.YggdrasilTown.Travelers;

/// <summary>Adapted from ExampleStabbingSword_Config: draggable panel and shared slider textures.</summary>
public class TravelerControlPanel : ModSystem
{
	private const int Width = 540;
	private const int Height = 482;
	private const int RowTop = 134;
	private const int RowHeight = 24;
	private static Vector2 position = new(24, 116);
	private static Vector2 dragOffset;
	private static int dragging = -1;
	private static bool hidden;
	private static bool suppressClick;
	private static bool wasLeftDown;

	private static bool Chinese => Language.ActiveCulture.Name.StartsWith("zh", StringComparison.OrdinalIgnoreCase);

	private static string Text(string english, string chinese) => Chinese ? chinese : english;

	private static bool Visible => !Main.dedServ && !Main.gameMenu && !Main.LocalPlayer.dead && !Main.playerInventory
		&& !Main.blockInput && !Main.gamePaused && !hidden
		&& Main.LocalPlayer.HeldItem.type == ModContent.ItemType<VFXPersonItem>();

	private static float FitScale => Math.Min(1f, Math.Min(PlayerInput.RealScreenWidth / Main.UIScale / (Width + 16), PlayerInput.RealScreenHeight / Main.UIScale / (Height + 16)));

	// Main mouse/screen coordinates are temporarily scaled by Terraria's UI/world passes.
	private static Vector2 Mouse => new Vector2(PlayerInput.MouseInfo.X, PlayerInput.MouseInfo.Y) / Main.UIScale / FitScale;

	private static Rectangle Area => new((int)position.X, (int)position.Y, Width, Height);

	public static bool CapturesMouse => (suppressClick && Main.mouseLeft) || (Visible && (dragging != -1 || Area.Contains(Mouse.ToPoint())));

	public static void Toggle()
	{
		hidden = !hidden;
		dragging = -1;
	}

	public override void OnWorldLoad() => Reset();

	public override void OnWorldUnload() => Reset();

	public override void Unload() => Reset();

	private static void Reset()
	{
		hidden = false;
		suppressClick = false;
		wasLeftDown = false;
		dragging = -1;
		position = new Vector2(24, 116);
	}

	private static Rectangle Rect(int x, int y, int width, int height) => new((int)position.X + x, (int)position.Y + y, width, height);

	private static Vector2 KeepOnScreen(Vector2 point) => new(
		Math.Clamp(point.X, 8, Math.Max(8, PlayerInput.RealScreenWidth / Main.UIScale / FitScale - Width - 8)),
		Math.Clamp(point.Y, 8, Math.Max(8, PlayerInput.RealScreenHeight / Main.UIScale / FitScale - Height - 8)));

	public override void UpdateUI(GameTime gameTime)
	{
		bool pressed = Main.mouseLeft && !wasLeftDown;
		wasLeftDown = Main.mouseLeft;
		if (!Main.mouseLeft)
		{
			suppressClick = false;
		}
		if (!Main.hasFocus || !Visible)
		{
			dragging = -1;
			return;
		}
		position = KeepOnScreen(position);
		Vector2 mouse = Mouse;
		if (CapturesMouse)
		{
			Main.LocalPlayer.mouseInterface = true;
		}
		if (!Main.mouseLeft)
		{
			dragging = -1;
			return;
		}
		if (dragging == -2)
		{
			position = KeepOnScreen(mouse - dragOffset);
			return;
		}
		if (dragging >= 0)
		{
			SetSlider(dragging, mouse.X);
			return;
		}
		if (!pressed || !Area.Contains(mouse.ToPoint()))
		{
			return;
		}
		suppressClick = true;
		if (Rect(Width - 32, 6, 24, 24).Contains(mouse.ToPoint()))
		{
			hidden = true;
			return;
		}
		if (Rect(0, 0, Width - 36, 34).Contains(mouse.ToPoint()))
		{
			dragOffset = mouse - position;
			dragging = -2;
			return;
		}
		for (int i = 0; i < 3; i++)
		{
			if (Rect(12 + i * 174, 42, 166, 26).Contains(mouse.ToPoint()))
			{
				if (i == 0)
				{
					TownTravelerSystem.SetMode(TravelerMode.Off);
				}
				else if ((int)TownTravelerSystem.Mode != i)
				{
					TownTravelerSystem.ToggleMode((TravelerMode)i);
				}
				return;
			}
			if (Rect(12 + i * 174, 76, 166, 26).Contains(mouse.ToPoint()))
			{
				TownTravelerSystem.Options.ApplyPreset(i);
				return;
			}
		}
		for (int i = 0; i < TravelerOptions.Definitions.Count; i++)
		{
			if (Rect(300, RowTop + i * RowHeight, 170, RowHeight).Contains(mouse.ToPoint()))
			{
				dragging = i;
				SetSlider(i, mouse.X);
				return;
			}
		}
	}

	private static void SetSlider(int index, float mouseX)
	{
		var definition = TravelerOptions.Definitions[index];
		float progress = Math.Clamp((mouseX - position.X - 310) / 150, 0, 1);
		TownTravelerSystem.Options[definition.Option] = MathHelper.Lerp(definition.Min, definition.Max, progress);
	}

	public override void PostDrawInterface(SpriteBatch spriteBatch)
	{
		if (!Visible)
		{
			return;
		}
		SpriteBatchState state = GraphicsUtils.GetState(spriteBatch).Value;
		spriteBatch.End();
		spriteBatch.Begin(state.SortMode, BlendState.AlphaBlend, SamplerState.PointClamp, state.DepthStencilState,
			RasterizerState.CullNone, state.Effect, Matrix.CreateScale(FitScale) * Main.UIScaleMatrix);
		try
		{
			DrawPanel(spriteBatch);
		}
		finally
		{
			spriteBatch.End();
			spriteBatch.Begin(state);
		}
	}

	private static void Fill(SpriteBatch spriteBatch, Rectangle rect, Color color) => spriteBatch.Draw(TextureAssets.MagicPixel.Value, rect, color);

	private static void Label(SpriteBatch spriteBatch, string value, int x, int y, Color? color = null, float scale = 0.75f) =>
		Utils.DrawBorderString(spriteBatch, value, position + new Vector2(x, y), color ?? Color.White, scale);

	private static void Button(SpriteBatch spriteBatch, string value, Rectangle rect, bool selected = false)
	{
		bool hover = rect.Contains(Mouse.ToPoint());
		Fill(spriteBatch, rect, selected ? new Color(46, 112, 140) : hover ? new Color(65, 76, 95) : new Color(37, 44, 59));
		Utils.DrawBorderString(spriteBatch, value, new Vector2(rect.Center.X, rect.Y + 4), Color.White, 0.7f, 0.5f);
	}

	private static void DrawPanel(SpriteBatch spriteBatch)
	{
		Fill(spriteBatch, Area, new Color(18, 23, 34, 240));
		Fill(spriteBatch, Rect(0, 0, Width, 34), new Color(36, 59, 79));
		Label(spriteBatch, Text("Town Travelers · drag to move", "天穹镇人群 · 拖动标题移动"), 12, 7);
		Button(spriteBatch, "×", Rect(Width - 32, 6, 24, 24));
		string[] modes = [Text("Off / clear", "关闭并清理"), Text("NPC mode", "NPC 模式"), Text("Lightweight", "轻量模式")];
		string[] presets = [Text("Quiet", "安静"), Text("Lively / default", "热闹／默认"), Text("Busy", "繁忙")];
		for (int i = 0; i < 3; i++)
		{
			Button(spriteBatch, modes[i], Rect(12 + i * 174, 42, 166, 26), (int)TownTravelerSystem.Mode == i);
			Button(spriteBatch, presets[i], Rect(12 + i * 174, 76, 166, 26));
		}
		Label(spriteBatch, Text($"Population: {TownTravelerSystem.Population} / {TownTravelerSystem.Options.PopulationLimit(TownTravelerSystem.Mode)} · live settings",
			$"当前人数：{TownTravelerSystem.Population} / {TownTravelerSystem.Options.PopulationLimit(TownTravelerSystem.Mode)} · 修改即时生效"), 12, 110, Color.LightSkyBlue, 0.7f);
		var track = Commons.ModAsset.SlideTrack_black.Value;
		var block = Commons.ModAsset.SlideBlock.Value;
		var defaultCut = Commons.ModAsset.SlideDefaultCut.Value;
		for (int i = 0; i < TravelerOptions.Definitions.Count; i++)
		{
			var definition = TravelerOptions.Definitions[i];
			int y = RowTop + i * RowHeight;
			if (i % 2 == 0)
			{
				Fill(spriteBatch, Rect(8, y, Width - 16, RowHeight), new Color(27, 34, 47));
			}
			Label(spriteBatch, Text(definition.English, definition.Chinese), 14, y + 3, scale: 0.7f);
			Vector2 center = position + new Vector2(385, y + 12);
			spriteBatch.Draw(track, center, null, Color.White, 0, track.Size() * 0.5f, 1f, SpriteEffects.None, 0);
			float value = TownTravelerSystem.Options[definition.Option];
			float x = 310 + (value - definition.Min) / (definition.Max - definition.Min) * 150;
			float defaultX = 310 + (definition.Default - definition.Min) / (definition.Max - definition.Min) * 150;
			spriteBatch.Draw(defaultCut, position + new Vector2(defaultX, y + 12), null, Color.SandyBrown, 0, defaultCut.Size() * 0.5f, 1f, SpriteEffects.None, 0);
			spriteBatch.Draw(block, position + new Vector2(x, y + 12), null, Color.White, 0, block.Size() * 0.5f, 1f, SpriteEffects.None, 0);
			Label(spriteBatch, value.ToString("0.##"), 478, y + 3, Color.LightSkyBlue, 0.7f);
		}
		Label(spriteBatch, Text("Weights: 0 disables source · train arrivals are separate", "来源权重设为 0 可关闭；列车下客独立控制"), 12, 429, Color.Silver, 0.65f);
		Label(spriteBatch, Text("Saved per character · lower caps stop new arrivals", "参数随角色保存；调低上限只暂停补人"), 12, 448, Color.Silver, 0.65f);
	}
}

using Everglow.Commons.DataStructures;
using Terraria.GameInput;
using Terraria.Localization;
using Everglow.Commons.Mechanics.MirrorReflection.Core;

namespace Everglow.Example.AdvancedMirror;

public class AdvancedMirrorConfigUI : ModSystem
{
	private enum Parameter
	{
		OffsetX,
		OffsetY,
		Scale,
		Blur,
		Sigma,
		NormalStrength,
		NormalPeriod,
		Pitch,
		Yaw,
		Roll,
		Distance,
		SurfaceRed,
		SurfaceGreen,
		SurfaceBlue,
		SurfaceOpacity,
		ReflectionOpacity,
	}

	private sealed class Slider(string key, float min, float max, float initial, float step)
	{
		public readonly string Key = key;
		public readonly float Min = min;
		public readonly float Max = max;
		public readonly float Initial = initial;
		public readonly float Step = step;
		public float Value = initial;
	}

	private readonly Slider[] sliders =
	[
		new("OffsetX", -100, 100, 12, 1), new("OffsetY", -30, 30, 0, 1), new("Scale", 0.5f, 1.5f, 1, 0.01f),
		new("Blur", 0, 4, 1, 0.05f), new("Sigma", 0.25f, 3, 1.5f, 0.05f),
		new("NormalStrength", 0, 24, 4, 0.25f), new("NormalPeriod", 32, 256, 96, 1),
		new("Pitch", -60, 60, 0, 1), new("Yaw", -60, 60, 0, 1), new("Roll", -90, 90, 0, 1),
		new("Distance", 240, 1200, 480, 10),
		new("SurfaceRed", 0, 255, 75, 1), new("SurfaceGreen", 0, 255, 95, 1), new("SurfaceBlue", 0, 255, 115, 1),
		new("SurfaceOpacity", 0, 1, 1, 0.01f),
		new("ReflectionOpacity", 0, 1, 0.85f, 0.01f),
	];

	private static readonly MirrorNormalPattern[] NormalPatterns = Enum.GetValues<MirrorNormalPattern>();

	private const int Width = 412;
	private const int TrackLeft = 174;
	private const int TrackWidth = 150;
	private Vector2 position;
	private Vector2 dragOffset;
	private int dragging = -1;
	private int topRow;
	private int lastWheel;
	private bool positioned;
	private bool wasDown;

	public Vector2 Offset => new(Value(Parameter.OffsetX), Value(Parameter.OffsetY));

	public float ReflectionScale => Value(Parameter.Scale);

	public bool FlipHorizontally { get; private set; }

	public float Blur => Value(Parameter.Blur);

	public float Sigma => Value(Parameter.Sigma);

	public MirrorNormalPattern NormalPattern { get; private set; }

	public float NormalStrength => Value(Parameter.NormalStrength);

	public float NormalPeriod => Value(Parameter.NormalPeriod);

	public float Distance => Value(Parameter.Distance);

	public Color SurfaceTint => new((int)Value(Parameter.SurfaceRed), (int)Value(Parameter.SurfaceGreen), (int)Value(Parameter.SurfaceBlue));

	public float SurfaceOpacity => Value(Parameter.SurfaceOpacity);

	public float ReflectionOpacity => Value(Parameter.ReflectionOpacity);

	internal MirrorReflectionDefinition Definition => new(ReflectionOpacity)
	{
		Offset = Offset,
		ReflectionScale = ReflectionScale,
		FlipHorizontally = FlipHorizontally,
		Blur = Blur,
		Sigma = Sigma,
		NormalPattern = NormalPattern,
		NormalStrength = NormalStrength,
		NormalPeriod = NormalPeriod,
		Pitch = Value(Parameter.Pitch),
		Yaw = Value(Parameter.Yaw),
		Roll = Value(Parameter.Roll),
		Distance = Distance,
	};

	private float Value(Parameter parameter) => sliders[(int)parameter].Value;

	private static bool Visible => !Main.dedServ && !Main.gameMenu && !Main.mapFullscreen && Main.LocalPlayer.active
		&& !Main.LocalPlayer.dead && Main.LocalPlayer.HeldItem.ModItem is AdvancedMirrorItem;

	private static Vector2 Mouse => Vector2.Transform(new Vector2(PlayerInput.MouseInfo.X, PlayerInput.MouseInfo.Y), Matrix.Invert(Main.UIScaleMatrix));

	private Rectangle Panel
	{
		get
		{
			Vector2 size = Vector2.Transform(PlayerInput.OriginalScreenSize, Matrix.Invert(Main.UIScaleMatrix));
			int height = (int)Math.Clamp(size.Y - 16, 182, 450);
			if (!positioned)
			{
				position = new Vector2(size.X - Width - 24, 180);
				positioned = true;
			}
			position.X = Math.Clamp(position.X, 8, Math.Max(8, size.X - Width - 8));
			position.Y = Math.Clamp(position.Y, 8, Math.Max(8, size.Y - height - 8));
			return new Rectangle((int)position.X, (int)position.Y, Width, height);
		}
	}

	public bool CapturesMouse => Visible && (dragging != -1 || Panel.Contains(Mouse.ToPoint()));

	private int VisibleRows(Rectangle panel) => Math.Clamp((panel.Height - 136) / 30, 1, sliders.Length);

	private static Rectangle Track(Rectangle panel, int row) => new(panel.X + TrackLeft, panel.Y + 53 + row * 30, TrackWidth, 8);

	private static Rectangle PatternButton(Rectangle panel) => new(panel.X + 12, panel.Bottom - 96, Width - 24, 26);

	private static Rectangle FlipButton(Rectangle panel) => new(panel.X + 12, panel.Bottom - 38, 240, 26);

	private static Rectangle ResetButton(Rectangle panel) => new(panel.Right - 132, panel.Bottom - 38, 120, 26);

	public override void UpdateUI(GameTime gameTime)
	{
		if (Main.dedServ)
		{
			return;
		}
		bool down = Main.mouseLeft;
		int wheel = PlayerInput.MouseInfo.ScrollWheelValue;
		if (!Visible)
		{
			dragging = -1;
			wasDown = down;
			lastWheel = wheel;
			return;
		}
		Rectangle panel = Panel;
		Vector2 mouse = Mouse;
		int count = VisibleRows(panel);
		topRow = Math.Clamp(topRow, 0, sliders.Length - count);
		if (CapturesMouse)
		{
			Main.LocalPlayer.mouseInterface = true;
			PlayerInput.LockVanillaMouseScroll("Everglow: Advanced mirror tuning");
			if (dragging == -1 && wheel != lastWheel)
			{
				topRow = Math.Clamp(topRow - Math.Sign(wheel - lastWheel), 0, sliders.Length - count);
			}
		}
		if (down && !wasDown)
		{
			if (new Rectangle(panel.X, panel.Y, Width, 34).Contains(mouse.ToPoint()))
			{
				dragging = -2;
				dragOffset = mouse - position;
			}
			else if (PatternButton(panel).Contains(mouse.ToPoint()))
			{
				CycleNormalPattern();
			}
			else if (FlipButton(panel).Contains(mouse.ToPoint()))
			{
				FlipHorizontally = !FlipHorizontally;
			}
			else if (ResetButton(panel).Contains(mouse.ToPoint()))
			{
				foreach (Slider slider in sliders)
				{
					slider.Value = slider.Initial;
				}
				FlipHorizontally = false;
				NormalPattern = MirrorNormalPattern.Wave;
			}
			else
			{
				for (int row = 0; row < count; row++)
				{
					Rectangle hit = Track(panel, row);
					hit.Inflate(6, 8);
					if (hit.Contains(mouse.ToPoint()))
					{
						dragging = topRow + row;
						break;
					}
				}
			}
		}
		if (down && dragging == -2)
		{
			position = mouse - dragOffset;
		}
		else if (down && dragging >= 0)
		{
			Slider slider = sliders[dragging];
			float fraction = Math.Clamp((mouse.X - panel.X - TrackLeft) / TrackWidth, 0, 1);
			slider.Value = Math.Clamp(MathF.Round(MathHelper.Lerp(slider.Min, slider.Max, fraction) / slider.Step) * slider.Step, slider.Min, slider.Max);
		}
		if (!down)
		{
			dragging = -1;
		}
		wasDown = down;
		lastWheel = wheel;
	}

	public override void PostDrawInterface(SpriteBatch spriteBatch)
	{
		if (!Visible)
		{
			return;
		}
		Rectangle panel = Panel;
		int count = VisibleRows(panel);
		topRow = Math.Clamp(topRow, 0, sliders.Length - count);
		using var scope = (SpriteBatchState.Defered with { TransformMatrix = Main.UIScaleMatrix }).BeginScope(spriteBatch);
		Texture2D pixel = Commons.ModAsset.White.Value;
		spriteBatch.Draw(pixel, panel, new Color(18, 24, 34, 235));
		spriteBatch.Draw(pixel, new Rectangle(panel.X, panel.Y, Width, 34), new Color(39, 58, 80));
		Utils.DrawBorderString(spriteBatch, Text("Title"), new Vector2(panel.X + 12, panel.Y + 8), Color.White, 0.8f);
		for (int row = 0; row < count; row++)
		{
			Slider slider = sliders[topRow + row];
			Rectangle track = Track(panel, row);
			spriteBatch.Draw(Commons.ModAsset.SlideTrack_black.Value, track, Color.White);
			Texture2D tick = Commons.ModAsset.SlideDefaultCut.Value;
			Texture2D block = Commons.ModAsset.SlideBlock.Value;
			float initialX = track.X + (slider.Initial - slider.Min) / (slider.Max - slider.Min) * TrackWidth;
			float blockX = track.X + (slider.Value - slider.Min) / (slider.Max - slider.Min) * TrackWidth;
			spriteBatch.Draw(tick, new Vector2(initialX, track.Center.Y), null, Color.SandyBrown, 0, tick.Size() / 2, 1, SpriteEffects.None, 0);
			spriteBatch.Draw(block, new Vector2(blockX, track.Center.Y), null, dragging == topRow + row ? Color.LightSkyBlue : Color.White, 0, block.Size() / 2, 1, SpriteEffects.None, 0);
			Utils.DrawBorderString(spriteBatch, Text(slider.Key), new Vector2(panel.X + 12, track.Y - 7), Color.White, 0.7f);
			Utils.DrawBorderString(spriteBatch, slider.Value.ToString(slider.Step < 1 ? "0.00" : "0"), new Vector2(track.Right + 12, track.Y - 7), Color.White, 0.7f);
		}
		Utils.DrawBorderString(spriteBatch, Text("ScrollHint"), new Vector2(panel.X + 12, panel.Bottom - 64), Color.LightGray, 0.65f);
		DrawButton(spriteBatch, PatternButton(panel), Text("NormalPattern") + ": " + Text("Pattern" + NormalPattern));
		DrawButton(spriteBatch, FlipButton(panel), Text("Flip") + ": " + Text(FlipHorizontally ? "On" : "Off"));
		DrawButton(spriteBatch, ResetButton(panel), Text("Reset"));
	}

	private void CycleNormalPattern()
	{
		int index = Array.IndexOf(NormalPatterns, NormalPattern);
		NormalPattern = NormalPatterns[(index + 1) % NormalPatterns.Length];
		// 切换时提供明显的预设效果，之后仍可通过原有滑条调节。
		bool original = NormalPattern == MirrorNormalPattern.Wave;
		sliders[(int)Parameter.NormalStrength].Value = original ? 4 : 20;
		sliders[(int)Parameter.NormalPeriod].Value = original ? 96 : 160;
		sliders[(int)Parameter.Blur].Value = original ? 1 : 0.25f;
	}

	private static void DrawButton(SpriteBatch batch, Rectangle bounds, string label)
	{
		batch.Draw(Commons.ModAsset.White.Value, bounds, bounds.Contains(Mouse.ToPoint()) ? new Color(70, 96, 125) : new Color(42, 59, 78));
		Utils.DrawBorderString(batch, label, new Vector2(bounds.X + 8, bounds.Y + 4), Color.White, 0.7f);
	}

	private static string Text(string key) => Language.GetTextValue(AdvancedMirrorItem.TextKey + "Panel." + key);
}

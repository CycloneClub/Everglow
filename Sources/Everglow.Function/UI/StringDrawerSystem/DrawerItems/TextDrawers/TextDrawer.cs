using Everglow.Commons.Utilities;
using FontStashSharp;

namespace Everglow.Commons.UI.StringDrawerSystem.DrawerItems.TextDrawers;

public class TextDrawer : DrawerItem
{
	private static readonly Dictionary<string, float> _fontOffset = new()
	{
		{ nameof(FontManager.FusionPixel12), -0.1f },
	};

	public string Text = string.Empty;
	public Color Color = Color.White;
	public Vector2 Offset = Vector2.Zero;
	public Vector2 Scale = Vector2.One;
	public Vector2 Origin = Vector2.Zero;
	public float Rotation = 0f;
	public TextStyle TextStyle = TextStyle.None;
	public FontSystemEffect FontSystemEffect = FontSystemEffect.None;
	public float CharacterSpacing = 0f;
	public int EffectAmount = 0;
	public float FontSize = 30f;
	public float LayerDepth = 0f;
	public FontSystem FontSystem;

	public SpriteFontBase Font => FontSystem.GetFont(FontSize);

	public static TextDrawer Create(StringDrawer stringDrawer, string text, int line)
	{
		TextDrawer drawerItem = new TextDrawer();
		StringParameters stringParameters = new StringParameters();
		stringParameters["Text"] = text;
		drawerItem.Init(stringDrawer, text, nameof(TextDrawer), stringParameters);
		drawerItem.Line = line;
		return drawerItem;
	}

	public override string ToString() => $"{Line} {Text}";

	protected virtual Vector2 GetTextSize(string text) =>
		new Vector2(
			Font.MeasureString(text, Scale, CharacterSpacing, 0, FontSystemEffect, EffectAmount).X,
			Font.LineHeight * text.ReplaceLineEndings().Split(Environment.NewLine).Length);

	/// <summary>
	/// Wrap text to fit content with specified width constraint.
	/// </summary>
	/// <param name="index">Drawer item index</param>
	/// <param name="drawerItems">Drawer item list</param>
	/// <param name="line">Line index</param>
	/// <param name="width">Remaining line width</param>
	/// <param name="maxLineWidth">Maximum allowed width for any line</param>
	/// <param name="maxLineCount">Optional maximum number of allowed lines</param>
	/// <returns>Remaining available width in last line after processing</returns>
	public override float WordWrap(ref int index, List<DrawerItem> drawerItems, ref int line, float width, float maxLineWidth, int? maxLineCount = null)
	{
		// Line limit
		if (maxLineCount.HasValue && (index >= maxLineCount || line >= maxLineCount))
		{
			return 0;
		}

		// Handle empty text case
		if (string.IsNullOrEmpty(Text))
		{
			return 0;
		}

		// Ensure there's enough space for at least the first character.
		var firstCharWidth = GetTextSize(Text[0].ToString()).X;
		if (maxLineWidth < firstCharWidth)
		{
			return 0;
		}

		// If the remaining line width is less than the width of first charactor, move to the next line.
		if (width < firstCharWidth)
		{
			width = maxLineWidth;
			Line++;
			line++;
		}

		// =================================== Core Logic ================================ //
		string initialText = Text; // Preserve the original text for processing.
		int currentSegmentStart = 0; // Starting index for the current segment of text being processed.

		int safetyCounter = initialText.Length * 2; // Infinite loop prevention
		while (safetyCounter-- > 0 && currentSegmentStart < initialText.Length)
		{
			// Part 1: Find maximum fitting segment
			// ====================================
			var bestEndIndex = -1;
			for (int i = currentSegmentStart; i < initialText.Length; i++)
			{
				Vector2 segmentSize = GetTextSize(initialText[currentSegmentStart..(i + 1)]);
				if (segmentSize.X < width)
				{
					bestEndIndex = i;
				}
				else if (segmentSize.X > width)
				{
					break;
				}
			}

			if (bestEndIndex == -1)
			{
				return 0;
			}

			// Part 2: Validate split boundaries.
			// ==================================
			if (!TextUtils.IsSplitBoundary(initialText, bestEndIndex))
			{
				var naturalBreakIndex = TextUtils.FindPreviousSplitBoundary(initialText, bestEndIndex);

				// If the draw length of english word is more than max line width, then line break will be enforced.
				bestEndIndex = naturalBreakIndex >= currentSegmentStart
					? naturalBreakIndex // Use natural line break index
					: bestEndIndex; // Force line break
			}

			// Part 3: Content segmentation.
			// =============================
			string segment = initialText[currentSegmentStart..(bestEndIndex + 1)];
			if (currentSegmentStart == 0)
			{
				// Handle primary text segment.
				Text = segment;
				width = maxLineWidth;
			}
			else
			{
				// Create a new drawer item for the splited segments.
				var item = Decomposition(segment, drawerItems, index);
				item.Line = ++line;
				drawerItems.Insert(++index, item);
			}

			// Update processing state.
			currentSegmentStart = bestEndIndex + 1;

			// Completion check
			// ================
			if (currentSegmentStart >= initialText.Length)
			{
				return maxLineWidth - drawerItems[index].GetSize().X;
			}
		}

		return 0; // Fallback return (safety counter exhausted)
	}

	public virtual TextDrawer Decomposition(string text, List<DrawerItem> drawerItems, int index)
	{
		TextDrawer lastDrawer = (TextDrawer)drawerItems[index];
		TextDrawer drawerItem = (TextDrawer)Activator.CreateInstance(GetType());
		drawerItem.Text = text;
		drawerItem.Color = Color;
		drawerItem.Offset = Offset;
		drawerItem.Scale = Scale;
		drawerItem.Origin = Origin;
		drawerItem.Rotation = Rotation;
		drawerItem.TextStyle = TextStyle;
		drawerItem.FontSystemEffect = FontSystemEffect;
		drawerItem.CharacterSpacing = CharacterSpacing;
		drawerItem.EffectAmount = EffectAmount;
		drawerItem.FontSize = FontSize;
		drawerItem.LayerDepth = LayerDepth;
		drawerItem.FontSystem = FontSystem;
		drawerItem.TailLinkItems = new List<DrawerItem>(lastDrawer.TailLinkItems);
		lastDrawer.TailLinkItems.Clear();
		lastDrawer.TailLink(drawerItem);
		drawerItem.HeadLink(lastDrawer);
		return drawerItem;
	}

	public override Vector2 GetSize()
	{
		return GetTextSize(Text);
	}

	public override void Init(StringDrawer stringDrawer, string originalText, string name, StringParameters stringParameters)
	{
		base.Init(stringDrawer, originalText, name, stringParameters);
		if (Permission(stringDrawer, originalText, name, stringParameters) && stringParameters != null)
		{
			Text = stringParameters["Text"];

			Color = stringParameters.GetColor(
				"Color",
				stringDrawer.DefaultParameters.GetColor("Color", Color.White));

			Scale = stringParameters.GetVector2(
				"Scale",
				stringDrawer.DefaultParameters.GetVector2("Scale", Vector2.One));

			Origin = stringParameters.GetVector2(
				"Origin",
				stringDrawer.DefaultParameters.GetVector2("Origin"));

			Rotation = stringParameters.GetFloat(
				"Rotation",
				stringDrawer.DefaultParameters.GetFloat("Rotation"));

			TextStyle = (TextStyle)stringParameters.GetInt(
				"TextStyle",
				stringDrawer.DefaultParameters.GetInt("TextStyle"));

			FontSystemEffect = (FontSystemEffect)stringParameters.GetInt(
				"FontSystemEffect",
				stringDrawer.DefaultParameters.GetInt("FontSystemEffect"));

			CharacterSpacing = stringParameters.GetFloat(
				"CharacterSpacing",
				stringDrawer.DefaultParameters.GetFloat("CharacterSpacing"));

			EffectAmount = stringParameters.GetInt(
				"EffectAmount",
				stringDrawer.DefaultParameters.GetInt("EffectAmount"));

			FontSize = stringParameters.GetFloat(
				"FontSize",
				stringDrawer.DefaultParameters.GetFloat("FontSize", 30f));

			LayerDepth = stringParameters.GetFloat(
				"LayerDepth",
				stringDrawer.DefaultParameters.GetFloat("LayerDepth", 1f));

			var fontName = stringParameters.GetString(
				"Font",
				stringDrawer.DefaultParameters.GetString("Font", nameof(FontManager.FusionPixel12)));

			FontSystem = FontManager.Instance.Fonts[fontName];

			var fontOffset = _fontOffset.TryGetValue(fontName, out float value)
				? new Vector2(0f, GetSize().Y * value)
				: Vector2.Zero;
			Offset = stringParameters.GetVector2(
				"Offset",
				stringDrawer.DefaultParameters.GetVector2("Offset", fontOffset));
		}
		else
		{
			Text = originalText;
			FontSystem = FontManager.FusionPixel12;
		}
	}

	public override TextDrawer GetInstance(StringDrawer stringDrawer, string originalText, string name, StringParameters stringParameters)
	{
		TextDrawer drawerItem = (TextDrawer)Activator.CreateInstance(GetType());
		drawerItem.Init(stringDrawer, originalText, name, stringParameters);
		return drawerItem;
	}

	public override void Draw(SpriteBatch sb)
	{
		sb.DrawString(Font, Text, Position + Offset, Color, Scale, Rotation, Origin, LayerDepth, CharacterSpacing, 0, TextStyle, FontSystemEffect, EffectAmount);
	}
}

using System.Text;
using FontStashSharp;
using Terraria.GameContent;

namespace Everglow.Commons.UI.StringDrawerSystem.DrawerItems.ImageDrawers;

public class ItemDrawer : DrawerItem
{
	public int ItemType;

	public Vector2 ItemBlockSize;

	public string ItemStack;
	public Color ItemStackColor;
	public int ItemStackFontSize;

	public override string ToString()
	{
		return $"{Line} Item:{ItemType}";
	}

	public override void Draw(SpriteBatch sb)
	{
		//sb.Draw(
		//	TextureAssets.MagicPixel.Value,
		//	new Rectangle((int)Position.X, (int)Position.Y,
		//	(int)ItemBlockSize.X, (int)ItemBlockSize.Y), Color.White);

		var tex = TextureAssets.Item[ItemType].Value;
		float scale = Math.Min(ItemBlockSize.X / tex.Width, ItemBlockSize.Y / tex.Height);
		sb.Draw(tex, Position + ItemBlockSize / 2f - tex.Size() * scale / 2f, null,
			Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

		var font = FontManager.FusionPixel12.GetFont(ItemStackFontSize);
		var stackSize = new Vector2(font.MeasureString(ItemStack).X, font.LineHeight);
		sb.DrawString(font, ItemStack, Position + ItemBlockSize * 0.8f - stackSize / 2f, ItemStackColor,
			null, 0, default, 0, 0, 0, TextStyle.None, FontSystemEffect.Stroked, 1);
	}

	public override DrawerItem GetInstance(StringDrawer stringDrawer, string originalText, string name, StringParameters stringParameters)
	{
		ItemDrawer drawerItem = (ItemDrawer)Activator.CreateInstance(GetType());
		drawerItem.Init(stringDrawer, originalText, name, stringParameters);
		return drawerItem;
	}

	public override Vector2 GetSize()
	{
		var font = FontManager.FusionPixel12.GetFont(ItemStackFontSize);
		var stackSize = new Vector2(font.MeasureString(ItemStack).X, font.LineHeight);
		var fs = ItemBlockSize * 0.8f + stackSize / 2f;
		return new Vector2(Math.Max(ItemBlockSize.X, fs.X), Math.Max(ItemBlockSize.Y, fs.Y));
	}

	public override void Init(StringDrawer stringDrawer, string originalText, string name, StringParameters stringParameters)
	{
		base.Init(stringDrawer, originalText, name, stringParameters);
		ItemType = stringParameters.GetInt("Type",
			stringDrawer.DefaultParameters.GetInt("ItemType", 0));
		ItemBlockSize = stringParameters.GetVector2("BlockSize",
			stringDrawer.DefaultParameters.GetVector2("ItemBlockSize", new Vector2(26f)));
		ItemStack = stringParameters.GetString("Stack",
			stringDrawer.DefaultParameters.GetString("ItemStack", string.Empty));
		ItemStackColor = stringParameters.GetColor("StackColor",
			stringDrawer.DefaultParameters.GetColor("ItemStackColor", Color.White));
		ItemStackFontSize = stringParameters.GetInt("StackFontSize",
			stringDrawer.DefaultParameters.GetInt("ItemStackFontSize", 16));
	}

	public override float WordWrap(ref int index, List<DrawerItem> drawerItems, ref int line, float width, float originWidth, int? maxLine = null)
	{
		Line++;
		line++;
		return originWidth - GetSize().X;
	}

	/// <summary>
	/// Create the structured string of <see cref="ItemDrawer"/>
	/// </summary>
	/// <param name="type">Item type</param>
	/// <param name="stack">Item stack</param>
	/// <param name="stackColor">Stack font color</param>
	/// <param name="stackFontSize">Stack font size</param>
	/// <returns>Structured string</returns>
	public static string Create(int type, int? stack = null, Color? stackColor = null, int? stackFontSize = null)
	{
		if(type <= 0)
		{
			return null;
		}

		if(stack.HasValue && stack < 0)
		{
			return null;
		}

		if (stackFontSize.HasValue && stackFontSize <= 0)
		{
			return null;
		}

		var sb = new StringBuilder();
		sb.Append('[');
		sb.Append($"{nameof(ItemDrawer)},Type='{type}'");
		if(stack != null)
		{
			sb.Append($",Stack='{stack}'");
		}
		if(stackColor != null)
		{
			sb.Append($",StackColor='{stackColor.Value.R},{stackColor.Value.G},{stackColor.Value.B},{stackColor.Value.A}'");
		}
		if(stackFontSize != null)
		{
			sb.Append($",StackFontSize='{stackFontSize}'");
		}
		sb.Append(']');
		return sb.ToString();
	}
}

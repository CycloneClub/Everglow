namespace Everglow.Commons.UI.StringDrawerSystem.DrawerItems;

public abstract class DrawerItem : IDrawable
{
	public int Line = 0;
	public Vector2 Position = Vector2.Zero;
	public string ID = string.Empty;
	public string LinkID = string.Empty;
	public string BeLinkID = string.Empty;
	public StringDrawer StringDrawer;
	public List<DrawerItem> HeadLinkItems = new List<DrawerItem>();
	public List<DrawerItem> TailLinkItems = new List<DrawerItem>();

	public virtual void HeadLink(DrawerItem item)
	{
		HeadLinkItems.Add(item);
	}

	public virtual void TailLink(DrawerItem item)
	{
		TailLinkItems.Add(item);
	}

	public virtual void ResetAnimation()
	{
	}

	public virtual void StartAnimation()
	{
		foreach (var i in TailLinkItems)
		{
			i.StartAnimation();
		}
	}

	public virtual void ResetLinkAnimation()
	{
		foreach (var i in TailLinkItems)
		{
			i.ResetLinkAnimation();
		}
		ResetAnimation();
	}

	public abstract float WordWrap(ref int index, List<DrawerItem> drawerItems, ref int line, float width, float maxLineWidth, int? maxLineCount = null);

	public virtual void PostCalculationLineSize(int line, ref Vector2 size)
	{
	}

	public virtual void OnCalculationLineSize(ref Vector2 size)
	{
		Vector2 iSize = GetSize();
		size.X += iSize.X;
		size.Y = Math.Max(size.Y, iSize.Y);
	}

	public virtual void ProcessingLineHeight(ref float height, Vector2 lineSize)
	{
	}

	public virtual void SetPosition(float height, Vector2 lineSize, ref Vector2 pos)
	{
		var iSize = GetSize();
		Position = pos;
		Position.Y += height + (lineSize.Y - iSize.Y) / 2f;
		pos.X += iSize.X;
	}

	public abstract Vector2 GetSize();

	public virtual void Init(StringDrawer stringDrawer, string originalText, string name, StringParameters stringParameters)
	{
		ID = stringParameters["ID"];
		LinkID = stringParameters["LinkID"];
		BeLinkID = stringParameters["BeLinkID"];
		StringDrawer = stringDrawer;
		if (!string.IsNullOrEmpty(LinkID) || !string.IsNullOrEmpty(BeLinkID))
		{
			stringDrawer.PreDrawerItemAdded += di =>
			{
				if (di != this)
				{
					if (!string.IsNullOrEmpty(LinkID) && LinkID == di.ID)
					{
						TailLink(di);
						di.HeadLink(this);
					}
					if (!string.IsNullOrEmpty(BeLinkID) && BeLinkID == di.ID)
					{
						HeadLink(di);
						di.TailLink(this);
					}
				}
			};
		}
	}

	public abstract DrawerItem GetInstance(StringDrawer stringDrawer, string originalText, string name, StringParameters stringParameters);

	public virtual bool Permission(StringDrawer stringDrawer, string originalText, string name, StringParameters stringParameters)
	{
		return name == GetType().Name;
	}

	public abstract void Draw(SpriteBatch sb);
}

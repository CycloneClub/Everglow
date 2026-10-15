using Everglow.Commons.UI.UIElements;

namespace Everglow.Commons.UI.UIContainers.Sidebar;

public class SidebarList : BaseElement
{
	private float elementSpacing = 6f;
	private List<SidebarUIElement> elements = [];

	public override bool Register(BaseElement element)
	{
		if (element is SidebarUIElement sideUIElement)
		{
			element.Info.Width.SetValue(0f, 1f);

			var op = base.Register(element);
			if (!op)
			{
				return op;
			}

			elements.Add(sideUIElement);
			Calculation();
			return op;
		}
		else
		{
			return false;
		}
	}

	public override bool Remove(BaseElement element)
	{
		if (element is not SidebarUIElement)
		{
			return false;
		}

		var op = elements.Remove((SidebarUIElement)element) && base.Remove(element);
		Calculation();
		return op;
	}

	public override void RemoveAll()
	{
		base.RemoveAll();
		elements.Clear();
		Calculation();
	}

	/// <summary>
	/// 计算所有快捷按钮元素的中心位置
	/// </summary>
	public void CalculationQuickElementCenter()
	{
		for (int i = 0; i < elements.Count; i++)
		{
			SidebarUIElement element = elements[i];
			element.Center = Info.Location + new Vector2(Info.HitBox.Width / 2f, 5f + (2 * i + 1) * (4f + Info.HitBox.Width / 2f));
		}
	}

	public void SwapCenter(int index, int index1)
	{
		if (index == -1 || index1 == -1 || index >= elements.Count || index1 >= elements.Count)
		{
			return;
		}

		SidebarUIElement element = elements[index], element1 = elements[index1];
		elements.RemoveAt(index);
		elements.Insert(index1, element);
		elements.RemoveAt(index1 + (index < index1 ? -1 : 1));
		elements.Insert(index, element1);
	}

	public override void Update(GameTime gt)
	{
		base.Update(gt);
		CalculationQuickElementCenter();
		SidebarUIElement movingElement = null;
		int index = -1;
		for (int i = 0; i < elements.Count; i++)
		{
			elements[i].UpdateInfo();

			if (elements[i].IsMoveing)
			{
				movingElement = elements[i];
				index = i;
				break;
			}
		}
		if (movingElement == null || index < 0)
		{
			return;
		}
		if (index > 0 && movingElement.Info.Location.Y <
			(elements[index - 1].Center.Y + elements[index - 1].Info.Size.Y / 2f + elementSpacing / 2f))
		{
			SwapCenter(index, index - 1);
		}
		if (index < (elements.Count - 1) && movingElement.Info.Location.Y >
			(elements[index + 1].Center.Y - elements[index + 1].Info.Size.Y / 2f - elementSpacing / 2f))
		{
			SwapCenter(index, index + 1);
		}
	}

	public override void Calculation()
	{
		Info.Height.SetValue(elements.Where(e => e.Info.IsVisible).Count() * (Info.Size.X + elementSpacing) + 14f, 0f);

		float left, top;
		if (ParentElement == null)
		{
			Info.TotalLocation.X = Info.Left.GetPixelBaseParent(Main.screenWidth);
			Info.TotalLocation.Y = Info.Top.GetPixelBaseParent(Main.screenHeight);
			Info.TotalSize.X = Info.Width.GetPixelBaseParent(Main.screenWidth);
			Info.TotalSize.Y = Info.Height.GetPixelBaseParent(Main.screenHeight);

			left = Info.LeftMargin.GetPixelBaseParent(Main.screenWidth);
			top = Info.TopMargin.GetPixelBaseParent(Main.screenHeight);
			Info.Location.X = Info.TotalLocation.X + left;
			Info.Location.Y = Info.TotalLocation.Y + top;
			Info.Size.X = Info.TotalSize.X - Info.RightMargin.GetPixelBaseParent(Main.screenWidth) - left;
			Info.Size.Y = Info.TotalSize.Y - Info.BottomMargin.GetPixelBaseParent(Main.screenHeight) - top;
		}
		else
		{
			Info.TotalLocation.X = ParentElement.Info.Location.X + Info.Left.GetPixelBaseParent(ParentElement.Info.Size.X);
			Info.TotalLocation.Y = ParentElement.Info.Location.Y + Info.Top.GetPixelBaseParent(ParentElement.Info.Size.Y);
			Info.TotalSize.X = Info.Width.GetPixelBaseParent(ParentElement.Info.Size.X);
			Info.TotalSize.Y = Info.Height.GetPixelBaseParent(ParentElement.Info.Size.Y);

			left = Info.LeftMargin.GetPixelBaseParent(ParentElement.Info.Size.X);
			top = Info.TopMargin.GetPixelBaseParent(ParentElement.Info.Size.Y);
			Info.Location.X = Info.TotalLocation.X + left;
			Info.Location.Y = Info.TotalLocation.Y + top;
			Info.Size.X = Info.TotalSize.X - Info.RightMargin.GetPixelBaseParent(ParentElement.Info.Size.X) - left;
			Info.Size.Y = Info.TotalSize.Y - Info.BottomMargin.GetPixelBaseParent(ParentElement.Info.Size.Y) - top;
		}
		Info.HitBox = new Rectangle((int)Info.Location.X, (int)Info.Location.Y, (int)Info.Size.X, (int)Info.Size.Y);
		Info.TotalHitBox = new Rectangle((int)Info.TotalLocation.X, (int)Info.TotalLocation.Y, (int)Info.TotalSize.X, (int)Info.TotalSize.Y);
		Info.InitDone = true;
		Events.Calculation(this);
	}
}

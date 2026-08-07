namespace Everglow.Commons.UI.UIElements;

public class BaseElement : IDrawable
{
	/// <summary>
	/// 表示位置信息
	/// </summary>
	public struct PositionStyle
	{
		public static readonly PositionStyle Empty = CreatePositionStyle();
		public static readonly PositionStyle Full = new PositionStyle(0f, 1f);
		public static readonly PositionStyle Half = new PositionStyle(0f, 0.5f);

		/// <summary>
		/// 绝对距离，单位为像素
		/// </summary>
		public float Pixel;

		/// <summary>
		/// 相对距离
		/// </summary>
		public float Percent;

		public static PositionStyle CreatePositionStyle()
		{
			PositionStyle style = default(PositionStyle);
			style.Pixel = 0f;
			style.Percent = 0f;
			return style;
		}

		public PositionStyle(float pixel)
		{
			Pixel = pixel;
			Percent = 0f;
		}

		public PositionStyle(float pixel, float percent)
		{
			Pixel = pixel;
			Percent = percent;
		}

		public void SetEmpty()
		{
			Pixel = 0f;
			Percent = 0f;
		}

		public void SetFull()
		{
			Pixel = 0f;
			Percent = 1f;
		}

		public void SetHalf()
		{
			Pixel = 0f;
			Percent = 0.5f;
		}

		/// <summary>
		/// 获取基于输入参数的精确坐标
		/// </summary>
		/// <param name="pixel">输入参数</param>
		/// <returns></returns>
		public float GetPixelBaseParent(float pixel)
		{
			return Percent * pixel + Pixel;
		}

		public void SetValue(float pixel)
		{
			Pixel = pixel;
		}

		public void SetValue(float pixel, float percent)
		{
			Pixel = pixel;
			Percent = percent;
		}

		public void SetValue(PositionStyle style)
		{
			Pixel = style.Pixel;
			Percent = style.Percent;
		}

		public static PositionStyle Lerp(PositionStyle ps1, PositionStyle ps2, float value)
		{
			return ps1 + (ps2 - ps1) * value;
		}

		public static PositionStyle operator +(PositionStyle ps1, PositionStyle ps2)
		{
			var output = ps1;
			output.Pixel += ps2.Pixel;
			output.Percent += ps2.Percent;
			return output;
		}

		public static PositionStyle operator -(PositionStyle ps1, PositionStyle ps2)
		{
			var output = ps1;
			output.Pixel -= ps2.Pixel;
			output.Percent -= ps2.Percent;
			return output;
		}

		public static PositionStyle operator *(PositionStyle ps1, float ps2)
		{
			var output = ps1;
			output.Pixel *= ps2;
			output.Percent *= ps2;
			return output;
		}

		public static PositionStyle operator /(PositionStyle ps1, float ps2)
		{
			var output = ps1;
			output.Pixel /= ps2;
			output.Percent /= ps2;
			return output;
		}

		public static implicit operator PositionStyle((float pixel, float percent) value)
		{
			return new PositionStyle(value.pixel, value.percent);
		}

		public override string ToString()
		{
			return $"Pixel:{Pixel} Percent:{Percent}";
		}
	}

	/// <summary>
	/// 储存位置、大小等信息
	/// </summary>
	public struct ElementInfo
	{
		/// <summary>
		/// 左坐标
		/// </summary>
		public PositionStyle Left;

		/// <summary>
		/// 上坐标
		/// </summary>
		public PositionStyle Top;

		/// <summary>
		/// 宽度
		/// </summary>
		public PositionStyle Width;

		/// <summary>
		/// 高度
		/// </summary>
		public PositionStyle Height;

		/// <summary>
		/// 左边距
		/// </summary>
		public PositionStyle LeftMargin;

		/// <summary>
		/// 右边距
		/// </summary>
		public PositionStyle RightMargin;

		/// <summary>
		/// 上边距
		/// </summary>
		public PositionStyle TopMargin;

		/// <summary>
		/// 下边距
		/// </summary>
		public PositionStyle BottomMargin;

		public SamplerState SamplerState;

		/// <summary>
		/// 是否隐藏溢出
		/// </summary>
		public bool HiddenOverflow;

		/// <summary>
		/// UI部件是否激活
		/// </summary>
		public bool IsVisible;

		/// <summary>
		/// UI部件是否隐藏（不调用 DrawSelf 方法）
		/// </summary>
		public bool IsHidden;

		/// <summary>
		/// 是否敏感（触发子元素事件时同时触发此元素事件）
		/// </summary>
		public bool IsSensitive;

		/// <summary>
		/// 是否可以被交互
		/// </summary>
		public bool CanBeInteract;

		/// <summary>
		/// 指示实际坐标与实际大小是否已经经过计算
		/// </summary>
		public bool InitDone;

		/// <summary>
		/// UI事件遮罩，在开启时不再处理被次UI部件遮挡的UI部件的鼠标事件
		/// </summary>
		public bool InteractiveMask;

		/// <summary>
		/// 实际坐标（被内边距裁切过）
		/// </summary>
		public Vector2 Location;

		/// <summary>
		/// 实际大小（被内边距裁切过）
		/// </summary>
		public Vector2 Size;

		/// <summary>
		/// 实际坐标（无内边距裁切）
		/// </summary>
		public Vector2 TotalLocation;

		/// <summary>
		/// 实际大小（无内边距裁切）
		/// </summary>
		public Vector2 TotalSize;

		/// <summary>
		/// 碰撞箱（被内边距裁切过）
		/// </summary>
		public Rectangle HitBox;

		/// <summary>
		/// 完整碰撞箱（无内边距裁切）
		/// </summary>
		public Rectangle TotalHitBox;

		public static ElementInfo CreateElementInfo()
		{
			ElementInfo info = default(ElementInfo);
			info.Left = PositionStyle.Empty;
			info.Top = PositionStyle.Empty;
			info.Width = PositionStyle.Empty;
			info.Height = PositionStyle.Empty;
			info.LeftMargin = PositionStyle.Empty;
			info.RightMargin = PositionStyle.Empty;
			info.TopMargin = PositionStyle.Empty;
			info.BottomMargin = PositionStyle.Empty;
			info.HiddenOverflow = false;
			info.IsVisible = true;
			info.IsHidden = false;
			info.IsSensitive = false;
			info.CanBeInteract = true;
			info.InitDone = false;
			info.InteractiveMask = false;
			info.Location = Vector2.Zero;
			info.Size = Vector2.Zero;
			info.TotalLocation = Vector2.Zero;
			info.TotalSize = Vector2.Zero;
			info.HitBox = Rectangle.Empty;
			info.TotalHitBox = Rectangle.Empty;
			info.SamplerState = SamplerState.PointWrap;
			return info;
		}

		/// <summary>
		/// 设置内边距
		/// </summary>
		/// <param name="pixel">以像素为单位的内边距</param>
		public void SetMargin(float pixel)
		{
			LeftMargin.Pixel = pixel;
			RightMargin.Pixel = pixel;
			TopMargin.Pixel = pixel;
			BottomMargin.Pixel = pixel;
		}

		public void SetToCenter() => SetCenter(PositionStyle.Half, PositionStyle.Half);

		public void SetCenter(PositionStyle centerX, PositionStyle centerY)
		{
			Left.SetValue(centerX - Width / 2f);
			Top.SetValue(centerY - Height / 2f);
		}

		public void SetToRight() => SetRight(PositionStyle.Full);

		public void SetRight(PositionStyle right)
		{
			Left.SetValue(right - Width);
		}

		public void SetToBottom() => SetBottom(PositionStyle.Full);

		public void SetBottom(PositionStyle bottom)
		{
			Top.SetValue(bottom - Height);
		}

		public void SetScale(float scale)
		{
			Width *= scale;
			Height *= scale;
		}
	}

	/// <summary>
	/// 储存事件
	/// </summary>
	public class ElementEvents
	{
		/// <summary>
		/// 被鼠标点击的委托
		/// </summary>
		public delegate void UIMouseEvent(BaseElement baseElement);

		public delegate void UIUpdateEvent(BaseElement baseElement, GameTime gt);

		public delegate bool UIElementEvent(BaseElement baseElement);

		/// <summary>
		/// 左键点击UI的事件（按下时触发）
		/// </summary>
		public event UIMouseEvent OnLeftClick;

		/// <summary>
		/// 右键点击UI的事件（按下时触发）
		/// </summary>
		public event UIMouseEvent OnRightClick;

		/// <summary>
		/// 左键双击UI的事件
		/// </summary>
		public event UIMouseEvent OnLeftDoubleClick;

		/// <summary>
		/// 右键双击UI的事件
		/// </summary>
		public event UIMouseEvent OnRightDoubleClick;

		/// <summary>
		/// 鼠标左键按下的事件
		/// </summary>
		public event UIMouseEvent OnLeftDown;

		/// <summary>
		/// 鼠标左键抬起的事件
		/// </summary>
		public event UIMouseEvent OnLeftUp;

		/// <summary>
		/// 鼠标右键按下的事件
		/// </summary>
		public event UIMouseEvent OnRightDown;

		/// <summary>
		/// 鼠标右键抬起的事件
		/// </summary>
		public event UIMouseEvent OnRightUp;

		/// <summary>
		/// 鼠标进入UI时的事件
		/// </summary>
		public event UIMouseEvent OnMouseOver;

		/// <summary>
		/// 鼠标离开UI时的事件
		/// </summary>
		public event UIMouseEvent OnMouseOut;

		/// <summary>
		/// 鼠标在UI上时的事件
		/// </summary>
		public event UIMouseEvent OnMouseHover;

		/// <summary>
		/// 更新时的事件
		/// </summary>
		public event UIUpdateEvent OnUpdate;

		/// <summary>
		/// 计算部件具体信息的事件
		/// </summary>
		public event UIElementEvent OnCalculation;

		public void LeftClick(BaseElement element) => OnLeftClick?.Invoke(element);

		public void RightClick(BaseElement element) => OnRightClick?.Invoke(element);

		public void LeftDoubleClick(BaseElement element) => OnLeftDoubleClick?.Invoke(element);

		public void RightDoubleClick(BaseElement element) => OnRightDoubleClick?.Invoke(element);

		public void LeftDown(BaseElement element) => OnLeftDown?.Invoke(element);

		public void LeftUp(BaseElement element) => OnLeftUp?.Invoke(element);

		public void RightDown(BaseElement element) => OnRightDown?.Invoke(element);

		public void RightUp(BaseElement element) => OnRightUp?.Invoke(element);

		public void MouseOver(BaseElement element) => OnMouseOver?.Invoke(element);

		public void MouseOut(BaseElement element) => OnMouseOut?.Invoke(element);

		public void MouseHover(BaseElement element) => OnMouseHover?.Invoke(element);

		public void Update(BaseElement element, GameTime gt) => OnUpdate?.Invoke(element, gt);

		public bool Calculation(BaseElement element) => OnCalculation?.Invoke(element) ?? false;
	}

	/// <summary>
	/// 事件管理
	/// </summary>
	private ElementEvents events;

	/// <summary>
	/// UI信息
	/// </summary>
	public ElementInfo Info;

	/// <summary>
	/// 上一级UI部件
	/// </summary>
	public BaseElement ParentElement { get; private set; }

	/// <summary>
	/// 下一级UI部件
	/// </summary>
	public List<BaseElement> ChildrenElements { get; private set; }

	/// <summary>
	/// 指示此UI部件是否激活
	/// </summary>
	public virtual bool IsVisible { get => Info.IsVisible; }

	/// <summary>
	/// 溢出隐藏的裁切矩形
	/// </summary>
	public virtual Rectangle HiddenOverflowRectangle
	{
		get
		{
			return new Rectangle(
				(int)Info.Location.X,
				(int)Info.Location.Y,
				(int)Info.Size.X,
				(int)Info.Size.Y);
		}
	}

	public virtual Rectangle TransformedHiddenOverflowRectangle
	{
		get
		{
			var location = Vector2.Transform(Info.Location, Main.UIScaleMatrix);
			var bottomRight = Vector2.Transform(Info.Location + Info.Size, Main.UIScaleMatrix);

			return new Rectangle(
				(int)location.X,
				(int)location.Y,
				(int)Math.Max(bottomRight.X - location.X, 0),
				(int)Math.Max(bottomRight.Y - location.Y, 0));
		}
	}

	/// <summary>
	/// 事件管理器
	/// </summary>
	public virtual ElementEvents Events { get => events; }

	public virtual Rectangle HitBox { get => Info.TotalHitBox; }

	public BaseElement()
	{
		events = new ElementEvents();
		Info = ElementInfo.CreateElementInfo();
		ChildrenElements = new List<BaseElement>();
	}

	/// <summary>
	/// 加载事件
	/// </summary>
	public virtual void LoadEvents()
	{
	}

	/// <summary>
	/// 初始化UI部件
	/// </summary>
	public virtual void OnInitialization()
	{
		// ChildrenElements.ForEach(child => child.OnInitialization());
		LoadEvents();
	}

	/// <summary>
	/// 用于执行逻辑的更新方法（不受IsVisible限制）
	/// </summary>
	/// <param name="gt"></param>
	public virtual void PreUpdate(GameTime gt)
	{
		ChildrenElements.ForEach(child => { child?.PreUpdate(gt); });
	}

	/// <summary>
	/// 用于执行逻辑的更新方法（受IsVisible限制）
	/// </summary>
	/// <param name="gt"></param>
	public virtual void Update(GameTime gt)
	{
		ChildrenElements.ForEach(child => { if (child != null && child.IsVisible) { child.Update(gt); } });

		if (IsVisible)
		{
			Events.Update(this, gt);
		}
	}

	/// <summary>
	/// 绘制
	/// </summary>
	/// <param name="sb">画笔</param>
	public virtual void Draw(SpriteBatch sb)
	{
		// 声明光栅化状态，剔除状态为不剔除，开启剪切测试
		var overflowHiddenRasterizerState = new RasterizerState
		{
			CullMode = CullMode.None,
			ScissorTestEnable = true,
		};

		// 如果不隐藏UI部件
		if (!Info.IsHidden && IsVisible)
		{
			// 关闭画笔
			sb.End();

			// 启用画笔，传参：延迟绘制（纹理合批优化），alpha颜色混合模式，点采样，不启用深度模式，UI大小矩阵
			sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap,
				DepthStencilState.None, overflowHiddenRasterizerState, null, Main.UIScaleMatrix);

			// 绘制自己
			DrawSelf(sb);
		}

		// 设定gd是画笔绑定的图像设备
		var gd = sb.GraphicsDevice;

		// 储存绘制原剪切矩形
		var scissorRectangle = gd.ScissorRectangle;

		// 如果启用溢出隐藏
		if (Info.HiddenOverflow)
		{
			// 关闭画笔以便修改绘制参数
			sb.End();

			// 修改光栅化状态
			sb.GraphicsDevice.RasterizerState = overflowHiddenRasterizerState;

			// 修改GD剪切矩形为原剪切矩形与现剪切矩形的交集
			gd.ScissorRectangle = Rectangle.Intersect(gd.ScissorRectangle, TransformedHiddenOverflowRectangle);

			// 启用画笔，传参：延迟绘制（纹理合批优化），alpha颜色混合模式，点采样，不启用深度模式，UI大小矩阵
			sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap,
				DepthStencilState.None, overflowHiddenRasterizerState, null, Main.UIScaleMatrix);
		}

		// 绘制子元素
		DrawChildren(sb);

		// 如果启用溢出隐藏
		if (Info.HiddenOverflow)
		{
			// 关闭画笔
			sb.End();

			// 修改光栅化状态
			gd.RasterizerState = overflowHiddenRasterizerState;

			// 将剪切矩形换回原剪切矩形
			gd.ScissorRectangle = scissorRectangle;

			// 启用画笔
			sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,
				SamplerState.PointWrap, DepthStencilState.None,
				overflowHiddenRasterizerState, null, Main.UIScaleMatrix);
		}
	}

	/// <summary>
	/// 绘制自己
	/// </summary>
	/// <param name="sb">画笔</param>
	protected virtual void DrawSelf(SpriteBatch sb)
	{
	}

	/// <summary>
	/// 绘制子元素
	/// </summary>
	/// <param name="sb">画笔</param>
	protected virtual void DrawChildren(SpriteBatch sb)
	{
		ChildrenElements.ForEach(child => { if (child != null && child.IsVisible) { child.Draw(sb); } });
	}

	/// <summary>
	/// 添加子元素
	/// </summary>
	/// <param name="element">需要添加的子元素</param>
	/// <returns>成功时返回true，否则返回false</returns>
	public virtual bool Register(BaseElement element)
	{
		if (element == null || ChildrenElements.Contains(element) || element.ParentElement != null)
		{
			return false;
		}

		element.ParentElement = this;
		element.OnInitialization();
		element.Calculation();
		ChildrenElements.Add(element);
		return true;
	}

	/// <summary>
	/// 移除子元素
	/// </summary>
	/// <param name="element">需要移除的子元素</param>
	/// <returns>成功时返回true，否则返回false</returns>
	public virtual bool Remove(BaseElement element)
	{
		if (element == null || !ChildrenElements.Contains(element) || element.ParentElement == null)
		{
			return false;
		}

		element.ParentElement = null;
		ChildrenElements.Remove(element);
		return true;
	}

	/// <summary>
	/// 移除所有子元素
	/// </summary>
	public virtual void RemoveAll()
	{
		ChildrenElements.ForEach(child => child.ParentElement = null);
		ChildrenElements.Clear();
	}

	public virtual void SetParentElement(BaseElement element)
	{
		ParentElement = element;
	}

	/// <summary>
	/// 将相对坐标计算为具体坐标
	/// </summary>
	public virtual void Calculation()
	{
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
		ChildrenElements.ForEach(child => { child?.Calculation(); });

		if (Events.Calculation(this))
		{
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
			ChildrenElements.ForEach(child => { child?.Calculation(); });
		}
	}

	/// <summary>
	/// 此UI部件是否包含点
	/// </summary>
	/// <param name="point">输入的点</param>
	/// <returns>如果包含返回true，否则返回false</returns>
	public virtual bool ContainsPoint(Point point) =>
		(GetParentElementIsHiddenOverflow() ? GetCanHitBox() : Info.TotalHitBox).Contains(point);

	/// <summary>
	/// 此UI部件是否包含点
	/// </summary>
	/// <param name="point">输入的点</param>
	/// <returns>如果包含返回true，否则返回false</returns>
	public virtual bool ContainsPoint(Vector2 point) => ContainsPoint(point.ToPoint());

	/// <summary>
	/// 获取在点上的UI部件及上级敏感部件
	/// </summary>
	/// <param name="point">输入的点</param>
	/// <returns>包含点上的UI部件及敏感部件的集合</returns>
	public List<BaseElement> GetElementsContainsPoint(Point point)
	{
		List<BaseElement> elements = new List<BaseElement>();
		bool contains = ContainsPoint(point);
		if (contains && Info.IsSensitive && Info.CanBeInteract)
		{
			elements.Add(this);
		}

		if (ChildrenElements.Count > 0)
		{
			BaseElement child;
			List<BaseElement> childrens;
			for (int i = ChildrenElements.Count - 1; i >= 0; i--)
			{
				child = ChildrenElements[i];
				if (child != null && child.IsVisible)
				{
					childrens = child.GetElementsContainsPoint(point);
					elements.AddRange(childrens);
					if (childrens.Count > 0 && child.Info.InteractiveMask)
					{
						break;
					}
				}
			}
		}

		if (elements.Count == 0 && contains && Info.CanBeInteract && !elements.Contains(this))
		{
			elements.Add(this);
		}
		return elements;
	}

	/// <summary>
	/// 使此UI部件包括其所有UI部件执行输入的方法
	/// </summary>
	/// <param name="action">输入的方法</param>
	public void ForEach(Action<BaseElement> action)
	{
		action(this);
		ChildrenElements.ForEach(child => action(child));
	}

	/// <summary>
	/// 获取被父部件裁切过的碰撞箱
	/// </summary>
	/// <returns>被父部件裁切过的碰撞箱</returns>
	public virtual Rectangle GetCanHitBox()
	{
		if (ParentElement == null)
		{
			return Rectangle.Intersect(new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), HitBox);
		}

		return Rectangle.Intersect(Rectangle.Intersect(HitBox, ParentElement.HiddenOverflowRectangle), ParentElement.GetCanHitBox());
	}

	/// <summary>
	/// 获取此元素与父元素是否开启溢出隐藏
	/// </summary>
	/// <returns>如果有则返回true，否则返回false</returns>
	public bool GetParentElementIsHiddenOverflow()
	{
		if (Info.HiddenOverflow)
		{
			return true;
		}

		if (ParentElement == null)
		{
			return false;
		}

		return ParentElement.GetParentElementIsHiddenOverflow();
	}
}

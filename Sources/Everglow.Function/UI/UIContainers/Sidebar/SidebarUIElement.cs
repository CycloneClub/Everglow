using Everglow.Commons.UI.UIContainers.Sidebar.SidebarElements;
using Everglow.Commons.UI.UIElements;

namespace Everglow.Commons.UI.UIContainers.Sidebar
{
	public delegate void HandleTrigger(SidebarUIElement quickElement);

	public abstract class SidebarUIElement : BaseElement
	{
		private UIImage _icon;
		private int _waitTime = 0;
		private Vector2 _waitToMouse = Vector2.Zero;
		private bool _mouseDown = false;

		public Vector2 Center = Vector2.Zero;

		public event HandleTrigger OnTrigger;

		public int WaitTime { get; set; } = 30;

		public ISidebarElement BaseInfo { get; private set; }

		public bool IsMoveing => _waitTime >= WaitTime;

		public string Tooltip => BaseInfo.Tooltip;

		public SidebarUIElement(Texture2D texture, Color color)
		{
			float size = 0.64f * SidebarContainer.SidebarContainerWidth;
			Info.Width.SetValue(size, 0);
			Info.Height.SetValue(size, 0);
			Info.IsSensitive = true;

			_icon = new(texture, color);
			_icon.Events.OnUpdate += (e, gt) =>
			{
				e.Info.SetToCenter();
			};
			_icon.Info.IsSensitive = true;
			if (texture.Width > texture.Height)
			{
				_icon.Style = UIImage.CalculationStyle.LockAspectRatioMainWidth;
				_icon.Info.Width.SetValue(0f, 1f);
			}
			else
			{
				_icon.Style = UIImage.CalculationStyle.LockedAspectRatioMainHeight;
				_icon.Info.Height.SetValue(0f, 1f);
			}
			_icon.Info.SetToCenter();
			Register(_icon);
		}

		public override void LoadEvents()
		{
			base.LoadEvents();
			Events.OnLeftDown += Events_OnLeftDown;
			Events.OnLeftUp += Events_OnLeftUp;
			Events.OnMouseHover += Events_OnMouseHover;
			Events.OnMouseOver += Events_OnMouseHover;
			Events.OnMouseOut += Events_OnMouseOut;
		}

		private void Events_OnLeftUp(BaseElement baseElement)
		{
			_mouseDown = false;
			if (!IsMoveing)
			{
				OnTrigger?.Invoke(this);
			}
		}

		private void Events_OnLeftDown(BaseElement baseElement)
		{
			_mouseDown = true;
		}

		private void Events_OnMouseHover(BaseElement baseElement)
		{
			_icon.Color = Color.White;
		}

		private void Events_OnMouseOut(BaseElement baseElement)
		{
			_icon.Color = Color.Gray;
		}

		public override void Update(GameTime gt)
		{
			base.Update(gt);

			if (_mouseDown)
				_waitTime++;
			else
				_waitTime = 0;

			if (IsMoveing)
			{
				var c = Main.MouseScreen;
				if (c.X + Info.TotalSize.X / 2f > ParentElement.Info.Location.X + ParentElement.Info.Size.X)
				{
					c.X = ParentElement.Info.Location.X + ParentElement.Info.Size.X - Info.TotalSize.X / 2f;
				}
				if (c.X - Info.TotalSize.X / 2f < ParentElement.Info.Location.X)
				{
					c.X = ParentElement.Info.Location.X + Info.TotalSize.X / 2f;
				}
				if (c.Y + Info.TotalSize.Y / 2f > ParentElement.Info.Location.Y + ParentElement.Info.Size.Y)
				{
					c.Y = ParentElement.Info.Location.Y + ParentElement.Info.Size.Y - Info.TotalSize.Y / 2f;
				}
				if (c.Y - Info.TotalSize.Y / 2f < ParentElement.Info.Location.Y)
				{
					c.Y = ParentElement.Info.Location.Y + Info.TotalSize.Y / 2f;
				}
				MoveTo(c);
			}
			else
				MoveTo(Center);
		}

		public bool MoveTo(Vector2 center)
		{
			if (_waitToMouse != center)
			{
				_waitToMouse += (center - _waitToMouse) / 4f;
				Info.Left.SetValue(_waitToMouse.X - (ParentElement == null ? 0 : ParentElement.Info.Location.X) - Info.Size.X / 2f, 0f);
				Info.Top.SetValue(_waitToMouse.Y - (ParentElement == null ? 0 : ParentElement.Info.Location.Y) - Info.Size.Y / 2f, 0f);
				Calculation();
				return false;
			}
			else
				return true;
		}

		public void SetInfo(ISidebarElement sidebarElement)
		{
			BaseInfo = sidebarElement;
			OnTrigger += element =>
			{
				sidebarElement.Invoke();
			};
		}

		public void UpdateInfo()
		{
			if (Info.IsVisible != BaseInfo.IsVisible())
			{
				Info.IsVisible = BaseInfo.IsVisible();
			}
		}
	}
}

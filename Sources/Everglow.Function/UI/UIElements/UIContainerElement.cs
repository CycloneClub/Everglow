namespace Everglow.Commons.UI.UIElements
{
	public abstract class UIContainerElement : BaseElement
	{
		public virtual string Name { get => GetType().FullName; }

		public virtual bool AutoLoad { get => true; }

		public virtual bool IsInteractiveMask { get => false; }

		public UIContainerElement()
		{
			Info.IsVisible = false;
			Info.InteractiveMask = true;
		}

		public override void OnInitialization()
		{
			base.OnInitialization();
			Info.Width = new PositionStyle(0f, 1f);
			Info.Height = new PositionStyle(0f, 1f);
			Info.CanBeInteract = false;
		}

		public virtual void Show(params object[] args)
		{
			Info.IsVisible = true;
			foreach (var i in UISystem.EverglowUISystem.Elements)
			{
				if (i.Value == this)
				{
					UISystem.EverglowUISystem.SetContainerTop(i.Key);
					break;
				}
			}
			Calculation();
		}

		public virtual void Close(params object[] args)
		{
			Info.IsVisible = false;
		}
	}
}

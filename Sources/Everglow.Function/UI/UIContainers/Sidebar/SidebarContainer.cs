using Everglow.Commons.UI.UIContainers.Sidebar.SidebarElements;
using Everglow.Commons.UI.UIElements;
using ReLogic.Graphics;
using Terraria.GameContent;

namespace Everglow.Commons.UI.UIContainers.Sidebar
{
	public class SidebarContainer : UIContainerElement
	{
		public const float SidebarContainerWidth = 50f;

		private UIPanel mainPanel;
		private bool open = true;
		private string mouseText = string.Empty;
		private KeyCooldown cardModeVisibleCoolDown = new KeyCooldown(() => true, 14);

		public static SidebarContainer Instance => (SidebarContainer)UISystem.EverglowUISystem.Elements[typeof(SidebarContainer).FullName];

		public override bool IsVisible => !Main.playerInventory;

		public override void OnInitialization()
		{
			base.OnInitialization();

			mainPanel = new UIPanel();
			mainPanel.Info.Width.SetValue(SidebarContainerWidth, 0f);
			mainPanel.Events.OnCalculation += element =>
			{
				element.Info.Top.SetValue(-element.Info.Size.Y / 2f, 0.5f);
				return true;
			};
			mainPanel.Info.SetMargin(0f);
			Register(mainPanel);

			SidebarList quickBar = new SidebarList();
			quickBar.Info.Width.SetValue(0f, 0.7f);
			quickBar.Info.Left.SetValue(0f, 0.15f);
			quickBar.Events.OnCalculation += element =>
			{
				mainPanel.Info.Height.SetValue(element.Info.Size.Y + mainPanel.Info.TopMargin.Pixel + mainPanel.Info.BottomMargin.Pixel,
					0f + mainPanel.Info.TopMargin.Percent + mainPanel.Info.BottomMargin.Percent);
				return false;
			};
			mainPanel.Register(quickBar);

			LoadSidebarElement(quickBar);

			// 展开侧边栏/收回侧边栏按钮
			UIImage image = new UIImage(ModAsset.Array.Value, Color.White);
			image.Info.Left.SetValue(2f, 1f);
			image.Info.Top.SetValue(-image.Info.Height.Pixel / 2f, 0.5f);
			image.Events.OnUpdate += (element, gt) =>
			{
				if (mainPanel.Info.TotalLocation.X - mainPanel.Info.TotalSize.X < 4f)
					((UIImage)element).SpriteEffects = SpriteEffects.None;
				if (mainPanel.Info.TotalLocation.X < 2f && mainPanel.Info.TotalLocation.X > -2f)
					((UIImage)element).SpriteEffects = SpriteEffects.FlipHorizontally;
			};
			image.Events.OnLeftClick += element =>
			{
				open = !open;
			};
			mainPanel.Register(image);
		}

		private void LoadSidebarElement(SidebarList quickBar)
		{
			var sidebarElements = Ins.ModuleManager.CreateInstances<ISidebarElement>();

			foreach (var sidebarElement in sidebarElements)
			{
				var quickElement = new TriggeredTypeSidebarUIElement(sidebarElement.Icon, Color.Gray);

				quickElement.SetInfo(sidebarElement);
				quickElement.Events.OnMouseHover += element =>
				{
					mouseText = (element as TriggeredTypeSidebarUIElement).Tooltip;
				};

				quickBar.Register(quickElement);
			}
		}

		public override void Update(GameTime gt)
		{
			base.Update(gt);
			cardModeVisibleCoolDown.Update();

			if (!open && mainPanel.Info.TotalLocation.X != -mainPanel.Info.TotalSize.X)
			{
				mainPanel.Info.Left.SetValue(mainPanel.Info.TotalLocation.X - (mainPanel.Info.TotalLocation.X + mainPanel.Info.TotalSize.X) / 4f, 0f);
			}
			if (open && mainPanel.Info.TotalLocation.X != 0)
			{
				mainPanel.Info.Left.SetValue(mainPanel.Info.TotalLocation.X - mainPanel.Info.TotalLocation.X / 4f, 0f);
			}
			mainPanel.Calculation();
		}

		public override void Draw(SpriteBatch sb)
		{
			base.Draw(sb);

			// 绘制鼠标悬停在按钮上时要显示的文本
			if (!string.IsNullOrEmpty(mouseText))
			{
				var pos = Main.MouseScreen + new Vector2(10f, 18f);
				var textSize = FontAssets.MouseText.Value.MeasureString(mouseText);

				if (pos.X + textSize.X > Main.screenWidth)
				{
					pos.X = Main.screenWidth - textSize.X;
				}
				if (pos.Y + textSize.Y > Main.screenHeight)
				{
					pos.Y = Main.screenHeight - textSize.Y;
				}
				if (pos.X < 0)
				{
					pos.X = 0;
				}
				if (pos.Y < 0)
				{
					pos.Y = 0;
				}

				var PanelColor = new Color(191, 106, 106);
				Texture2D texture = ModAsset.Panel.Value;
				Point textureSize = new Point(texture.Width, texture.Height);
				Rectangle rectangle = new Rectangle((int)pos.X, (int)pos.Y, (int)textSize.X, (int)textSize.Y);

				// 绘制四个角
				sb.Draw(texture, new Vector2(rectangle.X, rectangle.Y), new Rectangle(0, 0, 6, 6), PanelColor);
				sb.Draw(texture, new Vector2(rectangle.X + rectangle.Width - 6, rectangle.Y), new Rectangle(textureSize.X - 6, 0, 6, 6), PanelColor);
				sb.Draw(texture, new Vector2(rectangle.X, rectangle.Y + rectangle.Height - 6), new Rectangle(0, textureSize.Y - 6, 6, 6), PanelColor);
				sb.Draw(texture, new Vector2(rectangle.X + rectangle.Width - 6, rectangle.Y + rectangle.Height - 6), new Rectangle(textureSize.X - 6, textureSize.Y - 6, 6, 6), PanelColor);

				// 绘制本体
				sb.Draw(texture, new Rectangle(rectangle.X + 6, rectangle.Y, rectangle.Width - 12, 6), new Rectangle(6, 0, textureSize.X - 12, 6), PanelColor);
				sb.Draw(texture, new Rectangle(rectangle.X + 6, rectangle.Y + rectangle.Height - 6, rectangle.Width - 12, 6), new Rectangle(6, textureSize.Y - 6, textureSize.X - 12, 6), PanelColor);
				sb.Draw(texture, new Rectangle(rectangle.X, rectangle.Y + 6, 6, rectangle.Height - 12), new Rectangle(0, 6, 6, textureSize.Y - 12), PanelColor);
				sb.Draw(texture, new Rectangle(rectangle.X + rectangle.Width - 6, rectangle.Y + 6, 6, rectangle.Height - 12), new Rectangle(textureSize.X - 6, 6, 6, textureSize.Y - 12), PanelColor);
				sb.Draw(texture, new Rectangle(rectangle.X + 6, rectangle.Y + 6, rectangle.Width - 12, rectangle.Height - 12), new Rectangle(6, 6, textureSize.X - 12, textureSize.Y - 12), PanelColor);

				sb.DrawString(FontAssets.MouseText.Value, mouseText, pos + new Vector2(0f, 5f), Color.Cyan);

				mouseText = string.Empty;
			}
		}
	}
}

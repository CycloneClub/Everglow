using Everglow.Commons.UI.UIElements;

namespace Everglow.Commons.UI
{
	public class EverglowUISystem
	{
		/// <summary>
		/// 存放着所有<see cref="UIContainerElement"/>实例的字典
		/// </summary>
		public Dictionary<string, UIContainerElement> Elements { get; private set; }

		/// <summary>
		/// 访问顺序
		/// </summary>
		public List<string> CallOrder { get; private set; }

		/// <summary>
		/// 交互部件缓存
		/// </summary>
		private List<BaseElement> interactContainerElementsBuffer;

		/// <summary>
		/// 记录需要触发MouseLeftUp事件的部件
		/// </summary>
		private List<BaseElement> needCallMouseLeftUpElements;

		/// <summary>
		/// 记录需要触发MouseRightUp事件的部件
		/// </summary>
		private List<BaseElement> needCallMouseRightUpElements;

		/// <summary>
		/// 缓存鼠标左键状态
		/// </summary>
		private bool mouseLeftDown = false;

		/// <summary>
		/// 缓存鼠标右键状态
		/// </summary>
		private bool mouseRightDown = false;

		/// <summary>
		/// 鼠标右键冷却
		/// </summary>
		private KeyCooldown mouseLeftCooldown;

		/// <summary>
		/// 鼠标左键冷却
		/// </summary>
		private KeyCooldown mouseRightCooldown;

		private Point screenSize;

		public EverglowUISystem()
		{
			Elements = new Dictionary<string, UIContainerElement>();
			CallOrder = new List<string>();
			interactContainerElementsBuffer = new List<BaseElement>();
			needCallMouseLeftUpElements = new List<BaseElement>();
			needCallMouseRightUpElements = new List<BaseElement>();
			mouseLeftCooldown = new KeyCooldown(() =>
			{
				return Main.mouseLeft;
			});
			mouseRightCooldown = new KeyCooldown(() =>
			{
				return Main.mouseRight;
			});
		}

		/// <summary>
		/// 反射加载所有ContainerElement
		/// </summary>
		public void Load()
		{
			var containers = from c in Ins.ModuleManager.Types
							 where !c.IsAbstract && c.IsSubclassOf(typeof(UIContainerElement))
							 select c;
			UIContainerElement element;
			foreach (var c in containers)
			{
				element = (UIContainerElement)Activator.CreateInstance(c);
				if (element.AutoLoad)
				{
					Register(element);
				}
			}
		}

		/// <summary>
		/// 执行逻辑
		/// </summary>
		/// <param name="gt"></param>
		public void Update(GameTime gt)
		{
			if (screenSize != Main.ScreenSize)
			{
				screenSize = Main.ScreenSize;
				Calculation();
			}

			if (CallOrder.Count == 0 || Elements.Count == 0)
			{
				return;
			}

			List<BaseElement> interact = new List<BaseElement>();
			UIContainerElement child;
			Point mousePos = Main.MouseScreen.ToPoint();
			foreach (var key in CallOrder)
			{
				child = Elements[key];
				child?.PreUpdate(gt);
				if (child != null && child.IsVisible)
				{
					child.Update(gt);
					interact.AddRange(child.GetElementsContainsPoint(mousePos));
					if (interact.Count > 0 && child.Info.InteractiveMask)
					{
						break;
					}
				}
			}

			if (interact.Count > 0)
			{
				Main.LocalPlayer.mouseInterface = true;
				interact.ForEach(x => x.Events.MouseHover(x));
			}

			foreach (var ce in interact)
			{
				if (!interactContainerElementsBuffer.Contains(ce))
				{
					ce.Events.MouseOver(ce);
				}
			}
			foreach (var ce in interactContainerElementsBuffer)
			{
				if (!interact.Contains(ce))
				{
					ce.Events.MouseOut(ce);
				}
			}
			interactContainerElementsBuffer = interact;

			if (mouseLeftDown != Main.mouseLeft)
			{
				if (Main.mouseLeft)
				{
					interact.ForEach(x => x.Events.LeftDown(x));
					needCallMouseLeftUpElements.AddRange(interact);
				}
				else
				{
					if (mouseLeftCooldown.IsCoolDown())
					{
						interact.ForEach(x => x.Events.LeftClick(x));
						mouseLeftCooldown.ResetCoolDown();
					}
					else
					{
						interact.ForEach(x => x.Events.LeftDoubleClick(x));
						mouseLeftCooldown.CoolDown();
					}
					needCallMouseLeftUpElements.ForEach(x => x.Events.LeftUp(x));
					needCallMouseLeftUpElements.Clear();
				}

				mouseLeftDown = Main.mouseLeft;
			}

			if (mouseRightDown != Main.mouseRight)
			{
				if (Main.mouseRight)
				{
					interact.ForEach(x => x.Events.RightDown(x));
				}
				else
				{
					if (mouseRightCooldown.IsCoolDown())
					{
						interact.ForEach(x => x.Events.RightClick(x));
						mouseRightCooldown.ResetCoolDown();
					}
					else
					{
						interact.ForEach(x => x.Events.RightDoubleClick(x));
						mouseRightCooldown.CoolDown();
					}
					needCallMouseRightUpElements.ForEach(x => x.Events.RightUp(x));
					needCallMouseRightUpElements.Clear();
				}
				mouseRightDown = Main.mouseRight;
			}

			mouseLeftCooldown.Update();
			mouseRightCooldown.Update();
		}

		/// <summary>
		/// 绘制
		/// </summary>
		/// <param name="sb">画笔</param>
		public void Draw(SpriteBatch sb)
		{
			if (CallOrder.Count == 0 || Elements.Count == 0)
			{
				return;
			}

			UIContainerElement child;
			for (int i = CallOrder.Count - 1; i >= 0; i--)
			{
				child = Elements[CallOrder[i]];
				if (child != null && child.IsVisible)
				{
					child.Draw(sb);
				}
			}
		}

		/// <summary>
		/// 添加子元素
		/// </summary>
		/// <param name="element">需要添加的子元素</param>
		/// <returns>成功时返回true，否则返回false</returns>
		public bool Register(UIContainerElement element)
		{
			return Register(element.Name, element);
		}

		/// <summary>
		/// 添加子元素
		/// </summary>
		/// <param name="name">需要添加的子元素的Name</param>
		/// <param name="element">需要添加的子元素</param>
		/// <returns>成功时返回true，否则返回false</returns>
		public bool Register(string name, UIContainerElement element)
		{
			if (element == null || Elements.ContainsKey(name) || CallOrder.Contains(name))
			{
				return false;
			}

			Elements.Add(name, element);
			CallOrder.Add(element.Name);
			element.OnInitialization();
			element.Calculation();
			return true;
		}

		/// <summary>
		/// 移除子元素
		/// </summary>
		/// <param name="name">需要移除的子元素的Key</param>
		/// <returns>成功时返回true，否则返回false</returns>
		public bool Remove(string name)
		{
			if (CallOrder.Count == 0 || Elements.Count == 0 || !(Elements.ContainsKey(name) || CallOrder.Contains(name)))
			{
				return false;
			}

			Elements.Remove(name);
			CallOrder.Remove(name);
			return true;
		}

		/// <summary>
		/// 将所有容器相对坐标计算为具体坐标
		/// </summary>
		public void Calculation()
		{
			foreach (var child in Elements.Values)
			{
				child?.Calculation();
			}
		}

		/// <summary>
		/// 将容器置顶
		/// </summary>
		/// <param name="name">需要置顶的容器Name</param>
		/// <returns>成功返回true，否则返回false</returns>
		public bool SetContainerTop(string name)
		{
			if (CallOrder.Count == 0 || Elements.Count == 0 || !(Elements.ContainsKey(name) || CallOrder.Contains(name)))
			{
				return false;
			}

			if (CallOrder[0] == name)
			{
				return true;
			}

			CallOrder.Remove(name);
			CallOrder.Insert(0, name);
			return true;
		}

		/// <summary>
		/// 交换两个容器的顺序
		/// </summary>
		/// <param name="name1">容器1的Name</param>
		/// <param name="name2">容器2的Name</param>
		/// <returns>是否交换成功。成功则返回true，否则返回false</returns>
		public bool ExchangeContainer(string name1, string name2)
		{
			if (CallOrder.Count == 0 || Elements.Count == 0 || !(Elements.ContainsKey(name1) || CallOrder.Contains(name1)) ||
				!(Elements.ContainsKey(name2) || CallOrder.Contains(name2)))
			{
				return false;
			}

			int index1 = CallOrder.FindIndex(x => x == name1);
			int index2 = CallOrder.FindIndex(x => x == name2);
			CallOrder.Remove(name1);
			CallOrder.Remove(name2);
			CallOrder.Insert(index1, name2);
			CallOrder.Insert(index2, name1);
			return true;
		}

		/// <summary>
		/// 寻找开启的顶部容器索引
		/// </summary>
		/// <returns>开启的顶部容器索引</returns>
		public int FindTopContainer()
		{
			return CallOrder.FindIndex(x => Elements[x].IsVisible);
		}

		/// <summary>
		/// 关闭所有容器
		/// </summary>
		public void CloseAllContainers()
		{
			foreach (var c in CallOrder)
			{
				Elements[c].Info.IsVisible = false;
			}
		}

		public void DrawInRenderTarget(SpriteBatch sb)
		{
			if (CallOrder.Count == 0 || Elements.Count == 0)
			{
				return;
			}

			UIContainerElement child;
			for (int i = CallOrder.Count - 1; i >= 0; i--)
			{
				child = Elements[CallOrder[i]];
				if (child != null && child.IsVisible)
				{
					DrawEachRt2DChildren(child, sb);
				}
			}
		}

		public void DrawEachRt2DChildren(BaseElement parent, SpriteBatch sb)
		{
			parent.ChildrenElements.ForEach(child =>
			{
				if (child != null && child.IsVisible)
				{
					if (child is IDrawable_InRt2D rt_child)
					{
						rt_child.Draw_InRt2D(sb);
					}
					DrawEachRt2DChildren(child, sb);
				}
			});
		}
	}
}

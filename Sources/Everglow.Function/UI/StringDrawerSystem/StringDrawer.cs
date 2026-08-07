using Everglow.Commons.UI.StringDrawerSystem.DrawerItems;
using Everglow.Commons.UI.StringDrawerSystem.DrawerItems.TextDrawers;

namespace Everglow.Commons.UI.StringDrawerSystem;

/// <summary>
/// 文本绘制工具
/// </summary>
public class StringDrawer : IDrawable
{
	/// <summary>
	/// 绘制元素被添加事件处理器
	/// </summary>
	public delegate void DrawerItemAddedEventHandler(DrawerItem drawerItem);

	private List<DrawerItem> drawerItems = [];
	private List<Vector2> lineSize = [];
	private Vector2 size;

	/// <summary>
	/// 绘制元素
	/// </summary>
	public IReadOnlyCollection<DrawerItem> DrawerItems => drawerItems;

	/// <summary>
	/// 文本绘制尺寸
	/// </summary>
	public Vector2 Size => size;

	/// <summary>
	/// 文本行数
	/// </summary>
	public int Line => lineSize.Count;

	/// <summary>
	/// 文本绘制参数
	/// </summary>
	public StringParameters DefaultParameters { get; } = new StringParameters();

	/// <summary>
	/// 绘制元素被添加前HOOK
	/// </summary>
	public event DrawerItemAddedEventHandler PreDrawerItemAdded;

	/// <summary>
	/// 重置所有绘制元素<see cref="DrawerItems"/>的动画进度
	/// </summary>
	public void ResetAnimation()
	{
		foreach (DrawerItem item in drawerItems)
		{
			item.ResetAnimation();
		}
	}

	/// <summary>
	/// 计算所有文本行的尺寸，结果存放于<see cref="lineSize"/>
	/// </summary>
	public void CalculateLineSize()
	{
		int line = 0;
		lineSize.Clear();
		size = Vector2.Zero;
		List<DrawerItem> lineItems = drawerItems.FindAll(i => i.Line == line);
		while (lineItems.Count > 0)
		{
			Vector2 size = Vector2.Zero;
			foreach (DrawerItem item in lineItems)
			{
				item.OnCalculationLineSize(ref size);
			}
			foreach (DrawerItem item in drawerItems)
			{
				item.PostCalculationLineSize(line, ref size);
			}
			lineSize.Add(size);
			this.size.X = Math.Max(size.X, this.size.X);
			this.size.Y += size.Y;
			line++;
			lineItems = drawerItems.FindAll(i => i.Line == line);
		}
	}

	/// <summary>
	/// 改变元素的位置
	/// </summary>
	/// <param name="pos"></param>
	public void SetPosition(Vector2 pos)
	{
		if (lineSize.Count == 0)
		{
			return;
		}

		Vector2[] linePos = new Vector2[lineSize.Count];
		float[] lineHeight = new float[lineSize.Count];
		linePos[0] = pos;
		for (int i = 1; i < linePos.Length; i++)
		{
			linePos[i].X = pos.X;
			linePos[i].Y += lineSize[i - 1].Y;
			linePos[i].Y += linePos[i - 1].Y;
		}
		foreach (DrawerItem item in drawerItems)
		{
			item.ProcessingLineHeight(ref lineHeight[item.Line], lineSize[item.Line]);
		}
		foreach (DrawerItem item in drawerItems)
		{
			item.SetPosition(lineHeight[item.Line], lineSize[item.Line], ref linePos[item.Line]);
		}
	}

	/// <summary>
	/// Set word wrap
	/// </summary>
	/// <param name="maxLineWidth"></param>
	/// <param name="maxLineWidth"></param>
	public void SetWordWrap(float maxLineWidth, int? maxLineCount = null)
	{
		if (drawerItems.Count == 0)
		{
			return;
		}

		int addLine = 0, nowLine = 0;
		float width = maxLineWidth;
		float nowWidth = 0f;
		for (int i = 0; i < drawerItems.Count; i++)
		{
			var item = drawerItems[i];
			if (nowLine != item.Line)
			{
				nowLine = item.Line;
				nowWidth = 0f;
				width = maxLineWidth;
			}
			item.Line += addLine;
			float itemLength = item.GetSize().X;
			nowWidth += itemLength;
			if (nowWidth == width)
			{
				nowWidth = 0f;
				width = maxLineWidth;
			}
			else if (nowWidth > width)
			{
				int line = item.Line, lineCache = line;
				width = item.WordWrap(ref i, drawerItems, ref line,
					width - nowWidth + itemLength, maxLineWidth, maxLineCount - nowLine + 1);
				addLine += line - lineCache;
				nowWidth = 0f;
			}
		}
		CalculateLineSize();
	}

	public void Init(string initText)
	{
		if (string.IsNullOrEmpty(initText))
		{
			return;
		}

		drawerItems.Clear();

		// TODO: 该行会清除所有事件处理器，应该被清理
		PreDrawerItemAdded = null;

		ParseText(initText);
		CalculateLineSize();
	}

	/// <summary>
	/// 解析源文本，生成绘制实例并加入<see cref="DrawerItems"/>
	/// </summary>
	/// <param name="initText"></param>
	private void ParseText(string initText)
	{
		int line = 0;
		var splitedText = initText.Split('\n');

		Array.ForEach(splitedText, text =>
		{
			// tIndex: text index, 无需匹配的普通内容
			// teIndex: text end index, 无需匹配的普通内容的结尾
			// dIndex: drawer index, []内的内容
			// sIndex: string index, ''内的内容
			// nIndex: name index, 特殊处理项的匹配名
			// pIndex: parameter index, 参数名
			int tIndex = 0, teIndex = 0, dIndex = -1, sIndex = -1, nIndex = -1, pIndex = -1;

			// 转义状态是否被激活，使用'/'作为转义符
			bool escapeSequenceActivated = false;

			StringParameters stringParameters = null;
			string parameterName = string.Empty,
				parameterValue = string.Empty,
				drawerName = string.Empty;
			for (int i = 0; i < text.Length;)
			{
				// 当前字符为转义符'/'，激活转义状态
				if (!escapeSequenceActivated && text[i] == '/')
				{
					escapeSequenceActivated = true;
					i++;
				}
				else
				{
					// 转义状态若处于激活中，则进行转义处理
					if (escapeSequenceActivated)
					{
						// 如果该字符'/'作为转义符使用，则删除该转义符
						if (text[i] == '[' || text[i] == ']' || text[i] == '\'' || text[i] == '/' ||
							text[i] == ',' || text[i] == '=')
						{
							// 删除转义符
							// 注：该处删除了一个字符，等同于进行了i++
							text = text.Remove(i - 1, 1);
						}
						else
						{
							i++;
						}

						// 关闭转义状态
						escapeSequenceActivated = false;
					}
					else
					{
						// 解析结构化字符串 [ \ ' , =
						// 例：[ItemDrawer,Type='123',Stack='9999',StackColor='196,241,255']
						if (text[i] == '[')
						{
							// 开始解析，初始化数据
							nIndex = i;
							dIndex = i;
							teIndex = i;
							if (stringParameters == null || !stringParameters.IsEmpty)
							{
								stringParameters = new StringParameters();
							}
						}
						else if (text[i] == ']' && dIndex != -1)
						{
							// 结束解析
							// 将结构化字符串之前的普通文本创建为TextDrawer
							if (tIndex < teIndex)
							{
								var di = TextDrawer.Create(this, text[tIndex..teIndex], line);
								PreDrawerItemAdded?.Invoke(di);
								drawerItems.Add(di);
							}

							// 将结构化字符串创建为对应DrawerItem
							var drawerItem = DrawerItemLoader.Instance.GetDrawerItem(this, text[dIndex..(i + 1)], drawerName, stringParameters, line);
							PreDrawerItemAdded?.Invoke(drawerItem);
							drawerItems.Add(drawerItem);

							// 重置数据
							tIndex = i + 1;
							dIndex = -1;
							sIndex = -1;
						}
						else if (text[i] == '\'' && dIndex != -1)
						{
							if (sIndex == -1) // 开始获取参数值
							{
								sIndex = i;
							}
							else // 结束获取参数值
							{
								parameterValue = text[(sIndex + 1)..i];
								stringParameters[parameterName] = parameterValue;
								sIndex = -1;
							}
						}
						else if (text[i] == ',' && dIndex != -1)
						{
							// 获取绘制类名
							if (nIndex == dIndex)
							{
								drawerName = text[(nIndex + 1)..i];
								nIndex = -1;
							}
							pIndex = i;
						}
						else if (text[i] == '=')
						{
							// 获取参数名
							if (pIndex != -1)
							{
								parameterName = text[(pIndex + 1)..i];
								pIndex = -1;
							}
						}
						i++;
					}
				}
			}

			if (tIndex != text.Length)
			{
				// 将该行剩余的普通文本生成为TextDrawer
				var td = TextDrawer.Create(this, text[tIndex..text.Length], line);
				PreDrawerItemAdded?.Invoke(td);
				drawerItems.Add(td);
			}
			else if (string.IsNullOrEmpty(text))
			{
				// 如果该行是空的，就创建一个空绘制行
				var td = TextDrawer.Create(this, text, line);
				PreDrawerItemAdded?.Invoke(td);
				drawerItems.Add(td);
			}

			line++;
		});
	}

	public void Draw(SpriteBatch sb)
	{
		foreach (DrawerItem item in drawerItems)
		{
			item.Draw(sb);
		}
	}
}

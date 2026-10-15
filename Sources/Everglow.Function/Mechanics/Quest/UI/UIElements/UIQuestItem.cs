using Everglow.Commons.Mechanics.Quest.Presentation;
using Everglow.Commons.Mechanics.Quest.Presentation.Views;
using Everglow.Commons.UI;
using Everglow.Commons.UI.UIElements;
using Terraria.GameContent;
using static Everglow.Commons.Mechanics.Quest.UI.QuestContainer;

namespace Everglow.Commons.Mechanics.Quest.UI.UIElements;

/// <summary>
/// 任务列表<see cref="QuestContainer"/>的任务项
/// </summary>
public class UIQuestItem : UIBlock, IDrawable_InRt2D
{
	private UIBlock block;
	private UIBlock nameContainer;
	private UITextPlus name;

	private float oldScale;

	private bool mouseOver = false;
	private bool selected = false;

	public QuestPresentationEntry Entry { get; private set; }

	public QuestView View => Entry.View;

	public UIQuestItem(QuestPresentationEntry entry)
	{
		Entry = entry;
		PanelColor = Color.Transparent;
		BorderWidth = 0;

		// 初始化UI信息
		Info.Width.SetValue(320f * Scale, 0f);
		Info.Height.SetValue(60f * Scale, 0f);
		Info.Left.SetValue(100 * Scale);
		Info.SetMargin(0);
		Info.IsSensitive = true;

		// 鼠标悬停时改变颜色
		Events.OnMouseHover += OnMouseOver;
		Events.OnMouseOver += OnMouseOver;
		Events.OnMouseOut += OnMouseLeave;

		// 任务项容器
		block = new UIBlock();
		block.Info.Width.SetFull();
		block.Info.Height.SetFull();
		block.BorderWidth = 0;
		block.PanelColor = Color.Transparent;
		block.Info.SetMargin(0);
		Register(block);

		// 任务名称
		nameContainer = new UIBlock();
		nameContainer.Info.Width.SetValue(220 * Scale);
		nameContainer.Info.Height.SetFull();
		nameContainer.Info.SetToCenter();
		nameContainer.Info.Left.SetValue(0, 0.2f);
		nameContainer.Info.HiddenOverflow = true;
		nameContainer.PanelColor = Color.Transparent;
		nameContainer.BorderWidth = 0;
		block.Register(nameContainer);

		var font = FontManager.FusionPixel12.GetFont(40f * Scale);
		name = new UITextPlus(View.DisplayName);
		name.StringDrawer.DefaultParameters.SetParameter("FontSize", 36f * Scale);
		name.StringDrawer.Init(name.Text);
		nameContainer.Register(name);

		name.Info.SetToCenter();
		name.Info.Left.SetEmpty();
		name.Calculation();
	}

	public override void Calculation()
	{
		base.Calculation();
		float questListWidth = ParentElement.ParentElement.ParentElement.Info.Width.Pixel;
		Info.Width.SetValue(questListWidth - 120, 0f);
		Info.Height.SetValue(93f * Scale, 0f);
		Info.Left.SetValue(20 * Scale);

		nameContainer.Info.Width.SetValue(questListWidth - 240);
		nameContainer.Info.Height.SetFull();
		nameContainer.Info.SetToCenter();
		nameContainer.Info.Left.SetValue(90, 0);

		if (oldScale != Scale)
		{
			oldScale = Scale;

			nameContainer.ChildrenElements.RemoveAll(m => m is UITextPlus);
			name = new UITextPlus(View.DisplayName);
			name.StringDrawer.DefaultParameters.SetParameter("FontSize", 36f * Scale);
			name.StringDrawer.Init(name.Text);
			nameContainer.Register(name);

			name.Info.SetToCenter();
			name.Info.Left.SetEmpty();
			name.Calculation();
		}
	}

	/// <summary>
	/// 鼠标悬停时
	/// <para/>更新任务的颜色，但不更新选中的任务的颜色
	/// </summary>
	/// <param name="e"></param>
	private void OnMouseOver(BaseElement e)
	{
		mouseOver = true;
	}

	/// <summary>
	/// 鼠标离开时
	/// <para/>更新任务的颜色，但不更新选中的任务的颜色
	/// </summary>
	/// <param name="e"></param>
	private void OnMouseLeave(BaseElement e)
	{
		mouseOver = false;
	}

	/// <summary>
	/// 选中任务时
	/// <para/>更新任务的颜色
	/// </summary>
	public void OnSelected()
	{
		selected = true;
	}

	/// <summary>
	/// 取消选中任务时
	/// <para/>更新任务的颜色
	/// </summary>
	public void OnUnselected()
	{
		selected = false;
	}

	protected override void DrawChildren(SpriteBatch sb)
	{
		DrawTimerProgress();
		DrawPanel(sb);
		base.DrawChildren(sb);
	}

	private void DrawPanel(SpriteBatch sb)
	{
		if (ParentElement.ParentElement.ParentElement is UIQuestList uiml)
		{
			int index = uiml.QuestItems.IndexOf(this);
			Vector2 coord = new Vector2(HitBox.X, index * 93 + ParentElement.ParentElement.HitBox.Y) - ParentElement.ParentElement.ParentElement.ParentElement.HitBox.TopLeft();
			var panel_texRectangle = new Rectangle((int)coord.X, (int)coord.Y, HitBox.Width, HitBox.Height);
			Texture2D background = ModAsset.Marble_Texture.Value;
			sb.Draw(background, HitBox, panel_texRectangle, Color.White);
		}
		Texture2D tex = ModAsset.QuestStackPanel.Value;
		var drawBox = HitBox;
		int outer_coord_x = 41;
		int outer_coord_y = 38;
		if (mouseOver)
		{
			outer_coord_x = 41;
			outer_coord_y = 65;
			if (selected)
			{
				outer_coord_x = 50;
				outer_coord_y = 65;
			}
		}
		else if (selected)
		{
			outer_coord_x = 50;
			outer_coord_y = 38;
		}

		// out bound
		Draw9Piece_QuestStackPanel7x7(sb, drawBox, outer_coord_x, outer_coord_y);

		drawBox.X += 5;
		drawBox.Y += 4;
		drawBox.Width -= 10;
		drawBox.Height -= 8;

		// golden framework
		Draw9Piece_QuestStackPanel7x7(sb, drawBox, 59, 38);

		drawBox.X += 68;
		drawBox.Y += 10;
		drawBox.Width -= 79;
		drawBox.Height -= 20;

		// content area
		Draw9Piece_QuestStackPanel7x7(sb, drawBox, 41, 47);

		var gem_frame = ColorDefinition.GetGemFrame(View.Type);
		if (selected)
		{
			gem_frame.Y += 104;
			sb.Draw(tex, HitBox.Left() + new Vector2(40, 0), gem_frame, Color.White, 0, gem_frame.Size() * 0.5f, 1f, SpriteEffects.None, 0);
			gem_frame.Y -= 104;
			sb.Draw(tex, HitBox.Left() + new Vector2(40, 0), gem_frame, new Color(1f, 1f, 1f, 0.5f), 0, gem_frame.Size() * 0.5f, 1f, SpriteEffects.None, 0);
		}
		else
		{
			sb.Draw(tex, HitBox.Left() + new Vector2(40, 0), gem_frame, Color.White, 0, gem_frame.Size() * 0.5f, 1f, SpriteEffects.None, 0);
		}
		var reflectionFrame = new Rectangle(158, 36 + gem_frame.X / 11, 33, 2);
		sb.Draw(tex, HitBox.TopLeft() + new Vector2(40, 5), reflectionFrame, Color.White, 0, reflectionFrame.Size() * 0.5f, 1f, SpriteEffects.None, 0);
		sb.Draw(tex, HitBox.TopLeft() + new Vector2(40, 88), reflectionFrame, Color.White, MathHelper.Pi, reflectionFrame.Size() * 0.5f, 1f, SpriteEffects.None, 0);
		gem_frame = new Rectangle(0, 36, 39, 39);
		sb.Draw(tex, HitBox.Left() + new Vector2(40, 0), gem_frame, Color.White, 0, gem_frame.Size() * 0.5f, 1f, SpriteEffects.None, 0);

		var stateFrame = ColorDefinition.GetQuestStateFrame(View.State);
		sb.Draw(tex, HitBox.Right() + new Vector2(-40, 0), stateFrame, Color.White, 0, stateFrame.Size() * 0.5f, 1f, SpriteEffects.None, 0);
	}

	private void Draw9Piece_QuestStackPanel7x7(SpriteBatch sb, Rectangle hitbox, int frameX, int frameY)
	{
		Texture2D tex = ModAsset.QuestStackPanel.Value;

		// row1
		Rectangle frame = new Rectangle(frameX, frameY, 3, 3);
		Rectangle des = hitbox;
		des.Width = 3;
		des.Height = 3;
		sb.Draw(tex, des, frame, Color.White);

		frame = new Rectangle(frameX + 3, frameY, 1, 3);
		des.Width = hitbox.Width - 6;
		des.X += 3;
		sb.Draw(tex, des, frame, Color.White);

		frame = new Rectangle(frameX + 4, frameY, 3, 3);
		des.Width = 3;
		des.X = hitbox.Right - 3;
		sb.Draw(tex, des, frame, Color.White);

		// row2
		frame = new Rectangle(frameX, frameY + 3, 3, 1);
		des = hitbox;
		des.Width = 3;
		des.Height = hitbox.Height - 6;
		des.Y += 3;
		sb.Draw(tex, des, frame, Color.White);

		frame = new Rectangle(frameX + 3, frameY + 3, 1, 1);
		des.Width = hitbox.Width - 6;
		des.Height = hitbox.Height - 6;
		des.X += 3;
		sb.Draw(tex, des, frame, Color.White);

		frame = new Rectangle(frameX + 4, frameY + 3, 3, 1);
		des.Width = 3;
		des.Height = hitbox.Height - 6;
		des.X = hitbox.Right - 3;
		sb.Draw(tex, des, frame, Color.White);

		// row3
		frame = new Rectangle(frameX, frameY + 4, 3, 3);
		des = hitbox;
		des.Width = 3;
		des.Height = 3;
		des.Y = hitbox.Bottom - 3;
		sb.Draw(tex, des, frame, Color.White);

		frame = new Rectangle(frameX + 3, frameY + 4, 1, 3);
		des.Width = hitbox.Width - 6;
		des.Height = 3;
		des.X += 3;
		sb.Draw(tex, des, frame, Color.White);

		frame = new Rectangle(frameX + 4, frameY + 4, 3, 3);
		des.Width = 3;
		des.X = hitbox.Right - 3;
		des.Height = 3;
		sb.Draw(tex, des, frame, Color.White);
	}

	private void DrawTimerProgress()
	{
		if (View.TimeLimit is not int timeLimit)
		{
			return;
		}

		var progress = 1 - View.ElapsedTime / (float)timeLimit;
		var colorValue = MathF.Sqrt(progress);
		var offset = (int)(45 * QuestContainer.Scale);
		var dest = new Rectangle(HitBox.X + offset, HitBox.Y, (int)((HitBox.Width - offset) * progress), HitBox.Height);
		Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, dest, new Color(0.5f, colorValue * 0.5f, colorValue * 0.5f, 0.1f));
	}

	public void UpdateEntry(QuestPresentationEntry entry)
	{
		Entry = entry;
	}

	public void Draw_InRt2D(SpriteBatch sb)
	{
		//this.Draw(sb);
	}
}

using Everglow.Commons.DataStructures;
using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Presentation;
using Everglow.Commons.Mechanics.Quest.Presentation.Views;
using Everglow.Commons.UI.UIElements;
using Everglow.Commons.Utilities;
using Everglow.Commons.Vertex;

namespace Everglow.Commons.Mechanics.Quest.UI.UIElements;

public class UIQuestFilter : BaseElement
{
	private const float RotationSnapThreshold = 0.01f;

	// Outer ring rotation state
	private float _outerRotation;
	private Vector2? _outerMouseDownPosition;
	private float _outerMouseClickRotation;
	private bool _outerHeld;
	private Vector2 _outerDispalcement;
	private Vector2 _outerClickPoint;
	private float? _outerClickTargetRotation;
	private float? _outerHoverTargetRotation;
	private float _outerRotationMisaligment;

	// Inner ring rotation state
	private float _innerRotation;
	private Vector2? _innerMouseDownPosition;
	private float _innerMouseClickRotation;
	private bool _innerHeld;
	private Vector2 _innerDispalcement;
	private Vector2 _innerClickPoint;
	private float? _innerClickTargetRotation;
	private float? _innerHoverTargetRotation;
	private float _innerRotationMisaligment;

	/// <summary>
	/// Available quest state selections (null represents "All" option)
	/// </summary>
	private static List<QuestViewState?> QuestStateList { get; } =
	[
		null,
		QuestViewState.Available,
		QuestViewState.Active,
		QuestViewState.Completed,
		QuestViewState.Failed,
		QuestViewState.Locked,
	];

	/// <summary>
	/// Available quest type selections (null represents "All" option)
	/// </summary>
	private static List<QuestType?> QuestTypeList { get; } = [null, .. Enum.GetValues<QuestType>()];

	/// <summary>
	/// Currently selected quest state filter
	/// </summary>
	public QuestViewState? QuestStateValue { get; private set; }

	/// <summary>
	/// Currently selected quest type filter
	/// </summary>
	public QuestType? QuestTypeValue { get; private set; }

	public bool SpectrumBlockedAtInner => _innerRotationMisaligment > 0.05f;

	public bool SpectrumBlockedAtOuter => _outerRotationMisaligment > 0.05f;

	private float MouseRotation => HitBox.Center.ToVector2().AngleTo(Main.MouseScreen);

	private float MouseHoldDisplacementLimitForAutoRotation => 10 * QuestContainer.Scale;

	private static QuestType? RotationToQuestType(float rotation)
	{
		var unit = MathHelper.TwoPi / QuestTypeList.Count;
		var standard = ((rotation % MathHelper.TwoPi) + MathHelper.TwoPi) % MathHelper.TwoPi;
		var index = (int)Math.Round(standard / unit) % QuestTypeList.Count;
		return QuestTypeList[index];
	}

	private QuestType? RotationToQuestTypeCheckGemMisalignment(float rotation)
	{
		var unit = MathHelper.TwoPi / QuestTypeList.Count;
		var standard = ((rotation % MathHelper.TwoPi) + MathHelper.TwoPi) % MathHelper.TwoPi;
		var index = (int)Math.Round(standard / unit) % QuestTypeList.Count;
		float angularMisalignment = MathF.Abs((standard + unit * 0.5f) % unit - unit * 0.5f);
		_outerRotationMisaligment = angularMisalignment;

		return QuestTypeList[index];
	}

	private static QuestViewState? RotationToQuestState(float rotation)
	{
		var unit = MathHelper.TwoPi / QuestStateList.Count;
		var standard = ((rotation % MathHelper.TwoPi) + MathHelper.TwoPi) % MathHelper.TwoPi;
		var index = (int)Math.Round(standard / unit) % QuestStateList.Count;
		return QuestStateList[index];
	}

	private QuestViewState? RotationToQuestStateCheckGemMisalignment(float rotation)
	{
		var unit = MathHelper.TwoPi / QuestStateList.Count;
		var standard = ((rotation % MathHelper.TwoPi) + MathHelper.TwoPi) % MathHelper.TwoPi;
		var index = (int)Math.Round(standard / unit) % QuestStateList.Count;
		float angularMisalignment = MathF.Abs((standard + unit * 0.5f) % unit - unit * 0.5f);
		_innerRotationMisaligment = angularMisalignment;

		return QuestStateList[index];
	}

	private static float QuestTypeToRotation(QuestType? type) => QuestTypeList.IndexOf(type) * MathHelper.TwoPi / QuestTypeList.Count;

	private static float QuestStateToRotation(QuestViewState? state) => QuestStateList.IndexOf(state) * MathHelper.TwoPi / QuestStateList.Count;

	private static bool DistanceWithinInnerRing(float distance)
	{
		var innerRadius1 = 252 / 2f;
		innerRadius1 *= QuestContainer.Scale;
		var innerRadius2 = 174 / 2f;
		innerRadius2 *= QuestContainer.Scale;
		return distance < innerRadius1 && distance > innerRadius2;
	}

	private static bool DistanceWithinOuterRing(float distance)
	{
		var outerRadius1 = 342 / 2f;
		outerRadius1 *= QuestContainer.Scale;
		var outerRadius2 = 252 / 2f;
		outerRadius2 *= QuestContainer.Scale;
		return distance < outerRadius1 && distance > outerRadius2;
	}

	private static float GetNaturalRotation(float targetRotation, float originRotation)
	{
		var rotationDiff = (targetRotation - originRotation) % MathHelper.TwoPi;
		if (MathF.Abs(rotationDiff) > MathF.PI)
		{
			if (rotationDiff > 0)
			{
				rotationDiff -= MathHelper.TwoPi;
			}
			else if (rotationDiff < 0)
			{
				rotationDiff += MathHelper.TwoPi;
			}
		}

		return rotationDiff;
	}

	/// <summary>
	/// Calculate the nearest snap rotation based on the current rotation and the count of options.
	/// </summary>
	/// <param name="currentRotation"></param>
	/// <param name="optionCount"></param>
	/// <returns></returns>
	/// <exception cref="InvalidDataException"></exception>
	private static float CalculateNearestSnapRotation(float currentRotation, float optionCount)
	{
		if (optionCount <= 0)
		{
			throw new InvalidDataException();
		}

		var unit = MathHelper.TwoPi / optionCount;
		var diff = currentRotation % MathHelper.TwoPi % unit;

		if (diff > 0)
		{
			if (diff > unit / 2)
			{
				diff = unit - diff;
			}
			else
			{
				diff = -diff;
			}
		}
		else if (diff < 0)
		{
			if (diff < -unit / 2)
			{
				diff = -unit - diff;
			}
			else
			{
				diff = -diff;
			}
		}
		return currentRotation + diff;
	}

	public override void OnInitialization()
	{
		base.OnInitialization();

		Events.OnMouseHover += ManageHeldLogic;
	}

	public override void Calculation()
	{
		base.Calculation();

		var scale = QuestContainer.Scale;
	}

	public override void Update(GameTime gt)
	{
		// Update quest type
		var questType = RotationToQuestTypeCheckGemMisalignment(_outerRotation);
		if (QuestTypeValue != questType)
		{
			QuestTypeValue = questType;
			QuestContainer.Instance.RefreshList();
		}

		// Update quest state
		var questState = RotationToQuestStateCheckGemMisalignment(_innerRotation);
		if (QuestStateValue != questState)
		{
			QuestStateValue = questState;
			QuestContainer.Instance.RefreshList();
		}

		ManageAutoRotation();

		var distanceToCenter = Main.MouseScreen.Distance(HitBox.Center.ToVector2());

		// Outer ring hover logic: show quest type text and highlight the selected quest type
		if (DistanceWithinOuterRing(distanceToCenter))
		{
			var hoverQuestType = RotationToQuestType(MathHelper.Pi - MouseRotation + _outerRotation);
			_outerHoverTargetRotation = QuestTypeToRotation(hoverQuestType);
			QuestContainer.Instance.MouseText = TextDefinition.GetQuestTypeText(hoverQuestType);
		}
		else
		{
			_outerHoverTargetRotation = null;
		}

		// Inner ring hover logic: show quest state text and highlight the selected quest state
		if (DistanceWithinInnerRing(distanceToCenter))
		{
			var hoverQuestState = RotationToQuestState(MathHelper.Pi - MouseRotation + _innerRotation);
			_innerHoverTargetRotation = QuestStateToRotation(hoverQuestState);
			QuestContainer.Instance.MouseText = TextDefinition.GetQuestStateText(hoverQuestState);
		}
		else
		{
			_innerHoverTargetRotation = null;
		}
	}

	/// <summary>
	/// Manage the ring held logic: rotate the inner and outer ring with mouse movement when held.
	/// </summary>
	/// <param name="baseElement"></param>
	private void ManageHeldLogic(BaseElement baseElement)
	{
		// Reset held state when mouse is not clicking
		var mouseLeftClicking = Main.mouseLeft && !Main.mouseLeftRelease;
		if (!mouseLeftClicking)
		{
			_innerHeld = false;
			_outerHeld = false;
		}

		var distanceToCenter = Main.MouseScreen.Distance(HitBox.Center.ToVector2());

		// Outer ring held logic
		if (((DistanceWithinOuterRing(distanceToCenter) && mouseLeftClicking) || _outerHeld) && !_innerHeld)
		{
			RotateRingWithMouse(ref _outerMouseDownPosition, ref _outerMouseClickRotation, ref _outerRotation, ref _outerHeld);
		}
		else
		{
			// Outer on release
			ResetHeldState(ref _outerMouseDownPosition);
		}

		// Inner ring held logic
		if (((DistanceWithinInnerRing(distanceToCenter) && mouseLeftClicking) || _innerHeld) && !_outerHeld)
		{
			RotateRingWithMouse(ref _innerMouseDownPosition, ref _innerMouseClickRotation, ref _innerRotation, ref _innerHeld);
		}
		else
		{
			// Inner on release
			ResetHeldState(ref _innerMouseDownPosition);
		}
	}

	/// <summary>
	/// Clear the ring mouse held state when released.
	/// </summary>
	/// <param name="mouseDownPosition"></param>
	private void ResetHeldState(ref Vector2? mouseDownPosition)
	{
		mouseDownPosition = null;
	}

	/// <summary>
	/// Rotate the ring with mouse movement when held.
	/// </summary>
	/// <param name="mouseDownPosition"></param>
	/// <param name="mouseClickRotation"></param>
	/// <param name="rotation"></param>
	/// <param name="held"></param>
	private void RotateRingWithMouse(ref Vector2? mouseDownPosition, ref float mouseClickRotation, ref float rotation, ref bool held)
	{
		if (mouseDownPosition == null)
		{
			mouseDownPosition = Main.MouseScreen;
			mouseClickRotation = rotation;
			held = true;
		}

		var enterAngle = HitBox.Center.ToVector2().AngleTo(mouseDownPosition.Value);
		rotation = mouseClickRotation + MouseRotation - enterAngle;
	}

	/// <summary>
	/// Rotate the ring to the target rotation.
	/// </summary>
	/// <param name="targetRotation"></param>
	/// <param name="currentRotation"></param>
	private void RotateRingTo(float targetRotation, ref float currentRotation)
	{
		var rotationDiff = GetNaturalRotation(targetRotation, currentRotation);

		if (MathF.Abs(rotationDiff) > RotationSnapThreshold)
		{
			currentRotation += rotationDiff / 10;
		}
		else
		{
			currentRotation += rotationDiff;
		}
	}

	/// <summary>
	/// Manage auto-rotation of two rings.
	/// <para/>The auto-rotation is consist of two part: click-to-select and fix rotation.
	/// </summary>
	private void ManageAutoRotation()
	{
		// Inner spin
		if (_innerHeld)
		{
			_innerDispalcement = Main.MouseScreen - _innerClickPoint;
			_innerClickTargetRotation = null;
		}
		else
		{
			// TODO: _innerDispalcement will reset to Vector2.zero when resolution change. This caused the filter rotated.
			if (_innerDispalcement.Length() < MouseHoldDisplacementLimitForAutoRotation/* && _innerDispalcement.Length() != 0*/)
			{
				var clickedQuestState = RotationToQuestState(MathHelper.Pi - MouseRotation + _innerRotation);
				_innerClickTargetRotation = QuestStateToRotation(clickedQuestState);
			}
			_innerDispalcement = new Vector2(1000);
			_innerClickPoint = Main.MouseScreen;

			// If there's no click target rotation, then fix the rotation to nearest snap.
			_innerClickTargetRotation ??= CalculateNearestSnapRotation(_innerRotation, QuestStateList.Count);

			RotateRingTo(_innerClickTargetRotation.Value, ref _innerRotation);
		}

		// Outer spin
		if (_outerHeld)
		{
			_outerDispalcement = Main.MouseScreen - _outerClickPoint;
			_outerClickTargetRotation = null;
		}
		else
		{
			// TODO: _outerDispalcement will reset to Vector2.zero when resolution change. This caused the filter rotated.
			if (_outerDispalcement.Length() < MouseHoldDisplacementLimitForAutoRotation/* && _outerDispalcement.Length() != 0*/)
			{
				var clickedQuestType = RotationToQuestType(MathHelper.Pi - MouseRotation + _outerRotation);
				_outerClickTargetRotation = QuestTypeToRotation(clickedQuestType);
			}
			_outerDispalcement = new Vector2(1000);
			_outerClickPoint = Main.MouseScreen;

			// If there's no click target rotation, then fix the rotation to nearest snap.
			_outerClickTargetRotation ??= CalculateNearestSnapRotation(_outerRotation, QuestTypeList.Count);
			RotateRingTo(_outerClickTargetRotation.Value, ref _outerRotation);
		}
	}

	public override void Draw(SpriteBatch sb)
	{
		base.Draw(sb);

		var drawPos = new Vector2(Info.HitBox.X + Info.HitBox.Width / 2, Info.HitBox.Y + Info.HitBox.Height / 2);
		var scale = QuestContainer.Scale;

		// gems and panel
		Vector2 panel_relativePos = drawPos - ParentElement.Info.HitBox.TopLeft();
		Texture2D background = ModAsset.Marble_Texture.Value;
		Vector2 parentSize = background.Size();
		SpriteBatchState sBS = GraphicsUtils.GetState(sb).Value;
		sb.End();
		sb.Begin(SpriteSortMode.Immediate, sBS.BlendState, SamplerState.PointWrap, sBS.DepthStencilState, sBS.RasterizerState, sBS.Effect, sBS.TransformMatrix);
		List<Vertex2D> bars = [];
		Main.graphics.graphicsDevice.Textures[0] = background;
		for (int i = 0; i <= 100;i++)
		{
			float rot = i / 100f * MathHelper.TwoPi;
			Vector2 addPos0 = new Vector2(-171, 0).RotatedBy(rot);
			Vector2 addPos1 = new Vector2(-126, 0).RotatedBy(rot);
			bars.Add(drawPos + addPos0.RotatedBy(_outerRotation), Color.White, new Vector3((panel_relativePos + addPos0) / parentSize, 0));
			bars.Add(drawPos + addPos1.RotatedBy(_outerRotation), Color.White, new Vector3((panel_relativePos + addPos1) / parentSize, 0));
		}
		Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

		bars = [];
		for (int i = 0; i <= 80; i++)
		{
			float rot = i / 80f * MathHelper.TwoPi;
			Vector2 addPos0 = new Vector2(-126, 0).RotatedBy(rot);
			Vector2 addPos1 = new Vector2(-87, 0).RotatedBy(rot);
			bars.Add(drawPos + addPos0.RotatedBy(_innerRotation), Color.White, new Vector3((panel_relativePos + addPos0) / parentSize, 0));
			bars.Add(drawPos + addPos1.RotatedBy(_innerRotation), Color.White, new Vector3((panel_relativePos + addPos1) / parentSize, 0));
		}
		Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		sb.End();
		sb.Begin(sBS);

		var typeFilter = ModAsset.QuestSortRing_Type.Value;
		sb.Draw(typeFilter, drawPos, null, Color.White, _outerRotation, typeFilter.Size() / 2, 1, SpriteEffects.None, 0);
		var gems = ModAsset.QuestSortGems.Value;
		for (int k = 0; k < 8; k++)
		{
			Rectangle frame = new Rectangle(26 * k, 0, 26, 26);
			float subRot = k / 8f * MathHelper.TwoPi;
			sb.Draw(gems, drawPos + new Vector2(-149, 0).RotatedBy(subRot + _outerRotation), frame, Color.White, 0, frame.Size() / 2, 1f, SpriteEffects.None, 0);
		}

		var statusFilter = ModAsset.QuestSortRing_Status.Value;
		sb.Draw(statusFilter, drawPos, null, Color.White, _innerRotation, statusFilter.Size() / 2, 1, SpriteEffects.None, 0);
		for (int k = 0; k < QuestStateList.Count; k++)
		{
			Rectangle frame = ColorDefinition.GetQuestStateGemFrame(QuestStateList[k]);
			float subRot = k / (float)QuestStateList.Count * MathHelper.TwoPi;
			sb.Draw(gems, drawPos + new Vector2(-107, 0).RotatedBy(subRot + _innerRotation), frame, Color.White, 0, frame.Size() / 2, 1f, SpriteEffects.None, 0);
		}

		if (_innerHoverTargetRotation != null)
		{
			var status_highlight = ModAsset.QuestSortRing_Status_highlight.Value;
			int sideLength = status_highlight.Width;
			var frame0 = new Rectangle(0, 0, sideLength, sideLength);
			sb.Draw(status_highlight, drawPos, frame0, Color.White, _innerRotation - _innerHoverTargetRotation.Value, frame0.Size() / 2, scale, SpriteEffects.None, 0);
			float timeValue = 0.4f;
			var frame1 = new Rectangle(0, sideLength, sideLength, sideLength);
			sb.Draw(status_highlight, drawPos, frame0, new Color(1f, 1f, 1f, 0) * timeValue, _innerRotation - _innerHoverTargetRotation.Value, frame1.Size() / 2, scale, SpriteEffects.None, 0);
		}
		if (_outerHoverTargetRotation != null)
		{
			var type_highlight = ModAsset.QuestSortRing_Type_highlight.Value;
			int sideLength = type_highlight.Width;
			var frame0 = new Rectangle(0, 0, sideLength, sideLength);
			sb.Draw(type_highlight, drawPos, frame0, Color.White, _outerRotation - _outerHoverTargetRotation.Value, frame0.Size() / 2, scale, SpriteEffects.None, 0);
			float timeValue = 0.4f;
			var frame1 = new Rectangle(0, sideLength, sideLength, sideLength);
			sb.Draw(type_highlight, drawPos, frame0, new Color(1f, 1f, 1f, 0) * timeValue, _outerRotation - _outerHoverTargetRotation.Value, frame1.Size() / 2, scale, SpriteEffects.None, 0);
		}
	}
}

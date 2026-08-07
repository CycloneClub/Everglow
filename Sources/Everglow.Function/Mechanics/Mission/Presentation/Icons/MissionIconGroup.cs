namespace Everglow.Commons.Mechanics.Mission.Presentation.Icons;

public sealed class MissionIconGroup
{
	public MissionIconGroup()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="MissionIconGroup"/> class with icons.
	/// </summary>
	/// <param name="icons">The initial list of icons.</param>
	public MissionIconGroup(params MissionIconBase[] icons)
	{
		_iconDetails.AddRange(icons);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="MissionIconGroup"/> class with lists of icons.
	/// </summary>
	/// <param name="icons">The initial list of icons.</param>
	public MissionIconGroup(params IEnumerable<MissionIconBase>[] icons)
	{
		_iconDetails.AddRange(icons.SelectMany(x => x));
	}

	private readonly List<MissionIconBase> _iconDetails = [];
	private int _currentIndex;

	public MissionIconBase DefaultIcon => TextureMissionIcon.Create(ModAsset.Point.Value);

	/// <summary>
	/// Gets the current icon.
	/// </summary>
	public MissionIconBase CurrentIcon
	{
		get
		{
			if (_iconDetails.Count == 0)
			{
				return DefaultIcon;
				throw new InvalidOperationException("No icons are available in the group.");
			}

			if (_currentIndex < 0 || _currentIndex >= _iconDetails.Count)
			{
				return DefaultIcon;
				throw new InvalidOperationException("Current index is out of bounds.");
			}

			return _iconDetails[_currentIndex];
		}
	}

	public IReadOnlyList<MissionIconBase> Icons => _iconDetails;

	public int CurrentIndex => _currentIndex;

	/// <summary>
	/// Adds an icon to the group.
	/// </summary>
	/// <param name="icon">The icon to add.</param>
	public void Add(params MissionIconBase[] icon)
	{
		ArgumentNullException.ThrowIfNull(icon);

		_iconDetails.AddRange(icon);
	}

	public void AddRange(IEnumerable<MissionIconBase> icons)
	{
		ArgumentNullException.ThrowIfNull(icons);

		_iconDetails.AddRange(icons);
	}

	/// <summary>
	/// Removes an icon from the group.
	/// </summary>
	/// <param name="icon">The icon to remove.</param>
	/// <returns>True if the icon was removed; otherwise, false.</returns>
	public bool Remove(MissionIconBase icon)
	{
		ArgumentNullException.ThrowIfNull(icon);

		return _iconDetails.Remove(icon);
	}

	/// <summary>
	/// Moves to the previous icon in the group.
	/// </summary>
	public void Prev()
	{
		if (_iconDetails.Count == 0)
		{
			throw new InvalidOperationException("No icons are available in the group.");
		}

		_currentIndex = (_currentIndex - 1 + _iconDetails.Count) % _iconDetails.Count;
	}

	/// <summary>
	/// Moves to the next icon in the group.
	/// </summary>
	public void Next()
	{
		if (_iconDetails.Count == 0)
		{
			throw new InvalidOperationException("No icons are available in the group.");
		}

		_currentIndex = (_currentIndex + 1) % _iconDetails.Count;
	}

	/// <summary>
	/// Gets the number of icons in the group.
	/// </summary>
	public int IconCount => _iconDetails.Count;

	public bool IsLastIcon => _currentIndex == _iconDetails.Count - 1;

	public bool IsFirstIcon => _currentIndex == 0;
}

using Everglow.Food.FoodUtilities;

namespace Everglow.Food;

public class FoodInfo
{
	public int Satiety
	{
		get;
		set;
	}

	public int BuffType
	{
		get;
		set;
	}

	public FoodDuration BuffTime
	{
		get;
		set;
	}

	public string Name
	{
		get;
		set;
	}
}

public class DrinkInfo
{
	public bool Thirsty
	{
		get;
		set;
	}

	public int BuffType
	{
		get;
		set;
	}

	public FoodDuration BuffTime
	{
		get;
		set;
	}

	public string Name
	{
		get;
		set;
	}
}

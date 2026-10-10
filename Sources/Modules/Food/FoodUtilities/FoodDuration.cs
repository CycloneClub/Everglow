using Newtonsoft.Json;
using System.Text;
using Terraria.Localization;

namespace Everglow.Food.FoodUtilities;

public class FoodDuration
{
	public double TotalSeconds
	{
		get
		{
			return TotalFrames / (double)FramesPerSecond;
		}
	}

	public double TotalMinutes
	{
		get
		{
			return TotalFrames / (double)FramesPerMinute;
		}
	}

	public double TotalHours
	{
		get
		{
			return TotalFrames / (double)FramesPerHour;
		}
	}

	public int TotalFrames
	{
		get
		{
			return ((hours * 60 + minutes) * 60 + seconds) * 60 + frames;
		}
	}

	[JsonProperty(PropertyName = "Hours")]
	private int hours;
	[JsonProperty(PropertyName = "Minutes")]
	private int minutes;
	[JsonProperty(PropertyName = "Seconds")]
	private int seconds;
	[JsonProperty(PropertyName = "Frames")]
	private int frames;

	private const int FramesPerSecond = 60;
	private const int FramesPerMinute = 3600;
	private const int FramesPerHour = 216000;

	public FoodDuration(int hours, int minutes, int seconds, int frames)
	{
		this.hours = hours;
		this.minutes = minutes;
		this.seconds = seconds;
		this.frames = frames;

		Normalize();
	}

	public FoodDuration(int minutes, int seconds, int frames)
		: this(0, minutes, seconds, frames)
	{
	}

	public FoodDuration(int seconds, int frames)
		: this(0, 0, seconds, frames)
	{
	}

	public FoodDuration(int frames)
		: this(0, 0, 0, frames)
	{
	}

	private void Normalize()
	{
		int carry = frames / 60;
		frames %= 60;

		seconds += carry;
		carry = seconds / 60;
		seconds %= 60;

		minutes += carry;
		carry = minutes / 60;
		minutes %= 60;

		hours += carry;
	}

	public string ToBuffTimeString()
	{
		var sb = new StringBuilder();
		if (hours != 0)
		{
			sb.Append($"{hours} " + Language.GetTextValue("Mods.Everglow.Common.Hour"));
		}

		if (minutes != 0)
		{
			sb.Append($"{minutes} " + Language.GetTextValue("Mods.Everglow.Common.Minute"));
		}

		if (seconds != 0 && frames == 0)
		{
			sb.Append($"{seconds} " + Language.GetTextValue("Mods.Everglow.Common.Second"));
		}
		else if (seconds != 0 && frames != 0)
		{
			sb.Append($"{(seconds + frames / 60.0).ToString("0.##")} " + Language.GetTextValue("Mods.Everglow.Common.Second"));
		}
		sb.Append(" " + Language.GetTextValue("Mods.Everglow.Common.Duration"));
		return sb.ToString();
	}
}

namespace Everglow.Commons.UI.StringDrawerSystem;

public class StringParameters
{
	private Dictionary<StringParameterKey, string> parameters = [];

	public bool IsEmpty => parameters.Count == 0;

	public string this[StringParameterKey key]
	{
		get => parameters.TryGetValue(key, out string value) ? value : string.Empty;
		set
		{
			if (!parameters.TryAdd(key, value))
			{
				parameters[key] = value;
			}
		}
	}

	public void SetParameter(StringParameterKey key, object value)
	{
		if (value is Vector2 vector2)
		{
			parameters[key] = $"{vector2.X},{vector2.Y}";
		}
		else if (value is Color color)
		{
			parameters[key] = $"{color.R},{color.G},{color.B},{color.A}";
		}
		else if (value is Rectangle rectangle)
		{
			parameters[key] = $"{rectangle.X},{rectangle.Y},{rectangle.Width},{rectangle.Height}";
		}
		else
		{
			parameters[key] = value.ToString();
		}
	}

	public int GetInt(StringParameterKey key, int defaultValue = 0)
	{
		return TryGetInt(key, out int value) ? value : defaultValue;
	}

	public bool TryGetInt(StringParameterKey key, out int value)
	{
		if (parameters.ContainsKey(key) && int.TryParse(this[key], out int result))
		{
			value = result;
			return true;
		}
		else
		{
			value = 0;
			return false;
		}
	}

	public string GetString(StringParameterKey key, string defaultValue = "")
	{
		return TryGetString(key, out string value) ? value : defaultValue;
	}

	public bool TryGetString(StringParameterKey key, out string value)
	{
		return parameters.TryGetValue(key, out value);
	}

	public float GetFloat(StringParameterKey key, float defaultValue = 0)
	{
		return TryGetFloat(key, out float value) ? value : defaultValue;
	}

	public bool TryGetFloat(StringParameterKey key, out float value)
	{
		if (parameters.ContainsKey(key) && float.TryParse(this[key], out float result))
		{
			value = result;
			return true;
		}
		else
		{
			value = 0f;
			return false;
		}
	}

	public Vector2 GetVector2(StringParameterKey key, Vector2 defaultValue = default)
	{
		return TryGetVector2(key, out Vector2 value) ? value : defaultValue;
	}

	public bool TryGetVector2(StringParameterKey key, out Vector2 value)
	{
		if (parameters.ContainsKey(key))
		{
			var ps = this[key].Split(',');
			if (ps.Length == 1 && float.TryParse(ps[0], out float f1))
			{
				value = new Vector2(f1);
				return true;
			}
			if (ps.Length == 2 && float.TryParse(ps[0], out f1) && float.TryParse(ps[1], out float f2))
			{
				value = new Vector2(f1, f2);
				return true;
			}
			value = Vector2.Zero;
			return false;
		}
		else
		{
			value = Vector2.Zero;
			return false;
		}
	}

	public Color GetColor(StringParameterKey key, Color defaultValue = default)
	{
		return TryGetColor(key, out Color value) ? value : defaultValue;
	}

	public bool TryGetColor(StringParameterKey key, out Color value)
	{
		if (parameters.ContainsKey(key))
		{
			var ps = this[key].Split(',');
			if (ps.Length == 3 && byte.TryParse(ps[0], out byte c1) && byte.TryParse(ps[1], out byte c2)
				&& byte.TryParse(ps[2], out byte c3))
			{
				value = new Color(c1, c2, c3);
				return true;
			}
			if (ps.Length == 4 && byte.TryParse(ps[0], out c1) && byte.TryParse(ps[1], out c2)
				&& byte.TryParse(ps[2], out c3) && byte.TryParse(ps[3], out byte c4))
			{
				value = new Color(c1, c2, c3, c4);
				return true;
			}

			value = default;
			return false;
		}
		else
		{
			value = default;
			return false;
		}
	}

	public Rectangle GetRectangle(StringParameterKey key, Rectangle defaultValue = default)
	{
		return TryGetRectangle(key, out Rectangle value) ? value : defaultValue;
	}

	public bool TryGetRectangle(StringParameterKey key, out Rectangle value)
	{
		if (parameters.ContainsKey(key))
		{
			var ps = this[key].Split(',');
			if (ps.Length == 4 && int.TryParse(ps[0], out int c1) && int.TryParse(ps[1], out int c2)
				&& int.TryParse(ps[2], out int c3) && int.TryParse(ps[3], out int c4))
			{
				value = new Rectangle(c1, c2, c3, c4);
				return true;
			}

			value = default;
			return false;
		}
		else
		{
			value = default;
			return false;
		}
	}
}

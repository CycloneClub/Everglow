namespace Everglow.Commons.UI.StringDrawerSystem;

/// <summary>
/// The key type of <see cref="StringParameters"/>, used to improve the code safety
/// </summary>
public class StringParameterKey : IEquatable<StringParameterKey>
{
	public static readonly StringParameterKey FontSize = new(nameof(FontSize));

	public StringParameterKey(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			throw new ArgumentNullException(InvalidValueErrorMsg(value));
		}

		Value = value;
	}

	public string Value { get; }

	public static implicit operator string(StringParameterKey key) => key.Value;

#warning 以下隐式转换不利于保持代码一致性
	public static implicit operator StringParameterKey(string key) => new(key);

	public bool Equals(StringParameterKey other) => Value.Equals(other.Value);

	public override bool Equals(object obj)
	{
		if (obj is null)
		{
			return false;
		}

		return obj is StringParameterKey other && Equals(other);
	}

	public override int GetHashCode() => Value.GetHashCode();

	public override string ToString() => Value;

	public static bool operator ==(StringParameterKey a, StringParameterKey b) => a.Value == b.Value;

	public static bool operator !=(StringParameterKey a, StringParameterKey b) => !(a == b);

	public static string InvalidValueErrorMsg(string value) => $"Value '{value}' is invalid for StringParameterKey!";
}

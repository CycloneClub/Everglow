using Everglow.Commons.Enums;

namespace Everglow.Commons.Interfaces;

public interface IVisualQualityController
{
	public VisualQuality Quality { get; }

	public bool High => Quality == VisualQuality.High;

	public bool Low => Quality == VisualQuality.Low;
}

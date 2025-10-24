namespace Everglow.Commons.Physics.PBEngine.Core
{
	/// <summary>
	/// 刚体的移动模式，用于区分模拟方法以及提升性能
	/// </summary>
	public enum MovementType
	{
		/// <summary>
		/// 静态物体，无法移动、转向
		/// </summary>
		Static,

		/// <summary>
		/// 动态物体，但是拥有无限质量，动作无法被其他动态物体影响
		/// </summary>
		Kinematic,

		/// <summary>
		/// 玩家物体，动态，具有质量但是不可旋转
		/// </summary>
		Player,

		/// <summary>
		/// 动态物体，可以任意移动和旋转，可以被任何物体影响
		/// </summary>
		Dynamic,
	}
}
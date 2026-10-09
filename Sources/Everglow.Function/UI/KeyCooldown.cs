namespace Everglow.Commons.UI
{
	internal class KeyCooldown
	{
		private int _coolDownTicks;
		private Func<bool> _keyDown;
		private int coolDown = 0;

		/// <summary>
		/// 冷却的时长
		/// </summary>
		public int CoolDownTime
		{
			get { return _coolDownTicks; }
			set
			{
				if (value >= 0)
				{
					_coolDownTicks = value;
				}
			}
		}

		public KeyCooldown(Func<bool> keyDown, int coolDownTime = 14)
		{
			CoolDownTime = coolDownTime;
			_keyDown = keyDown;
		}

		public bool IsKeyDown()
		{
			return IsCoolDown() && _keyDown();
		}

		public void Update()
		{
			if (coolDown > 0)
			{
				coolDown--;
			}
		}

		public bool IsCoolDown()
		{
			return coolDown == 0;
		}

		public void CoolDown()
		{
			coolDown = 0;
		}

		public void ResetCoolDown()
		{
			coolDown = CoolDownTime;
		}
	}
}

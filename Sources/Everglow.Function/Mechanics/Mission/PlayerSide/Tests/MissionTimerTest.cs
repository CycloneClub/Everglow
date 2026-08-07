using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Everglow.Commons.Mechanics.Mission.PlayerSide.Abstractions;

namespace Everglow.Commons.Mechanics.Mission.PlayerSide.Tests;

public class MissionTimerTest : PlayerMissionBase
{
	public override string DisplayName => GetType().Name;

	public override long TimeLimit => 18000;
}

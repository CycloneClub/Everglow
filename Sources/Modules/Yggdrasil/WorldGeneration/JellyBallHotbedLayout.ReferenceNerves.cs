namespace Everglow.Yggdrasil.WorldGeneration;

internal sealed partial class JellyBallHotbedLayout
{
	private static readonly (float X, float Y)[][] OuterNerveTrunks =
	[
		[(266, 443), (281, 479), (283, 520), (284, 543), (269, 575), (269, 616), (257, 673), (240, 719), (220, 735), (205, 770), (220, 784)],
		[(348, 449), (350, 489), (370, 523), (393, 560), (371, 587), (354, 619), (358, 649), (341, 675), (330, 708), (318, 731), (324, 764), (347, 796), (350, 829)],
		[(558, 480), (574, 520), (576, 550), (558, 575), (540, 593), (517, 608), (510, 634)],
		[(576, 550), (589, 583), (615, 611), (632, 644), (628, 673), (619, 696), (628, 730), (648, 748), (663, 768), (660, 790), (675, 813)],
		[(220, 784), (244, 809), (273, 818), (297, 838), (324, 828), (350, 829), (363, 850), (390, 826), (414, 824), (439, 837), (466, 813), (490, 824), (516, 849), (542, 841), (562, 826), (587, 820), (616, 801), (642, 809), (675, 813)],
		[(220, 735), (190, 721), (158, 718), (125, 721), (116, 746), (118, 764)],
		[(619, 696), (598, 714), (584, 737), (561, 748), (543, 742), (535, 721), (506, 720)],
		[(350, 829), (353, 856), (337, 880), (342, 899)],
	];

	private static readonly (float X, float Y)[][] OuterNerveBranches =
	[
		[(281, 479), (286, 462), (298, 446)],
		[(284, 543), (302, 527), (315, 510)],
		[(284, 543), (259, 522), (239, 497), (234, 483)],
		[(318, 731), (303, 746), (301, 780)],
		[(576, 550), (597, 563), (621, 557), (647, 542), (657, 505)],
		[(540, 593), (534, 611), (537, 628)],
		[(615, 611), (603, 622), (600, 637)],
		[(535, 721), (548, 702), (543, 680), (551, 666)],
		[(628, 730), (649, 710), (648, 695)],
		[(648, 748), (630, 754), (621, 779), (616, 801)],
		[(675, 813), (697, 808), (700, 783), (698, 766)],
		[(663, 768), (680, 749), (679, 735)],
		[(158, 718), (180, 733), (176, 747), (153, 769), (147, 798)],
		[(205, 770), (185, 788), (164, 804), (151, 802)],
		[(244, 809), (237, 834), (252, 859)],
		[(273, 818), (268, 844), (284, 865), (300, 868)],
		[(297, 838), (311, 855), (313, 885)],
		[(324, 828), (322, 807), (338, 798)],
		[(363, 850), (380, 875), (398, 891)],
		[(390, 826), (394, 850), (416, 869), (424, 890)],
		[(244, 809), (251, 797), (262, 801), (273, 818)],
		[(297, 838), (293, 816), (281, 800)],
		[(350, 829), (365, 812), (379, 818), (390, 826)],
		[(353, 856), (370, 841), (382, 843), (394, 850)],
		[(587, 820), (597, 807), (602, 791)],
	];

	// 中央与右下参考中心线：离线细化后简化为曲线控制点，不保留笔画厚度。
	private static readonly (float X, float Y)[][] InnerNerveTrunks =
	[
		[(323, 714), (333, 695), (333, 685), (340, 676), (351, 674)],
		[(323, 714), (339, 744)],
		[(345, 460), (349, 466)],
		[(353, 470), (355, 484)],
		[(357, 664), (361, 647)],
		[(359, 618), (361, 609), (387, 574)],
		[(359, 623), (363, 640)],
		[(377, 549), (387, 574)],
		[(387, 574), (401, 588)],
		[(419, 612), (437, 634)],
		[(434, 825), (466, 857)],
		[(441, 639), (449, 658), (445, 686)],
		[(441, 698), (445, 686)],
		[(445, 686), (457, 684), (467, 671)],
		[(466, 857), (467, 874), (479, 890)],
		[(466, 857), (471, 837), (487, 818)],
		[(487, 804), (487, 818)],
		[(487, 818), (497, 816)],
		[(491, 715), (491, 734)],
		[(491, 734), (500, 726)],
		[(516, 812), (522, 812), (525, 805)],
		[(581, 815), (597, 838), (599, 848), (607, 852)],
		[(639, 798), (641, 818)],
		[(639, 798), (650, 801)],
	];

	private static readonly (float X, float Y)[][] InnerNerveBranches =
	[
		[(331, 660), (337, 639), (359, 621)],
		[(332, 724), (357, 720)],
		[(351, 468), (363, 435)],
		[(353, 514), (379, 548)],
		[(353, 673), (351, 692), (363, 714)],
		[(356, 667), (379, 684), (411, 692)],
		[(400, 844), (415, 848)],
		[(400, 830), (410, 839)],
		[(400, 890), (415, 863), (415, 848)],
		[(415, 848), (425, 853)],
		[(419, 817), (425, 842)],
		[(425, 842), (425, 853)],
		[(425, 842), (437, 837)],
		[(425, 853), (431, 870)],
		[(439, 640), (439, 619), (455, 591)],
		[(467, 828), (467, 820), (471, 824)],
		[(473, 837), (477, 856), (493, 873)],
		[(493, 858), (493, 827), (499, 820), (499, 804)],
		[(493, 873), (505, 886)],
		[(499, 862), (531, 880)],
		[(513, 814), (525, 826), (535, 852)],
		[(529, 806), (534, 815)],
		[(531, 880), (535, 856)],
		[(531, 880), (539, 898)],
		[(534, 815), (541, 814), (552, 829)],
		[(535, 852), (539, 841), (552, 829)],
		[(535, 856), (555, 870), (557, 876)],
		[(561, 824), (569, 813)],
		[(561, 824), (567, 880)],
		[(592, 808), (599, 800)],
		[(604, 803), (605, 830), (617, 838)],
		[(604, 803), (611, 795)],
	];

	/// <summary>
	/// 固定细曲线骨架：1 为主脉，2 为支脉，仅替换已有球冻组织。
	/// </summary>
	public static byte[,] TraceReferenceNerves(byte[,] layout)
	{
		int width = layout.GetLength(0);
		int height = layout.GetLength(1);
		var nerves = new byte[width, height];
		int surface = height;
		for (int x = 4; x < width - 4; x++)
		{
			for (int y = 4; y < height - 4; y++)
			{
				if (layout[x, y] == Honey)
				{
					surface = Math.Min(surface, y);
				}
			}
		}
		if (surface == height)
		{
			surface = (int)(height * 0.85f);
		}
		float underPool = surface + (height - 10 - surface) * 0.54f;
		(float X, float Y) Map((float X, float Y) point)
		{
			// 只用全局锚点定位整组曲线，膜与细胞的局部凹凸不得传递给神经。
			float y = point.Y <= 780 ? surface + (point.Y - 780) * width / 720f
				: point.Y <= 820 ? surface + (point.Y - 780) / 40 * (underPool - surface)
				: underPool + (point.Y - 820) / 90 * (height - 10 - underPool);
			return (width * 0.5f + (point.X - 420) * width / 760f, y);
		}
		static bool IsInnerNerveRegion(float x, float y) => (x >= 320 && x <= 500 && y >= 432 && y <= 785) || (x >= 400 && x <= 650 && y >= 795 && y <= 910);
		void Draw((float X, float Y)[] knots, byte style, bool innerRegion = false)
		{
			var points = knots.Select(Map).ToArray();
			for (int segment = 0; segment < points.Length - 1; segment++)
			{
				int previous = Math.Max(0, segment - 1);
				int next = Math.Min(points.Length - 1, segment + 2);
				var referenceA = knots[previous];
				var referenceB = knots[segment];
				var referenceC = knots[segment + 1];
				var referenceD = knots[next];
				var a = points[previous];
				var b = points[segment];
				var c = points[segment + 1];
				var d = points[next];
				int steps = Math.Max(8, (int)MathF.Ceiling((MathF.Abs(c.X - b.X) + MathF.Abs(c.Y - b.Y)) * 4));
				for (int step = 0; step <= steps; step++)
				{
					float t = step / (float)steps;
					float referenceX = SampleNerveCurve(referenceA.X, referenceB.X, referenceC.X, referenceD.X, t);
					float referenceY = SampleNerveCurve(referenceA.Y, referenceB.Y, referenceC.Y, referenceD.Y, t);
					if (IsInnerNerveRegion(referenceX, referenceY) != innerRegion)
					{
						continue;
					}
					int x = (int)MathF.Round(SampleNerveCurve(a.X, b.X, c.X, d.X, t));
					int y = (int)MathF.Round(SampleNerveCurve(a.Y, b.Y, c.Y, d.Y, t));
					if (x > 3 && x < width - 4 && y > 3 && y < height - 4
						&& layout[x, y] is Jelly or Fresh)
					{
						nerves[x, y] = style;
					}
				}
			}
		}
		foreach (var branch in OuterNerveBranches)
		{
			Draw(branch, 2);
		}
		foreach (var trunk in OuterNerveTrunks)
		{
			Draw(trunk, 1);
		}
		foreach (var branch in InnerNerveBranches)
		{
			Draw(branch, 2, true);
		}
		foreach (var trunk in InnerNerveTrunks)
		{
			Draw(trunk, 1, true);
		}
		return nerves;
	}

	private static float SampleNerveCurve(float p0, float p1, float p2, float p3, float t)
	{
		return 0.5f * ((2 * p1) + (-p0 + p2) * t
			+ (2 * p0 - 5 * p1 + 4 * p2 - p3) * t * t + (-p0 + 3 * p1 - 3 * p2 + p3) * t * t * t);
	}
}

namespace Everglow.Yggdrasil.WorldGeneration;

internal sealed partial class JellyBallHotbedLayout
{
	private static readonly (float X, float Y)[][] OuterNerveTrunks =
	[
		[(266, 443), (281, 479), (283, 520), (284, 543), (269, 575), (269, 616), (257, 673), (240, 719), (220, 735), (205, 770), (220, 784)],
		[(348, 449), (350, 489), (370, 523), (393, 560), (371, 587), (354, 619), (358, 649), (341, 675), (330, 708), (318, 731), (324, 764), (347, 796), (350, 829)],
		[(558, 480), (574, 520), (576, 550), (558, 575), (540, 593)],
		[(220, 784), (244, 809), (273, 818), (297, 838), (312, 837), (324, 828), (340, 825), (350, 829), (363, 842), (373, 837), (390, 826)],
		[(220, 735), (190, 721), (158, 718), (125, 721), (116, 746), (118, 764)],
		[(619, 696), (608, 701), (598, 711), (588, 727), (572, 737), (556, 737), (545, 730), (535, 721), (521, 717), (506, 720)],
		[(350, 829), (353, 856), (337, 880), (342, 899)],
	];

	private static readonly (float X, float Y)[][] OuterNerveBranches =
	[
		[(281, 479), (285, 467), (292, 453), (298, 446)],
		[(284, 543), (302, 527), (315, 510)],
		[(284, 543), (259, 522), (239, 497), (234, 483)],
		[(318, 731), (303, 746), (301, 780)],
		[(576, 550), (597, 563), (621, 557), (647, 542), (657, 505)],
		[(540, 593), (534, 611), (537, 628)],
		[(615, 611), (603, 622), (600, 637)],
		[(535, 721), (548, 702), (543, 680), (551, 666)],
		[(618, 749), (629, 735), (636, 722), (638, 710)],
		[(618, 749), (628, 741), (644, 739), (656, 744)],
		[(662, 806), (674, 799), (684, 789), (692, 776), (696, 763), (701, 755)],
		[(680, 809), (684, 821), (684, 836), (679, 851)],
		[(177, 719), (182, 731), (176, 747), (153, 769), (147, 786), (147, 797), (151, 802)],
		[(205, 770), (185, 788), (164, 804), (151, 802)],
		[(244, 809), (237, 834), (252, 859)],
		[(273, 818), (268, 844), (284, 865), (300, 868)],
		[(297, 838), (311, 855), (313, 885)],
		[(324, 828), (322, 819), (326, 810), (336, 802)],
		[(363, 842), (369, 858), (380, 875), (398, 891)],
		[(390, 826), (393, 836), (396, 842), (400, 844)],
	];

	// 中央与右下参考中心线：离线细化后简化为曲线控制点，不保留笔画厚度。
	private static readonly (float X, float Y)[][] InnerNerveTrunks =
	[
		[(323, 714), (333, 695), (333, 685), (340, 676), (351, 674)],
		[(323, 714), (326, 726), (339, 744)],
		[(353, 470), (355, 484)],
		[(357, 664), (361, 647)],
		[(359, 618), (361, 609), (387, 574)],
		[(359, 623), (363, 640)],
		[(441, 698), (445, 686), (449, 668), (447, 652), (441, 639), (437, 634), (419, 612), (410, 600), (401, 588), (387, 574), (377, 549)],
		[(434, 825), (448, 839), (460, 849), (466, 857), (467, 874), (479, 890)],
		[(487, 804), (487, 818)],
		[(466, 857), (471, 837), (480, 824), (487, 818), (497, 816), (506, 814), (516, 812), (522, 810), (525, 805), (527, 801)],
		[(493, 733), (493, 720), (490, 709)],
		[(581, 815), (597, 838), (599, 848), (607, 852)],
		[(639, 798), (641, 808), (646, 820), (651, 827)],
	];

	private static readonly (float X, float Y)[][] InnerNerveBranches =
	[
		[(331, 660), (337, 639), (359, 621)],
		[(332, 724), (357, 720)],
		[(351, 468), (363, 435)],
		[(353, 514), (379, 548)],
		[(351, 674), (351, 692), (363, 714)],
		[(356, 667), (379, 684), (411, 692)],
		[(400, 844), (407, 844), (415, 848), (425, 853), (429, 861), (432, 872)],
		[(400, 830), (409, 839), (415, 848)],
		[(396, 893), (405, 881), (413, 867), (415, 858), (415, 848)],
		[(419, 817), (420, 829), (425, 842), (425, 853)],
		[(425, 842), (431, 841), (439, 835), (442, 832)],
		[(439, 640), (439, 619), (455, 591)],
		[(461, 815), (471, 824), (475, 829), (473, 837), (477, 856), (493, 873)],
		[(493, 858), (493, 827), (499, 820), (499, 804)],
		[(493, 873), (505, 886)],
		[(499, 862), (531, 880)],
		[(513, 814), (524, 827), (531, 841), (535, 852)],
		[(529, 806), (533, 812), (539, 816)],
		[(531, 880), (539, 898)],
		[(539, 816), (545, 819), (552, 829)],
		[(531, 880), (535, 856), (535, 852), (539, 842), (546, 835), (552, 829), (561, 824), (569, 813)],
		[(535, 856), (555, 870), (557, 876)],
		[(561, 824), (567, 880)],
		[(592, 808), (599, 800)],
		[(604, 803), (605, 822), (604, 835), (599, 848)],
	];

	// 参考图被空洞、核和分区拆开的笔画仍属于同一条曲线；显式补齐衔接，不能按端点距离乱连。
	private static readonly (float X, float Y)[][] NerveTrunkLinks =
	[
		[(323, 714), (319, 721), (318, 731)],
		[(349, 466), (353, 470)],
		[(359, 618), (359, 623)],
		[(363, 640), (361, 647)],
		[(357, 664), (356, 667), (353, 673), (351, 674)],
		[(339, 744), (347, 796)],
		[(390, 826), (414, 824), (434, 825)],
		[(445, 686), (451, 685), (459, 680), (470, 666), (480, 652), (492, 640), (505, 631), (514, 626)],
		[(491, 739), (493, 733), (500, 726), (506, 720)],
		[(576, 550), (593, 583), (615, 611), (625, 644), (627, 672), (619, 696), (613, 722), (618, 749), (617, 765), (625, 785), (639, 798), (650, 801), (662, 806), (680, 809), (705, 808), (716, 815), (721, 820)],
		[(611, 794), (615, 794), (618, 794), (626, 787)],
	];

	private static readonly (float X, float Y)[][] NerveBranchLinks =
	[
		// 短支沿原有切线接回邻近主脉，不扩宽笔画；空洞和核仍由材质遮罩截断。
		[(355, 484), (353, 514)],
		[(379, 548), (377, 549)],
		[(329, 726), (332, 724)],
		[(439, 640), (441, 639)],
		[(604, 803), (607, 797), (611, 794)],
		[(493, 858), (493, 864), (493, 873)],
		[(493, 858), (499, 862)],
		[(525, 805), (529, 806)],
		[(569, 813), (575, 811), (581, 815)],
		[(599, 800), (604, 803)],
		[(592, 808), (587, 812), (581, 815)],
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
				if (layout[x, y] == Nutrition)
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
		void Draw((float X, float Y)[] knots, byte style, bool innerRegion = false, bool constrainRegion = true)
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
					if (constrainRegion && IsInnerNerveRegion(referenceX, referenceY) != innerRegion)
					{
						continue;
					}
					int x = (int)MathF.Round(SampleNerveCurve(a.X, b.X, c.X, d.X, t));
					int y = (int)MathF.Round(SampleNerveCurve(a.Y, b.Y, c.Y, d.Y, t));
					if (x > 3 && x < width - 4 && y > 3 && y < height - 4
						&& layout[x, y] == Jelly)
					{
						nerves[x, y] = style;
					}
				}
			}
		}
		// 连接可跨越参考线分区，材质遮罩仍生效；原笔画最后绘制，保留交会处原有颜色。
		foreach (var branch in NerveBranchLinks)
		{
			Draw(branch, 2, constrainRegion: false);
		}
		foreach (var trunk in NerveTrunkLinks)
		{
			Draw(trunk, 1, constrainRegion: false);
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

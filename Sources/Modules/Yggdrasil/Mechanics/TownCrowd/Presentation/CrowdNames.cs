namespace Everglow.Yggdrasil.Mechanics.TownCrowd;

/// <summary>One stable localized identity per randomly seeded journey.</summary>
public static class CrowdNames
{
	private static readonly string[] Pool =
	[
		// Chinese: 58 names (25% of the pool).
		"ChenAn", "LinYue", "ZhouNing", "XuQing", "ShenXing", "SuHe", "LinAn", "ChenMo",
		"ShenQinghe", "XuXingyao", "OuyangMingyue", "SituQingfeng", "ZhaoYu", "QianSong", "SunZhu", "LiQuan",
		"ZhouLan", "WuTong", "ZhengChuan", "WangXi", "FengNing", "WeiLan", "JiangFeng", "HanLu",
		"YangFan", "ZhuChen", "QinLang", "YouXia", "HeQiu", "LyuYan", "ShiYu", "ZhangHe",
		"KongYue", "CaoXue", "YanShuang", "HuaXing", "JinHe", "TaoRan", "JiangLi", "GuZhou",
		"BaiRuoxi", "JiangYunshu", "YeTingyu", "FangZhiyuan", "SongWanqing", "TangShuning", "WenYian", "XieChangfeng",
		"ChengJingming", "ShaoQinghe", "XiaChuxue", "FuXinghe", "GuNanzhi", "MengXingyuan", "ShangguanYunshu", "ZhugeQinghe",
		"SimaXinghe", "XiahouMingxuan",

		// English: 58 names (25% of the pool).
		"Alden", "Mira", "Rowan", "Lyra", "Orin", "Elara", "Finn", "Nia",
		"Silas", "Iris", "Theo", "Luna", "Arlo", "Cora", "Jasper", "Faye",
		"Kai", "Asha", "Tarin", "Selene", "Bram", "Tessa", "Eira", "Ravi",
		"Adam", "Alex", "Arthur", "Asher", "Blake", "Caleb", "Colin", "Daniel",
		"David", "Dean", "Edgar", "Elliot", "Eric", "Ethan", "Henry", "Isaac",
		"Owen", "Ada", "Alice", "Ava", "Bella", "Daisy", "Eden", "Ella",
		"Emma", "Flora", "Grace", "Hazel", "Holly", "Ivy", "Lily", "Lucy",
		"Nora", "Ruby",

		// French: 29 names (12.5% of the pool).
		"Jules", "Luc", "Louis", "Remy", "Manon", "Ines", "Lise", "Chloe",
		"Noel", "Camille", "Elise", "Margot", "Sophie", "Jean", "Paul", "Marc",
		"Yves", "Guy", "Rene", "Lucie", "Adele", "Amelie", "Aline", "Celine",
		"Fanny", "Odile", "Ninon", "Sylvie", "Benoit",

		// German: 29 names (12.5% of the pool).
		"Hans", "Fritz", "Klaus", "Kurt", "Otto", "Greta", "Lotte", "Frieda",
		"Emil", "Felix", "Clara", "Lena", "Hugo", "Karl", "Max", "Franz",
		"Rolf", "Uwe", "Jens", "Jorg", "Gerd", "Ernst", "Heinz", "Inga",
		"Elke", "Anke", "Ute", "Rudi", "Hilda",

		// Japanese: 29 names (12.5% of the pool).
		"Haru", "Yuki", "Aki", "Rin", "Nao", "Mio", "Yui", "Riku",
		"Sora", "Ren", "Yuna", "Aoi", "Hana", "Hina", "Kiko", "Miki",
		"Nami", "Nana", "Rina", "Risa", "Ryo", "Rei", "Ken", "Shin",
		"Jun", "Taro", "Yuto", "Yuma", "Saki",

		// Russian: 29 names (12.5% of the pool).
		"Sasha", "Misha", "Dima", "Vanya", "Kolya", "Katya", "Nadya", "Zoya",
		"Vera", "Anna", "Anya", "Dasha", "Masha", "Tanya", "Sonya", "Olya",
		"Ira", "Liza", "Nina", "Rita", "Vika", "Yura", "Borya", "Petya",
		"Pasha", "Vova", "Kostya", "Tolya", "Grisha",
	];

	public static string KeyFor(int seed) => $"Mods.Everglow.TownCrowds.Names.{Pool[(uint)seed % Pool.Length]}";
}

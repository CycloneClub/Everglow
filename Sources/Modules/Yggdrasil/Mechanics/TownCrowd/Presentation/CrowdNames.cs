namespace Everglow.Yggdrasil.Mechanics.TownCrowd;

/// <summary>One stable localized identity per randomly seeded journey.</summary>
public static class CrowdNames
{
	private static readonly string[] Pool =
	[
		"Alden", "Mira", "Rowan", "Lyra", "Orin", "Elara", "Finn", "Nia",
		"Silas", "Iris", "Theo", "Luna", "Arlo", "Cora", "Jasper", "Vera",
		"Emil", "Faye", "Noel", "Sora", "Ren", "Yuna", "Lian", "Mei",
		"Kai", "Asha", "Tarin", "Selene", "Bram", "Tessa", "Eira", "Ravi",
		"ChenAn", "LinYue", "ZhouNing", "XuQing", "ShenXing", "SuHe",
		"LinAn", "ChenMo", "ShenQinghe", "XuXingyao", "OuyangMingyue", "SituQingfeng",
		"Aaron", "Adam", "Adrian", "Aiden", "Alan", "Albert", "Alex", "Alfred",
		"Arthur", "Asher", "August", "Austin", "Benjamin", "Blake", "Caleb", "Cedric",
		"Charles", "Colin", "Daniel", "David", "Dean", "Dorian", "Edgar", "Edmund",
		"Edward", "Elias", "Elliot", "Eric", "Ethan", "Felix", "Gabriel", "Gareth",
		"George", "Graham", "Hector", "Henry", "Hugo", "Isaac", "Julian", "Leo",
		"Leon", "Levi", "Liam", "Lucas", "Marcus", "Milo", "Nathan", "Oliver",
		"Oscar", "Owen", "Ada", "Alice", "Amelia", "Anna", "Aurora", "Ava",
		"Beatrice", "Bella", "Camille", "Celia", "Clara", "Daisy", "Daphne", "Diana",
		"Eden", "Elise", "Ella", "Elsie", "Emma", "Estelle", "Eva", "Evelyn",
		"Flora", "Freya", "Grace", "Hazel", "Helena", "Holly", "Isabel", "Ivy",
		"Julia", "Lena", "Lily", "Lucia", "Lucy", "Maeve", "Margot", "Maya",
		"Nora", "Olive", "Opal", "Pearl", "Phoebe", "Rose", "Ruby", "Sadie",
		"Sophie", "Stella", "Violet", "Willow", "ZhaoYu", "QianSong", "SunZhu", "LiQuan",
		"ZhouLan", "WuTong", "ZhengChuan", "WangXi", "FengNing", "WeiLan", "JiangFeng", "HanLu",
		"YangFan", "ZhuChen", "QinLang", "YouXia", "HeQiu", "LyuYan", "ShiYu", "ZhangHe",
		"KongYue", "CaoXue", "YanShuang", "HuaXing", "JinHe", "TaoRan", "JiangLi", "GuZhou",
		"BaiRuoxi", "JiangYunshu", "YeTingyu", "FangZhiyuan", "SongWanqing", "TangShuning", "WenYian", "XieChangfeng",
		"ChengJingming", "ShaoQinghe", "XiaChuxue", "FuXinghe", "XuWangshan", "LinJianlu", "ChenYingyue", "ShenGuizhou",
		"LuHuaijin", "SuJinnian", "ZhouTinglan", "QinMobai", "LiuRuyan", "YunZhixia", "GuNanzhi", "MengXingyuan",
		"ShangguanYunshu", "ZhugeQinghe", "SimaXinghe", "XiahouMingxuan",
		"Jules", "Luc", "Louis", "Remy", "Manon", "Ines", "Lise", "Chloe",
		"Hans", "Fritz", "Klaus", "Kurt", "Otto", "Greta", "Lotte", "Frieda",
		"Haru", "Yuki", "Aki", "Rin", "Nao", "Mio", "Yui", "Riku",
		"Sasha", "Misha", "Dima", "Vanya", "Kolya", "Katya", "Nadya", "Zoya",
	];

	public static string KeyFor(int seed) => $"Mods.Everglow.TownCrowds.Names.{Pool[(uint)seed % Pool.Length]}";
}

namespace Everglow.Commons.Utilities;

public static partial class NPCUtils
{
	public static List<int> VanillaBloodMoonNPCs { get; private set; } =
	[
		#region 困难模式前
		// 腐化兔兔
		NPCID.CorruptBunny,

		// 骷髅博士
		NPCID.DoctorBones,

		// 僵尸新郎
		NPCID.TheGroom,

		// 腐化金鱼
		NPCID.CorruptGoldfish,

		// 腐化企鹅
		NPCID.CorruptPenguin,

		// 僵尸
		NPCID.PincushionZombie,

		// 僵尸
		NPCID.SlimedZombie,

		// 僵尸
		NPCID.SwampZombie,

		// 僵尸
		NPCID.TwiggyZombie,

		// 恶魔眼
		NPCID.CataractEye,

		NPCID.SleepyEye,

		NPCID.DialatedEye,

		NPCID.GreenEye,

		NPCID.PurpleEye,

		// 鬼魂
		NPCID.Ghost,

		// 恶魔眼
		NPCID.DemonEyeOwl,

		NPCID.DemonEyeSpaceship,

		// 僵尸
		NPCID.ZombieDoctor,

		NPCID.ZombieSuperman,

		NPCID.ZombiePixie,

		// 骷髅
		NPCID.SkeletonTopHat,

		NPCID.SkeletonAstonaut,

		NPCID.SkeletonAlien,

		// 僵尸
		NPCID.ZombieXmas,

		NPCID.ZombieSweater,

		// 毒兔兔
		NPCID.CrimsonBunny,

		// 毒金鱼
		NPCID.CrimsonGoldfish,

		// 毒企鹅
		NPCID.CrimsonPenguin,

		// 血腥僵尸
		NPCID.BloodZombie,

		// 滴滴怪
		NPCID.Drippler,

		// 游荡眼球怪鱼
		NPCID.EyeballFlyingFish,

		// 僵尸
		NPCID.TorchZombie,
		#endregion

		#region 困难模式后
		// 小丑
		NPCID.Clown,

		// 嗒嗒牙齿炸弹
		NPCID.ChatteringTeethBomb,

		// 僵尸
		NPCID.ArmedZombie,

		// 冰冻僵尸
		NPCID.ArmedZombieEskimo,

		// 僵尸
		NPCID.ArmedZombiePincussion,

		NPCID.ArmedZombieSwamp,

		NPCID.ArmedZombieTwiggy,

		NPCID.ArmedZombieCenx,

		NPCID.ArmedTorchZombie,

		// 恐惧鹦鹉螺
		NPCID.BloodNautilus,

		// 血乌贼
		NPCID.BloodSquid,

		// 血浆哥布林鲨鱼
		NPCID.GoblinShark,

		// 血鳗鱼
		NPCID.BloodEelHead,

		NPCID.BloodEelBody,

		NPCID.BloodEelTail,
		#endregion
	];

	public static List<int> VanillaEclipseNPCs { get; private set; } =
	[
		// 眼怪
		NPCID.Eyezor,

		// 死神
		NPCID.Reaper,

		// 科学怪人
		NPCID.Frankenstein,

		// 沼泽怪
		NPCID.SwampThing,

		// 吸血鬼
		NPCID.Vampire,

		NPCID.VampireBat,

		// 屠夫
		NPCID.Butcher,

		// 水月怪
		NPCID.CreatureFromTheDeep,

		// 弗里茨
		NPCID.Fritz,

		// 钉头
		NPCID.Nailhead,

		// 变态人
		NPCID.Psycho,

		// 致命球
		NPCID.DeadlySphere,

		// 苍蝇人博士
		NPCID.DrManFly,

		// 攀爬魔
		NPCID.ThePossessed,

		// 蛾怪
		NPCID.Mothron,

		// 蛾怪蛋
		NPCID.MothronEgg,

		// 蛾怪宝宝
		NPCID.MothronSpawn,
	];

	public static List<int> VanillaFrostMoonNPCs { get; private set; } =
	[
		// 僵尸精灵
		NPCID.ZombieElf,

		NPCID.ZombieElfBeard,

		NPCID.ZombieElfGirl,

		// 礼物宝箱怪
		NPCID.PresentMimic,

		// 姜饼人
		NPCID.GingerbreadMan,

		// 雪兽
		NPCID.Yeti,

		// 常绿尖叫怪
		NPCID.Everscream,

		// 冰雪女王
		NPCID.IceQueen,

		// 圣诞坦克
		NPCID.SantaNK1,

		// 精灵直升机
		NPCID.ElfCopter,

		// 胡桃夹士
		NPCID.Nutcracker,

		NPCID.NutcrackerSpinning,

		// 精灵弓箭手
		NPCID.ElfArcher,

		// 坎卜斯
		NPCID.Krampus,

		// 雪花怪
		NPCID.Flocko,
	];

	public static List<int> VanillaPumpkinMoonNPCs { get; private set; } =
	[
		// 稻草人
		NPCID.Scarecrow1,

		NPCID.Scarecrow2,

		NPCID.Scarecrow3,

		NPCID.Scarecrow4,

		NPCID.Scarecrow5,

		NPCID.Scarecrow6,

		NPCID.Scarecrow7,

		NPCID.Scarecrow8,

		NPCID.Scarecrow9,

		NPCID.Scarecrow10,

		// 无头骑士
		NPCID.HeadlessHorseman,

		// 哀木
		NPCID.MourningWood,

		// 树精
		NPCID.Splinterling,

		// 南瓜王
		NPCID.Pumpking,

		NPCID.PumpkingBlade,

		// 地狱犬
		NPCID.Hellhound,

		// 胡闹鬼
		NPCID.Poltergeist,
	];
}

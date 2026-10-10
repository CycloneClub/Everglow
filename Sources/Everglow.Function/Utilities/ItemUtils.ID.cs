namespace Everglow.Commons.Utilities;

public static partial class ItemUtils
{
	public static List<int> VanillaDuck => RecipeGroups.Ducks.ValidItems.ToList();

	public static List<int> VanillaButterfly => RecipeGroups.Butterflies.ValidItems.ToList();

	public static List<int> VanillaFruit => RecipeGroups.Fruit.ValidItems.Concat([ItemID.Grapes]).ToList();

	public static List<int> VanillaTurtle => RecipeGroups.Turtles.ValidItems.ToList();

	public static List<int> VanillaBug => RecipeGroups.Bugs.ValidItems.ToList();

	public static List<int> VanillaSquirrel => RecipeGroups.Squirrels.ValidItems.ToList();

	public static List<int> VanillaDragonfly => RecipeGroups.Dragonflies.ValidItems.ToList();

	public static List<int> VanillaSnail => RecipeGroups.Snails.ValidItems.ToList();

	public static List<int> VanillaFirefly => RecipeGroups.Fireflies.ValidItems.ToList();

	public static List<int> VanillaScorpion => RecipeGroups.Scorpions.ValidItems.ToList();

	public static List<int> VanillaParrot => RecipeGroups.Cockatiels.ValidItems.Concat(RecipeGroups.Macaws.ValidItems).ToList();

	public static List<int> VanillaBird => RecipeGroups.Birds.ValidItems.Concat(VanillaParrot).ToList();

	public static List<int> VanillaQuestFish => Main.anglerQuestItemNetIDs.ToList();

	public static List<int> VanillaSpecialFish => [
		ItemID.ArmoredCavefish,
		ItemID.ChaosFish,
		ItemID.CrimsonTigerfish,
		ItemID.Damselfish,
		ItemID.DoubleCod,
		ItemID.Ebonkoi,
		ItemID.FlarefinKoi,
		ItemID.FrostMinnow,
		ItemID.Hemopiranha,
		ItemID.Honeyfin,
		ItemID.NeonTetra,
		ItemID.Obsidifish,
		ItemID.PrincessFish,
		ItemID.Prismite,
		ItemID.SpecularFish,
		ItemID.Stinkfish,
		ItemID.VariegatedLardfish,
	];

	public static List<int> VanillaNormalFish => [
		ItemID.AtlanticCod,
		ItemID.Bass,
		ItemID.Flounder,
		ItemID.RedSnapper,
		ItemID.Salmon,
		ItemID.Trout,
		ItemID.Tuna,
	];

	public static List<int> VanillaFish => VanillaQuestFish.Concat(VanillaSpecialFish).Concat(VanillaNormalFish).ToList();

	public static class Melee
	{
		public static List<int> Boomerangs { get; private set; } = new List<int>
		{
			// 木回旋镖
			ItemID.WoodenBoomerang,

			// 附魔回旋镖
			ItemID.EnchantedBoomerang,

			// 水果蛋糕旋刃
			ItemID.FruitcakeChakram,

			// 血腥砍刀
			ItemID.BloodyMachete,

			// 蘑菇回旋镖
			ItemID.Shroomerang,

			// 冰雪回旋镖
			ItemID.IceBoomerang,

			// 荆棘旋刃
			ItemID.ThornChakram,

			// 战斗扳手
			ItemID.CombatWrench,

			// 烈焰回旋镖
			ItemID.Flamarang,

			// 三尖回旋镖
			ItemID.Trimarang,

			// 飞刀
			ItemID.FlyingKnife,

			// 中士联盾
			ItemID.BouncingShield,

			// 光辉飞盘
			ItemID.LightDisc,

			// 香蕉回旋镖
			ItemID.Bananarang,

			// 疯狂飞斧
			ItemID.PossessedHatchet,

			// 圣骑士锤
			ItemID.PaladinsHammer,
		};

		public static List<int> BroadSwords { get; private set; } = new List<int>
		{
			// 木剑
			ItemID.WoodenSword,

			// 针叶木剑
			ItemID.BorealWoodSword,

			// 棕榈木剑
			ItemID.PalmWoodSword,

			// 红木剑
			ItemID.RichMahoganySword,

			// 乌木剑
			ItemID.EbonwoodSword,

			// 暗影木剑
			ItemID.ShadewoodSword,

			// 珍珠木剑
			ItemID.PearlwoodSword,

			// 仙人掌剑
			ItemID.CactusSword,

			// 铜阔剑
			ItemID.CopperBroadsword,

			// 锡阔剑
			ItemID.TinBroadsword,

			// 铁阔剑
			ItemID.IronBroadsword,

			// 铅阔剑
			ItemID.LeadBroadsword,

			// 银阔剑
			ItemID.SilverBroadsword,

			// 钨阔剑
			ItemID.TungstenBroadsword,

			// 金阔剑
			ItemID.GoldBroadsword,

			// 铂金阔剑
			ItemID.PlatinumBroadsword,

			// 利刃手套
			ItemID.BladedGlove,

			// 僵尸手臂
			ItemID.ZombieArm,

			// 时尚剪刀
			ItemID.StylistKilLaKillScissorsIWish,

			// 芦苇管
			ItemID.BreathingReed,

			// 骨剑
			ItemID.BoneSword,

			// 糖棒剑
			ItemID.CandyCaneSword,

			// 武士刀
			ItemID.Katana,

			// 冰雪刃
			ItemID.IceBlade,

			// 魔光剑
			ItemID.LightsBane,

			// 血腥屠刀
			ItemID.BloodButcherer,

			// 附魔剑
			ItemID.EnchantedSword,

			// 异域弯刀
			ItemID.DyeTradersScimitar,

			// 陨石光剑
			ItemID.BluePhaseblade,

			ItemID.GreenPhaseblade,

			ItemID.OrangePhaseblade,

			ItemID.PurplePhaseblade,

			ItemID.RedPhaseblade,

			ItemID.WhitePhaseblade,

			ItemID.YellowPhaseblade,

			// 养蜂人
			ItemID.BeeKeeper,

			// 草剑
			ItemID.BladeofGrass,

			// 炽焰巨剑
			ItemID.FieryGreatsword,

			// 猎鹰刃
			ItemID.FalconBlade,

			// 村正
			ItemID.Muramasa,

			// 永夜刃
			ItemID.NightsEdge,

			// 蝙蝠棍
			ItemID.BatBat,

			// 触手钉锤
			ItemID.TentacleSpike,

			// 钴剑
			ItemID.CobaltSword,

			// 钯金剑
			ItemID.PalladiumSword,

			// 秘银剑
			ItemID.MythrilSword,

			// 山铜剑
			ItemID.OrichalcumSword,

			// 精金剑
			ItemID.AdamantiteSword,

			// 钛金剑
			ItemID.TitaniumSword,

			// 晶光刃
			ItemID.BluePhasesaber,

			ItemID.RedPhasesaber,

			ItemID.GreenPhasesaber,

			ItemID.PurplePhasesaber,

			ItemID.WhitePhasesaber,

			ItemID.YellowPhasesaber,

			// 精致手杖
			ItemID.TaxCollectorsStickOfDoom,

			// 拍拍手
			ItemID.SlapHand,

			// 毁灭刃
			ItemID.BreakerBlade,

			// 海盗弯刀
			ItemID.Cutlass,

			// 霜印剑
			ItemID.Frostbrand,

			// 光束剑
			ItemID.BeamSword,

			// 舌锋剑
			ItemID.Bladetongue,

			// 臭虎爪
			ItemID.FetidBaghnakhs,

			// 火腿棍
			ItemID.HamBat,

			// 圣剑
			ItemID.Excalibur,

			// 华夫饼烘烤模
			ItemID.WaffleIron,

			// 真圣剑
			ItemID.TrueExcalibur,

			// 真永夜
			ItemID.TrueNightsEdge,

			// 叶绿军刀
			ItemID.ChlorophyteSaber,

			// 叶绿双刃刀
			ItemID.ChlorophyteClaymore,

			// 变态刀
			ItemID.PsychoKnife,

			// 死神镰刀
			ItemID.DeathSickle,

			// 钥匙剑
			ItemID.Keybrand,

			// 无头骑士剑
			ItemID.TheHorsemansBlade,

			// 圣诞树剑
			ItemID.ChristmasTreeSword,

			// 种子弯刀
			ItemID.Seedler,

			// 飞龙
			ItemID.DD2SquireBetsySword,

			// 泰拉刃
			ItemID.TerraBlade,

			// 波涌之刃
			ItemID.InfluxWaver,

			// 狂星之怒
			ItemID.StarWrath,

			// 彩虹猫之刃
			ItemID.Meowmere,
		};

		public static List<int> Flails { get; private set; } = new List<int>
		{
			// 链刀
			ItemID.ChainKnife,

			// 链锤
			ItemID.Mace,

			// 烈焰链锤
			ItemID.FlamingMace,

			// 链球
			ItemID.BallOHurt,

			// 血肉之球
			ItemID.TheMeatball,

			// 蓝月
			ItemID.BlueMoon,

			// 阳炎之怒
			ItemID.Sunfury,

			// 锚
			ItemID.Anchor,

			// 致胜炮
			ItemID.KOCannon,

			// 滴滴怪致残者
			ItemID.DripplerFlail,

			// 铁链血滴子
			ItemID.ChainGuillotines,

			// 太极连枷
			ItemID.DaoofPow,

			// 花冠
			ItemID.FlowerPow,

			// 石巨人之拳
			ItemID.GolemFist,

			// 猪鲨链球
			ItemID.Flairon,
		};

		public static List<int> ShortSwords { get; private set; } = new List<int>
		{
			// 铜短剑
			ItemID.CopperShortsword,

			// 锡短剑
			ItemID.TinShortsword,

			// 铁短剑
			ItemID.IronShortsword,

			// 铅短剑
			ItemID.LeadShortsword,

			// 银短剑
			ItemID.SilverShortsword,

			// 钨短剑
			ItemID.TungstenShortsword,

			// 金短剑
			ItemID.GoldShortsword,

			// 铂金短剑
			ItemID.PlatinumShortsword,

			// 伞
			ItemID.Umbrella,

			// 悲剧雨伞
			ItemID.TragicUmbrella,

			// 标尺
			ItemID.Ruler,

			// 罗马短剑
			ItemID.Gladius,
		};

		public static List<int> Spears { get; private set; } = new()
		{
			// 长矛
			ItemID.Spear,

			// 三叉戟
			ItemID.Trident,

			// 风暴长矛
			ItemID.ThunderSpear,

			// 腐叉
			ItemID.TheRottedFork,

			// 剑鱼
			ItemID.Swordfish,

			// 暗黑长枪
			ItemID.DarkLance,

			// 钴薙刀
			ItemID.CobaltNaginata,

			// 钯金刺矛
			ItemID.PalladiumPike,

			// 秘银长戟
			ItemID.MythrilHalberd,

			// 山铜长戟
			ItemID.OrichalcumHalberd,

			// 精金关刀
			ItemID.AdamantiteGlaive,

			// 钛金三叉戟
			ItemID.TitaniumTrident,

			// 永恒之枪
			ItemID.Gungnir,

			// 恐怖关刀
			ItemID.MonkStaffT2,

			// 叶绿镋
			ItemID.ChlorophytePartisan,

			// 蘑菇长矛
			ItemID.MushroomSpear,

			// 黑曜石剑鱼
			ItemID.ObsidianSwordfish,

			// 北极
			ItemID.NorthPole,
		};

		public static List<int> Yoyos { get; private set; } = new List<int>
		{
			// 木悠悠球
			ItemID.WoodYoyo,

			// 对打球
			ItemID.Rally,

			// 抑郁球
			ItemID.CorruptYoyo,

			// 血脉球
			ItemID.CrimsonYoyo,

			// 亚马逊球
			ItemID.JungleYoyo,

			// 代码1球
			ItemID.Code1,

			// 英勇球
			ItemID.Valor,

			// 喷流球
			ItemID.Cascade,

			// 蜂巢球
			ItemID.HiveFive,

			// 好胜球
			ItemID.FormatC,

			// 渐变球
			ItemID.Gradient,

			// 吉克球
			ItemID.Chik,

			// 狱火球
			ItemID.HelFire,

			// 冰雪悠悠球
			ItemID.Amarok,

			// 代码2球
			ItemID.Code2,

			// 叶列茨球
			ItemID.Yelets,

			// Red的抛球
			ItemID.RedsYoyo,

			// 女武神悠悠球
			ItemID.ValkyrieYoyo,

			// 克拉肯球
			ItemID.Kraken,

			// 克苏鲁之眼
			ItemID.TheEyeOfCthulhu,

			// 泰拉悠悠球
			ItemID.Terrarian,
		};

		public static List<int> OtherMeleeWeapons { get; private set; } = new List<int>
		{
			// 泰拉魔刃
			ItemID.Terragrim,

			// Arkhalis剑
			ItemID.Arkhalis,

			// 骑枪
			ItemID.JoustingLance,

			// 暗影焰刀
			ItemID.ShadowFlameKnife,

			// 神圣骑枪
			ItemID.HallowJoustingLance,

			// 瞌睡章鱼
			ItemID.MonkStaffT1,

			// 腐化者之戟
			ItemID.ScourgeoftheCorruptor,

			// 暗影骑枪
			ItemID.ShadowJoustingLance,

			// 吸血鬼刀
			ItemID.VampireKnives,

			// 星光
			ItemID.PiercingStarlight,

			// 天龙之怒
			ItemID.MonkStaffT3,

			// 破晓之光
			ItemID.DayBreak,

			// 日耀喷发剑
			ItemID.SolarEruption,

			// 天顶剑
			ItemID.Zenith,
		};
	}

	public static class Ranged
	{
		public static List<int> Bows { get; private set; } = new List<int>()
		{
			// 木弓
			ItemID.WoodenBow,

			// 针叶木弓
			ItemID.BorealWoodBow,

			// 棕榈木弓
			ItemID.PalmWoodBow,

			// 红木弓
			ItemID.RichMahoganyBow,

			// 乌木弓
			ItemID.EbonwoodBow,

			// 暗影木弓
			ItemID.ShadewoodBow,

			 // 珍珠木弓
			ItemID.PearlwoodBow,

			// 铜弓
			ItemID.CopperBow,

			// 锡弓
			ItemID.TinBow,

			// 铁弓
			ItemID.IronBow,

			// 铅弓
			ItemID.LeadBow,

			// 银弓
			ItemID.SilverBow,

			// 钨弓
			ItemID.TungstenBow,

			// 金弓
			ItemID.GoldBow,

			// 铂金弓
			ItemID.PlatinumBow,

			// 恶魔弓
			ItemID.DemonBow,

			// 肌腱弓
			ItemID.TendonBow,

			// 血雨弓
			ItemID.BloodRainBow,

			// 熔火之弓
			ItemID.MoltenFury,

			// 蜂膝弓
			ItemID.BeesKnees,

			// 地狱之翼弓
			ItemID.HellwingBow,

			// 骨弓
			ItemID.Marrow,

			// 冰霜弓
			ItemID.IceBow,

			// 代达罗斯风暴弓
			ItemID.DaedalusStormbow,

			// 暗影焰弓
			ItemID.ShadowFlameBow,

			// 幽灵凤凰
			ItemID.DD2PhoenixBow,

			// 脉冲弓
			ItemID.PulseBow,

			// 空中祸害
			ItemID.DD2BetsyBow,

			// 海啸
			ItemID.Tsunami,

			// 日暮
			ItemID.FairyQueenRangedItem,

			// 幻象
			ItemID.Phantasm,
		};

		public static List<int> Consumables { get; private set; } = new List<int>
		{
            // 纸飞机
            ItemID.PaperAirplaneA,

            // 白纸飞机
            ItemID.PaperAirplaneB,

            // 手里剑
            ItemID.Shuriken,

            // 投刀
            ItemID.ThrowingKnife,

            // 毒刀
            ItemID.PoisonedKnife,

            // 雪球
            ItemID.Snowball,

            // 尖球
            ItemID.SpikyBall,

            // 骨头
            ItemID.Bone,

            // 臭蛋
            ItemID.RottenEgg,

            // 星形茴香
            ItemID.StarAnise,

            // 莫洛托夫鸡尾酒
            ItemID.MolotovCocktail,

            // 寒霜飞鱼
            ItemID.FrostDaggerfish,

            // 标枪
            ItemID.Javelin,

            // 骨头标枪
            ItemID.BoneJavelin,

            // 骨投刀
            ItemID.BoneDagger,

            // 手榴弹
            ItemID.Grenade,

            // 粘性手榴弹
            ItemID.StickyGrenade,

            // 弹力手榴弹
            ItemID.BouncyGrenade,

            // 蜜蜂手榴弹
            ItemID.Beenade,

            // 快乐手榴弹
            ItemID.PartyGirlGrenade,
		};

		public static List<int> Handguns { get; private set; } = new List<int>()
		{
			// 燧发枪
			ItemID.FlintlockPistol,

			// 夺命枪
			ItemID.TheUndertaker,

			// 左轮手枪
			ItemID.Revolver,

			// 手枪
			ItemID.Handgun,

			// 凤凰爆破枪
			ItemID.PhoenixBlaster,

			// 气喇叭
			ItemID.PewMaticHorn,

			// 维纳斯万能枪
			ItemID.VenusMagnum,
		};

		public static List<int> Launchers { get; private set; } = new List<int>()
		{
			// 榴弹发射器
			ItemID.GrenadeLauncher,

			// 感应雷发射器
			ItemID.ProximityMineLauncher,

			// 火箭发射器
			ItemID.RocketLauncher,

			// 雪人炮
			ItemID.SnowmanCannon,

			// 喜庆弹射器
			ItemID.FireworksLauncher,

			// 电圈发射器
			ItemID.ElectrosphereLauncher,

			// 喜庆弹射器Mk2
			ItemID.Celeb2,

			// 尖桩发射器
			ItemID.StakeLauncher,
		};

		public static List<int> LongGuns { get; private set; } = new List<int>
		{
			// 红莱德枪
			ItemID.RedRyder,

			// 火枪
			ItemID.Musket,

			// 狙击枪
			ItemID.SniperRifle,
		};

		public static List<int> MachineGuns { get; private set; } = new List<int>
		{
			// 迷你鲨
			ItemID.Minishark,

			// 链式发条步枪
			ItemID.ClockworkAssaultRifle,

			// 鳄式机枪
			ItemID.Gatligator,

			// 乌兹冲锋枪
			ItemID.Uzi,

			// 巨兽鲨
			ItemID.Megashark,

			// 链式机枪
			ItemID.ChainGun,

			// 星璇机枪
			ItemID.VortexBeater,

			// 太空海豚机枪
			ItemID.SDMG,
		};

		public static List<int> Repeaters { get; private set; } = new List<int>()
		{
			// 钴弩
			ItemID.CobaltRepeater,

			// 钯金弩
			ItemID.PalladiumRepeater,

			// 秘银弩
			ItemID.MythrilRepeater,

			// 山铜弩
			ItemID.OrichalcumRepeater,

			// 精金弩
			ItemID.AdamantiteRepeater,

			// 钛金弩
			ItemID.TitaniumRepeater,

			// 神圣弩
			ItemID.HallowedRepeater,

			// 叶绿弩
			ItemID.ChlorophyteShotbow,
		};

		public static List<int> Shotguns { get; private set; } = new List<int>
		{
			// 三发猎枪
			ItemID.Boomstick,

			// 四管霰弹枪
			ItemID.QuadBarrelShotgun,

			// 霰弹枪
			ItemID.Shotgun,

			// 玛瑙爆破枪
			ItemID.OnyxBlaster,

			// 战术霰弹枪
			ItemID.TacticalShotgun,

			// 外星霰弹枪
			ItemID.Xenopopper,
		};

		public static List<int> OtherRangedWeapons { get; private set; } = new List<int>
		{
			// 信号枪
			ItemID.FlareGun,

			// 麦芽酒投掷器
			ItemID.AleThrowingGlove,

			// 吹管
			ItemID.Blowpipe,

			// 吹枪
			ItemID.Blowgun,

			// 雪球炮
			ItemID.SnowballCannon,

			// 彩弹枪
			ItemID.PainterPaintballGun,

			// 鱼叉枪
			ItemID.Harpoon,

			// 300颗
			ItemID.StarCannon,

			// 毒液枪
			ItemID.Toxikarp,

			// 飞镖手枪
			ItemID.DartPistol,

			// 飞镖步枪
			ItemID.DartRifle,

			// 火焰喷射器
			ItemID.Flamethrower,

			// 水虎鱼枪
			ItemID.PiranhaGun,

			// 精灵熔炉
			ItemID.ElfMelter,

			// 超级300颗
			ItemID.SuperStarCannon,

			// 钉枪
			ItemID.NailGun,

			// 毒刺发射器
			ItemID.Stynger,

			// 杰克南瓜灯发射器
			ItemID.JackOLanternLauncher,
		};
	}

	public static class Magic
	{
		public static List<int> MagicBooks { get; private set; } = new List<int>()
		{
			// 水箭
			ItemID.WaterBolt,

			// 骷髅头法术
			ItemID.BookofSkulls,

			// 恶魔镰刀
			ItemID.DemonScythe,

			// 咒焰
			ItemID.CursedFlames,

			// 黄金尿
			ItemID.GoldenShower,

			// 水晶风暴
			ItemID.CrystalStorm,

			// 磁球
			ItemID.MagnetSphere,

			// 利刃台风
			ItemID.RazorbladeTyphoon,

			// 月耀
			ItemID.LunarFlareBook,
		};

		public static List<int> MagicGuns { get; private set; } = new List<int>()
		{
			// 太空枪
			ItemID.SpaceGun,

			// 蜜蜂枪
			ItemID.BeeGun,

			// 灰冲击枪
			ItemID.ZapinatorGray,

			// 激光步枪
			ItemID.LaserRifle,

			// 橙冲击枪
			ItemID.ZapinatorOrange,

			// 吹叶机
			ItemID.LeafBlower,

			// 彩虹枪
			ItemID.RainbowGun,

			// 胡蜂枪
			ItemID.WaspGun,

			// 高温射线枪
			ItemID.HeatRay,

			// 激光机枪
			ItemID.LaserMachinegun,

			// 充能爆破炮
			ItemID.ChargedBlasterCannon,

			// 泡泡枪
			ItemID.BubbleGun,
		};

		public static List<int> Wands { get; private set; } = new List<int>()
		{
			// 火花魔棒
			ItemID.WandofSparking,

			// 结霜魔杖
			ItemID.WandofFrosting,

			// 霹雳法杖
			ItemID.ThunderStaff,

			// 紫晶法杖
			ItemID.AmethystStaff,

			// 黄玉法杖
			ItemID.TopazStaff,

			// 蓝玉法杖
			ItemID.SapphireStaff,

			// 翡翠法杖
			ItemID.EmeraldStaff,

			// 红玉法杖
			ItemID.RubyStaff,

			// 钻石法杖
			ItemID.DiamondStaff,

			// 琥珀法杖
			ItemID.AmberStaff,

			// 魔刺
			ItemID.Vilethorn,

			// 天候棒
			ItemID.WeatherPain,

			// 魔法导弹
			ItemID.MagicMissile,

			// 海蓝权杖
			ItemID.AquaScepter,

			// 烈焰火鞭
			ItemID.Flamelash,

			// 火之花
			ItemID.FlowerofFire,

			// 寒霜之花
			ItemID.FlowerofFrost,

			// 裂天剑
			ItemID.SkyFracture,

			// 水晶蛇
			ItemID.CrystalSerpent,

			// 寒霜法杖
			ItemID.FrostStaff,

			// 魔晶碎块
			ItemID.CrystalVileShard,

			// 夺命杖
			ItemID.SoulDrain,

			// 流星法杖
			ItemID.MeteorStaff,

			// 剧毒法杖
			ItemID.PoisonStaff,

			// 彩虹魔杖
			ItemID.RainbowRod,

			// 邪恶三叉戟
			ItemID.UnholyTrident,

			// 无限智慧巨著
			ItemID.BookStaff,

			// 毒液法杖
			ItemID.VenomStaff,

			// 爆裂藤蔓
			ItemID.NettleBurst,

			// 蝙蝠权杖
			ItemID.BatScepter,

			// 暴雪法杖
			ItemID.BlizzardStaff,

			// 狱火叉
			ItemID.InfernoFork,

			// 暗影束法杖
			ItemID.ShadowbeamStaff,

			// 幽灵法杖
			ItemID.SpectreStaff,

			// 共鸣权杖
			ItemID.PrincessWeapon,

			// 剃刀松
			ItemID.Razorpine,

			// 大地法杖
			ItemID.StaffofEarth,

			// 双足翼龙怒气
			ItemID.ApprenticeStaffT3,
		};

		public static List<int> OtherMagicWeapons { get; private set; } = new List<int>()
		{
			// 魔法飞刀
			ItemID.MagicDagger,

			// 蛇发女妖头
			ItemID.MedusaHead,

			// 神灯烈焰
			ItemID.SpiritFlame,

			// 暗影焰妖娃
			ItemID.ShadowFlameHexDoll,

			// 血荆棘
			ItemID.SharpTears,

			// 魔法竖琴
			ItemID.MagicalHarp,

			// 毒气瓶
			ItemID.ToxicFlask,

			// 夜光
			ItemID.FairyQueenMagicItem,

			// 星星吉他
			ItemID.SparkleGuitar,

			// 星云奥秘
			ItemID.NebulaArcanum,

			// 星云烈焰
			ItemID.NebulaBlaze,

			// 终极棱镜
			ItemID.LastPrism,

			// 血雨法杖
			ItemID.CrimsonRod,

			// 冰雪魔杖
			ItemID.IceRod,

			// 爬藤怪法杖
			ItemID.ClingerStaff,

			// 雨云魔杖
			ItemID.NimbusRod,
		};
	}

	public static class Summon
	{
		public static List<int> Minions { get; private set; } = new List<int>()
		{
			// 雀杖
			ItemID.BabyBirdStaff,

			// 史莱姆法杖
			ItemID.SlimeStaff,

			// 小雪怪法杖
			ItemID.FlinxStaff,

			// 黄蜂法杖
			ItemID.HornetStaff,

			// 阿比盖尔的花
			ItemID.AbigailsFlower,

			// 吸血鬼青蛙法杖
			ItemID.VampireFrogStaff,

			// 小鬼法杖
			ItemID.ImpStaff,

			// 刃杖
			ItemID.Smolstar,

			// 蜘蛛法杖
			ItemID.SpiderStaff,

			// 海盗法杖
			ItemID.PirateStaff,

			// 血红法杖
			ItemID.SanguineStaff,

			// 魔眼法杖
			ItemID.OpticStaff,

			// 致命球法杖
			ItemID.DeadlySphereStaff,

			// 矮人法杖
			ItemID.PygmyStaff,

			// 乌鸦法杖
			ItemID.RavenStaff,

			// 沙漠虎杖
			ItemID.StormTigerStaff,

			// 暴风雨法杖
			ItemID.TempestStaff,

			// 泰拉棱镜
			ItemID.EmpressBlade,

			// 外星法杖
			ItemID.XenoStaff,

			// 星尘细胞法杖
			ItemID.StardustCellStaff,

			// 星尘之龙法杖
			ItemID.StardustDragonStaff,
		};

		public static List<int> Sentrys { get; private set; } = new List<int>()
		{
			#region 撒旦军团T1
			// 闪电光环魔杖
			ItemID.DD2LightningAuraT1Popper,

			// 爆炸烈焰魔杖
			ItemID.DD2FlameburstTowerT1Popper,

			// 爆炸机关魔杖
			ItemID.DD2ExplosiveTrapT1Popper,

			// 弩车魔杖
			ItemID.DD2BallistraTowerT1Popper,
			#endregion

			#region 撒旦军团T2
			// 闪电光环手杖
			ItemID.DD2LightningAuraT2Popper,

			// 爆炸烈焰手杖
			ItemID.DD2FlameburstTowerT2Popper,

			// 爆炸机关手杖
			ItemID.DD2ExplosiveTrapT2Popper,

			// 弩车手杖
			ItemID.DD2BallistraTowerT2Popper,
			#endregion

			#region 撒旦军团T3
			// 闪电光环法杖
			ItemID.DD2LightningAuraT3Popper,

			// 爆炸烈焰法杖
			ItemID.DD2FlameburstTowerT3Popper,

			// 爆炸机关法杖
			ItemID.DD2ExplosiveTrapT3Popper,

			// 弩车法杖
			ItemID.DD2BallistraTowerT3Popper,
			#endregion

			// 眼球激光塔
			ItemID.HoundiusShootius,

			// 蜘蛛女王法杖
			ItemID.QueenSpiderStaff,

			// 寒霜九头龙法杖
			ItemID.StaffoftheFrostHydra,

			// 月亮传送门法杖
			ItemID.MoonlordTurretStaff,

			// 七彩水晶法杖
			ItemID.RainbowCrystalStaff,
		};

		public static List<int> Whips { get; private set; } = new List<int>()
		{
			// 皮鞭
			ItemID.BlandWhip,

			// 荆鞭
			ItemID.ThornWhip,

			// 脊柱骨鞭
			ItemID.BoneWhip,

			// 鞭炮
			ItemID.FireWhip,

			// 冷鞭
			ItemID.CoolWhip,

			// 迪郎达尔
			ItemID.SwordWhip,

			// 暗黑收割
			ItemID.ScytheWhip,

			// 晨星
			ItemID.MaceWhip,

			// 万花筒
			ItemID.RainbowWhip,
		};
	}
}

using Everglow.Commons.Coroutines;
using Everglow.Commons.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;

namespace Everglow.Commons.Templates.Enemies;

public abstract class Caterpillar : ModNPC
{
	/// <summary>
	/// 运行AI用的协程,很重要不要动
	/// </summary>
	public CoroutineManager _caterpillarCoroutine = new CoroutineManager();

	/// <summary>
	/// 体节结构体
	/// </summary>
	public struct Segment
	{
		public Vector2 Normal;
		public int Index;
		public int Style;
		public int SelfDirection;
		public Vector2 SelfPosition;

		public Segment()
		{
		}
	}

	/// <summary>
	/// 体节
	/// </summary>
	public List<Segment> Segments = new List<Segment>();

	public override void SetDefaults()
	{
		NPCID.Sets.DontDoHardmodeScaling[Type] = true;
		NPC.width = GetBoundingBox().X;
		NPC.height = GetBoundingBox().Y;

		NPC.lifeMax = 16000;
		NPC.aiStyle = -1;
		NPC.damage = 22;
		NPC.defense = 8;
		NPC.canDisplayBuffs = false;

		NPC.knockBackResist = 0.2f;
		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;

		NPC.noTileCollide = true;
		NPC.friendly = false;
		NPC.noGravity = true;
	}

	/// <summary>
	/// 动画速度,默认3
	/// </summary>
	public int AnimationSpeed = 3;

	/// <summary>
	/// 韧性,打破后解除霸体
	/// </summary>
	public float Toughness = 3;

	/// <summary>
	/// 韧度
	/// </summary>
	public float MaxToughness = 3;

	/// <summary>
	/// 旋转速度
	/// </summary>
	public float Omega = 0f;

	/// <summary>
	/// 距离上次被打的时间,经过调参一帧-5
	/// </summary>
	public int HitTimer = 0;

	/// <summary>
	/// 是否悬浮,判定Falling的时候有关键作用
	/// </summary>
	public bool Flying = true;

	/// <summary>
	/// 是否正在生成时坠落
	/// </summary>
	public bool Fall = false;

	/// <summary>
	/// 是否正在常规坠落
	/// </summary>
	public bool FallHit = false;

	/// <summary>
	/// 是否正在挤压
	/// </summary>
	public bool Crawl_1 = false;

	/// <summary>
	/// 是否正在舒展
	/// </summary>
	public bool Crawl_2 = false;

	/// <summary>
	/// 体节碰撞大小，默认40
	/// </summary>
	public int SegmentHitBoxSize = 40;

	/// <summary>
	/// 受击粒子类型，默认-1表示没有粒子
	/// </summary>
	public int DustType = -1;

	/// <summary>
	/// 体节行为大小
	/// </summary>
	public float SegmentBehavioralSize = 30;

	/// <summary>
	/// 体节数量
	/// </summary>
	public int SegmentCount = 10;

	/// <summary>
	/// 防御性手段,确保至少一个协程活着
	/// </summary>
	public int AnyAliveCoroutineTimer;

	public override float SpawnChance(NPC.Spawner spawner)
	{
		return 0f;
	}

	public override void OnSpawn(IEntitySource source)
	{
		Segments = new List<Segment>();
		int startDir = 1;
		if (Main.rand.NextBool(2))
		{
			startDir = -1;
		}
		for (int i = 0; i < SegmentCount; i++)
		{
			var segment = new Segment();
			segment.Style = 1;
			if (i == 0)
			{
				segment.Style = 0;
			}
			if (i == SegmentCount - 1)
			{
				segment.Style = 2;
			}
			segment.SelfPosition = new Vector2((i - (SegmentCount / 2f - 1)) * SegmentBehavioralSize * startDir, 0);
			segment.Normal = new Vector2(0, -1 * startDir);
			segment.SelfDirection = startDir;
			Segments.Add(segment);
		}
		NPC.velocity *= 0;
		NPC.width = GetBoundingBox().Width;
		NPC.height = GetBoundingBox().Height;
		Toughness = MaxToughness;
		_caterpillarCoroutine.StartCoroutine(new Coroutine(Falling()));
	}

	/// <summary>
	/// 重置碰撞箱大小保障Buff的数字不要乱跳
	/// </summary>
	public override void ResetEffects()
	{
		Vector2 v0 = NPC.Center;
		NPC.width = 10;
		NPC.height = 10;
		NPC.Center = v0;
		AnyAliveCoroutineTimer++;

		// 正常情况下所有协程不会都死掉,真死完了等一秒防御性重启
		if (AnyAliveCoroutineTimer > 60)
		{
			_caterpillarCoroutine.StartCoroutine(new Coroutine(Crawling()));
		}
	}

	public override void AI()
	{
		UpdateBuffVFX();
		Vector2 v0 = NPC.Center;
		NPC.width = GetBoundingBox().Width;
		NPC.height = GetBoundingBox().Height;
		NPC.Center = v0;
		for (int moveStep = 0; moveStep < AnimationSpeed; moveStep++)
		{
			_caterpillarCoroutine.Update();
		}
		Omega *= 0.98f;
		NPC.velocity *= 0;
		if (HitTimer > 0)
		{
			HitTimer -= 5;
		}
		else
		{
			HitTimer = 0;
		}
		AdjustPosition();
	}

	/// <summary>
	/// 生成时落下
	/// </summary>
	/// <returns></returns>
	public virtual IEnumerator<ICoroutineInstruction> Falling()
	{
		Fall = true;

		// addTime是缓冲时间,一个体节落地即判定为落地,但是给够时间等其他体节全部落下更加优雅,默认30
		int addTime = 30;
		while (addTime > 0)
		{
			AnyAliveCoroutineTimer = 0; // 在所有可能活着的协程的执行里面都归零
			if (Flying)
			{
				addTime = 30;
			}
			bool hasAddTime = false;
			for (int i = 0; i < Segments.Count; i++)
			{
				Segment segment = Segments[i];
				Vector2 checkNormal = GetNormalOfTiles(segment.SelfPosition + NPC.Center);

				// 如果附近有物块,停止下落
				if (checkNormal != Vector2.zeroVector)
				{
					Flying = false;
					NPC.velocity *= 0f;
					segment.Normal = Vector2.Lerp(segment.Normal, checkNormal * segment.SelfDirection, 0.02f);
				}
				else// 悬空则继续下落
				{
					segment.SelfPosition += new Vector2(0, 5);

					float distance0 = SegmentBehavioralSize + 10;
					float distance1 = SegmentBehavioralSize + 10;
					float force = 0;
					while (distance0 > SegmentBehavioralSize || distance1 > SegmentBehavioralSize)
					{
						force++;
						if (force > 5)
						{
							if (Main.rand.NextBool(4 * SegmentCount / 10) && !hasAddTime)
							{
								hasAddTime = true;
								addTime++;
							}
							break;
						}
						if (i > 0)// 除头外,与前一节距离过远则拉住
						{
							Vector2 v = Segments[i - 1].SelfPosition - segment.SelfPosition;
							distance0 = v.Length();
							if (distance0 > SegmentBehavioralSize)
							{
								segment.SelfPosition = Segments[i - 1].SelfPosition - Vector2.Normalize(v) * (SegmentBehavioralSize - 2 + force);
							}
						}
						if (i < Segments.Count - 1)// 除尾外,与后一节距离过远也拉住
						{
							Vector2 v = Segments[i + 1].SelfPosition - segment.SelfPosition;
							distance1 = v.Length();
							if (distance1 > SegmentBehavioralSize)
							{
								segment.SelfPosition = Segments[i + 1].SelfPosition - Vector2.Normalize(v) * (SegmentBehavioralSize - 2 + force);
							}
						}
					}

					// 调整角度
					if (i > 0 && i < Segments.Count - 1)
					{
						Vector2 direction = Segments[i + 1].SelfPosition - Segments[i - 1].SelfPosition;

						direction = direction.RotatedBy(-MathHelper.PiOver2);
						segment.Normal = Vector2.Normalize(direction);
					}

					// 头尾特判
					if (i == 0)
					{
						segment.Normal = Vector2.Lerp(segment.Normal, Segments[i + 1].Normal, 0.3f);
					}
					if (i == Segments.Count - 1)
					{
						segment.Normal = Vector2.Lerp(segment.Normal, Segments[i - 1].Normal, 0.3f);
					}
				}
				Segments[i] = segment;
			}

			addTime--;
			AdjustPosition();
			yield return new SkipThisFrame();
		}
		NPC.velocity *= 0;
		Toughness = MaxToughness;
		Fall = false;

		// 如果该协程不在运行就打开
		if (!Crawl_1)
		{
			_caterpillarCoroutine.StartCoroutine(new Coroutine(Crawling()));
		}
	}

	/// <summary>
	/// 由于某些原因落下,包括被打或者滑落
	/// </summary>
	/// <returns></returns>
	public virtual IEnumerator<ICoroutineInstruction> Falling_HitByDamege()
	{
		FallHit = true;
		Flying = true;

		// addTime是缓冲时间,一个体节落地即判定为落地,但是给够时间等其他体节全部落下更加优雅,默认300,被击落瘫痪5s
		int addTime = 300;
		yield return new SkipThisFrame();
		Toughness = MaxToughness;
		while (addTime > 0)
		{
			AnyAliveCoroutineTimer = 0; // 在所有可能活着的协程的执行里面都归零
			if (Flying)
			{
				addTime = 300;
			}
			for (int i = 0; i < Segments.Count; i++)
			{
				Segment segment = Segments[i];
				Vector2 checkNormal = GetNormalOfTiles(segment.SelfPosition + NPC.Center);

				// 如果附近有物块,停止下落
				if (checkNormal != Vector2.zeroVector)
				{
					if (Flying)
					{
						if (Segments[0].SelfPosition.X > Segments.Last().SelfPosition.X)
						{
							for (int j = 0; j < Segments.Count; j++)
							{
								Segment segmentII = Segments[j];
								segmentII.SelfDirection = -1;
								Segments[j] = segmentII;
							}
							segment.SelfDirection = -1;
						}
						if (Segments[0].SelfPosition.X < Segments.Last().SelfPosition.X)
						{
							for (int j = 0; j < Segments.Count; j++)
							{
								Segment segmentII = Segments[j];
								segmentII.SelfDirection = 1;
								Segments[j] = segmentII;
							}
							segment.SelfDirection = 1;
						}
						Flying = false;
					}

					NPC.velocity *= 0f;
					segment.Normal = Vector2.Lerp(segment.Normal, checkNormal * segment.SelfDirection, 0.02f);
				}
				else// 悬空则继续下落
				{
					segment.SelfPosition += new Vector2(0, 5);

					float distance0 = SegmentBehavioralSize + 10;
					float distance1 = SegmentBehavioralSize + 10;
					float force = 0;
					while (distance0 > SegmentBehavioralSize || distance1 > SegmentBehavioralSize)
					{
						force++;
						if (force > 5)
						{
							break;
						}
						if (i > 0)// 除头外,与前一节距离过远则拉住
						{
							Vector2 v = Segments[i - 1].SelfPosition - segment.SelfPosition;
							distance0 = v.Length();
							if (distance0 > SegmentBehavioralSize)
							{
								segment.SelfPosition = Segments[i - 1].SelfPosition - Vector2.Normalize(v) * (SegmentBehavioralSize - 2 + force);
							}
						}
						if (i < Segments.Count - 1)// 除尾外,与后一节距离过远也拉住
						{
							Vector2 v = Segments[i + 1].SelfPosition - segment.SelfPosition;
							distance1 = v.Length();
							if (distance1 > SegmentBehavioralSize)
							{
								segment.SelfPosition = Segments[i + 1].SelfPosition - Vector2.Normalize(v) * (SegmentBehavioralSize - 2 + force);
							}
						}
					}

					// 调整角度
					if (i > 0 && i < Segments.Count - 1)
					{
						Vector2 direction = Segments[i + 1].SelfPosition - Segments[i - 1].SelfPosition;

						direction = direction.RotatedBy(-MathHelper.PiOver2);
						segment.Normal = Vector2.Normalize(direction);
					}
					if (i == 0)
					{
						segment.Normal = Vector2.Lerp(segment.Normal, Segments[i + 1].Normal, 0.3f);
					}
					if (i == Segments.Count - 1)
					{
						segment.Normal = Vector2.Lerp(segment.Normal, Segments[i - 1].Normal, 0.3f);
					}
				}
				Segments[i] = segment;
			}
			if (Flying)
			{
				for (int i = 0; i < Segments.Count; i++)
				{
					Segment segment = Segments[i];
					int middleIndex = SegmentCount / 2;
					segment.SelfPosition += segment.SelfPosition - Segments[middleIndex].SelfPosition - (segment.SelfPosition - Segments[middleIndex].SelfPosition).RotatedBy(Omega);

					// 调整角度
					if (i > 0 && i < Segments.Count - 1)
					{
						Vector2 direction = Segments[i + 1].SelfPosition - Segments[i - 1].SelfPosition;

						direction = direction.RotatedBy(-MathHelper.PiOver2 * segment.SelfDirection);
						segment.Normal = Vector2.Normalize(direction);
					}
					if (i == 0)
					{
						segment.Normal = Vector2.Lerp(segment.Normal, Segments[i + 1].Normal, 0.3f);
					}
					if (i == Segments.Count - 1)
					{
						segment.Normal = Vector2.Lerp(segment.Normal, Segments[i - 1].Normal, 0.3f);
					}

					Segments[i] = segment;
				}
			}
			addTime--;
			AdjustPosition();
			yield return new SkipThisFrame();
		}
		NPC.velocity *= 0;

		FallHit = false;
		if (!Crawl_1)
		{
			_caterpillarCoroutine.StartCoroutine(new Coroutine(Crawling()));
		}
	}

	/// <summary>
	/// 挤压
	/// </summary>
	/// <returns></returns>
	public virtual IEnumerator<ICoroutineInstruction> Crawling()
	{
		Crawl_1 = true;
		float tValue = 0;
		var toHead = Vector2.Normalize(Segments[Segments.Count - 1].SelfPosition - Segments[0].SelfPosition);

		// 随机决定拱起程度
		float curveValue = Main.rand.NextFloat(1.7f, 3.3f);
		for (int t = 0; t < 60; t++)
		{
			// 被打破防了,掉下去,终止此协程
			if (Toughness <= 0)
			{
				Crawl_1 = false;
				yield break;
			}
			AnyAliveCoroutineTimer = 0; // 在所有可能活着的协程的执行里面都归零
			Segment tail = Segments[Segments.Count - 1]; // 尾巴
			tail.SelfPosition = Vector2.Lerp(tail.SelfPosition, Segments[0].SelfPosition + toHead * (SegmentCount * SegmentBehavioralSize / 3f), 0.03f);
			float height = CheckOverHeight(tail.SelfPosition + NPC.Center, Vector2.Normalize(toHead).RotatedBy(MathHelper.PiOver2));
			if (height > 2 && height < 100)
			{
				tail.SelfPosition += 2 * toHead.RotatedBy(MathHelper.PiOver2);
			}
			tValue = (float)Utils.Lerp(tValue, SegmentBehavioralSize * SegmentCount / 5f, 0.03f);
			Segments[Segments.Count - 1] = tail;
			for (int i = 1; i < Segments.Count - 1; i++)
			{
				float lerp = 1 - i / (float)Segments.Count;
				Segment segment = Segments[i];
				segment.SelfPosition = Vector2.Lerp(tail.SelfPosition, Segments[0].SelfPosition, MathF.Pow(lerp, 1.05f));
				float x0 = MathF.Max(0.5f - Math.Abs(lerp - 0.5f), 0) * 2;
				x0 = MathF.Pow(x0, curveValue);

				// 向上拱起向量
				Vector2 round = new Vector2(0, tValue * 0.7f * segment.SelfDirection).RotatedBy(-lerp * MathHelper.TwoPi * segment.SelfDirection) + new Vector2(0, -tValue * segment.SelfDirection);
				segment.SelfPosition += round.RotatedBy(toHead.ToRotation()) * MathF.Sin(x0 * MathHelper.Pi * 0.5f) * 0.75f;

				Vector2 direction = Segments[i + 1].SelfPosition - Segments[i - 1].SelfPosition;

				direction = direction.RotatedBy(-MathHelper.PiOver2);
				segment.Normal = Vector2.Normalize(direction);
				Segments[i] = segment;
			}
			yield return new SkipThisFrame();
		}
		AdjustPosition();
		Crawl_1 = false;
		if (!Crawl_2)
		{
			_caterpillarCoroutine.StartCoroutine(new Coroutine(CrawlingII()));
		}
	}

	/// <summary>
	/// 舒展
	/// </summary>
	/// <returns></returns>
	public virtual IEnumerator<ICoroutineInstruction> CrawlingII()
	{
		Crawl_2 = true;
		float tValue = 0;
		Vector2 toTail = GetNormalOfTiles(Segments[0].SelfPosition + NPC.Center).RotatedBy(-MathHelper.PiOver4 * Segments[0].SelfDirection);
		if (toTail == Vector2.zeroVector)
		{
			toTail = Vector2.Normalize(Segments[0].SelfPosition - Segments[Segments.Count - 1].SelfPosition);
		}

		// 从头部开始拉开虫体
		for (int t = 0; t < 60; t++)
		{
			// 被打破防了,掉下去,终止此协程
			if (Toughness <= 0)
			{
				Crawl_2 = false;
				yield break;
			}
			AnyAliveCoroutineTimer = 0; // 在所有可能活着的协程的执行里面都归零
			Segment head = Segments[0]; // 头
			head.SelfPosition += toTail * 2f * (SegmentCount * SegmentBehavioralSize / 300f);
			toTail = toTail.RotatedBy(-0.029f / (SegmentCount / 10f) * MathF.Sin(t / 60f * MathF.PI * head.SelfDirection));
			head.Normal = toTail.RotatedBy(MathHelper.PiOver2);
			tValue = (float)Utils.Lerp(tValue, SegmentBehavioralSize * SegmentCount / 5f, 0.03f);
			Segments[0] = head;

			for (int i = 1; i < Segments.Count; i++)
			{
				Segment segment = Segments[i];
				if (GetNormalOfTiles(segment.SelfPosition + NPC.Center) == Vector2.zeroVector)
				{
					segment.SelfPosition += toTail.RotatedBy(-MathHelper.PiOver2 * segment.SelfDirection) * tValue / 30f;
				}
				Vector2 v = Segments[i - 1].SelfPosition - segment.SelfPosition;
				if (v.Length() > SegmentBehavioralSize)
				{
					segment.SelfPosition = Segments[i - 1].SelfPosition - Vector2.Normalize(v) * (SegmentBehavioralSize - 2);
				}
				Vector2 direction = segment.SelfPosition - Segments[i - 1].SelfPosition;
				if (i != Segments.Count - 1)
				{
					direction = Segments[i + 1].SelfPosition - Segments[i - 1].SelfPosition;
				}
				direction = direction.RotatedBy(-MathHelper.PiOver2);
				segment.Normal = Vector2.Normalize(direction);
				Segments[i] = segment;
			}
			if (t > 10 && GetNormalOfTiles(Segments[0].SelfPosition + NPC.Center) != Vector2.zeroVector)
			{
				break;
			}
			yield return new SkipThisFrame();
		}

		// 头部寻找落点
		while (GetNormalOfTiles(Segments[0].SelfPosition + NPC.Center) == Vector2.zeroVector)
		{
			// 被打破防了,掉下去,终止此协程
			if (Toughness <= 0)
			{
				Crawl_2 = false;
				yield break;
			}
			AnyAliveCoroutineTimer = 0; // 在所有可能活着的协程的执行里面都归零
			Segment head = Segments[0]; // 头
			toTail += toTail.RotatedBy(-MathHelper.PiOver2 * head.SelfDirection) * 0.02f;
			toTail = Vector2.Normalize(toTail);
			head.SelfPosition += toTail * 2 * (SegmentCount / 10f);

			head.Normal = Vector2.Lerp(head.Normal, Segments[1].Normal, 0.2f);
			tValue = (float)Utils.Lerp(tValue, 120, 0.03f);
			Segments[0] = head;
			bool shouldFall = true;
			for (int i = 1; i < Segments.Count; i++)
			{
				Segment segment = Segments[i];
				Vector2 getNormal = GetNormalOfTiles(segment.SelfPosition + NPC.Center);
				if (getNormal == Vector2.zeroVector)
				{
					segment.SelfPosition += toTail.RotatedBy(-MathHelper.PiOver2 * segment.SelfDirection) * 2;
				}
				else
				{
					shouldFall = false;
				}
				Vector2 v = Segments[i - 1].SelfPosition - segment.SelfPosition;
				if (v.Length() > SegmentBehavioralSize)
				{
					segment.SelfPosition = Segments[i - 1].SelfPosition - Vector2.Normalize(v) * (SegmentBehavioralSize - 2);
				}
				Vector2 direction = segment.SelfPosition - Segments[i - 1].SelfPosition;

				direction = direction.RotatedBy(-MathHelper.PiOver2);
				segment.Normal = Vector2.Normalize(direction);
				Segments[i] = segment;
			}

			// 如果整条虫都被头带上天了,在重力作用下滑落
			if (shouldFall)
			{
				Crawl_2 = false;
				_caterpillarCoroutine.StartCoroutine(new Coroutine(Falling_HitByDamege()));
				yield break;
			}
			yield return new SkipThisFrame();
		}
		AdjustPosition();
		Crawl_2 = false;

		// 平坦程度
		float LineValue = 0;
		for (int i = 1; i < Segments.Count; i++)
		{
			LineValue += (Segments[i].Normal - Segments[i - 1].Normal).Length();
		}

		// 蠕虫只会在比较平坦的时候休息,1/10的几率休息5~50秒
		if (Main.rand.NextBool(10) && LineValue < 1)
		{
			_caterpillarCoroutine.StartCoroutine(new Coroutine(Waiting(Main.rand.Next(300, 3000))));
			yield break;
		}
		if (!Crawl_1)
		{
			_caterpillarCoroutine.StartCoroutine(new Coroutine(Crawling()));
		}
	}

	/// <summary>
	/// 转向
	/// </summary>
	/// <returns></returns>
	public virtual IEnumerator<ICoroutineInstruction> CrawlingIII_Turn()
	{
		for (int j = 0; j < Segments.Count; j++)
		{
			Segment segmentII = Segments[j];
			segmentII.SelfDirection *= -1;
			Segments[j] = segmentII;
		}
		Crawl_1 = true;
		float tValue = 0;
		var toHead = Vector2.Normalize(Segments[Segments.Count - 1].SelfPosition - Segments[0].SelfPosition);
		for (int t = 0; t < 60; t++)
		{
			if (Toughness <= 0)
			{
				Crawl_1 = false;
				yield break;
			}
			AnyAliveCoroutineTimer = 0; // 在所有可能活着的协程的执行里面都归零
			Segment tail = Segments[Segments.Count - 1]; // 尾巴
			tail.SelfPosition = Vector2.Lerp(tail.SelfPosition, Segments[0].SelfPosition + toHead * (SegmentCount * SegmentBehavioralSize / 3f), 0.03f);
			float height = CheckOverHeight(tail.SelfPosition + NPC.Center, Vector2.Normalize(toHead).RotatedBy(MathHelper.PiOver2));
			if (height > 2 && height < 100)
			{
				tail.SelfPosition += 2 * toHead.RotatedBy(MathHelper.PiOver2);
			}
			tValue = (float)Utils.Lerp(tValue, SegmentBehavioralSize * SegmentCount / 5f, 0.03f);
			Segments[Segments.Count - 1] = tail;
			for (int i = 1; i < Segments.Count - 1; i++)
			{
				float lerp = 1 - i / (float)Segments.Count;
				Segment segment = Segments[i];
				segment.SelfPosition = Vector2.Lerp(tail.SelfPosition, Segments[0].SelfPosition, MathF.Pow(lerp, 1.05f));
				float x0 = MathF.Max(0.5f - Math.Abs(lerp - 0.5f), 0);

				// 拱起向量
				Vector2 round = new Vector2(0, tValue * 0.7f * segment.SelfDirection).RotatedBy(-lerp * MathHelper.TwoPi * segment.SelfDirection) + new Vector2(0, -tValue * segment.SelfDirection);
				segment.SelfPosition += round.RotatedBy(toHead.ToRotation()) * x0 * 1.5f;

				Vector2 direction = Segments[i + 1].SelfPosition - Segments[i - 1].SelfPosition;

				direction = direction.RotatedBy(-MathHelper.PiOver2);
				segment.Normal = Vector2.Normalize(direction);
				Segments[i] = segment;
			}
			yield return new SkipThisFrame();
		}
		AdjustPosition();
		Crawl_1 = false;
		if (!Crawl_2)
		{
			_caterpillarCoroutine.StartCoroutine(new Coroutine(CrawlingII()));
		}
	}

	/// <summary>
	/// 休息
	/// </summary>
	/// <param name="time"></param>
	/// <returns></returns>
	public virtual IEnumerator<ICoroutineInstruction> Waiting(int time)
	{
		for (int t = 0; t < time; t++)
		{
			if (HitTimer > 0)
			{
				_caterpillarCoroutine.StartCoroutine(new Coroutine(Crawling()));
				yield break;
			}
			AnyAliveCoroutineTimer = 0; // 在所有可能活着的协程的执行里面都归零
			yield return new SkipThisFrame();
		}
		if (!Crawl_1)
		{
			if (Main.rand.NextBool(8))
			{
				_caterpillarCoroutine.StartCoroutine(new Coroutine(CrawlingIII_Turn()));
			}
			else
			{
				_caterpillarCoroutine.StartCoroutine(new Coroutine(Crawling()));
			}
		}
	}

	/// <summary>
	/// 移动碰撞箱的位置
	/// </summary>
	public void AdjustPosition()
	{
		Vector2 v4P = Segments[4].SelfPosition;
		NPC.Center += v4P;
		for (int i = 0; i < Segments.Count; i++)
		{
			Segment segment = Segments[i];
			segment.SelfPosition -= v4P;
			Segments[i] = segment;
		}
	}

	/// <summary>
	/// 获取绘制帧
	/// </summary>
	/// <param name="Style"></param>
	/// <returns></returns>
	public virtual Rectangle GetDrawFrame(int Style)
	{
		Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
		int width = texture.Width / 3;
		return new Rectangle(Style * width, 0, width, texture.Height);
	}

	/// <summary>
	/// 绘制
	/// </summary>
	/// <param name="spriteBatch"></param>
	/// <param name="screenPos"></param>
	/// <param name="drawColor"></param>
	/// <returns></returns>
	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
		for (int i = 1; i < Segments.Count; i++)
		{
			Vector2 drawPos = Segments[i].SelfPosition + NPC.Center - Main.screenPosition;

			Rectangle drawRectangle = GetDrawFrame(Segments[i].Style);
			SpriteEffects spriteEffect = SpriteEffects.None;
			if (Segments[i].SelfDirection == -1)
			{
				spriteEffect = SpriteEffects.FlipVertically;
			}
			Vector2 origin = drawRectangle.Size() / 2f;
			spriteBatch.Draw(texture, drawPos, drawRectangle, drawColor, Segments[i].Normal.ToRotation() + MathHelper.PiOver2, origin, 1f, spriteEffect, 0);
			if (HitTimer > 0)
			{
				var hitColor = new Color(drawColor.R / 255f, 0, 0, HitTimer / 30f);
				spriteBatch.Draw(texture, drawPos, drawRectangle, hitColor, Segments[i].Normal.ToRotation() + MathHelper.PiOver2, origin, 1f, spriteEffect, 0);
			}
		}
		for (int i = 0; i < 1; i++)
		{
			if (Segments.Count > 0)
			{
				Vector2 drawPos = Segments[i].SelfPosition + NPC.Center - Main.screenPosition;
				Rectangle drawRectangle = GetDrawFrame(Segments[i].Style);
				SpriteEffects spriteEffect = SpriteEffects.None;
				if (Segments[i].SelfDirection == -1)
				{
					spriteEffect = SpriteEffects.FlipVertically;
				}
				var origin = new Vector2(drawRectangle.Width, drawRectangle.Height / 2f);
				Vector2 offset = Segments[i].Normal.RotatedBy(MathHelper.PiOver2) * drawRectangle.Width / 3f;
				spriteBatch.Draw(texture, drawPos + offset, drawRectangle, drawColor, Segments[i].Normal.ToRotation() + MathHelper.PiOver2, origin, 1f, spriteEffect, 0);
				if (HitTimer > 0)
				{
					var hitColor = new Color(drawColor.R / 255f, 0, 0, HitTimer / 30f);
					spriteBatch.Draw(texture, drawPos + offset, drawRectangle, hitColor, Segments[i].Normal.ToRotation() + MathHelper.PiOver2, origin, 1f, spriteEffect, 0);
				}
			}
		}
		return false;
	}

	/// <summary>
	/// 能否打中玩家
	/// </summary>
	/// <param name="target"></param>
	/// <param name="cooldownSlot"></param>
	/// <returns></returns>
	public override bool CanHitPlayer(Player target, ref int cooldownSlot)
	{
		for (int i = 0; i < Segments.Count; i++)
		{
			Segment segment = Segments[i];
			int x = (int)(NPC.Center.X + segment.SelfPosition.X - 20);
			int y = (int)(NPC.Center.Y + segment.SelfPosition.Y - 20);
			var rectangle = new Rectangle(x, y, 40, 40);

			if (Rectangle.Intersect(rectangle, target.Hitbox) != Rectangle.emptyRectangle)
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// 判定能否被弹幕打中,疑似存在问题
	/// </summary>
	/// <param name="projectile"></param>
	/// <returns></returns>
	public override bool? CanBeHitByProjectile(Projectile projectile)
	{
		if (projectile != null && projectile.active)
		{
			for (int i = 0; i < Segments.Count; i++)
			{
				Segment segment = Segments[i];
				int x = (int)(NPC.Center.X + segment.SelfPosition.X - SegmentHitBoxSize / 2);
				int y = (int)(NPC.Center.Y + segment.SelfPosition.Y - SegmentHitBoxSize / 2);
				var rectangle = new Rectangle(x, y, SegmentHitBoxSize, SegmentHitBoxSize);

				if (projectile.Colliding(projectile.Hitbox, rectangle) && NPC.immune[projectile.owner] == 0 && projectile.friendly)
				{
					Vector2 strikeForce = projectile.velocity;
					if (strikeForce.Length() > 80f)
					{
						strikeForce = Vector2.Normalize(strikeForce) * 80f;
					}
					segment.SelfPosition += strikeForce;
					return true;
				}
			}
		}
		return false;
	}

	/// <summary>
	/// 判定能否被玩家近战打中
	/// </summary>
	/// <param name="player"></param>
	/// <param name="item"></param>
	/// <param name="meleeAttackHitbox"></param>
	/// <returns></returns>
	public override bool? CanCollideWithPlayerMeleeAttack(Player player, Item item, Rectangle meleeAttackHitbox)
	{
		for (int i = 0; i < Segments.Count; i++)
		{
			Segment segment = Segments[i];
			int x = (int)(NPC.Center.X + segment.SelfPosition.X - SegmentHitBoxSize / 2);
			int y = (int)(NPC.Center.Y + segment.SelfPosition.Y - SegmentHitBoxSize / 2);
			var rectangle = new Rectangle(x, y, SegmentHitBoxSize, SegmentHitBoxSize);

			if (Rectangle.Intersect(meleeAttackHitbox, rectangle) != Rectangle.emptyRectangle)
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// 被任何东西打中,出伤显示数字,压缩碰撞箱为10x10
	/// </summary>
	/// <param name="modifiers"></param>
	public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
	{
		Vector2 v0 = NPC.Center;
		NPC.width = 10;
		NPC.height = 10;
		NPC.Center = v0;
	}

	/// <summary>
	/// 被弹幕打中,在此还原碰撞箱
	/// </summary>
	/// <param name="projectile"></param>
	/// <param name="hit"></param>
	/// <param name="damageDone"></param>
	public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
	{
		Vector2 v0 = NPC.Center;
		NPC.width = GetBoundingBox().Width;
		NPC.height = GetBoundingBox().Height;
		NPC.Center = v0;
		HitTimer += (int)Math.Min(hit.Knockback * 5f + 10, 30);
		HitTimer = Math.Min(HitTimer, 30);
		if (!FallHit)
		{
			Toughness -= hit.Knockback;
			if (Toughness <= 0)
			{
				_caterpillarCoroutine.StartCoroutine(new Coroutine(Falling_HitByDamege()));
				Omega = Main.rand.NextFloat(-0.04f, 0.04f);
				FallHit = true;
				Flying = true;
				NPC.position += Segments[2].Normal * Segments[2].SelfDirection * 50;
			}
		}
	}

	/// <summary>
	/// 被武器打中,在此还原碰撞箱
	/// </summary>
	/// <param name="player"></param>
	/// <param name="item"></param>
	/// <param name="hit"></param>
	/// <param name="damageDone"></param>
	public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
	{
		Vector2 v0 = NPC.Center;
		NPC.width = GetBoundingBox().Width;
		NPC.height = GetBoundingBox().Height;
		NPC.Center = v0;
		HitTimer += (int)Math.Min(hit.Knockback * 5f + 10, 30);
		HitTimer = Math.Min(HitTimer, 30);
		if (!FallHit)
		{
			Toughness -= hit.Knockback;
			if (Toughness <= 0)
			{
				_caterpillarCoroutine.StartCoroutine(new Coroutine(Falling_HitByDamege()));
				Omega = Main.rand.NextFloat(-0.04f, 0.04f);
				FallHit = true;
				Flying = true;
				NPC.position += Segments[2].Normal * Segments[2].SelfDirection * 50;
			}
		}
	}

	/// <summary>
	/// 更改血条位置
	/// </summary>
	/// <param name="hbPosition"></param>
	/// <param name="scale"></param>
	/// <param name="position"></param>
	/// <returns></returns>
	public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
	{
		if (Segments.Count > 5)
		{
			position = Segments[4].SelfPosition + NPC.Center;
		}
		return true;
	}

	/// <summary>
	/// 虫体曲线的外接正交矩形为鼠标碰撞箱
	/// </summary>
	/// <param name="boundingBox"></param>
	public override void ModifyHoverBoundingBox(ref Rectangle boundingBox)
	{
		if (GetBoundingBox() != Rectangle.emptyRectangle)
		{
			boundingBox = GetBoundingBox();
		}
	}

	/// <summary>
	/// 被打到的效果,包含流血和碎尸
	/// </summary>
	/// <param name="hit"></param>
	public override void HitEffect(NPC.HitInfo hit)
	{
		if (DustType != -1)
		{
			for (int j = 0; j < Segments.Count; j++)
			{
				Vector2 pos = NPC.Center + Segments[j].SelfPosition;
				Dust.NewDustDirect(pos - new Vector2(SegmentHitBoxSize / 2), SegmentHitBoxSize / 2, SegmentHitBoxSize / 2, DustType);
			}
		}
	}

	/// <summary>
	/// Buff效果
	/// </summary>
	public virtual void UpdateBuffVFX()
	{
		if (NPC.markedByScytheWhip && Main.rand.NextBool(3))
		{
			ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.BlackLightningSmall, new ParticleOrchestraSettings
			{
				MovementVector = Main.rand.NextVector2Circular(1f, 1f),
				PositionInWorld = Main.rand.NextVector2FromRectangle(NPC.Hitbox),
			});
		}
		if (NPC.poisoned && Main.rand.NextBool(30))
		{
			var dust = Dust.NewDustDirect(NPC.Center, 0, 0, DustID.Poisoned, 0f, 0f, 120, default, 0.2f);
			dust.noGravity = true;
			dust.fadeIn = 1.9f;
			dust.position = RedistributionInMyShape() - new Vector2(4);
		}
		if (NPC.venom && Main.rand.NextBool(10))
		{
			var dust2 = Dust.NewDustDirect(NPC.Center, 0, 0, DustID.Venom, 0f, 0f, 100, default, 0.5f);
			dust2.noGravity = true;
			dust2.fadeIn = 1.5f;
			dust2.position = RedistributionInMyShape() - new Vector2(4);
		}
		if (NPC.shadowFlame && Main.rand.Next(5) < 4)
		{
			var dust3 = Dust.NewDustDirect(NPC.Center, 0, 0, DustID.Shadowflame, 0, 0, 180, default, 1.95f);
			dust3.noGravity = true;
			dust3.velocity *= 0.75f;
			dust3.velocity.X *= 0.75f;
			dust3.velocity.Y -= 1f;
			if (Main.rand.NextBool(4))
			{
				dust3.noGravity = false;
				dust3.scale *= 0.5f;
			}
			dust3.position = RedistributionInMyShape() - new Vector2(4);
		}
		if (NPC.onFire)
		{
			if (Main.rand.Next(4) < 3)
			{
				var dust4 = Dust.NewDustDirect(NPC.Center, 0, 0, DustID.Torch, 0, 0, 100, default, 3.5f);
				dust4.noGravity = true;
				dust4.velocity *= 1.8f;
				dust4.velocity.Y -= 0.5f;
				if (Main.rand.NextBool(4))
				{
					dust4.noGravity = false;
					dust4.scale *= 0.5f;
				}
				dust4.position = RedistributionInMyShape() - new Vector2(4);
			}
			Lighting.AddLight(NPC.Center, 1f, 0.3f, 0.1f);
		}
		if (NPC.onFire3)
		{
			if (Main.rand.Next(4) < 3)
			{
				var dust5 = Dust.NewDustDirect(NPC.Center, 0, 0, DustID.Torch, 0, 0, 100, default, 3.5f);
				dust5.noGravity = true;
				dust5.velocity *= 1.8f;
				dust5.velocity.Y -= 0.5f;
				if (Main.rand.NextBool(4))
				{
					dust5.noGravity = false;
					dust5.scale *= 0.5f;
				}
				dust5.customData = 0;
				dust5.position = RedistributionInMyShape() - new Vector2(4);
			}
			Lighting.AddLight(NPC.Center, 1f, 0.3f, 0.1f);
		}
		if (NPC.daybreak)
		{
			if (Main.rand.Next(4) < 3)
			{
				var dust6 = Dust.NewDustDirect(NPC.Center, 0, 0, DustID.OrangeTorch, 0, 0, 100, default, 3.5f);
				dust6.noGravity = true;
				dust6.velocity *= 2.8f;
				dust6.velocity.Y -= 0.5f;
				if (Main.rand.NextBool(4))
				{
					dust6.noGravity = false;
					dust6.scale *= 0.5f;
				}
				dust6.position = RedistributionInMyShape() - new Vector2(4);
			}

			Lighting.AddLight(NPC.Center, 1f, 0.3f, 0.1f);
		}

		if (NPC.betsysCurse)
		{
			if (Main.rand.Next(4) < 3)
			{
				var dust7 = Dust.NewDustDirect(NPC.Center, 0, 0, DustID.Pixie, 0, 0, 100, default, 3.5f);
				dust7.noGravity = true;
				dust7.velocity *= 2.8f;
				dust7.velocity.Y -= 1.5f;
				dust7.noGravity = false;
				dust7.scale = 0.9f;
				dust7.color = new Color(0, 0, 180, 255);
				dust7.velocity *= 0.2f;
				dust7.position = RedistributionInMyShape() - new Vector2(4);
			}

			Lighting.AddLight(NPC.Center, 0.6f, 0.1f, 0.9f);
		}

		if (NPC.oiled && !Main.rand.NextBool(3))
		{
			int num = 175;
			var newColor = new Color(0, 0, 0, 250);
			if (Main.rand.NextBool(2))
			{
				var dust8 = Dust.NewDustDirect(NPC.Center, 0, 0, DustID.TintableDust, 0f, 0f, num, newColor, 1.4f);
				if (Main.rand.NextBool(2))
				{
					dust8.alpha += 25;
				}

				if (Main.rand.NextBool(2))
				{
					dust8.alpha += 25;
				}

				dust8.noLight = true;
				dust8.velocity *= 0.2f;
				dust8.velocity.Y += 0.2f;

				dust8.position = RedistributionInMyShape() - new Vector2(4);
			}
		}

		if (NPC.dryadWard && NPC.velocity.X != 0f && Main.rand.NextBool(4))
		{
			var dust9 = Dust.NewDustDirect(NPC.Center, 0, 0, DustID.PoisonStaff, 0, 0, 100, default, 1.5f);
			dust9.noGravity = true;
			dust9.noLight = true;
			dust9.velocity *= 0f;
			dust9.position = RedistributionInMyShape() - new Vector2(4);
		}

		if (NPC.dryadBane && Main.rand.NextBool(4))
		{
			var dust10 = Dust.NewDustDirect(NPC.Center, 0, 0, DustID.PoisonStaff, 0, 0, 100, default, 1.5f);
			dust10.noGravity = true;
			dust10.velocity *= new Vector2(Main.rand.NextFloat() * 4f - 2f, 0f);
			dust10.noLight = true;
			dust10.position = RedistributionInMyShape() - new Vector2(4);
		}

		if (NPC.loveStruck && Main.rand.NextBool(5))
		{
			var vector2 = new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11));
			vector2.Normalize();
			vector2.X *= 0.66f;
			int num2 = Gore.NewGore(NPC.Center, vector2 * Main.rand.Next(3, 6) * 0.33f, 331, Main.rand.Next(40, 121) * 0.01f);
			Main.gore[num2].sticky = false;
			Main.gore[num2].velocity *= 0.4f;
			Main.gore[num2].velocity.Y -= 0.6f;
			Main.gore[num2].position = RedistributionInMyShape() - new Vector2(Main.gore[num2].Width, Main.gore[num2].Height) * Main.gore[num2].scale / 2f;
		}

		if (NPC.stinky && Main.rand.NextBool(5))
		{
			var vector3 = new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11));
			vector3.Normalize();
			vector3.X *= 0.66f;
			vector3.Y = Math.Abs(vector3.Y);
			Vector2 vector4 = vector3 * Main.rand.Next(3, 5) * 0.25f;
			var dust11 = Dust.NewDustDirect(NPC.Center, 0, 0, DustID.FartInAJar, vector4.X, vector4.Y * 0.5f, 100, default, 1.5f);
			dust11.velocity *= 0.1f;
			dust11.velocity.Y -= 0.5f;
			dust11.position = RedistributionInMyShape() - new Vector2(4);
		}

		if (NPC.dripping && !Main.rand.NextBool(4))
		{
			if (Main.rand.NextBool(2))
			{
				var dust12 = Dust.NewDustDirect(NPC.Center, 0, 0, DustID.Wet, 0f, 0f, 50, default, 0.8f);
				if (Main.rand.NextBool(2))
				{
					dust12.alpha += 25;
				}

				if (Main.rand.NextBool(2))
				{
					dust12.alpha += 25;
				}

				dust12.noLight = true;
				dust12.velocity *= 0.2f;
				dust12.velocity.Y += 0.2f;
				dust12.position = RedistributionInMyShape() - new Vector2(4);
			}
			else
			{
				var dust13 = Dust.NewDustDirect(NPC.Center, 0, 0, DustID.Wet, 0f, 0f, 50, default, 1.1f);
				if (Main.rand.NextBool(2))
				{
					dust13.alpha += 25;
				}

				if (Main.rand.NextBool(2))
				{
					dust13.alpha += 25;
				}

				dust13.noLight = true;
				dust13.noGravity = true;
				dust13.velocity *= 0.2f;
				dust13.velocity.Y += 1f;
				dust13.position = RedistributionInMyShape() - new Vector2(4);
			}
		}

		if (NPC.drippingSlime && !Main.rand.NextBool(4))
		{
			int num3 = 175;
			var newColor2 = new Color(0, 80, 255, 100);
			if (Main.rand.NextBool(2))
			{
				var dust14 = Dust.NewDustDirect(NPC.Center, 0, 0, DustID.TintableDust, 0f, 0f, num3, newColor2, 1.4f);
				if (Main.rand.NextBool(2))
				{
					dust14.alpha += 25;
				}

				if (Main.rand.NextBool(2))
				{
					dust14.alpha += 25;
				}

				dust14.noLight = true;
				dust14.velocity *= 0.2f;
				dust14.velocity.Y += 0.2f;
				dust14.position = RedistributionInMyShape() - new Vector2(4);
			}
		}

		if (NPC.drippingSparkleSlime && !Main.rand.NextBool(4))
		{
			int num4 = 150;
			if (Main.rand.NextBool(2))
			{
				var dust15 = Dust.NewDustDirect(NPC.Center, 0, 0, DustID.PinkSlime, 0f, 0f, num4);
				if (Main.rand.NextBool(2))
				{
					dust15.alpha += 25;
				}

				if (Main.rand.NextBool(2))
				{
					dust15.alpha += 25;
				}

				dust15.noLight = true;
				dust15.velocity *= 0.2f;
				dust15.velocity.Y += 0.2f;
				dust15.position = RedistributionInMyShape() - new Vector2(4);
			}
		}

		if (NPC.onFrostBurn)
		{
			if (Main.rand.Next(4) < 3)
			{
				var dust16 = Dust.NewDustDirect(NPC.Center, 0, 0, DustID.IceTorch, 0, 0, 100, default, 3.5f);
				dust16.noGravity = true;
				dust16.velocity *= 1.8f;
				dust16.velocity.Y -= 0.5f;
				if (Main.rand.NextBool(4))
				{
					dust16.noGravity = false;
					dust16.scale *= 0.5f;
				}
				dust16.position = RedistributionInMyShape() - new Vector2(4);
			}

			Lighting.AddLight(NPC.Center, 0.1f, 0.6f, 1f);
		}

		if (NPC.onFrostBurn2)
		{
			if (Main.rand.Next(4) < 3)
			{
				var dust17 = Dust.NewDustDirect(NPC.Center, 0, 0, DustID.IceTorch, 0, 0, 100, default, 3.5f);
				dust17.noGravity = true;
				dust17.velocity *= 1.8f;
				dust17.velocity.Y -= 0.5f;
				if (Main.rand.NextBool(4))
				{
					dust17.noGravity = false;
					dust17.scale *= 0.5f;
				}
				dust17.position = RedistributionInMyShape() - new Vector2(4);
			}

			Lighting.AddLight(NPC.Center, 0.1f, 0.6f, 1f);
		}

		if (NPC.onFire2)
		{
			if (Main.rand.Next(4) < 3)
			{
				var dust18 = Dust.NewDustDirect(NPC.Center, 0, 0, DustID.CursedTorch, 0, 0, 100, default, 3.5f);
				dust18.noGravity = true;
				dust18.velocity *= 1.8f;
				dust18.velocity.Y -= 0.5f;
				if (Main.rand.NextBool(4))
				{
					dust18.noGravity = false;
					dust18.scale *= 0.5f;
				}
				dust18.position = RedistributionInMyShape() - new Vector2(4);
			}

			Lighting.AddLight(NPC.Center, 1f, 0.3f, 0.1f);
		}

		// netShimmer = false;
		// if (shimmering)
		// {
		// shimmerTransparency += 0.01f;
		// if (Main.netMode != 1 && (double)shimmerTransparency > 0.9)
		// GetShimmered();

		// if (shimmerTransparency > 1f)
		// shimmerTransparency = 1f;
		// }
		// else if (shimmerTransparency > 0f)
		// {
		// if (justHit)
		// shimmerTransparency -= 0.1f;

		// if (buffImmune[353])
		// shimmerTransparency -= 0.015f;
		// else
		// shimmerTransparency -= 0.001f;

		// if (shimmerTransparency < 0f)
		// shimmerTransparency = 0f;
		// }
	}

	/// <summary>
	/// 在虫体上随机生成一个点
	/// </summary>
	/// <returns></returns>
	public Vector2 RedistributionInMyShape()
	{
		int choiceIndex = Main.rand.Next(Segments.Count);
		Vector2 pos = NPC.Center + Segments[choiceIndex].SelfPosition;
		return pos;
	}

	/// <summary>
	/// 获取虫体外接正交矩形
	/// </summary>
	/// <returns></returns>
	public Rectangle GetBoundingBox()
	{
		Rectangle boundingBox = Rectangle.Empty;
		int minX = Main.maxTilesX * 16;
		int minY = Main.maxTilesY * 16;
		int maxX = 0;
		int maxY = 0;
		for (int i = 0; i < Segments.Count; i++)
		{
			Segment segment = Segments[i];
			var pos = (segment.SelfPosition + NPC.Center).ToPoint();
			if (pos.X - 20 < minX)
			{
				minX = pos.X - 20;
			}
			if (pos.Y - 20 < minY)
			{
				minY = pos.Y - 20;
			}
			if (pos.X + 20 > maxX)
			{
				maxX = pos.X + 20;
			}
			if (pos.Y + 20 > maxY)
			{
				maxY = pos.Y + 20;
			}
		}
		if (maxY > minY && maxX > minX)
		{
			boundingBox = new Rectangle(minX, minY, maxX - minX, maxY - minY);
		}
		return boundingBox;
	}

	/// <summary>
	/// 获得附近物块的倾斜朝向
	/// </summary>
	/// <param name="postion"></param>
	/// <returns></returns>
	public Vector2 GetNormalOfTiles(Vector2 postion)
	{
		Vector2 normal = Vector2.Zero;
		for (int i = 0; i < 16; i++)
		{
			Vector2 check = new Vector2(0, SegmentHitBoxSize * 0.4f).RotatedBy(i / 16f * MathHelper.TwoPi);
			Vector2 checkWorld = postion + check;
			if (TileUtils.PlatformCollision(checkWorld))
			{
				normal -= check;
			}
			else
			{
				normal += check;
			}
		}
		if (normal.Length() < 0.1f)
		{
			return Vector2.zeroVector;
		}
		return Vector2.Normalize(normal);
	}

	/// <summary>
	/// 获得在一个方向上投影的高度
	/// </summary>
	/// <param name="postion"></param>
	/// <param name="normalDirection"></param>
	/// <returns></returns>
	public float CheckOverHeight(Vector2 postion, Vector2 normalDirection)
	{
		normalDirection = Vector2.Normalize(normalDirection);
		for (int i = 0; i < 1000; i++)
		{
			Vector2 check = normalDirection * i;
			Vector2 checkWorld = postion + check;
			if (TileUtils.PlatformCollision(checkWorld))
			{
				return i;
			}
		}
		return 1000;
	}
}

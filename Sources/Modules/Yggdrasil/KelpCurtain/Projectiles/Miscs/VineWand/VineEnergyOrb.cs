using Terraria.GameContent;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Miscs.VineWand;

public class VineEnergyOrb : ModProjectile
{
	public Vector2 StartPosition;
	public Vector2 EndPosition;

	public Vector2 TileDestination = Vector2.zeroVector;
	public float Progress = 0f;
	public float Speed = 0.03f;

	// 用于存储轨迹点以实现拖尾效果
	private List<Vector2> trailPositions = new List<Vector2>();
	private List<float> trailRotations = new List<float>();
	private const int MaxTrailLength = 20;

	// 添加动态效果变量
	private float timer = 0f;
	private float pulseTimer = 0f;
	private float pulse = 0f;

	public override void SetStaticDefaults()
	{
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
	}

	public override void SetDefaults()
	{
		Projectile.width = 4;
		Projectile.height = 4;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 180;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.alpha = 100;
	}

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];

		// 如果位置信息为零，则从ai参数读取
		if (StartPosition == Vector2.Zero)
		{
			StartPosition = Projectile.Center;
		}
		if (TileDestination == Vector2.zeroVector)
		{
			EndPosition = player.Center;
		}
		else
		{
			EndPosition = TileDestination;
		}

		// 移动能量球：沿着贝塞尔曲线从起点移动到终点
		Progress += Speed;

		// 计算贝塞尔曲线上的位置
		Vector2 currentPosition = CalculateBezierPosition(Progress);

		// 计算移动方向（用于拖尾旋转）
		Vector2 velocity = currentPosition - Projectile.Center;
		float rotation = velocity != Vector2.Zero ? velocity.ToRotation() : Projectile.rotation;

		Projectile.Center = currentPosition;
		Projectile.rotation = rotation;

		// 更新拖尾
		UpdateTrail(currentPosition, rotation);

		// 动态大小和透明度
		Projectile.scale = 0.3f + pulse * 0.15f;
		Projectile.alpha = (int)(80 + Math.Sin(pulseTimer * 2) * 20);

		// 生成粒子效果
		SpawnParticles(currentPosition);

		// 如果到达终点，销毁弹幕
		if (Progress >= 1f)
		{
			Projectile.Kill();
		}
	}

	private void SpawnParticles(Vector2 position)
	{
		// 随机生成粒子
		if (Main.rand.NextBool(60))
		{
			Color[] dustColors = new Color[]
			{
				new Color(30, 120, 30),
				new Color(60, 180, 60),
				new Color(120, 220, 120),
				new Color(180, 230, 100),
				new Color(220, 255, 150),
			};

			Color dustColor = dustColors[Main.rand.Next(dustColors.Length)];
			Dust dust = Dust.NewDustPerfect(
				position + Main.rand.NextVector2Circular(10, 10),
				DustID.TerraBlade,
				Main.rand.NextVector2Circular(2, 2),
				0, dustColor, 0.5f);
			dust.noGravity = true;
			dust.fadeIn = 1f;
		}
	}

	private void UpdateTrail(Vector2 newPosition, float newRotation)
	{
		// 添加新位置和旋转到拖尾
		trailPositions.Insert(0, newPosition);
		trailRotations.Insert(0, newRotation);

		// 限制拖尾长度
		if (trailPositions.Count > MaxTrailLength)
		{
			trailPositions.RemoveAt(trailPositions.Count - 1);
			trailRotations.RemoveAt(trailRotations.Count - 1);
		}
	}

	private Vector2 CalculateBezierPosition(float t)
	{
		// 计算控制点：在连线中点附近偏移，形成曲线
		Vector2 lineCenter = (StartPosition + EndPosition) / 2f;
		Vector2 lineDirection = (EndPosition - StartPosition).SafeNormalize(Vector2.UnitY);
		Vector2 perpendicular = new Vector2(-lineDirection.Y, lineDirection.X);

		// 使用基于时间的偏移，使曲线有动态变化
		float offsetAmount = 40f;
		float timeOffset = (float)Main.timeForVisualEffects * 0.03f;
		float sinOffset = (float)System.Math.Sin(timeOffset + Projectile.identity * 0.5f) * offsetAmount;

		Vector2 controlPoint = lineCenter + perpendicular * sinOffset;

		// 贝塞尔曲线计算
		float u = 1 - t;
		Vector2 position = u * u * StartPosition +
						  2 * u * t * controlPoint +
						  t * t * EndPosition;

		return position;
	}

	public override bool PreDraw(ref Color lightColor)
	{
		// 绘制细长拖尾
		DrawTrail();

		// 绘制能量球头部
		DrawHead();


		return false;
	}

	private void DrawHead()
	{
		Texture2D tex = ModAsset.ForestRainVine_Cut.Value;
		Texture2D bloom = ModAsset.ForestRainVine_Cut_Bloom.Value;
		Main.spriteBatch.Draw(bloom, Projectile.Center - Main.screenPosition, null, new Color(0.13f, 0.4f, 0.04f, 0f), Projectile.rotation + MathHelper.PiOver2, bloom.Size() * 0.5f, Projectile.ai[0], SpriteEffects.None, 0);
		Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, new Color(0.7f, 1f, 0.4f, 1f), Projectile.rotation + MathHelper.PiOver2, tex.Size() * 0.5f, Projectile.ai[0], SpriteEffects.None, 0);
	}

	private void DrawTrail()
	{
		Texture2D texture = TextureAssets.MagicPixel.Value;
		Rectangle sourceRect = new Rectangle(0, 0, 1, 1);

		for (int i = 0; i < trailPositions.Count - 1; i++)
		{
			float progress = (float)i / trailPositions.Count;
			Vector2 currentPos = trailPositions[i] - Main.screenPosition;
			Vector2 nextPos = trailPositions[i + 1] - Main.screenPosition;

			// 计算两点之间的向量
			Vector2 segmentVector = nextPos - currentPos;
			float segmentLength = segmentVector.Length();

			if (segmentLength > 0)
			{
				Vector2 segmentDirection = segmentVector / segmentLength;
				float segmentRotation = segmentDirection.ToRotation();

				// 拖尾逐渐变小和透明
				float trailAlpha = (1f - progress) * 0.7f;
				float trailWidth = 8f * (1f - progress) * 0.5f; // 宽度加倍

				// 根据拖尾位置使用不同颜色
				Color trailColor;
				if (progress < 0.2f)
				{
					trailColor = new Color(140, 250, 160, 255);
				}
				else if (progress < 0.3f)
				{
					trailColor = new Color(60, 140, 60, 30);
				}
				else if (progress < 0.7f)
				{
					trailColor = new Color(20, 90, 120, 15);
				}
				else
				{
					trailColor = new Color(1, 12, 70, 6);
				}

				trailColor *= Projectile.alpha / 255f;

				// 绘制细长拖尾段 - 使用矩形纹理
				Main.EntitySpriteDraw(
					texture,
					currentPos,
					sourceRect,
					Color.Black * (1 - progress) * 0.5f,
					segmentRotation,
					new Vector2(0, 0.5f),
					new Vector2(segmentLength, trailWidth * Projectile.ai[0]),
					SpriteEffects.None,
					0);
				Main.EntitySpriteDraw(
					texture,
					currentPos,
					sourceRect,
					trailColor,
					segmentRotation,
					new Vector2(0, 0.5f),
					new Vector2(segmentLength, trailWidth * Projectile.ai[0]),
					SpriteEffects.None,
					0);
			}

			if (i % 3 == 0)
			{
				Texture2D orbTexture = GetOrbTexture();
				float pointScale = 0.1f + (1f - progress) * 0.2f;
				Color pointColor = new Color(150, 255, 150, (int)(200 * (1f - progress))) * (Projectile.alpha / 255f);

				Main.EntitySpriteDraw(
					orbTexture,
					currentPos,
					null,
					pointColor,
					0f,
					orbTexture.Size() * 0.5f,
					pointScale * Projectile.ai[0],
					SpriteEffects.None,
					0);
			}
		}
	}

	private Texture2D GetOrbTexture()
	{
		return (Texture2D)ModContent.Request<Texture2D>(Texture);
	}
}

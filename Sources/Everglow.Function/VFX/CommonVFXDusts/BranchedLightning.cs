using Everglow.Commons.Enums;
using Everglow.Commons.Utilities;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX.Pipelines;

namespace Everglow.Commons.VFX.CommonVFXDusts;

public class BranchedLightningPipeline : Pipeline
{
	public override void Load()
	{
		effect = ModAsset.BranchedLightning;
		effect.Value.Parameters["uDotLight"].SetValue(ModAsset.Point.Value);
		effect.Value.Parameters["uDisplacement"].SetValue(ModAsset.Noise_perlin.Value);
		effect.Value.Parameters["uDisplaceIntensity"].SetValue(BranchedLightning.DISPLACE_INTENSITY);
		effect.Value.Parameters["uNoiseSize"].SetValue(BranchedLightning.NOISE_SIZE);
		effect.Value.Parameters["uLineProportion"].SetValue(BranchedLightning.LINE_PROPORTION);

		// 设置闪电边缘颜色
		effect.Value.Parameters["uEdgeColor"].SetValue(new Color(0, 200, 255).ToVector4());
		effect.Value.Parameters["uBlurProportion"].SetValue(BranchedLightning.BLUR_PROPORTION);
		effect.Value.Parameters["uTransitPeriod"].SetValue(BranchedLightning.TRANSIT_PERIOD);
		effect.Value.Parameters["uDeformPeriod"].SetValue(BranchedLightning.DEFORM_PERIOD);
	}

	public override void BeginRender()
	{
		var effect = this.effect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.Parameters["uDisplacementShift"].SetValue((float)Main.time);

		Texture2D lightningTrail = ModAsset.Trail.Value;
		Ins.Batch.BindTexture<Vertex2D>(lightningTrail);
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.AnisotropicClamp;
		Ins.Batch.Begin(BlendState.AlphaBlend, DepthStencilState.None, SamplerState.LinearClamp, RasterizerState.CullNone);
		effect.CurrentTechnique.Passes[0].Apply();
	}

	public override void EndRender()
	{
		Ins.Batch.End();
	}
}

[Pipeline(typeof(BranchedLightningPipeline), typeof(BloomPipeline))]
public class BranchedLightning : Visual
{
	public const int MAX_SEGMENTS = 12;
	public const float DEFAULT_RENDER_STRIP_WIDTH = 125f;

	// 生长
	public const float LENGTH_EXTENSION_SPEED = 175f; // 单位：像素/帧
	public const float DEFAULT_SEGMENT_LENGTH = 125f;
	public const float WIDTH_EXPANSION_RATE = 1 / 5f;
	public const float WIDTH_SHRINK_RATE = 1 / 10f;

	// 子分支
	public const float DEVIATION_ANGLE = (float)Math.PI / 4;
	public const int MAX_BRANCH_COUNT = 2;
	public const float WIDTH_DECAY = 0.85f;
	public const float LENGTH_DECAY = 0.95f;
	public const float CHILD_ANGLE_AMPLIFICATION = 1.5f;

	// 变形
	public const float DISPLACE_INTENSITY = 0.35f;
	public const float TRANSIT_PERIOD = 20f;
	public const float DEFORM_PERIOD = 120f;
	public const float NOISE_SIZE = 512;
	public const float ANGULAR_ACCELERATION_PROPORTION = 0.2f;
	public const float DEFAULT_ANGULAR_SPEED_LIMIT = (float)(Math.PI / 5);

	public const float LINE_PROPORTION = 0.1f;
	public const float EDGE_LINE_RATIO = 0.15f;
	public const float BLUR_PROPORTION = 0.02f;

	// 碰撞检测
	public const float COLLISION_DETECTION_UNIT = 2.5f;

	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public Vector2 Position;
	public float Rotation;
	public float Timer;
	public float MaxTime;

	private LightningNode lightningRoot;

	private void SetUp(float segmentLength, float renderStripWidth, float wiggleAngularSpeedLimit)
	{
		Active = true;
		Visible = true;
		Timer = 0;
		lightningRoot = new LightningNode(LINE_PROPORTION, Vector2.UnitX * segmentLength, segmentLength, renderStripWidth, wiggleAngularSpeedLimit);
	}

	public BranchedLightning()
	{
		SetUp(DEFAULT_SEGMENT_LENGTH, DEFAULT_RENDER_STRIP_WIDTH, DEFAULT_ANGULAR_SPEED_LIMIT);
	}

	public BranchedLightning(float segmentLength, float width, Vector2 Position, float Rotation, float MaxTime, float wiggleAngularSpeedLimit = DEFAULT_ANGULAR_SPEED_LIMIT)
	{
		this.Position = Position;
		this.Rotation = Rotation;
		this.MaxTime = MaxTime;
		SetUp(segmentLength, width / LINE_PROPORTION, wiggleAngularSpeedLimit);
	}

	public override void Update()
	{
		lightningRoot.Update(Position, Rotation);
		if (Collision.SolidCollision(Position, 0, 0))
		{
			Active = false;
			Visible = false;
		}
		Timer++;

		float remainingLife = MaxTime - Timer;
		if (remainingLife <= 0)
		{
			Active = false;
		}
		else if (remainingLife <= 1 / WIDTH_SHRINK_RATE)
		{
			lightningRoot.StartShrink();
		}
	}

	public override void Draw()
	{
		List<Vertex2D> barsList = new List<Vertex2D>();

		// 绘制底（边缘色）
		lightningRoot.AddVertexesToList(barsList, true);

		// 绘制主体（白色）
		lightningRoot.AddVertexesToList(barsList, false);

		Ins.Batch.Draw(barsList, PrimitiveType.TriangleList);
	}

	// LightningNode: 单独闪电节点
	private class LightningNode
	{
		// 长度(末尾位置)
		private Vector2 rawOffset;
		private Vector2 currentEndPos;
		private float lengthProgress;
		private float lengthSpeed;
		private float segmentLength;

		// 波动
		private float angularVel;
		private float wiggleAngularSpeedLimit;

		// 宽度
		private float widthProportion;
		private float widthProgress;
		private bool widthShrink;
		private float renderStripWidth;

		// 生成
		private int depth;
		private LightningNode parent;
		private List<LightningNode> children;

		// 噪波
		private float distortionX;

		public LightningNode(
			float nodeWidth,
			Vector2 rawOffset,
			float segmentLength = DEFAULT_SEGMENT_LENGTH,
			float renderStripWidth = DEFAULT_RENDER_STRIP_WIDTH,
			float wiggleAngularSpeedLimit = DEFAULT_ANGULAR_SPEED_LIMIT,
			LightningNode parentNode = null,
			int nodeDepth = 0)
		{
			this.widthShrink = false;
			this.rawOffset = rawOffset;
			this.parent = parentNode;
			this.depth = nodeDepth;
			this.children = null;
			this.segmentLength = segmentLength;
			this.renderStripWidth = renderStripWidth;
			this.wiggleAngularSpeedLimit = wiggleAngularSpeedLimit;
			this.distortionX = Main.rand.NextFloat();

			if (this.depth < MAX_SEGMENTS)
			{
				this.widthProgress = 0;
				this.widthProportion = nodeWidth;
			}
			else
			{
				this.widthProgress = 1f;
				this.widthProportion = 0;
			}

			if (parent != null)
			{
				this.lengthProgress = 0f;
				this.lengthSpeed = LENGTH_EXTENSION_SPEED / (rawOffset - parent.rawOffset).Length();
				this.currentEndPos = parent.currentEndPos;
				this.angularVel = Main.rand.NextFloat(-1, 1) * this.wiggleAngularSpeedLimit;
			}
			else
			{
				this.lengthProgress = 0f;
				this.lengthSpeed = 1;
				this.angularVel = 0;
			}
		}

		private Vector2? GetCollisionPosition()
		{
			if (parent == null)
			{
				int testCollisionWidth = (int)(renderStripWidth * widthProgress * LINE_PROPORTION);
				if (Collision.SolidCollision(currentEndPos - Vector2.One * testCollisionWidth * 0.5f, testCollisionWidth, testCollisionWidth))
				{
					return currentEndPos;
				}
			}
			else
			{
				Vector2 currentWorldPos = Vector2.Lerp(parent.currentEndPos, currentEndPos, lengthProgress);
				int detectIterations = (int)Math.Ceiling((currentWorldPos - parent.currentEndPos).Length() / COLLISION_DETECTION_UNIT);
				for (int i = 0; i < detectIterations; i++)
				{
					Vector2 testPos = Vector2.Lerp(parent.currentEndPos, currentWorldPos, (float)i / detectIterations);
					if (Collision.SolidCollision(testPos - Vector2.One * COLLISION_DETECTION_UNIT * 0.5f, (int)COLLISION_DETECTION_UNIT, (int)COLLISION_DETECTION_UNIT))
					{
						return testPos;
					}
				}
			}

			return null;
		}

		public void Update(Vector2 rootPosition, float lightningRotation)
		{
			// 更新位置和旋转
			if (parent == null)
			{
				currentEndPos = rootPosition;
			}
			else
			{
				float angleLimit = DEVIATION_ANGLE * ((depth - parent.depth <= 1) ? 1 : CHILD_ANGLE_AMPLIFICATION);
				float acceleration = Main.rand.NextFloat(-ANGULAR_ACCELERATION_PROPORTION, ANGULAR_ACCELERATION_PROPORTION) * this.wiggleAngularSpeedLimit;
				float updatedAngularVel = angularVel + acceleration;

				if (updatedAngularVel > this.wiggleAngularSpeedLimit)
				{
					updatedAngularVel = this.wiggleAngularSpeedLimit * 0.9f;
				}
				else if ((-updatedAngularVel) > this.wiggleAngularSpeedLimit)
				{
					updatedAngularVel = this.wiggleAngularSpeedLimit * (-0.9f);
				}
				angularVel = updatedAngularVel;

				if (Math.Abs(rawOffset.ToRotation() + angularVel) >= angleLimit)
				{
					angularVel *= -0.2f;
				}

				rawOffset = rawOffset.RotatedBy(angularVel);

				float parentRotation = 0;
				if (parent.parent != null)
				{
					// 获得parent的部分旋转
					parentRotation = parent.rawOffset.ToRotation() * 0.5f;
				}
				currentEndPos = parent.currentEndPos + rawOffset.RotatedBy(parentRotation + lightningRotation);
			}

			if (lengthProgress < 1f)
			{
				// 成长未完成
				lengthProgress += lengthSpeed;

				if (lengthProgress >= 1f)
				{
					lengthProgress = 1;

					if (depth < MAX_SEGMENTS)
					{
						CreateChildren();
					}
				}
			}
			else
			{
				// 成长已完成
				if (children != null)
				{
					foreach (LightningNode child in children)
					{
						child.Update(rootPosition, lightningRotation);
					}
					children.RemoveAll(child => (child.widthShrink && child.widthProgress <= 0));

					if (children.Count == 0)
					{
						widthProportion = 0;
					}
				}
			}

			// 更新宽度
			if (lengthProgress >= 1f && widthProgress < 1f && !widthShrink)
			{
				// 长度成长完成，未开始萎缩，尚未完全扩张
				widthProgress += WIDTH_EXPANSION_RATE;
			}
			else if (widthShrink && widthProgress > 0)
			{
				// 开始萎缩，且尚未萎缩完成
				widthProgress -= WIDTH_SHRINK_RATE;
			}

			// 测试碰撞
			Vector2? collisionPos = GetCollisionPosition();
			if (collisionPos != null)
			{
				CollisionKill((Vector2)collisionPos);
			}
		}

		/* 碰撞物块效果：
		 * - 碰撞闪电段宽度归零
		 * - 碰撞闪电段生长进度调整至碰撞点对应的进度（会继续尝试生长和生成子闪电段）
		 * - 随机生成电火花
		 * - 消除所有子闪电段
		 */
		private void CollisionKill(Vector2 collisionPos)
		{
			widthProgress = 0;

			if (Main.rand.NextFloat() < 0.25f)
			{
				Dust d = Dust.NewDustDirect(collisionPos, 0, 0, ModContent.DustType<ElectricMiddleDust>(), 0, 0);
				d.scale *= 0.2f;
			}

			children = null;
			if (parent != null)
			{
				lengthProgress = (parent.currentEndPos - collisionPos).Length() / (parent.currentEndPos - currentEndPos).Length();
			}
		}

		private void CreateChildren()
		{
			int branchCount = parent != null ? Main.rand.Next(0, MAX_BRANCH_COUNT) + (((depth - parent.depth) <= 1) ? 1 : 0) : 1;

			for (int i = 0; i < branchCount; i++)
			{
				if (i == 0)
				{
					children = new List<LightningNode>();
				}

				int childDepth = depth + 1;
				float childWidth = widthProportion * WIDTH_DECAY;
				float childLengthDecay = LENGTH_DECAY;
				float childAngle = Main.rand.NextFloat(-0.5f, 0.5f) * DEVIATION_ANGLE;
				if (i != 0)
				{
					// 非主分支
					childWidth *= 0.45f;
					childAngle *= CHILD_ANGLE_AMPLIFICATION;
					childDepth += 2;
				}

				children.Add(new LightningNode(
					childWidth,
					childAngle.ToRotationVector2() * childLengthDecay * rawOffset.Length(),
					segmentLength,
					renderStripWidth,
					wiggleAngularSpeedLimit,
					this,
					childDepth));
			}

			if (children == null)
			{
				widthProportion = 0;
			}
		}

		public void StartShrink()
		{
			widthShrink = true;
			if (children != null)
			{
				foreach (LightningNode child in children)
				{
					child.StartShrink();
				}
			}
		}

		public void AddVertexesToList(List<Vertex2D> barsList, bool drawEgde)
		{
			if (children != null)
			{
				foreach (LightningNode child in children)
				{
					child.AddVertexesToList(barsList, drawEgde);
				}
			}
			float multiplyFactor = drawEgde ? 1 : (1 - 2 * EDGE_LINE_RATIO);
			float useEdgeColor = drawEgde ? 1f : 0f;

			if (parent == null)
			{
				/* 绘制原点（光球）
				 * 传入shader的顶点属性为：
				 * - Position: 绘制四边形的顶点位置
				 * - color: (N/A, N/A, 0（绘制光球）, 是否使用闪电边缘颜色)
				 * - texCoord: 绘制四边形的顶点uv
				 */
				float currentWidth = 2 * renderStripWidth * LINE_PROPORTION * widthProgress * multiplyFactor;
				Color c = new Color(0, 0, 0, useEdgeColor);

				Vertex2D upperLeft = new Vertex2D(
					currentEndPos + currentWidth * new Vector2(-1, -1),
					c, new Vector3(0, 0, 0));
				Vertex2D upperRight = new Vertex2D(
					currentEndPos + currentWidth * new Vector2(1, -1),
					c, new Vector3(1, 0, 0));
				Vertex2D lowerLeft = new Vertex2D(
					currentEndPos + currentWidth * new Vector2(-1, 1),
					c, new Vector3(0, 1, 0));
				Vertex2D lowerRight = new Vertex2D(
					currentEndPos + currentWidth * new Vector2(1, 1),
					c, new Vector3(1, 1, 0));
				AddVertexesForOne(barsList, upperLeft, upperRight, lowerLeft, lowerRight);
			}
			else
			{
				/* 绘制闪电段
				 * 传入shader的顶点属性为：
				 * - Position: 绘制四边形的顶点位置
				 * - color: (噪波纹理u （用于使闪电段两端无位移）, 噪波纹理v, 1（绘制闪电段）, 是否使用闪电边缘颜色)
				 * - texCoord: （当前宽度，噪波uv在u方向上的随机位移，闪电段长度）
				 */
				Vector2 worldPos = Vector2.Lerp(parent.currentEndPos, currentEndPos, lengthProgress);
				Vector2 parentWorldPos = parent.currentEndPos;

				Vector2 normalDisplacement = GetRenderNormal();
				Vector2 parentNormalDisplacement = parent.GetRenderNormal();
				if (parentNormalDisplacement == Vector2.Zero)
				{
					parentNormalDisplacement = GetNormalDir() * 4 * parent.renderStripWidth * LINE_PROPORTION;
				}

				float currentWidth = widthProportion * widthProgress * multiplyFactor;
				float parentCurrentWidth = parent.widthProportion * parent.widthProgress * multiplyFactor;

				float offSetLength = rawOffset.Length();
				Vertex2D upperLeft = new Vertex2D(parentWorldPos - parentNormalDisplacement, new Color(0f, 0f, 1, useEdgeColor), new Vector3(parentCurrentWidth, distortionX, offSetLength));
				Vertex2D upperRight = new Vertex2D(worldPos - normalDisplacement, new Color(1f, 0f, 1, useEdgeColor), new Vector3(currentWidth, distortionX, offSetLength));
				Vertex2D lowerLeft = new Vertex2D(parentWorldPos + parentNormalDisplacement, new Color(0f, 1f, 1, useEdgeColor), new Vector3(parentCurrentWidth, distortionX, offSetLength));
				Vertex2D lowerRight = new Vertex2D(worldPos + normalDisplacement, new Color(1f, 1f, 1, useEdgeColor), new Vector3(currentWidth, distortionX, offSetLength));
				AddVertexesForOne(barsList, upperLeft, upperRight, lowerLeft, lowerRight);
			}
		}

		private void AddVertexesForOne(List<Vertex2D> barsList, Vertex2D upperLeft, Vertex2D upperRight, Vertex2D lowerLeft, Vertex2D lowerRight)
		{
			barsList.Add(upperLeft);
			barsList.Add(upperRight);
			barsList.Add(lowerLeft);
			barsList.Add(lowerLeft);
			barsList.Add(upperRight);
			barsList.Add(lowerRight);
		}

		private Vector2 GetNormalDir()
		{
			Vector2 renderNormal = parent == null ? Vector2.Zero : (currentEndPos - parent.currentEndPos).NormalizeSafe().RotatedBy(Math.PI / 2);

			return renderNormal;
		}

		private Vector2 GetRenderNormal()
		{
			return 0.5f * renderStripWidth * GetNormalDir();
		}
	}
}

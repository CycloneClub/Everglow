using Everglow.Commons.DataStructures;
using Everglow.Commons.MEAC.Enums;
using Everglow.Commons.Utilities;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.GameContent.Shaders;

namespace Everglow.Commons.MEAC;

public abstract class MeleeProj : ModProjectile, IWarpProjectile, IBloomProjectile
{
	/// <summary>
	/// The currant attack style. range from 0 to (<see cref="maxAttackType"/> - 1) (include).<br></br>
	/// If you do not override the style swithcing logic, this value loops inside the range, adding by 1 after a entire attack.
	/// </summary>
	public int currantAttackType = 0;

	/// <summary>
	/// Determine the range of <see cref="currantAttackType"/>.
	/// </summary>
	public int maxAttackType = 3;

	/// <summary>
	/// The tip of the weapon projectile compare with player.
	/// </summary>
	public Vector2 mainAxisDirection;

	/// <summary>
	/// The queue of old positions, it always updates by accumulating expired <see cref="mainAxisDirection"/>.
	/// </summary>
	public Queue<Vector2> slashTrail;

	/// <summary>
	/// Determine the max length of <see cref="slashTrail"/>.
	/// </summary>
	public int maxSlashTrailLength = 40;

	/// <summary>
	/// Local variable, running the animation of weapon attacking.
	/// </summary>
	public int timer = 0;

	/// <summary>
	/// Be true to allow hitting.
	/// </summary>
	public bool canHit = false;

	/// <summary>
	/// Be false to disable the slash effect.
	/// </summary>
	public bool useSlash = true;

	/// <summary>
	/// 断开左键是否自动结束攻击
	/// </summary>
	public bool autoEnd = true;

	/// <summary>
	/// 绑定AutoEnd参数, 用于判定上次攻击结束前是否按下鼠标左键从而实现连击
	/// </summary>
	public bool hasContinueLeftClick = false;

	/// <summary>
	/// 允许长按左键?(一般情况用来做重击)
	/// </summary>
	public bool canLongLeftClick = false;

	/// <summary>
	/// 绑定<see cref="canLongLeftClick"/>, 用于判定重击
	/// </summary>
	public int clickTimer = 0;

	/// <summary>
	/// 绑定<see cref="canLongLeftClick"/>, 用于判定重击所需要的蓄力时长
	/// </summary>
	public int maxClickTimer = 90;

	/// <summary>
	/// 能否穿墙攻击敌人
	/// </summary>
	public bool ignoreTile = false;

	/// <summary>
	/// 是否为长柄武器
	/// </summary>
	public bool longHandle = false;

	public float drawScaleFactor = 1f;

	public float disFromPlayer = 6;
	public MeleeTrailShaderType shaderType = MeleeTrailShaderType.ArcBladeAutoTransparent;
	public bool isRightClick = false;
	public bool useBloom;

	public override void SetDefaults()
	{
		Projectile.width = 30;
		Projectile.height = 15;
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 30;
		Projectile.extraUpdates = 1;
		Projectile.scale = 1f;
		Projectile.hostile = false;
		Projectile.friendly = true;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.penetrate = -1;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 15;
		Projectile.DamageType = DamageClass.Melee;
		SetDef();
		slashTrail = new Queue<Vector2>(maxSlashTrailLength + 1);
	}

	public virtual void SetDef()
	{
	}

	public string ShaderTypeName => shaderType.ToString();

	public Player Player => Main.player[Projectile.owner];

	public Vector2 MainVec_WithoutGravDir
	{
		get
		{
			Vector2 vec = mainAxisDirection;
			if (Player.gravDir == -1)
			{
				vec.Y *= -1;
			}

			return vec;
		}
	}

	public Vector2 MouseWorld_WithoutGravDir
	{
		get
		{
			Vector2 vec = Player.MouseWorld();
			if (Player.gravDir == -1)
			{
				vec = WrapY(vec);
			}

			return vec;
		}
	}

	public Vector2 ProjCenter_WithoutGravDir
	{
		get
		{
			Vector2 vec = Projectile.Center;
			if (Player.gravDir == -1)
			{
				vec = WrapY(vec);
			}

			return vec;
		}
	}

	private Vector2 WrapY(Vector2 vec)
	{
		vec.Y -= Main.screenPosition.Y;
		float d = vec.Y - Main.screenHeight / 2;
		vec.Y -= 2 * d;
		vec.Y += Main.screenPosition.Y;
		return vec;
	}

	public override void SendExtraAI(BinaryWriter writer)
	{
		writer.WriteVector2(mainAxisDirection);
		writer.Write(disFromPlayer);
		writer.Write(Projectile.spriteDirection);
	}

	public override void ReceiveExtraAI(BinaryReader reader)
	{
		mainAxisDirection = reader.ReadVector2();
		disFromPlayer = reader.ReadSingle();
		Projectile.spriteDirection = reader.ReadInt32();
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		modifiers.HitDirectionOverride = target.Center.X > Main.player[Projectile.owner].Center.X ? 1 : -1;
	}

	public float GetMeleeSpeed(Player player, float max = 100)
	{
		return Math.Min((player.GetAttackSpeed(DamageClass.Melee) - 1) * 100, max);
	}

	public override void AI()
	{
		Player.ListenMouseWorld();
		Player.heldProj = Projectile.whoAmI;
		Player.GetModPlayer<MEACPlayer>().isUsingMeleeProj = true;
		Projectile.Center = Player.Center + Utils.SafeNormalize(mainAxisDirection, Vector2.One) * disFromPlayer;
		canHit = false;
		Projectile.ownerHitCheck = !ignoreTile;
		Projectile.timeLeft++;
		Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, mainAxisDirection.ToRotation() - 1.57f);
		Attack();
		timer++;
		if (!canHit)
		{
			if (!isRightClick)
			{
				bool IsEnd = autoEnd ? !Player.controlUseItem || Player.dead : Player.dead;
				if (IsEnd)
				{
					End();
				}
			}
			else
			{
				bool IsEnd = autoEnd ? !Player.controlUseTile || Player.dead : Player.dead;
				if (IsEnd)
				{
					End();
				}
			}
		}
		if (!hasContinueLeftClick && timer > 15) // 大于1/60s即判定为下一击继续
		{
			if (Main.mouseLeft)
			{
				hasContinueLeftClick = true;
			}
		}
		if (canHit)
		{
			Player.direction = Projectile.spriteDirection;
		}

		if (useSlash)
		{
			slashTrail.Enqueue(mainAxisDirection);
			if (slashTrail.Count > maxSlashTrailLength)
			{
				slashTrail.Dequeue();
			}
		}
		else // 清空！
		{
			slashTrail.Clear();
		}
		if (canLongLeftClick)
		{
			if (Main.mouseLeft)
			{
				clickTimer++;
			}
			else
			{
				clickTimer = 0;
			}
		}
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		ScreenShake();
		base.OnHitNPC(target, hit, damageDone);
	}

	public virtual void Attack()
	{
	}

	public override bool ShouldUpdatePosition()
	{
		return false;
	}

	public virtual void ProduceWaterRipples(Vector2 beamDims)
	{
		var shaderData = (WaterShaderData)Terraria.Graphics.Effects.Filters.Scene["WaterDistortion"].GetShader();
		float waveSine = 1f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 20f);
		Vector2 ripplePos = Projectile.Center + new Vector2(beamDims.X * 0.5f, 0f).RotatedBy(mainAxisDirection.ToRotation());
		Color waveData = new Color(0.5f, 0.1f * Math.Sign(waveSine) + 0.5f, 0f, 1f) * Math.Abs(waveSine);
		shaderData.QueueRipple(ripplePos, waveData, beamDims, RippleShape.Square, mainAxisDirection.ToRotation());
	}

	public override void CutTiles()
	{
		DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
		var cut = new Terraria.Utils.TileActionAttempt(DelegateMethods.CutTiles);
		Vector2 beamStartPos = Projectile.Center;
		Vector2 beamEndPos = beamStartPos + mainAxisDirection;
		Utils.PlotTileLine(beamStartPos, beamEndPos, Projectile.width * Projectile.scale, cut);
	}

	public void ScreenShake()
	{
		ShakerManager.AddShaker(Player.Center + mainAxisDirection, new Vector2(0, -1).RotatedByRandom(MathHelper.TwoPi), 6, 0.8f, 16, 0.9f, 0.8f, 30);
	}

	public void NextAttackType()
	{
		if (!canHit && !autoEnd)
		{
			if (clickTimer >= maxClickTimer)
			{
				LeftLongThump();
				End();
			}
			if (!isRightClick)
			{
				if (!hasContinueLeftClick || Player.dead)
				{
					Player player = Main.player[Projectile.owner];
					End();
					player.GetModPlayer<MEACPlayer>().isUsingMeleeProj = false;
				}
			}
			else
			{
				if (!Player.controlUseTile || Player.dead)
				{
					Player player = Main.player[Projectile.owner];
					End();
					player.GetModPlayer<MEACPlayer>().isUsingMeleeProj = false;
				}
			}
		}
		hasContinueLeftClick = false;
		timer = 0;
		currantAttackType++;
		if (currantAttackType > maxAttackType)
		{
			currantAttackType = 0;
		}
	}

	public virtual void LeftLongThump()
	{
	}

	public virtual void End()
	{
		Player player = Main.player[Projectile.owner];
		TestPlayerDrawer Tplayer = player.GetModPlayer<TestPlayerDrawer>();
		player.legFrame = new Rectangle(0, 0, player.legFrame.Width, player.legFrame.Height);
		player.fullRotation = 0;
		player.legRotation = 0;
		Tplayer.HeadRotation = 0;
		Tplayer.HideLeg = false;
		player.legPosition = Vector2.Zero;
		Projectile.Kill();
		player.GetModPlayer<MEACPlayer>().isUsingMeleeProj = false;
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		return canHit
			&& Collision.CheckAABBvLineCollision2(
				targetHitbox.TopLeft(),
				targetHitbox.Size(),
				ProjCenter_WithoutGravDir + MainVec_WithoutGravDir * Projectile.scale * (longHandle ? 0.2f : 0.1f),
				ProjCenter_WithoutGravDir + MainVec_WithoutGravDir * Projectile.scale);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		DrawTrail(lightColor);
		DrawSelf(Main.spriteBatch, lightColor);
		ProduceWaterRipples(new Vector2(mainAxisDirection.Length(), 30));
		return false;
	}

	public virtual void DrawSelf(SpriteBatch spriteBatch, Color lightColor, Vector4 diagonal = default, Vector2 drawScale = default, Texture2D glowTexture = null)
	{
		if (diagonal == default)
		{
			diagonal = new Vector4(0, 1, 1, 0);
		}
		if (drawScale == default)
		{
			drawScale = new Vector2(0, 1);
			if (longHandle)
			{
				drawScale = new Vector2(-0.6f, 1);
			}
			drawScale *= drawScaleFactor;
		}
		Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
		Vector2 drawCenter = Projectile.Center - Main.screenPosition;

		SpriteBatchState sBS = GraphicsUtils.GetState(spriteBatch).Value;
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		DrawVertexByTwoLine(tex, lightColor, diagonal.XY(), diagonal.ZW(), drawCenter + mainAxisDirection * drawScale.X, drawCenter + mainAxisDirection * drawScale.Y);
		if (glowTexture != null)
		{
			DrawVertexByTwoLine(glowTexture, new Color(1f, 1f, 1f, 0), diagonal.XY(), diagonal.ZW(), drawCenter + mainAxisDirection * drawScale.X, drawCenter + mainAxisDirection * drawScale.Y);
		}

		spriteBatch.End();
		spriteBatch.Begin(sBS);
	}

	public void DrawVertexByTwoLine(Texture2D texture, Color drawColor, Vector2 textureCoordStart, Vector2 textureCoordEnd, Vector2 positionStart, Vector2 positionEnd)
	{
		Vector2 coordVector = textureCoordEnd - textureCoordStart;
		coordVector.X *= texture.Width;
		coordVector.Y *= texture.Height;
		float theta = MathF.Atan2(coordVector.Y, coordVector.X);
		Vector2 drawVector = positionEnd - positionStart;

		Vector2 mainVectorI = drawVector.RotatedBy(theta * -Projectile.spriteDirection) * MathF.Cos(theta);
		Vector2 mainVectorJ = drawVector.RotatedBy((theta - MathHelper.PiOver2) * -Projectile.spriteDirection) * MathF.Sin(theta);

		List<Vertex2D> vertex2Ds = new List<Vertex2D>
		{
			new Vertex2D(positionStart, drawColor, new Vector3(textureCoordStart, 0)),
			new Vertex2D(positionStart + mainVectorI, drawColor, new Vector3(textureCoordEnd.X, textureCoordStart.Y, 0)),

			new Vertex2D(positionStart + mainVectorJ, drawColor, new Vector3(textureCoordStart.X, textureCoordEnd.Y, 0)),
			new Vertex2D(positionEnd, drawColor, new Vector3(textureCoordEnd, 0)),
		};
		Main.graphics.GraphicsDevice.Textures[0] = texture;
		Main.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertex2Ds.ToArray(), 0, vertex2Ds.Count - 2);
	}

	public virtual float TrailAlpha(float factor) => MathHelper.Lerp(0f, 1, factor);

	public virtual string TrailShapeTex() => ModAsset.Melee_Mod;

	public virtual string TrailColorTex() => ModAsset.MEAC_Color1_Mod;

	public virtual BlendState TrailBlendState() => BlendState.NonPremultiplied;

	public virtual void DrawTrail(Color color)
	{
		List<Vector2> SmoothTrailX = GraphicsUtils.CatmullRom(slashTrail.ToList()); // 平滑
		var SmoothTrail = new List<Vector2>();
		for (int x = 0; x < SmoothTrailX.Count - 1; x++)
		{
			Vector2 vec = SmoothTrailX[x];

			SmoothTrail.Add(Vector2.Normalize(vec) * (vec.Length() + disFromPlayer));
		}
		if (slashTrail.Count != 0)
		{
			Vector2 vec = slashTrail.ToArray()[slashTrail.Count - 1];

			SmoothTrail.Add(Vector2.Normalize(vec) * (vec.Length() + disFromPlayer));
		}
		Vector2 center = Projectile.Center - Vector2.Normalize(mainAxisDirection) * disFromPlayer;
		int length = SmoothTrail.Count;
		if (length <= 3)
		{
			return;
		}

		Vector2[] trail = SmoothTrail.ToArray();
		var bars = new List<Vertex2D>();

		for (int i = 0; i < length; i++)
		{
			float factor = i / (length - 1f);
			float w = TrailAlpha(factor);
			if (!longHandle)
			{
				bars.Add(new Vertex2D(center + trail[i] * 0.15f * Projectile.scale, Color.White, new Vector3(factor, 1, 0f)));
				bars.Add(new Vertex2D(center + trail[i] * Projectile.scale, Color.White, new Vector3(factor, 0, w)));
			}
			else
			{
				bars.Add(new Vertex2D(center + trail[i] * 0.3f * Projectile.scale, Color.White, new Vector3(factor, 1, 0f)));
				bars.Add(new Vertex2D(center + trail[i] * Projectile.scale, Color.White, new Vector3(factor, 0, w)));
			}
		}
		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, TrailBlendState(), SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone);
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;

		Effect MeleeTrail = ModAsset.MeleeTrail.Value;
		MeleeTrail.Parameters["uTransform"].SetValue(model * projection);
		Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>(TrailShapeTex(), ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
		MeleeTrail.Parameters["tex1"].SetValue(ModContent.Request<Texture2D>(TrailColorTex(), ReLogic.Content.AssetRequestMode.ImmediateLoad).Value);
		MeleeTrail.CurrentTechnique.Passes[ShaderTypeName].Apply();

		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
	}

	public virtual void DrawWarp(VFXBatch spriteBatch)
	{
		List<Vector2> SmoothTrailX = GraphicsUtils.CatmullRom(slashTrail.ToList()); // 平滑
		var SmoothTrail = new List<Vector2>();
		for (int x = 0; x < SmoothTrailX.Count - 1; x++)
		{
			Vector2 vec = SmoothTrailX[x];

			SmoothTrail.Add(Vector2.Normalize(vec) * (vec.Length() + disFromPlayer));
		}
		if (slashTrail.Count != 0)
		{
			Vector2 vec = slashTrail.ToArray()[slashTrail.Count - 1];

			SmoothTrail.Add(Vector2.Normalize(vec) * (vec.Length() + disFromPlayer));
		}
		Vector2 center = Projectile.Center - Vector2.Normalize(mainAxisDirection) * disFromPlayer;
		int length = SmoothTrail.Count;
		if (length <= 3)
		{
			return;
		}

		Vector2[] trail = SmoothTrail.ToArray();
		var bars = new List<Vertex2D>();
		for (int i = 0; i < length; i++)
		{
			float factor = i / (length - 1f);
			float w = 0.02f;
			float d = trail[i].ToRotation() + 3.14f + 1.57f;
			if (d > 6.28f)
			{
				d -= 6.28f;
			}

			float dir = d / MathHelper.TwoPi;

			float dir1 = dir;
			if (i > 0)
			{
				float d1 = trail[i - 1].ToRotation() + 3.14f + 1.57f;
				if (d1 > 6.28f)
				{
					d1 -= 6.28f;
				}

				dir1 = d1 / MathHelper.TwoPi;
			}

			if (dir - dir1 > 0.5)
			{
				var midValue = (1 - dir) / (1 - dir + dir1);
				var midPoint = midValue * trail[i] + (1 - midValue) * trail[i - 1];
				midPoint.X = 0;
				var oldFactor = (i - 1) / (length - 1f);
				var midFactor = midValue * factor + (1 - midValue) * oldFactor;
				bars.Add(new Vertex2D(center - Main.screenPosition, new Color(0, w, 0, 1), new Vector3(midFactor, 1, 1)));
				bars.Add(new Vertex2D(center - Main.screenPosition + midPoint * Projectile.scale * 1.1f, new Color(0, w, 0, 1), new Vector3(midFactor, 0, 1)));
				bars.Add(new Vertex2D(center - Main.screenPosition, new Color(1, w, 0, 1), new Vector3(midFactor, 1, 1)));
				bars.Add(new Vertex2D(center - Main.screenPosition + midPoint * Projectile.scale * 1.1f, new Color(1, w, 0, 1), new Vector3(midFactor, 0, 1)));
			}
			if (dir1 - dir > 0.5)
			{
				var midValue = (1 - dir1) / (1 - dir1 + dir);
				var midPoint = midValue * trail[i - 1] + (1 - midValue) * trail[i];
				midPoint.X = 0;
				var oldFactor = (i - 1) / (length - 1f);
				var midFactor = midValue * oldFactor + (1 - midValue) * factor;
				bars.Add(new Vertex2D(center - Main.screenPosition, new Color(1, w, 0, 1), new Vector3(midFactor, 1, 1)));
				bars.Add(new Vertex2D(center - Main.screenPosition + midPoint * Projectile.scale * 1.1f, new Color(1, w, 0, 1), new Vector3(midFactor, 0, 1)));
				bars.Add(new Vertex2D(center - Main.screenPosition, new Color(0, w, 0, 1), new Vector3(midFactor, 1, 1)));
				bars.Add(new Vertex2D(center - Main.screenPosition + midPoint * Projectile.scale * 1.1f, new Color(0, w, 0, 1), new Vector3(midFactor, 0, 1)));
			}
			bars.Add(new Vertex2D(center - Main.screenPosition, new Color(dir, w, 0, 1), new Vector3(factor, 1, w)));
			bars.Add(new Vertex2D(center - Main.screenPosition + trail[i] * Projectile.scale * 1.1f, new Color(dir, w, 0, 1), new Vector3(factor, 0, w)));
		}

		spriteBatch.Draw(ModContent.Request<Texture2D>(ModAsset.Melee_Warp_Mod).Value, bars, PrimitiveType.TriangleStrip);
	}

	/// <summary>
	/// 根据鼠标位置锁定玩家方向
	/// </summary>
	/// <param name="player"></param>
	public void LockPlayerDir(Player player)
	{
		Projectile.spriteDirection = player.MouseWorld().X > player.Center.X ? 1 : -1;
		player.direction = Projectile.spriteDirection;
	}

	/// <summary>
	/// 圆的透视投影
	/// </summary>
	/// <param name="radius"></param>
	/// <param name="rot0"></param>
	/// <param name="rot1"></param>
	/// <param name="rot2"></param>
	/// <param name="viewZ"></param>
	/// <returns></returns>
	public static Vector2 Vector2Elipse(float radius, float rot0, float rot1, float rot2 = 0, float viewZ = 1000)
	{
		Vector3 v = Vector3.Transform(Vector3.UnitX, Matrix.CreateRotationZ(rot0)) * radius;
		v = Vector3.Transform(v, Matrix.CreateRotationX(-rot1));
		if (rot2 != 0)
		{
			v = Vector3.Transform(v, Matrix.CreateRotationZ(-rot2));
		}

		float k = -viewZ / (v.Z - viewZ);
		return k * new Vector2(v.X, v.Y);
	}

	public float GetAngToMouse()
	{
		Vector2 vec = MouseWorld_WithoutGravDir - Main.player[Projectile.owner].Center;
		if (vec.X < 0)
		{
			vec = -vec;
		}

		return -vec.ToRotation();
	}

	public void AttSound(SoundStyle sound) => SoundEngine.PlaySound(sound, Projectile.Center);

	public void DrawBloom()
	{
		if (useBloom)
		{
			DrawTrail(Color.White);
		}
	}
}

using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;

namespace Everglow.Commons.Templates.Weapons.Whips;

public abstract class WhipProjectile : ModProjectile
{
	public List<Vector2> WhipPointsForCollision = new List<Vector2>();

	public override void SetDefaults()
	{
		SegmentCount = 20;
		DustType = 0;
		WhipLength = 300;
		VerticalFrameCount = 5;
		DefaultToWhip();
		SetDef();
	}

	public virtual void SetDef()
	{
	}

	/// <summary>
	/// The dust that will generate by whip.
	/// </summary>
	public int DustType;

	/// <summary>
	/// How many SegmentCount of this whip, default to 20.
	/// </summary>
	public int SegmentCount;

	/// <summary>
	/// The max range this whip can reach, default to 300.
	/// </summary>
	public float WhipLength;

	/// <summary>
	/// The animation(max) time of this whip.
	/// </summary>
	public float TimeToFlyOut;

	/// <summary>
	/// The vertical frames of whip texture.
	/// </summary>
	public int VerticalFrameCount;

	public void DefaultToWhip()
	{
		Projectile.width = 18;
		Projectile.height = 18;
		Projectile.aiStyle = -1; // 165
		Projectile.friendly = true;
		Projectile.penetrate = -1;
		Projectile.tileCollide = false;
		Projectile.scale = 1f;
		Projectile.ownerHitCheck = true;
		Projectile.extraUpdates = 1;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = -1;
		Projectile.DamageType = DamageClass.Summon;
	}

	public override void OnSpawn(IEntitySource source)
	{
		Player player = Main.player[Projectile.owner];
		player.itemAnimationMax = (int)(player.itemAnimationMax / player.meleeSpeed);
		player.itemAnimation = player.itemAnimationMax;
		player.itemTimeMax = player.itemAnimationMax;
		player.itemTime = player.itemAnimationMax;
		TimeToFlyOut = player.itemAnimationMax * Projectile.MaxUpdates;
	}

	public override void CutTiles()
	{
		var value = new Vector2(Projectile.width * Projectile.scale / 2f, 0f);
		for (int i = 0; i < WhipPointsForCollision.Count; i++)
		{
			DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
			Utils.PlotTileLine(WhipPointsForCollision[i] - value, WhipPointsForCollision[i] + value, Projectile.height * Projectile.scale, new Utils.TileActionAttempt(DelegateMethods.CutTiles));
		}
		base.CutTiles();
	}

	public override bool? CanCutTiles()
	{
		return true;
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		float lineSpeedMax = WhipLength / TimeToFlyOut / 3f;
		if (lineSpeedMax >= 1)
		{
			for (int t = 0; t < lineSpeedMax; t++)
			{
				var list0 = new List<Vector2>();
				float fade = 1 - t / MathF.Floor(lineSpeedMax + 1);
				FillWhipControlPoints(list0, (1 - fade) * 3);
				for (int i = 0; i < list0.Count; i++)
				{
					var point = list0[i].ToPoint();
					projHitbox.Location = new Point(point.X - projHitbox.Width / 2, point.Y - projHitbox.Height / 2);
					if (projHitbox.Intersects(targetHitbox))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public override void AI()
	{
		AI_165_Whip();
		WhipPointsForCollision.Clear();
		FillWhipControlPoints(WhipPointsForCollision);
		return;
	}

	/// <summary>
	/// Whip AI after adjusted.
	/// Projectile.ai[0] work as a timer, you should not change it.
	///
	/// </summary>
	public virtual void AI_165_Whip()
	{
		Player player = Main.player[Projectile.owner];
		Projectile.rotation = Projectile.velocity.ToRotation() + 1.5707964f;
		Projectile.ai[0] += 1f;
		float rangeMultiplier = WhipLength / 300f;
		Projectile.tileCollide = false;
		Projectile.Center = Main.GetPlayerArmPosition(Projectile) + Projectile.velocity * (Projectile.ai[0] - 1f);
		Projectile.spriteDirection = Vector2.Dot(Projectile.velocity, Vector2.UnitX) < 0f ? -1 : 1;
		if (Projectile.ai[0] >= TimeToFlyOut || player.itemAnimation == 0)
		{
			Projectile.Kill();
			return;
		}
		player.heldProj = Projectile.whoAmI;
		if (Projectile.ai[0] == (int)(TimeToFlyOut / 2f))
		{
			Vector2 position = WhipPointsForCollision[WhipPointsForCollision.Count - 1];
			SoundEngine.PlaySound(SoundID.Item153, position);
		}
		GenerateDusts();
	}

	public virtual void GenerateDusts()
	{
		Player player = Main.player[Projectile.owner];
		float t = Projectile.ai[0] / TimeToFlyOut;
		if (Utils.GetLerpValue(0.1f, 0.7f, t, true) * Utils.GetLerpValue(0.9f, 0.7f, t, true) > 0.5f)
		{
			float times = (Main.rand.NextFloat(3) - 1) * player.meleeSpeed;
			if (times < 0)
			{
				times = 0;
			}
			if (WhipPointsForCollision.Count > 10)
			{
				for (int x = 0; x < times; x++)
				{
					int randSegment = Main.rand.Next(WhipPointsForCollision.Count - 10, WhipPointsForCollision.Count);
					Rectangle rectangle = Utils.CenteredRectangle(WhipPointsForCollision[randSegment], new Vector2(30f, 30f));
					var dust = Dust.NewDustDirect(rectangle.TopLeft(), rectangle.Width, rectangle.Height, DustType, 0f, 0f, 100, Color.White, 1f);
					dust.position = WhipPointsForCollision[randSegment];
					dust.fadeIn = 0.3f;
					Vector2 spinningpoint = WhipPointsForCollision[randSegment] - WhipPointsForCollision[randSegment - 1];
					dust.noGravity = true;
					dust.velocity *= 0.5f;
					dust.velocity += spinningpoint.RotatedBy((double)(player.direction * 1.5707964f), default);
					dust.velocity *= 0.5f;
				}
			}
		}
	}

	public virtual void FillWhipControlPoints(List<Vector2> controlPoints, float deltaStep = 0)
	{
		float rangeMultiplier = WhipLength / 300f;
		float duration = (Projectile.ai[0] - deltaStep) / TimeToFlyOut;
		if (duration < 0)
		{
			duration = 0;
		}
		float duration15 = 1.5f;
		float maxRotation = MathHelper.Pi * 10f * (1f - duration * duration15) * -Projectile.spriteDirection / SegmentCount;
		float durationSquare15 = duration * duration15;
		float factor = 0f;
		if (durationSquare15 > 1f)
		{
			factor = (durationSquare15 - 1f) / 0.5f;
			durationSquare15 = MathHelper.Lerp(1f, 0f, factor);
		}
		Player player = Main.player[Projectile.owner];
		Item heldItem = player.HeldItem;
		float durationValue = heldItem.useAnimation * 2 * duration * player.whipRangeMultiplier;
		durationValue = Projectile.velocity.Length() * durationValue * durationSquare15 * rangeMultiplier / SegmentCount;
		Vector2 startArmPosition = Main.GetPlayerArmPosition(Projectile);
		Vector2 value0 = startArmPosition;
		float alpha = -MathHelper.PiOver2;
		Vector2 value1 = value0;
		float beta = MathHelper.PiOver2 + MathHelper.PiOver2 * Projectile.spriteDirection;
		Vector2 value2 = value0;
		float gama = MathHelper.PiOver2;
		controlPoints.Add(startArmPosition);
		for (int i = 0; i < SegmentCount; i++)
		{
			float segmentPrecent = i / (float)SegmentCount;
			float rotProcess = maxRotation * segmentPrecent;
			Vector2 vector0 = value0 + alpha.ToRotationVector2() * durationValue;
			Vector2 vector1 = value1 + beta.ToRotationVector2() * (durationValue * 2f);
			Vector2 vector2 = value2 + gama.ToRotationVector2() * (durationValue * 2f);
			float duration0 = 1f - durationSquare15;
			float duration1 = 1f - duration0 * duration0;
			var value3 = Vector2.Lerp(vector2, vector0, duration1 * 0.9f + 0.1f);
			var value4 = Vector2.Lerp(vector1, value3, duration1 * 0.7f + 0.3f);
			Vector2 spinningpoint = startArmPosition + (value4 - startArmPosition) * new Vector2(1f, duration15);
			Vector2 thisPos = spinningpoint.RotatedBy((double)(Projectile.rotation + 3 * MathHelper.PiOver2 * factor * factor * Projectile.spriteDirection), startArmPosition);
			controlPoints.Add(thisPos);
			alpha += rotProcess;
			gama += rotProcess;
			beta += rotProcess;
			value0 = vector0;
			value1 = vector1;
			value2 = vector2;
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		float lineSpeedMax = WhipLength / TimeToFlyOut / 3f;
		// draw ghost image when line speed very fast.
		if (lineSpeedMax >= 1)
		{
			for (int t = 1; t < lineSpeedMax; t++)
			{
				float fade = 1 - t / MathF.Floor(lineSpeedMax + 1);
				DrawWhip((1 - fade) * 3);
			}
		}
		DrawWhip();
		return false;
	}

	public virtual void DrawWhip(float foreStep = 0)
	{
		Texture2D mainTexture = TextureAssets.Projectile[Projectile.type].Value;
		int frameHeight = mainTexture.Height / VerticalFrameCount;

		var list0 = new List<Vector2>();
		FillWhipControlPoints(list0, foreStep);
		for (int i = 0; i < list0.Count - 1; i++)
		{
			int frame = (int)(i / (float)(list0.Count - 1) * VerticalFrameCount);
			if (frame == 0 && i > 0)
			{
				frame = 1;
			}
			if (frame != 0 && i == 0)
			{
				frame = 0;
			}
			var rectangle = new Rectangle(0, frameHeight * frame, mainTexture.Width, frameHeight);
			var origin = new Vector2(rectangle.Width / 2, 2f);
			Vector2 positionNow = list0[i];
			Vector2 positionAdd = list0[i + 1] - positionNow;
			float rotation = positionAdd.ToRotation() - MathHelper.PiOver2;
			Color color = Lighting.GetColor(positionNow.ToTileCoordinates());
			if (foreStep != 0)
			{
				color *= (1 - foreStep / 3f) * 0.2f;
			}
			var scale = new Vector2(1f, (positionAdd.Length() + 2f) / rectangle.Height * 2f);
			Main.spriteBatch.Draw(mainTexture, list0[i] - Main.screenPosition, new Rectangle?(rectangle), color, rotation, origin, scale, SpriteEffects.None, 0f);
		}
	}
}

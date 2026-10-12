using System.Reflection;
using Everglow.Commons.CustomTiles;
using Everglow.Commons.Utilities;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace Everglow.Commons.Templates.Weapons.Yoyos;

public abstract class YoyoProjectile : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeProjectiles;

	/// <summary>
	/// Maximum seconds the yoyo will remain deployed before returning.
	/// Set less than 0 for infinite lifetime.
	/// <br/>Default to -1f.
	/// <para/> Set <see cref="ProjectileID.Sets.YoyosLifeTimeMultiplier"/> to change.
	/// </summary>
	public float YoyosLifeTimeMultiplier => ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type];

	/// <summary>
	/// Maximum string length. Default to 200f.
	/// <para/> Set <see cref="ProjectileID.Sets.YoyosMaximumRange"/> to change.
	/// </summary>
	public float YoyosMaximumRange => ProjectileID.Sets.YoyosMaximumRange[Projectile.type];

	/// <summary>
	/// Acceleration used by the yoyo when moving toward the target position. Default to 10f.
	/// <para/> Set <see cref="ProjectileID.Sets.YoyosTopSpeed"/> to change.
	/// </summary>
	public float YoyosTopSpeed => ProjectileID.Sets.YoyosTopSpeed[Projectile.type];

	/// <summary>
	/// Angular velocity applied to the yoyo's rotation each tick. Default to 0.45f.
	/// </summary>
	public float RotationalSpeed { get; protected set; }

	/// <summary>
	/// Inverse factor for rebound speed when hitting an NPC. Higher values reduce bounce velocity.
	/// <br/>Default to 4f.
	/// </summary>
	public float Weight { get; protected set; }

	/// <summary>
	/// Points that form the rendered yoyo string (from player to yoyo). Use <see cref="GenerateYoyo_String"/> to populate.
	/// </summary>
	public List<Vector2> YoyoStringPos { get; protected set; } = [];

	/// <summary>
	/// Target X position (World Coordinate) for the yoyo to move toward. Set to -1 to return to player.
	/// </summary>
	public ref float TargetX => ref Projectile.ai[0];

	/// <summary>
	/// Target Y position (World Coordinate) for the yoyo to move toward.
	/// </summary>
	public ref float TargetY => ref Projectile.ai[1];

	/// <summary>
	/// Used to control the behavior of the yoyo over time. Not always time in seconds.
	/// <br/> Make difference between yoyo projectiles by adding different values per tick.
	/// </summary>
	public ref float DeployTimer => ref Projectile.localAI[0];

	public override void SetStaticDefaults()
	{
		// The following sets are only applicable to yoyo that use aiStyle 99.

		// YoyosLifeTimeMultiplier is how long in seconds the yoyo will stay out before automatically returning to the player.
		// Vanilla values range from 3f (Wood) to 16f (Chik), and defaults to -1f. Leaving as -1 will make the time infinite.
		ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = -1;

		// YoyosMaximumRange is the maximum distance the yoyo sleep away from the player.
		// Vanilla values range from 130f (Wood) to 400f (Terrarian), and defaults to 200f.
		ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 200f;

		// YoyosTopSpeed is top speed of the yoyo Projectile.
		// Vanilla values range from 9f (Wood) to 17.5f (Terrarian), and defaults to 10f.
		ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 10f;
	}

	public sealed override void SetDefaults()
	{
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.penetrate = -1;
		Projectile.width = 16;
		Projectile.height = 16;
		Projectile.scale = 1f;

		Projectile.aiStyle = -1;
		Projectile.DamageType = DamageClass.MeleeNoSpeed;

		RotationalSpeed = 0.45f;
		Weight = 4f;
		SetCustomDefaults();
	}

	public virtual void SetCustomDefaults()
	{
	}

	public override void Load()
	{
		Ins.HookManager.AddHook(typeof(Projectile).GetMethod(nameof(Projectile.AI_099_1), BindingFlags.NonPublic | BindingFlags.Instance), ILHook_Projectile_Counterweight);
	}

	/// <summary>
	/// Injects IL to adjust the counterweight check inside Projectile.AI_099_1 so modded yoyo projectiles are handled.
	/// </summary>
	/// <param name="il">The IL context provided by MonoMod for the hook.</param>
	private void ILHook_Projectile_Counterweight(ILContext il)
	{
		ILCursor cursor = new ILCursor(il);

		var isNotModYoyoProjLabel = cursor.DefineLabel();
		if (cursor.TryGotoNext(
			MoveType.Before,
			x => x.MatchLdsfld(typeof(Main).GetField("projectile")),
			x => x.MatchLdloc(10),
			x => x.MatchLdelemRef(),
			x => x.MatchLdfld<Projectile>("aiStyle"),
			x => x.MatchLdcI4(99)))
		{
			cursor.MarkLabel(isNotModYoyoProjLabel);
		}

		var isYoyoProjLabel = cursor.DefineLabel();
		if (cursor.TryGotoNext(
			MoveType.After,
			x => x.MatchLdfld<Projectile>("type"),
			x => x.MatchLdcI4(561),
			x => x.MatchBle(out _)))
		{
			cursor.MarkLabel(isYoyoProjLabel);
		}

		if (cursor.TryGotoPrev(
			MoveType.After,
			x => x.MatchLdfld<Projectile>("owner"),
			x => x.MatchLdarg0(),
			x => x.MatchLdfld<Projectile>("owner"),
			x => x.MatchBneUn(out _))) // find the following branch instruction
		{
			cursor.EmitLdsfld(typeof(Main).GetField("projectile"));
			cursor.EmitLdloc(10);
			cursor.EmitLdelemRef();
			cursor.EmitCallvirt(typeof(Projectile).GetMethod("get_ModProjectile"));
			cursor.EmitBrfalse(isNotModYoyoProjLabel);

			cursor.EmitLdsfld(typeof(Main).GetField("projectile"));
			cursor.EmitLdloc(10);
			cursor.EmitLdelemRef();
			cursor.Emit(OpCodes.Callvirt, typeof(Projectile).GetMethod("get_ModProjectile"));
			cursor.EmitIsinst(typeof(YoyoProjectile));
			cursor.EmitBrtrue(isYoyoProjLabel);
		}
	}

	public override void AI()
	{
		YoyoAI();
	}

	/// <summary>
	/// Apply the bounce effect and counterweight/yoyo glove effect on hit.
	/// </summary>
	/// <param name="target"></param>
	/// <param name="hit"></param>
	/// <param name="damageDone"></param>
	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		ModifyOnHitBounce(target, hit, damageDone);
		ModifyCounterWeightAndYoyoGloveEffect(target.Center, Projectile.damage, Projectile.knockBack);
	}

	public virtual void ModifyOnHitBounce(NPC target, NPC.HitInfo hit, int damageDone)
	{
		Projectile.velocity += Vector2.Normalize(Projectile.Center - target.Center) / (Weight + 0.01f) * 100f;
	}

	/// <summary>
	/// Vanilla function, spawn counterweight and the other yoyo on hit.
	/// </summary>
	/// <param name="hitPos"></param>
	/// <param name="damage"></param>
	/// <param name="knockBack"></param>
	public virtual void ModifyCounterWeightAndYoyoGloveEffect(Vector2 hitPos, int damage, float knockBack)
	{
		if (Projectile.owner < 0 || Projectile.owner >= Main.player.Length)
		{
			return;
		}
		Player player = Main.player[Projectile.owner];
		if (!player.yoyoGlove && player.counterWeight <= 0)
		{
			return;
		}

		int lastYoyoWhoAmI = -1;
		int ownYoyoCount = 0;
		int ownCounterWeightCount = 0;
		for (int i = 0; i < 1000; i++)
		{
			Projectile proj = Main.projectile[i];
			if (proj.active && proj.owner == player.whoAmI)
			{
				if (proj.counterweight)
				{
					ownCounterWeightCount++;
				}
				else if (proj.aiStyle == ProjAIStyleID.Yoyo || proj.ModProjectile is YoyoProjectile)
				{
					ownYoyoCount++;
					lastYoyoWhoAmI = i;
				}
			}
		}

		if (lastYoyoWhoAmI < 0 || lastYoyoWhoAmI >= Main.projectile.Length)
		{
			return;
		}

		Projectile parentProj = Main.projectile[lastYoyoWhoAmI];
		if (player.yoyoGlove && ownYoyoCount < 2)
		{
			Vector2 vector = hitPos - player.Center;
			vector.Normalize();
			vector *= 16f;
			Projectile.NewProjectile(Projectile.InheritSource(parentProj), player.Center, vector, parentProj.type, parentProj.damage, parentProj.knockBack, player.whoAmI, 1f);
		}
		else if (ownCounterWeightCount < ownYoyoCount)
		{
			Vector2 vector2 = hitPos - player.Center;
			vector2.Normalize();
			vector2 *= 16f;
			knockBack = (knockBack + 6f) / 2f;
			IEntitySource spawnSource = Projectile.InheritSource(parentProj);
			if (ownCounterWeightCount > 0)
			{
				Projectile.NewProjectile(spawnSource, player.Center, vector2, player.counterWeight, (int)(damage * 0.8f), knockBack, player.whoAmI, 1f);
			}
			else
			{
				Projectile.NewProjectile(spawnSource, player.Center, vector2, player.counterWeight, (int)(damage * 0.8f), knockBack, player.whoAmI);
			}
		}
	}

	public override bool OnTileCollide(Vector2 oldVelocity) => false;

	/// <summary>
	/// Yoyo AI from vanilla. Based on vanilla <see cref="Projectile.AI_099_2"/>.
	/// </summary>
	public virtual void YoyoAI()
	{
		Player player = Main.player[Projectile.owner];

		bool isSubYoyoProj = false; // Used for additional yoyos from Yoyo Glove.
		float maxRange = YoyosMaximumRange;
		for (int i = 0; i < Projectile.whoAmI; i++)
		{
			if (Main.projectile[i].active && Main.projectile[i].owner == Projectile.owner && Main.projectile[i].type == Projectile.type)
			{
				isSubYoyoProj = true;
			}
		}

		if (Projectile.owner == Main.myPlayer)
		{
			DeployTimer += 1f;
			if (isSubYoyoProj)
			{
				DeployTimer += Main.rand.NextFloat(1.0f, 3.1f);
			}

			float elapsedSeconds = DeployTimer / 60f;
			elapsedSeconds /= (1f + player.meleeSpeed) / 2f;
			if (elapsedSeconds > YoyosLifeTimeMultiplier && YoyosLifeTimeMultiplier > 0)
			{
				TargetX = -1f;
			}
		}
		if (player.dead)
		{
			Projectile.Kill();
			return;
		}
		if (!isSubYoyoProj)
		{
			player.heldProj = Projectile.whoAmI;
			player.SetDummyItemTime(2);
			if (Projectile.Center.X > player.Center.X)
			{
				player.ChangeDir(1);
				Projectile.direction = 1;
			}
			else
			{
				player.ChangeDir(-1);
				Projectile.direction = -1;
			}
		}
		if (Projectile.velocity.HasNaNs())
		{
			Projectile.Kill();
		}

		Projectile.timeLeft = 6;
		if (player.yoyoString)
		{
			maxRange = YoyosMaximumRange * 1.25f + 30f;
		}
		maxRange /= (1f + player.inverseMeleeSpeed * 3f) / 4f;
		float topSpeed = YoyosTopSpeed / ((1f + player.inverseMeleeSpeed * 3f) / 4f);

		float velocityLerpDivisor = MathF.Max(14f - topSpeed / 2f, 1.01f); // controls smoothing of velocity updates

		float accelThreshold = 5f + topSpeed / 2f;
		if (isSubYoyoProj)
		{
			accelThreshold += 20f;
		}

		if (TargetX >= 0f)
		{
			if (Projectile.velocity.Length() > topSpeed)
			{
				Projectile.velocity *= 0.98f;
			}

			bool isBeyondRange = false;
			bool isFarBeyondRange = false;
			Vector2 vectorToPlayer = player.Center - Projectile.Center;
			if (vectorToPlayer.Length() > maxRange)
			{
				isBeyondRange = true;
				if (vectorToPlayer.Length() > maxRange * 1.3)
				{
					isFarBeyondRange = true;
				}
			}
			if (Projectile.owner == Main.myPlayer)
			{
				if (!player.channel || player.stoned || player.frozen)
				{
					TargetX = -1f;
					TargetY = 0f;
					Projectile.netUpdate = true;
				}
				else
				{
					Vector2 targetWorld = Main.ReverseGravitySupport(Main.MouseScreen) + Main.screenPosition;
					float targetX = targetWorld.X;
					float targetY = targetWorld.Y;
					Vector2 offsetToPlayer = new Vector2(targetX, targetY) - player.Center;
					if (offsetToPlayer.Length() > maxRange)
					{
						offsetToPlayer.Normalize();
						offsetToPlayer *= maxRange;
						offsetToPlayer = player.Center + offsetToPlayer;
						targetX = offsetToPlayer.X;
						targetY = offsetToPlayer.Y;
					}

					if (TargetX != targetX || TargetY != targetY)
					{
						var clampedTarget = new Vector2(targetX, targetY);
						Vector2 clampedOffset = clampedTarget - player.Center;
						if (clampedOffset.Length() > maxRange - 1f)
						{
							clampedOffset.Normalize();
							clampedOffset *= maxRange - 1f;
							clampedTarget = player.Center + clampedOffset;
							targetX = clampedTarget.X;
							targetY = clampedTarget.Y;
						}
						TargetX = targetX;
						TargetY = targetY;
						Projectile.netUpdate = true;
					}
				}
			}
			if (isFarBeyondRange && Projectile.owner == Main.myPlayer)
			{
				TargetX = -1f;
				Projectile.netUpdate = true;
			}
			if (TargetX >= 0f)
			{
				if (isBeyondRange)
				{
					velocityLerpDivisor /= 2f;
					topSpeed *= 2f;
					if (Projectile.Center.X > player.Center.X && Projectile.velocity.X > 0f)
					{
						Projectile.velocity.X *= 0.5f;
					}

					if (Projectile.Center.Y > player.Center.Y && Projectile.velocity.Y > 0f)
					{
						Projectile.velocity.Y *= 0.5f;
					}

					if (Projectile.Center.X < player.Center.X && Projectile.velocity.X > 0f)
					{
						Projectile.velocity.X *= 0.5f;
					}

					if (Projectile.Center.Y < player.Center.Y && Projectile.velocity.Y > 0f)
					{
						Projectile.velocity.Y *= 0.5f;
					}
				}

				Vector2 toTarget = new Vector2(TargetX, TargetY) - Projectile.Center;
				if (isBeyondRange)
				{
					velocityLerpDivisor = 1f;
				}

				float distanceToTarget = toTarget.Length();
				if (distanceToTarget > accelThreshold)
				{
					toTarget.Normalize();
					float computedSpeed = Math.Min(distanceToTarget / 2f, topSpeed);
					if (isBeyondRange)
					{
						computedSpeed = Math.Min(computedSpeed, topSpeed / 2f);
					}

					toTarget *= computedSpeed;
					Projectile.velocity = (Projectile.velocity * (velocityLerpDivisor - 1f) + toTarget) / velocityLerpDivisor;
				}
				else if (isSubYoyoProj)
				{
					if ((double)Projectile.velocity.Length() < (double)topSpeed * 0.6)
					{
						toTarget = Projectile.velocity;
						toTarget.Normalize();
						toTarget *= topSpeed * 0.6f;
						Projectile.velocity = (Projectile.velocity * (velocityLerpDivisor - 1f) + toTarget) / velocityLerpDivisor;
					}
				}
				else
				{
					Projectile.velocity *= 0.8f;
				}
				if (isSubYoyoProj && !isBeyondRange && (double)Projectile.velocity.Length() < (double)topSpeed * 0.6)
				{
					Projectile.velocity.Normalize();
					Projectile.velocity *= topSpeed * 0.6f;
				}
			}
		}
		else
		{
			velocityLerpDivisor *= 0.8f;
			topSpeed *= 1.5f;
			Projectile.tileCollide = false;
			Vector2 toOwner = player.Center - Projectile.Center;
			float distanceToOwner = toOwner.Length();
			if (distanceToOwner < topSpeed + 10f || distanceToOwner == 0f || distanceToOwner > 2000f)
			{
				Projectile.Kill();
			}
			else
			{
				toOwner.Normalize();
				toOwner *= topSpeed;
				Projectile.velocity = (Projectile.velocity * (velocityLerpDivisor - 1f) + toOwner) / velocityLerpDivisor;
			}
		}
		Projectile.rotation += RotationalSpeed;
	}

	public override bool PreDraw(ref Color lightColor)
	{
		DrawYoyo_String();
		return true;
	}

	/// <summary>
	/// playerHeldPos, keep default to the holding position of player's hand, or set to a world position manually.
	/// </summary>
	/// <param name="playerHeldPos"></param>
	public virtual void DrawYoyo_String(Vector2 playerHeldPos = default)
	{
		Player player = Main.player[Projectile.owner];
		Vector2 mountedCenter = player.MountedCenter;
		Vector2 stringUnitPos = mountedCenter;
		stringUnitPos.Y += player.gfxOffY;
		if (playerHeldPos != default)
		{
			stringUnitPos = playerHeldPos;
		}
		Vector2 unitToYoyoCenter = Projectile.Center - stringUnitPos;
		if (!Projectile.counterweight)
		{
			int stringDirection = -1;
			if (Projectile.position.X + Projectile.width / 2 < player.position.X + player.width / 2)
			{
				stringDirection = 1;
			}
			stringDirection *= -1;
			Vector2 rotVec = unitToYoyoCenter * stringDirection;
			player.itemRotation = rotVec.ToRotationSafe();
		}

		float unitLength = 12f;
		float finalLength;
		float finalRot;
		YoyoStringPos.Clear();
		GenerateYoyo_String(stringUnitPos, YoyoStringPos, out finalLength, out finalRot);
		DrawYoyo_String_Pieces(player, unitLength, finalLength, finalRot);
	}

	public virtual void DrawYoyo_String_Pieces(Player player, float unitLength, float finalLength, float finalRot)
	{
		Texture2D tex = TextureAssets.FishingLine.Value;
		int stringColor = player.stringColor;
		for (int i = 0; i < YoyoStringPos.Count; i++)
		{
			float rotation = finalRot;
			if (i < YoyoStringPos.Count - 1)
			{
				rotation = (YoyoStringPos[i + 1] - YoyoStringPos[i]).ToRotation() - MathHelper.PiOver2;
			}
			else
			{
				unitLength = finalLength;
			}
			Vector2 drawPos = YoyoStringPos[i] + tex.Size() * 0.5f - new Vector2(6, 0);
			Color color = ModifyYoyoStringColor_VanillaRender(stringColor, drawPos, i, YoyoStringPos.Count);
			drawPos -= Main.screenPosition;
			Main.spriteBatch.Draw(tex, drawPos, new Rectangle(0, 0, tex.Width, (int)unitLength), color, rotation, new Vector2(tex.Width * 0.5f, 0f), 1f, SpriteEffects.None, 0f);
			DrawYoyo_String_Attachments(drawPos, rotation, color, unitLength, i, YoyoStringPos.Count);
		}
	}

	public virtual void DrawYoyo_String_Attachments(Vector2 drawPos, float rotation, Color color, float length, float index, float stringUnitCount)
	{
	}

	/// <summary>
	/// Generate a yoyo-string that consist of a <see cref="List{Vector2}"/>.
	/// </summary>
	/// <param name="stringUnitPos"></param>
	/// <param name="yoyo_string_pos"></param>
	/// <param name="finalLength"></param>
	/// <param name="finalRot"></param>
	public virtual void GenerateYoyo_String(Vector2 stringUnitPos, List<Vector2> yoyo_string_pos, out float finalLength, out float finalRot)
	{
		finalLength = 12f;
		finalRot = 0f;
		Vector2 unitToYoyoCenter = Projectile.Center - stringUnitPos;
		bool checkSelf = true;
		if (unitToYoyoCenter == Vector2.zeroVector)
		{
			checkSelf = false;
		}
		else
		{
			float stringLength = unitToYoyoCenter.Length();
			stringLength = 12f / stringLength;
			unitToYoyoCenter *= stringLength;
			stringUnitPos -= unitToYoyoCenter * 0.1f;
			unitToYoyoCenter = Projectile.position + Projectile.Size * 0.5f - stringUnitPos;
		}
		while (checkSelf)
		{
			float restStringLength = unitToYoyoCenter.Length();
			float restStringLength_Copy = restStringLength;
			if (float.IsNaN(restStringLength) && float.IsNaN(restStringLength_Copy))
			{
				checkSelf = false;
			}
			else
			{
				if (restStringLength < 20f)
				{
					finalLength = restStringLength - 8f;
					checkSelf = false;
				}
				restStringLength = 12f / restStringLength;
				unitToYoyoCenter *= restStringLength;
				stringUnitPos += unitToYoyoCenter;
				unitToYoyoCenter = Projectile.position + Projectile.Size * new Vector2(0.5f, 0.1f) - stringUnitPos;
				if (restStringLength_Copy > 12f)
				{
					float speedChangeRate = 0.3f;
					float manhattanSpeed = Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y);
					if (manhattanSpeed > 16f)
					{
						manhattanSpeed = 16f;
					}
					manhattanSpeed = 1f - manhattanSpeed / 16f;
					speedChangeRate *= manhattanSpeed;
					manhattanSpeed = restStringLength_Copy / 80f;
					if (manhattanSpeed > 1f)
					{
						manhattanSpeed = 1f;
					}
					speedChangeRate *= manhattanSpeed;
					if (speedChangeRate < 0f)
					{
						speedChangeRate = 0f;
					}
					speedChangeRate *= manhattanSpeed;
					speedChangeRate *= 0.5f;
					if (unitToYoyoCenter.Y > 0f)
					{
						unitToYoyoCenter.Y *= 1f + speedChangeRate;
						unitToYoyoCenter.X *= 1f - speedChangeRate;
					}
					else
					{
						manhattanSpeed = Math.Abs(Projectile.velocity.X) / 3f;
						if (manhattanSpeed > 1f)
						{
							manhattanSpeed = 1f;
						}
						manhattanSpeed -= 0.5f;
						speedChangeRate *= manhattanSpeed;
						if (speedChangeRate > 0f)
						{
							speedChangeRate *= 2f;
						}
						unitToYoyoCenter.Y *= 1f + speedChangeRate;
						unitToYoyoCenter.X *= 1f - speedChangeRate;
					}
				}
				yoyo_string_pos.Add(stringUnitPos);
				finalRot = unitToYoyoCenter.ToRotation() - MathHelper.PiOver2;
			}
		}
	}

	public virtual Color ModifyYoyoStringColor_VanillaRender(int playerStringColor, Vector2 worldPos, float index, float stringCount)
	{
		Color color = WorldGen.paintColor(playerStringColor);
		if (color.R < 75)
		{
			color.R = 75;
		}
		if (color.G < 75)
		{
			color.G = 75;
		}
		if (color.B < 75)
		{
			color.B = 75;
		}
		if (playerStringColor == 13)
		{
			color = new Color(20, 20, 20);
		}
		else if (playerStringColor == 14 || playerStringColor == 0)
		{
			color = new Color(200, 200, 200);
		}
		else if (playerStringColor == 28)
		{
			color = new Color(163, 116, 91);
		}
		else if (playerStringColor == 27)
		{
			color = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);
		}
		color.A = (byte)(color.A * 0.4f);
		color = Lighting.GetColor(worldPos.ToTileCoordinates(), color);
		color *= 0.5f;
		return ModifyYoyoStringColor_PostVanillaRender(color, worldPos, index, stringCount);
	}

	public virtual Color ModifyYoyoStringColor_PostVanillaRender(Color vanillaColor, Vector2 worldPos, float index, float stringCount)
	{
		return vanillaColor;
	}
}

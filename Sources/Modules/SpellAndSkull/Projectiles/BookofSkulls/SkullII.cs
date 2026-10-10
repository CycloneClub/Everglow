using Everglow.Commons.MEAC;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;
using Everglow.SpellAndSkull.GlobalItems;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.SpellAndSkull.Projectiles.BookofSkulls;

public class SkullII : ModProjectile, IWarpProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 26;
		Projectile.height = 26;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.ignoreWater = false;
		Projectile.tileCollide = true;
		Projectile.extraUpdates = 3;
		Projectile.timeLeft = 6000;
		Projectile.alpha = 0;
		Projectile.penetrate = 5;
		Projectile.scale = 1f;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 30;
		Projectile.DamageType = DamageClass.Magic;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 120;
	}

	internal int Aimnpc = -1;

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.position.Y -= 15f;
	}

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		if (Projectile.penetrate != 1 && Projectile.friendly)
		{
			Lighting.AddLight((int)(Projectile.Center.X / 16), (int)(Projectile.Center.Y / 16), 0.64f * Projectile.scale, 0.36f * Projectile.scale, 0.24f * Projectile.scale);

			Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X);

			if (Aimnpc != -1)
			{
				NPC npc = Main.npc[Aimnpc];
				if (npc.active)
				{
					Vector2 v0 = npc.Center - Projectile.Center;
					Projectile.velocity += Vector2.Normalize(v0) * 0.025f;
				}
			}

			if (Main.rand.NextBool(2))
			{
				var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Projectile.velocity.X * 2, Projectile.velocity.Y * 2, 100, default, Main.rand.NextFloat(0.85f, 1.45f) * Projectile.scale);
				d.noGravity = true;
				float MinDis = 250;
				foreach (var target in Main.npc)
				{
					if (target.active && Main.rand.NextBool(2))
					{
						if (!target.dontTakeDamage && !target.friendly && target.CanBeChasedBy())
						{
							Vector2 ToTarget = target.Center - Projectile.Center;
							float dis = ToTarget.Length();
							if (dis < MinDis)
							{
								MinDis = dis;
								Aimnpc = target.whoAmI;
							}
						}
					}
				}
			}
		}

		if (Projectile.penetrate != 1 && Projectile.friendly)
		{
			if (player.HeldItem.type == ItemID.BookofSkulls)
			{
				Projectile.velocity = Vector2.Normalize(Projectile.velocity) * player.HeldItem.shootSpeed / Projectile.extraUpdates;
			}
			else
			{
				Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 3.5f / 3f;
			}
		}

		if (Projectile.penetrate <= 0)
		{
			Projectile.Kill();
		}

		if (Projectile.penetrate == 1 && Projectile.timeLeft > 120)
		{
			Projectile.timeLeft = 120;
			Projectile.velocity *= 0.01f;
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D Light = ModAsset.SkullII.Value;

		var c0 = new Color(1f, 0.4f, 0f, 0);
		var c1 = new Color(1f, 1f, 1f, 0.7f);

		float width = 16;

		int TrueL = 0;
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			TrueL++;
		}

		DrawFlameTrail(TrueL, width, true, Color.White * 0.5f);

		var Frame = new Rectangle(0, (int)(Main.timeForVisualEffects / 10f % 3) * 30, 26, 30);

		DrawFlameTrail(TrueL, width, false, c0);

		if (Projectile.penetrate != 1 && Projectile.friendly)
		{
			if (Projectile.velocity.X < 0)
			{
				Main.spriteBatch.Draw(Light, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + Projectile.velocity, Frame, c1, Projectile.rotation, Frame.Size() / 2f, Projectile.scale, SpriteEffects.FlipVertically, 0);
			}
			else
			{
				Main.spriteBatch.Draw(Light, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + Projectile.velocity, Frame, c1, Projectile.rotation, Frame.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
			}
		}
		return false;
	}

	private void DrawFlameTrail(int TrueL, float width, bool Shade = false, Color c0 = default(Color), float Mulfactor = 1.6f)
	{
		var bars = new List<Vertex2D>();
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			float mulColor = 1f;
			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));
			if (i == 1)
			{
				mulColor = 0f;
			}

			if (i >= 2)
			{
				var normalDirII = Projectile.oldPos[i - 2] - Projectile.oldPos[i - 1];
				normalDirII = Vector2.Normalize(new Vector2(-normalDirII.Y, normalDirII.X));
				if (Vector2.Dot(normalDirII, normalDir) <= 0.965f)
				{
					mulColor = 0f;
				}
			}
			if (i < Projectile.oldPos.Length - 1)
			{
				var normalDirII = Projectile.oldPos[i] - Projectile.oldPos[i + 1];
				normalDirII = Vector2.Normalize(new Vector2(-normalDirII.Y, normalDirII.X));
				if (Vector2.Dot(normalDirII, normalDir) <= 0.965f)
				{
					mulColor = 0f;
				}
			}
			var factor = i / (float)TrueL;
			float x0 = factor * Mulfactor - (float)(Main.timeForVisualEffects / 15d) + 100000;
			x0 %= 1f;
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width * (1 - factor) + new Vector2(13f) - Main.screenPosition, c0 * mulColor, new Vector3(x0, 1, 0)));
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width * (1 - factor) + new Vector2(13f) - Main.screenPosition, c0 * mulColor, new Vector3(x0, 0, 0)));
			var factorII = factor;
			factor = (i + 1) / (float)TrueL;
			var x1 = factor * Mulfactor - (float)(Main.timeForVisualEffects / 15d) + 100000;
			x1 %= 1f;
			if (x0 > x1)
			{
				float DeltaValue = 1 - x0;
				var factorIII = factorII * x0 + factor * DeltaValue;
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width * (1 - factorIII) + new Vector2(13f) - Main.screenPosition, c0 * mulColor, new Vector3(1, 1, 0)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width * (1 - factorIII) + new Vector2(13f) - Main.screenPosition, c0 * mulColor, new Vector3(1, 0, 0)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width * (1 - factorIII) + new Vector2(13f) - Main.screenPosition, c0 * mulColor, new Vector3(0, 1, 0)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width * (1 - factorIII) + new Vector2(13f) - Main.screenPosition, c0 * mulColor, new Vector3(0, 0, 0)));
			}
		}
		Texture2D t = Commons.ModAsset.Trail_6.Value;
		if (Shade)
		{
			t = ModAsset.Darkline.Value;
		}

		Main.graphics.GraphicsDevice.Textures[0] = t;

		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
		float width = 16;

		int TrueL = 0;
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			TrueL++;
		}
		var bars = new List<Vertex2D>();
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			float mulColor = 1f;
			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));
			if (i == 1)
			{
				mulColor = 0f;
			}

			if (i >= 2)
			{
				var normalDirII = Projectile.oldPos[i - 2] - Projectile.oldPos[i - 1];
				normalDirII = Vector2.Normalize(new Vector2(-normalDirII.Y, normalDirII.X));
				if (Vector2.Dot(normalDirII, normalDir) <= 0.965f)
				{
					mulColor = 0f;
				}
			}
			if (i < Projectile.oldPos.Length - 1)
			{
				var normalDirII = Projectile.oldPos[i] - Projectile.oldPos[i + 1];
				normalDirII = Vector2.Normalize(new Vector2(-normalDirII.Y, normalDirII.X));
				if (Vector2.Dot(normalDirII, normalDir) <= 0.965f)
				{
					mulColor = 0f;
				}
			}

			float k0 = (float)Math.Atan2(normalDir.Y, normalDir.X);
			k0 += 3.14f + 1.57f;
			if (k0 > 6.28f)
			{
				k0 -= 6.28f;
			}

			var c0 = new Color(k0, 0.04f * mulColor, 0, 0);

			var factor = i / (float)TrueL;
			float x0 = factor * 1.3f - (float)(Main.timeForVisualEffects / 15d) + 100000;
			x0 %= 1f;
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width * (1 - factor) + new Vector2(13f) - Main.screenPosition, c0, new Vector3(x0, 1, 0)));
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width * (1 - factor) + new Vector2(13f) - Main.screenPosition, c0, new Vector3(x0, 0, 0)));
			var factorII = factor;
			factor = (i + 1) / (float)TrueL;
			var x1 = factor * 1.3f - (float)(Main.timeForVisualEffects / 15d) + 100000;
			x1 %= 1f;
			if (x0 > x1)
			{
				float DeltaValue = 1 - x0;
				var factorIII = factorII * x0 + factor * DeltaValue;
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width * (1 - factorIII) + new Vector2(13f) - Main.screenPosition, c0, new Vector3(1, 1, 0)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width * (1 - factorIII) + new Vector2(13f) - Main.screenPosition, c0, new Vector3(1, 0, 0)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width * (1 - factorIII) + new Vector2(13f) - Main.screenPosition, c0, new Vector3(0, 1, 0)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width * (1 - factorIII) + new Vector2(13f) - Main.screenPosition, c0, new Vector3(0, 0, 0)));
			}
		}
		Texture2D t = ModAsset.FogTraceLight.Value;
		if (bars.Count > 3)
		{
			spriteBatch.Draw(t, bars, PrimitiveType.TriangleStrip);
		}
	}

	public override void OnKill(int timeLeft)
	{
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		SoundEngine.PlaySound(SoundID.NPCHit2, Projectile.Center);
		int type = ModContent.ProjectileType<BoneSpike>();
		Player player = Main.player[Projectile.owner];

		if (player.ownedProjectileCounts[type] < 12)
		{
			for (int x = 0; x < 3; x++)
			{
				player.GetModPlayer<MagicBookPlayer>().WaterBoltHasHit = 0;
				var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), player.Center + new Vector2(0, -30 * player.gravDir), new Vector2(0, 18 * player.gravDir), type, player.HeldItem.damage / 2, player.HeldItem.knockBack, Projectile.owner, Main.rand.NextFloat(-1.5f, 7f), Main.rand.NextFloat(0.65f, 0.95f));
				p.CritChance = player.GetWeaponCrit(player.HeldItem);
			}
		}
		if (Projectile.penetrate == 1)
		{
			GenerateDust(60);
			Projectile.friendly = false;
			Projectile.velocity *= 0.001f;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 120;
		}
	}

	private void GenerateDust(int Times)
	{
		for (int d = 0; d < Times; d++)
		{
			Vector2 BasePos = Projectile.Center - new Vector2(4) - Projectile.velocity;
			var d0 = Dust.NewDustDirect(BasePos, 0, 0, DustID.Bone, 0, 0, 0, default, Main.rand.NextFloat(0.6f, 1.2f));
			d0.velocity = new Vector2(0, Main.rand.NextFloat(0.6f, 7.5f)).RotatedByRandom(6.283);
			d0.noGravity = true;
		}
		for (int d = 0; d < Times; d++)
		{
			Vector2 BasePos = Projectile.Center - new Vector2(4) - Projectile.velocity;
			var d0 = Dust.NewDustDirect(BasePos, 0, 0, DustID.Torch, 0, 0, 0, default, Main.rand.NextFloat(0.8f, 1.9f));
			d0.velocity = new Vector2(0, Main.rand.NextFloat(0.6f, 2.5f)).RotatedByRandom(6.283);
			d0.noGravity = true;
		}
	}

	public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
	{
		SoundEngine.PlaySound(SoundID.NPCHit2, Projectile.Center);
		int type = ModContent.ProjectileType<BoneSpike>();
		Player player = Main.player[Projectile.owner];

		if (player.ownedProjectileCounts[type] < 12)
		{
			for (int x = 0; x < 3; x++)
			{
				player.GetModPlayer<MagicBookPlayer>().WaterBoltHasHit = 0;
				var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), player.Center + new Vector2(0, -30 * player.gravDir), new Vector2(0, 18 * player.gravDir), type, player.HeldItem.damage / 2, player.HeldItem.knockBack, Projectile.owner, Main.rand.NextFloat(-1.5f, 7f), Main.rand.NextFloat(0.65f, 0.95f));
				p.CritChance = player.GetWeaponCrit(player.HeldItem);
			}
		}
		if (Projectile.penetrate == 1)
		{
			GenerateDust(60);
			Projectile.friendly = false;
			Projectile.velocity *= 0.001f;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 120;
		}
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		SoundEngine.PlaySound(SoundID.NPCHit2, Projectile.Center);
		if (Projectile.velocity.X != oldVelocity.X)
		{
			Projectile.velocity.X = -oldVelocity.X;
		}

		if (Projectile.velocity.Y != oldVelocity.Y)
		{
			Projectile.velocity.Y = -oldVelocity.Y;
		}

		Projectile.penetrate = 1;
		GenerateDust(60);
		Projectile.friendly = false;
		Projectile.velocity *= 0.001f;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 120;

		return false;
	}
}

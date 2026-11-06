namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.Clubs;

public class SpikeClub : ClubProj
{
	private List<Vector2> moonBladeI = [];
	private List<Vector2> moonBladeII = [];
	private List<Vector2> moonBladeIII = [];
	private List<Vector2> moonBladeIV = [];

	private int timeI = 60;
	private int timeII = 60;
	private int timeIII = 60;
	private int timeIV = 60;

	public override void SetCustomDefaults()
	{
		EnableReflection = true;
		HitLength = 32f;
		ReflectionStrength = 6f;
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		modifiers.Knockback *= 0.4f;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		Player player = Main.player[Projectile.owner];
		Vector2 v = new Vector2(0, 6).RotatedByRandom(Math.PI * 2) * 5f;
		Projectile.NewProjectile(Projectile.GetSource_OnHit(target), target.Center - v * 3, v, ModContent.ProjectileType<SpikeClubSlash>(), Projectile.damage / 2, 0, player.whoAmI, Main.rand.NextFloat(-0.05f, 0.05f));
		target.AddBuff(BuffID.Bleeding, 600);
	}

	private void DrawMoon(List<Vector2> listVec, float timeLeft)
	{
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		List<Vector2> SmoothTrailX = GraphicsUtils.CatmullRom(listVec.ToList());
		var SmoothTrail = new List<Vector2>();
		for (int x = 0; x < SmoothTrailX.Count - 1; x++)
		{
			SmoothTrail.Add(SmoothTrailX[x]);
		}
		if (listVec.Count != 0)
		{
			SmoothTrail.Add(listVec.ToArray()[listVec.Count - 1]);
		}

		int length = SmoothTrail.Count;
		if (length <= 3)
		{
			return;
		}

		var bars = new List<Vertex2D>();
		Color light = Lighting.GetColor((int)(Projectile.Center.X / 16f), (int)(Projectile.Center.Y / 16f));
		light *= 2;
		light.A /= 4;
		for (int i = 0; i < length; i++)
		{
			float factor = i / (length - 1f);
			float delta = (1 - timeLeft / 60f * 0.45f) * Omega / MaxOmega + 1 - Omega / MaxOmega;
			float maxValue = 20f;
			if (length < maxValue)
			{
				delta = delta * length / maxValue + (maxValue - length) / maxValue;
			}

			bars.Add(new Vertex2D(Projectile.Center + SmoothTrail[i] * delta * Projectile.scale - Main.screenPosition, light, new Vector3(factor, 1, 0f)));
			bars.Add(new Vertex2D(Projectile.Center + SmoothTrail[i] * Projectile.scale - Main.screenPosition, light, new Vector3(factor, 0, 0f)));
		}
		Main.graphics.GraphicsDevice.Textures[0] = ModAsset.BloomLight.Value;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
	}

	private void UpdateMoon(ref List<Vector2> listVec)
	{
		if (listVec.Count > 0)
		{
			float value = 1;
			if (listVec.Count > 6)
			{
				value = 6 / (float)listVec.Count;
			}

			listVec.Add(listVec[listVec.Count - 1].RotatedBy(Omega * value));
		}
	}

	private void ActivateMoon(ref List<Vector2> listVec)
	{
		if (listVec.Count > 0)
		{
			return;
		}

		listVec = new List<Vector2>();
		if (Main.rand.NextBool(2))
		{
			listVec.Add(TrailVecs.ToList()[1] * Main.rand.NextFloat(0.95f, Math.Min(1.75f, 1 + Omega * 1.5f)));
		}
		else
		{
			listVec.Add(TrailVecs.ToList()[1] * -Main.rand.NextFloat(0.95f, Math.Min(1.75f, 1 + Omega * 1.5f)));
		}
	}

	private void DeactivateMoon(ref List<Vector2> listVec)
	{
		listVec.Clear();
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		float point = 0;
		Vector2 HitRange = new Vector2(HitLength, HitLength * Projectile.spriteDirection).RotatedBy(Projectile.rotation) * Projectile.scale;
		if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - HitRange / 32f * 45f, Projectile.Center + HitRange / 32f * 45f, 2 * HitLength / 32f * Omega / 0.3f, ref point))
		{
			return true;
		}

		return false;
	}

	public override void PostPreDraw()
	{
		var bars = CreateTrailVertices(0.7f, 0.6f, useSpecialAplha: true);
		if (bars == null)
		{
			return;
		}

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, TrailBlendState(), SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone);

		Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>(TrailShapeTex(), ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		if (!Main.gamePaused)
		{
			if (Main.rand.NextBool(10) && Omega > 0.1f)
			{
				ActivateMoon(ref moonBladeI);
			}

			if (moonBladeI.Count > 20 && timeI == 60)
			{
				if (Main.rand.Next(moonBladeI.Count, 50) > 44)
				{
					timeI--;
				}
			}
			if (timeI < 60)
			{
				timeI--;
				if (timeI <= 0)
				{
					timeI = 60;
					DeactivateMoon(ref moonBladeI);
				}
			}

			if (Main.rand.NextBool(10) && Omega > 0.1f)
			{
				ActivateMoon(ref moonBladeII);
			}

			if (moonBladeII.Count > 20 && timeII == 60)
			{
				if (Main.rand.Next(moonBladeII.Count, 50) > 44)
				{
					timeII--;
				}
			}
			if (timeII < 60)
			{
				timeII--;
				if (timeII <= 0)
				{
					timeII = 60;
					DeactivateMoon(ref moonBladeII);
				}
			}

			if (Main.rand.NextBool(10) && Omega > 0.1f)
			{
				ActivateMoon(ref moonBladeIII);
			}

			if (moonBladeIII.Count > 20 && timeIII == 60)
			{
				if (Main.rand.Next(moonBladeIII.Count, 50) > 44)
				{
					timeIII--;
				}
			}
			if (timeIII < 60)
			{
				timeIII--;
				if (timeIII <= 0)
				{
					timeIII = 60;
					DeactivateMoon(ref moonBladeIII);
				}
			}
			if (Main.rand.NextBool(10) && Omega > 0.1f)
			{
				ActivateMoon(ref moonBladeIV);
			}

			if (moonBladeIV.Count > 20 && timeIV == 60)
			{
				if (Main.rand.Next(moonBladeIV.Count, 50) > 44)
				{
					timeIV--;
				}
			}
			if (timeIV < 60)
			{
				timeIV--;
				if (timeIV <= 0)
				{
					timeIV = 60;
					DeactivateMoon(ref moonBladeIV);
				}
			}

			UpdateMoon(ref moonBladeI);
			UpdateMoon(ref moonBladeIV);
			UpdateMoon(ref moonBladeIII);
			UpdateMoon(ref moonBladeIV);
		}

		DrawMoon(moonBladeI, timeI);
		DrawMoon(moonBladeII, timeII);
		DrawMoon(moonBladeIII, timeIII);
		DrawMoon(moonBladeIV, timeIV);
	}
}

namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.Clubs;

public class CrystalClub : ClubProj
{
	private int flyClubCooling = 0;

	public override void SetCustomDefaults()
	{
		EnableReflection = true;
		Beta = 0.005f;
		MaxOmega = 0.45f;
		WarpValue = 0.03f;
		ReflectionStrength = 1.2f;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		int type = 0;
		switch (Main.rand.Next(3))
		{
			case 0:
				type = DustID.BlueCrystalShard;
				break;
			case 1:
				type = DustID.PinkCrystalShard;
				break;
			case 2:
				type = DustID.PurpleCrystalShard;
				break;
		}
		for (float d = 0.1f; d < Omega; d += 0.04f)
		{
			var D = Dust.NewDustDirect(target.Center - new Vector2(4)/*Dust的Size=8x8*/, 0, 0, type, 0, 0, 150, default, Main.rand.NextFloat(0.4f, 1.1f));
			D.noGravity = true;
			D.velocity = new Vector2(0, Main.rand.NextFloat(Omega * 25f)).RotatedByRandom(6.283);
		}
	}

	public override void AI()
	{
		base.AI();
		if (Omega > 0.1f)
		{
			for (float d = 0.1f; d < Omega; d += 0.1f)
			{
			}
			if (flyClubCooling > 0)
			{
				flyClubCooling--;
			}

			if (flyClubCooling <= 0 && Omega > 0.2f)
			{
				flyClubCooling = 36;
				Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<CrystalClub_Fly>(), (int)(Projectile.damage * 0.3f), Projectile.knockBack * 0.4f, Projectile.owner);
			}
			GenerateDust();
		}
	}

	private void GenerateDust()
	{
		var v0 = new Vector2(1, 1);
		v0 *= Main.rand.NextFloat(Main.rand.NextFloat(1, HitLength), HitLength);
		v0.X *= Projectile.spriteDirection;
		if (Main.rand.NextBool(2))
		{
			v0 *= -1;
		}

		v0 = v0.RotatedBy(Projectile.rotation);
		float Speed = Math.Min(Omega * 0.5f, 0.181f);
		int type = 0;
		switch (Main.rand.Next(3))
		{
			case 0:
				type = DustID.BlueCrystalShard;
				break;
			case 1:
				type = DustID.PinkCrystalShard;
				break;
			case 2:
				type = DustID.PurpleCrystalShard;
				break;
		}
		for (float d = 0.1f; d < Omega; d += 0.04f)
		{
			var D = Dust.NewDustDirect(Projectile.Center + v0 - new Vector2(4)/*Dust的Size=8x8*/, 0, 0, type, -v0.Y * Speed, v0.X * Speed, 150, default, Main.rand.NextFloat(0.4f, 0.7f));
			D.noGravity = true;
			D.velocity = new Vector2(-v0.Y * Speed, v0.X * Speed);
		}
	}

	public override void PostPreDraw()
	{
		var bars = CreateTrailVertices(useSpecialAplha: true);
		if (bars == null)
		{
			return;
		}

		var lightColor = Lighting.GetColor((int)(Projectile.Center.X / 16), (int)(Projectile.Center.Y / 16)).ToVector4();
		lightColor.W = 0.7f * Omega;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;

		var sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, TrailBlendState(), SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone);

		Effect MeleeTrail = ModAsset.CrystalClubTrail.Value;
		MeleeTrail.Parameters["uTransform"].SetValue(model * projection);
		MeleeTrail.Parameters["tex1"].SetValue(ModAsset.CrystalClub_Fly.Value);
		MeleeTrail.Parameters["Light"].SetValue(lightColor);
		MeleeTrail.CurrentTechnique.Passes["TrailByOrigTex"].Apply();

		Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>(TrailShapeTex(), ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
	}
}

using Everglow.Commons.DataStructures;
using Everglow.Commons.Graphics;
using Everglow.Commons.Templates.Weapons;
using Everglow.Myth.LanternMoon.Items.Weapons;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Myth.LanternMoon.Projectiles.Weapons;

public class KeroseneLanternFlameThrower_Hold : HandholdProjectile, IWarpProjectile_warpStyle2
{
	public int Timer = 0;

	public struct SubProj
	{
		public Vector2 Pos;
		public Vector2 Vel;
		public bool Active;
	}

	public List<SubProj> ShootProjPos = [];

	public override void SetDef()
	{
		Projectile.width = 64;
		Projectile.height = 64;
		Projectile.friendly = false;
		TextureRotation = 0;
		MaxRotationSpeed = 0.05f;
		DepartLength = 20;
		DrawOffset = new Vector2(-6, -8);
	}

	public override void OnSpawn(IEntitySource source)
	{
		SoundEngine.PlaySound(new SoundStyle(ModAsset.Flamethrower_begin_Mod), Projectile.Center);
	}

	public override void AI()
	{
		Timer++;
		float shootSpeed = GetShootSpeed();
		Vector2 vel = (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * shootSpeed;
		Vector2 pos = Projectile.Center + vel.NormalizeSafe() * (32 - shootSpeed) + DrawOffset;
		var sub = new SubProj
		{
			Pos = pos,
			Vel = vel,
			Active = true,
		};
		ShootProjPos.Add(sub);
		if (ShootProjPos.Count > 20)
		{
			ShootProjPos.RemoveAt(0);
		}
		for (int k = 0; k < ShootProjPos.Count; k++)
		{
			var proj = ShootProjPos[k];
			for (int i = 0; i < 2; i++)
			{
				proj.Pos += proj.Vel * 0.5f;
				if (proj.Active && (Collision.SolidCollision(proj.Pos - new Vector2(10), 20, 20) || TileUtils.SafeGetTile(proj.Pos.ToTileCoordinates()).LiquidAmount > 0))
				{
					proj.Active = false;
				}
			}
			ShootProjPos[k] = proj;
		}
		base.AI();
	}

	public override void HeldProjectileAI()
	{
		Player player = Main.player[Projectile.owner];
		if (player.wet)
		{
			Projectile.Kill();
		}

		player.heldProj = Projectile.whoAmI;
		ArmRootPos = player.MountedCenter + new Vector2(-4 * player.direction, -2);
		player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (Projectile.rotation - MathF.PI * 0.25f) * player.gravDir - MathF.PI * 0.5f);
		int dir = 1;
		if (Main.MouseWorld.X < player.MountedCenter.X)
		{
			dir = -1;
		}
		Vector2 mouseToPlayer = Main.MouseWorld - ArmRootPos;
		mouseToPlayer = mouseToPlayer.NormalizeSafe();
		float powerRate = 1;
		if (player.HeldItem is not null && player.HeldItem.type == ModContent.ItemType<KeroseneLanternFlameThrower>())
		{
			KeroseneLanternFlameThrower kLFT = player.HeldItem.ModItem as KeroseneLanternFlameThrower;
			powerRate = (kLFT.PowerRate - 0.5f) * 2f;
		}
		if (player.controlUseItem)
		{
			if (Timer % (int)(45 * 2.5f / (powerRate + 2)) == (int)(15 * 2.5f / (powerRate + 2)))
			{
				SoundEngine.PlaySound(new SoundStyle(ModAsset.Flamethrower_cycle_Mod).WithPitchOffset(powerRate), Projectile.Center);
			}
			float addRot = -MathF.Asin(Vector3.Cross(new Vector3(mouseToPlayer, 0), new Vector3(new Vector2(0, 1).RotatedBy(Projectile.rotation - Math.PI * 0.75), 0)).Z);
			addRot *= LerpFactorOfRotation;
			if (Math.Abs(addRot) > MaxRotationSpeed)
			{
				addRot *= MaxRotationSpeed / Math.Abs(addRot);
			}
			Projectile.rotation += addRot;
			Projectile.Center = ArmRootPos + new Vector2(0, 1).RotatedBy(Projectile.rotation - Math.PI * 0.75) * DepartLength;
			Projectile.timeLeft = player.itemTimeMax;
			float shootSpeed = GetShootSpeed();
			if (Math.Abs(addRot) > 0.02f
				|| Timer % 2 == 1)
			{
				Vector2 vel = (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * shootSpeed;
				Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + vel.NormalizeSafe() * 60 + DrawOffset, vel, ModContent.ProjectileType<KeroseneLanternFlameThrower_Shoot>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
			}
		}
		else
		{
			SoundEngine.StopTrackedSounds();
			SoundEngine.PlaySound(new SoundStyle(ModAsset.Flamethrower_end_Mod).WithPitchOffset(Math.Clamp(powerRate, -1, 0)), Projectile.Center);
		}
		player.direction = dir;
	}

	public float GetShootSpeed()
	{
		Player player = Main.player[Projectile.owner];
		if (player.HeldItem is not null
			&& player.HeldItem.type == ModContent.ItemType<KeroseneLanternFlameThrower>()
			&& player.HeldItem.ModItem is KeroseneLanternFlameThrower kLFT)
		{
			return (kLFT.PowerRate + 0.2f) * 36f * player.HeldItem.shootSpeed;
		}
		return 12f;
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		Player previousDrawPlayer = DrawPlayer;
		DrawPlayer = player;
		try
		{
			DrawBaseTexture(lightColor);

			return false;
		}
		finally
		{
			DrawPlayer = previousDrawPlayer;
		}
	}

	public override void DrawBaseTexture(Color lightColor)
	{
		Player player = (DrawPlayer ?? Main.player[Projectile.owner]);
		var texMain = ModAsset.KeroseneLanternFlameThrower_Hold.Value;
		SpriteEffects se = SpriteEffects.None;
		if (player.direction == -1)
		{
			se = SpriteEffects.FlipVertically;
		}

		float rot = Projectile.rotation - (float)(Math.PI * 0.25) + TextureRotation * player.direction;
		Vector2 drawCenter = Projectile.Center - Main.screenPosition + DrawOffset;

		var bars0 = new List<Vertex2D>();
		var bars1 = new List<Vertex2D>();
		var bars2 = new List<Vertex2D>();
		CreateTrailVertex(bars0, bars1, bars2, 1, -Main.screenPosition);
		if (bars0.Count >= 2 && bars1.Count >= 2 && bars2.Count >= 3)
		{
			SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_12.Value;
			Effect effect = Commons.ModAsset.Trailing.Value;
			var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
			var model = Matrix.CreateTranslation(new Vector3(0)) * Main.GameViewMatrix.TransformationMatrix;
			effect.Parameters["uTransform"].SetValue(model * projection);
			effect.CurrentTechnique.Passes[0].Apply();
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars0.ToArray(), 0, bars0.Count - 2);
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars1.ToArray(), 0, bars1.Count - 2);
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars2.ToArray(), 0, bars2.Count - 2);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(sBS);
		}

		Main.spriteBatch.Draw(texMain, drawCenter, null, lightColor, rot, texMain.Size() * 0.5f, 1f, se, 0);
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
		if (ShootProjPos.Count <= 0)
		{
			return;
		}
		Texture2D warpTex = Commons.ModAsset.Trail_21.Value;
		var bars0 = new List<Vertex2D>();
		var bars1 = new List<Vertex2D>();
		var bars2 = new List<Vertex2D>();
		CreateTrailVertex(bars0, bars1, bars2, 2, -Main.screenPosition);
		if (bars0.Count >= 2 && bars1.Count >= 2 && bars2.Count >= 3)
		{
			spriteBatch.Draw(warpTex, bars0, PrimitiveType.TriangleStrip);
			spriteBatch.Draw(warpTex, bars1, PrimitiveType.TriangleStrip);
			spriteBatch.Draw(warpTex, bars2, PrimitiveType.TriangleStrip);
		}
	}

	public void CreateTrailVertex(List<Vertex2D> bars0, List<Vertex2D> bars1, List<Vertex2D> bars2, int style, Vector2 offset = default)
	{
		// If there is no any element here, return.
		if (ShootProjPos.Count <= 0)
		{
			return;
		}

		float trailLength = 20;

		// If dark, the Color.White will be proper.
		float trailWidth = 158;
		if (style == 1)
		{
			trailWidth = 120;
		}
		if (GetShootSpeed() < 15f)
		{
			trailWidth *= GetShootSpeed() / 15f;
		}
		for (int i = 0; i < ShootProjPos.Count; ++i)
		{
			// factor, among 0 to 1, usually for deciding the trail's texture.coord.X.
			float mulFac = Timer / (float)trailLength;
			if (mulFac > 1f)
			{
				mulFac = 1f;
			}
			float factor = (i + 1) / (float)ShootProjPos.Count * mulFac;

			float width = 0;
			if (i >= 0)
			{
				width = 1f;
				if (i >= ShootProjPos.Count - 5)
				{
					width = 1 - MathF.Cos((ShootProjPos.Count - i) / 5f * MathHelper.Pi);
					width *= 0.5f;
				}
			}

			// timeValue, animate the trail.
			float timeValue = Timer * 0.05f;
			Vector2 drawPos = Projectile.Center;
			if (i >= 0)
			{
				drawPos = ShootProjPos[i].Pos;
			}
			Color drawColor = GetTrailColor(i, style);
			drawPos += offset;
			bars0.Add(drawPos + new Vector2(0, 1).RotatedBy(MathHelper.TwoPi * 2f / 3f) * trailWidth, drawColor, ModifyWarpTexCoordinate(factor, timeValue, 0, width));
			bars0.Add(drawPos, drawColor, ModifyWarpTexCoordinate(factor, timeValue, 1, width));
			bars1.Add(drawPos + new Vector2(0, 1).RotatedBy(MathHelper.TwoPi * 1f / 3f) * trailWidth, drawColor, ModifyWarpTexCoordinate(factor, timeValue, 2, width));
			bars1.Add(drawPos, drawColor, ModifyWarpTexCoordinate(factor, timeValue, 3, width));
			bars2.Add(drawPos + new Vector2(0, 1).RotatedBy(MathHelper.TwoPi * 0f / 3f) * trailWidth, drawColor, ModifyWarpTexCoordinate(factor, timeValue, 4, width));
			bars2.Add(drawPos, drawColor, ModifyWarpTexCoordinate(factor, timeValue, 5, width));
		}
	}

	public Color GetTrailColor(int index, int style)
	{
		if (style == 1)
		{
			if (index >= 0 && index < ShootProjPos.Count)
			{
				if (!ShootProjPos[index].Active)
				{
					return Color.Transparent;
				}
			}
			GradientColor colorG = new GradientColor();
			colorG.colorList.Add((new Color(1f, 1f, 1f, 0), 0f));
			colorG.colorList.Add((new Color(1f, 1f, 0.9f, 0), 0.17f));
			colorG.colorList.Add((new Color(0.8f, 0.6f, 0.15f, 0), 0.27f));
			colorG.colorList.Add((new Color(0.7f, 0.3f, 0f, 0), 0.4f));
			colorG.colorList.Add((new Color(0.5f, 0f, 0.1f, 0), 0.7f));
			colorG.colorList.Add((new Color(0f, 0f, 0f, 0f), 1f));
			float factor = index / (float)ShootProjPos.Count;
			return colorG.GetColor(1 - factor);
		}
		var normalDir = Projectile.velocity;
		if (index >= 1)
		{
			normalDir = ShootProjPos[index - 1].Pos - ShootProjPos[index].Pos;
		}
		normalDir = normalDir.NormalizeSafe();
		float warpValue = 1f;
		if (!ShootProjPos[index].Active)
		{
			warpValue = 0;
		}
		var drawColor = new Color(1 - (normalDir.X + 25f) / 50f, 1 - (normalDir.Y + 25f) / 50f, warpValue, 1);
		return drawColor;
	}

	public Vector3 ModifyWarpTexCoordinate(float factor, float timeValue, float phase, float widthValue)
	{
		float x = factor + timeValue;
		float y = 1;
		float z = widthValue;
		if (phase == 2)
		{
			y = 0;
		}
		if (phase % 2 == 1)
		{
			y = 0.5f;
		}
		return new Vector3(x, y, z);
	}
}

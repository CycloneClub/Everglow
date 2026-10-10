using Everglow.Commons.DataStructures;
using Everglow.Commons.Utilities;
using Everglow.Commons.Vertex;
using Terraria.Audio;

namespace Everglow.Commons.Templates.Weapons.Slingshots;

public abstract class SlingshotProjectile : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 36;
		Projectile.height = 36;
		Projectile.tileCollide = false;
		Projectile.DamageType = DamageClass.Ranged;
		SetDef();
	}

	public virtual void SetDef()
	{
	}

	/// <summary>
	/// 内部变量,别动
	/// </summary>
	public bool Release = true;

	/// <summary>
	/// 内部变量,别动
	/// </summary>
	public int Power = 0;

	/// <summary>
	/// 默认NormalAmmo(常规弹)
	/// </summary>
	public int ShootProjType = ModContent.ProjectileType<NormalAmmo>();

	/// <summary>
	/// 中心到弹幕绑绳子位置的距离,默认14
	/// </summary>
	public int SlingshotLength = 14;

	/// <summary>
	/// Y形弹弓两头的距离,默认5
	/// </summary>
	public int SplitBranchDis = 5;

	/// <summary>
	/// 最大蓄力,默认120(2s时间)
	/// </summary>
	public int MaxPower = 120;

	// TODO:以下的MouseLeft需要联机同步
	public override void AI()
	{
		if (Power < MaxPower)
		{
			Power++;
		}

		Player player = Main.player[Projectile.owner];
		player.heldProj = Projectile.whoAmI;
		if (Power == 24 && player.controlUseItem)
		{
			SoundEngine.PlaySound(new SoundStyle("Everglow/Myth/Misc/Sounds/NewSlingshot" + Main.rand.Next(8).ToString()).WithVolumeScale(0.8f), Projectile.Center);
		}

		Vector2 MouseToPlayer = Main.MouseWorld - player.MountedCenter;
		if (player.controlUseItem && Release)
		{
			Projectile.rotation = (float)(Math.Atan2(MouseToPlayer.Y, MouseToPlayer.X) + Math.PI * 0.25);
			Projectile.Center = player.MountedCenter + Vector2.Normalize(MouseToPlayer) * 15f + new Vector2(0, -4);
			Projectile.timeLeft = 5 + Power;
		}
		float DrawRot;
		if (Projectile.Center.X < player.MountedCenter.X)
		{
			player.direction = -1;
			DrawRot = Projectile.rotation - MathF.PI / 4f;
		}
		else
		{
			player.direction = 1;
			DrawRot = Projectile.rotation - MathF.PI / 4f;
		}
		Vector2 MinusShootDir = new Vector2(SlingshotLength, -SlingshotLength).RotatedBy(DrawRot) + Projectile.Center - Main.MouseWorld;
		Vector2 SlingshotStringHead = new Vector2(SlingshotLength, -SlingshotLength).RotatedBy(DrawRot) + Vector2.Normalize(MinusShootDir) * Power / 3f;
		if (player.direction == -1)
		{
			MinusShootDir = new Vector2(SlingshotLength, -SlingshotLength).RotatedBy(DrawRot + Math.PI / 2d) + Projectile.Center - Main.MouseWorld;
			SlingshotStringHead = new Vector2(SlingshotLength, -SlingshotLength).RotatedBy(DrawRot + Math.PI / 2d) + Vector2.Normalize(MinusShootDir) * Power / 3f;
		}
		if (!player.controlUseItem && Release)
		{
			Projectile.Center = player.MountedCenter + Vector2.Normalize(MouseToPlayer) * 15f + new Vector2(0, -4);
			SoundEngine.PlaySound(new SoundStyle("Everglow/Myth/Misc/Sounds/SlingshotShoot"), Projectile.Center);
			if (Power == MaxPower)
			{
				SoundEngine.PlaySound(new SoundStyle("Everglow/Myth/Misc/Sounds/SlingshotShoot2"), Projectile.Center);
			}

			Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + SlingshotStringHead, -Vector2.Normalize(MinusShootDir) * (float)(Power / 5f + 8f), ShootProjType, (int)(Projectile.damage * (1 + Power / 40f)), Projectile.knockBack, player.whoAmI, Power / 450f);

			Projectile.timeLeft = 5;
			Release = false;
		}
		if (!player.controlUseItem && !Release)
		{
			Projectile.Center = player.MountedCenter + Vector2.Normalize(MouseToPlayer) * 15f + new Vector2(0, -4);
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		return false;
	}

	public override void PostDraw(Color lightColor)
	{
		Player player = Main.player[Projectile.owner];
		var TexMain = (Texture2D)ModContent.Request<Texture2D>(Texture);
		Color drawColor = Lighting.GetColor((int)(Projectile.Center.X / 16.0), (int)(Projectile.Center.Y / 16.0));
		SpriteEffects spriteEffect = SpriteEffects.None;
		float DrawRot = Projectile.rotation - MathF.PI / 4f;

		Vector2 MouseToPlayer = Main.MouseWorld - player.MountedCenter;
		Vector2 SlingshotStringHead = new Vector2(SlingshotLength, -SlingshotLength).RotatedBy(DrawRot) + Projectile.Center - Main.MouseWorld;
		Vector2 SlingshotStringTail = new Vector2(SlingshotLength, -SlingshotLength).RotatedBy(DrawRot) + Vector2.Normalize(SlingshotStringHead) * Power * 0.2625f;
		if (player.direction == -1)
		{
			SlingshotStringHead = new Vector2(SlingshotLength, -SlingshotLength).RotatedBy(DrawRot + Math.PI / 2d) + Projectile.Center - Main.MouseWorld;
			SlingshotStringTail = new Vector2(SlingshotLength, -SlingshotLength).RotatedBy(DrawRot + Math.PI / 2d) + Vector2.Normalize(SlingshotStringHead) * Power * 0.2625f;
		}

		Vector2 SlingshotStringTailToPlayer = player.MountedCenter - (Projectile.Center + SlingshotStringTail);

		if (Projectile.Center.X < player.MountedCenter.X)
		{
			player.direction = -1;
			spriteEffect = SpriteEffects.FlipVertically;
			if (player.controlUseItem)
			{
				player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (float)(Math.Atan2(MouseToPlayer.Y, MouseToPlayer.X) - MathF.PI * 0.75f));
				player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, (float)(Math.Atan2(SlingshotStringTailToPlayer.Y, SlingshotStringTailToPlayer.X) - MathF.PI * 1.5f));
			}
		}
		else
		{
			player.direction = 1;
			if (player.controlUseItem)
			{
				player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (float)(Math.Atan2(MouseToPlayer.Y, MouseToPlayer.X) - MathF.PI * 0.25f));
				player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, (float)(Math.Atan2(SlingshotStringTailToPlayer.Y, SlingshotStringTailToPlayer.X) + MathF.PI * 0.5f));
			}
		}
		Main.spriteBatch.Draw(TexMain, Projectile.Center - Main.screenPosition, null, drawColor, DrawRot, TexMain.Size() / 2f, 1f, spriteEffect, 0);
		DrawString();
	}

	/// <summary>
	/// 绘制弹弓的弦 TODO:这部分的绘制移到玩家身后
	/// </summary>
	public virtual void DrawString()
	{
		Player player = Main.player[Projectile.owner];
		Color drawColor = Lighting.GetColor((int)(Projectile.Center.X / 16.0), (int)(Projectile.Center.Y / 16.0));
		float DrawRot = Projectile.rotation - MathF.PI / 4f;
		Vector2 HeadCenter = new Vector2(SlingshotLength, -SlingshotLength).RotatedBy(DrawRot);
		if (player.direction == -1)
		{
			HeadCenter = new Vector2(SlingshotLength, -SlingshotLength).RotatedBy(DrawRot + Math.PI / 2d);
		}

		HeadCenter += Projectile.Center - Main.screenPosition;
		Vector2 SlingshotStringHead = new Vector2(SlingshotLength, -SlingshotLength).RotatedBy(DrawRot) + Projectile.Center - Main.MouseWorld;
		Vector2 SlingshotStringTail = new Vector2(SlingshotLength, -SlingshotLength).RotatedBy(DrawRot) + Vector2.Normalize(SlingshotStringHead) * Power * 0.2625f;
		if (player.direction == -1)
		{
			SlingshotStringHead = new Vector2(SlingshotLength, -SlingshotLength).RotatedBy(DrawRot + Math.PI / 2d) + Projectile.Center - Main.MouseWorld;
			SlingshotStringTail = new Vector2(SlingshotLength, -SlingshotLength).RotatedBy(DrawRot + Math.PI / 2d) + Vector2.Normalize(SlingshotStringHead) * Power * 0.2625f;
		}
		SlingshotStringTail += Projectile.Center - Main.screenPosition;
		Vector2 Head1 = HeadCenter + HeadCenter.RotatedBy(Math.PI / 8 + DrawRot).SafeNormalize(Vector2.Zero) * SplitBranchDis;
		Vector2 Head2 = HeadCenter - HeadCenter.RotatedBy(Math.PI / 8 + DrawRot).SafeNormalize(Vector2.Zero) * SplitBranchDis;
		if (player.direction == -1)
		{
			Head1 = HeadCenter + HeadCenter.RotatedBy(Math.PI / 8 * 5 + DrawRot).SafeNormalize(Vector2.Zero) * SplitBranchDis;
			Head2 = HeadCenter - HeadCenter.RotatedBy(Math.PI / 8 * 5 + DrawRot).SafeNormalize(Vector2.Zero) * SplitBranchDis;
		}
		SpriteBatchState spriteBatchState = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		DrawTexLine(Head1, SlingshotStringTail, 1, drawColor, ModAsset.String.Value);
		DrawTexLine(Head2, SlingshotStringTail, 1, drawColor, ModAsset.String.Value);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(spriteBatchState);
	}

	public static void DrawTexLine(Vector2 StartPos, Vector2 EndPos, float width, Color color, Texture2D tex)
	{
		Vector2 Width = Vector2.Normalize(StartPos - EndPos).RotatedBy(Math.PI / 2d) * width;

		var vertex2Ds = new List<Vertex2D>();

		vertex2Ds.Add(new Vertex2D(StartPos + Width, color, new Vector3(0, 0, 0)));
		vertex2Ds.Add(new Vertex2D(StartPos - Width, color, new Vector3(0, 1, 0)));

		vertex2Ds.Add(new Vertex2D(EndPos + Width, color, new Vector3(1, 0, 0)));
		vertex2Ds.Add(new Vertex2D(EndPos - Width, color, new Vector3(1, 1, 0)));

		Main.graphics.GraphicsDevice.Textures[0] = tex;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertex2Ds.ToArray(), 0, vertex2Ds.Count - 2);
	}
}

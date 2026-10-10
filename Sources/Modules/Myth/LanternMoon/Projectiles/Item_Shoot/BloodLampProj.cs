using Everglow.Commons.Coroutines;
using ReLogic.Content;
using Terraria.Audio;

namespace Everglow.Myth.LanternMoon.Projectiles.Item_Shoot;

public class BloodLampProj : ModProjectile
{
	private CoroutineManager _coroutineManager = new CoroutineManager();

	public override void SetDefaults()
	{
		Projectile.width = 38;
		Projectile.height = 60;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 600;
		Projectile.scale = 1;
		for (int x = -1; x < 15; x++)
		{
			bLantern[x + 1] = ModContent.Request<Texture2D>("Everglow/Myth/LanternMoon/Projectiles/BloodLampFrame/BloodLamp_" + x.ToString());
		}
	}

	// 这种贴图每个Proj都是一样的也不会变化，干脆直接readonly然后在SSD里Request，然后所有Proj共用一个数组
	private static Asset<Texture2D>[] bLantern = new Asset<Texture2D>[16];

	// 这个bool数组每个Proj不同，所以要到Clone里new，但是直接构造是不必要的，因为不是clone获得的那个实例不会调用AI与Draw
	private bool[] noPedal;

	// 值类型就不必在clone里重新初始化了
	private float pearlRot = 0;
	private float pearlOmega = 0;
	private int col = 0;
	private int timer = 0;
	private static Vector2[] pedalPos = new Vector2[]
	{
			new Vector2(19, 19),
			new Vector2(19, 19),
			new Vector2(12, 9),
			new Vector2(26, 9),
			new Vector2(19, 8),
			new Vector2(34, 14),
			new Vector2(4, 14),
			new Vector2(8, 10),
			new Vector2(30, 10),
			new Vector2(8, 25),
			new Vector2(30, 25),
			new Vector2(32, 21),
			new Vector2(6, 21),
			new Vector2(19, 23),
			new Vector2(23, 16),
			new Vector2(15, 16),
	};

	public override void AI()
	{
		timer++;
		if (Projectile.localAI[0] == 0)
		{
			_coroutineManager.StartCoroutine(new Coroutine(Task()));
			Projectile.localAI[0] = 1;
		}
		_coroutineManager.Update();

		Projectile.rotation = Projectile.velocity.X * 0.05f;
		Projectile.velocity *= 0.9f * Projectile.timeLeft / 600f;
		if (Projectile.velocity.Length() > 0.3f)
		{
			Projectile.velocity.Y -= 0.15f * Projectile.timeLeft / 600f;
		}
	}

	public override void PostDraw(Player player, Color lightColor)
	{
		pearlOmega += (Projectile.rotation - pearlRot) / 75f;
		pearlOmega *= 0.95f;
		pearlRot += pearlOmega;
		Color color = Lighting.GetColor((int)(Projectile.Center.X / 16d), (int)(Projectile.Center.Y / 16d));
		if (noPedal == null)
		{
			noPedal = new bool[16];
		}

		if (bLantern == null)
		{
			bLantern = new Asset<Texture2D>[16];
			for (int x = -1; x < 15; x++)
			{
				bLantern[x + 1] = ModContent.Request<Texture2D>("Everglow/Myth/LanternMoon/Projectiles/BloodLampFrame/BloodLamp_" + x.ToString());
			}
		}
		for (int x = 0; x < 16; x++)
		{
			float Rot = Projectile.rotation;
			var Cen = new Vector2(19f, 19f);
			if (x >= 2)
			{
				Rot = -(float)Math.Sin(Main.time / 26d + x) / 7f + Projectile.rotation;
			}
			else if (x == 0)
			{
				Rot = pearlRot;
				Cen = new Vector2(19f, 51f);
			}
			if (!noPedal[x] || x < 1)
			{
				Main.spriteBatch.Draw(bLantern[x].Value, Projectile.Center - Main.screenPosition + Cen - bLantern[x].Size() / 2f/*坐标校正*/, null, color, Rot, Cen, 1, SpriteEffects.None, 0);
				if (!noPedal[x] && x == 1)
				{
					var Cen2 = new Vector2(19f, 19f);
					Texture2D Glow = ModAsset.BloodLamp_Glow.Value;
					Main.spriteBatch.Draw(Glow, Projectile.Center - Main.screenPosition + Cen2 - Glow.Size() / 2f/*坐标校正*/, null, new Color(col, col, col, 0), Projectile.rotation, Cen2, 1, SpriteEffects.None, 0);
				}
			}
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		return false;
	}

	private IEnumerator<ICoroutineInstruction> Task()
	{
		SoundEngine.PlaySound(
			new SoundStyle(
			"Everglow/Myth/LanternMoon/Sounds/PowerBomb"), Projectile.Center);
		_coroutineManager.StartCoroutine(new Coroutine(ControlVolume()));
		_coroutineManager.StartCoroutine(new Coroutine(JitterFlash()));
		yield return new WaitForFrames(170);
		yield return new WaitForFrames(75);
		col = 255;

		noPedal[1] = true;
		yield return new WaitForFrames(5);

		yield return new WaitForFrames(10);

		yield return new WaitForFrames(65);
		Projectile.Kill();
	}

	private IEnumerator<ICoroutineInstruction> JitterFlash()
	{
		while (true)
		{
			double x0 = timer * 0.0156923;
			col = (int)(Math.Clamp(Math.Sin(x0 * x0) + Math.Log(x0 + 1), 0, 2) / 2d * 255);
			yield return new SkipThisFrame();
		}
	}

	private IEnumerator<ICoroutineInstruction> ControlVolume()
	{
		float originalVolume = Main.musicVolume;
		for (int i = 0; i < 260; i++)
		{
			Main.musicVolume *= 0.98f;
			yield return new SkipThisFrame();
		}
		for (int i = 0; i < 65; i++)
		{
			Main.musicVolume = Main.musicVolume * 0.96f + originalVolume * 0.04f;
			yield return new SkipThisFrame();
		}
		Main.musicVolume = originalVolume;
	}
}

using Terraria.DataStructures;

namespace Everglow.Myth.Acytaea.Projectiles;

public class AcytaeaSwordArray_0 : ModProjectile
{
	public override string Texture => "Everglow/Myth/Acytaea/Projectiles/AcytaeaSword_projectile";
	public override void SetDefaults()
	{
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 3600;
		Projectile.extraUpdates = 0;
		Projectile.scale = 1f;
		Projectile.hostile = true;
		Projectile.friendly = false;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.penetrate = -1;
		Projectile.DamageType = DamageClass.Melee;

		Projectile.width = 80;
		Projectile.height = 80;
	}
	public int Timer = 0;
	public NPC Owner = new NPC();
	public override void OnSpawn(IEntitySource source)
	{
		int index = (int)Projectile.ai[0];
		if (index >= 0 && index < 200)
		{
			Owner = Main.npc[index];
		}
		else
		{
			Projectile.Kill();
		}
		base.OnSpawn(source);
	}
	public override void AI()
	{
		Timer++;
		// CheckFrame();
		if (Owner == null || !Owner.active)
		{
			Projectile.Kill();
		}
		Vector2 toNPC = Owner.Center - Projectile.Center;
		Projectile.rotation = MathF.Atan2(toNPC.Y, toNPC.X) + MathHelper.PiOver2 * 2;
		float timeValue = Timer / 40f;
		timeValue = MathF.Pow(timeValue, 3);
		if (Timer > 40f)
		{
			timeValue = 1f;
		}
		Projectile.Center = Owner.Center + new Vector2(0, Projectile.ai[2]).RotatedBy(Projectile.ai[1] * timeValue);
		if (Timer > 120)
		{
			if (Main.rand.Next(Math.Min(Timer, 480), 480) > 470)
			{
				Projectile.Kill();
			}
		}
	}
	public override void OnKill(int timeLeft)
	{
		Vector2 toNPC = Owner.Center - Projectile.Center;
		Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, -toNPC * 0.2f, ModContent.ProjectileType<AcytaeaFlySword>(), Projectile.damage, Projectile.knockBack);
		p.frame = Main.rand.Next(4);
		p.frameCounter = Main.rand.Next(6);
	}
	public override bool PreDraw(ref Color lightColor)
	{
		return false;
	}
	public override void PostDraw(Color lightColor)
	{
		Texture2D tex = ModAsset.AcytaeaFlySword_red.Value;
		Rectangle projFrame = new Rectangle(0, Projectile.frame * Projectile.height, Projectile.width, Projectile.height);
		Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, projFrame, new Color(255, 0, 215, 155), Projectile.rotation + MathHelper.PiOver4, new Vector2(40), Projectile.scale, SpriteEffects.None, 0);
	}
}

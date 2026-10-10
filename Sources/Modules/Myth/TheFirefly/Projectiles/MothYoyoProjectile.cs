using Everglow.Commons.Templates.Weapons.Yoyos;
using Everglow.Myth.TheFirefly.Items.Accessories;

namespace Everglow.Myth.TheFirefly.Projectiles;

internal class MothYoyoProjectile : YoyoProjectile
{
	private readonly Vector3[] cubeVec =
	[
		new Vector3(1, 1, 1),
		new Vector3(1, 1, -1),
		new Vector3(1, -1, -1),
		new Vector3(1, -1, 1),
		new Vector3(-1, 1, 1),
		new Vector3(-1, 1, -1),
		new Vector3(-1, -1, -1),
		new Vector3(-1, -1, 1),
	];

	public override void SetStaticDefaults()
	{
		ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 40f;
		ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 300f;
		ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 16f;
	}

	public override void PostAI()
	{
		if (Projectile.localAI[0] == 1 && Main.myPlayer == Projectile.owner)
		{
			for (int i = 0; i < cubeVec.Length; i++)
			{
				var proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<MothYoyoSub>(), Projectile.damage / 2, 0, Projectile.owner, Projectile.whoAmI);
				(proj.ModProjectile as MothYoyoSub).targetPos = cubeVec[i] * 30;
				proj.CritChance = Projectile.CritChance;
				proj.netUpdate = true;
			}
			if (MothEye.LocalOwner != null && MothEye.LocalOwner.TryGetModPlayer(out MothEyePlayer mothEyePlayer))
			{
				if (mothEyePlayer.MothEyeEquipped && ModContent.GetInstance<FireflyBiome>().IsBiomeActive(Main.LocalPlayer) && Main.hardMode)
				{
					for (int i = 0; i < cubeVec.Length; i++)
					{
						var proj2 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<MothYoyoSub>(), Projectile.damage / 2, 0, Projectile.owner, Projectile.whoAmI);
						(proj2.ModProjectile as MothYoyoSub).targetPos = cubeVec[i] * 60;
						proj2.CritChance = Projectile.CritChance;
						proj2.netUpdate = true;
					}
				}
			}
		}
	}

	public override void PostDraw(Player player, Color lightColor)
	{
		Player previousDrawPlayer = DrawPlayer;
		DrawPlayer = player;
		try
		{
			Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
			Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), Projectile.rotation, tex.Size() / 2, Projectile.scale, 0, 0);
		}
		finally
		{
			DrawPlayer = previousDrawPlayer;
		}
	}

	public override Color ModifyYoyoStringColor_VanillaRender(int playerStringColor, Vector2 worldPos, float index, float stringCount) => new Color(0, 150, 255, 0) * 0.5f;
}

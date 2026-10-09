namespace Everglow.SpellAndSkull.Projectiles.RazorbladeTyphoon;

internal class RazorbladeTyphoonBook : MagicBookProjectile
{
	public override void SetDef()
	{
		UseGlow = false;
		ItemType = ItemID.RazorbladeTyphoon;
		DustType = DustID.RazorbladeTyphoon;
		ProjType = ModContent.ProjectileType<TyphoonII>();
		effectColor = new Color(0, 125, 225, 20);

		// TODO：等到凝胶的贴图优化完再改
		// string pathBase = "SpellAndSkull/Textures/";
		// FrontTexPath = pathBase + "RazorbladeTyphoon_A";
		// PaperTexPath = pathBase + "RazorbladeTyphoon_C";
		// BackTexPath = pathBase + "RazorbladeTyphoon_B";

		// TexCoordTop = new Vector2(0, 6);
		// TexCoordLeft =  new Vector2(10, 30);
		// TexCoordDown = new Vector2(32, 22);
		// TexCoordRight =  new Vector2(21, 0);
	}
	internal float ConstantUsingTime = 0;
	public override void SpecialAI()
	{
		ConstantUsingTime += 1;
	}
	public override void OnKill(int timeLeft)
	{
		int HitType = ModContent.ProjectileType<HurricaneMask>();
		float WindHole = Math.Min(ConstantUsingTime / 720f - 0.2f, 1f);
		if (WindHole > 0 && WindHole < 0.3f)
		{
			WindHole = 0.3f;
		}

		if (WindHole > 0)
		{

			var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, HitType, Projectile.damage, Projectile.knockBack * 6, Projectile.owner, WindHole/*ai[0]代表强度*/, 0);
			p.CritChance = (int)Main.player[Projectile.owner].GetTotalCritChance(DamageClass.Magic);
		}
		ConstantUsingTime = 0;
		base.OnKill(timeLeft);
	}
}

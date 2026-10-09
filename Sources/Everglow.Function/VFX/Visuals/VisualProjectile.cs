using Everglow.Commons.Enums;
using Everglow.Commons.Interfaces;
using Terraria.DataStructures;

namespace Everglow.Commons.VFX.Visuals;

public abstract class VisualProjectile : ModProjectile, IVisual
{
	internal int SpawnWhoAmI = -1;
	internal int SpawnType = -1;
	public override void OnSpawn(IEntitySource source)
	{
		SpawnWhoAmI = Projectile.whoAmI;
		SpawnType = Projectile.type;
	}
	public bool Active => Projectile.active && SpawnWhoAmI == Projectile.whoAmI && SpawnType == Projectile.type;

	public virtual CodeLayer DrawLayer => CodeLayer.PostDrawProjectiles;

	public bool Visible => !Projectile.hide && VFXManager.InScreen(Projectile.position, ProjectileID.Sets.DrawScreenCheckFluff[Type]);

	public virtual void Draw()
	{
	}

	public override void SetDefaults()
	{
		if (!Main.gameMenu)
		{
			Ins.VFXManager.Add(this);
		}
	}

	public void Kill()
	{
		Projectile.Kill();
	}

	public void Update()
	{
	}
}

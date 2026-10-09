using Everglow.Commons.Coroutines;
using Everglow.Commons.Skeleton2D;
using Everglow.Commons.Skeleton2D.Reader;
using Everglow.Commons.Skeleton2D.Renderer;
using Everglow.Commons.Skeleton2D.Renderer.DrawCommands;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.WorldBuilding;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Summon.FaelanternProj;

public class FaelanternProj : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.SummonProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 112;
		Projectile.height = 112;
		Projectile.netImportant = true;
		Projectile.friendly = true;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 360000;
		Projectile.penetrate = -1;
		Projectile.sentry = true;
		Projectile.DamageType = DamageClass.Summon;
		// Projectile.hide = true;
	}

	public void Suicide()
	{
		int tileX = (int)Projectile.Center.X / 16;
		int tileY = (int)Projectile.Center.Y / 16;
		if (Main.tile[tileX, tileY + 3] == null || Main.tile[tileX - 1, tileY + 3] == null || Main.tile[tileX - 1, tileY + 3] == null
			|| !WorldGen.SolidTile2(tileX, tileY + 3) || !WorldGen.SolidTile2(tileX - 1, tileY + 3) || !WorldGen.SolidTile2(tileX - 1, tileY + 3))
		{
			Projectile.Kill();
		}
	}

	public override bool? CanDamage()
	{
		return false;
	}

	public CoroutineManager _coroutineManager = new CoroutineManager();
	public Skeleton2D FaelanternSkeleton;
	public SkeletonRenderer skeletonRenderer = new SkeletonRenderer();
	public SkeletonDebugRenderer skeletonDebugRenderer = new SkeletonDebugRenderer();

	public override void OnSpawn(IEntitySource source)
	{
		var data = Mod.GetFileBytes(ModAsset.Faelanternj_Path);
		_coroutineManager.StartCoroutine(new Coroutine(Growth()));
		if (FaelanternSkeleton == null)
		{
			var json = Mod.GetFileBytes(ModAsset.Faelanternj_Path);
			var atlas = Mod.GetFileBytes(ModAsset.Faelanterna_Path);
			FaelanternSkeleton = Skeleton2DReader.ReadSkeleton(atlas, json, ModAsset.FaelanternProj.Value);
			FaelanternSkeleton.AnimationState.SetAnimation(0, "idle", true);
		}
	}

	private int timer = 0;
	private Projectile Fae;

	public override void AI()
	{
		FaelanternSkeleton.AnimationState.Apply(FaelanternSkeleton.Skeleton);
		timer++;
		if (timer == 10)
		{
			FaelanternSkeleton.Skeleton.UpdateWorldTransform();
			var pos = new Vector2(FaelanternSkeleton.Skeleton.FindBone("bone6").WorldX, FaelanternSkeleton.Skeleton.FindBone("bone6").WorldY);
			Fae = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), pos, Vector2.Zero, ModContent.ProjectileType<Fae>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
			Fae.ai[0] = Projectile.whoAmI;
			Fae.ai[1] = -1;
		}
		_coroutineManager.Update();
		FaelanternSkeleton.Position = Projectile.Bottom;
		FaelanternSkeleton.Skeleton.FindBone("root").Rotation = Projectile.rotation / Spine.MathUtils.DegRad;
		FaelanternSkeleton.Skeleton.FindBone("root").ScaleY = Projectile.spriteDirection;
		FaelanternSkeleton.Skeleton.FindBone("root").ScaleX = 1;
		Projectile.velocity = Vector2.zeroVector;
		Suicide();
		return;
	}

	public IEnumerator<ICoroutineInstruction> Growth()
	{
		FaelanternSkeleton.AnimationState.SetAnimation(0, "growth", true);
		for (int i = 0; i < 120; i++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 2f)).RotatedByRandom(MathHelper.TwoPi);
			if (i % 2 == 0)
			{
				var somg = new RockSmogDust
				{
					Velocity = newVelocity,
					Active = true,
					Visible = true,
					Position = Projectile.Center + new Vector2(0, 25f).RotatedByRandom(MathHelper.PiOver2),
					MaxTime = Main.rand.Next(25, 32),
					Scale = Main.rand.NextFloat(25f, 50f),
					Rotation = Main.rand.NextFloat(6.283f),
					ai = [Main.rand.NextFloat(0.0f, 0.93f), 0],
				};
				Ins.VFXManager.Add(somg);
			}

			FaelanternSkeleton.AnimationState.Update(1 / 60f);
			yield return new SkipThisFrame();
		}
		_coroutineManager.StartCoroutine(new Coroutine(NextAttack()));
	}

	public IEnumerator<ICoroutineInstruction> Idle()
	{
		FaelanternSkeleton.AnimationState.SetAnimation(0, "idle", true);
		for (int i = 0; i < 240; i++)
		{
			FaelanternSkeleton.AnimationState.Update(1 / 60f);
			yield return new SkipThisFrame();
		}
		_coroutineManager.StartCoroutine(new Coroutine(NextAttack()));
	}

	public IEnumerator<ICoroutineInstruction> Attack(int target)
	{
		FaelanternSkeleton.AnimationState.SetAnimation(0, "attack", true);
		for (int i = 0; i < 120; i++)
		{
			FaelanternSkeleton.AnimationState.Update(1 / 60f);

			if (i == 40 || i == 50 || i == 60)
			{
				NPC Target = Main.npc[target];

				int direction = 1;
				if (Target.Center.X > Projectile.Center.X)
				{
					direction = -1;
				}

				int tileX = (int)Target.Center.X / 16 - direction * (i / 2 - 20 + Main.rand.Next(-1, 2));
				int tileY = (int)Target.Center.Y / 16 - 3;

				for (; tileY < Main.maxTilesY - 10 && (Findtile(tileX + 3 * direction, tileY + 4) || Findtile(tileX + 4 * direction, tileY + 4) || Findtile(tileX + 5 * direction, tileY + 4) || Findtile(tileX + 6 * direction, tileY + 4)); tileY++)
				{ }

				var pos = new Vector2(tileX * 16, tileY * 16 + 8);
				Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), pos, Vector2.Zero, ModContent.ProjectileType<Faelanternbranch>(), Projectile.damage, Projectile.knockBack, Projectile.owner, direction);
			}
			yield return new SkipThisFrame();
		}
		_coroutineManager.StartCoroutine(new Coroutine(NextAttack()));
	}

	public bool Findtile(int tileX, int tileY)
	{
		return Main.tile[tileX, tileY] == null || !WorldGen.SolidTile2(tileX, tileY);
	}

	public IEnumerator<ICoroutineInstruction> Charm(int target)
	{
		FaelanternSkeleton.AnimationState.SetAnimation(0, "charm", true);
		for (int i = 0; i < 160; i++)
		{
			FaelanternSkeleton.AnimationState.Update(1 / 60f);
			if (i == 30)
			{
				Fae.ai[1] = target;
			}
			yield return new SkipThisFrame();
		}

		_coroutineManager.StartCoroutine(new Coroutine(NextAttack()));
	}

	private int charmindex;

	private static Conditions.NotNull _cachedConditions_notNull = new Conditions.NotNull();
	private static Conditions.IsSolid _cachedConditions_solid = new Conditions.IsSolid();

	public IEnumerator<ICoroutineInstruction> NextAttack()
	{
		Player Owner = Main.player[Projectile.owner];
		float Searchdis = 1000f;
		int charmtarget = -1;
		int attacktarget = -1;
		bool charm = false;
		bool attack = false;

		if (attacktarget <= 0)
		{
			foreach (NPC npc in Main.npc)
			{
				if (npc != null && npc.CanBeChasedBy() && Projectile.Distance(npc.Center) < Searchdis)
				{
					if (WorldUtils.Find(npc.Center.ToTileCoordinates(), Searches.Chain(new Searches.Down(8), _cachedConditions_notNull), out var _))
					{
						attacktarget = npc.whoAmI;
						charmindex--;
						attack = true;
						break;
					}
				}
			}
		}

		if (charmindex < 0 || !attack)
		{
			if (charmtarget < 0)
			{
				foreach (NPC npc in Main.npc)
				{
					if (npc != null && npc.CanBeChasedBy())
					{
						if (Owner.Distance(npc.Center) < Searchdis * 0.75f)
						{
							if (npc.buffImmune[BuffID.Confused] || npc.HasBuff(BuffID.Confused))
							{
								continue;
							}
							charmtarget = npc.whoAmI;
							charmindex = 3;
							charm = true;
							break;
						}
					}
				}
			}
		}

		if (charm && Fae.ai[1] == -1)
		{
			_coroutineManager.StartCoroutine(new Coroutine(Charm(charmtarget)));
		}
		else if (attack)
		{
			_coroutineManager.StartCoroutine(new Coroutine(Attack(attacktarget)));
		}
		else
		{
			_coroutineManager.StartCoroutine(new Coroutine(Idle()));
		}
		yield break;
	}

	private Vector2[] eyepos =
	{
		new Vector2(4, 42),
		new Vector2(7, 44),
		new Vector2(15, 46),
		new Vector2(16, 30),
		new Vector2(20, 28),
		new Vector2(25, 18),
		new Vector2(24, 48),
		new Vector2(25, 32),
		new Vector2(32, 38),
	};

	private int[] closetimer = new int[9];

	public override bool PreDraw(ref Color lightColor)
	{
		if (FaelanternSkeleton == null)
		{
			var json = Mod.GetFileBytes(ModAsset.Faelanternj_Path);
			var atlas = Mod.GetFileBytes(ModAsset.Faelanterna_Path);
			FaelanternSkeleton = Skeleton2DReader.ReadSkeleton(atlas, json, ModAsset.FaelanternProj.Value);
			FaelanternSkeleton.AnimationState.SetAnimation(0, "idle", true);
		}

		skeletonDebugRenderer.DisableAll();
		skeletonDebugRenderer.DrawBones = true;

		FaelanternSkeleton.Skeleton.UpdateWorldTransform();

		skeletonRenderer.UseEnvironmentLight = true;
		skeletonRenderer.DrawOffset = -Main.screenPosition;

		var cmdList = skeletonRenderer.Draw(FaelanternSkeleton);

		skeletonRenderer.UseEnvironmentLight = true;
		if (Skeleton2D.ShowSkeleton)
		{
			cmdList.AddRange(skeletonDebugRenderer.Draw(FaelanternSkeleton));
		}
		var executer = new NaiveExecuter();
		executer.Execute(cmdList, Main.graphics.graphicsDevice);

		if (timer >= 120)
		{
			for (int i = 0; i < eyepos.Length; i++)
			{
				if (closetimer[i] > 0)
				{
					closetimer[i]--;
				}
				else
				{
					closetimer[i] = Main.rand.NextBool(300) ? 20 : 0;
				}

				Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, Projectile.Center - Main.screenPosition + eyepos[i], new Rectangle(0, 0, 1, 1), new Color(0.13f, 0.976f, 0.977f, 0.25f), 0f, Vector2.Zero, closetimer[i] > 0 ? 0f : i == 1 || i == 4 ? 2f : 1f, SpriteEffects.None, 0);
				Lighting.AddLight(Projectile.Center + eyepos[i], 0.05f, 0.249f, 0.25f);
			}
		}
		return false;
	}
}

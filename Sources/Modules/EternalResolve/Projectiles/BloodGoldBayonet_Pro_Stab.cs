using Everglow.Commons.Coroutines;
using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.Commons.Templates.Weapons.StabbingSwords.VFX;
using Everglow.EternalResolve.Buffs;
using Everglow.EternalResolve.Items.Weapons.StabbingSwords.Dusts;
using Everglow.EternalResolve.VFXs;

namespace Everglow.EternalResolve.Projectiles
{
	public class BloodGoldBayonet_Pro_Stab : StabbingProjectile_Stab
	{
		public NPC ProjTarget;

		public override void SetCustomDefaults()
		{
			StabColor = Color.Red;
			StabShade = 0.5f;
			StabEffectWidth = 0.4f;
			HitTileSparkColor = new Color(255, 0, 20, 185);
		}

		public override void DrawEffect(Color lightColor)
		{
			base.DrawEffect(lightColor);
		}

		public override IEnumerator<ICoroutineInstruction> Generate3DRingVFX(Vector2 velocity)
		{
			yield return new WaitForFrames(45);
			StabVFX v = new BloodGoldStabVFX()
			{
				Position = Projectile.Center + Projectile.velocity * StabDistance * 80 * (1 - StabTimer / 135f),
				Velocity = velocity,
				StabEffectColor = StabColor * 0.4f,
				Scale = 25,
				MaxTime = 10,
				TimeLeft = 10,
			};
			if (StabEndPoint_WorldPos == Vector2.Zero)
			{
				Ins.VFXManager.Add(v);
			}
			yield return new WaitForFrames(40);
			v = new BloodGoldStabVFX()
			{
				Position = Projectile.Center + Projectile.velocity * StabDistance * 80 * (1 - StabTimer / 135f),
				Velocity = velocity,
				StabEffectColor = StabColor * 0.4f,
				Scale = 15,
				MaxTime = 10,
				TimeLeft = 10,
			};
			if (StabEndPoint_WorldPos == Vector2.Zero)
			{
				Ins.VFXManager.Add(v);
			}
		}

		public override void AI()
		{
			if (Main.rand.NextBool(6))
			{
				Vector2 end = Projectile.Center + Projectile.velocity * 80 * StabDistance;
				if (StabEndPoint_WorldPos != Vector2.zeroVector)
				{
					end = StabEndPoint_WorldPos;
				}
				var dust = Dust.NewDustDirect(Vector2.Lerp(StabStartPoint_WorldPos, end, Main.rand.NextFloat(0.3f, 1f)) - new Vector2(4), 0, 0, ModContent.DustType<BloodShine>(), 0, 0, 0, default, Main.rand.NextFloat(0.85f, 1.2f));
				dust.velocity = new Vector2(0, Main.rand.NextFloat(2f)).RotateRandom(6.283);
			}

			base.AI();
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(ModContent.BuffType<BloodDrinking>(), 180);
			base.OnHitNPC(target, hit, damageDone);
		}
	}

	public class BloodDrinkingTarget : GlobalNPC
	{
		public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			base.PostDraw(npc, spriteBatch, screenPos, drawColor);
			if (npc.HasBuff(ModContent.BuffType<BloodDrinking>()))
			{
				float light = 1;
				float dark = 1;
				int indexBuff = npc.FindBuffIndex(ModContent.BuffType<BloodDrinking>());
				int restTime = npc.buffTime[indexBuff];
				if (restTime < 30)
				{
					light = restTime / 30f;
					dark = restTime / 6f;
				}
				Texture2D bloodMark = ModAsset.BloodDrinkingMark.Value;
				Texture2D bloodMarkBlack = ModAsset.BloodDrinkingMark_dark.Value;
				if (4f * npc.width * npc.height / 5000f * npc.scale > 1.5f)
				{
					spriteBatch.Draw(bloodMarkBlack, npc.Center - Main.screenPosition, null, new Color(dark, dark, dark, dark), 0, bloodMark.Size() * 0.5f, 3f, SpriteEffects.None, 0f);
					spriteBatch.Draw(bloodMark, npc.Center - Main.screenPosition, null, new Color(light, light, light, 0), 0, bloodMark.Size() * 0.5f, 3f, SpriteEffects.None, 0f);
				}
				else
				{
					spriteBatch.Draw(bloodMarkBlack, npc.Center - Main.screenPosition, null, new Color(dark, dark, dark, dark), 0, bloodMark.Size() * 0.5f, 4f * npc.width * npc.height / 5000f * npc.scale * 2, SpriteEffects.None, 0f);
					spriteBatch.Draw(bloodMark, npc.Center - Main.screenPosition, null, new Color(light, light, light, 0), 0, bloodMark.Size() * 0.5f, 4f * npc.width * npc.height / 5000f * npc.scale * 2, SpriteEffects.None, 0f);
				}
			}
		}
	}
}

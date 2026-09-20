using Everglow.Commons.DataStructures;
using Everglow.Commons.Graphics;
using Everglow.Commons.Interfaces;
using Terraria.DataStructures;
using Terraria.Enums;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Accessories
{
	public class AnemoShell : ModItem
	{
		public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

		public override void SetDefaults()
		{
			Item.DefaultToAccessory(28, 22);
			Item.SetShopValues(ItemRarityColor.Green2, Item.buyPrice(silver: 5600));
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			AnemoPlayer p = player.GetModPlayer<AnemoPlayer>();
			p.AnemoEquipped = true;
			p.Shell = Item;
		}

		public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			Player player = Main.LocalPlayer;
			if (!player.armor.Contains(Item))
			{
				return;
			}
			AnemoPlayer p = player.GetModPlayer<AnemoPlayer>();
			if (p.Cooldown != 0)
			{
				ValueBarHelper.DrawValueBar(spriteBatch, position + new Vector2(0, 16), 1 - p.Cooldown / (float)p.ProtectionCooldown, new Color(122, 226, 204), new Color(122, 226, 204));
			}
			base.PostDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale);
		}
	}

	public class AnemoPlayer : ModPlayer
	{
		public bool AnemoEquipped = false;
		public Item Shell = null;

		public int Cooldown = 0;
		public bool InEffect = false;
		public bool InProtection = false;

		public int EffectStartCounter = 0;
		public int ProtectionCounter = 0;
		public int EffectDuration = 5;
		public int ProtectionDuration = 12;
		public int ProtectionEndDuration = 32;
		public int ProtectionEaseoutDuration = 16;
		public int ProtectionCooldown = 3600;
		public int Radius = 24;

		public Vector2 CollidePosition = new(0, 0);
		public bool NeedDraw = false;
		public RenderTarget2D ShellTarget;
		public RenderTarget2D OriginTarget;

		// 气囊直径48，这些数表示对应帧时气囊顶部变瘪的像素数
		public int[] ProtectionAnimation =
			[0, 0, 1, 1, 2, 3, 5, 7, 10, 14, 18, 22, 27];

		public float[] AnimationTopRatio =
			[.5f, .6f, .64f, .68f, .7f, .72f, .75f, .77f, .78f, .80f, .81f, .81f, .81f, .80f, .78f, .77f, .75f, .72f, .7f, .68f, .64f, .6f, .5f];

		public float[] AnimationBottomRatio =
			[.5f, .4f, .35f, .3f, .25f, .21f, .18f, .15f, .12f, .08f, .05f, .05f, .05f, .08f, .12f, .15f, .18f, .21f, .25f, .3f, .35f, .4f, .5f];

		// 变瘪后横向拉伸比例
		public float[] AnimationStretch =
			[1f, 1f, 1f, 1f, 1.01f, 1.01f, 1.02f, 1.02f, 1.03f, 1.03f, 1.04f, 1.05f, 1.06f, 1.07f, 1.08f, 1.09f, 1.09f, 1.1f, 1.1f, 1.11f, 1.11f, 1.12f, 1.12f, 1.12f, 1.12f];

		private IHookHandler _resizeHook;
		private IHookHandler _exitHook;

		public override void OnEnterWorld()
		{
			Ins.MainThread.AddTask(() =>
			{
				AllocateRenderTarget();
			});
			_resizeHook = Ins.HookManager.AddHook(CodeLayer.ResolutionChanged, (Vector2 size) =>
			{
				ShellTarget?.Dispose();
				OriginTarget?.Dispose();
				AllocateRenderTarget();
			});
			_exitHook = Ins.HookManager.AddHook(CodeLayer.PreSaveAndQuit, () =>
			{
				ShellTarget?.Dispose();
				OriginTarget?.Dispose();
				if (_resizeHook != null)
				{
					Ins.HookManager.RemoveHook(_resizeHook);
					_resizeHook = null;
				}
			});
			base.OnEnterWorld();
		}

		public void AllocateRenderTarget()
		{
			var gd = Main.instance.GraphicsDevice;
			ShellTarget = new RenderTarget2D(gd, 30, 30);
			OriginTarget = new RenderTarget2D(gd, Main.screenWidth, Main.screenHeight);
		}

		public override void ResetEffects()
		{
			AnemoEquipped = false;
			Shell = null;
			if (Cooldown > 0)
			{
				Cooldown--;
			}
		}

		public int CheckGroundDistance()
		{
			for (int h = 0; h < 7; h++)
			{
				for (int i = -1; i <= 1; i++)
				{
					int offset = 16 * i;
					Vector2 pos = Player.Center + new Vector2(offset, h * 16 * Player.gravDir + 24);
					Point bottomPos = pos.ToTileCoordinates();
					bottomPos.X = Math.Clamp(bottomPos.X, 20, Main.maxTilesX - 20);
					bottomPos.Y = Math.Clamp(bottomPos.Y, 20, Main.maxTilesY - 20);
					if (TileUtils.PlatformCollision(pos) || ((Player.waterWalk || Player.waterWalk2) && Main.tile[bottomPos].LiquidAmount > 0 && !Player.wet))
					{
						return h;
					}
				}
			}
			return 999;
		}

		public void ResetStatus()
		{
			EffectStartCounter = 0;
			ProtectionCounter = 0;
			InEffect = false;
			InProtection = false;
		}

		public override void PreUpdate()
		{
			if (Player.noFallDmg || !AnemoEquipped)
			{
				return;
			}

			NeedDraw = true;

			// 气囊弹出动画
			if (InProtection)
			{
				if (ProtectionCounter < ProtectionDuration + ProtectionEndDuration + ProtectionEaseoutDuration)
				{
					ProtectionCounter++;
				}
				else
				{
					ResetStatus();
				}
				return;
			}

			if (Cooldown > 0)
			{
				return;
			}
			float fallDistance = (float)(Player.position.Y / 16f) - Player.fallStart;
			float fallDmgThreshold = 25 + Player.extraFall;
			bool hasDamage = fallDistance * Player.gravDir > fallDmgThreshold;

			int distance = CheckGroundDistance();
			if (distance > 10 || (!hasDamage && !InProtection))
			{
				EffectStartCounter--;
			}
			if (EffectStartCounter < 0)
			{
				ResetStatus();
				return;
			}

			if (!hasDamage)
			{
				return;
			}

			float dmg = 10f * (fallDistance - fallDmgThreshold);
			if (dmg <= 5)
			{
				return;
			}

			if (distance < 10)
			{
				InEffect = true;
			}

			// 根据最大下落速度，在对应高度以内触发保护效果
			if (InEffect && distance < Player.maxFallSpeed / 8f)
			{
				InProtection = true;
				Cooldown = ProtectionCooldown;
				Player.fallStart = (int)(Player.position.Y / 16);
				Player.fallStart2 = (int)(Player.position.Y / 16);
				Player.velocity.Y = -6;

				CollidePosition = Player.Center + new Vector2(0, distance * 16 * Player.gravDir + 24);
				Rectangle hitbox = Player.Hitbox;
				hitbox.Y += 80;
				CombatText.NewText(hitbox, new Color(0.4f, 1f, 0.8f, 1f), "POP!", true);

				// 烟雾效果
				AnemoShell_Smog aSmog = new AnemoShell_Smog()
				{
					Active = true,
					Visible = true,
					Timer = 0,
					TimeMax = 48,
					Center = CollidePosition,
				};
				Ins.VFXManager.Add(aSmog);
			}

			// 接近地面时从底部跳出的动画
			if (distance < 10 && EffectStartCounter < EffectDuration)
			{
				EffectStartCounter++;
			}
		}

		public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
		{
			if (!InEffect)
			{
				return;
			}
			if (!NeedDraw && !Main.gamePaused)
			{
				return;
			}

			NeedDraw = false;
			Texture2D texture = ModAsset.AnemoShell.Value;
			Color light = Lighting.GetColor(new Point((int)CollidePosition.X / 16, (int)CollidePosition.Y / 16));
			float lr = (float)light.R / 255;
			float lg = (float)light.G / 255;
			float lb = (float)light.B / 255;
			float la = (float)light.A / 255;

			if (InProtection)
			{
				if (ProtectionCounter > ProtectionDuration + ProtectionEndDuration)
				{
					float progress = (float)(ProtectionCounter - ProtectionDuration - ProtectionEndDuration) / ProtectionEaseoutDuration;
					byte alpha = (byte)((1 - progress) * 255);

					Color color = new((byte)(alpha * lr), (byte)(alpha * lg), (byte)(alpha * lb), (byte)(alpha * la));

					Main.spriteBatch.Draw(texture, CollidePosition - new Vector2(14, 12) - Main.screenPosition, color);

					return;
				}
				Main.spriteBatch.Draw(texture, CollidePosition - new Vector2(14, 12) - Main.screenPosition, light);
				SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
				Main.spriteBatch.End();
				List<Vertex2D> bars = [];
				List<Vertex2D> strokes = [];
				Effect effect = Commons.ModAsset.Shader2D.Value;
				Texture2D white = Commons.ModAsset.Point.Value;

				if (ProtectionCounter < ProtectionDuration)
				{
					float progress = (float)ProtectionCounter / ProtectionDuration;
					int size = AnimationTopRatio.Length;
					Color color = default;
					color.B = color.G = progress > 0.4f ? (byte)(255 - 100 * (progress - 0.4f)) : (byte)255;
					color.R = (byte)(255 - 200 * progress);
					color.A = 255;

					float stretch = AnimationStretch[ProtectionCounter];
					float offset = ProtectionAnimation[ProtectionCounter];
					var basePos = new Vector2(30 - Radius * stretch, 30);

					bars.Add(basePos + new Vector2(0, offset * AnimationTopRatio[0]), color, new Vector3(.25f, 0.5f, 0f));

					for (int i = 1; i < size - 1; i++)
					{
						float fixedX = MathF.Floor(size / 2);
						float x = (float)(i - fixedX) / fixedX;
						float n = MathF.Sqrt(1 - x * x);
						float px = (float)i / (size - 1) * stretch;
						float vr = progress * progress * 0.25f;

						float topOffset = AnimationTopRatio[i];
						float bottomOffset = AnimationBottomRatio[i];

						bars.Add(basePos + new Vector2(px * Radius * 2, -n * Radius + offset * topOffset), color, new Vector3(0.5f - px * vr, n * vr + .5f, 0));
						bars.Add(basePos + new Vector2(px * Radius * 2, n * Radius + offset * bottomOffset), color, new Vector3(0.5f - px * vr, -n * vr + .5f, 0));
					}
					bars.Add(basePos + new Vector2(Radius * 2 * stretch, offset * AnimationTopRatio[^1]), color, new Vector3(.75f, 0.5f, 0));

					for (int i = 0; i < bars.Count; i += 2)
					{
						strokes.Add(bars[i].position, bars[i].color, new Vector3(0.5f + progress * 0.2f, 0.5f + progress * 0.2f, 1));
					}
					for (int i = bars.Count % 2 == 0 ? bars.Count - 1 : bars.Count - 2; i >= 0; i -= 2)
					{
						strokes.Add(bars[i].position, bars[i].color, new Vector3(0.5f + progress * 0.2f, 0.5f + progress * 0.2f, 1));
					}
					strokes.Add(bars[0].position, bars[0].color, new Vector3(0.5f + progress * 0.2f, 0.5f + progress * 0.2f, 1));
				}
				else if (ProtectionCounter < ProtectionDuration + ProtectionEndDuration)
				{
					float progress = (float)(ProtectionCounter - ProtectionDuration) / ProtectionEndDuration;
					Color color = new(110, 255, 150, 255);

					int size = AnimationTopRatio.Length;

					float stretch = AnimationStretch[^1];
					float offset = ProtectionAnimation[^1];
					var basePos = new Vector2(30 - Radius * stretch, 30);
					var refPos = new Vector2(30, 60);

					Vector2 targetLeftPos = refPos + new Vector2(3, 12);
					Vector2 baseLeftPos = basePos + new Vector2(0, offset * AnimationTopRatio[0]);
					Vector2 deltaLeftPos = targetLeftPos - baseLeftPos;
					bars.Add(deltaLeftPos * progress + baseLeftPos, color, new Vector3(.25f, 0.5f, 0f));

					for (int i = 1; i < size - 1; i++)
					{
						float fixedX = MathF.Floor(size / 2);
						float x = (float)(i - fixedX) / fixedX;
						float n = MathF.Sqrt(1 - x * x);
						float px = (float)i / (size - 1) * stretch;

						float topOffset = AnimationTopRatio[i];
						float bottomOffset = AnimationBottomRatio[i];

						Vector2 targetTopPos = refPos + new Vector2(x * 3, n * 4 - 12);
						Vector2 targetBottomPos = refPos + new Vector2(x * 3, -n * 4 - 12);
						Vector2 baseTopPos = basePos + new Vector2(px * Radius * 2, -n * Radius + offset * topOffset);
						Vector2 baseBottomPos = basePos + new Vector2(px * Radius * 2, n * Radius + offset * bottomOffset);
						Vector2 deltaTopPos = targetTopPos - baseTopPos;
						Vector2 deltaBottomPos = targetBottomPos - baseBottomPos;

						bars.Add(deltaTopPos * progress + baseTopPos, color, new Vector3(px / 2 + .25f, n / 4 + .5f, 0));
						bars.Add(deltaBottomPos * progress + baseBottomPos, color, new Vector3(px / 2 + .25f, -n / 4 + .5f, 0));
					}

					Vector2 targetRightPos = refPos + new Vector2(3, -12);
					Vector2 baseRightPos = basePos + new Vector2(Radius * 2 * stretch, offset * AnimationTopRatio[^1]);
					Vector2 deltaRightPos = targetRightPos - baseRightPos;
					bars.Add(deltaRightPos * progress + baseRightPos, color, new Vector3(.75f, 0.5f, 0f));
					for (int i = 0; i < bars.Count; i += 2)
					{
						strokes.Add(bars[i].position, bars[i].color, new Vector3(0.7f, 0.7f, 1));
					}
					for (int i = bars.Count % 2 == 0 ? bars.Count - 1 : bars.Count - 2; i >= 0; i -= 2)
					{
						strokes.Add(bars[i].position, bars[i].color, new Vector3(0.7f, 0.7f, 1));
					}
					strokes.Add(bars[0].position, bars[0].color, new Vector3(0.7f, 0.7f, 1));
				}

				var gd = Main.instance.GraphicsDevice;
				var cur = Main.screenTarget;

				gd.SetRenderTarget(OriginTarget);
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone);
				Main.spriteBatch.Draw(cur, Vector2.Zero, Color.White);
				Main.spriteBatch.End();

				gd.SetRenderTarget(ShellTarget);
				gd.Clear(Color.Transparent);

				if (bars.Count > 0)
				{
					Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
					var proj = Matrix.CreateOrthographicOffCenter(0, 60, 60, 0, 0, 1);
					effect.Parameters["uTransform"].SetValue(proj);
					effect.CurrentTechnique.Passes[0].Apply();
					gd.Textures[0] = white;
					gd.SamplerStates[0] = SamplerState.PointWrap;
					gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
					gd.DrawUserPrimitives(PrimitiveType.LineStrip, strokes.ToArray(), 0, strokes.Count - 1);
					Main.spriteBatch.End();
				}

				gd.SetRenderTarget(Main.screenTarget);
				gd.Clear(Color.Transparent);
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone);
				Main.spriteBatch.Draw(OriginTarget, Vector2.Zero, Color.White);
				Main.spriteBatch.End();

				Vector2 center = CollidePosition - Main.screenPosition;
				Vector2 shellPos = center - new Vector2(30, 60);
				Main.spriteBatch.Begin(sBS);
				Main.spriteBatch.Draw(ShellTarget, new Rectangle((int)shellPos.X, (int)shellPos.Y, 60, 60), light);

			}
			else
			{
				float startProgress = (float)EffectStartCounter / EffectDuration;
				Vector2 accessoryPos = Player.Center + new Vector2(0, (int)(startProgress * 24));
				Main.spriteBatch.Draw(texture, accessoryPos - new Vector2(14, 0) - Main.screenPosition, light);
			}
		}
	}
}

using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace Everglow.Myth.Common;

public class MythContentNPCLoot : GlobalNPC
{
	// ModifyNPCLoot uses a unique system called the ItemDropDatabase, which has many different rules for many different drop use cases.
	// Here we go through all of them, and how they can be used.
	// There are tons of other examples in vanilla! In a decompiled vanilla build, GameContent/ItemDropRules/ItemDropDatabase adds item drops to every single vanilla NPC, which can be a good resource.
	// TODO: Finish Weapon Ports first
	// public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
	// {
	//    Player player = Main.LocalPlayer;
	//    if (npc.type == NPCID.Nutcracker || npc.type == 349)
	//    {
	//        /*´óÊ¦*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), ModContent.ItemType<Items.Accessories.WalnutClip>(), 180/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*×¨¼Ò*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<Items.Accessories.WalnutClip>(), 270/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*ÆÕÍ¨*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<Items.Accessories.WalnutClip>(), 360/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//    }
	//    if (npc.type == 48)
	//    {
	//        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FeatherMagic>(), 70));
	//        /*´óÊ¦*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), ModContent.ItemType<FeatherMagic>(), 60/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*×¨¼Ò*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<FeatherMagic>(), 90/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*ÆÕÍ¨*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<FeatherMagic>(), 120/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//    }
	//    if (npc.type == 125 || npc.type == 126)
	//    {
	//        /*´óÊ¦*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), ModContent.ItemType<BloodLightCyanFlame>(), 16/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*×¨¼Ò*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<BloodLightCyanFlame>(), 50/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*ÆÕÍ¨*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<BloodLightCyanFlame>(), 200/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//    }
	//    if (npc.type == 50)
	//    {
	//        /*´óÊ¦*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), ModContent.ItemType<JellySlingshot>(), 8/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*×¨¼Ò*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<JellySlingshot>(), 25/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*ÆÕÍ¨*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<JellySlingshot>(), 100/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//    }
	//    if (npc.type == 262)
	//    {
	//        /*´óÊ¦*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), ModContent.ItemType<ThornBomb>(), 8/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*×¨¼Ò*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<ThornBomb>(), 25/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*ÆÕÍ¨*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<ThornBomb>(), 100/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//    }
	//    if (npc.type == 245)
	//    {
	//        /*´óÊ¦*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), ModContent.ItemType<TrapYoyo>(), 8/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*×¨¼Ò*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<TrapYoyo>(), 25/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*ÆÕÍ¨*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<TrapYoyo>(), 100/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//    }
	//    if (npc.type == 127)
	//    {
	//        /*´óÊ¦*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), ModContent.ItemType<MachineFit>(), 8/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*×¨¼Ò*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<MachineFit>(), 25/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*ÆÕÍ¨*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<MachineFit>(), 100/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//    }
	//    if (npc.type == 127)
	//    {
	//        /*´óÊ¦*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), ModContent.ItemType<MachineSkeGun>(), 40/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*×¨¼Ò*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<MachineSkeGun>(), 125/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*ÆÕÍ¨*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<MachineSkeGun>(), 500/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//    }
	//    if (npc.type == 113)
	//    {
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), ModContent.ItemType<BloodGoldBlade>(), 40/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*×¨¼Ò*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<BloodGoldBlade>(), 125/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*ÆÕÍ¨*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<BloodGoldBlade>(), 500/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));

	// }
	//    if (npc.type == 134)
	//    {
	//        /*´óÊ¦*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), ModContent.ItemType<LaserWhip>(), 8/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*×¨¼Ò*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<LaserWhip>(), 25/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*ÆÕÍ¨*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<LaserWhip>(), 100/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//    }
	//    if (npc.type == 266)
	//    {
	//        /*´óÊ¦*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), ModContent.ItemType<ChaosCurrent>(), 8/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*×¨¼Ò*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<ChaosCurrent>(), 25/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*ÆÕÍ¨*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<ChaosCurrent>(), 100/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//    }
	//    if (npc.type == 636)
	//    {
	//        /*´óÊ¦*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), ModContent.ItemType<RainbowLight>(), 8/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*×¨¼Ò*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<RainbowLight>(), 25/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*ÆÕÍ¨*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<RainbowLight>(), 100/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//    }
	//    if (npc.type == ModContent.NPCType<NPCs.BloodTusk.BloodTusk>())
	//    {
	//        /*´óÊ¦*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), ModContent.ItemType<ToothSpear>(), 40/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*×¨¼Ò*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<ToothSpear>(), 125/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*ÆÕÍ¨*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<ToothSpear>(), 500/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//    }
	//    if (npc.type == ModContent.NPCType<NPCs.BloodTusk.BloodTusk>())
	//    {
	//        /*´óÊ¦*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), ModContent.ItemType<ToothBow>(), 8/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*×¨¼Ò*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<ToothBow>(), 25/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*ÆÕÍ¨*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<ToothBow>(), 100/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//    }
	//    if (npc.type == 657)
	//    {
	//        /*´óÊ¦*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), ModContent.ItemType<GelPowerGun>(), 8/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*×¨¼Ò*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<GelPowerGun>(), 25/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*ÆÕÍ¨*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<GelPowerGun>(), 100/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//    }
	//    if (npc.type == 39)
	//    {
	//        /*´óÊ¦*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), ModContent.ItemType<AshBone>(), 60/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*×¨¼Ò*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<AshBone>(), 90/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*ÆÕÍ¨*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<AshBone>(), 120/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//    }
	//    if (npc.type == 398)
	//    {
	//        /*´óÊ¦*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), ModContent.ItemType<SilenceMirror>(), 40/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*×¨¼Ò*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<SilenceMirror>(), 125/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*ÆÕÍ¨*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<SilenceMirror>(), 500/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//    }
	//    if (npc.type == 370)
	//    {
	//        /*´óÊ¦*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), ModContent.ItemType<BlueRain>(), 8/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*×¨¼Ò*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<BlueRain>(), 25/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*ÆÕÍ¨*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<BlueRain>(), 100/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//    }
	//    if (npc.type == 140)
	//    {
	//        /*´óÊ¦*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), ModContent.ItemType<AmbiguousNight>(), 100/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*×¨¼Ò*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<AmbiguousNight>(), 150/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*ÆÕÍ¨*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<AmbiguousNight>(), 200/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//    }
	//    if (npc.type == NPCID.WyvernHead)
	//    {
	//        /*´óÊ¦*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), ModContent.ItemType<ThunderFlower>(), 50/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*×¨¼Ò*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<ThunderFlower>(), 100/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*ÆÕÍ¨*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<ThunderFlower>(), 140/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//    }
	//    if (npc.type == 82)
	//    {
	//        /*´óÊ¦*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), ModContent.ItemType<ComingGhost>(), 100/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*×¨¼Ò*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<ComingGhost>(), 150/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*ÆÕÍ¨*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<ComingGhost>(), 200/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//    }
	//    if (npc.type == 370)
	//    {
	//        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DukeTooth>(), 3));
	//    }
	//    if (npc.type == 628)
	//    {
	//        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<WindMoveSeed>(), 1, 3, 5));
	//    }
	//    if (npc.type == 1)
	//    {
	//        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BlueGel>(), 800));
	//    }
	//    if (npc.type == -3)
	//    {
	//        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RedGel>(), 800));
	//    }
	//    if (npc.type == -1)
	//    {
	//        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GreenGel>(), 800));
	//    }
	//    if (!npc.friendly && npc.lifeMax > 100)
	//    {
	//        /*´óÊ¦*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new EclipseMasterPostPlant(), ModContent.ItemType<GoldRoundYoyo>(), 40/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*×¨¼Ò*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new EclipseExpertPostPlant(), ModContent.ItemType<GoldRoundYoyo>(), 60/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*ÆÕÍ¨*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new EclipseNormalPostPlant(), ModContent.ItemType<GoldRoundYoyo>(), 80/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//    }
	//    if (!npc.friendly && npc.lifeMax > 100)
	//    {
	//        /*´óÊ¦*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new CorruptionMasterHardmode(), ModContent.ItemType<CurseClub>(), 100/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*×¨¼Ò*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new CorruptionExpertHardmode(), ModContent.ItemType<CurseClub>(), 150/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*ÆÕÍ¨*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new CorruptionNormalHardmode(), ModContent.ItemType<CurseClub>(), 200/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//    }
	//    if (!npc.friendly && npc.lifeMax > 100)
	//    {
	//        /*´óÊ¦*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new CorruptionMasterHardmode(), ModContent.ItemType<CorruptEye>(), 160/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*×¨¼Ò*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new CorruptionExpertHardmode(), ModContent.ItemType<CorruptEye>(), 275/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*ÆÕÍ¨*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new CorruptionNormalHardmode(), ModContent.ItemType<CorruptEye>(), 450/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//    }
	//    if (!npc.friendly && npc.lifeMax > 100)
	//    {
	//        /*´óÊ¦*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new CrimsonExpertHardmode(), ModContent.ItemType<GoldLiquidPupil>(), 160/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*×¨¼Ò*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new CrimsonExpertHardmode(), ModContent.ItemType<GoldLiquidPupil>(), 275/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*ÆÕÍ¨*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new CrimsonNormalHardmode(), ModContent.ItemType<GoldLiquidPupil>(), 450/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//    }
	//    if (!npc.friendly && npc.lifeMax > 100)
	//    {
	//        /*´óÊ¦*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new CrimsonMasterHardmode(), ModContent.ItemType<BloodLiquidClub>(), 100/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*×¨¼Ò*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new CrimsonExpertHardmode(), ModContent.ItemType<BloodLiquidClub>(), 150/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*ÆÕÍ¨*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new CrimsonNormalHardmode(), ModContent.ItemType<BloodLiquidClub>(), 200/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//    }
	//    if (!npc.friendly && npc.lifeMax > 100)
	//    {
	//        /*´óÊ¦*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new HallowMasterHardmode(), ModContent.ItemType<CrystalClub>(), 100/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*×¨¼Ò*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new HallowExpertHardmode(), ModContent.ItemType<CrystalClub>(), 150/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        /*ÆÕÍ¨*/
	//        npcLoot.Add(ItemDropRule.ByCondition(new HallowNormalHardmode(), ModContent.ItemType<CrystalClub>(), 200/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//    }
	//    if (npc.type == 344)
	//    {
	//        npcLoot.Add(ItemDropRule.ByCondition(new InFrostMoonFinal(), ModContent.ItemType<FrozenStormPine>(), 50/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//        npcLoot.Add(ItemDropRule.ByCondition(new InFrostMoonFinal(), ModContent.ItemType<XmasWhip>(), 50/*¸ÅÂÊ·ÖÄ¸*/, 1/*×îÐ¡*/, 1/*×î´ó*/, 1/*¸ÅÂÊ·Ö×Ó*/));
	//    }
	// }
}
internal class CrimsonExpertHardmode : IItemDropRuleCondition
{
	// TODO:ÏÂÁÐµôÂäÌõ¼þÐèÒª·­Òë
	private bool CanD => Main.expertMode && !Main.masterMode && Main.hardMode && Main.LocalPlayer.ZoneCrimson;
	public bool CanDrop(DropAttemptInfo info)
	{
		return CanD;
	}
	public bool CanShowItemDropInUI()
	{
		return CanD;
	}
	public string GetConditionDescription()
	{
		string desc = "Only hardmode and in the Crimson";
		if (Language.ActiveCulture.Name == "zh-Hans")
		{
			desc = "½öÔÚÀ§ÄÑÄ£Ê½µÄÐÉºìÖ®µØ";
		}

		return desc;
	}
}
internal class CrimsonMasterHardmode : IItemDropRuleCondition
{
	private bool CanD => Main.masterMode && Main.hardMode && Main.LocalPlayer.ZoneCrimson;
	public bool CanDrop(DropAttemptInfo info)
	{
		return CanD;
	}
	public bool CanShowItemDropInUI()
	{
		return CanD;
	}
	public string GetConditionDescription()
	{
		string desc = "Only hardmode and in the  Crimson";
		if (Language.ActiveCulture.Name == "zh-Hans")
		{
			desc = "½öÔÚÀ§ÄÑÄ£Ê½µÄÐÉºìÖ®µØ";
		}

		return desc;
	}
}
internal class CrimsonNormalHardmode : IItemDropRuleCondition
{
	private bool CanD => !Main.expertMode && Main.hardMode && Main.LocalPlayer.ZoneCrimson;
	public bool CanDrop(DropAttemptInfo info)
	{
		return CanD;
	}
	public bool CanShowItemDropInUI()
	{
		return CanD;
	}
	public string GetConditionDescription()
	{
		string desc = "Only hardmode and in the  Crimson";
		if (Language.ActiveCulture.Name == "zh-Hans")
		{
			desc = "½öÔÚÀ§ÄÑÄ£Ê½µÄÐÉºìÖ®µØ";
		}

		return desc;
	}
}
internal class CorruptionExpertHardmode : IItemDropRuleCondition
{
	private bool CanD => Main.expertMode && !Main.masterMode && Main.hardMode && Main.LocalPlayer.ZoneCorrupt;
	public bool CanDrop(DropAttemptInfo info)
	{
		return CanD;
	}
	public bool CanShowItemDropInUI()
	{
		return CanD;
	}
	public string GetConditionDescription()
	{
		string desc = "Only hardmode and in the Corrupt";
		if (Language.ActiveCulture.Name == "zh-Hans")
		{
			desc = "½öÔÚÀ§ÄÑÄ£Ê½µÄ¸¯»¯Ö®µØ";
		}

		return desc;
	}
}
internal class CorruptionMasterHardmode : IItemDropRuleCondition
{
	private bool CanD => Main.masterMode && Main.hardMode && Main.LocalPlayer.ZoneCorrupt;
	public bool CanDrop(DropAttemptInfo info)
	{
		return CanD;
	}
	public bool CanShowItemDropInUI()
	{
		return CanD;
	}
	public string GetConditionDescription()
	{
		string desc = "Only hardmode and in the Corrupt";
		if (Language.ActiveCulture.Name == "zh-Hans")
		{
			desc = "½öÔÚÀ§ÄÑÄ£Ê½µÄ¸¯»¯Ö®µØ";
		}

		return desc;
	}
}
internal class CorruptionNormalHardmode : IItemDropRuleCondition
{
	private bool CanD => !Main.expertMode && Main.hardMode && Main.LocalPlayer.ZoneCorrupt;
	public bool CanDrop(DropAttemptInfo info)
	{
		return CanD;
	}
	public bool CanShowItemDropInUI()
	{
		return CanD;
	}
	public string GetConditionDescription()
	{
		string desc = "Only hardmode and in the Corrupt";
		if (Language.ActiveCulture.Name == "zh-Hans")
		{
			desc = "½öÔÚÀ§ÄÑÄ£Ê½µÄ¸¯»¯Ö®µØ";
		}

		return desc;
	}
}
internal class HallowExpertHardmode : IItemDropRuleCondition
{
	private bool CanD => Main.expertMode && !Main.masterMode && Main.hardMode && Main.LocalPlayer.ZoneHallow;
	public bool CanDrop(DropAttemptInfo info)
	{
		return CanD;
	}
	public bool CanShowItemDropInUI()
	{
		return CanD;
	}
	public string GetConditionDescription()
	{
		string desc = "Only hardmode and in the Hallow";
		if (Language.ActiveCulture.Name == "zh-Hans")
		{
			desc = "½öÔÚÀ§ÄÑÄ£Ê½µÄÉñÊ¥Ö®µØ";
		}

		return desc;
	}
}
internal class HallowMasterHardmode : IItemDropRuleCondition
{
	private bool CanD => Main.masterMode && Main.hardMode && Main.LocalPlayer.ZoneHallow;
	public bool CanDrop(DropAttemptInfo info)
	{
		return CanD;
	}
	public bool CanShowItemDropInUI()
	{
		return CanD;
	}
	public string GetConditionDescription()
	{
		string desc = "Only hardmode and in the Hallow";
		if (Language.ActiveCulture.Name == "zh-Hans")
		{
			desc = "½öÔÚÀ§ÄÑÄ£Ê½µÄÉñÊ¥Ö®µØ";
		}

		return desc;
	}
}
internal class HallowNormalHardmode : IItemDropRuleCondition
{
	private bool CanD => !Main.expertMode && Main.hardMode && Main.LocalPlayer.ZoneHallow;
	public bool CanDrop(DropAttemptInfo info)
	{
		return CanD;
	}
	public bool CanShowItemDropInUI()
	{
		return CanD;
	}
	public string GetConditionDescription()
	{
		string desc = "Only hardmode and in the Hallow";
		if (Language.ActiveCulture.Name == "zh-Hans")
		{
			desc = "½öÔÚÀ§ÄÑÄ£Ê½µÄÉñÊ¥Ö®µØ";
		}

		return desc;
	}
}
internal class EclipseExpertPostPlant : IItemDropRuleCondition
{
	private bool CanD => Main.expertMode && !Main.masterMode && Main.eclipse && NPC.downedPlantBoss;
	public bool CanDrop(DropAttemptInfo info)
	{
		return CanD;
	}
	public bool CanShowItemDropInUI()
	{
		return CanD;
	}
	public string GetConditionDescription()
	{
		string desc = "Only defeated Plantera and in the Eclipse";
		if (Language.ActiveCulture.Name == "zh-Hans")
		{
			desc = "½öÔÚÊÀ¼ÍÖ®»¨ºóÈÕÊ³";
		}

		return desc;
	}
}
internal class EclipseMasterPostPlant : IItemDropRuleCondition
{
	private bool CanD => Main.masterMode && Main.eclipse && NPC.downedPlantBoss;
	public bool CanDrop(DropAttemptInfo info)
	{
		return CanD;
	}
	public bool CanShowItemDropInUI()
	{
		return CanD;
	}
	public string GetConditionDescription()
	{
		string desc = "Only defeated Plantera and in the Eclipse";
		if (Language.ActiveCulture.Name == "zh-Hans")
		{
			desc = "½öÔÚÊÀ¼ÍÖ®»¨ºóÈÕÊ³";
		}

		return desc;
	}
}
internal class EclipseNormalPostPlant : IItemDropRuleCondition
{
	private bool CanD => !Main.expertMode && Main.eclipse && NPC.downedPlantBoss;
	public bool CanDrop(DropAttemptInfo info)
	{
		return CanD;
	}
	public bool CanShowItemDropInUI()
	{
		return CanD;
	}
	public string GetConditionDescription()
	{
		string desc = "Only defeated Plantera and in the Eclipse";
		if (Language.ActiveCulture.Name == "zh-Hans")
		{
			desc = "½öÔÚÊÀ¼ÍÖ®»¨ºóÈÕÊ³";
		}

		return desc;
	}
}
internal class InFrostMoonFinal : IItemDropRuleCondition
{
	private bool CanD => Main.snowMoon && Main.invasionProgressWave >= 20;
	public bool CanDrop(DropAttemptInfo info)
	{
		return CanD;
	}
	public bool CanShowItemDropInUI()
	{
		return CanD;
	}
	public string GetConditionDescription()
	{
		string desc = "Only Frost Moon during Chrismas";
		if (Language.ActiveCulture.Name == "zh-Hans")
		{
			desc = "½öÔÚÊ¥µ®½ÚÆÚ¼äËªÔÂ";
		}

		return desc;
	}
}
internal class InPumpkMoonFinal : IItemDropRuleCondition
{
	private bool CanD => Main.pumpkinMoon && Main.invasionProgressWave >= 20;
	public bool CanDrop(DropAttemptInfo info)
	{
		return CanD;
	}
	public bool CanShowItemDropInUI()
	{
		return CanD;
	}
	public string GetConditionDescription()
	{
		string desc = "Only Pumpkin Moon during Chrismas";
		if (Language.ActiveCulture.Name == "zh-Hans")
		{
			desc = "½öÔÚÊ¥µ®½ÚÆÚ¼äËªÔÂ";
		}

		return desc;
	}
}

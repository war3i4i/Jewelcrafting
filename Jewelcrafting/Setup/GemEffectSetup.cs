using ItemManager;
using Jewelcrafting.GemEffects;
using Jewelcrafting.GemEffects.Groups;
using UnityEngine;

namespace Jewelcrafting;

public static class GemEffectSetup
{
	public static GameObject swordFall = null!;
	public static StatusEffect gliding = null!;
	public static StatusEffect glidingDark = null!;
	public static SE_Stats glowingSpirit = null!;
	public static GameObject glowingSpiritPrefab = null!;
	public static SE_Stats lightningSpeed = null!;
	public static SE_Stats rootedRevenge = null!;
	public static SE_Stats poisonousDrain = null!;
	public static GameObject poisonousDrainCloud = null!;
	public static SE_Stats icyProtection = null!;
	public static SE_Stats fieryDoom = null!;
	public static GameObject fieryDoomExplosion = null!;
	public static SE_Stats apotheosis = null!;
	public static SE_Stats awareness = null!;
	public static GameObject heardIcon = null!;
	public static GameObject attackedIcon = null!;
	public static SE_Stats warmth = null!;
	public static SE_Stats headhunter = null!;
	public static SE_Stats rigidFinger = null!;
	public static GameObject magicRepair = null!;
	public static SE_Stats aquatic = null!;
	public static SE_Stats lightningStart = null!;
	public static SE_Stats rootStart = null!;
	public static SE_Stats poisonStart = null!;
	public static SE_Stats iceStart = null!;
	public static SE_Stats fireStart = null!;
	public static SE_Stats apotheosisStart = null!;
	public static SE_Stats friendshipStart = null!;
	public static SE_Stats friendship = null!;
	public static SE_Stats loneliness = null!;
	public static GameObject friendshipTether = null!;
	public static SE_Stats cowardice = null!;
	public static SE_Stats fireBossDebuff = null!;
	public static SE_Stats frostBossDebuff = null!;
	public static SE_Stats poisonBossDebuff = null!;
	public static SE_Stats thunderclapMark = null!;
	public static GameObject thunderclapExplosion = null!;
	public static SE_Stats fading = null!;
	public static Material fadingMaterial = null!;
	public static GameObject fusingFailSound = null!;
	public static GameObject fusingSuccessSound = null!;

	public static void initializeGemEffect(AssetBundle assets)
	{
		swordFall = PrefabManager.RegisterPrefab(assets, "JC_Buff_FX_9");
		gliding = assets.LoadAsset<SE_Stats>("JCGliding");
		glidingDark = assets.LoadAsset<SE_Stats>("SE_DarkWings");
		glowingSpirit = assets.LoadAsset<SE_Stats>("SE_Crystal_Magelight");
		glowingSpiritPrefab = PrefabManager.RegisterPrefab(assets, "JC_Crystal_Magelight");
		glowingSpiritPrefab.AddComponent<GlowingSpirit.OrbDestroy>();
		lightningSpeed = Utils.ConvertStatusEffect<LightningSpeed.LightningSpeedEffect>(assets.LoadAsset<SE_Stats>("JC_Electric_Wings_SE"));
		apotheosis = Utils.ConvertStatusEffect<Apotheosis.ApotheosisEffect>(assets.LoadAsset<SE_Stats>("SE_Apotheosis"));
		rootedRevenge = assets.LoadAsset<SE_Stats>("SE_Boss_3");
		poisonousDrain = assets.LoadAsset<SE_Stats>("SE_Boss_2");
		poisonousDrainCloud = PrefabManager.RegisterPrefab(assets, "JC_Buff_FX_2");
		icyProtection = assets.LoadAsset<SE_Stats>("SE_Boss_4");
		fieryDoom = assets.LoadAsset<SE_Stats>("SE_Boss_5");
		fieryDoomExplosion = PrefabManager.RegisterPrefab(assets, "JC_Buff_FX_3");
		awareness = assets.LoadAsset<SE_Stats>("JC_SE_Necklace_Red");
		heardIcon = assets.LoadAsset<GameObject>("JC_Eyeball_Obj");
		attackedIcon = assets.LoadAsset<GameObject>("JC_Alert_Obj");
		warmth = assets.LoadAsset<SE_Stats>("JC_Se_Ring_Red");
		headhunter = assets.LoadAsset<SE_Stats>("JC_Se_Ring_Green");
		headhunter.m_modifyAttackSkill = Skills.SkillType.All;
		assets.LoadAsset<SE_Stats>("JC_SE_Necklace_Yellow");
		rigidFinger = assets.LoadAsset<SE_Stats>("JC_Se_Ring_Purple");
		rigidFinger.m_modifyAttackSkill = Skills.SkillType.All;
		magicRepair = PrefabManager.RegisterPrefab(assets, "VFX_Buff_Green");
		aquatic = assets.LoadAsset<SE_Stats>("JC_Se_Necklace_Blue");
		aquatic.m_modifyAttackSkill = Skills.SkillType.All;
		lightningStart = assets.LoadAsset<SE_Stats>("SE_VFX_Start_Purple");
		rootStart = assets.LoadAsset<SE_Stats>("SE_VFX_Start_Brown");
		poisonStart = assets.LoadAsset<SE_Stats>("SE_VFX_Start_Green");
		iceStart = assets.LoadAsset<SE_Stats>("SE_VFX_Start_Blue");
		fireStart = assets.LoadAsset<SE_Stats>("SE_VFX_Start_Red");
		apotheosisStart = assets.LoadAsset<SE_Stats>("SE_VFX_Start_Black");
		friendshipStart = assets.LoadAsset<SE_Stats>("SE_VFX_Start_Purple");
		friendship = Utils.ConvertStatusEffect<TogetherForever.TogetherForeverEffect>(assets.LoadAsset<SE_Stats>("SE_Friendship_Group"));
		loneliness = Utils.ConvertStatusEffect<TogetherForever.LonelinessEffect>(assets.LoadAsset<SE_Stats>("SE_Loneliness_Group"));
		friendshipTether = assets.LoadAsset<GameObject>("VFX_FriendLine_Render");
		friendshipTether.AddComponent<FriendshipTether>();
		cowardice = assets.LoadAsset<SE_Stats>("SE_Cowardice");
		fireBossDebuff = assets.LoadAsset<SE_Stats>("SE_Boss_Fire");
		frostBossDebuff = assets.LoadAsset<SE_Stats>("SE_Boss_Frost");
		poisonBossDebuff = assets.LoadAsset<SE_Stats>("SE_Boss_Poison");
		thunderclapMark = assets.LoadAsset<SE_Stats>("SE_ThunderClap_Marked");
		thunderclapExplosion = assets.LoadAsset<GameObject>("JC_Marked_Explode");
		fading = Utils.ConvertStatusEffect<Fade.FadeSE>(assets.LoadAsset<SE_Stats>("SE_VFX_Fade"));
		fadingMaterial = assets.LoadAsset<Material>("JC_Player_Distortion");
		fusingFailSound = assets.LoadAsset<GameObject>("sfx_crystal_destroyed");
		Object.Destroy(fusingFailSound.GetComponent<ZNetView>());
		fusingSuccessSound = assets.LoadAsset<GameObject>("sfx_crystal_fuse");
		Object.Destroy(fusingSuccessSound.GetComponent<ZNetView>());
	}
}

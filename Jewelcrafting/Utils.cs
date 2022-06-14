﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using ExtendedItemDataFramework;
using HarmonyLib;
using Jewelcrafting.GemEffects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Jewelcrafting;

public static class Utils
{
	private static readonly MethodInfo MemberwiseCloneMethod = AccessTools.DeclaredMethod(typeof(object), "MemberwiseClone");
	public static T Clone<T>(T input) where T : notnull => (T)MemberwiseCloneMethod.Invoke(input, Array.Empty<object>());

	public static bool IsSocketableItem(ItemDrop.ItemData.SharedData item)
	{
		return item.m_itemType is
			       ItemDrop.ItemData.ItemType.Bow or
			       ItemDrop.ItemData.ItemType.Chest or
			       ItemDrop.ItemData.ItemType.Hands or
			       ItemDrop.ItemData.ItemType.Helmet or
			       ItemDrop.ItemData.ItemType.Legs or
			       ItemDrop.ItemData.ItemType.Shield or
			       ItemDrop.ItemData.ItemType.Shoulder or
			       ItemDrop.ItemData.ItemType.Utility or
			       ItemDrop.ItemData.ItemType.TwoHandedWeapon ||
		       (item.m_itemType is ItemDrop.ItemData.ItemType.OneHandedWeapon && !item.m_attack.m_consumeItem);
	}

	public static void ApplyToAllPlayerItems(Player player, Action<ItemDrop.ItemData?> callback)
	{
		callback(player.m_rightItem);
		callback(player.m_leftItem);
		callback(player.m_chestItem);
		callback(player.m_legItem);
		callback(player.m_ammoItem);
		callback(player.m_helmetItem);
		callback(player.m_shoulderItem);
		callback(player.m_utilityItem);
	}

	public static string ZDOName(this Effect effect) => "Jewelcrafting Socket " + effect;
	public static float GetEffect(this Player player, Effect effect) => player.GetEffect<DefaultPower>(effect).Power;

	public static T GetEffect<T>(this Player player, Effect effect) where T : struct
	{
		if (player.m_nview.m_zdo?.GetByteArray(effect.ZDOName()) is not { } effectBytes)
		{
			return default;
		}

		T output;
		unsafe
		{
			fixed (void* source = &effectBytes[0])
			{
				output = Marshal.PtrToStructure<T>((IntPtr)source);
			}
		}

		return output;
	}

	public static WaitForSeconds WaitEffect<T>(this Player player, Effect effect, Func<T, float> minWait, Func<T, float> maxWait) where T : struct
	{
		T config = player.GetEffect<T>(effect);
		float minSeconds = minWait(config);
		float maxSeconds = maxWait(config);
		if (minSeconds > 0)
		{
			return new WaitForSeconds(Random.Range(minSeconds, maxSeconds));
		}

		float wait = 0;
		if (Jewelcrafting.SocketEffects.TryGetValue(effect, out List<EffectDef> defs))
		{
			wait = defs.Min(def => minWait((T)def.Power[0]));
		}
		return new WaitForSeconds(Mathf.Max(wait, 1));
	}

	public static GemLocation GetGemLocation(ItemDrop.ItemData.SharedData item) => item.m_itemType switch
	{
		ItemDrop.ItemData.ItemType.Helmet => GemLocation.Head,
		ItemDrop.ItemData.ItemType.Chest => GemLocation.Chest,
		ItemDrop.ItemData.ItemType.Legs => GemLocation.Legs,
		ItemDrop.ItemData.ItemType.Utility => GemLocation.Utility,
		ItemDrop.ItemData.ItemType.Shoulder => GemLocation.Cloak,
		ItemDrop.ItemData.ItemType.Tool => GemLocation.Tool,
		_ => item.m_skillType switch
		{
			Skills.SkillType.Swords => GemLocation.Sword,
			Skills.SkillType.Knives => GemLocation.Knife,
			Skills.SkillType.Clubs => GemLocation.Club,
			Skills.SkillType.Polearms => GemLocation.Polearm,
			Skills.SkillType.Spears => GemLocation.Spear,
			Skills.SkillType.Blocking => GemLocation.Shield,
			Skills.SkillType.Axes => GemLocation.Axe,
			Skills.SkillType.Bows => GemLocation.Bow,
			Skills.SkillType.Pickaxes => GemLocation.Tool,
			Skills.SkillType.Unarmed when item.m_itemType == ItemDrop.ItemData.ItemType.TwoHandedWeapon => GemLocation.Knife,
			_ => throw new ArgumentOutOfRangeException()
		}
	};

	public static byte[] ReadEmbeddedFileBytes(string name)
	{
		using MemoryStream stream = new();
		Assembly.GetExecutingAssembly().GetManifestResourceStream("Jewelcrafting." + name)?.CopyTo(stream);
		return stream.ToArray();
	}

	[HarmonyPatch(typeof(InventoryGui), nameof(InventoryGui.DoCrafting))]
	private static class TransferEIDFComponentsOnUpgrade
	{
		private static ItemDrop.ItemData BackupOldComponents(ItemDrop.ItemData item, List<BaseExtendedItemComponent> components)
		{
			if (item.Extended() is { } extended)
			{
				components.AddRange(extended.Components);
				extended.Components.Clear();
			}
			return item;
		}

		private static ItemDrop.ItemData? AddPreviousComponents(ItemDrop.ItemData? item, List<BaseExtendedItemComponent> components)
		{
			if (item?.Extended() is { } extended)
			{
				extended.Components.AddRange(components);
			}
			return item;
		}

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilg)
		{
			LocalBuilder localVar = ilg.DeclareLocal(typeof(List<BaseExtendedItemComponent>));
			yield return new CodeInstruction(OpCodes.Newobj, localVar.LocalType!.GetConstructor(Array.Empty<Type>()));
			yield return new CodeInstruction(OpCodes.Stloc, localVar.LocalIndex);

			MethodInfo itemDeleter = AccessTools.DeclaredMethod(typeof(Inventory), nameof(Inventory.RemoveItem), new[] { typeof(ItemDrop.ItemData) });
			foreach (CodeInstruction instruction in instructions)
			{
				if (instruction.opcode == OpCodes.Callvirt && instruction.OperandIs(itemDeleter))
				{
					yield return new CodeInstruction(OpCodes.Ldloc, localVar.LocalIndex);
					yield return new CodeInstruction(OpCodes.Call, AccessTools.DeclaredMethod(typeof(TransferEIDFComponentsOnUpgrade), nameof(BackupOldComponents)));
				}
				yield return instruction;
				// handle mods replacing the method, recognize it by retval
				if ((instruction.opcode == OpCodes.Call || instruction.opcode == OpCodes.Callvirt) && instruction.operand is MethodInfo method && method.ReturnType == typeof(ItemDrop.ItemData))
				{
					yield return new CodeInstruction(OpCodes.Ldloc, localVar.LocalIndex);
					yield return new CodeInstruction(OpCodes.Call, AccessTools.DeclaredMethod(typeof(TransferEIDFComponentsOnUpgrade), nameof(AddPreviousComponents)));
				}
			}
		}
	}

	public static bool isAdmin(ZRpc? rpc)
	{
		return rpc is null || ZNet.instance.m_adminList.Contains(rpc.GetSocket().GetHostName());
	}
}
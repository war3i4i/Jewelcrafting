﻿using System.Collections;
using System.Runtime.InteropServices;
using HarmonyLib;

namespace Jewelcrafting.GemEffects;

public static class LightningSpeed
{
	static LightningSpeed()
	{
		EffectDef.ConfigTypes.Add(Effect.Lightningspeed, typeof(Config));
		ApplyAttackSpeed.Modifiers.Add(player => player.m_seman.HaveStatusEffect(Jewelcrafting.lightningSpeed.name) ? player.GetEffect<Config>(Effect.Lightningspeed).AttackSpeed / 100f : 0);
	}
	
	[StructLayout(LayoutKind.Sequential)]
	private struct Config
	{
		public readonly float MovementSpeed;
		public readonly float MinCooldown;
		public readonly float MaxCooldown;
		public readonly float Duration;
		public readonly float AttackSpeed;
		public readonly float Stamina;
	}
	
	[HarmonyPatch(typeof(Player), nameof(Player.SetLocalPlayer))]
	private class StartCoroutineForEffect
	{
		private static void Postfix(Player __instance)
		{
			__instance.StartCoroutine(BurstOfSpeed(__instance));
		}
	}

	private static IEnumerator BurstOfSpeed(Player player)
	{
		while (true)
		{
			yield return player.WaitEffect<Config>(Effect.Lightningspeed, c => c.MinCooldown, c => c.MaxCooldown);
			Config config = player.GetEffect<Config>(Effect.Lightningspeed);
			if (config.Duration > 0)
			{
				Jewelcrafting.lightningSpeed.m_ttl = config.Duration;
				if (player.m_seman.AddStatusEffect(Jewelcrafting.lightningSpeed) is SE_Stats statusEffect)
				{
					statusEffect.m_speedModifier = 1 + config.MovementSpeed / 100f;
				}
			}
		}
		// ReSharper disable once IteratorNeverReturns
	}

	[HarmonyPatch(typeof(Player), nameof(Player.RPC_UseStamina))]
	private class ReduceStaminaUsage
	{
		private static void Prefix(Player __instance, ref float v)
		{
			if (__instance.m_seman.HaveStatusEffect(Jewelcrafting.lightningSpeed.name))
			{
				v *= __instance.GetEffect<Config>(Effect.Lightningspeed).Stamina / 100f;
			}
		}
	}
}
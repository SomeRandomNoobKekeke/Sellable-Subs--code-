using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;

namespace SellableSubs
{
  public partial class Plugin : IAssemblyPlugin
  {
    public static void GameSession_TryPurchaseSubmarine_Postfix(SubmarineInfo newSubmarine, ref bool __result)
    {
      if (!__result) return;

      if (isCurSub("tosell")) markCurSubAs("sold");
    }

    public static void clearStates()
    {
      markCurSubAs("tosell", false);

      Mod.mainSubStateCache.Clear();
      Mod.totalRepairCost = 0;

      info($"round start, tosell = " + isCurSub("tosell").ToString() + " sold = " + isCurSub("sold").ToString());

#if CLIENT
      Mod.screens?.Clear();
      Mod.mixins?.Clear();
      
      if (GameMain.GameSession?.Campaign?.CampaignUI?.submarineSelection != null)
      {
        GameMain.GameSession.Campaign.CampaignUI.submarineSelection = null;
      }
#endif
    }
  }
}
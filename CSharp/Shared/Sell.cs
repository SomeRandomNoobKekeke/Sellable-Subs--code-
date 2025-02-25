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
    public Dictionary<string, bool> mainSubStateCache = new Dictionary<string, bool>();
    public int totalRepairCost = 0;

    public static void updateRepairCost()
    {
      if (!(GameMain.GameSession?.GameMode is CampaignMode campaign)) { return; }

      Location location = campaign.Map.CurrentLocation;

      int hullRepairCost = CampaignMode.GetHullRepairCost();
      int itemRepairCost = CampaignMode.GetItemRepairCost();
      //int shuttleRetrieveCost = CampaignMode.ShuttleReplaceCost;

      hullRepairCost = location.GetAdjustedMechanicalCost(hullRepairCost);
      itemRepairCost = location.GetAdjustedMechanicalCost(itemRepairCost);
      //shuttleRetrieveCost = location.GetAdjustedMechanicalCost(shuttleRetrieveCost);

      Mod.totalRepairCost = hullRepairCost + itemRepairCost; //+ shuttleRetrieveCost;
    }

    public static bool markCurSubAs(string mark, bool state = true)
    {
      Mod.mainSubStateCache ??= new Dictionary<string, bool>();
      if (Submarine.MainSub == null) return false;

      foreach (var i in Submarine.MainSub.GetItems(false))
      {
        if (i.HasTag("dock"))
        {
          if (state) i.AddTag(mark);
          else i.RemoveTag(mark);
        }
      }

      Mod.mainSubStateCache[mark] = state;
      return state;
    }

    public static bool isCurSub(string mark)
    {
      Mod.mainSubStateCache ??= new Dictionary<string, bool>();
      if (Submarine.MainSub == null) return false;

      if (!Mod.mainSubStateCache.ContainsKey(mark))
      {
        Mod.mainSubStateCache[mark] = Submarine.MainSub.GetItems(false).Any(i => i.HasTag("dock") && i.HasTag(mark));
      }
      return Mod.mainSubStateCache[mark];
    }

    // this is selling of owned not selected sub
    public static void sellOwnedSub(SubmarineInfo sub)
    {
      if (!(GameMain.GameSession?.GameMode is CampaignMode campaign)) { return; }
      if (sub == null) return;

      int price = sub.GetPrice();
      Wallet wallet = campaign.Bank;
      wallet.Give(price);

      int i = GameMain.GameSession.OwnedSubmarines.FindIndex(s => s.Name == sub.Name);
      if (i != -1) GameMain.GameSession.OwnedSubmarines.RemoveAt(i);
    }
  }
}
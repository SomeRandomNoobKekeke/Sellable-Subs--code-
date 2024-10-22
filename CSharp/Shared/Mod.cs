using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;
using System.IO;

[assembly: IgnoresAccessChecksTo("Barotrauma")]
[assembly: IgnoresAccessChecksTo("DedicatedServer")]
[assembly: IgnoresAccessChecksTo("BarotraumaCore")]

namespace SellableSubs
{
  public partial class Plugin : IAssemblyPlugin
  {
    public Harmony harmony;

    public static string ModName = "Sellable Subs";
    public static string ModDir = "";
    public static float sellMult = 0.9f;

    public bool showAllSubs = false;
    public bool debug = false;

    public static Plugin Mod;

    public void Initialize()
    {
      Mod = this;

      harmony = new Harmony("sellable.subs");

      findModFolder();
      if (ModDir.Contains("LocalMods"))
      {
        debug = true;
        info($"found {ModName} in LocalMods, debug: {debug}");
      }


      patchShared();

#if CLIENT
      InitializeClient();
#elif SERVER
      InitializeServer();
#endif

      info("Compiled");
    }

    public void patchShared()
    {
      harmony.Patch(
        original: typeof(SubmarineInfo).GetMethod("GetPrice", AccessTools.all),
        postfix: new HarmonyMethod(typeof(Plugin).GetMethod("substractMainSubPrice"))
      );

      harmony.Patch(
        original: typeof(CampaignMode).GetMethod("SwitchSubs", AccessTools.all),
        postfix: new HarmonyMethod(typeof(Plugin).GetMethod("CampaignMode_SwitchSubs_Postfix"))
      );

      harmony.Patch(
        original: typeof(GameSession).GetMethod("TryPurchaseSubmarine", AccessTools.all),
        postfix: new HarmonyMethod(typeof(Plugin).GetMethod("GameSession_TryPurchaseSubmarine_Postfix"))
      );

      harmony.Patch(
        original: typeof(GameSession).GetMethod("StartRound", AccessTools.all, new Type[]{
          typeof(LevelData),
          typeof(bool),
          typeof(SubmarineInfo),
          typeof(SubmarineInfo),
        }),
        postfix: new HarmonyMethod(typeof(Plugin).GetMethod("clearStates"))
      );
    }

    public void findModFolder()
    {
      bool found = false;

      foreach (ContentPackage p in ContentPackageManager.EnabledPackages.All)
      {
        if (p.Name.Contains(ModName))
        {
          found = true;
          ModDir = Path.GetFullPath(p.Dir);
          break;
        }
      }

      if (!found) err($"Couldn't find {ModName} mod folder");
    }

    public void OnLoadCompleted() { }
    public void PreInitPatching() { }

    public void Dispose()
    {
#if CLIENT
      DisposeClient();
#endif
    }
  }
}

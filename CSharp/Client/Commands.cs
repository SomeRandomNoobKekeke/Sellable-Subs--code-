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
    public List<DebugConsole.Command> addedCommands = new List<DebugConsole.Command>();

    public void addCommands()
    {
      addedCommands ??= new List<DebugConsole.Command>();

      addedCommands.Add(new DebugConsole.Command("debug_sellablesubs", "", (string[] args) =>
      {
        debug = !debug;
        log($"SellableSubs.debug = {debug}");
      }));


      addedCommands.ForEach(c => DebugConsole.Commands.Add(c));
    }

    public void removeCommands()
    {
      addedCommands.ForEach((c => DebugConsole.Commands.RemoveAll(which => which.Names.Contains(c.Names[0]))));

      addedCommands.Clear();
    }

    public static void permitCommands(Identifier command, ref bool __result)
    {
      if (command.Value == "debug_sellablesubs") __result = true;
    }
  }
}
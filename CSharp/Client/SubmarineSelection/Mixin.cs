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
    public class SubmarineSelectionMixin
    {
      public GUIButton sellButton;
      public GUITickBox sellCurrentTickBox;
      public GUILayoutGroup bottomContainer;
    }

    public Dictionary<SubmarineSelection, SubmarineSelectionMixin> mixins = new Dictionary<SubmarineSelection, SubmarineSelectionMixin>();

  }
}
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
    public static void SyncState(SubmarineSelection _)
    {
      try
      {
        bool owned = false;

        if (_.selectedSubmarine != null) owned = GameMain.GameSession.IsSubmarineOwned(_.selectedSubmarine);

        bool isTargetSub = false;
        if (_.selectedSubmarine != null && SubmarineSelection.CurrentOrPendingSubmarine() != null)
        {
          isTargetSub = _.selectedSubmarine.Name == SubmarineSelection.CurrentOrPendingSubmarine().Name;
        }

        // SubmarineInfo currentSub = SubmarineSelection.CurrentOrPendingSubmarine();

        if (Mod.mixins.ContainsKey(_))
        {
          Mod.mixins[_].sellCurrentTickBox.hideIf(isCurSub("sold") || owned, 0.15f);
          Mod.mixins[_].sellCurrentTickBox.Text = TextManager.Get("campaignstoretab.sell") + " " + (Submarine.MainSub.Info.DisplayName);

          Mod.mixins[_].sellCurrentTickBox.RectTransform.Resize(
            new Point(
              (int)Mod.mixins[_].sellCurrentTickBox.ContentWidth,
              Mod.mixins[_].sellCurrentTickBox.Rect.Height
            )
          );

          Mod.mixins[_].sellButton.revealIf(owned && !(_.IsSelectedSubCurrentSub || isTargetSub), 0.15f);
        }

        if (_.purchaseService)
        {
          _.transferItemsTickBox.hideIf(_.IsSelectedSubCurrentSub, 0.2f);
          _.confirmButtonAlt.hideIf(owned, 0.1f);
          _.confirmButton.hideIf(_.IsSelectedSubCurrentSub && isCurSub("sold"), 0.25f);
        }

        if (_.transferService)
        {
          _.transferItemsTickBox.hideIf(_.IsSelectedSubCurrentSub, 0.2f);
          _.confirmButton.hideIf(_.IsSelectedSubCurrentSub && isCurSub("sold"), 0.25f);
        }


        if (_.selectedSubmarine != null)
        {
          if (_.IsSelectedSubCurrentSub)
          {
            _.TransferItemsOnSwitch = false;
            _.transferItemsTickBox.hide();
            _.itemTransferInfoBlock.revealIf(_.confirmButton.Enabled && !isCurSub("sold"), 0.6f);
            _.itemTransferInfoBlock.Text = TextManager.Get("switchingbacktocurrentsub");
          }
          else if (GameMain.GameSession?.Campaign?.PendingSubmarineSwitch?.Name == _.selectedSubmarine.Name)
          {
            _.transferItemsTickBox.hide();
            _.itemTransferInfoBlock.reveal(0.6f);
            _.itemTransferInfoBlock.Text = GameMain.GameSession.Campaign.TransferItemsOnSubSwitch ? TextManager.Get("itemtransferenabledreminder") : TextManager.Get("itemtransferdisabledreminder");
          }
          else
          {
            _.transferItemsTickBox.Selected = true; //_.TransferItemsOnSwitch;
            _.transferItemsTickBox.reveal(0.2f);
            _.itemTransferInfoBlock.hide();
          }
        }
        else
        {
          _.transferItemsTickBox.hide();
          _.itemTransferInfoBlock.hide();
        }

        if (Mod.mixins.ContainsKey(_))
        {
          Mod.mixins[_].bottomContainer?.recalc();
        }

      }
      catch (Exception e) { err(e); }
    }
  }
}
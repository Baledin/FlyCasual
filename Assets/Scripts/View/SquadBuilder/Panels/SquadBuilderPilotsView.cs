﻿using Editions;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SquadBuilderNS
{
    public class SquadBuilderPilotsView
    {
        private float FromLeft = 0;

        public void ShowAvailablePilots(Faction faction, string shipName, Boolean isCampaign = false)
        {
            PilotPanelSquadBuilder.WaitingToLoad = 0;

            ShipRecord shipRecord = Global.SquadBuilder.Database.AllShips.Find(n => n.ShipName == shipName);

            List<PilotRecord> AllPilotsFiltered = new List<PilotRecord>();

            if (isCampaign)
            {
                AllPilotsFiltered = Global.SquadBuilder.Database.AllPilots
                .Where(n =>
                    n.Ship == shipRecord
                    && n.PilotFaction == faction
                    && n.Instance.GetType().ToString().Contains(Edition.Current.NameShort)
                    && n.PilotName.ToString().Contains("Hotac")
                    && !n.Instance.IsHiddenSquadbuilderOnly
                )
                .OrderByDescending(n => n.PilotSkill).
                OrderByDescending(n => n.Instance.PilotInfo.Cost).
                ToList();
            }
            else
            {
                AllPilotsFiltered = Global.SquadBuilder.Database.AllPilots
                .Where(n =>
                    n.Ship == shipRecord
                    && n.PilotFaction == faction
                    && n.Instance.GetType().ToString().Contains(Edition.Current.NameShort)
                    && !n.Instance.IsHiddenSquadbuilderOnly
                ).OrderByDescending(n => n.PilotSkill).
                OrderByDescending(n => n.Instance.PilotInfo.Cost).
                OrderByDescending(n => n.Tags.Contains(Content.Tags.BoY) || n.Tags.Contains(Content.Tags.SoC) ? 0 : 1).
                ToList();
            }
            
            int pilotsCount = AllPilotsFiltered.Count();

            Transform contentTransform = GameObject.Find("UI/Panels/SelectPilotPanel/Panel/Scroll View/Viewport/Content").transform;
            SquadBuilderView.DestroyChildren(contentTransform);
            contentTransform.localPosition = new Vector3(0, contentTransform.localPosition.y, contentTransform.localPosition.z);

            FromLeft = 25f;
            foreach (PilotRecord pilot in AllPilotsFiltered)
            {
                ShowAvailablePilot(pilot);
            }

            contentTransform.GetComponent<RectTransform>().sizeDelta = new Vector2(FromLeft+300, 0);
        }

        private void ShowAvailablePilot(PilotRecord pilotRecord)
        {
            GameObject prefab = (GameObject)Resources.Load("Prefabs/SquadBuilder/PilotPanel", typeof(GameObject));
            Transform contentTransform = GameObject.Find("UI/Panels/SelectPilotPanel/Panel/Scroll View/Viewport/Content").transform;
            GameObject newPilotPanel = MonoBehaviour.Instantiate(prefab, contentTransform);

            if (pilotRecord.Instance.PilotInfo.IsStandardLayout)
            {
                newPilotPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(722f, 418f);
            }

            newPilotPanel.transform.localPosition = new Vector3(FromLeft, newPilotPanel.GetComponent<RectTransform>().sizeDelta.y / 2f);
            FromLeft += newPilotPanel.GetComponent<RectTransform>().sizeDelta.x + 25f;

            GenericShip newShip = (GenericShip)Activator.CreateInstance(Type.GetType(pilotRecord.PilotTypeName));
            Edition.Current.AdaptShipToRules(newShip);
            Edition.Current.AdaptPilotToRules(newShip);

            PilotPanelSquadBuilder script = newPilotPanel.GetComponent<PilotPanelSquadBuilder>();
            script.Initialize(newShip, PilotSelectedIsClicked, true);
        }

        public void PilotSelectedIsClicked(GenericShip ship)
        {
            Global.SquadBuilder.CurrentSquad.AddPilotToSquad(ship, isFromUi: true);
            MainMenu.CurrentMainMenu.ChangePanel("SquadBuilderPanel");
        }
    }
}

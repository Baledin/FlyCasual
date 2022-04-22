﻿using Editions;
using ExtraOptions.ExtraOptionsList;
using Players;
using SubPhases;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using BoardTools;
using Ship;
using GameCommands;
using GameModes;
using System;


namespace SquadBuilderNS
{
    public class DeploymentConfig
    {
        public List<GenericShip> Ships { get; set; }
        public GameObject StartingZone { get; set; }

        public DeploymentConfig(List<GenericShip> Ships, GameObject StartingZone)
        {
            this.Ships = Ships;
            this.StartingZone = StartingZone;
        }
    }
    public class CampaignShipLoader
    {
        private float FormationWidth { get; set; }

        private Dictionary<GenericShip, Vector3> PlannedShipPositions = new Dictionary<GenericShip, Vector3>();

        private int ShipDirection { get; set; }

        private JSONObject campaignMission { get; set; }

        public void LoadCampaign()
        {
            List<JSONObject> campaignMissionJsons = GetCampaignMissionsJsons();
            campaignMission = campaignMissionJsons[0];
            List<DeploymentConfig> deploymentConfigs = new List<DeploymentConfig>();

            Phases.Events.OnSetupStart += InitialSetup;
            Phases.Events.OnPlanningPhaseStart += MidGameShipAddition;
        }

        //public void BrowseCampaignMissions()
        //{
        //    List<JSONObject> campaignMissionJsons = GetCampaignMissionsJsons();

        //    ShowCampaignMissions(campaignMissionJsons);
        //}

        private List<JSONObject> GetCampaignMissionsJsons()
        {
            List<JSONObject> campaignMissionsJsons = new List<JSONObject>();

            string directoryPath = Application.persistentDataPath + "/" + Edition.Current.Name + "/" + Edition.Current.PathToCampaignSetup;
            if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

            if (Directory.GetFiles(directoryPath).Length < 1)
            { 
                string saveFilePath = directoryPath + "/001 Hotac Test.json";
                File.WriteAllText(saveFilePath, "{\"description\":\"Hotac Test\",\"name\":\"001 Hotac Test\",\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"deploymentConfigs\":[{\"faction\":\"galacticempire\",\"startingZone\":\"StartingZoneCampaign3\",\"deploymentRound\":\"0\",\"pilots\":[{\"id\":\"academypilot\",\"ship\":\"tielnfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"academypilot\",\"ship\":\"tielnfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}}]},{\"faction\":\"galacticempire\",\"startingZone\":\"StartingZoneCampaign4\",\"deploymentRound\":\"0\",\"pilots\":[{\"id\":\"academypilot\",\"ship\":\"tielnfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"academypilot\",\"ship\":\"tielnfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}}]},{\"faction\":\"galacticempire\",\"startingZone\":\"random\",\"deploymentRound\":\"4\",\"pilots\":[{\"id\":\"alphasquadronpilot\",\"ship\":\"tieininterceptor\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}}]},{\"faction\":\"galacticempire\",\"startingZone\":\"random\",\"deploymentRound\":\"7\",\"pilots\":[{\"id\":\"academypilot\",\"ship\":\"tielnfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"academypilot\",\"ship\":\"tielnfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}}]}],\"points\":76,\"version\":\"0.3.0\"}");
            }

            foreach (var filePath in Directory.GetFiles(directoryPath))
            {
                string content = File.ReadAllText(filePath);
                JSONObject missionJson = new JSONObject(content);
                missionJson.AddField("filename", new FileInfo(filePath).Name);
                campaignMissionsJsons.Add(missionJson);
            }

            return campaignMissionsJsons;
        }

        private void InitialSetup()
        {
            LoadShipsForRound(0);
        }

        private void LoadShipsForRound(int roundNumber)
        {
            List<DeploymentConfig> deploymentConfigs = new List<DeploymentConfig>();

            if (campaignMission.HasField("deploymentConfigs"))
            {
                JSONObject deploymentConfigsJson = campaignMission["deploymentConfigs"];
                foreach (JSONObject deploymentConfigJson in deploymentConfigsJson.list)
                {
                    List<GenericShip> deploymentGroup = new List<GenericShip>();
                    GameObject startingZone = new GameObject();
                    if (deploymentConfigJson.HasField("deploymentRound") && deploymentConfigJson["deploymentRound"].str.Equals(roundNumber.ToString()))
                    {
                        string factionNameXws = deploymentConfigJson["faction"].str;
                        Faction faction = Edition.Current.XwsToFaction(factionNameXws);

                        if (deploymentConfigJson.HasField("pilots"))
                        {
                            JSONObject pilotJsons = deploymentConfigJson["pilots"];
                            foreach (JSONObject pilotJson in pilotJsons.list)
                            {
                                string shipNameXws = pilotJson["ship"].str;

                                string shipNameGeneral = "";
                                ShipRecord shipRecord = SquadBuilder.Instance.Database.AllShips.FirstOrDefault(n => n.ShipNameCanonical == shipNameXws);
                                if (shipRecord == null)
                                {
                                    Messages.ShowError("Cannot find ship: " + shipNameXws);
                                    continue;
                                }

                                shipNameGeneral = shipRecord.ShipName;

                                string pilotNameXws = pilotJson["id"].str;
                                PilotRecord pilotRecord = SquadBuilder.Instance.Database.AllPilots.FirstOrDefault(n => n.PilotNameCanonical == pilotNameXws && n.Ship.ShipName == shipNameGeneral && n.PilotFaction == faction);
                                if (pilotRecord == null)
                                {
                                    Messages.ShowError("Cannot find pilot: " + pilotNameXws);
                                    continue;
                                }

                                GenericShip newShipInstance = (GenericShip)Activator.CreateInstance(Type.GetType(pilotRecord.PilotTypeName));
                                Edition.Current.AdaptShipToRules(newShipInstance);
                                SquadListShip newShip = Global.SquadBuilder.SquadLists[PlayerNo.Player2].AddShip(newShipInstance);

                                ShipFactory.SpawnShip(newShip);
                                deploymentGroup.Add(newShipInstance);
                                Roster.AddShipToLists(newShipInstance);

                                Dictionary<string, string> upgradesThatCannotBeInstalled = new Dictionary<string, string>();

                                if (pilotJson.HasField("upgrades"))
                                {
                                    JSONObject upgradeJsons = pilotJson["upgrades"];
                                    if (upgradeJsons.keys != null)
                                    {
                                        foreach (string upgradeType in upgradeJsons.keys)
                                        {
                                            JSONObject upgradeNames = upgradeJsons[upgradeType];
                                            foreach (JSONObject upgradeRecord in upgradeNames.list)
                                            {
                                                UpgradeRecord newUpgradeRecord = SquadBuilder.Instance.Database.AllUpgrades.FirstOrDefault(n => n.UpgradeNameCanonical == upgradeRecord.str);
                                                if (newUpgradeRecord == null)
                                                {
                                                    Messages.ShowError("Cannot find upgrade: " + upgradeRecord.str);
                                                }

                                                bool upgradeInstalledSucessfully = newShip.InstallUpgrade(upgradeRecord.str, Edition.Current.XwsToUpgradeType(upgradeType));
                                                if (!upgradeInstalledSucessfully && !upgradesThatCannotBeInstalled.ContainsKey(upgradeRecord.str)) upgradesThatCannotBeInstalled.Add(upgradeRecord.str, upgradeType);
                                            }
                                        }

                                        while (upgradeJsons.Count != 0)
                                        {
                                            Dictionary<string, string> upgradesThatCannotBeInstalledCopy = new Dictionary<string, string>(upgradesThatCannotBeInstalled);

                                            bool wasSuccess = false;
                                            foreach (var upgrade in upgradesThatCannotBeInstalledCopy)
                                            {
                                                bool upgradeInstalledSucessfully = newShip.InstallUpgrade(upgrade.Key, Edition.Current.XwsToUpgradeType(upgrade.Value));
                                                if (upgradeInstalledSucessfully)
                                                {
                                                    wasSuccess = true;
                                                    upgradesThatCannotBeInstalled.Remove(upgrade.Key);
                                                }
                                            }

                                            if (!wasSuccess) break;
                                        }
                                    }
                                }

                                if (pilotJson.HasField("vendor"))
                                {
                                    JSONObject vendorData = pilotJson["vendor"];
                                    if (vendorData.HasField("Sandrem.FlyCasual"))
                                    {
                                        JSONObject myVendorData = vendorData["Sandrem.FlyCasual"];
                                        if (myVendorData.HasField("skin"))
                                        {
                                            newShip.Instance.ModelInfo.SkinName = myVendorData["skin"].str;
                                        }
                                    }
                                }
                            }
                        }
                        if (deploymentConfigJson.HasField("startingZone"))
                        {
                            if (deploymentConfigJson["startingZone"].str == "random")
                            {
                                startingZone = RandomStartZone(1, 6);
                            }
                            else
                            {
                                startingZone = Board.GetStartingZoneCampaign(deploymentConfigJson["startingZone"].str);
                            }
                        }
                        DeploymentConfig deploymentConfig = new DeploymentConfig(deploymentGroup, startingZone);
                        deploymentConfigs.Add(deploymentConfig);
                    }                    
                }

                if(deploymentConfigs.Count>0)
                {
                    var subphase = Phases.StartTemporarySubPhaseNew<SetupCampaignShipSubPhase>(
                    "Setup",
                    delegate
                    {
                    }
                );

                    subphase.DeploymentConfigs = deploymentConfigs;

                    subphase.Start();
                }
            }
        }

        private void MidGameShipAddition()
        {
            LoadShipsForRound(Phases.RoundCounter);
        }


        private GameObject RandomStartZone(int min, int max)
        {
            int randomVector = UnityEngine.Random.Range(min, max);
            switch (randomVector)
            {
                case 1:
                    return Board.StartingZoneCampaign1;
                case 2:
                    return Board.StartingZoneCampaign2;
                case 3:
                    return Board.StartingZoneCampaign3;
                case 4:
                    return Board.StartingZoneCampaign4;
                case 5:
                    return Board.StartingZoneCampaign5;
                case 6:
                    return Board.StartingZoneCampaign6;
                default:
                    return Board.StartingZoneCampaign1;
            }
        }

    }
    public static class CampaignLoader
    {
        public static void LoadCampaign()
        {
            CampaignShipLoader currentCampaignShipLoader = new CampaignShipLoader();
            currentCampaignShipLoader.LoadCampaign();
        }
    }
}

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
using Newtonsoft.Json;
using Mods;


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
        
        private int squadSize { get; set; }
        private int averageInitiative { get; set; }

        public void LoadCampaign()
        {
            List<DeploymentConfig> deploymentConfigs = new List<DeploymentConfig>();

            Phases.Events.OnSetupStart += InitialSetup;
            Phases.Events.OnPlanningPhaseStart += MidGameShipAddition;
        }

        private void InitialSetup()
        {
            squadSize = Roster.GetPlayer(PlayerNo.Player1).Ships.Count;
            determineAverageInitiative();
            LoadShipsForRound(0);
        }

        private void determineAverageInitiative()
        {
            List<GenericShip> ships =  Roster.GetPlayer(PlayerNo.Player1).Ships.Values.ToList();
            int initiativeTotal = 0;
            foreach (GenericShip ship in ships)
            {
                initiativeTotal += ship.PilotInfo.Initiative;
            }
            averageInitiative = initiativeTotal / ships.Count;
        }

        private void LoadShipsForRound(int roundNumber)
        {
            List<DeploymentConfig> deploymentConfigs = new List<DeploymentConfig>();

            if (CampaignLoader.campaignMission.HasField("deploymentConfigs"))
            {
                JSONObject deploymentConfigsJson = CampaignLoader.campaignMission["deploymentConfigs"];
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
                                string squadSizeString = pilotJson["squadSize"].str;

                                int squadSizeValue = Int16.Parse(pilotJson["squadSize"].str);

                                int averageInitiaveValue = 0;

                                if(pilotJson.HasField("averageInitiave"))
                                {
                                    averageInitiaveValue = Int16.Parse(pilotJson["averageInitiave"].str);
                                }

                                if (squadSizeValue <= squadSize && averageInitiaveValue <= averageInitiative)
                                {
                                    if(pilotJson.HasField("replace"))
                                    {
                                        string typeValue = pilotJson["replace"].str;
                                        GenericShip replaceShip = null;
                                        string replaceShipId = null;
                                        foreach(KeyValuePair<string, GenericShip> shipEntry in Roster.Player2.Ships)
                                        {
                                            if(typeValue.Equals(shipEntry.Value.ShipTypeCanonical))
                                            {
                                                replaceShip = shipEntry.Value;
                                                replaceShipId = shipEntry.Key;
                                            }
                                        }
                                        if(replaceShip!=null)
                                        {
                                            Roster.RemoveDestroyedShip(replaceShipId);
                                            deploymentGroup.Remove(replaceShip);
                                        }
                                    }
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
                                    
                                    Dictionary<string, string> upgradesThatCannotBeInstalled = new Dictionary<string, string>();

                                    if (pilotJson.HasField("upgrades"))
                                    {
                                        JSONObject upgradeJsons = pilotJson["upgrades"];
                                        InstallCampaignUpgrade(newShip, upgradesThatCannotBeInstalled, upgradeJsons);
                                    }

                                    if(pilotJson.HasField("randomUpgrades"))
                                    {
                                        string upgradeClass = pilotJson["randomUpgrades"].str;
                                        InstallElitePilotUpgrades(upgradeClass, newShip, newShipInstance, upgradesThatCannotBeInstalled);
                                    }

                                    while (true)
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
                                    ShipFactory.SpawnShip(newShip);
                                    deploymentGroup.Add(newShipInstance);
                                    Roster.AddShipToLists(newShipInstance);
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
                        if (deploymentGroup.Count > 0)
                        {
                            DeploymentConfig deploymentConfig = new DeploymentConfig(deploymentGroup, startingZone);
                            deploymentConfigs.Add(deploymentConfig);
                        }
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

        private void InstallElitePilotUpgrades(string upgradeClass, SquadListShip newShip, GenericShip newShipInstance, Dictionary<string, string> upgradesThatCannotBeInstalled)
        {
            JSONObject elitePilotUpgrades = LoadElitePilotUpgrades();

            if(elitePilotUpgrades.HasField("ships"))
            {
                JSONObject shipsJson = elitePilotUpgrades["ships"];
                foreach (JSONObject shipJson in shipsJson.list)
                {
                    if (shipJson.HasField("type") && shipJson["type"].str.Equals(newShip.Instance.ShipTypeCanonical))
                    {
                        if (shipJson.HasField("upgrades"))
                        {
                            JSONObject upgrades = shipJson["upgrades"];
                            int randomUpgrade = UnityEngine.Random.Range(0, upgrades.Count);
                            JSONObject upgrade = upgrades[randomUpgrade];                            
                            if (upgrade.HasField("basic"))
                            {
                                JSONObject basicUpgrade = upgrade["basic"];
                                InstallCampaignUpgrade(newShip, upgradesThatCannotBeInstalled, basicUpgrade);
                            }
                            if (upgradeClass.Equals("basic"))
                            {
                                newShipInstance.PilotInfo.Initiative = 1;
                            }
                            if (upgradeClass.Equals("elite"))
                            {
                                if (upgrade.HasField("elite"))
                                {
                                    JSONObject eliteUpgrade = upgrade["elite"];
                                    InstallCampaignUpgrade(newShip, upgradesThatCannotBeInstalled, eliteUpgrade);
                                }
                                newShipInstance.PilotInfo.Initiative = 3;
                                if(averageInitiative>=3)
                                {
                                    newShipInstance.PilotInfo.Initiative = 4;
                                    if(upgrade.HasField("elite3"))
                                    {
                                        JSONObject eliteUpgrade = upgrade["elite3"];
                                        InstallCampaignUpgrade(newShip, upgradesThatCannotBeInstalled, eliteUpgrade);
                                    }
                                }
                                if (averageInitiative >= 4)
                                {
                                    newShipInstance.PilotInfo.Initiative = 5;
                                    if (upgrade.HasField("elite4"))
                                    {
                                        JSONObject eliteUpgrade = upgrade["elite4"];
                                        InstallCampaignUpgrade(newShip, upgradesThatCannotBeInstalled, eliteUpgrade);
                                    }
                                }
                                if (averageInitiative >= 5)
                                {
                                    newShipInstance.PilotInfo.Initiative = 6;
                                    if (upgrade.HasField("elite5"))
                                    {
                                        JSONObject eliteUpgrade = upgrade["elite5"];
                                        InstallCampaignUpgrade(newShip, upgradesThatCannotBeInstalled, eliteUpgrade);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private JSONObject LoadElitePilotUpgrades()
        {
            string directoryPath = Application.persistentDataPath + "/" + Edition.Current.Name + "/" + Edition.Current.PathToElitePilotUpgrades;
            if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

            string filePath = directoryPath + "/elitePilotUpgrades.json";
            File.WriteAllText(filePath, "{\"name\":\"elitePilotUpgrades\",\"ships\":[{\"type\":\"tiesabomber\",\"upgrades\":[{\"basic\":{\"missile\":[\"homingmissiles\"]},\"elite\":{\"torpedo\":[\"extramunitions\"],\"talent\":[\"calculation\"]},\"elite3\":{\"pilot\":[\"zertikstrompilotability\"]},\"elite4\":{\"talent\":[\"elusiveness\"]},\"elite5\":{\"pilot\":[\"rexlerbrathpilotability\"]}},{\"basic\":{\"torpedo\":[\"advprotontorpedoes\"]},\"elite\":{\"torpedo\":[\"extramunitions\"],\"modification\":[\"shieldupgrade\"]},\"elite3\":{\"pilot\":[\"majorrhymerpilotability\"]},\"elite4\":{\"talent\":[\"opportunist\"]},\"elite5\":{\"pilot\":[\"commanderkenkirkpilotability\"]}},{\"basic\":{\"missile\":[\"clustermissiles\"]},\"elite\":{\"torpedo\":[\"extramunitions\"],\"talent\":[\"outmaneuver\"]},\"elite3\":{\"pilot\":[\"redlinepilotability\"]},\"elite4\":{\"talent\":[\"ruthless\"]},\"elite5\":{\"pilot\":[\"kirkanospilotability\"]}},{\"basic\":{\"missile\":[\"clustermissiles\"]},\"elite\":{\"torpedo\":[\"extramunitions\"],\"talent\":[\"marksmanship\"]},\"elite3\":{\"pilot\":[\"commanderalozenpilotability\"]},\"elite4\":{\"talent\":[\"predator\"]},\"elite5\":{\"pilot\":[\"majorrhymerpilotability\"]}},{\"basic\":{\"missile\":[\"ionmissiles\"]},\"elite\":{\"torpedo\":[\"extramunitions\"],\"talent\":[\"swarmtactics\"]},\"elite3\":{\"pilot\":[\"howlrunnerpilotability\"]},\"elite4\":{\"talent\":[\"outmaneuver\"]},\"elite5\":{\"pilot\":[\"darkcursepilotability\"]}},{\"basic\":{\"torpedo\":[\"protontorpedoes\"]},\"elite\":{\"torpedo\":[\"extramunitions\"],\"talent\":[\"calculation\"]},\"elite3\":{\"pilot\":[\"nightbeastpilotability\"]},\"elite4\":{\"talent\":[\"predator\"]},\"elite5\":{\"pilot\":[\"kathscarletempirepilotability\"]}}]},{\"type\":\"tieininterceptor\",\"upgrades\":[{\"basic\":{\"modification\":[\"autothrusters\"]}},{\"basic\":{\"modification\":[\"autothrusters\"]}},{\"basic\":{\"modification\":[\"stealthdevice\"]}},{\"basic\":{\"modification\":[\"stealthdevice\"]}},{\"basic\":{\"modification\":[\"hullupgrade\"]}},{\"basic\":{\"modification\":[\"hullupgrade\"]}}]},{\"type\":\"tieadvancedx1\",\"upgrades\":[{\"basic\":{\"sensor\":[\"accuracycorrector\"]},\"elite\":{\"modification\":[\"shieldupgrade\"],\"talent\":[\"swarmtactics\"]},\"elite3\":{\"pilot\":[\"coloneljendon\"]}}]}]}");            
            string content = File.ReadAllText(filePath);
            JSONObject elitePilotJson = new JSONObject(content);
            return elitePilotJson;
        }

        private static void InstallCampaignUpgrade(SquadListShip newShip, Dictionary<string, string> upgradesThatCannotBeInstalled, JSONObject upgradeJsons)
        {
            if (upgradeJsons.keys != null)
            {
                foreach (string upgradeKey in upgradeJsons.keys)
                {
                    JSONObject upgradeNames = upgradeJsons[upgradeKey];
                    foreach (JSONObject upgradeRecord in upgradeNames.list)
                    {
                        string upgradeName = upgradeRecord.str;
                        string upgradeType = upgradeKey;

                        UpgradeRecord newUpgradeRecord = SquadBuilder.Instance.Database.AllUpgrades.FirstOrDefault(n => n.UpgradeNameCanonical == upgradeName);
                        if (newUpgradeRecord == null)
                        {
                            Messages.ShowError("Cannot find upgrade: " + upgradeName);
                        }

                        bool upgradeInstalledSucessfully = newShip.InstallUpgrade(upgradeName, Edition.Current.XwsToUpgradeType(upgradeType));
                        if (!upgradeInstalledSucessfully && !upgradesThatCannotBeInstalled.ContainsKey(upgradeName)) upgradesThatCannotBeInstalled.Add(upgradeName, upgradeType);
                    }
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
        public static JSONObject campaignMission { get; set; }
        public static void LoadCampaign()
        {
            CampaignShipLoader currentCampaignShipLoader = new CampaignShipLoader();
            currentCampaignShipLoader.LoadCampaign();
        }
    }
}

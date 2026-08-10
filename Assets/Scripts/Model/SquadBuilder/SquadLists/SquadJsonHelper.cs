using Content;
using Editions;
using Obstacles;
using Ship;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Upgrade;

namespace SquadBuilderNS
{
    public static class SquadJsonHelper
    {
        public static JSONObject GetSquadInJson(SquadList squadList)
        {
            JSONObject squadJson = new();

            squadJson.AddField("name", squadList.Name);
            squadJson.AddField("faction", Edition.Current.FactionToXws(squadList.SquadFaction));
            squadJson.AddField("ruleset", squadList.Format.ToString());
            squadJson.AddField("points", squadList.Points);
            squadJson.AddField("version", squadList.Format == Legality.XWA ? Global.CurrentXWAVersion : Global.CurrentAMGVersion);

            JSONObject vendor = new();
            JSONObject vendorFields = new();
            vendorFields.AddField("version", Global.CurrentVersion);
            vendorFields.AddField("versionInt", Global.CurrentVersionInt);
            vendor.AddField("Baledin.FlyCasual", vendorFields);

            squadJson.AddField("vendor", vendor);

            List<SquadListShip> playerShipConfigs = squadList.Ships;
            JSONObject[] squadPilotsArrayJson = new JSONObject[playerShipConfigs.Count];
            for (int i = 0; i < squadPilotsArrayJson.Length; i++)
            {
                squadPilotsArrayJson[i] = GenerateSquadPilot(playerShipConfigs[i]);
            }

            JSONObject squadPilotsJson = new(squadPilotsArrayJson);
            squadJson.AddField("pilots", squadPilotsJson);

            JSONObject squadObstalesArrayJson = new(JSONObject.Type.ARRAY);
            for (int i = 0; i < squadList.ChosenObstacles.Count; i++)
            {
                squadObstalesArrayJson.Add(squadList.ChosenObstacles[i].ShortName);
            }

            squadJson.AddField("obstacles", squadObstalesArrayJson);
            squadJson.AddField("description", GetDescriptionOfSquadJson(squadJson));

            return squadJson;
        }

        public static string GetDescriptionOfSquadJson(JSONObject squadJson)
        {
            string result = "";

            try
            {
                if (squadJson.HasField("pilots"))
                {
                    JSONObject pilotJsons = squadJson["pilots"];
                    foreach (JSONObject pilotJson in pilotJsons.list)
                    {
                        if (result != "") result += " | ";

                        string shipNameXws = pilotJson["ship"].str;
                        string shipNameGeneral = SquadBuilder.Instance.Database.AllShips.Find(n => n.ShipNameCanonical == shipNameXws).ShipName;

                        string pilotNameXws = pilotJson["id"].str;
                        string pilotNameGeneral = SquadBuilder.Instance.Database.AllPilots.Find(n => n.PilotNameCanonical == pilotNameXws).PilotName;

                        result += pilotNameGeneral;

                        if (SquadBuilder.Instance.Database.AllPilots.Count(n => n.PilotName == pilotNameGeneral) > 1)
                        {
                            result += " (" + shipNameGeneral + ")";
                        }

                        if (pilotJson.HasField("upgrades"))
                        {
                            try
                            {
                                JSONObject upgradeJsons = pilotJson["upgrades"];
                                foreach (string upgradeType in upgradeJsons.keys)
                                {
                                    JSONObject upgradeNames = upgradeJsons[upgradeType];
                                    foreach (JSONObject upgradeRecord in upgradeNames.list)
                                    {
                                        string upgradeName = SquadBuilder.Instance.Database.AllUpgrades.Find(n => n.UpgradeNameCanonical == upgradeRecord.str).UpgradeName;
                                        result += " + " + upgradeName;
                                    }
                                }
                            }
                            catch (Exception) { }
                        }
                    }
                }

                result = result.Replace("\"", "\\\"");
            }
            catch (Exception)
            {
                Messages.ShowError("Error during creation of description of squadron");
            }

            return result;
        }

        private static JSONObject GenerateSquadPilot(SquadListShip shipHolder)
        {
            JSONObject pilotJson = new();
            pilotJson.AddField("id", shipHolder.Instance.PilotNameCanonical);
            pilotJson.AddField("ship", shipHolder.Instance.ShipTypeCanonical);

            Dictionary<string, JSONObject> upgradesDict = new();
            if (!(shipHolder.Instance.PilotInfo as PilotCardInfo25).IsStandardLayout)
            {
                foreach (GenericUpgrade installedUpgrade in shipHolder.Instance.UpgradeBar.GetUpgradesAll())
                {
                    string slotName = Edition.Current.UpgradeTypeToXws(installedUpgrade.UpgradeInfo.UpgradeTypes[0]);
                    if (!upgradesDict.ContainsKey(slotName))
                    {
                        JSONObject upgrade = new();
                        upgrade.Add(installedUpgrade.NameCanonical);
                        upgradesDict.Add(slotName, upgrade);
                    }
                    else
                    {
                        upgradesDict[slotName].Add(installedUpgrade.NameCanonical);
                    }
                }
            }

            JSONObject upgradesDictJson = new(upgradesDict);
            pilotJson.AddField("upgrades", upgradesDictJson);

            JSONObject vendorJson = new();
            JSONObject skinJson = new();
            skinJson.AddField("skin", (shipHolder.Instance.PilotInfo as PilotCardInfo25).SkinName);
            vendorJson.AddField("Baledin.FlyCasual", skinJson);

            pilotJson.AddField("vendor", vendorJson);

            return pilotJson;
        }

        public static void CreateSquadFromImportedJson(SquadList squad, string jsonString)
        {
            JSONObject squadJson = new(jsonString);
            SetPlayerSquadFromImportedJson(squad, squadJson);
        }

        public static void SetPlayerSquadFromImportedJson(SquadList squad, JSONObject squadJson)
        {
            try
            {
                squad.ClearAll();

                if (squadJson.HasField("name"))
                {
                    squad.Name = squadJson["name"].str;
                }

                if (squad.PlayerNo == Players.PlayerNo.Player1 || squad.PlayerNo == Players.PlayerNo.PlayerNone)
                {
                    if (squadJson.HasField("ruleset"))
                    {
                        squad.Format = Options.GetFormatAsLegality(squadJson["ruleset"].str);
                    }
                    else
                    {
                        squad.Format = Legality.ExtendedLegal;
                    }

                    Options.ListFormat = squad.Format; // Options.ListFormat keeps everything in sync for the saved list

                    MainMenu.SetEdition(squad.Format);
                }
                else
                {
                    squad.Format = Options.ListFormat; // ensures second player format is in sync with first player format
                }

                string factionNameXws = squadJson["faction"].str;
                Faction faction = Edition.Current.XwsToFaction(factionNameXws);
                squad.SquadFaction = faction;

                if (squadJson.HasField("pilots"))
                {
                    JSONObject pilotJsons = squadJson["pilots"];
                    foreach (JSONObject pilotJson in pilotJsons.list)
                    {
                        string shipNameXws = pilotJson["ship"].str;

                        string shipNameGeneral = "";
                        ShipRecord shipRecord = SquadBuilder.Instance.Database.AllShips.FirstOrDefault(n => n.ShipNameCanonical == shipNameXws && n.AllowableFormats.Contains(squad.Format));
                        if (shipRecord == null)
                        {
                            Messages.ShowError("Cannot find ship: " + shipNameXws);
                            continue;
                        }

                        shipNameGeneral = shipRecord.ShipName;

                        string pilotNameXws = pilotJson["id"].str;
                        pilotNameXws = pilotNameXws.Replace("-btanr2wywing", "-wartime"); // fix for old Wartime Loadout ship names
                        PilotRecord pilotRecord = SquadBuilder.Instance.Database.AllPilots.FirstOrDefault(n => n.PilotNameCanonical == pilotNameXws && n.Ship.ShipName == shipNameGeneral && n.PilotFaction == faction && n.AllowableFormats.Contains(squad.Format));
                        if (pilotRecord == null)
                        {
                            Messages.ShowError("Cannot find pilot: " + pilotNameXws);
                            continue;
                        }

                        GenericShip newShipInstance = (GenericShip)Activator.CreateInstance(Type.GetType(pilotRecord.PilotTypeName));
                        Edition.Current.AdaptShipToRules(newShipInstance);
                        SquadListShip newShip = squad.AddPilotToSquad(newShipInstance);

                        Dictionary<string, string> upgradesThatCannotBeInstalled = new();

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
                                        UpgradeRecord newUpgradeRecord = SquadBuilder.Instance.Database.AllUpgrades.FirstOrDefault(n => n.UpgradeNameCanonical == upgradeRecord.str && n.AllowableFormats.Contains(squad.Format));
                                        if (newUpgradeRecord == null)
                                        {
                                            Messages.ShowError("Cannot find upgrade: " + upgradeRecord.str);
                                        }

                                        bool upgradeInstalledSucessfully = newShip.InstallUpgrade(newUpgradeRecord);
                                        if (!upgradeInstalledSucessfully && !upgradesThatCannotBeInstalled.ContainsKey(upgradeRecord.str)) upgradesThatCannotBeInstalled.Add(upgradeRecord.str, upgradeType);
                                    }
                                }

                                while (upgradeJsons.Count != 0)
                                {
                                    Dictionary<string, string> upgradesThatCannotBeInstalledCopy = new(upgradesThatCannotBeInstalled);

                                    bool wasSuccess = false;
                                    foreach (KeyValuePair<string, string> upgrade in upgradesThatCannotBeInstalledCopy)
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

                        if ((newShipInstance.PilotInfo as PilotCardInfo25).IsStandardLayout)
                        {
                            foreach (Type upgradeType in newShipInstance.MustHaveUpgrades)
                            {
                                bool upgradeInstalledSucessfully = newShip.InstallUpgrade(upgradeType.ToString());
                                if (!upgradeInstalledSucessfully)
                                {
                                    Messages.ShowError("Cannot install upgrade: " + upgradeType.ToString());
                                }
                            }
                        }

                        if (pilotJson.HasField("vendor"))
                        {
                            JSONObject vendorData = pilotJson["vendor"];
                            if (vendorData.HasField("Baledin.FlyCasual"))
                            {
                                JSONObject myVendorData = vendorData["Baledin.FlyCasual"];
                                if (myVendorData.HasField("skin"))
                                {
                                    (newShip.Instance.PilotInfo as PilotCardInfo25).SkinName = myVendorData["skin"].str;
                                }
                            }
                        }
                    }
                }
                else
                {
                    Messages.ShowError("The squad has no pilots");
                }

                if (squadJson.HasField("obstacles"))
                {
                    if (squadJson["obstacles"].Count == 3)
                    {
                        squad.ChosenObstacles.Clear();
                        squad.ChosenObstacles.AddRange(
                            new List<GenericObstacle>()
                            {
                                ObstaclesManager.GetPossibleObstacle(squadJson["obstacles"][0].str),
                                ObstaclesManager.GetPossibleObstacle(squadJson["obstacles"][1].str),
                                ObstaclesManager.GetPossibleObstacle(squadJson["obstacles"][2].str)
                            }
                        );
                    }
                    else
                    {
                        Messages.ShowError("Not enough obstacles in imported XWS, default obstacles are set");
                        squad.SetDefaultObstacles();
                    }
                }
                else
                {
                    squad.SetDefaultObstacles();
                }
            }
            catch (Exception)
            {
                Messages.ShowError("Error during creation of squadron");
            }
        }

        public static void SaveSquadronToFile(SquadList squad, string squadName)
        {
            squad.Name = CleanFileName(squadName);

            // check that directory exists, if not create it
            string directoryPath = Application.persistentDataPath + "/" + Edition.Current.Name + "/SavedSquadrons";
            if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

            string filePath = $"{directoryPath}/{squad.Name}.json";

            if (File.Exists(filePath)) File.Delete(filePath);

            File.WriteAllText(filePath, GetSquadInJson(squad).ToString());
        }

        private static string CleanFileName(string fileName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
        }
    }
}
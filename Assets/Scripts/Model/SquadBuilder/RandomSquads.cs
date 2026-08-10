using Editions;
using Players;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace SquadBuilderNS
{
    public static class RandomSquads
    {
        public static void SetRandomAiSquad()
        {
            JSONObject json = GetRandomAiSquad();

            if (json != null)
            {
                Global.SquadBuilder.CurrentPlayer = PlayerNo.Player2;

                Global.SquadBuilder.CurrentSquad.SetPlayerSquadFromImportedJson(json);
            }
        }

        private static JSONObject GetRandomAiSquad()
        {
            List<string> preGeneratedSquadrons = new();

            preGeneratedSquadrons.AddRange(GetDefaultAiSquads());

            preGeneratedSquadrons.AddRange(GetCustomAiSquads());

            if (preGeneratedSquadrons.Count == 0)
            {
                return null;
            }

            return new (preGeneratedSquadrons.ElementAt(Random.Range(0, preGeneratedSquadrons.Count)));
        }

        private static List<string> GetDefaultAiSquads()
        {
            List<string> defaultSquads = new();

            if (!DebugManager.NoDefaultAiSquads)
            {
                defaultSquads.AddRange(Edition.Current.PreGeneratedAiSquadrons.Values);
            }

            return defaultSquads;
        }


        private static List<string> GetCustomAiSquads()
        {
            List<string> customSquads = new();

            string directory = Application.persistentDataPath + "/" + Edition.Current.Name + "/AiSquadrons";

            if (Directory.Exists(directory) && Directory.GetFiles(directory).Count() > 0)
            {
                foreach(string file in Directory.GetFiles(directory))
                {
                    customSquads.Add(File.ReadAllText(file));
                }
            }

            return customSquads;
        }
    }
}
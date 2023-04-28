﻿using Mods.ModsList;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RZ1AWing
    {
        public class HeraSyndullaPhoenixSquadronMod : RZ1AWing
        {
            public HeraSyndullaPhoenixSquadronMod() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Hera Syndulla",
                    5,
                    38,
                    pilotTitle: "Spectre-2",
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.HeraSyndullaAbility),
                    extraUpgradeIcons: new List<UpgradeType>() { UpgradeType.Talent, UpgradeType.Talent }
                );

                RequiredMods = new List<Type>() { typeof(PhoenixSquadronModSE) };
                PilotNameCanonical = "herasyndulla-rz1awing-phoenixsquadronmod";

                ModelInfo.SkinName = "Hera Syndulla";
            }
        }
    }
}
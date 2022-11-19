﻿
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.GauntletFighter
    {
        public class Chopper : GauntletFighter
        {
            public Chopper() : base()
            {
                //IsWIP = true;

                RequiredMods = new List<Type>() { typeof(Mods.ModsList.UnreleasedContentMod) };

                PilotInfo = new PilotCardInfo
                (
                    "\"Chopper\"",
                    2,
                    51,
                    pilotTitle: "Spectre-3",
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.ChopperPilotAbility),
                    factionOverride: Faction.Rebel
                );

                ShipInfo.ActionIcons.SwitchToDroidActions();

                PilotNameCanonical = "chopper-gauntletfighter";

                ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/4/45/Choppergauntlet.png";
            }
        }
    }
}
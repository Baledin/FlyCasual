﻿using Mods.ModsList;
using System;
using System.Collections.Generic;
using Upgrade;
using Content;

namespace Ship
{
    namespace SecondEdition.TIEDDefender
    {
        public class DarthVaderMyOtherRideIsMod : TIEDDefender
        {
            public DarthVaderMyOtherRideIsMod() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Darth Vader",
                    6,
                    125,
                    pilotTitle: "Black Leader",
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.DarthVaderAbility),
                    tags: new List<Tags>
                    {
                        Tags.DarkSide,
                        Tags.Sith
                    },
                    force: 3,
                    extraUpgradeIcons: new List<UpgradeType>(){ UpgradeType.ForcePower, UpgradeType.Sensor }
                );

                RequiredMods = new List<Type>() { typeof(MyOtherRideIsModSE) };
                PilotNameCanonical = "darthvader-tieddefender-myotherrideis";

                ImageUrl = "https://i.imgur.com/QwUseck.png";
            }
        }
    }
}
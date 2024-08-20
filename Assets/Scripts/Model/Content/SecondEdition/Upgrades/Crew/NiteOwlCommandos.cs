﻿using Ship;
using Upgrade;
using System.Collections.Generic;
using System;
using Bombs;
using BoardTools;
using Movement;
using SubPhases;
using Remote;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class NiteOwlCommandos : GenericUpgrade
    {
        public NiteOwlCommandos() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Nite Owl Commandos",
                types: new List<UpgradeType>()
                {
                    UpgradeType.Crew,
                    UpgradeType.Crew
                },
                subType: UpgradeSubType.Remote,
                cost: 10,
                isLimited: true,
                charges: 2,
                cannotBeRecharged: true,
                restrictions: new UpgradeCardRestrictions(
                    new FactionRestriction(Faction.Republic),
                    new BaseSizeRestriction(BaseSize.Medium, BaseSize.Large)
                ),
                abilityType: typeof(Abilities.SecondEdition.CommandosAbility),
                remoteType: typeof(Remote.CommandoTeam)
            );
        }
        public override List<ManeuverTemplate> GetDefaultDropTemplates()
        {
            return new List<ManeuverTemplate>()
            {
                new ManeuverTemplate(ManeuverBearing.Straight, ManeuverDirection.Forward, ManeuverSpeed.Speed1, isBombTemplate: true)
            };
        }
    }
}

﻿using Ship;
using Upgrade;
using UnityEngine;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Conditions;

namespace UpgradesList.SecondEdition
{
    public class KyloRen : GenericUpgrade
    {
        public KyloRen() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Kylo Ren",
                UpgradeType.Crew,
                cost: 9,
                isLimited: true,
                restriction: new FactionRestriction(Faction.FirstOrder),
                abilityType: typeof(Abilities.SecondEdition.KyloRenCrewAbility),
                addForce: 1
            );

            Avatar = new AvatarInfo(
                Faction.FirstOrder,
                new Vector2(286, 1)
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class KyloRenCrewAbility : Abilities.FirstEdition.KyloRenCrewAbility
    {
        protected override bool IsActionAvailbale()
        {
            return HostShip.State.Force > 0;
        }

        protected override void SpendExtra(Action callback)
        {
            HostShip.State.SpendForce(1, callback);
        }
    }
}
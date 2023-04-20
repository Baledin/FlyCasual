﻿using Ship;
using Upgrade;
using System.Collections.Generic;
using Abilities;
using ActionsList;
using Actions;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class TacticalOfficer : GenericUpgrade
    {
        public TacticalOfficer() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Tactical Officer",
                UpgradeType.Crew,
                cost: 2,
                addAction: new ActionInfo(typeof(CoordinateAction)),
                restriction: new ActionBarRestriction(typeof(CoordinateAction), ActionColor.Red)
            );

            Avatar = new AvatarInfo(
                Faction.Imperial,
                new Vector2(550, 3),
                new Vector2(125, 125)
            );
        }
    }
}
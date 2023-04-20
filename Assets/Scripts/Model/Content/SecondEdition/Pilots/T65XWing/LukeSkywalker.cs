﻿using Abilities.SecondEdition;
using Upgrade;
using System.Collections.Generic;
using Content;

namespace Ship
{
    namespace SecondEdition.T65XWing
    {
        public class LukeSkywalker : T65XWing
        {
            public LukeSkywalker() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Luke Skywalker",
                    5,
                    61,
                    isLimited: true,
                    abilityType: typeof(LukeSkywalkerAbility),
                    tags: new List<Tags>
                    {
                        Tags.LightSide
                    },
                    force: 2,
                    extraUpgradeIcon: UpgradeType.ForcePower
                );

                ModelInfo.SkinName = "Luke Skywalker";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LukeSkywalkerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnDefenceStartAsDefender += RecoverForce;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnDefenceStartAsDefender -= RecoverForce;
        }

        private void RecoverForce()
        {
            if (HostShip.State.Force < HostShip.State.MaxForce)
            {
                HostShip.State.RestoreForce();
                Messages.ShowInfo(HostShip.PilotInfo.PilotName + " recovered 1 Force");
            }
        }
    }
}
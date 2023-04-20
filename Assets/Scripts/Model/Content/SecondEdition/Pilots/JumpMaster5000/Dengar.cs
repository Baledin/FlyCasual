﻿using BoardTools;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.JumpMaster5000
    {
        public class Dengar : JumpMaster5000
        {
            public Dengar() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Dengar",
                    6,
                    53,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.DengarPilotAbility),
                    charges: 1,
                    regensCharges: 1,
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DengarPilotAbility : Abilities.FirstEdition.DengarPilotAbility
    {
        protected override bool CanCounterattackUsingShotInfo(ShotInfo counterAttackInfo)
        {
            return HostShip.SectorsInfo.IsShipInSector(Combat.Attacker, Arcs.ArcType.Front);
        }

        protected override bool CanUseAbility()
        {
            return HostShip.State.Charges > 0;
        }

        protected override void MarkAbilityAsUsed()
        {
            HostShip.SpendCharge();
        }
    }
}
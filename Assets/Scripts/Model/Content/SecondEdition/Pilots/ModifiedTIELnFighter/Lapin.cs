﻿using ActionsList;
using Ship;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ModifiedTIELnFighter
    {
        public class Lapin : ModifiedTIELnFighter
        {
            public Lapin() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Lapin",
                    2,
                    25,
                    pilotTitle: "Stickler for Details",
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.LapinAbility),
                    extraUpgradeIcons: new List<UpgradeType>() { UpgradeType.Talent}
                );
                
                ImageUrl = "https://images.squarespace-cdn.com/content/v1/5ce432b1f9d2be000134d8ae/1437f98b-cff9-42d8-bca6-2f466f9b9d89/SWZ97_Lapinlegal.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LapinAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnTryAddAvailableDiceModificationGlobal += UseLapinRestriction;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnTryAddAvailableDiceModificationGlobal -= UseLapinRestriction;
        }

        private void UseLapinRestriction(GenericShip ship, GenericAction diceModification, ref bool canBeUsed)
        {
            if (!diceModification.IsNotRealDiceModification
                && ship.Tokens.HasToken(typeof(StressToken))
                && (IsShipIsFightingAgainstLapin(ship))
            )
            {
                Messages.ShowErrorToHuman($"{HostShip.PilotInfo.PilotName}: {ship.PilotInfo.PilotName} is unable to modify dice");
                canBeUsed = false;
            }
        }

        private bool IsShipIsFightingAgainstLapin(GenericShip ship)
        {
            if (Tools.IsSameShip(Combat.Attacker, HostShip) && Tools.IsSameShip(Combat.Defender, ship)) return true;
            else if (Tools.IsSameShip(Combat.Defender, HostShip) && Tools.IsSameShip(Combat.Attacker, ship)) return true;
            else return false;
        }
    }
}
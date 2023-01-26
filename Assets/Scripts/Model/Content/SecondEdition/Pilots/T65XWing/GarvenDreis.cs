﻿using System.Collections;
using System.Collections.Generic;
using Ship;
using SubPhases;
using Tokens;
using Abilities.FirstEdition;
using Upgrade;
using BoardTools;

namespace Ship
{
    namespace SecondEdition.T65XWing
    {
        public class GarvenDreis : T65XWing
        {
            public GarvenDreis() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Garven Dreis",
                    4,
                    46,
                    isLimited: true,
                    abilityType: typeof(GarvenDreisAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 4
                );

                PilotNameCanonical = "garvendreis-t65xwing";
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class GarvenDreisAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnTokenIsSpent += RegisterGarvenDreisPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnTokenIsSpent -= RegisterGarvenDreisPilotAbility;
        }

        private void RegisterGarvenDreisPilotAbility(GenericShip ship, GenericToken token)
        {
            if (token is FocusToken)
            {
                RegisterAbilityTrigger(TriggerTypes.OnTokenIsSpent, StartSubphaseForGarvenDreisPilotAbility);
            }
        }

        private void StartSubphaseForGarvenDreisPilotAbility(object sender, System.EventArgs e)
        {
            if (HasFriendlyShipsInRange())
            {
                SelectTargetForAbility(
                    SelectGarvenDreisAbilityTarget,
                    FilterAbilityTarget,
                    GetAiAbilityPriority,
                    HostShip.Owner.PlayerNo,
                    HostShip.PilotInfo.PilotName,
                    "Choose another ship to assign Focus token to it",
                    HostShip
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private bool HasFriendlyShipsInRange()
        {
            foreach (GenericShip friendlyShip in HostShip.Owner.Ships.Values)
            {
                if (friendlyShip.ShipId != HostShip.ShipId && Tools.IsFriendly(friendlyShip, HostShip))
                {
                    DistanceInfo distInfo = new DistanceInfo(HostShip, friendlyShip);
                    if (distInfo.Range >= 1 && distInfo.Range <= 3) return true;
                }
            }

            return false;
        }

        protected virtual bool FilterAbilityTarget(GenericShip ship)
        {
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.OtherFriendly }) && FilterTargetsByRange(ship, 1, 3);
        }

        private int GetAiAbilityPriority(GenericShip ship)
        {
            int result = 0;
            int shipFocusTokens = ship.Tokens.CountTokensByType(typeof(FocusToken));
            if (shipFocusTokens == 0) result += 100;
            result += (5 - shipFocusTokens);
            return result;
        }

        private void SelectGarvenDreisAbilityTarget()
        {
            MovementTemplates.ReturnRangeRuler();

            TargetShip.Tokens.AssignToken(typeof(FocusToken), SelectShipSubPhase.FinishSelection);
        }
    }
}

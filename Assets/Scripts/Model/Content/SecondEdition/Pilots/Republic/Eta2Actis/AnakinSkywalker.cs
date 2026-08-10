using BoardTools;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace Ship.SecondEdition.Eta2Actis
{
    public class AnakinSkywalker : Eta2Actis
    {
        public AnakinSkywalker()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Anakin Skywalker",
                "Hero of Coruscant",
                Faction.Republic,
                6,
                5,
                15,
                isLimited: true,
                force: 3,
                abilityType: typeof(Abilities.SecondEdition.AnakinSkywalkerActisAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.ForcePower,
                    UpgradeType.ForcePower,
                    UpgradeType.Talent,
                    UpgradeType.Astromech,
                    UpgradeType.Modification,
                    UpgradeType.Cannon
                },
                tags: new List<Tags>
                {
                    Tags.DarkSide,
                    Tags.Jedi,
                    Tags.LightSide
                },
                skinName: "Yellow",
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            PilotNameCanonical = "anakinskywalker-eta2actis";
        }
    }

    public class AnakinSkywalkerXWA : AnakinSkywalker
    {
        public AnakinSkywalkerXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 12;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 13;
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class AnakinSkywalkerActisAbility : GenericAbility
    {
        private GenericShip TriggedShip;

        public override void ActivateAbility()
        {
            GenericShip.OnMovementFinishGlobal += CheckFirstPartOfConditions;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnMovementFinishGlobal -= CheckFirstPartOfConditions;
        }

        private void CheckFirstPartOfConditions(GenericShip ship)
        {
            if (((IsMe(ship)) || (IsObiWanKenobiInRange(ship)))
                && HasMoreEnemyShipsInRange(ship)
            )
            {
                TriggedShip = ship;
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, CheckSecondPartOfConditions);
            }
        }

        private bool IsMe(GenericShip ship)
        {
            return ship.ShipId == HostShip.ShipId;
        }

        private bool IsObiWanKenobiInRange(GenericShip ship)
        {
            bool result = false;

            if (ship.PilotInfo.PilotName == "Obi-Wan Kenobi" && ship.Owner.PlayerNo == HostShip.Owner.PlayerNo)
            {
                DistanceInfo distanceInfo = new DistanceInfo(HostShip, ship);
                if (distanceInfo.Range <= 3)
                {
                    result = true;
                }
            }

            return result;
        }

        private bool HasMoreEnemyShipsInRange(GenericShip ship)
        {
            int anotherFriendlyShipsInRange = 0;
            int enemyShipsInRange = 0;

            foreach (GenericShip anotherShip in Roster.AllShips.Values)
            {
                if (anotherShip.ShipId == ship.ShipId) continue;

                DistanceInfo distInfo = new DistanceInfo(ship, anotherShip);
                if (distInfo.Range <= 1)
                {
                    if (ship.Owner.PlayerNo == anotherShip.Owner.PlayerNo)
                    {
                        anotherFriendlyShipsInRange++;
                    }
                    else
                    {
                        enemyShipsInRange++;
                    }
                }
            }

            return enemyShipsInRange > anotherFriendlyShipsInRange;
        }

        private void CheckSecondPartOfConditions(object sender, EventArgs e)
        {
            if (TriggedShip.Tokens.HasTokenByColor(TokenColors.Red) && HostShip.State.Force > 0)
            {
                AskToUseAnakinsAbility();
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void AskToUseAnakinsAbility()
        {
            var subphase = Phases.StartTemporarySubPhaseNew<AnakinSkywalkerRemoveRedTokenAbilityDecisionSubPhase>(
                "Anakin Skywalker: You may spend 1 force to remove 1 red token",
                Triggers.FinishTrigger
            );
            subphase.ImageSource = HostShip;
            subphase.AbilityHostShip = HostShip;
            subphase.Start();
        }
    }
}

namespace SubPhases
{
    public class AnakinSkywalkerRemoveRedTokenAbilityDecisionSubPhase : RemoveRedTokenDecisionSubPhase
    {
        public GenericShip AbilityHostShip;

        public override void PrepareCustomDecisions()
        {
            DescriptionShort = "Anakin Skywalker";
            DescriptionLong = "You may spend 1 force to remove 1 red token";

            DecisionOwner = Selection.ThisShip.Owner;
            DefaultDecisionName = decisions.First().Name;
        }

        public override void DoCustomFinishDecision()
        {
            AbilityHostShip.State.SpendForce(1, base.DoCustomFinishDecision);
        }
    }
}

using Abilities.SecondEdition;
using ActionsList;
using BoardTools;
using Content;
using Ship;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEInterceptor
    {
        public class Sigma6BoY : TIEInterceptor
        {
            public Sigma6BoY() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Sigma 6",
                    "Battle of Yavin",
                    Faction.Imperial,
                    4,
                    4,
                    0,
                    charges: 2,
                    isLimited: true,
                    abilityType: typeof(Sigma6BoYAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    isStandardLayout: true,
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );

                ShipInfo.Hull++;

                AutoThrustersAbility oldAbility = (AutoThrustersAbility)ShipAbilities.First(n => n.GetType() == typeof(AutoThrustersAbility));
                ShipAbilities.Remove(oldAbility);
                ShipAbilities.Add(new SensitiveControlsBoYRealAbility());

                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.Daredevil));
                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.AfterBurners));

                PilotNameCanonical = "sigma6-battleofyavin";
            }
        }

        public class Sigma6BoYXWA : Sigma6BoY
        {
            public Sigma6BoYXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 11;
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class Sigma6BoYAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (Board.IsOffTheBoard(ship)) return;

            if (ship.AssignedManeuver.Speed >= 3
                && HostShip.State.Charges > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskPerformSlamAction);
            }
        }

        private void AskPerformSlamAction(object sender, System.EventArgs e)
        {
            if (HostShip.State.Charges > 0)
            {
                HostShip.BeforeActionIsPerformed += PayChargeCost;

                HostShip.AskPerformFreeAction
                (
                    new SlamAction(HostShip),
                    CleanUp,
                    HostShip.PilotInfo.PilotName,
                    "You may spend 1 charge to perform an SLAM action"
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void PayChargeCost(GenericAction action, ref bool isFreeAction)
        {
            HostShip.SpendCharge();
            HostShip.BeforeActionIsPerformed -= PayChargeCost;
        }

        private void CleanUp()
        {
            HostShip.BeforeActionIsPerformed -= PayChargeCost;
            Triggers.FinishTrigger();
        }
    }
}
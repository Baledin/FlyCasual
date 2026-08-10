using Abilities.SecondEdition;
using ActionsList;
using Content;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEInterceptor
    {
        public class SecondSister : TIEInterceptor
        {
            public SecondSister() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Second Sister",
                    "Manipulative Monster",
                    Faction.Imperial,
                    4,
                    5,
                    14,
                    force: 2,
                    isLimited: true,
                    abilityType: typeof(SecondSisterAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.ForcePower,
                        UpgradeType.Talent,
                        UpgradeType.Missile,
                        UpgradeType.Modification,
                        UpgradeType.Configuration
                    },
                    tags: new List<Tags>
                    {
                        Tags.DarkSide,
                        Tags.Tie
                    },
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );
            }
        }

        public class SecondSisterXWA : SecondSister
        {
            public SecondSisterXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 12;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 12;
                (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
                {
                    UpgradeType.ForcePower,
                    UpgradeType.Sensor,
                    UpgradeType.Modification,
                    UpgradeType.Configuration
                };
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SecondSisterAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModificationsCompareResults += TrySecondSisterDiceMofication;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModificationsCompareResults -= TrySecondSisterDiceMofication;
        }

        private void TrySecondSisterDiceMofication(GenericShip host)
        {
            if (Combat.DiceRollAttack.Successes > Combat.DiceRollDefence.Successes)
            {
                AddDiceModification(host);
            }
        }

        private void AddDiceModification(GenericShip host)
        {
            GenericAction newAction = new ActionsList.SecondEdition.SecondSisterDiceModification()
            {
                ImageUrl = HostShip.ImageUrl,
                HostShip = host,
            };
            host.AddAvailableDiceModificationOwn(newAction);
        }
    }
}


namespace ActionsList.SecondEdition
{
    public class SecondSisterDiceModification : GenericAction
    {
        public SecondSisterDiceModification()
        {
            Name = DiceModificationName = "Second Sister's ability";
            DiceModificationTiming = DiceModificationTimingType.CompareResults;
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if (Combat.Defender.State.ShieldsCurrent < Combat.DiceRollAttack.Successes) result = 100;
            result -= ActionsHolder.CountEnemiesTargeting(HostShip) * 50;

            return result;
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;

            if (Combat.DiceRollAttack.Successes > Combat.DiceRollDefence.Successes && Combat.Attacker.State.Force >= 2)
            {
                result = true;
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.DiceRollAttack.ChangeAll(DieSide.Success, DieSide.Crit, false);
            Combat.Attacker.State.SpendForce(2, callBack);
        }

    }
}
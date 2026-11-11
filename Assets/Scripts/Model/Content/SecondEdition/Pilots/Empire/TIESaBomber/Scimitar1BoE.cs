using Content;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship
{
    namespace SecondEdition.TIESaBomber
    {
        public class Scimitar1BoE : TIESaBomber
        {
            public Scimitar1BoE() : base()
            {
                PilotInfo = new PilotCardInfo25(
                    "Scimitar 1",
                    "Battle Over Endor",
                    Faction.Imperial,
                    3,
                    4,
                    loadoutValue: 0,
                    isStandardLayout: true,
                    isLimited: true,
                    charges: 2,
                    abilityType: typeof(Abilities.SecondEdition.Scimitar1Ability),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Torpedo,
                        UpgradeType.Device
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );

                MustHaveUpgrades.Add(typeof(Marksmanship));
                MustHaveUpgrades.Add(typeof(NoEscape));
                MustHaveUpgrades.Add(typeof(ProtonTorpedoes));
                MustHaveUpgrades.Add(typeof(IonBombs));

                PilotNameCanonical = "scimitar1-battleoverendor";
            }
        }

        public class Scimitar1BoEXWA : Scimitar1BoE
        {
            public Scimitar1BoEXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 12;
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //After a friendly ship at range 0-3 performs an attack, you may spend 1 charge
    //to acquire a lock on the defender.
    public class Scimitar1Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnAttackFinishGlobal += RegisterTrigger;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnAttackFinishGlobal -= RegisterTrigger;
        }

        private void RegisterTrigger(GenericShip ship)
        {
            var range = new BoardTools.DistanceInfo(HostShip, Combat.Attacker).Range;

            if (HostShip.State.Charges > 0
                && Tools.IsFriendly(Combat.Attacker, HostShip)
                && range >= 0 && range <= 3)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, AskAcquireLock);
            }
        }

        private void AskAcquireLock(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostShip.PilotInfo.PilotName,
                AlwaysUseByDefault,
                AcquireLock,
                descriptionLong: "Do you want to acquire a lock on the defender?",
                imageHolder: HostShip
            );
        }

        private void AcquireLock(object sender, EventArgs e)
        {
            Messages.ShowInfo(HostName + ": Acquires lock on " + Combat.Defender.PilotInfo.PilotName);
            ActionsHolder.AcquireTargetLock(HostShip, Combat.Defender, CleanUp, CleanUp);
        }

        private void CleanUp()
        {
            HostShip.SpendCharge();
            SubPhases.DecisionSubPhase.ConfirmDecision();
        }
    }
}
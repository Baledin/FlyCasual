using ActionsList;
using Bombs;
using Content;
using System;
using System.Collections.Generic;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship
{
    namespace SecondEdition.TIESaBomber
    {
        public class Scimitar3BoE : TIESaBomber
        {
            public Scimitar3BoE()
            {
                PilotInfo = new PilotCardInfo25(
                    "Scimitar 3",
                    "Battle Over Endor",
                    Faction.Imperial,
                    4,
                    4,
                    0,
                    tags: new List<Tags>
                    {
                        Tags.Tie,
                    },
                    isStandardLayout: true,
                    isLimited: true,
                    charges: 2,
                    abilityType: typeof(Abilities.SecondEdition.Scimitar3Ability),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Torpedo,
                        UpgradeType.Device
                    },
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );

                MustHaveUpgrades.Add(typeof(NoEscape));
                MustHaveUpgrades.Add(typeof(PartingGift));
                MustHaveUpgrades.Add(typeof(ProtonTorpedoes));
                MustHaveUpgrades.Add(typeof(ProtonBombs));

                PilotNameCanonical = "scimitar3-battleoverendor";
            }
        }

        public class Scimitar3BoEXWA : Scimitar3BoE
        {
            public Scimitar3BoEXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 10;
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you drop a bomb, you may spend 1 charge to perform a Boost action.
    public class Scimitar3Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnBombWasDropped += OnDeviceDropped;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnBombWasDropped -= OnDeviceDropped;
        }

        private void OnDeviceDropped()
        {
            if (HostShip.State.Charges > 0 && BombsManager.CurrentDevice.UpgradeInfo.SubType == UpgradeSubType.Bomb)
            {
                RegisterAbilityTrigger(TriggerTypes.OnBombWasDropped, AskUseAbility);
            }

        }

        private void AskUseAbility(object sender, EventArgs e)
        {
            HostShip.BeforeActionIsPerformed += RegisterSpendChargeTrigger;
            HostShip.AskPerformFreeAction(
                new BoostAction(),
                CleanUp,
                HostShip.PilotInfo.PilotName,
                "After you drop a bomb, you may spend 1 Charge to perform a Boost action.",
                HostShip
            );
        }

        private void RegisterSpendChargeTrigger(GenericAction action, ref bool isFreeAction)
        {
            HostShip.BeforeActionIsPerformed -= RegisterSpendChargeTrigger;
            RegisterAbilityTrigger(
                TriggerTypes.OnFreeAction,
                delegate
                {
                    HostShip.SpendCharge();
                    Triggers.FinishTrigger();
                }
            );
        }

        private void CleanUp()
        {
            HostShip.BeforeActionIsPerformed -= RegisterSpendChargeTrigger;
            Triggers.FinishTrigger();
        }
    }
}
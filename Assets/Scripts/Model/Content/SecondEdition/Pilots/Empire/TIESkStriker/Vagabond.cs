using Bombs;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESkStriker
    {
        public class Vagabond : TIESkStriker
        {
            public Vagabond() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Vagabond\"",
                    "Destitute Demolitionist",
                    Faction.Imperial,
                    2,
                    4,
                    12,
                    isLimited: true,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Gunner,
                        UpgradeType.Device,
                        UpgradeType.Device,
                        UpgradeType.Modification
                    },
                    abilityType: typeof(Abilities.SecondEdition.VagabondAbility),
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );
            }
        }

        public class VagabondXWA : Vagabond
        {
            public VagabondXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 9;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 16;
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
                (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
                {
                    UpgradeType.Gunner,
                    UpgradeType.Modification,
                    UpgradeType.Device,
                };
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you fully execute a maneuver using your Adaptive Ailerons, if you are not stressed, you may drop 1 device
    public class VagabondAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully -= RegisterAbility;
        }

        private void RegisterAbility(GenericShip ship)
        {
            if (HostShip.AssignedManeuver.GrantedBy == "Ailerons")
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskToDropDevice);
            }
        }

        private bool HasAvailableDevice()
        {
            return HostShip.UpgradeBar
                .GetInstalledUpgrades(UpgradeType.Device)
                .Any(device => device.State.Charges > 0);
        }

        private void AskToDropDevice(object sender, EventArgs e)
        {
            if (!HostShip.IsStressed && HasAvailableDevice())
            {
                AskToUseAbility(
                    HostName,
                    NeverUseByDefault,
                    DropBomb,
                    descriptionLong: "Do you want to drop a device?",
                    imageHolder: HostShip
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void DropBomb(object sender, EventArgs e)
        {
            if (!HostShip.IsStressed && HasAvailableDevice())
            {
                BombsManager.RegisterBombDropTriggerIfAvailable(
                    HostShip,
                    TriggerTypes.OnAbilityDirect,
                    onlyDrop: false,
                    isRealDrop: false
                );

                Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, Triggers.FinishTrigger);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }
    }
}
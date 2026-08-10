using Actions;
using ActionsList;
using Content;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.V19TorrentStarfighter
{
    public class Kickback : V19TorrentStarfighter
    {
        public Kickback()
        {
            PilotInfo = new PilotCardInfo25
            (
                "\"Kickback\"",
                "Blue Four",
                Faction.Republic,
                4,
                3,
                8,
                true,
                abilityType: typeof(Abilities.SecondEdition.KickbackAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Modification,
                    UpgradeType.Missile,
                },
                tags: new List<Tags>
                {
                    Tags.Clone
                },
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class KickbackXWA : Kickback
    {
        public KickbackXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 7;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 7;
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
                UpgradeType.Talent,
                UpgradeType.Modification,
                UpgradeType.Modification,
                UpgradeType.Missile,
            };

        }
    }
}

namespace Abilities.SecondEdition
{
    //After you perform a barrel roll action, you may perform a red lock action.
    public class KickbackAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckConditions;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckConditions;
        }

        private void CheckConditions(GenericAction action)
        {
            if (action is BarrelRollAction)
            {
                HostShip.OnActionDecisionSubphaseEnd += PerformLockAction;
            }
        }

        private void PerformLockAction(GenericShip ship)
        {
            HostShip.OnActionDecisionSubphaseEnd -= PerformLockAction;

            RegisterAbilityTrigger(TriggerTypes.OnFreeAction, AskPerformBoostAction);
        }

        private void AskPerformBoostAction(object sender, System.EventArgs e)
        {
            HostShip.AskPerformFreeAction(
                new TargetLockAction() { Color = ActionColor.Red },
                Triggers.FinishTrigger,
                HostShip.PilotInfo.PilotName,
                "After you perform a Barrel Roll action, you may perform a red Lock action",
                HostShip
            );
        }
    }
}

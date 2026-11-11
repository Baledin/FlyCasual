using Abilities.SecondEdition;
using Actions;
using ActionsList;
using Content;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.TIEDDefender
{
    public class ColonelJendonBoE : TIEDDefender
    {
        public ColonelJendonBoE() : base()
        {
            PilotInfo = new PilotCardInfo25(
                "Colonel Jendon",
                "Battle Over Endor",
                Faction.Imperial,
                6,
                7,
                loadoutValue: 0,
                tags: new List<Tags>
                {
                    Tags.Tie
                },
                isStandardLayout: true,
                isLimited: true,
                abilityType: typeof(ColonelJendonBattleOverEndorAbility),
                extraUpgradeIcons: new List<UpgradeType>()
                {
                    UpgradeType.Talent,
                    UpgradeType.Talent,
                    UpgradeType.Modification,
                    UpgradeType.Cannon,
                    UpgradeType.Cannon
                },
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            ShipInfo.UpgradeIcons.Upgrades.Remove(UpgradeType.Configuration);
            ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(EvadeAction), typeof(BarrelRollAction)));

            FullThrottleAbility oldAbility = (FullThrottleAbility)ShipAbilities.First(n => n.GetType() == typeof(FullThrottleAbility));
            ShipAbilities.Remove(oldAbility);
            ShipAbilities.Add(new ChissEngineeringAbility());

            MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.NoEscape));
            MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.PushTheLimit));
            MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.ProtonCannons));
            MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.ComputerAssistedHandling));

            PilotNameCanonical = "coloneljendon-battleoverendor";
        }
    }

    public class ColonelJendonBoEXWA : ColonelJendonBoE
    {
        public ColonelJendonBoEXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 20;
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}


namespace Abilities.SecondEdition
{
    public class ColonelJendonBattleOverEndorAbility : GenericAbility
    {
        //While you defend, if you are not shielded, you may change 1 of your blank results to a Focus result.
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostShip.PilotInfo.PilotName,
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Change,
                1,
                sidesCanBeSelected: new List<DieSide>() { DieSide.Blank },
                sideCanBeChangedTo: DieSide.Focus
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private bool IsAvailable()
        {
            return Combat.AttackStep == CombatStep.Defence
                && HostShip.State.ShieldsCurrent == 0
                && Combat.DiceRollDefence.Blanks > 0;
        }

        private int GetAiPriority()
        {
            return 100;
        }
    }
}
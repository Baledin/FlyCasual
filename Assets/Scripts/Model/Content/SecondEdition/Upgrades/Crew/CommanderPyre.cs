using Content;
using Ship;
using SubPhases;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using UnityEngine;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class CommanderPyre : GenericUpgrade
    {
        public CommanderPyre() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Commander Pyre",
                UpgradeType.Crew,
                cost: 5,
                isLimited: true,
                restriction: new FactionRestriction(Faction.FirstOrder),
                abilityType: typeof(Abilities.SecondEdition.CommanderPyreAbility),
                legalityInfo: new List<Legality>
                {
                    Legality.StandardBanned,
                    Legality.ExtendedLegal
                }
            );

            Avatar = new AvatarInfo(
                Faction.FirstOrder,
                new Vector2(317, 2)
            );
        }
    }

    public class CommanderPyreXWA : CommanderPyre
    {
        public CommanderPyreXWA() : base()
        {
            UpgradeInfo.Cost = 10;
            UpgradeInfo.LegalityInfo = new() { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    //Setup: After placing forces, choose an enemy ship. It gains 2 stress tokens.
    //While you defend, if the attacker is stressed, you may reroll 1 defense die.
    public class CommanderPyreAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnSetupEnd += RegisterAbility;
            AddDiceModification(
                HostName,
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Reroll,
                1
                );
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnSetupEnd -= RegisterAbility;
            RemoveDiceModification();
        }

        private int GetAiPriority()
        {
            return 90;
        }

        private bool IsAvailable()
        {
            return Combat.Defender == HostShip
                && Combat.AttackStep == CombatStep.Defence
                && Combat.Attacker.IsStressed;
        }

        protected void RegisterAbility()
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = HostShip.PilotInfo.PilotName,
                TriggerType = TriggerTypes.OnSetupEnd,
                TriggerOwner = HostShip.Owner.PlayerNo,
                EventHandler = SelectTarget
            });
        }

        private void SelectTarget(object Sender, System.EventArgs e)
        {
            SelectTargetForAbility(
                  AssignStress,
                  CheckRequirements,
                  GetAiPriority,
                  HostShip.Owner.PlayerNo,
                  HostName,
                  "Assign 2 stress tokens to 1 enemy ship",
                  HostUpgrade,
                  showSkipButton: false
            );
        }

        protected virtual void AssignStress()
        {
            TargetShip.Tokens.AssignTokens(() => new StressToken(TargetShip), 2, SelectShipSubPhase.FinishSelection);
        }

        protected virtual bool CheckRequirements(GenericShip ship)
        {
            return ship.Owner != HostShip.Owner;
        }

        private int GetAiPriority(GenericShip ship)
        {
            return ship.PilotInfo.Cost + ship.UpgradeBar.GetUpgradesOnlyFaceup().Sum(n => n.UpgradeInfo.Cost);
        }
    }
}
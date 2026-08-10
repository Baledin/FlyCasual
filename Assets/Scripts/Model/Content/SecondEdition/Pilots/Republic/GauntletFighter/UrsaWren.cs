using Abilities.SecondEdition;
using Content;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace Ship.SecondEdition.GauntletFighter
{
    public class UrsaWren : GauntletFighter
    {
        public UrsaWren() : base()
        {
            PilotInfo = new PilotCardInfo25(
                pilotName: "Ursa Wren",
                pilotTitle: "Countess of Clan Wren",
                faction: Faction.Republic,
                initiative: 3,
                cost: 15,
                loadoutValue: 17,
                isLimited: true,
                extraUpgradeIcons: new List<UpgradeType>()
                {
                    UpgradeType.Talent,
                    UpgradeType.Crew,
                    UpgradeType.Gunner,
                    UpgradeType.Illicit,
                    UpgradeType.Modification,
                    UpgradeType.Modification,
                    UpgradeType.Device,
                    UpgradeType.Configuration
                },
                tags: new List<Tags>()
                {
                    Tags.Mandalorian
                },
                abilityType: typeof(UrsaWrenGauntletAbility),
                legality: new List<Legality>() { Legality.XWA }
            );

            PilotNameCanonical = "ursawren-legendsandrelics";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class UrsaWrenGauntletAbility : GenericAbility
    {
        // After you acquire a lock on an enemy ship, if there are no enemy ships at range 0-1 of you,
        // you may gain 1 reinforce token.

        public override void ActivateAbility()
        {
            HostShip.OnTargetLockIsAcquired += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnTargetLockIsAcquired += CheckAbility;
        }

        private void CheckAbility(ITargetLockable target)
        {
            if (!Roster.AllShips.Values.Any(t => Tools.IsAnotherTeam(HostShip, t) && HostShip.GetRangeToShip(t) < 2))
            {
                RegisterAbilityTrigger(TriggerTypes.OnTargetLockIsAcquired, UseAbility);
            }
        }

        private void UseAbility(object sender, EventArgs e)
        {
            if (alwaysUseAbility)
            {
                AssignReinforceToken(Triggers.FinishTrigger);
            }
            else
            {
                AskToUseAbility(
                    HostShip.PilotInfo.PilotName,
                    AlwaysUseByDefault,
                    AssignReinforceToken,
                    callback: Triggers.FinishTrigger,
                    showAlwaysUseOption: true,
                    descriptionLong: "Gain 1 reinforce token?"
                );
            }
        }

        private void AssignReinforceToken(object sender, EventArgs e)
        {
            AssignReinforceToken(DecisionSubPhase.ConfirmDecision);
        }

        private void AssignReinforceToken(Action callback)
        {
            FreeReinforceSubPhase decisionSubphase = (FreeReinforceSubPhase)Phases.StartTemporarySubPhaseNew(
                HostShip.PilotInfo.PilotName,
                typeof(FreeReinforceSubPhase),
                callback
            );

            decisionSubphase.DescriptionShort = "Reinforce: Select a side";
            decisionSubphase.RequiredPlayer = HostShip.Owner.PlayerNo;

            decisionSubphase.AddDecision(
                "Fore side",
                delegate { HostShip.Tokens.AssignToken(typeof(ReinforceForeToken), DecisionSubPhase.ConfirmDecision); },
                isCentered: true
            );

            decisionSubphase.AddDecision(
                "Aft side",
                delegate { HostShip.Tokens.AssignToken(typeof(ReinforceAftToken), DecisionSubPhase.ConfirmDecision); },
                isCentered: true
            );

            decisionSubphase.DefaultDecisionName = GetDefaultDecision();

            decisionSubphase.Start();
        }

        private string GetDefaultDecision()
        {
            int resultFore = 25 + 30 * ActionsHolder.CountEnemiesTargeting(Selection.ThisShip, 1);
            int resultAft = 25 + 30 * ActionsHolder.CountEnemiesTargeting(Selection.ThisShip, -1);

            return resultFore >= resultAft ? "Fore side" : "Aft side";
        }

        private class FreeReinforceSubPhase : DecisionSubPhase { }
    }
}
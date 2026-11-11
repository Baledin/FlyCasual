using Actions;
using ActionsList;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship.SecondEdition.T65XWing
{
    public class KendyIdeleBoE : T65XWingBoE
    {
        public KendyIdeleBoE() : base()
        {
            PilotInfo = new Ship.PilotCardInfo25
            (
                "Kendy Idele",
                "Battle Over Endor",
                Faction.Rebel,
                4,
                4,
                0,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.KendyIdeleBoEAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Missile,
                    UpgradeType.Astromech,
                    UpgradeType.Modification
                },
                tags: new List<Tags>
                {
                    Tags.XWing
                },
                isStandardLayout: true,
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            MustHaveUpgrades.Add(typeof(ItsATrap));
            MustHaveUpgrades.Add(typeof(IonMissiles));
            MustHaveUpgrades.Add(typeof(ModifiedR4PUnit));
            MustHaveUpgrades.Add(typeof(ChaffParticlesBoE));

            PilotNameCanonical = "kendyidele-battleoverendor";
        }
    }

    public class KendyIdeleBoEXWA : KendyIdeleBoE
    {
        public KendyIdeleBoEXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 10;
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    // After you spend a green token, you may choose a ship at range 1-3 and gain a strain token. If you do, that ship may perform a red focus or red evade action.

    public class KendyIdeleBoEAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnTokenIsSpent += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnTokenIsSpent -= RegisterAbility;
        }

        private void RegisterAbility(GenericShip ship, GenericToken tokenType)
        {
            if (tokenType.TokenColor == TokenColors.Green)
            {
                RegisterAbilityTrigger(TriggerTypes.OnTokenIsSpent, SelectTargetForAbility);
            }
        }

        private void SelectTargetForAbility(object sender, EventArgs e)
        {
            if (HasTargetsForAbility())
            {
                SelectTargetForAbility(
                    GrantAction,
                    FilterTargets,
                    GetAiPriority,
                    HostShip.Owner.PlayerNo,
                    HostShip.PilotInfo.PilotName,
                    "You may choose a friendly ship at range 1-3 and gain a strain token. If you do, that ship may perform a red Focus or red Evade action.",
                    HostShip
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private int GetAiPriority(GenericShip ship)
        {
            int result = 0;

            result += NeedTokenPriority(ship);
            result += ship.PilotInfo.Cost + ship.UpgradeBar.GetUpgradesOnlyFaceup().Sum(n => n.UpgradeInfo.Cost);

            return result;
        }

        private int NeedTokenPriority(GenericShip ship)
        {
            if (!ship.Tokens.HasToken(typeof(FocusToken))) return 100;

            if (ship.ActionBar.HasAction(typeof(EvadeAction)) && !ship.Tokens.HasToken(typeof(EvadeToken))) return 50;

            return 0;
        }

        private void GrantAction()
        {
            TargetShip.BeforeActionIsPerformed += PayStrainCost;

            SelectShipSubPhase.FinishSelectionNoCallback();
            Selection.ThisShip = TargetShip;

            TargetShip.AskPerformFreeAction(
                new List<GenericAction>() {
                    new FocusAction() { Color = ActionColor.Red },
                    new EvadeAction() { Color = ActionColor.Red }
                },
                delegate
                {
                    Selection.ThisShip = HostShip;
                    TargetShip.BeforeActionIsPerformed -= PayStrainCost;
                    Triggers.FinishTrigger();
                },
                TargetShip.PilotInfo.PilotName,
                "You may perform an action, even if you are stressed.",
                HostShip
            );
        }

        private void PayStrainCost(GenericAction action, ref bool isFreeAction)
        {
            RegisterAbilityTrigger(TriggerTypes.BeforeActionIsPerformed, GainStrain);
        }

        private void GainStrain(object sender, EventArgs e)
        {
            HostShip.Tokens.AssignToken(typeof(StrainToken), Triggers.FinishTrigger);
        }

        private bool HasTargetsForAbility()
        {
            foreach (GenericShip ship in HostShip.Owner.Ships.Values)
            {
                if (FilterTargets(ship)) return true;
            }

            return false;
        }

        private bool FilterTargets(GenericShip ship)
        {
            return BoardTools.Board.GetShipsAtRange(HostShip, new UnityEngine.Vector2(1, 3), Team.Type.Friendly).Contains(ship);
        }
    }
}
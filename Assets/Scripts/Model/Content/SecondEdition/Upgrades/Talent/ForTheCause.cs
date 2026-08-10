using ActionsList;
using BoardTools;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using UnityEngine;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class ForTheCause : GenericUpgrade
    {
        public ForTheCause() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "For The Cause",
                UpgradeType.Talent,
                cost: 0,
                abilityType: typeof(Abilities.SecondEdition.ForTheCauseAbility),
                legalityInfo: new List<Legality> { Legality.XWA }
            );

            IsHidden = true;
            ImageUrl = "https://infinitearenas.com/xw2xwa/images/pilots/jaycristubbs-evacuationofdqar.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    // While you defend or perform an attack, you may spend 1 non-blank result to choose a friendly strained or depleted ship at range 1-2.
    // That ship may remove 1 strain or deplete token.
    public class ForTheCauseAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += ForTheCauseEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= ForTheCauseEffect;
        }

        private void ForTheCauseEffect(GenericShip host)
        {
            GenericAction newAction = new ActionsList.SecondEdition.ForTheCauseEffect()
            {
                HostShip = host,
                ImageUrl = HostUpgrade.ImageUrl,
                DoDiceModification = UseAbilityAction
            };
            host.AddAvailableDiceModificationOwn(newAction);
        }

        private void UseAbilityAction(Action callback)
        {
            SelectTargetForAbility(
                AskRemoveStrainOrDeplete,
                AnotherFriendlyShipInRange,
                ShipTargetAiPriority,
                HostShip.Owner.PlayerNo,
                HostName,
                "Choose a ship, that ship will remove one strain or deplete token.",
                HostUpgrade,
                true,
                callback
            );
        }

        private bool AnotherFriendlyShipInRange(GenericShip ship)
        {
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.AnyFriendly })
                && FilterTargetsByRange(ship, 1, 2)
                && (ship.Tokens.HasToken<StrainToken>()
                    || ship.Tokens.HasToken<DepleteToken>());
        }

        private void AskRemoveStrainOrDeplete()
        {            
            if (TargetShip.Tokens.HasToken<DepleteToken>() && TargetShip.Tokens.HasToken<StrainToken>())
            {
                ForTheCauseTokenDecisionSubPhase subphase = Phases.StartTemporarySubPhaseNew<ForTheCauseTokenDecisionSubPhase>(
                    $"{HostUpgrade.UpgradeInfo.Name} Token Decision SubPhase",
                    delegate
                    {
                        Phases.FinishSubPhase(typeof(ForTheCauseTokenDecisionSubPhase));
                        SelectShipSubPhase.FinishSelection();
                    }
                );

                subphase.DescriptionShort = $"{HostUpgrade.UpgradeInfo.Name} Decision";
                subphase.DescriptionLong = $"Select one token to remove from {TargetShip.PilotInfo.PilotName} (ID: {TargetShip.ShipId}).";

                subphase.AddDecision("Deplete", RemoveDeplete);
                subphase.AddDecision("Strain", RemoveStrain);

                subphase.DefaultDecisionName = AiChooseTokenToRemove(TargetShip);
                subphase.ShowSkipButton = false;

                subphase.Start();
            }
            else if (TargetShip.Tokens.HasToken<DepleteToken>())
            {
                TargetShip.Tokens.RemoveToken(typeof(DepleteToken),SelectShipSubPhase.FinishSelection);
            }
            else
            {
                TargetShip.Tokens.RemoveToken(typeof(StrainToken),SelectShipSubPhase.FinishSelection);
            }
        }

        private string AiChooseTokenToRemove(GenericShip ship)
        {
            bool shipHasTarget = ship.HasCombatActivation && HasAnyShipInArc(ship);
            if (shipHasTarget && ship.Tokens.HasToken<DepleteToken>())
            {
                return "Deplete";
            }
            return "Strain";
        }

        private void RemoveStrain(object sender, EventArgs e)
        {
            TargetShip.Tokens.RemoveToken(typeof(StrainToken),DecisionSubPhase.ConfirmDecision);
        }

        private void RemoveDeplete(object sender, EventArgs e)
        {
            TargetShip.Tokens.RemoveToken(typeof(DepleteToken),DecisionSubPhase.ConfirmDecision);
        }

        private int ShipTargetAiPriority(GenericShip ship)
        {
            if (!(ship.Tokens.HasToken<DepleteToken>() || ship.Tokens.HasToken<StrainToken>()))
            {
                return 0;
            }
            bool shipHasTarget = ship.HasCombatActivation && HasAnyShipInArc(ship);
            if (shipHasTarget && ship.Tokens.HasToken<DepleteToken>())
            {
                return 100;
            }
            bool shipIsTarget = AnyShipCanFireOnThis(ship);
            if (shipIsTarget && ship.Tokens.HasToken<StrainToken>())
            {
                return 50 + ship.PilotInfo.Cost;
            }
            if (ship.Tokens.HasToken<StressToken>())
            {
                // Token will be cleared by a blue maneuver next turn.
                return 10;
            }
            return 20;
        }
        
        private static bool HasAnyShipInArc(GenericShip ship)
        {
            return ship.GetAllWeapons().Any(weapon => Roster.AllShips.Values.Any(a => Tools.IsAnotherTeam(ship, a) && new ShotInfo(ship,a,weapon).InArc));
        }

        private static bool AnyShipCanFireOnThis(GenericShip ship)
        {
            return Roster.AllShips.Values.Any(a => Tools.IsAnotherTeam(ship, a) && a.HasCombatActivation && a.GetAllWeapons().Any(weapon => new ShotInfo(a,ship,weapon).InArc));
        }
        
        private class ForTheCauseTokenDecisionSubPhase : DecisionSubPhase { }
    }
}

namespace ActionsList.SecondEdition
{
    public class ForTheCauseEffect : GenericAction
    {
        public ForTheCauseEffect() : base()
        {
            Name = DiceModificationName =  "For The Cause";
        }

        public override void ActionEffect(Action callBack)
        {
            if (Combat.CurrentDiceRoll.HasResult(DieSide.Focus)) {
                Combat.CurrentDiceRoll.RemoveType(DieSide.Focus);
            } 
            else
            if (Combat.CurrentDiceRoll.HasResult(DieSide.Success)) {
                Combat.CurrentDiceRoll.RemoveType(DieSide.Success);
            }
            else
            {
                Combat.CurrentDiceRoll.RemoveType(DieSide.Crit);
            }
            base.ActionEffect(callBack);
        }

        public override bool IsDiceModificationAvailable()
        {
            return Combat.CurrentDiceRoll.HasResult(DieSide.Focus) || Combat.CurrentDiceRoll.HasResult(DieSide.Crit) || Combat.CurrentDiceRoll.HasResult(DieSide.Success);
        }

        public override int GetDiceModificationPriority()
        {
            if (Board.GetShipsAtRange(HostShip, new Vector2(1,2),Team.Type.Friendly).Any(a => a.Tokens.HasToken<StrainToken>() || a.Tokens.HasToken<DepleteToken>())
                && (Combat.CurrentDiceRoll.HasResult(DieSide.Focus)
                || Combat.Defender == HostShip
                    && Combat.DiceRollDefence.Successes > Combat.DiceRollAttack.SuccessesCancelable))
                // Don't spend extra hits for attack unless also checking for dice-adding abilities.
            {
                return 10;
            }
            return 0;
        }
    }
}

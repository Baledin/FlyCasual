using Actions;
using ActionsList;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using UnityEngine;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class K2SO : GenericUpgrade
    {
        public K2SO() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "K-2SO",
                UpgradeType.Crew,
                cost: 6,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Rebel),
                addActions: new List<ActionInfo> { new(typeof(CalculateAction)), new(typeof(JamAction)) },
                abilityType: typeof(Abilities.SecondEdition.K2SOAbility),
                legalityInfo: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            Avatar = new AvatarInfo(
                Faction.Rebel,
                new Vector2(300, 2)
            );
        }
    }

    public class K2SOXWA : K2SO
    {
        public K2SOXWA() : base()
        {
            UpgradeInfo.Cost = 8;
            UpgradeInfo.LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class K2SOAbility : GenericAbility
    {
        //During the System Phase, you may choose a friendly ship at range 0-3. That ship gains 1 calculate and 1 stress token
        public override void ActivateAbility()
        {
            HostShip.OnSystemsPhaseStart += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnSystemsPhaseStart -= RegisterAbility;
        }

        private void RegisterAbility(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnSystemsPhaseStart, Ability);
        }

        protected bool FilterAbilityTargets(GenericShip ship)
        {
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.This, TargetTypes.OtherFriendly }) && FilterTargetsByRange(ship, 0, 3);
        }

        private void Ability(object sender, EventArgs e)
        {
            SelectTargetForAbility(
                AssignToken,
                FilterAbilityTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                HostName,
                "You may choose a friendly ship at range 0-3. That ship gains 1 calculate and 1 stress token.",
                imageSource: HostUpgrade
            );
        }

        private int GetAiPriority(GenericShip ship)
        {
            //Prioritize high cost pilots that have planned a blue maneuver or are ionized
            int priority = ship.PilotInfo.Cost;

            if ((ship.AssignedManeuver != null && ship.AssignedManeuver.ColorComplexity == Movement.MovementComplexity.Easy) || ship.State.IsIonized)
                priority += 200;

            return priority;
        }

        private void AssignToken()
        {
            Messages.ShowInfo("K-2SO: " + TargetShip.PilotInfo.PilotName + " gains 1 calculate and 1 stress token");

            TargetShip.Tokens.AssignToken(typeof(CalculateToken), () =>
            {
                TargetShip.Tokens.AssignToken(typeof(StressToken), SelectShipSubPhase.FinishSelection);
            });
        }
    }
}
using ActionsList;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship.SecondEdition.YT2400LightFreighter
{
    public class LeeboSL : YT2400LightFreighter
    {
        public LeeboSL() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Leebo",
                "He Thinks He's Funny",
                Faction.Rebel,
                3,
                6,
                0,
                isLimited: true,
                isStandardLayout: true,
                abilityType: typeof(Abilities.SecondEdition.LeeboSLAbility),
                tags: new List<Tags>
                {
                    Tags.Droid,
                    Tags.Freighter
                },
                extraUpgradeIcons: new List<UpgradeType>()
                {
                    UpgradeType.Talent,
                    UpgradeType.Missile,
                    UpgradeType.Title
                },
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            ImageUrl = "https://infinitearenas.com/xw2xwa/images/quickbuilds/leebo-hethinkshesfunny-rebelalliance.png";
            PilotNameCanonical = "leebo-hethinkshesfunny-rebelalliance";

            ShipInfo.ActionIcons.SwitchToDroidActions();

            //MustHaveUpgrades.Add(typeof(EfficientProcessing));
            MustHaveUpgrades.Add(typeof(SeekerMissiles));
            MustHaveUpgrades.Add(typeof(Outrider));
        }
    }

    public class LeeboSLXWA : LeeboSL
    {
        public LeeboSLXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 15;
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }

    public class LeeboSLScum : LeeboSL
    {
        public LeeboSLScum() : base()
        {
            PilotInfo.Faction = Faction.Scum;
            ImageUrl = "https://infinitearenas.com/xw2xwa/images/quickbuilds/leebo-hethinkshesfunny-scumandvillainy.png";
            PilotNameCanonical = "leebo-hethinkshesfunny-scumandvillainy";
        }
    }

    public class LeeboSLScumXWA : LeeboSLScum
    {
        public LeeboSLScumXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 15;
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LeeboSLAbility : GenericAbility
    {
        // At the end of the Engagement Phase, you may spend a calculate token to acquire a lock on an enemy ship at range 2-3.

        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseEnd_Triggers += AskGetLock;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseEnd_Triggers -= AskGetLock;
        }

        private void AskGetLock()
        {
            if(HostShip.Tokens.HasToken<CalculateToken>() && HostShip.Owner.AnotherPlayer.Ships.Values.Any(s => FilterTargets(s)))
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseEnd, AskAcquireLock);
            }
        }

        private void AskAcquireLock(object sender, EventArgs e)
        {
            HostShip.OnActionIsPerformed += PayCost;

            SelectTargetForAbility(
                new TargetLockAction(),
                filterTargets: FilterTargets,
                getAiPriority: GetAiPriority,
                subphaseOwnerPlayerNo: HostShip.Owner.PlayerNo,
                name: HostShip.PilotInfo.PilotName,
                description: "You may spend a calculate token to acquire a lock on an enemy ship at range 2-3.",
                imageSource: HostShip,
                callback: Triggers.FinishTrigger,
                onSkip: CleanUp
            );
        }

        private bool FilterTargets(GenericShip ship)
        {
            return HostShip.GetRangeToShip(ship) is >= 2 and <= 3;
        }

        private void PayCost(GenericAction action)
        {
            HostShip.Tokens.SpendToken(typeof(CalculateToken), CleanUp);
        }

        private int GetAiPriority(GenericShip ship)
        {
            return 100 - HostShip.GetRangeToShip(ship);
        }

        private void CleanUp()
        {
            HostShip.OnActionIsPerformed -= PayCost;
        }
    }
}
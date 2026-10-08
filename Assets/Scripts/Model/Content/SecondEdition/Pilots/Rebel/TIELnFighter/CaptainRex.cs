using Conditions;
using Content;
using Ship;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship.SecondEdition.TIELnFighter
{
    public class CaptainRex : TIELnFighter
    {
        public CaptainRex() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Captain Rex",
                "Clone Wars Veteran",
                Faction.Rebel,
                2,
                3,
                8,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.CaptainRexPilotAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Talent,
                    UpgradeType.Modification
                },
                tags: new List<Tags>
                {
                    Tags.Clone,
                    Tags.Tie
                },
                seImageNumber: 48,
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            ModelInfo.ModelName = "TIE Fighter Rebel";
            ModelInfo.SkinName = "Rebel";
        }
    }

    public class CaptainRexXWA : CaptainRex
    {
        public CaptainRexXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 6;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 4;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>()
            {
                UpgradeType.Talent,
                UpgradeType.Modification,
                UpgradeType.Modification
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CaptainRexPilotAbility : GenericAbility
    {
        private CaptainRexCondition AssignedCondition;
        private GenericShip SufferedShip;
        private bool AttackedThisTurn;

        public override void ActivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker += AssignConditionToDefender;

            HostShip.OnAttackStartAsDefender += RemoveCondition;
            Phases.Events.OnCombatPhaseEnd_NoTriggers += CheckAttacked;
            HostShip.OnShipIsDestroyed += RemoveConditionOfShip;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker -= AssignConditionToDefender;

            HostShip.OnAttackStartAsDefender -= RemoveCondition;
            Phases.Events.OnCombatPhaseEnd_NoTriggers -= CheckAttacked;
            HostShip.OnShipIsDestroyed -= RemoveConditionOfShip;
        }

        private void AssignConditionToDefender(GenericShip ship)
        {
            RemoveCondition();
            Messages.ShowInfo("Suppressive Fire has been assigned by Captain Rex");

            AssignedCondition = new CaptainRexCondition(Combat.Defender) { Source = HostShip };
            SufferedShip = Combat.Defender;
            SufferedShip.Tokens.AssignCondition(AssignedCondition);

            AttackedThisTurn = true;
        }

        private void RemoveCondition()
        {
            if (SufferedShip != null)
            {
                Messages.ShowInfo("Suppressive Fire has been removed from " + SufferedShip.PilotInfo.PilotName);

                SufferedShip.Tokens.RemoveCondition(AssignedCondition);
                SufferedShip = null;
                AssignedCondition = null;
            }
        }

        private void RemoveConditionOfShip(GenericShip ship, bool isForced)
        {
            RemoveCondition();
        }

        private void CheckAttacked()
        {
            if (!AttackedThisTurn) RemoveCondition();
            AttackedThisTurn = false;
        }
    }
}

namespace Conditions
{
    public class CaptainRexCondition : GenericToken
    {
        public GenericShip Source;

        public CaptainRexCondition(GenericShip host) : base(host)
        {
            Name = ImageName = "Debuff Token";
            Tooltip = "https://infinitearenas.com/xw2/images/conditions/suppressivefire.png";

            Temporary = false;
        }

        public override void WhenAssigned()
        {
            Host.AfterGotNumberOfAttackDice += CheckAbility;
        }

        public override void WhenRemoved()
        {
            Host.AfterGotNumberOfAttackDice -= CheckAbility;
        }

        private void CheckAbility(ref int count)
        {
            if (Combat.Defender != Source)
            {
                Messages.ShowInfo("Captain Rex - Suppressive Fire: Since the attacker is not attacking Captain Rex, it rolls 1 fewer attack die");
                count--;
            }
        }
    }
}

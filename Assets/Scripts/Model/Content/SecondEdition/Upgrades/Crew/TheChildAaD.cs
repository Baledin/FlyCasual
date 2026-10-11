using Content;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class TheChildAaD : GenericUpgrade
    {
        public TheChildAaD() : base()
        {
            UpgradeInfo = new UpgradeCardInfo
            (
                "The Child",
                UpgradeType.Crew,
                cost: 7,
                abilityType: typeof(Abilities.SecondEdition.TheChildAaDAbility),
                restriction: new FactionRestriction(Faction.Scum),
                addForce: 2,
                legalityInfo: new List<Legality> { Legality.XWA }
            );
            IsHidden = true;
        }
    }
}

namespace Abilities.SecondEdition
{
    public class TheChildAaDAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCheckForceRecurring += DenyForceRecurring;
            HostShip.OnAttackFinishAsDefender += CheckForceRegenAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCheckForceRecurring -= DenyForceRecurring;
            HostShip.OnAttackFinishAsDefender -= CheckForceRegenAbility;
        }

        private void DenyForceRecurring(ref bool isForceRecurring)
        {
            isForceRecurring = false;
        }

        private void CheckForceRegenAbility(GenericShip ship)
        {
            if (Combat.DamageInfo.IsDefenderSufferedDamage && HostShip.State.Force < HostShip.State.MaxForce)
            {
                Triggers.RegisterTrigger(
                        new Trigger()
                        {
                            Name = "Recover Force",
                            TriggerType = TriggerTypes.OnAttackFinish,
                            TriggerOwner = HostShip.Owner.PlayerNo,
                            EventHandler = ForceRegen
                        }
                    );
            }
        }

        private void ForceRegen(object sender, EventArgs e)
        {
            Messages.ShowInfo($"{HostUpgrade.UpgradeInfo.Name}: {HostShip.PilotInfo.PilotName} recovers 1 Force");
            HostShip.State.RestoreForce();
            Triggers.FinishTrigger();
        }
    }
}
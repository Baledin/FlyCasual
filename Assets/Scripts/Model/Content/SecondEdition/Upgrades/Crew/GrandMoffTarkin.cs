using ActionsList;
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
    public class GrandMoffTarkin : GenericUpgrade
    {
        public GrandMoffTarkin()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Grand Moff Tarkin",
                UpgradeType.Crew,
                cost: 4,
                isLimited: true,
                restrictions: new UpgradeCardRestrictions(
                    new FactionRestriction(Faction.Imperial),
                    new ActionBarRestriction(typeof(TargetLockAction))),
                abilityType: typeof(Abilities.SecondEdition.GrandMoffTarkinAbility),
                seImageNumber: 117,
                charges: 2,
                regensCharges: true,
                legalityInfo: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            Avatar = new AvatarInfo(
                Faction.Imperial,
                new Vector2(436, 10),
                new Vector2(150, 150)
            );
        }
    }

    public class GrandMoffTarkinXWA : GrandMoffTarkin
    {
        public GrandMoffTarkinXWA() : base()
        {
            UpgradeInfo.Cost = 5;
            UpgradeInfo.LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class GrandMoffTarkinAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCheckSystemsAbilityActivation += CheckForAbility;
            HostShip.OnSystemsAbilityActivation += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCheckSystemsAbilityActivation -= CheckForAbility;
            HostShip.OnSystemsAbilityActivation += RegisterAbility;
        }

        private void CheckForAbility(GenericShip ship, ref bool flag)
        {
            if (HostUpgrade.State.Charges >= 2) flag = true;
        }

        private void RegisterAbility(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnSystemsAbilityActivation, AskToUseTarkinAbility);
        }

        private void AskToUseTarkinAbility(object sender, EventArgs e)
        {
            // Check locks here as other abilities may give a lock during system phase
            if (HostShip.Tokens.CountTokensByType<BlueTargetLockToken>() > 0)
            {
                AskToUseAbility(
                    HostUpgrade.UpgradeInfo.Name,
                    AlwaysUseByDefault,
                    UseAbility,
                    dontUseAbility: delegate { DecisionSubPhase.ConfirmDecision(); },
                    descriptionLong: "Do you want to spend 2 Charges? (If you do, each friendly ship may acquire a target lock on a ship that you have locked)",
                    imageHolder: HostUpgrade
                );
            }
            else
            {
                Messages.ShowErrorToHuman($"{HostShip.PilotInfo.PilotName} must have a lock to use {HostUpgrade.UpgradeInfo.Name}'s ability.");
                Triggers.FinishTrigger();
            }
        }

        protected void UseAbility(object sender, EventArgs e)
        {
            List<BlueTargetLockToken> tarkinsLocks = HostShip.Tokens.GetTokens<BlueTargetLockToken>('*');

            ITargetLockable[] targets = tarkinsLocks
                .Where(t => t.OtherTargetLockTokenOwner is GenericShip)
                .Select(t => t.OtherTargetLockTokenOwner)
                .ToArray();

            // Limit the list of friendlies down to those that can actually lock these targets.
            GenericShip[] friendlies = HostShip.Owner.Ships.Values
                .Where(f => targets.Any(t =>
                {
                    int range = t.GetRangeToShip(f);
                    return f != HostShip && f.TargetLockMinRange <= range && f.TargetLockMaxRange >= range;
                }))
                .ToArray();

            if (friendlies.Length == 0)
            {
                Messages.ShowInfo(string.Format($"No friendly ships are at Target Lock range of any of {HostShip.PilotInfo.PilotName}'s targets."));
            }
            else
            {
                HostUpgrade.State.SpendCharges(2);

                foreach (GenericShip friendly in friendlies)
                {
                    Triggers.RegisterTrigger(
                        new()
                        {
                            Name = friendly.PilotInfo.PilotName + " (" + friendly.ShipId + ") can acquire " + HostShip.PilotInfo.PilotName + "'s Target Lock.",
                            TriggerOwner = HostShip.Owner.PlayerNo,
                            TriggerType = TriggerTypes.OnAbilityDirect,
                            EventHandler = AskToAcquireTarkinsLock,
                            Sender = HostShip,
                            EventArgs = new TarkinAbilityEventArgs
                            {
                                tarkinsShip = HostShip,
                                tarkinsFriend = friendly,
                                tarkinsLocks = targets
                            }
                        }
                    );
                }
            }

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, DecisionSubPhase.ConfirmDecision);
        }

        public class TarkinAbilityEventArgs : EventArgs
        {
            public GenericShip tarkinsShip;
            public GenericShip tarkinsFriend;
            public ITargetLockable[] tarkinsLocks;
        }

        protected void AskToAcquireTarkinsLock(object sender, EventArgs e)
        {
            TarkinAbilityEventArgs args = e as TarkinAbilityEventArgs;
            GenericShip tarkinsShip = args.tarkinsShip;
            ITargetLockable[] targets = args.tarkinsLocks;
            GenericShip ship = args.tarkinsFriend;
            string shipName = ship.PilotInfo.PilotName;

            if (!ship.PilotInfo.IsLimited)
            {
                shipName += " (" + ship.ShipId + ")";
            }

            SelectTargetForAbility(
                () => LockTarget(ship, TargetShip),
                t => targets.Contains(t),
                t => GetAiTargetPriority(ship, t),
                HostShip.Owner.PlayerNo,
                HostUpgrade.UpgradeInfo.Name,
                shipName + ": You may lock any of " + tarkinsShip.PilotInfo.PilotName + "'s target(s)",
                HostUpgrade
            );
        }

        protected void LockTarget(GenericShip ship, GenericShip target)
        {
            ActionsHolder.AcquireTargetLock(ship, target, SelectShipSubPhase.FinishSelection, SelectShipSubPhase.FinishSelection, false);
        }

        protected int GetAiTargetPriority(GenericShip subject, GenericShip target)
        {
            bool isInArc = subject.ArcsInfo.Arcs
                .Select(arc => new BoardTools.ShotInfoArc(subject, target, arc))
                .Any(arc => arc.InArc);
            return isInArc ? 1 : 0;
        }
    }
}
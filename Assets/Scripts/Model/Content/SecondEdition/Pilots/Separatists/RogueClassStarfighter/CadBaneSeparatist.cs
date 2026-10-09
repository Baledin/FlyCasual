using ActionsList;
using BoardTools;
using Content;
using Remote;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.RogueClassStarfighter
{
    public class CadBaneSeparatist : RogueClassStarfighter
    {
        public CadBaneSeparatist() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Cad Bane",
                "Needs No Introduction",
                Faction.Separatists,
                4,
                4,
                13,
                isLimited: true,
                charges: 1,
                regensCharges: 1,
                abilityType: typeof(Abilities.SecondEdition.CadBaneSeparatistAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Cannon,
                    UpgradeType.Cannon,
                    UpgradeType.Missile,
                    UpgradeType.Illicit,
                    UpgradeType.Illicit,
                    UpgradeType.Modification,
                    UpgradeType.Title
                },
                tags: new List<Tags>()
                {
                    Tags.BountyHunter
                },
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            PilotNameCanonical = "cadbane-separatistalliance";
        }
    }

    public class CadBaneSeparatistXWA : CadBaneSeparatist
    {
        public CadBaneSeparatistXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 11;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 11;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
                UpgradeType.Talent,
                UpgradeType.Illicit,
                UpgradeType.Modification,
                UpgradeType.Modification,
                UpgradeType.Cannon,
                UpgradeType.Cannon,
                UpgradeType.Missile,
                UpgradeType.Title,
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}


namespace Abilities.SecondEdition
{
    public class CadBaneSeparatistAbility : GenericAbility
    {
        GenericShip destroyedShip;

        public override void ActivateAbility()
        {
            GenericShip.OnShipIsDestroyedGlobal += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnShipIsDestroyedGlobal -= RegisterAbility;
        }
        private void RegisterAbility(GenericShip ship, bool flag)
        {
            // Check if Cad Bane is the one being destroyed--ability states "another ship, exclude remotes"
            if (ship == HostShip || ship is GenericRemote) return;

            destroyedShip = ship;
            RegisterAbilityTrigger(TriggerTypes.OnShipIsDestroyed, PerformAction);
        }

        private void PerformAction(object sender, System.EventArgs e)
        {
            if (HostShip.State.Charges < 1 || Phases.CurrentPhase is not MainPhases.CombatPhase || new DistanceInfo(HostShip, destroyedShip).Range > 3)
            {
                Triggers.FinishTrigger();
                return;
            }


            GenericShip ship = Selection.ThisShip;
            Roster.HighlightPlayer(HostShip.Owner.PlayerNo);
            Selection.ChangeActiveShip(HostShip);

            CameraScript.RestoreCamera();

            HostShip.OnCanPerformActionWhileStressed += TemporaryAllowAnyActionsWhileStressed;
            HostShip.OnCheckCanPerformActionsWhileStressed += TemporaryAllowActionsWhileStressed;
            HostShip.OnActionIsPerformed += DisallowActionsWhileStressed;
            HostShip.OnActionIsSkipped += DisallowActionsWhileStressed;

            List<GenericAction> actions = Selection.ThisShip.GetAvailableActions();

            Messages.ShowInfoToHuman($"{HostName}: you may spend 1 charge to perform an action");

            HostShip.BeforeActionIsPerformed += SpendCharge;

            HostShip.AskPerformFreeAction(
                actions,
                delegate
                {
                    Roster.HighlightPlayer(ship.Owner.PlayerNo);
                    Selection.ChangeActiveShip(ship);
                    Triggers.FinishTrigger();
                },
                HostShip.PilotInfo.PilotName,
                "After another ship at range 0-3 is destroyed, you may spend 1 charge to perform an action, even while stressed",
                HostShip
            );
        }

        private void SpendCharge(GenericAction action, ref bool isFreeAction)
        {
            HostShip.BeforeActionIsPerformed -= SpendCharge;
            HostShip.SpendCharge();
        }

        private void DisallowActionsWhileStressed(GenericAction action)
        {
            DisallowActionsWhileStressed(HostShip);
        }

        private void DisallowActionsWhileStressed(GenericShip ship)
        {
            HostShip.OnCanPerformActionWhileStressed -= TemporaryAllowAnyActionsWhileStressed;
            HostShip.OnCheckCanPerformActionsWhileStressed -= TemporaryAllowActionsWhileStressed;
            HostShip.OnActionIsPerformed -= DisallowActionsWhileStressed;
            HostShip.OnActionIsSkipped -= DisallowActionsWhileStressed;
        }

        private void TemporaryAllowAnyActionsWhileStressed(GenericAction action, ref bool isAllowed)
        {
            isAllowed = true;
        }

        private void TemporaryAllowActionsWhileStressed(ref bool isAllowed)
        {
            isAllowed = true;
        }
    }
}
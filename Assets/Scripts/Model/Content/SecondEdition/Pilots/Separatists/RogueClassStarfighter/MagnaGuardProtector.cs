using Abilities.SecondEdition;
using BoardTools;
using Conditions;
using Content;
using Players;
using Ship;
using SubPhases;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace Ship.SecondEdition.RogueClassStarfighter
{
    public class MagnaGuardProtector : RogueClassStarfighter
    {
        public MagnaGuardProtector() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "MagnaGuard Protector",
                "Implacable Escort",
                Faction.Separatists,
                4,
                4,
                10,
                limited: 2,
                abilityType: typeof(Abilities.SecondEdition.MagnaGuardProtectorAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Cannon,
                    UpgradeType.Cannon,
                    UpgradeType.Missile,
                    UpgradeType.Modification
                },
                tags: new List<Tags>()
                {
                    Tags.Droid
                },
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            ShipInfo.ActionIcons.SwitchToDroidActions();

            DeadToRights oldAbility = (DeadToRights)ShipAbilities.First(n => n.GetType() == typeof(DeadToRights));
            oldAbility.DeactivateAbility();
            ShipAbilities.Remove(oldAbility);
            ShipAbilities.Add(new NetworkedCalculationsAbility());
        }
    }

    public class MagnaGuardProtectorXWA : MagnaGuardProtector
    {
        public MagnaGuardProtectorXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 10;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 10;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
                UpgradeType.Talent,
                UpgradeType.Modification,
                UpgradeType.Modification,
                UpgradeType.Cannon,
                UpgradeType.Cannon,
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class MagnaGuardProtectorAbility : GenericAbility
    {
        public static HashSet<PlayerNo> playerPrompted;

        protected virtual string Prompt
        {
            get
            {
                return "Assign the Guarded condition to 1 friendly ship other than MagnaGuard Protector.";
            }
        }
        public override void ActivateAbility()
        {
            Phases.Events.OnSetupStart += ClearPlayerPrompted;
            Phases.Events.OnSetupEnd += RegisterMagnaGuardProtectorAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnSetupStart += ClearPlayerPrompted;
            Phases.Events.OnSetupEnd -= RegisterMagnaGuardProtectorAbility;
        }

        private void ClearPlayerPrompted()
        {
            // This resets the playerPrompted between games, if not done then it will never ask after the first game
            playerPrompted = new();
        }

        private void RegisterMagnaGuardProtectorAbility()
        {
            if (playerPrompted.Contains(HostShip.Owner.PlayerNo))
                return; // Only prompt once per player

            playerPrompted.Add(HostShip.Owner.PlayerNo);

            Triggers.RegisterTrigger(new Trigger()
            {
                Name = HostShip.ShipId + ": Assign \"Guarded\" condition",
                TriggerType = TriggerTypes.OnSetupEnd,
                TriggerOwner = HostShip.Owner.PlayerNo,
                EventHandler = SelectMagnaGuardProtectorTarget,
            });
        }

        private void SelectMagnaGuardProtectorTarget(object Sender, System.EventArgs e)
        {
            SelectTargetForAbility(
                  AssignGuarded,
                  CheckRequirements,
                  GetAiGuardedPriority,
                  HostShip.Owner.PlayerNo,
                  "Guarded",
                  Prompt,
                  HostUpgrade
            );
        }

        protected virtual void AssignGuarded()
        {
            TargetShip.Tokens.AssignCondition(new Guarded(TargetShip) { });

            SelectShipSubPhase.FinishSelection();
        }

        protected virtual bool CheckRequirements(GenericShip ship)
        {
            return Tools.IsFriendly(ship, HostShip)
                && ship.PilotInfo.PilotName != "MagnaGuard Protector";
        }

        private int GetAiGuardedPriority(GenericShip ship)
        {
            return ship.PilotInfo.Cost + ship.UpgradeBar.GetUpgradesOnlyFaceup().Sum(n => n.UpgradeInfo.Cost);
        }
    }
}

namespace Conditions
{
    public class Guarded : GenericToken
    {
        public Guarded(GenericShip host) : base(host)
        {
            Name = ImageName = "Guarded Condition";
            Temporary = false;
            Tooltip = "https://infinitearenas.com/xw2/images/conditions/guarded.png";
        }

        public override void WhenAssigned()
        {
            Host.OnAttackStartAsDefender += CheckConditions;
        }

        public override void WhenRemoved()
        {
            Host.OnAttackStartAsDefender -= CheckConditions;
        }

        private void CheckConditions()
        {
            if (!Board.IsShipInArcByType(Combat.Attacker, Host, Arcs.ArcType.Bullseye))
            {
                Host.AfterGotNumberOfDefenceDice += RollExtraDice;
            }
        }

        private void RollExtraDice(ref int count)
        {
            int extraDice = Board.GetShipsInArcAtRange(Combat.Attacker, Combat.ArcForShot.ArcType, new UnityEngine.Vector2(Combat.ChosenWeapon.WeaponInfo.MinRange, Combat.ChosenWeapon.WeaponInfo.MaxRange), Team.Type.Enemy)
                .FindAll(s => s.PilotInfo.PilotName.Equals("MagnaGuard Protector") && (s.Tokens.HasToken<CalculateToken>() || s.Tokens.HasToken<EvadeToken>())).Count;

            if (extraDice > 0)
            {
                Messages.ShowInfo($"Host.PilotInfo.PilotName is \"Guarded\" and gains {extraDice} defense {(extraDice > 1 ? "dice" : "die")}.");
                count += extraDice;
            }

            Host.AfterGotNumberOfDefenceDice -= RollExtraDice;
        }
    }
}
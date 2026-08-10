using Abilities.SecondEdition;
using Actions;
using ActionsList;
using BoardTools;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using UnityEngine;
using Upgrade;

namespace Ship.SecondEdition.ASF01BWing
{
    public class GinaMoonsongBattleOverEndor : ASF01BWing
    {
        public GinaMoonsongBattleOverEndor() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Gina Moonsong",
                "Battle Over Endor",
                Faction.Rebel,
                5,
                5,
                0,
                isLimited: true,
                abilityType: typeof(GinaMoonsongBattleOverEndorAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Talent,
                    UpgradeType.Torpedo,
                    UpgradeType.Device
                },
                tags: new List<Tags>
                {
                    Tags.BWing
                },
                skinName: "Gina Moonsong",
                charges: 2,
                regensCharges: 1,
                isStandardLayout: true,
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            ImageUrl = "https://infinitearenas.com/xw2/images/quickbuilds/ginamoonsong-battleoverendor.png";

            ShipInfo.Shields++;

            MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.ItsATrap));
            MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.Juke));
            MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.ProtonTorpedoes));
            MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.IonBombs));

            ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(ReloadAction), ActionColor.Red));
            ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(BarrelRollAction), typeof(TargetLockAction)));

            ShipAbilities.Add(new GyroCockpit());

            PilotNameCanonical = "ginamoonsong-battleoverendor";

            DefaultUpgrades.Remove(typeof(UpgradesList.SecondEdition.StabilizedSFoilsOpen));
        }
    }

    public class GinaMoonsongBattleOverEndorXWA : GinaMoonsongBattleOverEndor
    {
        public GinaMoonsongBattleOverEndorXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 15;
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class GinaMoonsongBattleOverEndorAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= CheckAbility;
        }
        protected virtual void CheckAbility()
        {
            List<GenericShip> friendlyShipsAtRange = Board.GetShipsAtRange(HostShip, new Vector2(0, 2), Team.Type.Friendly);
            List<GenericShip> enemyShipsAtRange = Board.GetShipsAtRange(HostShip, new Vector2(0, 3), Team.Type.Enemy);

            foreach (GenericShip ship in friendlyShipsAtRange)
            {
                if (ship.PilotInfo.PilotName.Equals("Braylen Stramm") && ship.IsStressed && enemyShipsAtRange.Count > 0)
                {
                    RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, SelectTarget);
                }
            }
        }

        private void SelectTarget(object sender, EventArgs e)
        {
            SelectTargetForAbility(
                AcquireLock,
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                HostShip.PilotInfo.PilotName,
                "You may acquire a lock",
                HostShip,
                showSkipButton: true
            );
        }

        private int GetAiPriority(GenericShip ship)
        {
            return HostShip.Tokens.HasToken<BlueTargetLockToken>() ? 0 : 1000;
        }

        private bool FilterTargets(GenericShip ship)
        {
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.Enemy }) && FilterTargetsByRange(ship, 0, 3);
        }

        private void AcquireLock()
        {
            ActionsHolder.AcquireTargetLock(
                HostShip,
                TargetShip,
                DecisionSubPhase.ConfirmDecision,
                DecisionSubPhase.ConfirmDecision
            );
        }
    }
}
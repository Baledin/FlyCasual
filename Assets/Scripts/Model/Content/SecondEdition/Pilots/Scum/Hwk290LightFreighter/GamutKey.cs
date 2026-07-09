using BoardTools;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship.SecondEdition.Hwk290LightFreighter
{
    public class GamutKey : Hwk290LightFreighter
    {
        public GamutKey() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Gamut Key",
                "Collaborationist Governor",
                Faction.Scum,
                3,
                4,
                8,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.GamutKeyPilotAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Crew,
                    UpgradeType.Device,
                    UpgradeType.Illicit,
                    UpgradeType.Modification,
                    UpgradeType.Modification
                },
                tags: new List<Tags>
                {
                    Tags.Freighter
                },
                charges: 2,
                skinName: "Black",
                regensCharges: 1,
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class GamutKeyXWA : GamutKey
    {
        public GamutKeyXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 8;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 7;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
                UpgradeType.Talent,
                UpgradeType.Crew,
                UpgradeType.Illicit,
                UpgradeType.Modification,
                UpgradeType.Device,
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class GamutKeyPilotAbility : GenericAbility
    {
        GenericShip ShipThatKeepTokens { get; set; }

        public override void ActivateAbility()
        {
            Phases.Events.OnEndPhaseStart_Triggers += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnEndPhaseStart_Triggers -= CheckAbility;
        }

        private void CheckAbility()
        {
            if (HasEnoughCharges())
            {
                RegisterAbilityTrigger(TriggerTypes.OnEndPhaseStart, AskToSelectShip);
            }
        }

        protected virtual bool HasEnoughCharges()
        {
            return HostShip.State.Charges >= 2;
        }

        private void AskToSelectShip(object sender, EventArgs e)
        {
            SelectTargetForAbility(
                LeaveTokens,
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                name: GetAbilityName(),
                description: "2 charges: Choose a ship, during the End Phase circular tokens are not removed from it",
                imageSource: GetAbilityImage()
            );
        }

        protected virtual string GetAbilityName()
        {
            return HostShip.PilotInfo.PilotName;
        }

        protected virtual IImageHolder GetAbilityImage()
        {
            return HostShip;
        }

        private void LeaveTokens()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();

            SpendChargesForAbility();
            Messages.ShowInfo($"{GetAbilityName()}: {TargetShip.PilotInfo.PilotName} doesn't remove cirular tokens during this End Phase");
            ShipThatKeepTokens = TargetShip;

            ShipThatKeepTokens.BeforeRemovingTokenInEndPhase += DontRemoveCircularTokens;
            Phases.Events.OnPlanningPhaseStart += UnsubscribeGainedAbility;

            Triggers.FinishTrigger();
        }

        protected virtual void SpendChargesForAbility()
        {
            HostShip.SpendCharges(2);
        }

        private void DontRemoveCircularTokens(GenericShip ship, GenericToken token, ref bool willBeRemoved)
        {
            if (token.TokenShape == TokenShapes.Cirular) willBeRemoved = false;
        }

        private void UnsubscribeGainedAbility()
        {
            // Reset ability for new round
            Phases.Events.OnPlanningPhaseStart -= UnsubscribeGainedAbility;

            ShipThatKeepTokens.BeforeRemovingTokenInEndPhase -= DontRemoveCircularTokens;
            ShipThatKeepTokens = null;
        }

        protected virtual bool FilterTargets(GenericShip ship)
        {
            ShotInfo shotInfo = new ShotInfo(HostShip, ship, HostShip.PrimaryWeapons);
            return ship.ShipId == HostShip.ShipId
                || shotInfo.InArcByType(Arcs.ArcType.SingleTurret)
                && ship.Tokens.CountTokensByShape(TokenShapes.Cirular) > 0;
        }

        private int GetAiPriority(GenericShip ship)
        {
            int result = 0;

            if (Tools.IsSameTeam(HostShip, ship))
            {
                result += ship.PilotInfo.Cost + (ship.Tokens.CountTokensByShape(TokenShapes.Cirular) * 100);
            }

            return result;
        }
    }
}

using Abilities.SecondEdition;
using Content;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEInterceptor
    {
        public class SoontirFel : TIEInterceptor
        {
            public SoontirFel() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Soontir Fel",
                    "Ace of Legend",
                    Faction.Imperial,
                    6,
                    5,
                    10,
                    isLimited: true,
                    abilityType: typeof(SoontirFelAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Modification,
                        UpgradeType.Configuration
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    seImageNumber: 103,
                    skinName: "Red Stripes",
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );
            }
        }

        public class SoontirFelXWA : SoontirFel
        {
            public SoontirFelXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 13;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 10;
                (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
                {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Modification,
                        UpgradeType.Configuration
                };
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SoontirFelAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += RegisterSoontirFelAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= RegisterSoontirFelAbility;
        }

        private void RegisterSoontirFelAbility()
        {
            if (BoardTools.Board.GetShipsInBullseyeArc(HostShip, Team.Type.Enemy).Count > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, OnCombatAssignFocus);
            }
        }

        private void OnCombatAssignFocus(object sender, EventArgs e)
        {
            Messages.ShowInfo(HostShip.PilotInfo.PilotName + " has an enemy ship in Bullseye arc and gains Focus token");
            HostShip.Tokens.AssignToken(typeof(FocusToken), Triggers.FinishTrigger);
        }
    }
}
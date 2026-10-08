using Arcs;
using BoardTools;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.Hwk290LightFreighter
    {
        public class KananJarrus : Hwk290LightFreighter
        {
            public KananJarrus() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Kanan Jarrus",
                    "Lost Padawan",
                    Faction.Scum,
                    3,
                    4,
                    6,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.KananJarrusHwk290Ability),
                    force: 1,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.ForcePower,
                        UpgradeType.Crew,
                        UpgradeType.Device,
                        UpgradeType.Illicit,
                        UpgradeType.Modification,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Jedi,
                        Tags.LightSide,
                        Tags.Freighter
                    },
                    skinName: "Black",
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );

                PilotNameCanonical = "kananjarrus-hwk290lightfreighter";
            }
        }

        public class KananJarrusXWA : KananJarrus
        {
            public KananJarrusXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 10;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 7;
                (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
                {
                        UpgradeType.ForcePower,
                        UpgradeType.Crew,
                        UpgradeType.Illicit,
                        UpgradeType.Modification,
                        UpgradeType.Device,
                };
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class KananJarrusHwk290Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnAttackStartAsAttackerGlobal += CheckPilotAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnAttackStartAsAttackerGlobal -= CheckPilotAbility;
        }

        private void CheckPilotAbility()
        {
            bool hasForceTokens = HostShip.State.Force > 0;

            if (hasForceTokens && (IsDefenderMe() || IsDefenderInMyMobileArc()) && Tools.IsAnotherTeam(HostShip, Combat.Attacker))
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackStart, AskDecreaseAttack);
            }
        }

        private void AskDecreaseAttack(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostShip.PilotInfo.PilotName,
                AlwaysUseByDefault,
                DecreaseAttack,
                descriptionLong: "Do you want to spend a force token? (If you do, the attacker rolls 1 fewer attack die)",
                imageHolder: HostShip
            );
        }

        private void DecreaseAttack(object sender, EventArgs e)
        {
            RegisterDecreaseNumberOfAttackDice();
            HostShip.State.SpendForce(1, DecisionSubPhase.ConfirmDecision);
        }

        private void RegisterDecreaseNumberOfAttackDice()
        {
            Combat.Attacker.AfterGotNumberOfAttackDice += DecreaseNumberOfAttackDice;
        }

        private void DecreaseNumberOfAttackDice(ref int diceCount)
        {
            diceCount--;
            Combat.Attacker.AfterGotNumberOfAttackDice -= DecreaseNumberOfAttackDice;
        }

        private bool IsDefenderMe()
        {
            return Combat.Defender.ShipId == HostShip.ShipId;
        }

        private bool IsDefenderInMyMobileArc()
        {
            ShotInfo shotInfo = new ShotInfo(HostShip, Combat.Defender, HostShip.PrimaryWeapons);
            return shotInfo.InArcByType(ArcType.SingleTurret);
        }
    }
}

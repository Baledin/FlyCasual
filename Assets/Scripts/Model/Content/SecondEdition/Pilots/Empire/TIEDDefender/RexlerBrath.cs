using Content;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEDDefender
    {
        public class RexlerBrath : TIEDDefender
        {
            public RexlerBrath() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Rexler Brath",
                    "Onyx Leader",
                    Faction.Imperial,
                    5,
                    7,
                    13,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.RexlerBrathAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Sensor,
                        UpgradeType.Cannon,
                        UpgradeType.Missile,
                        UpgradeType.Missile,
                        UpgradeType.Configuration
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    seImageNumber: 122,
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );
            }
        }

        public class RexlerBrathXWA : RexlerBrath
        {
            public RexlerBrathXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 18;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 13;
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
                (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Sensor,
                    UpgradeType.Modification,
                    UpgradeType.Tech,
                    UpgradeType.Cannon,
                    UpgradeType.Missile,
                    UpgradeType.Configuration
                };
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class RexlerBrathAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackHitAsAttacker += CheckRexlerBrathTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackHitAsAttacker -= CheckRexlerBrathTrigger;
        }

        private void CheckRexlerBrathTrigger()
        {
            if (HostShip.Tokens.HasToken(typeof(EvadeToken)) && Combat.Defender.Damage.HasFacedownCards)
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = HostShip.PilotInfo.PilotName + " exposes facedown card",
                    TriggerType = TriggerTypes.OnAttackHit,
                    TriggerOwner = Combat.Defender.Owner.PlayerNo,
                    EventHandler = delegate
                    {
                        Combat.Defender.Damage.ExposeRandomFacedownCard(Triggers.FinishTrigger);
                    }
                });
            }
        }
    }
}
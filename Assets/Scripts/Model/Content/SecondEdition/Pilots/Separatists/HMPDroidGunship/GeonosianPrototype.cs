using Content;
using Ship;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.HMPDroidGunship
    {
        public class GeonosianPrototype : HMPDroidGunship
        {
            public GeonosianPrototype() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Geonosian Prototype",
                    "Devastation Protocols",
                    Faction.Separatists,
                    2,
                    4,
                    12,
                    limited: 2,
                    abilityType: typeof(Abilities.SecondEdition.GeonosianPrototypeAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Missile,
                        UpgradeType.Missile,
                        UpgradeType.Cannon,
                        UpgradeType.Cannon,
                        UpgradeType.TacticalRelay,
                        UpgradeType.Modification,
                        UpgradeType.Configuration
                    },
                    tags: new List<Tags>
                    {
                        Tags.Droid
                    },
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );
            }
        }

        public class GeonosianPrototypeXWA : GeonosianPrototype
        {
            public GeonosianPrototypeXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 10;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 13;
                (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
                {
                    UpgradeType.Modification,
                    UpgradeType.Device,
                    UpgradeType.Cannon,
                    UpgradeType.Cannon,
                    UpgradeType.Configuration,
                };
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class GeonosianPrototypeAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                "Geonosian Prototype",
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Reroll,
                2,
                payAbilityCost: RemoveTractorToken
            );
        }

        private bool IsAvailable()
        {
            bool result = false;

            if (Combat.ChosenWeapon.WeaponType == WeaponTypes.Cannon || Combat.ChosenWeapon.WeaponType == WeaponTypes.Missile)
            {
                if (Combat.Defender.Tokens.HasToken<TractorBeamToken>())
                {
                    result = true;
                }
            }

            return result;
        }

        private int GetAiPriority()
        {
            return 85;
        }

        private void RemoveTractorToken(Action<bool> callback)
        {
            if (Combat.Defender.Tokens.HasToken<TractorBeamToken>())
            {
                Combat.Defender.Tokens.RemoveToken
                (
                    typeof(TractorBeamToken),
                    delegate { callback(true); }
                );
            }
            else
            {
                callback(false);
            }
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}
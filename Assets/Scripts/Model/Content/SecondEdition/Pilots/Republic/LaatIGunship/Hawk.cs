using Abilities.Parameters;
using ActionsList;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.LaatIGunship
    {
        public class Hawk : LaatIGunship
        {
            public Hawk() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Hawk\"",
                    "Valkyrie 2929",
                    Faction.Republic,
                    4,
                    6,
                    25,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.HawkAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Crew,
                        UpgradeType.Crew,
                        UpgradeType.Gunner,
                        UpgradeType.Gunner,
                        UpgradeType.Modification,
                        UpgradeType.Missile,
                        UpgradeType.Missile
                    },
                    tags: new List<Tags>
                    {
                        Tags.Clone
                    },
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );
            }
        }

        public class HawkXWA : Hawk
        {
            public HawkXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 12;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 10;
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
                (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Crew,
                    UpgradeType.Crew,
                    UpgradeType.Gunner,
                    UpgradeType.Gunner,
                    UpgradeType.Modification,
                    UpgradeType.Missile,
                    UpgradeType.Missile,
                    UpgradeType.Torpedo
                };
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class HawkAbility : TriggeredAbility
    {
        public override TriggerForAbility Trigger => new AtTheStartOfPhase(typeof(SubPhases.EndStartSubPhase));

        public override AbilityPart Action => new EachShipCanDoAction
        (
            eachShipAction: PerformReposition,
            conditions: new ConditionsBlock
            (
                new TeamCondition(ShipTypes.Friendly),
                new RangeToHostCondition(minRange: 0, maxRange: 1),
                new RevealedManeuverCondition(minSpeed: 3, maxSpeed: 5)
            ),
            description: new AbilityDescription
            (
                "\"Hawk\"",
                "Friendly ships may gain 1 strain token to perfrom a Barrel Roll or Boost action",
                imageSource: HostShip
            )
        );

        private void PerformReposition(GenericShip ship, Action callback)
        {
            Selection.ChangeActiveShip(ship);

            ship.Tokens.AssignToken
            (
                typeof(Tokens.StrainToken),
                delegate
                {
                    ship.AskPerformFreeAction
                    (
                        new List<GenericAction>()
                        {
                            new BarrelRollAction(){ HostShip = ship },
                            new BoostAction(){ HostShip = ship },
                        },
                        callback,
                        descriptionShort: "\"Hawk\"",
                        descriptionLong: "You can perform barrel roll or boost action",
                        imageHolder: ship
                    );
                }
            );
        }
    }
}
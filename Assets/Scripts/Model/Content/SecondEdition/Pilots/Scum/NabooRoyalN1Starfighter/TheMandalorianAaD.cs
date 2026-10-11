using Abilities.SecondEdition;
using Actions;
using ActionsList;
using Content;
using System.Collections.Generic;
using System.Linq;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship.SecondEdition.NabooRoyalN1Starfighter
{
    public class TheMandalorianAaD : NabooRoyalN1Starfighter
    {
        public TheMandalorianAaD() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                pilotName: "The Mandalorian",
                pilotTitle: "Armed and Dangerous",
                faction: Faction.Scum,
                initiative: 5,
                cost: 11,
                loadoutValue: 0,
                isLimited: true,
                abilityType: typeof(TheMandalorianAbility),
                force: 0,
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Crew,
                    UpgradeType.Illicit,
                    UpgradeType.Configuration,
                    UpgradeType.Modification
                },
                tags: new List<Tags>
                {
                    Tags.Mandalorian,
                    Tags.BountyHunter
                },
                charges: 0,
                regensCharges: 0,
                isStandardLayout: true,
                legality: new List<Legality> { Legality.XWA },
                abilityText: "While you defend or perform an attack, if you are in the front arc at range 1-2 of 2 or more enemy ships, you may change 1 of your blank results to a focus result.",
                skinName: "Silver"

                );

            MustHaveUpgrades.Add(typeof(Outmaneuver));
            MustHaveUpgrades.Add(typeof(TheChildAaD));
            MustHaveUpgrades.Add(typeof(CalibratedLaserTargeting));
            MustHaveUpgrades.Add(typeof(KinesoSwitch));

            PilotNameCanonical = "themandalorian-armedanddangerous";

            ShipAbilities.Remove(ShipAbilities.First(a => a is FullThrottleAbility));
            ShipAbilities.Add(new RestoredSpeedsterAbility());

            ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(SlamAction)));
            ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(SlamAction), typeof(TargetLockAction), ActionColor.Red));
        }
    }
}
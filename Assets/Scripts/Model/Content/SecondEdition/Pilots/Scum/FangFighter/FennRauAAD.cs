using Content;
using System.Collections.Generic;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship.SecondEdition.FangFighter
{
    public class FennRauAaD : FangFighter
    {
        public FennRauAaD() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Fenn Rau",
                "Armed and Dangerous",
                Faction.Scum,
                6,
                17,
                0,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.FennRauScumAbility),
                tags: new List<Tags>
                {
                    Tags.Mandalorian
                },
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Modification,
                    UpgradeType.Modification
                },
                seImageNumber: 155,
                skinName: "Zealous Recruit",
                legality: new List<Legality> { Legality.XWA },
                isStandardLayout: true,
                abilityText: "While you defend or perform an attack, if the attack range is 1, you may roll 1 additional die."
            );
            MustHaveUpgrades.Add(typeof(Fearless));
            MustHaveUpgrades.Add(typeof(BeskarReinforcedPlating));
            MustHaveUpgrades.Add(typeof(AdaptablePowerSystems));

            PilotNameCanonical = "fennrau-armedanddangerous";

            ShipInfo.Hull++;
        }
    }
}
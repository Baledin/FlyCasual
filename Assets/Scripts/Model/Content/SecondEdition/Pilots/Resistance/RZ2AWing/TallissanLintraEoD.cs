using Actions;
using ActionsList;
using Content;
using System.Collections.Generic;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship.SecondEdition.RZ2AWing
{
    public class TallissanLintraEoD : RZ2AWing
    {
        public TallissanLintraEoD() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Tallissan Lintra",
                "Evacuation of D'Qar",
                Faction.Resistance,
                5,
                11,
                0,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.TallissanLintraAbility),
                charges: 1,
                regensCharges: 1,
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Talent,
                    UpgradeType.Talent,
                    UpgradeType.Missile
                },
                tags: new List<Tags>
                {
                    Tags.AWing
                },
                skinName: "Blue",
                legality: new List<Legality> { Legality.XWA },
                isStandardLayout: true
            );

            MustHaveUpgrades.Add(typeof(ForTheCause));
            MustHaveUpgrades.Add(typeof(Heroic));
            MustHaveUpgrades.Add(typeof(PushTheLimit));
            MustHaveUpgrades.Add(typeof(XX23SThreadTracers));

            ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(FocusAction), typeof(CoordinateAction), ActionColor.Red));

            PilotNameCanonical = "tallissanlintra-evacuationofdqar";
        }
    }
}
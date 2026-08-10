using BoardTools;
using Content;
using Movement;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class NiteOwlCommandos : GenericUpgrade
    {
        public NiteOwlCommandos() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Nite Owl Commandos",
                types: new List<UpgradeType>()
                {
                    UpgradeType.Crew,
                    UpgradeType.Crew
                },
                subType: UpgradeSubType.Remote,
                cost: 8,
                isLimited: true,
                charges: 2,
                cannotBeRecharged: true,
                restrictions: new UpgradeCardRestrictions(
                    new FactionRestriction(Faction.Republic),
                    new BaseSizeRestriction(BaseSize.Medium, BaseSize.Large)
                ),
                abilityType: typeof(Abilities.SecondEdition.DeployCommandoTeam),
                remoteType: typeof(Remote.CommandoTeam),
                legalityInfo: new List<Legality>() { Legality.StandardLegal, Legality.ExtendedLegal, Legality.XWA }
            );
        }

        public override List<ManeuverTemplate> GetDefaultDropTemplates()
        {
            return new List<ManeuverTemplate>()
            {
                new (ManeuverBearing.Straight, ManeuverDirection.Forward, ManeuverSpeed.Speed1, isBombTemplate: true)
            };
        }
    }
}
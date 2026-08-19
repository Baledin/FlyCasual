using BoardTools;
using System.Collections.Generic;

namespace UpgradesList.SecondEdition
{
    public interface IDroppable
    {
        List<ManeuverTemplate> GetDefaultDropTemplates();

        List<ManeuverTemplate> GetDefaultLaunchTemplates();
    }
}
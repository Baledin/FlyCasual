using BoardTools;
using System.Collections.Generic;

namespace Upgrade
{
    public interface IDroppable
    {
        List<ManeuverTemplate> GetDefaultDropTemplates();

        List<ManeuverTemplate> GetDefaultLaunchTemplates();
    }
}
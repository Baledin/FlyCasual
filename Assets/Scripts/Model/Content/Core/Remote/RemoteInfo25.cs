using Ship;
using System;

namespace Remote
{
    public class RemoteInfo25 : RemoteInfo
    {
        public ShipArcsInfo ArcInfo { get; set; }
        public ShipSoundInfo SoundInfo { get; set; }

        public RemoteInfo25(string name, int initiative, ShipArcsInfo arcInfo, int agility, int hull, string imageUrl, Type abilityType = null, int charges = 0, int regensCharges = 0, ShipSoundInfo soundInfo = null) : base(name, initiative, agility, hull, imageUrl, abilityType)
        {
            Charges = charges;
            RegensCharges = regensCharges;
            ArcInfo = arcInfo;
            SoundInfo = soundInfo;
        }
    }
}
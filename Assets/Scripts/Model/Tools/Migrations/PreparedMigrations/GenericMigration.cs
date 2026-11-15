using UnityEngine;

namespace Migrations
{
    public class GenericMigration
    {
        public int Version { get; protected set; }

        public void DoMigration()
        {
            CustomMigration();

            PlayerPrefs.SetInt("LastMigrationVersion", Version);
        }

        protected virtual void CustomMigration() { }
    }
}
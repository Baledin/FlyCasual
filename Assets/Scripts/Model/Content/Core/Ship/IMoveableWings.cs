using UnityEngine;

namespace Ship
{
    public enum WingPosition { Opened, Closed, None };

    public interface IMovableWings
    {
        WingPosition CurrentWingPosition { get; set; }

        public virtual void WingsOpen()
        {
            if (this.CurrentWingPosition == WingPosition.Opened) return;

            ChangeWingPosition(WingPosition.Opened);
        }

        public virtual void WingsClose()
        {
            if (this.CurrentWingPosition == WingPosition.Closed) return;

            ChangeWingPosition(WingPosition.Closed);
        }

        protected virtual void ChangeWingPosition(WingPosition newPosition)
        {
            foreach (Transform transform in (this as GenericShip).GetModelTransform())
            {
                if (!transform.name.StartsWith("Wing")) continue;

                CurrentWingPosition = newPosition;

                Sounds.PlayShipSound("Servomotors", (this as GenericShip));
                transform.GetComponent<Animation>().Play($"{transform.name}_{CurrentWingPosition}");
            }
        }
    }
}
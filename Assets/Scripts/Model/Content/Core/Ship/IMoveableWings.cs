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

            ToggleWingPosition(WingPosition.Opened);
        }

        public virtual void WingsClose()
        {
            if (this.CurrentWingPosition == WingPosition.Closed) return;

            ToggleWingPosition(WingPosition.Closed);
        }

        protected virtual void ToggleWingPosition(WingPosition position)
        {
            foreach (Transform transform in (this as GenericShip).GetModelTransform())
            {
                if (!transform.name.StartsWith("Wing")) continue;

                CurrentWingPosition = position;

                Sounds.PlayShipSound("Servomotors", (this as GenericShip));
                transform.GetComponent<Animation>().Play($"{transform.name}_{CurrentWingPosition}");
            }
        }
    }
}
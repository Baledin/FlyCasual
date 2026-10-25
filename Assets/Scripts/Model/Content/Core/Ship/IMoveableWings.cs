using UnityEngine;

namespace Ship
{
    public enum WingsPositions { Opened, Closed, None };

    public interface IMovableWings
    {

        WingsPositions CurrentWingsPosition { get; set; }

        public virtual void WingsOpen()
        {
            if (this.CurrentWingsPosition == WingsPositions.Closed)
            {
                WingsChangePosition("Open");
                this.CurrentWingsPosition = WingsPositions.Opened;
            }
        }

        public virtual void WingsClose()
        {
            if (this.CurrentWingsPosition == WingsPositions.Opened)
            {
                WingsChangePosition("Close");
                this.CurrentWingsPosition = WingsPositions.Closed;
            }
        }

        protected virtual void WingsChangePosition(string wingPosition)
        {
            Sounds.PlayShipSound("Servomotors", (this as GenericShip));

            foreach (Transform transform in (this as GenericShip).GetModelTransform())
            {
                if (!transform.name.StartsWith("Wing")) continue;

                Animation wingAnimator = transform.GetComponent<Animation>();
                wingAnimator.Play(transform.name + "_" + wingPosition);
            }
        }
    }
}
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace SubPhases
{

    public class RoundStartSubPhase : GenericSubPhase
    {

        public override void Start()
        {
            Name = "Round start";
            UpdateHelpInfo();
        }

        public override void Initialize()
        {
            Console.Write($"\nROUND IS STARTED: {Phases.RoundCounter+1}", isBold: true, color: "orange");

            InformAboutNewRoundStart();

            Phases.Events.CallRoundStartTrigger(delegate { Phases.FinishSubPhase(this.GetType()); });
        }

        private void InformAboutNewRoundStart()
        {
            Phases.RoundCounter++;
            UI.AddTestLogEntry("Round " + Phases.RoundCounter + " is started");
        }

        public override void Next()
        {
            GenericSubPhase subphase = Phases.StartTemporarySubPhaseNew("Notification", typeof(NotificationSubPhase), PlanningStartSubPhase);
            (subphase as NotificationSubPhase).TextToShow = "Round "+ Phases.RoundCounter;
            subphase.Start();
        }

        private void PlanningStartSubPhase()
        {
            Phases.CurrentSubPhase = new PlanningStartSubPhase();
            Phases.CurrentSubPhase.Start();
            Phases.CurrentSubPhase.Prepare();
            Phases.CurrentSubPhase.Initialize();
        }

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship, int mouseKeyIsPressed)
        {
            return false;
        }

        public override bool AnotherShipCanBeSelected(Ship.GenericShip targetShip, int mouseKeyIsPressed)
        {
            return false;
        }

    }

}

﻿using Players;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace GameCommands
{
    public class ObstaclePlacementCommand : GameCommand
    {
        public ObstaclePlacementCommand(GameCommandTypes type, Type subPhase, int subphaseId, string rawParameters) : base(type, subPhase, subphaseId, rawParameters)
        {

        }

        public override void Execute()
        {
            string obstacleName = GetString("name");

            Console.Write($"Obstacle is placed: {obstacleName}");

            if (Global.IsCampaignGame)
            {
                CampaignObstaclesPlacementSubPhase.PlaceObstacle
                (obstacleName,
                new Vector3
                (
                    float.Parse(GetString("positionX"), CultureInfo.InvariantCulture),
                    0,
                    float.Parse(GetString("positionZ"), CultureInfo.InvariantCulture)
                ),
                new Vector3
                (
                    float.Parse(GetString("rotationX"), CultureInfo.InvariantCulture),
                    float.Parse(GetString("rotationY"), CultureInfo.InvariantCulture),
                    float.Parse(GetString("rotationZ"), CultureInfo.InvariantCulture)
                )
             );
            }
            else
            {
                ObstaclesPlacementSubPhase.PlaceObstacle
                (obstacleName,
                new Vector3
                (
                    float.Parse(GetString("positionX"), CultureInfo.InvariantCulture),
                    0,
                    float.Parse(GetString("positionZ"), CultureInfo.InvariantCulture)
                ),
                new Vector3
                (
                    float.Parse(GetString("rotationX"), CultureInfo.InvariantCulture),
                    float.Parse(GetString("rotationY"), CultureInfo.InvariantCulture),
                    float.Parse(GetString("rotationZ"), CultureInfo.InvariantCulture)
                )
             );
            }


        }
    }

}

using Players;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;

namespace CommandsList
{
    public class TokenCommand : GenericCommand
    {
        private readonly Dictionary<string, Type> StringToType = new()
        {
            { "focus",          typeof(FocusToken)          },
            { "evade",          typeof(EvadeToken)          },
            { "stress",         typeof(StressToken)         },
            { "targetlock",     typeof(BlueTargetLockToken) },
            { "ion",            typeof(IonToken)            },
            { "tractorbeam",    typeof(TractorBeamToken)    },
            { "jam",            typeof(JamToken)            },
            { "reinforceaft",   typeof(ReinforceAftToken)   },
            { "reinforcefore",  typeof(ReinforceForeToken)  },
            { "cloak",          typeof(CloakToken)          },
            { "energy",         typeof(EnergyToken)         },
            { "calculate",      typeof(CalculateToken)      },
            { "force",          typeof(ForceToken)          },
            { "charge",         typeof(ChargeToken)         },
            { "strain",         typeof(StrainToken)         },
            { "deplete",        typeof(DepleteToken)        },
            { "disarm",         typeof(WeaponsDisabledToken)}
        };

        public TokenCommand()
        {
            Keyword = "token";
            Description = "token assign id:<shipId> type:<type> [target:<targetShipId>]\n" +
                            "Assign token to ship where type: focus, evade, stress, targetlock, ion, tractorbeam, jam, reinforceaft, reinforcefore, cloak, energy, calculate, force, charge, strain, deplete, disarm\n" +
                            "(target is used only for targetlock type)";

            Console.AddAvailableCommand(this);
        }

        public override void Execute(Dictionary<string, string> parameters)
        {
            if (!parameters.ContainsKey("assign"))
            {
                Console.ProcessCommand("help " + Keyword);
                return;
            }

            int shipId = -1;
            if (parameters.ContainsKey("id")) int.TryParse(parameters["id"], out shipId);

            int targetShipId = -1;
            if (parameters.ContainsKey("target")) int.TryParse(parameters["target"], out targetShipId);

            Type tokenType = null;
            string typeString = (parameters.ContainsKey("type")) ? parameters["type"] : null;
            if (typeString != null)
            {
                if (StringToType.ContainsKey(typeString)) tokenType = StringToType[typeString];
            }

            if (shipId != -1 && tokenType != null)
            {
                AssignToken(shipId, tokenType, targetShipId);
            }
            else
            {
                Console.ProcessCommand("help " + Keyword);
            }
        }

        private void AssignToken(int shipId, Type tokenType, int targetShipId)
        {
            GenericShip ship = Roster.AllUnits.FirstOrDefault(n => n.Key == "ShipId:" + shipId).Value;

            if (ship != null)
            {
                if (tokenType == typeof(BlueTargetLockToken))
                {
                    if (targetShipId != -1)
                    {
                        GenericShip targetShip = Roster.AllUnits.FirstOrDefault(n => n.Key == "ShipId:" + targetShipId).Value;
                        if (targetShip != null)
                        {
                            ActionsHolder.AcquireTargetLock(ship, targetShip, ShowMessage, ShowErrorMessage);
                        }
                        else
                        {
                            ShowHelp();
                        }
                    }
                    else
                    {
                        ShowHelp();
                    }
                }
                else if (tokenType == typeof(TractorBeamToken))
                {
                    GenericShip targetShip = Roster.AllUnits.FirstOrDefault(n => n.Key == "ShipId:" + targetShipId).Value;
                    TractorBeamToken token = new(ship, targetShip.Owner);
                    ship.Tokens.AssignToken(token, ShowMessage);
                }
                else
                {
                    GenericToken token;
                    if (tokenType == typeof(JamToken))
                    {
                        GenericPlayer assigner = ship.Owner == Roster.Player1 ? Roster.Player2 : Roster.Player1;
                        token = (GenericToken)Activator.CreateInstance(tokenType, ship, assigner);
                    }
                    else
                    {
                        token = (GenericToken)Activator.CreateInstance(tokenType, ship);
                    }

                    ship.Tokens.AssignToken(token, ShowMessage);
                }
            }
        }

        private void ShowMessage()
        {
            Console.Write("Token command is resolved", true);
        }

        private void ShowErrorMessage()
        {
            Console.Write("Token command is not resolved - error occured", true, "red");
        }
    }
}

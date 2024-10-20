﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using ConsoleAdventureUtils;
using System.Runtime;

/*
 *  Game Class connnect the different components for the game.
 *  - holds the location data and its creation.
 *  - holds the current state of the game (player position, unlocked location).
 *  - holds monologue texts.
 *  - holds choices values.
 */
public class Game
{
    // Game Location
    public enum Location
    {
        // states
        LOCKED,
        TRIED,

        // location
        SECOND_FLOOR_HALLWAY,
        MAIN_STAIRCASE,
        MASTER_BEDROOM,
        NURSERY,
        ART_STUDIO,
        GUEST_BEDROOM,
        GALLERY_OVERLOOK,
        GAME_ROOM,
        GRAND_FROYER,
        DRAWING_ROOM,
        DINING_HALL,
        GALLERY,
        ENTRANCE
    }

    public static BiDictionary<Location, string> locationMap = new BiDictionary<Location, string>();

    protected static Dictionary<Location, ArrayList> locations = new Dictionary<Location, ArrayList>();

    protected static Location[,] directions =
    {
        { Location.SECOND_FLOOR_HALLWAY, Location.MAIN_STAIRCASE },
        { Location.SECOND_FLOOR_HALLWAY, Location.MASTER_BEDROOM },
        { Location.SECOND_FLOOR_HALLWAY, Location.NURSERY },
        { Location.SECOND_FLOOR_HALLWAY, Location.ART_STUDIO },
        { Location.SECOND_FLOOR_HALLWAY, Location.GUEST_BEDROOM },
        { Location.SECOND_FLOOR_HALLWAY, Location.GALLERY_OVERLOOK },
        { Location.SECOND_FLOOR_HALLWAY, Location.GAME_ROOM },
        { Location.MAIN_STAIRCASE, Location.GRAND_FROYER },
        { Location.GRAND_FROYER, Location.DRAWING_ROOM },
        { Location.GRAND_FROYER, Location.DINING_HALL },
        { Location.GRAND_FROYER, Location.GALLERY },
        { Location.GRAND_FROYER, Location.ENTRANCE },
    };

    // Monologues
    public enum MonologueKeys
    {
        INTRO,
        IN_THE_CENTER,
        LOCATIONLOCKED,
        CRYING,
        NURSERY_FIRST_ENTER,
        MAIN_STAIRCASE_FIRST_ENTER,
        TRY_ENTRANCE_DOOR,
        ENTRANCE_DOOR_WAIL
    }

    private static readonly Dictionary<MonologueKeys, string> MonologueTexts = new Dictionary<MonologueKeys, string>
    {
        {
            MonologueKeys.INTRO,"You awaken in a vast, dimly lit mansion, its towering ceilings and ornate decor both magnificent and foreboding. Shadows stretch across the walls, whispering secrets you can’t quite grasp. The air is thick with an unsettling silence, broken only by the distant creak of old wood beneath your feet.\r\n\r\nYou search for clues about your identity, but your mind is a blank canvas, void of memories. As you take a cautious step forward, a chill runs down your spine. What lies within these walls? Are the eerie sounds mere echoes of your imagination, or do they hint at something lurking just beyond your sight? Your journey begins now—face your fears and uncover the truth hidden in the depths of this mansion. Will you discover who you are, or will the shadows consume you?"
        },
        {
            MonologueKeys.IN_THE_CENTER,"You are now in the center of the mansion!"
        },
        {
            MonologueKeys.LOCATIONLOCKED,"The Door is Locked!"
        },
        {
            MonologueKeys.CRYING,"A faint, chilling wail echoes through the hall—a child's cry, drifting from the direction of the Nursery. Your heart quickens. Could it be a lost soul, or something far more sinister?"
        },
        {
            MonologueKeys.NURSERY_FIRST_ENTER,"As you approach the Nursery, a sense of dread washes over you, tightening like a vice around your chest. The air grows colder, and each breath forms a misty cloud that lingers in the dim light. The room is filled with baby dolls—each one missing an eye, yet they seem to watch your every move with their remaining, lifeless gaze. A shiver runs through you as the icy air presses against your skin, as if unseen hands are drawing closer. You can't shake the feeling that something unspeakably sinister is waiting in the shadows, just beyond your sight."
        },
        {
            MonologueKeys.MAIN_STAIRCASE_FIRST_ENTER, "You find yourself above the main staircase, gazing down from the second-floor hallway into the Grand Foyer of the mansion. The oppressive silence wraps around you as you take in the staggering size of the space below. Majestic yet foreboding, the foyer is adorned with ornate decor, hinting at the mansion’s former glory. At the bottom of the staircase, a grand door looms, its imposing presence suggesting a potential escape from this eerie place. Could it be the way out of the mansion?"
        },
        {
            MonologueKeys.TRY_ENTRANCE_DOOR, "As you approach the entrance door, your gaze drifts to the windows nearby. Peering through the glass, you attempt to glimpse the outside world, but all you see is an inky darkness, as if the very landscape has been swallowed by shadow. There is nothing you can discern—only an unsettling void."
        },
        {
            MonologueKeys.ENTRANCE_DOOR_WAIL, "You grasp the handle of the entrance door and pull, but it resists your efforts, refusing to budge. The door isn’t locked, yet an unseen force seems to hold it fast, as if something is determined to keep you from escaping. As you struggle, eerie whispers slither through the air, echoing around you, chilling your spine. Each tug and pull only intensifies the haunting sounds, urging you to stop. Despite your determination, the door remains stubbornly sealed, leaving you with a sense of dread and helplessness."
        }
    };

    // Choices
    public enum ChoicesKeys
    {
        CHANGEPLACE,
        OPEN_ENTRANCE_DOOR,
    }

    public Dictionary<ChoicesKeys, string> ChoicesText = new Dictionary<ChoicesKeys, string>
    {
        {
           ChoicesKeys.CHANGEPLACE,
           "Goto: "
        },
        {
           ChoicesKeys.OPEN_ENTRANCE_DOOR,
           "Do you dare to try opening the entrance door? "
        }
    };

    public Dictionary<ChoicesKeys, string[]> Choices = new Dictionary<ChoicesKeys, string[]>
    {
        {
           ChoicesKeys.CHANGEPLACE,
           new string[] {}
        },
        {
           ChoicesKeys.OPEN_ENTRANCE_DOOR,
           new string[] {
               "Yes", "No"
           }
        },

    };

    // user input
    private ChoicesKeys currentChoices = ChoicesKeys.CHANGEPLACE;

    // game state
    protected Location currentLocation = Location.SECOND_FLOOR_HALLWAY;
    protected ArrayList locationStack = new ArrayList();
    public Location[] unlockedLocation;
    bool triedOpeningEntranceDoor = false;
    
    public Location[] triedLocation; // for displaying whether location is locked or unlocked after player tried it.

    // components initialization
    public readonly Action action;
    public readonly Input input;
    public readonly Display display;
    public readonly DecisionTree decisionTree;

    public Game() {
        // components
        this.display = new Display(this);
        this.decisionTree = new DecisionTree(this);
        this.input = new Input(this);
        this.action = new Action(this);

        // build map
        locationMap.Add(Location.SECOND_FLOOR_HALLWAY, "Second Floor Hallway");
        locationMap.Add(Location.MAIN_STAIRCASE, "Main Staircase");
        locationMap.Add(Location.MASTER_BEDROOM, "Master Bedroom");
        locationMap.Add(Location.NURSERY, "Nursery");
        locationMap.Add(Location.ART_STUDIO, "Art Studio");
        locationMap.Add(Location.GUEST_BEDROOM, "Guest Bedroom");
        locationMap.Add(Location.GALLERY_OVERLOOK, "Gallery Overlook");
        locationMap.Add(Location.GAME_ROOM, "Game Room");
        locationMap.Add(Location.GRAND_FROYER, "The Grand Foyer");
        locationMap.Add(Location.DRAWING_ROOM, "Drawing Room");
        locationMap.Add(Location.DINING_HALL, "Dining Hall");
        locationMap.Add(Location.GALLERY, "Gallery");
        locationMap.Add(Location.ENTRANCE, "Mansion Entrance Door");

        CreateLocation(); // initialize creation.

        locationStack.Add(locationMap[currentLocation]);

        // start unlocked locations
        int totalLocation = Enum.GetValues(typeof(Location)).Length;
        unlockedLocation = new Location[totalLocation];
        triedLocation = new Location[totalLocation];

        for (int i = 0; i < totalLocation; i++) {
            unlockedLocation[i] = Location.LOCKED;
        }

        UnlockLocation(currentLocation);
        UnlockLocation(Location.NURSERY);
        UnlockLocation(Location.MAIN_STAIRCASE);
        UnlockLocation(Location.GRAND_FROYER);
        UnlockLocation(Location.ENTRANCE);

        // initialize game
        decisionTree.ExecuteDecisionPlan();
        action.ExecutePlan();
        input.AskForInput();
    }


    protected static void ConstructPath(Location from, string to)
    {
        bool pathExists = locations.ContainsKey(from);

        if (pathExists)
        {
            ArrayList paths = locations[from];

            paths.Add(to);
        } else
        {
            ArrayList initialPaths = new ArrayList()
            {
                to
            };
            locations.Add(from, initialPaths);
        }
    }

    protected static void CreateLocation()
    {
        for (int i = 0; i < directions.GetLength(0); i++)
        {
            Location from = directions[i, 0];
            Location to = directions[i, 1];

            ConstructPath(from, locationMap[to]);
            ConstructPath(to, locationMap[from]);
        }
    }

    // get the direction player can move on a given location.
    protected static string[] GetDirections(Location location)
    {
        return (string[]) locations[location].ToArray(typeof(string));
    }

    // Location Methods
    public Location GetCurrentLocation()
    {
        return currentLocation;
    }

    public void SetCurrentLocation(Location location)
    {
        int locationIndex = (int)location;

        // for logging of the location tried by the player.
        if (!triedLocation[locationIndex].Equals(Location.TRIED))
        {
            TryLocation(location);
        } 

        if (unlockedLocation[locationIndex].Equals(Location.LOCKED))
        {
            action.AddToStack(new InputMapping(Input.InputState.MONOLOGUES, Game.MonologueKeys.LOCATIONLOCKED));
            return;
        }
        currentLocation = location;
        locationStack.Add(locationMap[location]);
    }

    public void UnlockLocation(Location location)
    {
        unlockedLocation[(int)location] = location;
    }

    public void TryLocation(Location location)
    {
        triedLocation[(int)location] = location;
    }

    // Choices Methods
    public string[] GetCurrentChoices()
    {
        switch (currentChoices)
        {
            case ChoicesKeys.CHANGEPLACE:
                {
                    return GetDirections(currentLocation);
                }

        }
        return Choices[currentChoices];
    }


    // Choices Methods
    public ChoicesKeys GetCurrentChoice()
    {
        return currentChoices;
    }

    public string GetCurrentChoicesText()
    {
        return ChoicesText[currentChoices]; 
    }

    public void SetCurrentChoices(ChoicesKeys choicesKeys)
    {
        currentChoices = choicesKeys;
    }

    public void MakeChoice(int chooseLevel)
    {
        string[] choices = GetCurrentChoices();
        string choice = choices[chooseLevel];

        switch (currentChoices)
        {
            case ChoicesKeys.CHANGEPLACE:
                SetCurrentLocation(locationMap[choice]);
                break;
            case ChoicesKeys.OPEN_ENTRANCE_DOOR:
                if (choice.Equals("Yes"))
                {
                    triedOpeningEntranceDoor = true;
                }
                break;
        }
    }

    // Retrieve Monologue
    public static string GetMonologue(MonologueKeys key)
    {
        return MonologueTexts[key];
    }

    // Retrieve Game States
    public Dictionary<string, object> GetState()
    {
        return new Dictionary<string, object>
        {
            { "currentLocation", currentLocation },
            { "locationStack", locationStack },
            { "triedOpeningEntranceDoor", triedOpeningEntranceDoor }
        };
    }
    

    // For Debugging
    public void RunDebugging()
    {
        string debuggingText = "--DEBUGGING--";
        debuggingText += "\n\n\nDECISION:\nSTART: " + decisionTree.GetCurrentDecision();
        decisionTree.Update(GetState());
        debuggingText += "\nEND: " + decisionTree.GetCurrentDecision();
        string debugLocationStackString = "\n\n Location Stack:\n" + String.Join(",", locationStack.ToArray(typeof(string)) as string[]);
        debuggingText += debugLocationStackString;
        string debugTriedLocationStackString = "\n\n Tried Location Stack:\n" + String.Join(",", triedLocation);
        debuggingText += debugTriedLocationStackString;
        debuggingText += "\n\nInput State:\n" + input.GetState();

        File.WriteAllText("debugging.txt", debuggingText);
    }

}

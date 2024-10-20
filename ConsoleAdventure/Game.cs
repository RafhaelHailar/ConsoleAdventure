﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using ConsoleAdventureUtils;

public class Game
{
    // Game Location
    public enum Location
    {
        SECOND_FLOOR_HALLWAY,
        MAIN_STAIRCASE,
        MASTER_BEDROOM,
        NURSERY,
        ART_STUDIO,
        GUEST_BEDROOM,
        GALLERY_OVERLOOK,
        GAME_ROOM
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
    };

    public enum MonologueKeys
    {
        INTRO,
        INTHECENTER
    }

    private static readonly Dictionary<MonologueKeys, string> MonologueTexts = new Dictionary<MonologueKeys, string>
    {
        {
            MonologueKeys.INTRO,"You awaken in a vast, dimly lit mansion, its towering ceilings and ornate decor both magnificent and foreboding. Shadows stretch across the walls, whispering secrets you can’t quite grasp. The air is thick with an unsettling silence, broken only by the distant creak of old wood beneath your feet.\r\n\r\nYou search for clues about your identity, but your mind is a blank canvas, void of memories. As you take a cautious step forward, a chill runs down your spine. What lies within these walls? Are the eerie sounds mere echoes of your imagination, or do they hint at something lurking just beyond your sight? Your journey begins now—face your fears and uncover the truth hidden in the depths of this mansion. Will you discover who you are, or will the shadows consume you?\r\n\r\n\r\n\r\n"
        },
        {
            MonologueKeys.INTHECENTER,"You are now in the center of the mansion!"
        },
    };

    public enum ChoicesKeys
    {
        CHANGEPLACE,
        NAME
    }

    public Dictionary<ChoicesKeys, string> ChoicesText = new Dictionary<ChoicesKeys, string>
    {
        {
           ChoicesKeys.CHANGEPLACE,
           "Goto: "
        },
        {
           ChoicesKeys.NAME,
           "Choose your name: "
        }
    };

    public Dictionary<ChoicesKeys, string[]> Choices = new Dictionary<ChoicesKeys, string[]>
    {
        {
           ChoicesKeys.CHANGEPLACE,
           new string[]{}
        },
        {
           ChoicesKeys.NAME,
           new string[] { "Manuel", "Pedro", "Kawowski" }
        }
    };

    // user input
    private ChoicesKeys currentChoices = ChoicesKeys.CHANGEPLACE;

    // game state
    protected static Location currentLocation = Location.SECOND_FLOOR_HALLWAY;
    protected static ArrayList locationStack = new ArrayList();

    // components initialization
    public readonly Input input;
    public readonly Display display;
    public readonly DecisionTree decisionTree;

    public Game() {
        // components
        this.display = new Display();
        this.decisionTree = new DecisionTree(this);
        this.input = new Input(this);

        // build map
        locationMap.Add(Location.SECOND_FLOOR_HALLWAY, "Second Floor Hallway");
        locationMap.Add(Location.MAIN_STAIRCASE, "Main Staircase");
        locationMap.Add(Location.MASTER_BEDROOM, "Master Bedroom");
        locationMap.Add(Location.NURSERY, "Nursery");
        locationMap.Add(Location.ART_STUDIO, "Art Studio");
        locationMap.Add(Location.GUEST_BEDROOM, "Guest Bedroom");
        locationMap.Add(Location.GALLERY_OVERLOOK, "Gallery Overlook");
        locationMap.Add(Location.GAME_ROOM, "Game Room");
        
        CreateLocation(); // initialize creation.

        locationStack.Add(locationMap[currentLocation]);

        // initialize game
        decisionTree.ExecuteDecisionPlan();
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
        }
    }

    // get the direction player can move on a given location.
    protected static string[] GetDirections(Location location)
    {
        return (string[])locations[location].ToArray(typeof(string));
    }

    public static Location GetCurrentLocation()
    {
        return currentLocation;
    }

    public void SetCurrentLocation(Location location)
    {
        currentLocation = location;
        locationStack.Add(locationMap[location]);
    }

    public string[] GetCurrentChoices()
    {
        switch (currentChoices)
        {
            case ChoicesKeys.CHANGEPLACE:
                return GetDirections(currentLocation);
        }
        return Choices[currentChoices];
    }
    public string GetCurrentChoicesText()
    {
        return ChoicesText[currentChoices]; 
    }

    public void SetCurrentChoices(ChoicesKeys choicesKeys)
    {
        currentChoices = choicesKeys;
    }

    public void MakeChoice(string choice)
    {
        if (choice == null) throw new ArgumentNullException();

        string[] choices = GetCurrentChoices();

        bool exists = false;

        for (int i = 0; i < choices.Length; i++)
        {
            if (choices[i].Equals(choice))
            {
                exists = true;
            }

        }

        if (!exists) throw new Exception("choice given is not part of choices!");

        switch (currentChoices)
        {
            case ChoicesKeys.CHANGEPLACE:
                SetCurrentLocation(locationMap[choice]);
                break;
            case ChoicesKeys.NAME:
                Console.WriteLine(choice);
                break;
        }
    }

    // Retrieve Monologue
    public static string GetMonologue(MonologueKeys key)
    {
        return MonologueTexts[key];
    }

    // Retrieve Game States
    public static Dictionary<string, object> GetState()
    {
        return new Dictionary<string, object>
        {
            { "currentLocation", currentLocation },
            { "locationStack", locationStack }
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
        debuggingText += "\n\nInput State:\n" + input.getState();

        File.WriteAllText("debugging.txt", debuggingText);
    }

}

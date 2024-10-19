using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;

public class Game
{
    protected static Dictionary<string, ArrayList> locations = new Dictionary<string, ArrayList>();

    protected static string[,] directions =
    {
        { "center", "spacious living room" },
        { "center", "wine room" },
        { "spacious living room", "center" },
        { "spacious living room", "family history room" },
        { "spacious living room", "study room" },
        { "study room", "spacious living room" },
        { "family history room", "spacious living room" },
        { "family history room", "wine room" },
        { "wine room", "center" },
    };

    public enum MonologueKeys
    {
        INTRO,
        INTHECENTER
    }

    private static readonly Dictionary<MonologueKeys, string> MonologueTexts = new Dictionary<MonologueKeys, string>
    {
        {
            MonologueKeys.INTRO,"Welcome, to the game of console"
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
    protected static string currentLocation = "center";
    protected static ArrayList locationStack = new ArrayList() { currentLocation };

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
        CreateLocation();

        //foreach (KeyValuePair<string, ArrayList> location in locations)
        //{
        //    ArrayList paths = location.Value;
        //    Console.Write("{0} -> ",
        //              location.Key);

        //    for (int i = 0; i < paths.Count; i++)
        //    {
        //        Console.Write("{0}, ", paths[i]);
        //    }

        //    Console.WriteLine("");
        //}

        // initialize game
        decisionTree.ExecuteDecisionPlan();
        input.AskForInput();
    }

    protected static void ConstructPath(string from, string to)
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
            string from = directions[i, 0];
            string to = directions[i, 1];

            ConstructPath(from, to);
        }
    }

    // get the direction player can move on a given location.
    protected static string[] GetDirections(string location)
    {
        return (string[])locations[location].ToArray(typeof(string));
    }

    public static string GetCurrentLocation()
    {
        return currentLocation;
    }

    public void SetCurrentLocation(string location)
    {
        currentLocation = location;
        locationStack.Add(location);
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
                SetCurrentLocation(choice);
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

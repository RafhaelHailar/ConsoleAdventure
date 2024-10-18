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
    public enum ChoicesKeys
    {
        CHANGEPLACE
    }

    // user input
    static private ChoicesKeys currentChoices = ChoicesKeys.CHANGEPLACE;
    private string userInput = "";

    // game state
    protected static string currentLocation = "center";
    protected static ArrayList locationStack = new ArrayList() { currentLocation };
    private readonly Input input;

    // components initialization
    private static readonly Display display = new Display();
    static DecisionTree decisionTree = new DecisionTree();

    public Game() {
        CreateLocation();

        input = new Input(display, decisionTree);

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

    protected static string[] GetDirections(string location)
    {
        return (string[])locations[location].ToArray(typeof(string));
    }


    public static Dictionary<ChoicesKeys, string[]> Choices = new Dictionary<ChoicesKeys, string[]>
    {
        {
           ChoicesKeys.CHANGEPLACE,
           new string[]{}
        }
    };

    public static string GetCurrentLocation()
    {
        return currentLocation;
    }

    public static void SetCurrentLocation(string location)
    {
        currentLocation = location;
        locationStack.Add(location);
    }

    public static string[] GetCurrentChoices()
    {
        switch (currentChoices)
        {
            case ChoicesKeys.CHANGEPLACE:
                return GetDirections(currentLocation);
        }
        return Choices[currentChoices];
    }

    public static void SetCurrentChoices(ChoicesKeys choicesKeys)
    {
        currentChoices = choicesKeys;
    }

    public static void MakeChoice(string choice)
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
        }
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
    public static void RunDebugging()
    {
        string debuggingText = "--DEBUGGING--";
        debuggingText += "\n\n\nDECISION:\nSTART: " + decisionTree.GetCurrentDecision();
        decisionTree.Update(GetState());
        debuggingText += "\nEND: " + decisionTree.GetCurrentDecision();
        string debugLocationStackString = "\n\n Location Stack:\n" + String.Join(",", locationStack.ToArray(typeof(string)) as string[]);
        debuggingText += debugLocationStackString;
        File.WriteAllText("debugging.txt", debuggingText);
    }

}

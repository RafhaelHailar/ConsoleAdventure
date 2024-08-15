using System;
using System.Collections.Generic;
using System.Collections;

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

    static private ChoicesKeys currentChoices = ChoicesKeys.CHANGEPLACE;

    protected static string currentLocation = "center";

    private string userInput = "";

    private readonly Display display = new Display();
    private readonly Input input;

    public Game() {
        CreateLocation();

        input = new Input(display);

        foreach (KeyValuePair<string, ArrayList> location in locations)
        {
            ArrayList paths = location.Value;
            Console.Write("{0} -> ",
                      location.Key);

            for (int i = 0; i < paths.Count; i++)
            {
                Console.Write("{0}, ", paths[i]);
            }

            Console.WriteLine("");
        }

        input.askForInput();
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

    protected static string[] getDirections(string location)
    {
        return (string[]) locations[location].ToArray(typeof(string));
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
    }

    public static string[] getCurrentChoices()
    {
        switch (currentChoices)
        {
            case ChoicesKeys.CHANGEPLACE:
                return getDirections(currentLocation);
        }
        return Choices[currentChoices];
    }

    public static void setCurrentChoices(ChoicesKeys choicesKeys)
    {
        currentChoices = choicesKeys;
    }

    public static void MakeChoice(string choice)
    {
        if (choice == null) throw new ArgumentNullException();

        string[] choices = getCurrentChoices();

        bool exists = false;

        for (int i = 0; i < choices.Length; i++)
        {
            if (choices[i].Equals(choice))
            {
                exists = true;
            }

        }

        if (!exists) throw new Exception("choice given is not part of choices!");

 
    }
}

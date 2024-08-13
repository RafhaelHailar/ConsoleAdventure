using System;
using System.Collections.Generic;
using System.Collections;

public class Game
{
    protected Dictionary<string, ArrayList> locations = new Dictionary<string, ArrayList>();

    protected string[,] directions =
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

    private string currentLocation = "center";

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

            for ( int i = 0; i < paths.Count; i++ )
            {
                Console.Write("{0}, ",paths[i]);
            }

            Console.WriteLine("");
        }

        input.askForInput();
    }

    protected void ConstructPath(string from, string to)
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

    protected void CreateLocation()
    {
        for (int i = 0; i < directions.GetLength(0); i++)
        {
            string from = directions[i, 0];
            string to = directions[i, 1];

            ConstructPath(from, to);
        }
    }


    private Dictionary<ChoicesKeys, string[]> Choices = new Dictionary<ChoicesKeys, string[]>
    {
        {
           ChoicesKeys.CHANGEPLACE,
           new string[]
           {
             "********************",
             "********************",
             "********************",
           }
        }
    };
}

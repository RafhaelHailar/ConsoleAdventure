using System;
using System.Collections.Generic;
using System.Collections;
using System.Data.Common;

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

    private string currentLocation = "center";

    private string userInput = "";

    private Display display = new Display();

    public Game() {
        createLocation();

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

        display.displayText(Display.DisplayTextsKeys.INTRO);

        askForInput();
    }

    protected void askForInput()
    {
        ConsoleKeyInfo name = Console.ReadKey();

        if (name.Key != ConsoleKey.Enter)
        {
            Console.Clear();
            display.reDisplayText();
        }
        else
        {
            display.endDisplayText();
        }

        askForInput();
    }

    protected void constructPath(string from, string to)
    { 
        bool pathExists = locations.ContainsKey(from);

        if (pathExists)
        {
            ArrayList paths = locations[from];

            paths.Add(to);
        } else
        {
            ArrayList initialPaths = new ArrayList();
            initialPaths.Add(to);
            locations.Add(from, initialPaths);
        }
    }

    protected void createLocation()
    {
        for (int i = 0; i < directions.GetLength(0); i++)
        {
            string from = directions[i, 0];
            string to = directions[i, 1];

            constructPath(from, to);
        }
    }
}

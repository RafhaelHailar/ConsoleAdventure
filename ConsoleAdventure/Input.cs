using System;
using System.Collections.Generic;
using System.Collections;

public class Input
{
    public enum UserState
    {
        MONOLOGUES,
        CHOOSING
    }

    private UserState userState = UserState.MONOLOGUES;

    Display display;
    DecisionTree decisionTree;

    private int chooseLevel = 0;
    public Input(Display display, DecisionTree decisionTree) {
        this.display = display;
        this.decisionTree = decisionTree;

    //    display.DisplayText(Display.DisplayTextsKeys.INTRO);
    } 

    public void AskForInput()
    {
        ConsoleKeyInfo name = Console.ReadKey();
        
        Console.Clear();

        switch (userState)
        {
            case UserState.MONOLOGUES:
                if (name.Key == ConsoleKey.Enter || name.Key == ConsoleKey.Spacebar)
                {
                    if (!display.IsDisplaying())
                    {
                        Console.WriteLine("Ended");
                    } else
                    {
                        userState = UserState.CHOOSING;
                        display.EndDisplayText();
                    }
                } else if (!display.IsDisplaying())
                {
                    display.ReDisplayText();
                  //  Console.WriteLine(name.Key);
                } 
                break;
            case UserState.CHOOSING:
                switch (name.Key)
                {
                    case ConsoleKey.Enter:
                    case ConsoleKey.Spacebar:
                        //Console.WriteLine("You are at {0} goto: (press \\Enter or \\Space to choose) \n", Game.GetCurrentLocation());
                        string[] choose = Game.GetCurrentChoices();
                        Game.MakeChoice(choose[chooseLevel]);
                        chooseLevel = 0;

                        //Dictionary<string, object> state = Game.GetState();
                        //decisionTree.Update(state);

                        // For Debugging;
                        Game.RunDebugging();

                        break;
                    case ConsoleKey.UpArrow:
                        MoveChoice(-1);
                        break;
                    case ConsoleKey.DownArrow:
                        MoveChoice(1);
                        break;  
                }
                Console.WriteLine("You are at {0} \n", Game.GetCurrentLocation());
                Console.WriteLine("Press \\Enter or \\Space to choose");
                Console.WriteLine("Goto: ");
                display.DisplayChoices(Game.GetCurrentChoices(), chooseLevel);
                break;
        }
        AskForInput();
    }

    protected void MoveChoice(int i)
    {
        int newChooseLevel = i + chooseLevel;
        string[] choose = Game.GetCurrentChoices();

        if (newChooseLevel < 0) newChooseLevel = 0;
        if (newChooseLevel >= choose.Length) newChooseLevel = choose.Length - 1;

        chooseLevel = newChooseLevel;
    }
}

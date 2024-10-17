using System;
using System.Collections.Generic;

public class Input
{
    public enum UserState
    {
        MONOLOGUES,
        CHOOSING
    }

    private UserState userState = UserState.CHOOSING;

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
                if (name.Key == ConsoleKey.Enter && !display.IsDisplaying())
                {
                    Console.WriteLine("Ended");
                }

                if (name.Key != ConsoleKey.Enter || !display.IsDisplaying())
                {
                    display.ReDisplayText();
                    Console.WriteLine(name.Key);
                }
                else
                {
                    display.EndDisplayText();
                }
                break;
            case UserState.CHOOSING:
                Console.WriteLine("You are at {0} goto: (press \\Enter or \\Space to choose) \n", Game.GetCurrentLocation());
                 
                switch (name.Key)
                {
                    case ConsoleKey.Enter:
                    case ConsoleKey.Spacebar:
                        string[] choose = Game.GetCurrentChoices();
                        Game.MakeChoice(choose[chooseLevel]);
                        chooseLevel = 0;
                        break;
                    case ConsoleKey.UpArrow:
                        MoveChoice(-1);
                        break;
                    case ConsoleKey.DownArrow:
                        MoveChoice(1);
                        break;
                }

                display.DisplayChoices(Game.GetCurrentChoices(), chooseLevel);
                break;
        }
        //Console.WriteLine("\n\n\n\n\n\n{0}", decisionTree.GetCurrentDecision());
        decisionTree.Update(Game.GetState());
        //Console.WriteLine("\n\n\n\n\n\n{0}", decisionTree.GetCurrentDecision());
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

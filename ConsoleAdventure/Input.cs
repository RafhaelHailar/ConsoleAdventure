using System;

public class Input
{
    public enum UserState
    {
        MONOLOGUES,
        CHOOSING
    }

    private UserState userState = UserState.CHOOSING;

    Display display;

    private int chooseLevel = 0;
    public Input(Display display) {
        this.display = display;

        //display.DisplayText(Display.DisplayTextsKeys.INTRO);
    } 

    public void askForInput()
    {
        ConsoleKeyInfo name = Console.ReadKey();
        
        Console.Clear();

        switch (userState)
        {
            case UserState.MONOLOGUES:
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
                Console.WriteLine("You are at {0} goto: \n", Game.GetCurrentLocation());
                 
                switch (name.Key)
                {
                    case ConsoleKey.Enter:
                        string[] choose = Game.getCurrentChoices();
                        Game.MakeChoice(choose[chooseLevel]);
                        break;
                    case ConsoleKey.UpArrow:
                        MoveChoice(-1);
                        break;
                    case ConsoleKey.DownArrow:
                        MoveChoice(1);
                        break;
                }

                display.displayChoices(Game.getCurrentChoices(), chooseLevel);
                break;
        }

        askForInput();
    }

    protected void MoveChoice(int i)
    {
        int newChooseLevel = i + chooseLevel;
        string[] choose = Game.getCurrentChoices();

        if (newChooseLevel < 0) newChooseLevel = 0;
        if (newChooseLevel >= choose.Length) newChooseLevel = choose.Length - 1;

        chooseLevel = newChooseLevel;
    }
}

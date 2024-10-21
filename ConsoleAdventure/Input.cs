using System;

/**
 * InputMapping Class, map the action with the value it will be perform along.
 * 
 */
public struct InputMapping
{
    public Input.InputState State { get; }
    public object Key { get; }

    public InputMapping(Input.InputState state, object key)
    {
        State = state;
        Key = key;
    }
}

/**
 * Input Class, take player keyboard inputs and perform the action the input does.
 *  - move choices if player is choosing.
 */
public class Input
{
    // input state
    public enum InputState
    {
        MONOLOGUES,
        CHOOSING
    }

    private InputState inputState = InputState.MONOLOGUES;

    // component
    readonly Game game;

    private int chooseLevel = 0;
    public Input(Game game) {
        // component
        this.game = game;
    } 

    public void AskForInput()
    {
        ConsoleKeyInfo name = Console.ReadKey();
        Console.Clear();

        switch (inputState)
        {
            case InputState.MONOLOGUES:
                if (name.Key == ConsoleKey.Enter || name.Key == ConsoleKey.Spacebar)
                {
                    if (!game.display.IsDisplaying())
                    {
                        game.action.ExecutePlan();
                        if (inputState == InputState.CHOOSING)
                        {
                            TriggerChoicesDisplay();
                        }
                    } else
                    {
                       game.display.EndDisplayText();
                       Console.WriteLine("\n\n[Press Enter or Space to Continue]");
                    }
                } else if (!game.display.IsDisplaying())
                {
                    game.display.ReDisplayText();
                }
                break;
            case InputState.CHOOSING:
                switch (name.Key)
                {
                    case ConsoleKey.Enter:
                    case ConsoleKey.Spacebar:
                        //Console.WriteLine("You are at {0} goto: (press \\Enter or \\Space to choose) \n", Game.GetCurrentLocation());
                        //string[] choose = game.GetCurrentChoices();
                        game.MakeChoice(chooseLevel);
                        chooseLevel = 0;

                        //Dictionary<string, object> state = game.GetState();
                        //game.decisionTree.Update(state);

                        game.action.ExecutePlan();

                        break;
                    case ConsoleKey.UpArrow:
                        MoveChoice(-1);
                        break;
                    case ConsoleKey.DownArrow:
                        MoveChoice(1);
                        break;  
                }

                TriggerChoicesDisplay();
                break;
        }
        // For Debugging;
        game.RunDebugging();
        AskForInput();
    }

    protected void MoveChoice(int i)
    {
        int newChooseLevel = i + chooseLevel;
        string[] choose = game.GetCurrentChoices();

        if (newChooseLevel < 0) newChooseLevel = 0;
        if (newChooseLevel >= choose.Length) newChooseLevel = choose.Length - 1;

        chooseLevel = newChooseLevel;
    }

    private void TriggerChoicesDisplay()
    {
        game.display.DisplayChoices(chooseLevel);
    }

    public void SetState(InputState state)
    {
        inputState = state;
    }


    public InputState GetState()
    {
        return inputState;
    }
}

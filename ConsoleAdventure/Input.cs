using System;
using System.Collections.Generic;
using System.Collections;

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
                        game.decisionTree.ExecuteDecisionPlan();
                        if (inputState == InputState.CHOOSING)
                        {
                            TriggerChoicesDisplay();
                        }
                    } else
                    {
                       game.display.EndDisplayText();
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
                        string[] choose = game.GetCurrentChoices();
                        game.MakeChoice(choose[chooseLevel]);
                        chooseLevel = 0;

                        //Dictionary<string, object> state = Game.GetState();
                        //decisionTree.Update(state);

                        game.decisionTree.ExecuteDecisionPlan();

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
        game.display.DisplayChoices(game.GetCurrentChoicesText(), game.GetCurrentChoices(), chooseLevel);
    }

    public void SetState(InputState state)
    {
        inputState = state;
    }


    public InputState getState()
    {
        return inputState;
    }
}

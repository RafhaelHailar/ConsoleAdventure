﻿using System;
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

    // monologue is displaying
    bool isMonologueDisplaying = true;

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
                    if (!isMonologueDisplaying)
                    {
                        game.decisionTree.ExecuteDecisionPlan();
                        if (inputState == InputState.CHOOSING)
                        {
                            game.display.DisplayChoices(game.GetCurrentChoices(), chooseLevel);
                        }
                        isMonologueDisplaying = true;
                    } else
                    {
                        isMonologueDisplaying = false;
                        game.display.EndDisplayText();
                    }
                } else if (!game.display.IsDisplaying())
                {
                    game.display.ReDisplayText();
                    isMonologueDisplaying = false;
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

                game.display.DisplayChoices(game.GetCurrentChoices(), chooseLevel);
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

    public void SetState(InputState state)
    {
        inputState = state;
    }

    public InputState getState()
    {
        return inputState;
    }
}

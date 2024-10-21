using System;
using System.Collections.Generic;

/**
 * 
 * Action - holds the actions needed to be executed.
 * - holds the plan execution stack.
 * - execute the plan.
 */
public class Action
{
    // plan execution stack
    private Stack<InputMapping> planExecutionStack = new Stack<InputMapping>();

    // component
    protected readonly Game game;

    public Action(Game game){
        this.game = game;
    }

    // add to stack
    public void AddToStack(InputMapping input)
    {
        planExecutionStack.Push(input);
    }

    // execute plan 
    public void ExecutePlan()
    {
        if (planExecutionStack.Count == 0) return;
        InputMapping currentPlan = planExecutionStack.Pop();

        switch (currentPlan.State)
        {
            case Input.InputState.MONOLOGUES:
                game.input.SetState(Input.InputState.MONOLOGUES);
                game.display.DisplayText(Game.GetMonologue((Game.MonologueKeys)currentPlan.Key));
                break;
            case Input.InputState.CHOOSING:
                game.input.SetState(Input.InputState.CHOOSING);
                game.SetCurrentChoices((Game.ChoicesKeys)currentPlan.Key);
                break;
            default:
                throw new Exception("Given State Is Invalid!");
        }
    }
}

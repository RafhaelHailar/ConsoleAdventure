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

    public Action(){}

    // add to stack
    public void AddToStack(InputMapping input)
    {
        planExecutionStack.Push(input);
    }

    // execute plan 
    public void ExecutePlan()
    {
        InputMapping input = planExecutionStack.Pop();

    }
}

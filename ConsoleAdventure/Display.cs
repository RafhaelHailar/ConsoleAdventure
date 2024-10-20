
using System;
using System.Timers;

/**
 * Dispaly Class, as its name implies display what player sees in the terminal.
 * - text animation
 * - choices ui
 */
public class Display
{
    private static Timer aTimer = new Timer();
    private int pointer = 0;
    private int counter = 0;
    private string currentText = "";
    private string previousText = null;

    // component
    Game game;

    public Display(Game game)
    {
        this.game = game;
    }

    public void DisplayText(string text)
    {
        aTimer.Dispose();
        aTimer.Stop();
        currentText = text;
        previousText = currentText;
        AnimateText();
    }

    private void AnimateText()
    {
        aTimer = new Timer(70);
        aTimer.Elapsed += ShowTimeText;
        aTimer.Enabled = true;
    }

    private void ShowTimeText(Object source, ElapsedEventArgs e)
    {
        counter++;
        if (pointer == currentText.Length)
        {
            StopTimer();
            ResetDisplayText();
            return;
        }

        Console.Clear();

        char symbol = currentText[pointer];

        Console.WriteLine(currentText.Substring(0, pointer) + symbol);
        pointer++;
    }

    private void StopTimer()
    {
        aTimer.Stop();
        aTimer.Dispose();
    }

    private void ResetDisplayText()
    {
        pointer = 0;
        counter = 0;
        currentText = "";
    }

    public void ReDisplayText()
    {
        Console.WriteLine(previousText);
    }

    public bool IsDisplaying()
    {
        return currentText != null && currentText != "";
    }

    public void EndDisplayText()
    {
        if (!IsDisplaying()) return;

        StopTimer();
        Console.WriteLine(currentText);
        ResetDisplayText();
    }

    public void DisplayChoices(string chooseText, string[] choose, int chooseLevel)
    {
        Console.WriteLine("You are at {0} \n", Game.locationMap[game.GetCurrentLocation()]);
        Console.Write(chooseText);
        Console.Write("(Press \\Enter or \\Space to choose)\n");
        for (int i = 0; i < choose.Length; i++)
        {
            string line = choose[i];
            char value = chooseLevel == i ? '*' : ' ';
            Console.WriteLine("[{0}] {1}", value, line);
        }
    }
}
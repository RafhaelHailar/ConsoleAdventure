
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Timers;

public class Display
{
    private static System.Timers.Timer aTimer;
    private int pointer = 0;
    private int counter = 0;
    private string currentText = "";
   
    public Display()
    {
   
    }

    public void displayText(string target)
    {
        currentText = DisplayTexts[target];
        animateText();
    }

    private void animateText()
    {
        aTimer = new System.Timers.Timer(70);
        aTimer.Elapsed += showTimeText;
        aTimer.Enabled = true;
    }

    private void showTimeText(Object source, ElapsedEventArgs e)
    {
        counter++;
        if (pointer == currentText.Length)
        {
            aTimer.Stop();
            aTimer.Dispose();
            pointer = 0;
            counter = 0;
            currentText = "";
            return;
        }

        char symbol = currentText[pointer];
        Console.Write(symbol);
        pointer++;
    }

    static Dictionary<string, string> DisplayTexts = new Dictionary<string, string>
    {
        {
            "intro","Welcome, to the game of console"
        }
    };
}
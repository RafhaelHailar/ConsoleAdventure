
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Timers;

public class Display
{
    private static System.Timers.Timer aTimer = new System.Timers.Timer();
    private int pointer = 0;
    private int counter = 0;
    private string currentText = "";
    private string previousText = null;

    public enum DisplayTextsKeys
    {
        INTRO = 0,
        INTHECENTER
    }

    public Display()
    {
       
    }

    public void displayText(DisplayTextsKeys target)
    {
        aTimer.Dispose();
        aTimer.Stop();
        currentText = DisplayTexts[target];
        previousText = currentText;
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
            stopTimer();
            resetDisplayText();
            return;
        }

        Console.Clear();

        char symbol = currentText[pointer];

        Console.WriteLine(currentText.Substring(0, pointer) + symbol);
        pointer++;
    }

    private void stopTimer()
    {
        aTimer.Stop();
        aTimer.Dispose();
    }

    private void resetDisplayText()
    {
        pointer = 0;
        counter = 0;
        currentText = "";
    }

    public void reDisplayText()
    {
        Console.WriteLine(previousText);
    }

    public bool isDisplaying()
    {
        return currentText != null && currentText != "";
    }

    public void endDisplayText()
    {
        if (!isDisplaying()) return;

        stopTimer();
        Console.WriteLine(currentText);
    }

    static Dictionary<DisplayTextsKeys, string> DisplayTexts = new Dictionary<DisplayTextsKeys, string>
    {
        {
            DisplayTextsKeys.INTRO,"Welcome, to the game of console"
        },
        {
            DisplayTextsKeys.INTHECENTER,"Welcome, to the game of console"
        },
    };
}
﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Console;


namespace PalmParadiseHotelApp.Helpers
{
    public class ConsoleHelper
    {
        public static void PrintHeader(string title)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Blue;

            string border = new string('=', 40); // Lång linje som ram
            Console.WriteLine(border);
            Console.WriteLine($"{"",15}{title}"); // Centrera titeln med blanksteg
            Console.WriteLine(border);

            Console.ResetColor();
        }

        public static void PrintMenuOption(string optionNumber, string description)
        {
            Console.WriteLine($"   {optionNumber}. {description}");
        }

        public static void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {message}");
            Console.ResetColor();
        }

        public static void PrintAsciiTitle(string[] asciiLines)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;

            // Säkerställ att fönstret är tillräckligt brett
            int requiredWidth = asciiLines.Max(line => line.Length) + 10; // Extra marginal
            if (Console.WindowWidth < requiredWidth)
            {
                Console.SetWindowSize(Math.Min(requiredWidth, Console.LargestWindowWidth), Console.WindowHeight);
            }

            // Centrera ASCII-text
            foreach (var line in asciiLines)
            {
                int padding = (Console.WindowWidth - line.Length) / 2;
                Console.WriteLine($"{new string(' ', Math.Max(padding, 0))}{line}");
            }

            Console.ResetColor();
            Console.WriteLine("\n");
        }
        public static void PrintSpectreTitle(string title)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(
    new FigletText("Palm Paradise Hotel")
        .Centered()
        .Color(Spectre.Console.Color.Aqua)); // Använd alias


        }
    }
}

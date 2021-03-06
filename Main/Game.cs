﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Main
{
    static class Game
    {
        private static List<Player> Players = new List<Player>();

        public static void StartGame()
        {
            StartScreen();
            InitGame();
            Tick();
            EndScreen();
        }

        private static void StartNewGame()
        {
            InitGame();
            Tick();
            EndScreen();
        }


        private static void StartScreen()
        {
            Console.SetBufferSize(300, 300);
            int width = Console.WindowWidth * 2;
            int height = Console.WindowHeight * 2;

            Console.SetWindowSize(width, height);
            

            PrintTextFile(@"..\..\TeamCanrum.txt");
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            PrintTextFile(@"..\..\BloodyJudge.txt");
            Console.ResetColor();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();


        }

        private static void EndScreen()
        {
            PrintTextFile(@"..\..\Thanks.txt");
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            PrintTextFile(@"..\..\BloodyJudge.txt");
            Console.ResetColor();
            Console.WriteLine("Press the 'any' key to quit game or 'Enter' to start a new one...");
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Enter)
            {
                StartNewGame();
            }

        }

        private static void InitGame()
        {
            Console.Write("Number of players: ");
            int numberOfPlayers = Int32.Parse(Console.ReadLine());

            for (int i = 0; i < numberOfPlayers; i++)
            {
                Console.Write("Enter player name: ");
                Players.Add(new Player(Console.ReadLine()));
            }
        }

        private static void Tick()
        {
            if (Players.Count > 1)
            {
                MultiPlayer();
            }
            else
            {
                SinglePlayer();
            }

        }

        private static void SinglePlayer()
        {
            var player = Players[0];

            for (player.Level = 1; player.Level <= 5; player.Level++)
            {
                Boss boss = player.GetCurrentBoss();
                if (boss.Name != "Nakov")
                {
                    Console.WriteLine("{0} suddenly steps in front of you, blocking your way.", boss.Name);
                    BossSpeaks(boss);
                    Console.WriteLine(" The way is shut, stranger! If you want passage you must complete the following task.");
                    BossSpeaks(boss);
                    Console.WriteLine(" If you succeed, I'll grant you passage and the knowledge of {0}.\n", boss.Award);

                    bool? completedSuccessfully = player.CompleteMission();
                    if (completedSuccessfully == true)
                    {
                        PrintBlinkingTextFile(@"..\..\Correct.txt", ConsoleColor.Green);
                        BossSpeaks(boss);
                        Console.WriteLine(" Well done, lad! Here's your promised award! Go on!");
                        BossSpeaks(boss);
                        Console.WriteLine(" You now possess the knowledge of {0}", boss.Award);
                        player.Awards.Add(boss.Award);
                    }
                    else
                    {
                        PrintBlinkingTextFile(@"..\..\Incorrect.txt", ConsoleColor.Red);
                        BossSpeaks(boss);
                        Console.WriteLine(" Your answer is incorrect! I'll let you go further, but do you consider yourself worthy?");
                    }
                }
                else
                {
                    Console.WriteLine("As you step carefully along the road, a dreadful, hideous creature arises from the shadows.");
                    Console.WriteLine("The air grows ripe with the stench of death and misery.");
                    Console.WriteLine("Your heart starts beating wildly, pumping blood into your head.");
                    Console.WriteLine("You know it's him. There's no turning back now.");
                    BossSpeaks(boss);
                    Console.WriteLine(" So... You think you've got what it takes. Let's find out.");

                    bool? completedSuccessfully = player.CompleteMission();
                    if (completedSuccessfully == true)
                    {
                        PrintBlinkingTextFile(@"..\..\Correct.txt", ConsoleColor.Green);
                        BossSpeaks(boss);
                        Console.WriteLine(" Congratulations! You win!");
                    }
                    else
                    {
                        PrintBlinkingTextFile(@"..\..\Incorrect.txt", ConsoleColor.Red);
                        BossSpeaks(boss);
                        Console.WriteLine(" Go away, maggot! Come back when you consider yourself worthy of speaking with me!");
                    }
                }
                Console.Clear();
            }
        }

        private static void MultiPlayer()
        {
            for (int i = 0; i < 5; i++)
            {
                foreach (var pl in Players)
                {
                    pl.PointsModifier = pl.DoublePoints ? 4 : 2;

                    Boss boss = pl.GetCurrentBoss();
                    Console.Clear();
                    Console.WriteLine(pl.Name + "'s turn.");
                    Console.WriteLine(pl.Name + " meets " + boss.Name);

                    bool? completedSuccessfully = pl.CompleteMission();
                    if (completedSuccessfully == true)
                    {
                        PrintBlinkingTextFile(@"..\..\Correct.txt", ConsoleColor.Green);
                        BossSpeaks(boss);
                        Console.WriteLine(" Well done, lad! Here's your promised award! Go on!");
                        pl.DoublePoints = false;
                        pl.Points += 5*pl.PointsModifier;
                        if (i < 4)
                        {
                            pl.Awards.Add(boss.Award);
                        }
                    }
                    else
                    {
                        if (i == 4)
                        {
                            PrintBlinkingTextFile(@"..\..\Incorrect.txt", ConsoleColor.Red);
                            BossSpeaks(boss);
                            Console.WriteLine(" You have one more chance to prove your worth!");
                        }
                        else
                        {
                            BossSpeaks(boss);
                            Console.WriteLine(" again for half points or next for double?");
                            if (Console.ReadLine() == "again")
                            {
                                pl.PointsModifier = 1;
                                pl.DoublePoints = false;
                            }
                            else
                            {
                                pl.DoublePoints = true;
                                break;
                            }
                        }

                        completedSuccessfully = pl.CompleteMission();
                        if (completedSuccessfully == true)
                        {
                            PrintBlinkingTextFile(@"..\..\Correct.txt", ConsoleColor.Green);
                            BossSpeaks(boss);
                            Console.WriteLine(" Well done, lad! Here's your promised award! Go on!");
                            pl.Points += 5*pl.PointsModifier;
                            if (i < 4)
                            {
                                pl.Awards.Add(boss.Award);
                            }
                        }
                        else
                        {
                            PrintBlinkingTextFile(@"..\..\Incorrect.txt", ConsoleColor.Red);
                            BossSpeaks(boss);
                            Console.WriteLine(" Your answer is incorrect! I'll let you go further, but do you consider yourself worthy?");
                            break;
                        }
                    }
                    pl.Level++;
                }
            }
            CheckWin(Players);
        }

        private static void BossSpeaks(Boss boss)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write("{0}:", boss.Name);
            Console.ResetColor();
        }

        private static void CheckWin(List<Player> players)
        {
            List<string> names = new List<string>();
            names.Add(players[0].Name);
            decimal score = players[0].Points;
            for (int i = 1; i < players.Count; i++)
            {
                if (players[i].Points > score)
                {
                    names.Clear();
                    names.Add(players[i].Name);
                    score = players[i].Points;
                }
                else if (players[i].Points == score)
                {
                    names.Add(players[i].Name);
                }
            }

            if (names.Count > 1)
            {
                Console.WriteLine("{1} are tied for 1st place with {0} points.", score, string.Join(" and ", names));
            }
            else
            {
                Console.WriteLine("{1} wins with {0} points.", score, names.First());
            }
        }

        private static void PrintTextFile(string path)
        {
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    while (sr.Peek() >= 0)
                    {
                        Console.WriteLine(sr.ReadLine());
                        Thread.Sleep(150);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
            Thread.Sleep(2000);
        }

        private static void PrintBlinkingTextFile(string path, ConsoleColor color)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    while (sr.Peek() >= 0)
                    {


                        sb.AppendLine(sr.ReadLine());
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }

            string display = sb.ToString();

            for (int i = 0; i < 11; i++)
            {
                if (i % 2 == 0)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = color;
                    Console.WriteLine(display);
                    Thread.Sleep(150);
                    Console.Clear();
                }
                else
                {
                    Console.BackgroundColor = color;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine(display);
                    Thread.Sleep(150);
                    Console.Clear();
                }
            }
            Console.ResetColor();
            Console.WriteLine(display);
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
        
    }
}

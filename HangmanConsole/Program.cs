using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace HangmanConsole
{
    public class Program
    {
        private static string key = "z3rwz6wBNCmshPfDFV9xoQOZhVctp1VaYSMjsnQBeCnQxuYQHH";

        static void Main(string[] args)
        {
            Console.Title = "HangmanConsole by Yanick Bélanger";
            App().GetAwaiter().GetResult();
        }

        public static async Task App()
        {
            while (true)
            {
                await Jeu();
                char input = Console.ReadKey().KeyChar;
                if (input == 'r')
                {
                    Console.Clear();
                    await Jeu();
                }
                break;
            }
        }

        public static async Task<string> GetRandomWord()
        {
            string apiURL = "https://apifort-random-word-v1.p.mashape.com/v1/generate/randomword?count=1";
            using (HttpClient c = new HttpClient())
            {
                c.DefaultRequestHeaders.Add("Accept", "application/json");
                c.DefaultRequestHeaders.Add("X-Mashape-Key", key);
                string json = await c.GetStringAsync(apiURL);
                WordsAPI.Rootobject result = JsonConvert.DeserializeObject<WordsAPI.Rootobject>(json);
                return result.result[0];
            }
        }

        public static async Task Jeu()
        {
            int lives = 5;
            string motDeviner = await GetRandomWord();
            List<char> lstLettres = new List<char>();
            string motCourant = "";
            for (int i = 0; i < motDeviner.Length; i++)
            {
                motCourant = motCourant + '_';
            }

            Console.WriteLine("The word to guess is " + motCourant);
            while (true)
            {
                char input = Console.ReadKey().KeyChar;
                if (!lstLettres.Contains(input))
                {
                    lstLettres.Add(input);
                    if (motDeviner.Contains(input.ToString()))
                        motCourant = VerifierLettre(input, motDeviner, motCourant);
                    else
                        lives--;
                }
                else
                {
                    Console.WriteLine("\nYou have already used this letter!");
                    lives--;
                }
                if (lives == 0) break;
                Console.WriteLine("\nRemaining lives: " + lives + ". So far you have used these letters: " + new string(lstLettres.ToArray()));
                Console.WriteLine("The current word is " + motCourant);

                if (!motCourant.Contains("_"))
                    break;
            }
            if (lives > 0)
                Console.WriteLine("\nYou won!");
            else
                Console.WriteLine("\nYou lost! The word was: " + motDeviner + "\nPress 'r' to restart.");
        }

        public static string VerifierLettre(char lettre, string motDeviner, string motCourant)
        {
            for (int i = 0; i < motDeviner.Length; i++)
            {
                if (motDeviner[i] == lettre)
                {
                    motCourant = motCourant.Insert(i, lettre.ToString());
                    motCourant = motCourant.Remove(i + 1, 1);
                }
            }
            return motCourant;
        }
    }

    public class WordsAPI
    {

        public class Rootobject
        {
            public Meta meta { get; set; }
            public string[] result { get; set; }
        }

        public class Meta
        {
            public string type { get; set; }
            public string code { get; set; }
            public object[] messages { get; set; }
        }
    }
    }

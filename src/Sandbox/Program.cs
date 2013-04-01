﻿using Mbtx.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Mbtx.Net.Objects;

namespace Sandbox {
    class Program {
        static void Main(string[] args) {
            Remote();

            (new AutoResetEvent(false)).WaitOne();
        }

        static async void Remote() {
            var remote = new RemoteClient();
            //var version = await remote.GetVersionAsync();
            //var connected = await remote.GetConnectedAsync();
            //var process = await remote.GetProcessAsync();
            var accounts = await remote.GetAccountsAsync();
            var account = accounts.First(x => x.AccountIdentifier.EndsWith("24077"));
            var positions = await remote.GetPositionsAsync(account);
            //var routes = await remote.GetRoutessAsync();
            //var history = await remote.GetOrderHistoryAsync();
            //var orders = await remote.GetOrdersAsync();
            //var alerts = await remote.GetAlertsAsync();
            //var types = await remote.GetTypedValues("CAPACITYVALUES");

            //Console.WriteLine(version + " " + connected);
        }

        static void Quotes() {
            var credentials = GetCredentials();
            var quotes = new QuoteClient();
            quotes.Connect(credentials.Username, credentials.Password);
            quotes.Subscribe("EUR/USD").Subscribe(OnQuote);
        }

        static void OnQuote(Quote quote) {
            Console.WriteLine(quote);
        }

        static Credentials GetCredentials(){
            var username = default(string);
            var password = default(string);
            
            Console.Write("Username: ");
            username = Console.ReadLine();

            Console.Write("Password: ");
            
            var info = Console.ReadKey(true);
            while (info.Key != ConsoleKey.Enter) {
                if (info.Key != ConsoleKey.Backspace) {
                    Console.Write("*");
                    password += info.KeyChar;
                }
                else if (info.Key == ConsoleKey.Backspace) {
                    if (!string.IsNullOrEmpty(password)) {
                        // remove one character from the list of password characters
                        password = password.Substring(0, password.Length - 1);
                        // get the location of the cursor
                        int pos = Console.CursorLeft;
                        // move the cursor to the left by one character
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                        // replace it with space
                        Console.Write(" ");
                        // move the cursor to the left by one character again
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                    }
                }
                info = Console.ReadKey(true);
            }
            Console.WriteLine();

            return new Credentials { Username = username, Password = password };
        }
    }

    class Credentials {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}

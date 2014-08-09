using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using NetflixHistory;

namespace NetflixHistory.Test
{
    class Program
    {
        static void Main(string[] args)
        {

            var email = ConfigurationManager.AppSettings["netflix.email"];
            var password = ConfigurationManager.AppSettings["netflix.password"];
            var profile = ConfigurationManager.AppSettings["netflix.profile"];

            Console.WriteLine("Getting history for {0}'s profile {1}", email, profile);

            var history = new Client(email, password, profile);

            foreach (var item in history.History().ViewedItems)
            {

                var ts = TimeSpan.FromSeconds(item.Duration);
                string rated;
                if (String.IsNullOrEmpty(item.Rating))
                {
                    rated = "Not rated";
                }
                else
                {
                    rated = item.Rating + " / 50";
                }

                Console.WriteLine("[{0}] {1}", item.ViewedOn.ToLocalTime().ToString("g"), item.Title);
                Console.WriteLine("\tDuration: {0}s\tRated: {1}", ts.ToString(), rated);

            }

            Console.WriteLine("That's all, folks!");
            Console.WriteLine("Press any key to continue!");
            Console.ReadKey();

        }
    }
}

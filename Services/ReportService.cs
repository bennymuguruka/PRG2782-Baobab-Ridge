using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using PRG2782_Baobab_Ridge.Models;

namespace PRG2782_Baobab_Ridge.Services
{
    /// <summary>
    /// Builds and writes the animal summary report in the required text format.
    /// </summary>
    public static class ReportService
    {
        private static readonly string[] StatusNames =
        {
            "Critical",
            "Serious",
            "Stable",
            "Recovering",
            "Release-Ready"
        };

        /// <summary>
        /// Calculates report values and formats every required line in order.
        /// </summary>
        public static string BuildReport(IEnumerable<Animal> animals, DateTime generatedAt)
        {
            List<Animal> animalList = animals == null
                ? new List<Animal>()
                : new List<Animal>(animals);
            Dictionary<string, int> statusCounts = new Dictionary<string, int>();
            foreach (string status in StatusNames)
            {
                statusCounts.Add(status, 0);
            }

            long totalAge = 0;
            long totalRecoveryScore = 0;
            foreach (Animal animal in animalList)
            {
                totalAge += animal.Age;
                totalRecoveryScore += animal.RecoveryScore;

                int statusCount;
                if (statusCounts.TryGetValue(animal.Status, out statusCount))
                {
                    statusCounts[animal.Status] = statusCount + 1;
                }
            }

            int totalAnimals = animalList.Count;
            string averageAge = totalAnimals == 0
                ? "N/A"
                : ((double)totalAge / totalAnimals).ToString("F2", CultureInfo.InvariantCulture);
            string averageRecoveryScore = totalAnimals == 0
                ? "N/A"
                : ((double)totalRecoveryScore / totalAnimals).ToString("F2", CultureInfo.InvariantCulture);

            List<string> reportLines = new List<string>
            {
                "BAOBAB RIDGE WILDLIFE REHABILITATION CENTRE",
                "ANIMAL SUMMARY REPORT",
                "Generated: " + generatedAt.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                "----------------------------------------",
                "Total animals: " + totalAnimals.ToString(CultureInfo.InvariantCulture),
                "Average age (years): " + averageAge,
                "Average recovery score: " + averageRecoveryScore,
                "----------------------------------------",
                "Animals per status:"
            };

            foreach (string status in StatusNames)
            {
                reportLines.Add(status + ": " + statusCounts[status].ToString(CultureInfo.InvariantCulture));
            }

            return string.Join(Environment.NewLine, reportLines);
        }

        /// <summary>
        /// Overwrites summary.txt beside the executable with the generated report.
        /// </summary>
        public static void SaveReport(string report)
        {
            string reportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "summary.txt");
            File.WriteAllText(reportPath, report, new UTF8Encoding(false));
        }
    }
}

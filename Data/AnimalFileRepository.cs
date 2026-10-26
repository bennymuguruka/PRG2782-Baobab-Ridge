using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using PRG2782_Baobab_Ridge.Models;
using PRG2782_Baobab_Ridge.Services;

namespace PRG2782_Baobab_Ridge.Data
{
    /// <summary>
    /// Reads and writes animal records in the plain-text file beside the executable.
    /// </summary>
    public class AnimalFileRepository
    {
        private static readonly Encoding Utf8Encoding = new UTF8Encoding(false);
        private readonly string _filePath;

        /// <summary>
        /// Creates a repository that uses animals.txt beside the running executable.
        /// </summary>
        public AnimalFileRepository()
        {
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "animals.txt");
        }

        /// <summary>
        /// Loads valid records in file order and returns the number of skipped lines.
        /// </summary>
        public List<Animal> LoadAnimals(out int skippedLineCount)
        {
            EnsureDataFileExists();

            string[] lines = File.ReadAllLines(_filePath, Utf8Encoding);
            List<Animal> animals = new List<Animal>();
            skippedLineCount = 0;

            foreach (string line in lines)
            {
                Animal animal;
                if (TryParseAnimalLine(line, out animal))
                {
                    animals.Add(animal);
                }
                else
                {
                    skippedLineCount++;
                }
            }

            return animals;
        }

        /// <summary>
        /// Appends one animal record without changing existing record contents.
        /// </summary>
        public void AppendAnimal(Animal animal)
        {
            EnsureDataFileExists();
            string existingText = File.ReadAllText(_filePath, Utf8Encoding);
            string separator = string.Empty;

            if (existingText.Length > 0 && !existingText.EndsWith("\n", StringComparison.Ordinal) &&
                !existingText.EndsWith("\r", StringComparison.Ordinal))
            {
                separator = Environment.NewLine;
            }

            File.AppendAllText(
                _filePath,
                separator + SerializeAnimal(animal) + Environment.NewLine,
                Utf8Encoding);
        }

        /// <summary>
        /// Rewrites the file with the supplied records in their existing order.
        /// </summary>
        public void RewriteAnimals(IEnumerable<Animal> animals)
        {
            EnsureDataFileExists();

            List<string> lines = new List<string>();
            foreach (Animal animal in animals)
            {
                lines.Add(SerializeAnimal(animal));
            }

            File.WriteAllLines(_filePath, lines.ToArray(), Utf8Encoding);
        }

        /// <summary>
        /// Creates an empty data file if it is not already present.
        /// </summary>
        private void EnsureDataFileExists()
        {
            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, string.Empty, Utf8Encoding);
            }
        }

        /// <summary>
        /// Parses a line and accepts it only when its seven fields form a valid record.
        /// </summary>
        private static bool TryParseAnimalLine(string line, out Animal animal)
        {
            animal = null;
            string[] fields = line.Split('|');
            if (fields.Length != 7)
            {
                return false;
            }

            string animalId = fields[0].Trim().ToUpperInvariant();
            string name = fields[1].Trim();
            string species = fields[2].Trim();
            int age;
            int recoveryScore;

            if (!IsValidAnimalId(animalId) ||
                name.Length < 1 || name.Length > 30 || name.Contains("|") ||
                species.Length < 1 || species.Length > 30 || species.Contains("|") ||
                !TryParseWholeNumber(fields[3], out age) || age < 0 || age > 100 ||
                !TryParseWholeNumber(fields[4], out recoveryScore) ||
                recoveryScore < 0 || recoveryScore > 100)
            {
                return false;
            }

            Animal parsedAnimal = new Animal
            {
                AnimalId = animalId,
                Name = name,
                Species = species,
                Age = age,
                RecoveryScore = recoveryScore
            };

            AnimalService.ApplyClassification(parsedAnimal);
            if (fields[5].Trim() != parsedAnimal.Status || fields[6].Trim() != parsedAnimal.HousingUnit)
            {
                return false;
            }

            animal = parsedAnimal;
            return true;
        }

        /// <summary>
        /// Checks the required WR- prefix and four ASCII digits.
        /// </summary>
        private static bool IsValidAnimalId(string animalId)
        {
            if (animalId.Length != 7 || !animalId.StartsWith("WR-", StringComparison.Ordinal))
            {
                return false;
            }

            for (int index = 3; index < animalId.Length; index++)
            {
                if (animalId[index] < '0' || animalId[index] > '9')
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Parses a base-10 whole number using culture-independent digits.
        /// </summary>
        private static bool TryParseWholeNumber(string value, out int number)
        {
            return int.TryParse(
                value.Trim(),
                NumberStyles.None,
                CultureInfo.InvariantCulture,
                out number);
        }

        /// <summary>
        /// Serializes an animal as the required seven pipe-separated fields.
        /// </summary>
        private static string SerializeAnimal(Animal animal)
        {
            return string.Join(
                "|",
                animal.AnimalId,
                animal.Name,
                animal.Species,
                animal.Age.ToString(CultureInfo.InvariantCulture),
                animal.RecoveryScore.ToString(CultureInfo.InvariantCulture),
                animal.Status,
                animal.HousingUnit);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using PRG2782_Baobab_Ridge.Models;

namespace PRG2782_Baobab_Ridge.Services
{
    /// <summary>
    /// Applies the shared recovery-score classification to an animal.
    /// </summary>
    public static class AnimalService
    {
        /// <summary>
        /// Creates a list without exactly the selected animal only after deletion is confirmed.
        /// </summary>
        public static bool TryRemoveAnimal(
            List<Animal> animals,
            Animal selectedAnimal,
            bool deletionConfirmed,
            out List<Animal> remainingAnimals)
        {
            remainingAnimals = animals == null ? new List<Animal>() : new List<Animal>(animals);
            if (animals == null || selectedAnimal == null || !deletionConfirmed)
            {
                return false;
            }

            int selectedIndex = animals.IndexOf(selectedAnimal);
            if (selectedIndex < 0)
            {
                return false;
            }

            remainingAnimals.RemoveAt(selectedIndex);
            return true;
        }

        /// <summary>
        /// Finds an exact animal ID match without regard to letter casing.
        /// </summary>
        public static Animal FindAnimalById(IEnumerable<Animal> animals, string animalIdInput)
        {
            string animalId = (animalIdInput ?? string.Empty).Trim();
            if (animals == null || animalId.Length == 0)
            {
                return null;
            }

            foreach (Animal animal in animals)
            {
                if (string.Equals(animal.AnimalId, animalId, StringComparison.OrdinalIgnoreCase))
                {
                    return animal;
                }
            }

            return null;
        }

        /// <summary>
        /// Validates editable fields and returns a replacement animal with recalculated classification.
        /// </summary>
        public static bool TryUpdateAnimal(
            Animal existingAnimal,
            string nameInput,
            string speciesInput,
            string ageInput,
            string recoveryScoreInput,
            out Animal updatedAnimal,
            out string invalidField,
            out string validationMessage)
        {
            if (existingAnimal == null)
            {
                throw new ArgumentNullException(nameof(existingAnimal));
            }

            // The saved ID is reused, and a null existing list avoids an unnecessary uniqueness check.
            return TryCreateAnimal(
                existingAnimal.AnimalId,
                nameInput,
                speciesInput,
                ageInput,
                recoveryScoreInput,
                null,
                out updatedAnimal,
                out invalidField,
                out validationMessage);
        }

        /// <summary>
        /// Validates Add inputs, normalizes text, rejects duplicate IDs, and creates a classified animal.
        /// </summary>
        public static bool TryCreateAnimal(
            string animalIdInput,
            string nameInput,
            string speciesInput,
            string ageInput,
            string recoveryScoreInput,
            IEnumerable<Animal> existingAnimals,
            out Animal animal,
            out string invalidField,
            out string validationMessage)
        {
            animal = null;
            invalidField = string.Empty;
            validationMessage = string.Empty;

            string animalId = (animalIdInput ?? string.Empty).Trim().ToUpperInvariant();
            if (animalId.Length == 0)
            {
                return RejectInput("Animal ID", "Animal ID is required.", out animal, out invalidField, out validationMessage);
            }

            if (!IsValidAnimalId(animalId))
            {
                return RejectInput("Animal ID", "Animal ID must be WR- followed by exactly four digits.", out animal, out invalidField, out validationMessage);
            }

            if (existingAnimals != null)
            {
                foreach (Animal existingAnimal in existingAnimals)
                {
                    if (string.Equals(existingAnimal.AnimalId, animalId, StringComparison.OrdinalIgnoreCase))
                    {
                        return RejectInput("Animal ID", "Animal ID must be unique.", out animal, out invalidField, out validationMessage);
                    }
                }
            }

            string name = (nameInput ?? string.Empty).Trim();
            if (name.Length == 0)
            {
                return RejectInput("Name", "Name is required.", out animal, out invalidField, out validationMessage);
            }

            if (name.Length > 30)
            {
                return RejectInput("Name", "Name must be 1 to 30 characters long.", out animal, out invalidField, out validationMessage);
            }

            if (name.Contains("|"))
            {
                return RejectInput("Name", "Name cannot contain the pipe character (|).", out animal, out invalidField, out validationMessage);
            }

            string species = (speciesInput ?? string.Empty).Trim();
            if (species.Length == 0)
            {
                return RejectInput("Species", "Species is required.", out animal, out invalidField, out validationMessage);
            }

            if (species.Length > 30)
            {
                return RejectInput("Species", "Species must be 1 to 30 characters long.", out animal, out invalidField, out validationMessage);
            }

            if (species.Contains("|"))
            {
                return RejectInput("Species", "Species cannot contain the pipe character (|).", out animal, out invalidField, out validationMessage);
            }

            int age;
            if (!TryParseWholeNumber(ageInput, out age) || age < 0 || age > 100)
            {
                return RejectInput("Age", "Age must be a whole number between 0 and 100.", out animal, out invalidField, out validationMessage);
            }

            int recoveryScore;
            if (!TryParseWholeNumber(recoveryScoreInput, out recoveryScore) || recoveryScore < 0 || recoveryScore > 100)
            {
                return RejectInput("Recovery Score", "Recovery Score must be a whole number between 0 and 100.", out animal, out invalidField, out validationMessage);
            }

            Animal newAnimal = new Animal
            {
                AnimalId = animalId,
                Name = name,
                Species = species,
                Age = age,
                RecoveryScore = recoveryScore
            };

            ApplyClassification(newAnimal);
            animal = newAnimal;
            return true;
        }

        /// <summary>
        /// Calculates and stores an animal's status and housing from its recovery score.
        /// Add and Update should both call this method after validating the score.
        /// </summary>
        /// <param name="animal">The animal to classify.</param>
        public static void ApplyClassification(Animal animal)
        {
            if (animal == null)
            {
                throw new ArgumentNullException(nameof(animal));
            }

            int score = animal.RecoveryScore;
            if (score < 0 || score > 100)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(animal.RecoveryScore),
                    "Recovery score must be a whole number from 0 through 100.");
            }

            if (score <= 19)
            {
                animal.Status = "Critical";
                animal.HousingUnit = "Intensive Care Unit";
            }
            else if (score <= 39)
            {
                animal.Status = "Serious";
                animal.HousingUnit = "High-Dependency Ward";
            }
            else if (score <= 59)
            {
                animal.Status = "Stable";
                animal.HousingUnit = "Recovery Ward";
            }
            else if (score <= 79)
            {
                animal.Status = "Recovering";
                animal.HousingUnit = "Outdoor Enclosure";
            }
            else
            {
                animal.Status = "Release-Ready";
                animal.HousingUnit = "Pre-Release Camp";
            }
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
        /// Parses a culture-independent whole number and rejects text and decimals.
        /// </summary>
        private static bool TryParseWholeNumber(string input, out int number)
        {
            return int.TryParse(
                (input ?? string.Empty).Trim(),
                NumberStyles.None,
                CultureInfo.InvariantCulture,
                out number);
        }

        /// <summary>
        /// Returns a field-specific validation failure and ensures no animal is produced.
        /// </summary>
        private static bool RejectInput(
            string field,
            string message,
            out Animal animal,
            out string invalidField,
            out string validationMessage)
        {
            animal = null;
            invalidField = field;
            validationMessage = message;
            return false;
        }
    }
}

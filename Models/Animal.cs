namespace PRG2782_Baobab_Ridge.Models
{
    /// <summary>
    /// Stores an animal's editable details and its calculated status and housing unit.
    /// </summary>
    public class Animal
    {
        private string _animalId = string.Empty;
        private string _name = string.Empty;
        private string _species = string.Empty;

        /// <summary>
        /// Gets or sets the animal ID, trimming whitespace and storing it in uppercase.
        /// </summary>
        public string AnimalId
        {
            get { return _animalId; }
            set { _animalId = (value ?? string.Empty).Trim().ToUpperInvariant(); }
        }

        /// <summary>
        /// Gets or sets the animal name with surrounding whitespace removed.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = (value ?? string.Empty).Trim(); }
        }

        /// <summary>
        /// Gets or sets the species with surrounding whitespace removed.
        /// </summary>
        public string Species
        {
            get { return _species; }
            set { _species = (value ?? string.Empty).Trim(); }
        }

        /// <summary>
        /// Gets or sets the animal's age in years.
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// Gets or sets the vet's whole-number recovery score.
        /// </summary>
        public int RecoveryScore { get; set; }

        /// <summary>
        /// Gets the status calculated from the recovery score.
        /// </summary>
        public string Status { get; internal set; } = string.Empty;

        /// <summary>
        /// Gets the housing unit calculated from the recovery score.
        /// </summary>
        public string HousingUnit { get; internal set; } = string.Empty;
    }
}

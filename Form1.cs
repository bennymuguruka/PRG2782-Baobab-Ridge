using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using PRG2782_Baobab_Ridge.Data;
using PRG2782_Baobab_Ridge.Models;
using PRG2782_Baobab_Ridge.Services;

namespace PRG2782_Baobab_Ridge
{
    public partial class Form1 : Form
    {
        private readonly AnimalFileRepository _animalRepository;
        private List<Animal> _animals = new List<Animal>();
        private DataGridView _animalGrid;
        private TableLayoutPanel _mainLayout;
        private TableLayoutPanel _inputLayout;
        private TableLayoutPanel _summaryLayout;
        private TextBox _animalIdTextBox;
        private TextBox _nameTextBox;
        private TextBox _speciesTextBox;
        private TextBox _ageTextBox;
        private TextBox _recoveryScoreTextBox;
        private TextBox _searchAnimalIdTextBox;
        private Button _addButton;
        private Button _saveChangesButton;
        private Button _searchButton;
        private Button _deleteButton;
        private Button _clearButton;
        private Button _generateSummaryButton;
        private TextBox _summaryTextBox;
        private Animal _editingAnimal;

        /// <summary>
        /// Initializes the animal grid and loads records from the text file.
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            Text = "Baobab Ridge Wildlife Rehabilitation Records";
            _animalRepository = new AnimalFileRepository();
            ConfigureAnimalGrid();
            ConfigureAddInputs();
            ConfigureSummaryArea();
            ConfigureFormLayout();
            LoadAnimals();
        }

        /// <summary>
        /// Creates a read-only grid with the required columns in file order.
        /// </summary>
        private void ConfigureAnimalGrid()
        {
            _animalGrid = new DataGridView
            {
                Name = "animalDataGridView",
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToOrderColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                EditMode = DataGridViewEditMode.EditProgrammatically,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false
            };
            _animalGrid.CellClick += AnimalGrid_CellClick;

            AddGridColumn("animalIdColumn", "Animal ID", "AnimalId");
            AddGridColumn("nameColumn", "Name", "Name");
            AddGridColumn("speciesColumn", "Species", "Species");
            AddGridColumn("ageColumn", "Age", "Age");
            AddGridColumn("recoveryScoreColumn", "Recovery Score", "RecoveryScore");
            AddGridColumn("statusColumn", "Status", "Status");
            AddGridColumn("housingUnitColumn", "Housing Unit", "HousingUnit");

        }

        /// <summary>
        /// Creates labelled animal inputs and controls for Add, Update, Search and Clear.
        /// </summary>
        private void ConfigureAddInputs()
        {
            _inputLayout = new TableLayoutPanel
            {
                Name = "animalInputLayout",
                Dock = DockStyle.Fill,
                ColumnCount = 7,
                RowCount = 3,
                Padding = new Padding(6)
            };

            for (int column = 0; column < 7; column++)
            {
                _inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / 7f));
            }

            _inputLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));
            _inputLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            _inputLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            _animalIdTextBox = AddLabeledInput("Animal ID", "animalIdTextBox", 0);
            _nameTextBox = AddLabeledInput("Name", "nameTextBox", 1);
            _speciesTextBox = AddLabeledInput("Species", "speciesTextBox", 2);
            _ageTextBox = AddLabeledInput("Age", "ageTextBox", 3);
            _recoveryScoreTextBox = AddLabeledInput("Recovery Score", "recoveryScoreTextBox", 4);

            Label actionLabel = new Label
            {
                Text = "Action",
                AutoSize = true,
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom,
                Margin = new Padding(3)
            };
            _inputLayout.Controls.Add(actionLabel, 5, 0);
            _inputLayout.SetColumnSpan(actionLabel, 2);

            _addButton = new Button
            {
                Name = "addAnimalButton",
                Text = "Add",
                Dock = DockStyle.Fill,
                Margin = new Padding(3)
            };
            _addButton.Click += AddAnimalButton_Click;
            _inputLayout.Controls.Add(_addButton, 5, 1);

            _saveChangesButton = new Button
            {
                Name = "saveChangesButton",
                Text = "Save Changes",
                Dock = DockStyle.Fill,
                Enabled = false,
                Margin = new Padding(3)
            };
            _saveChangesButton.Click += SaveChangesButton_Click;
            _inputLayout.Controls.Add(_saveChangesButton, 6, 1);

            Label searchLabel = new Label
            {
                Text = "Search ID",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Margin = new Padding(3)
            };
            _inputLayout.Controls.Add(searchLabel, 0, 2);

            _searchAnimalIdTextBox = new TextBox
            {
                Name = "searchAnimalIdTextBox",
                Dock = DockStyle.Fill,
                Margin = new Padding(3)
            };
            _inputLayout.Controls.Add(_searchAnimalIdTextBox, 1, 2);
            _inputLayout.SetColumnSpan(_searchAnimalIdTextBox, 3);

            _searchButton = new Button
            {
                Name = "searchAnimalButton",
                Text = "Search",
                Dock = DockStyle.Fill,
                Margin = new Padding(3)
            };
            _searchButton.Click += SearchButton_Click;
            _inputLayout.Controls.Add(_searchButton, 3, 2);

            _deleteButton = new Button
            {
                Name = "deleteAnimalButton",
                Text = "Delete",
                Dock = DockStyle.Fill,
                Margin = new Padding(3)
            };
            _deleteButton.Click += DeleteButton_Click;
            _inputLayout.Controls.Add(_deleteButton, 4, 2);

            _clearButton = new Button
            {
                Name = "clearAnimalButton",
                Text = "Clear",
                Dock = DockStyle.Fill,
                Margin = new Padding(3)
            };
            _clearButton.Click += ClearButton_Click;
            _inputLayout.Controls.Add(_clearButton, 5, 2);
            _inputLayout.SetColumnSpan(_clearButton, 2);
        }

        /// <summary>
        /// Creates one labelled text input and places it in the requested input column.
        /// </summary>
        private TextBox AddLabeledInput(string labelText, string controlName, int column)
        {
            Label label = new Label
            {
                Text = labelText,
                AutoSize = true,
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom,
                Margin = new Padding(3)
            };
            TextBox textBox = new TextBox
            {
                Name = controlName,
                Dock = DockStyle.Fill,
                Margin = new Padding(3)
            };

            _inputLayout.Controls.Add(label, column, 0);
            _inputLayout.Controls.Add(textBox, column, 1);
            return textBox;
        }

        /// <summary>
        /// Places the add inputs above the animal grid in the main form layout.
        /// </summary>
        private void ConfigureFormLayout()
        {
            _mainLayout = new TableLayoutPanel
            {
                Name = "mainLayout",
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3
            };
            _mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 108));
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 132));
            _mainLayout.Controls.Add(_inputLayout, 0, 0);
            _mainLayout.Controls.Add(_animalGrid, 0, 1);
            _mainLayout.Controls.Add(_summaryLayout, 0, 2);
            Controls.Add(_mainLayout);
        }

        /// <summary>
        /// Creates an on-form read-only report area and its Generate Summary button.
        /// </summary>
        private void ConfigureSummaryArea()
        {
            _summaryLayout = new TableLayoutPanel
            {
                Name = "summaryLayout",
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                Padding = new Padding(6)
            };
            _summaryLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            _summaryLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
            _summaryLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
            _summaryLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            Label summaryLabel = new Label
            {
                Text = "Summary Report",
                AutoSize = true,
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom,
                Margin = new Padding(3)
            };
            _summaryLayout.Controls.Add(summaryLabel, 0, 0);

            _generateSummaryButton = new Button
            {
                Name = "generateSummaryButton",
                Text = "Generate Summary",
                Dock = DockStyle.Fill,
                Margin = new Padding(3)
            };
            _generateSummaryButton.Click += GenerateSummaryButton_Click;
            _summaryLayout.Controls.Add(_generateSummaryButton, 1, 0);

            _summaryTextBox = new TextBox
            {
                Name = "summaryReportTextBox",
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                WordWrap = false,
                Font = new System.Drawing.Font("Consolas", 9)
            };
            _summaryLayout.Controls.Add(_summaryTextBox, 0, 1);
            _summaryLayout.SetColumnSpan(_summaryTextBox, 2);
        }

        /// <summary>
        /// Builds the report from current records, displays it, and overwrites summary.txt.
        /// </summary>
        private void GenerateSummaryButton_Click(object sender, EventArgs eventArgs)
        {
            string report = ReportService.BuildReport(_animals, DateTime.Now);
            _summaryTextBox.Text = report;

            try
            {
                ReportService.SaveReport(report);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show(
                    "Windows does not allow writing summary.txt.",
                    "Summary report error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (IOException)
            {
                MessageBox.Show(
                    "The summary report could not be written. Check that the file is available and try again.",
                    "Summary report error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Validates and appends one animal, then refreshes the grid and clears the inputs.
        /// </summary>
        private void AddAnimalButton_Click(object sender, EventArgs e)
        {
            Animal animal;
            string invalidField;
            string validationMessage;

            if (!AnimalService.TryCreateAnimal(
                _animalIdTextBox.Text,
                _nameTextBox.Text,
                _speciesTextBox.Text,
                _ageTextBox.Text,
                _recoveryScoreTextBox.Text,
                _animals,
                out animal,
                out invalidField,
                out validationMessage))
            {
                MessageBox.Show(
                    validationMessage,
                    "Invalid " + invalidField,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                GetInputForField(invalidField).Focus();
                return;
            }

            try
            {
                _animalRepository.AppendAnimal(animal);
                _animals.Add(animal);
                RefreshAnimalGrid();

                MessageBox.Show(
                    animal.AnimalId + " was added. Status: " + animal.Status +
                    ". Housing unit: " + animal.HousingUnit + ".",
                    "Animal added",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                ClearAddInputs();
            }
            catch (UnauthorizedAccessException)
            {
                ShowFileError("Windows does not allow changes to animals.txt.");
            }
            catch (IOException)
            {
                ShowFileError("The animal could not be saved because animals.txt is unavailable.");
            }
        }

        /// <summary>
        /// Returns the input control associated with a validation field name.
        /// </summary>
        private Control GetInputForField(string fieldName)
        {
            switch (fieldName)
            {
                case "Name":
                    return _nameTextBox;
                case "Species":
                    return _speciesTextBox;
                case "Age":
                    return _ageTextBox;
                case "Recovery Score":
                    return _recoveryScoreTextBox;
                default:
                    return _animalIdTextBox;
            }
        }

        /// <summary>
        /// Clears all Add inputs and returns focus to the Animal ID field.
        /// </summary>
        private void ClearAddInputs()
        {
            _animalIdTextBox.Clear();
            _nameTextBox.Clear();
            _speciesTextBox.Clear();
            _ageTextBox.Clear();
            _recoveryScoreTextBox.Clear();
            _animalIdTextBox.Focus();
        }

        /// <summary>
        /// Loads the selected grid record into the shared edit fields.
        /// </summary>
        private void AnimalGrid_CellClick(object sender, DataGridViewCellEventArgs eventArgs)
        {
            if (eventArgs.RowIndex < 0)
            {
                return;
            }

            Animal selectedAnimal = _animalGrid.Rows[eventArgs.RowIndex].DataBoundItem as Animal;
            if (selectedAnimal != null)
            {
                LoadAnimalForEditing(selectedAnimal);
            }
        }

        /// <summary>
        /// Finds an exact ID match and loads it into the same fields used by grid selection.
        /// </summary>
        private void SearchButton_Click(object sender, EventArgs eventArgs)
        {
            Animal matchingAnimal = AnimalService.FindAnimalById(_animals, _searchAnimalIdTextBox.Text);
            if (matchingAnimal == null)
            {
                MessageBox.Show(
                    "No animal was found with ID " + _searchAnimalIdTextBox.Text.Trim() + ".",
                    "Animal not found",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            LoadAnimalForEditing(matchingAnimal);
        }

        /// <summary>
        /// Populates the shared fields and switches the form into edit mode.
        /// </summary>
        private void LoadAnimalForEditing(Animal animal)
        {
            _editingAnimal = animal;
            _animalIdTextBox.Text = animal.AnimalId;
            _animalIdTextBox.ReadOnly = true;
            _nameTextBox.Text = animal.Name;
            _speciesTextBox.Text = animal.Species;
            _ageTextBox.Text = animal.Age.ToString();
            _recoveryScoreTextBox.Text = animal.RecoveryScore.ToString();
            _addButton.Enabled = false;
            _saveChangesButton.Enabled = true;
        }

        /// <summary>
        /// Validates edited fields and rewrites the file with only the selected record changed.
        /// </summary>
        private void SaveChangesButton_Click(object sender, EventArgs eventArgs)
        {
            if (_editingAnimal == null)
            {
                return;
            }

            Animal updatedAnimal;
            string invalidField;
            string validationMessage;
            if (!AnimalService.TryUpdateAnimal(
                _editingAnimal,
                _nameTextBox.Text,
                _speciesTextBox.Text,
                _ageTextBox.Text,
                _recoveryScoreTextBox.Text,
                out updatedAnimal,
                out invalidField,
                out validationMessage))
            {
                MessageBox.Show(
                    validationMessage,
                    "Invalid " + invalidField,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                GetInputForField(invalidField).Focus();
                return;
            }

            int editedIndex = _animals.IndexOf(_editingAnimal);
            if (editedIndex < 0)
            {
                MessageBox.Show(
                    "The selected animal is no longer in the current record list.",
                    "Animal not found",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            List<Animal> updatedAnimals = new List<Animal>(_animals);
            updatedAnimals[editedIndex] = updatedAnimal;

            try
            {
                _animalRepository.RewriteAnimals(updatedAnimals);
                _animals = updatedAnimals;
                RefreshAnimalGrid();
                ExitEditModeAndClearInputs();
                MessageBox.Show(
                    updatedAnimal.AnimalId + " was updated. Status: " + updatedAnimal.Status +
                    ". Housing unit: " + updatedAnimal.HousingUnit + ".",
                    "Animal updated",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (UnauthorizedAccessException)
            {
                ShowFileError("Windows does not allow changes to animals.txt.");
            }
            catch (IOException)
            {
                ShowFileError("The animal could not be updated because animals.txt is unavailable.");
            }
        }

        /// <summary>
        /// Confirms deletion of the selected grid animal and removes that one record.
        /// </summary>
        private void DeleteButton_Click(object sender, EventArgs eventArgs)
        {
            if (_animalGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Select one animal in the grid before choosing Delete.",
                    "No animal selected",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            Animal selectedAnimal = _animalGrid.SelectedRows[0].DataBoundItem as Animal;
            if (selectedAnimal == null)
            {
                MessageBox.Show(
                    "Select one animal in the grid before choosing Delete.",
                    "No animal selected",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            DialogResult confirmation = MessageBox.Show(
                "Delete " + selectedAnimal.AnimalId + " (" + selectedAnimal.Name + ")? This cannot be undone.",
                "Confirm deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            List<Animal> remainingAnimals;
            if (!AnimalService.TryRemoveAnimal(
                _animals,
                selectedAnimal,
                confirmation == DialogResult.Yes,
                out remainingAnimals))
            {
                return;
            }

            bool deletedAnimalWasLoaded = Object.ReferenceEquals(_editingAnimal, selectedAnimal);
            try
            {
                _animalRepository.RewriteAnimals(remainingAnimals);
                _animals = remainingAnimals;
                RefreshAnimalGrid();

                if (deletedAnimalWasLoaded)
                {
                    ExitEditModeAndClearInputs();
                }
            }
            catch (UnauthorizedAccessException)
            {
                ShowFileError("Windows does not allow changes to animals.txt.");
            }
            catch (IOException)
            {
                ShowFileError("The animal could not be deleted because animals.txt is unavailable.");
            }
        }

        /// <summary>
        /// Returns the form to Add mode and clears both record and search inputs.
        /// </summary>
        private void ClearButton_Click(object sender, EventArgs eventArgs)
        {
            ExitEditModeAndClearInputs();
        }

        /// <summary>
        /// Restores Add controls and clears the shared fields after Clear or a successful update.
        /// </summary>
        private void ExitEditModeAndClearInputs()
        {
            _editingAnimal = null;
            _animalIdTextBox.ReadOnly = false;
            _addButton.Enabled = true;
            _saveChangesButton.Enabled = false;
            _searchAnimalIdTextBox.Clear();
            ClearAddInputs();
        }

        /// <summary>
        /// Adds a non-sortable, read-only text column to the animal grid.
        /// </summary>
        private void AddGridColumn(string name, string headerText, string propertyName)
        {
            _animalGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = name,
                HeaderText = headerText,
                DataPropertyName = propertyName,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
        }

        /// <summary>
        /// Loads valid animals and reports the total skipped-line count once.
        /// </summary>
        private void LoadAnimals()
        {
            try
            {
                int skippedLineCount;
                _animals = _animalRepository.LoadAnimals(out skippedLineCount);
                RefreshAnimalGrid();

                if (skippedLineCount > 0)
                {
                    MessageBox.Show(
                        "Skipped " + skippedLineCount + " malformed line(s) in animals.txt.",
                        "Animal file contains invalid records",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (UnauthorizedAccessException)
            {
                ShowFileError("Windows does not allow access to animals.txt.");
            }
            catch (IOException)
            {
                ShowFileError("animals.txt could not be read. Check that it is available and try again.");
            }
        }

        /// <summary>
        /// Rebinds the grid without retaining old rows from a previous load.
        /// </summary>
        private void RefreshAnimalGrid()
        {
            _animalGrid.DataSource = null;
            _animalGrid.DataSource = _animals;
        }

        /// <summary>
        /// Shows a friendly file-operation error without exposing exception details.
        /// </summary>
        private void ShowFileError(string message)
        {
            MessageBox.Show(
                message,
                "Animal file error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}

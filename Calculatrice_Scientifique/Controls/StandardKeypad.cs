using Calculatrice_Scientifique.Models;

namespace Calculatrice_Scientifique.Controls
{
    public partial class StandardKeypad : KeypadBase
    {
        private double? _accumulator;
        private string? _pendingOperator;
        private bool _isEnteringNumber;
        private string _currentExpression = "";

        public StandardKeypad() : base(new StandardModel())
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            var tlp = new TableLayoutPanel();
            tlp.Dock = DockStyle.Fill;

            // Configuration de la grille : 4 colonnes, 5 lignes
            tlp.ColumnCount = 4;
            tlp.RowCount = 5;

            // Styles des colonnes (25% chacune)
            for (int i = 0; i < 4; i++)
                tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));

            // Styles des lignes (20% chacune pour 5 lignes)
            for (int i = 0; i < 5; i++)
                tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));

            // Ton nouveau layout de touches
            string[] labels = {
                "CE", "C", "⌫", "/",
                "7",  "8", "9", "*",
                "4",  "5", "6", "+",
                "1",  "2", "3", "-",
                "0",  "+/-", ".", "="
            };

            int idx = 0;
            for (int r = 0; r < 5; r++)
            {
                for (int c = 0; c < 4; c++)
                {
                    var b = new Button();
                    b.Text = labels[idx++];
                    b.Dock = DockStyle.Fill;
                    b.Margin = new Padding(4);
                    b.Font = new Font("Segoe UI Variable Text", 12F);
                    b.FlatStyle = FlatStyle.Flat;
                    b.FlatAppearance.BorderSize = 1;
                    b.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
                    b.Cursor = Cursors.Hand;
                    b.TabStop = false;

                    // Attribution des événements selon le texte du bouton
                    if (int.TryParse(b.Text, out _)) b.Click += Digit_Click;
                    else if (b.Text == ".") b.Click += Dot_Click;
                    else if (b.Text == "=") b.Click += Equals_Click;
                    else if (b.Text == "⌫") b.Click += Backspace_Click;
                    else if (b.Text == "C") b.Click += ClearAll_Click;
                    else if (b.Text == "CE") b.Click += ClearEntry_Click;
                    else if (b.Text == "+/-") b.Click += SignChange_Click;
                    else b.Click += Operator_Click;

                    tlp.Controls.Add(b, c, r);
                }
            }

            this.Controls.Add(tlp);
            this.Dock = DockStyle.Fill;
        }

        #region Logique des Boutons

        private void Digit_Click(object? sender, EventArgs e)
        {
            if (sender is not Button b) return;

            // Si on vient de finir un calcul précédent, on recommence à zéro
            if (!_isEnteringNumber && _pendingOperator == null)
            {
                _currentExpression = b.Text;
            }
            else
            {
                _currentExpression += b.Text;
            }

            SetDisplayText(_currentExpression);
            _isEnteringNumber = true;
        }

        private void Dot_Click(object? sender, EventArgs e)
        {
            string current = GetDisplayText();
            if (!_isEnteringNumber)
            {
                SetDisplayText("0.");
                _isEnteringNumber = true;
                return;
            }
            if (!current.Contains('.')) SetDisplayText(current + ".");
        }

        private void Operator_Click(object? sender, EventArgs e)
        {
            if (sender is not Button b) return;

            // Évite d'ajouter deux opérateurs de suite (ex: "3 + -")
            if (_currentExpression.EndsWith(" ")) return;

            if (double.TryParse(GetDisplayText().Split(' ').Last(), out double currentVal))
            {
                if (_accumulator is null)
                    _accumulator = currentVal;
                else if (_pendingOperator is not null)
                    _accumulator = _model.Calculate(_accumulator.Value, currentVal, _pendingOperator);
            }

            _pendingOperator = b.Text;
            _isEnteringNumber = false;

            // On ajoute l'opérateur avec des espaces pour la lisibilité : "3 - "
            _currentExpression += $" {b.Text} ";
            SetDisplayText(_currentExpression);
        }

        private void Equals_Click(object? sender, EventArgs e)
        {
            // On récupère le dernier nombre saisi dans l'expression
            string lastPart = _currentExpression.Split(' ').Last();

            if (double.TryParse(lastPart, out double currentVal) && _accumulator != null && _pendingOperator != null)
            {
                double result = _model.Calculate(_accumulator.Value, currentVal, _pendingOperator);

                // On affiche le résultat final
                SetDisplayText(result.ToString());
                RaiseHistory(_currentExpression, result);

                // Reset pour le prochain calcul
                _accumulator = null;
                _pendingOperator = null;
                _currentExpression = result.ToString(); // Permet de continuer à partir du résultat
                _isEnteringNumber = false;
            }
        }

        private void Backspace_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_currentExpression) || _currentExpression == "0") return;

            // Si on finit par un espace, c'est qu'on efface un opérateur (ex: "3 + ")
            // On doit enlever 3 caractères : " + "
            if (_currentExpression.EndsWith(" "))
            {
                _currentExpression = _currentExpression.Substring(0, _currentExpression.Length - 3);
                _pendingOperator = null; // On annule l'opérateur en attente
            }
            else
            {
                _currentExpression = _currentExpression.Substring(0, _currentExpression.Length - 1);
            }

            // Si tout est effacé, on remet à "0"
            if (string.IsNullOrEmpty(_currentExpression))
            {
                _currentExpression = "0";
                _isEnteringNumber = false;
            }

            SetDisplayText(_currentExpression);
        }

        private void ClearAll_Click(object? sender, EventArgs e)
        {
            _accumulator = null;
            _pendingOperator = null;
            _isEnteringNumber = false;
            _currentExpression = "0"; // Source de vérité remise à zéro
            SetDisplayText(_currentExpression);
        }

        private void ClearEntry_Click(object? sender, EventArgs e)
        {
            // On efface seulement le dernier nombre saisi dans l'expression
            var parts = _currentExpression.Split(' ');
            if (parts.Length > 1)
            {
                // On garde tout sauf le dernier segment
                _currentExpression = string.Join(" ", parts.Take(parts.Length - 1)) + " ";
            }
            else
            {
                _currentExpression = "0";
            }

            _isEnteringNumber = false;
            SetDisplayText(_currentExpression);
        }

        private void SignChange_Click(object? sender, EventArgs e)
        {
            var parts = _currentExpression.Split(' ').ToList();
            string lastPart = parts.Last();

            if (double.TryParse(lastPart, out double val))
            {
                double newVal = val * -1;
                parts[parts.Count - 1] = newVal.ToString();
                _currentExpression = string.Join(" ", parts);
                SetDisplayText(_currentExpression);
            }
        }

        #endregion
    }
}
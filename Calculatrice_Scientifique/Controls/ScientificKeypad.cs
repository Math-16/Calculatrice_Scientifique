using System.Linq;
using System.Drawing;
using Calculatrice_Scientifique.Models;

namespace Calculatrice_Scientifique.Controls
{
    public partial class ScientificKeypad : KeypadBase
    {
        private double? _accumulator;
        private string? _pendingOperator;
        private bool _isEnteringNumber;
        private bool _useDegrees = true;
        private string _currentExpression = "0";

        public ScientificKeypad() : base(new ScientificModel())
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            var tlp = new TableLayoutPanel();
            tlp.Dock = DockStyle.Fill;
            tlp.ColumnCount = 6;
            tlp.RowCount = 5;

            for (int i = 0; i < 6; i++) tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / 6F));
            for (int r = 0; r < 5; r++) tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));

            string[] labels = {
                "C", "⌫", "e", "π", "^", "/",
                "7", "8", "9", "*", "log", "√",
                "4", "5", "6", "-", "ln", "sin",
                "1", "2", "3", "+", "cos", "tan",
                "0", ".", "+/-", "=", "(", ")"
            };

            int idx = 0;
            for (int r = 0; r < 5; r++)
            {
                for (int c = 0; c < 6; c++)
                {
                    if (idx >= labels.Length) break;
                    var text = labels[idx++];
                    if (string.IsNullOrEmpty(text)) continue;

                    var b = new Button();
                    b.Text = text;
                    b.Dock = DockStyle.Fill;
                    b.Margin = new Padding(4);
                    b.Font = new Font("Segoe UI Variable Text", 10F);
                    b.FlatStyle = FlatStyle.Flat;
                    b.FlatAppearance.BorderSize = 1;
                    b.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
                    b.Cursor = Cursors.Hand;
                    b.TabStop = false;

                    if (char.IsDigit(text[0])) b.Click += Digit_Click;
                    else if (text == ".") b.Click += Dot_Click;
                    else if (text == "=") b.Click += Equals_Click;
                    else if (text == "C") b.Click += ClearAll_Click;
                    else if (text == "⌫") b.Click += Backspace_Click;
                    else if (text == "+/-") b.Click += SignChange_Click;
                    else if (text == "π" || text == "e") b.Click += Constant_Click;
                    else if (new[] { "sin", "cos", "tan", "log", "ln", "√" }.Contains(text)) b.Click += Function_Click;
                    else b.Click += Operator_Click;

                    tlp.Controls.Add(b, c, r);
                }
            }

            Controls.Add(tlp);
            this.Dock = DockStyle.Fill;
        }

        #region Logique

        private void Digit_Click(object? sender, EventArgs e)
        {
            if (sender is not Button b) return;

            // Si l'écran affiche "0", on remplace. Sinon on ajoute.
            if (_currentExpression == "0")
                _currentExpression = b.Text;
            else
                _currentExpression += b.Text;

            SetDisplayText(_currentExpression);
            _isEnteringNumber = true;
        }

        private void Dot_Click(object? sender, EventArgs e)
        {
            var parts = _currentExpression.Split(' ');
            string lastPart = parts.Last();

            if (!lastPart.Contains('.'))
            {
                _currentExpression += ".";
                SetDisplayText(_currentExpression);
                _isEnteringNumber = true;
            }
        }

        private void Constant_Click(object? sender, EventArgs e)
        {
            if (sender is not Button b) return;
            double val = b.Text == "π" ? Math.PI : Math.E;

            if (!_isEnteringNumber || _currentExpression == "0")
                _currentExpression = val.ToString();
            else
                _currentExpression += val.ToString();

            SetDisplayText(_currentExpression);
            _isEnteringNumber = false;
        }

        private void Function_Click(object? sender, EventArgs e)
        {
            if (sender is not Button b) return;
            string fn = b.Text;

            string lastPart = _currentExpression.Split(' ').Last();
            if (double.TryParse(lastPart, out double value))
            {
                double result = ((ScientificModel)_model).ApplyFunction(fn, value, _useDegrees);

                var parts = _currentExpression.Split(' ').ToList();
                string functionDisplay = $"{fn}({lastPart})";

                // On remplace le dernier nombre par le résultat visuellement pour l'expression continue
                parts[parts.Count - 1] = result.ToString();
                _currentExpression = string.Join(" ", parts);

                SetDisplayText(_currentExpression);
                RaiseHistory(functionDisplay, result);
                _isEnteringNumber = false;
            }
        }

        private void Operator_Click(object? sender, EventArgs e)
        {
            if (sender is not Button b) return;

            // 1. Bloquer si on essaie de mettre deux opérateurs de suite ("3 + *")
            if (_currentExpression.EndsWith(" ")) return;

            // 2. Logique mathématique (Calcul en arrière-plan)
            string lastPart = _currentExpression.Split(' ').Last();
            if (double.TryParse(lastPart, out double currentVal))
            {
                if (_accumulator is null)
                {
                    _accumulator = currentVal;
                }
                else if (_pendingOperator is not null)
                {
                    // On calcule la valeur réelle mais on ne l'affiche pas encore !
                    _accumulator = _model.Calculate(_accumulator.Value, currentVal, _pendingOperator);
                }
            }

            // 3. Mise à jour de l'état
            _pendingOperator = b.Text;
            _isEnteringNumber = false;

            // 4. MISE À JOUR DE L'AFFICHAGE (La correction est ici)
            // On ajoute l'opérateur à la suite du texte existant au lieu de reset avec l'accumulateur
            _currentExpression += $" {b.Text} ";
            SetDisplayText(_currentExpression);
        }

        private void Equals_Click(object? sender, EventArgs e)
        {
            string lastPart = _currentExpression.Split(' ').Last();

            if (double.TryParse(lastPart, out double currentVal) && _accumulator != null && _pendingOperator != null)
            {
                double result = _model.Calculate(_accumulator.Value, currentVal, _pendingOperator);

                // On sauvegarde l'expression complète (ex: "3 + 5 + 2") pour l'historique
                string finalExpression = _currentExpression;

                // On affiche le résultat final (Ici on reset l'affichage car le calcul est fini)
                _currentExpression = result.ToString();
                SetDisplayText(_currentExpression);

                RaiseHistory(finalExpression, result);

                // Reset des variables de calcul
                _accumulator = null;
                _pendingOperator = null;
                _isEnteringNumber = false;
            }
        }

        private void Backspace_Click(object? sender, EventArgs e)
        {
            if (_currentExpression == "0" || string.IsNullOrEmpty(_currentExpression)) return;

            if (_currentExpression.EndsWith(" "))
            {
                // On efface un opérateur et ses espaces " + "
                _currentExpression = _currentExpression.Substring(0, _currentExpression.Length - 3);
            }
            else
            {
                _currentExpression = _currentExpression.Substring(0, _currentExpression.Length - 1);
            }

            if (string.IsNullOrEmpty(_currentExpression)) _currentExpression = "0";
            SetDisplayText(_currentExpression);
        }

        private void ClearAll_Click(object? sender, EventArgs e)
        {
            _accumulator = null;
            _pendingOperator = null;
            _currentExpression = "0";
            _isEnteringNumber = false;
            SetDisplayText("0");
        }

        private void SignChange_Click(object? sender, EventArgs e)
        {
            var parts = _currentExpression.Split(' ').ToList();
            if (double.TryParse(parts.Last(), out double val))
            {
                parts[parts.Count - 1] = (val * -1).ToString();
                _currentExpression = string.Join(" ", parts);
                SetDisplayText(_currentExpression);
            }
        }
        #endregion
    }
}
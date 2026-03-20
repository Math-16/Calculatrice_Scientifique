using System.Drawing;
using System.Windows.Forms;
using Calculatrice_Scientifique.Controls;
using Calculatrice_Scientifique.Models;

namespace Calculatrice_Scientifique.Forms
{
    /// <summary>
    /// Main application window that hosts the calculator display, keypad and history panel.
    /// </summary>
    public partial class MainView : Form
    {
        private SplitContainer _split = null!;
        private Panel _leftHost = null!;
        private DisplayScreen _display = null!;
        private HistoryPanel _history = null!;
        private FlowLayoutPanel _topBar = null!;
        private ComboBox _cmbMode = null!;
        private Button _btnTheme = null!;
        private bool _isDark = false;

        // Palette de couleurs
        private Color _operatorAccent = Color.FromArgb(0, 120, 215);

        public MainView()
        {
            InitializeComponent();
            // On charge le mode par défaut au démarrage
            LoadStandard();
        }

        /// <summary>
        /// Create and arrange UI elements used by the main view.
        /// </summary>
        private void InitializeComponent()
        {
            _split = new SplitContainer();
            _leftHost = new Panel();
            _display = new DisplayScreen();
            _history = new HistoryPanel();
            _topBar = new FlowLayoutPanel();

            this.SuspendLayout();

            _topBar.Dock = DockStyle.Top;
            _topBar.Height = 45;
            _topBar.Padding = new Padding(10, 8, 10, 0);
            _topBar.BackColor = Color.FromArgb(245, 248, 250);

            _cmbMode = new ComboBox();
            _cmbMode.DropDownStyle = ComboBoxStyle.DropDownList;
            _cmbMode.Items.AddRange(new object[] { "Standard", "Scientific" });
            _cmbMode.SelectedIndex = 0;
            _cmbMode.Width = 180;
            _cmbMode.Font = new Font("Segoe UI", 10F);
            _cmbMode.FlatStyle = FlatStyle.Flat;
            _cmbMode.SelectedIndexChanged += (s, e) =>
            {
                if (_cmbMode.SelectedItem.ToString() == "Standard") LoadStandard();
                else LoadScientific();
            };

            _btnTheme = new Button();
            _btnTheme.Text = "Thème: Clair";
            _btnTheme.FlatStyle = FlatStyle.Flat;
            _btnTheme.FlatAppearance.BorderSize = 0;
            _btnTheme.AutoSize = true;
            _btnTheme.Cursor = Cursors.Hand;
            _btnTheme.Click += (s, e) => ToggleTheme();

            _topBar.Controls.Add(_cmbMode);
            _topBar.Controls.Add(_btnTheme);

            _split.Dock = DockStyle.Fill;
            _split.SplitterDistance = 500;
            _split.Panel1.Controls.Add(_leftHost);
            _split.Panel2.Controls.Add(_history);

            _leftHost.Dock = DockStyle.Fill;
            _split.Panel2.Padding = new Padding(15);
            _history.Dock = DockStyle.Fill;

            this.Controls.Add(_split);
            this.Controls.Add(_topBar);

            this.Text = "Calculatrice Avancé";
            this.MinimumSize = new Size(700, 550);
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            ApplyTheme(_isDark);
            this.ResumeLayout(false);
        }

        /// <summary>
        /// Load the standard keypad into the left host.
        /// </summary>
        private void LoadStandard()
        {
            SetupLeftLayout(new StandardKeypad());
        }

        /// <summary>
        /// Load the scientific keypad into the left host.
        /// </summary>
        private void LoadScientific()
        {
            SetupLeftLayout(new ScientificKeypad());
        }

        /// <summary>
        /// Arrange the display and keypad inside the left host using a two-row layout.
        /// </summary>
        /// <param name="keypad">Keypad control to host under the display.</param>
        private void SetupLeftLayout(KeypadBase keypad)
        {
            _leftHost.Controls.Clear();

            var tlp = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2
            };

            tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F));
            tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            keypad.HistoryAdded += Kp_HistoryAdded;
            keypad.Display = _display;
            keypad.Dock = DockStyle.Fill;

            tlp.Controls.Add(_display, 0, 0);
            tlp.Controls.Add(keypad, 0, 1);

            _leftHost.Controls.Add(tlp);

            ApplyThemeToControl(tlp, _isDark);
        }

        /// <summary>
        /// Toggle between light and dark themes.
        /// </summary>
        private void ToggleTheme()
        {
            _isDark = !_isDark;
            _btnTheme.Text = _isDark ? "Thème: Sombre" : "Thème: Clair";
            ApplyTheme(_isDark);
        }

        /// <summary>
        /// Apply the selected theme recursively to the main view controls.
        /// </summary>
        /// <param name="dark">If true apply dark theme, otherwise light theme.</param>
        private void ApplyTheme(bool dark)
        {
            Color back = dark ? Color.FromArgb(28, 28, 28) : Color.FromArgb(245, 248, 250);
            this.BackColor = back;

            _operatorAccent = dark ? Color.FromArgb(0, 120, 215) : Color.FromArgb(0, 100, 200);

            ApplyThemeToControl(_topBar, dark);
            ApplyThemeToControl(_split, dark);

            this.Refresh();
        }

        /// <summary>
        /// Recursively apply theme colors to a control and its children. Special-cases
        /// our custom DisplayScreen and HistoryPanel so they can manage internal elements.
        /// </summary>
        private void ApplyThemeToControl(Control c, bool dark)
        {
            if (c is null) return;

            Color surface = dark ? Color.FromArgb(32, 32, 32) : SystemColors.Window;
            Color text = dark ? Color.White : SystemColors.ControlText;

            if (c is Controls.DisplayScreen ds)
            {
                ds.ApplyTheme(dark, surface, text);
                ds.Parent.BackColor = surface;
                return;
            }

            if (c is Controls.HistoryPanel hp)
            {
                hp.ApplyTheme(dark, surface, text);
                hp.Parent.BackColor = surface;
                return;
            }

            if (c is Button btn)
            {
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;

                if (btn.Text == "C" || btn.Text == "⌫")
                {
                    btn.BackColor = Color.FromArgb(232, 17, 35);
                    btn.ForeColor = Color.White;
                }
                else if ("+-*/^=".Contains(btn.Text) && btn.Text.Length == 1)
                {
                    btn.BackColor = _operatorAccent;
                    btn.ForeColor = Color.White;
                }
                else
                {
                    btn.BackColor = dark ? Color.FromArgb(45, 45, 45) : Color.FromArgb(230, 230, 230);
                    btn.ForeColor = text;
                }
            }
            else if (c is ComboBox cb)
            {
                cb.BackColor = dark ? Color.FromArgb(45, 45, 45) : Color.White;
                cb.ForeColor = text;
            }
            else
            {
                c.BackColor = surface;
                c.ForeColor = text;
            }

            foreach (Control child in c.Controls)
                ApplyThemeToControl(child, dark);
        }

        private void Kp_HistoryAdded(object? sender, HistoryEventArgs e)
        {
            _history.AddEntry($"{e.Expression} = {e.Result}");
        }
    }
}
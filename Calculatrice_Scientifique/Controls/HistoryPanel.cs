using System.Drawing.Drawing2D;

namespace Calculatrice_Scientifique.Controls
{
    /// <summary>
    /// Panel that shows the history list for performed calculations.
    /// </summary>
    public partial class HistoryPanel : UserControl
    {
        private ListBox _lstHistory = null!;
        private Label _lblTitle = null!;

        public HistoryPanel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Apply theme colors to the history panel.
        /// </summary>
        public void ApplyTheme(Color surface, Color text)
        {
            BackColor = surface;
            _lblTitle.ForeColor = text;
            _lstHistory.BackColor = surface;
            _lstHistory.ForeColor = text;
        }

        private void InitializeComponent()
        {
            _lblTitle = new Label
            {
                Text = "Historique",
                Dock = DockStyle.Top,
                Height = 36,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point),
                Padding = new Padding(12, 0, 0, 0)
            };

            _lstHistory = new ListBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                Margin = new Padding(8)
            };

            this.Padding = new Padding(8);
            Controls.Add(_lstHistory);
            Controls.Add(_lblTitle);
            Size = new Size(260, 520);
            BackColor = SystemColors.Window;
        }

        public void AddEntry(string text) => _lstHistory.Items.Insert(0, text);

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            using var path = GetRoundedPath(ClientRectangle, 12);
            this.Region = new Region(path);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = ClientRectangle;
            rect.Inflate(-1, -1);
            using var path = GetRoundedPath(rect, 12);

            Color border = ControlPaint.Dark(BackColor.IsEmpty ? SystemColors.Control : BackColor);
            using var pen = new Pen(border, 2);
            e.Graphics.DrawPath(pen, path);
        }

        private static GraphicsPath GetRoundedPath(Rectangle r, int radius)
        {
            var path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(r.Left, r.Top, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Top, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.Left, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}

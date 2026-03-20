using System.Drawing.Drawing2D;

namespace Calculatrice_Scientifique.Controls
{
    /// <summary>
    /// UserControl that wraps a readonly TextBox for displaying the current value
    /// or expression. Renders a rounded border and exposes a method to apply
    /// theme colors.
    /// </summary>
    public partial class DisplayScreen : UserControl
    {
        private TextBox _txtDisplay = null!;

        public DisplayScreen()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            _txtDisplay = new TextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Text = "0",
                TextAlign = HorizontalAlignment.Right,
                Font = new Font("Segoe UI Semilight", 18F, FontStyle.Regular, GraphicsUnit.Point),
                BorderStyle = BorderStyle.None
            };

            this.Padding = new Padding(12);
            Controls.Add(_txtDisplay);
            Size = new Size(480, 60);
            BackColor = SystemColors.Window;
            _txtDisplay.BackColor = SystemColors.Window;
            _txtDisplay.ForeColor = SystemColors.ControlText;
        }

        public override string Text { get => _txtDisplay.Text; set => _txtDisplay.Text = value; }

        /// <summary>
        /// Apply theme surface and text color to the control.
        /// </summary>
        public void ApplyTheme(Color surface, Color text)
        {
            BackColor = surface;
            _txtDisplay.BackColor = surface;
            _txtDisplay.ForeColor = text;
            Invalidate();
        }

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

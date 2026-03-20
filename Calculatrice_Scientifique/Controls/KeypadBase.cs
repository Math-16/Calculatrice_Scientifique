using System.Drawing.Drawing2D;
using Calculatrice_Scientifique.Models;

namespace Calculatrice_Scientifique.Controls
{
    /// <summary>
    /// Base class for keypad user controls. Provides shared access to the display
    /// and common helpers used by concrete keypad implementations.
    /// </summary>
    public class KeypadBase : UserControl
    {
        /// <summary>
        /// Calculator model used to perform operations.
        /// </summary>
        protected ICalculator _model;

        /// <summary>
        /// Event raised when a calculation should be recorded in history.
        /// </summary>
        public event EventHandler<HistoryEventArgs>? HistoryAdded;

        /// <summary>
        /// Shared display control provided by the host form.
        /// </summary>
        public Controls.DisplayScreen? Display { get; set; }

        /// <summary>
        /// Creates a new keypad base with the specified calculator model.
        /// </summary>
        public KeypadBase(ICalculator model)
        {
            _model = model;
            DoubleBuffered = true;
            InitializeComponent();
        }

        // Minimal InitializeComponent placeholder used by code-only controls.
        private void InitializeComponent() { }

        /// <summary>
        /// Raise the history added event.
        /// </summary>
        protected void RaiseHistory(string expr, double result)
            => HistoryAdded?.Invoke(this, new HistoryEventArgs(expr, result));

        /// <summary>
        /// Get the current text displayed by the shared display.
        /// </summary>
        protected string GetDisplayText() => Display?.Text ?? "0";

        /// <summary>
        /// Set the text of the shared display if available.
        /// </summary>
        protected void SetDisplayText(string text)
        {
            if (Display is not null)
                Display.Text = text;
        }

        /// <summary>
        /// Draw a subtle rounded border around the keypad.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = ClientRectangle;
            rect.Inflate(-1, -1);
            using var path = GetRoundedPath(rect, 8);
            using var pen = new Pen(ControlPaint.Dark(BackColor), 2);
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

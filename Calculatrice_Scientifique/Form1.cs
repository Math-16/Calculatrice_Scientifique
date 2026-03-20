namespace Calculatrice_Scientifique
{
    // Form1 is obsolete; keep minimal shell to avoid Designer collisions.
    public partial class Form1 : Form
    {
        public Form1()
        {
            // Redirect to the new MainView for runtime
            var main = new Forms.MainView();
            main.Show();
            this.Load += (s, e) => this.BeginInvoke(new Action(() => this.Close()));
        }
    }
}

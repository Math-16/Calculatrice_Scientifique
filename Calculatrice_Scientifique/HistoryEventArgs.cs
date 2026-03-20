namespace Calculatrice_Scientifique
{
    public class HistoryEventArgs : EventArgs
    {
        public string Expression { get; }
        public double Result { get; }
        public HistoryEventArgs(string expr, double result) { Expression = expr; Result = result; }
    }
}

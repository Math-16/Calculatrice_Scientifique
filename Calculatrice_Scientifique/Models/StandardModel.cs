namespace Calculatrice_Scientifique.Models
{
    public class StandardModel : ICalculator
    {
        public double Calculate(double left, double right, string op)
        {
            return op switch
            {
                "+" => left + right,
                "-" => left - right,
                "*" => left * right,
                "/" => right == 0 ? double.NaN : left / right,
                _ => right
            };
        }

        public double ApplyFunction(string name, double value, bool useDegrees)
        {
            // Standard model doesn't support extra functions; passthrough
            return value;
        }
    }
}

namespace Calculatrice_Scientifique.Models
{
    public class ScientificModel : ICalculator
    {
        public double Calculate(double left, double right, string op)
        {
            return op switch
            {
                "+" => left + right,
                "-" => left - right,
                "*" => left * right,
                "/" => right == 0 ? double.NaN : left / right,
                "^" => Math.Pow(left, right),
                _ => right
            };
        }

        public double ApplyFunction(string name, double value, bool useDegrees)
        {
            return name switch
            {
                "sin" => useDegrees ? Math.Sin(DegreesToRadians(value)) : Math.Sin(value),
                "cos" => useDegrees ? Math.Cos(DegreesToRadians(value)) : Math.Cos(value),
                "tan" => useDegrees ? Math.Tan(DegreesToRadians(value)) : Math.Tan(value),
                "log" => Math.Log10(value),
                "ln" => Math.Log(value),
                "√" => Math.Sqrt(value),
                _ => value
            };
        }

        private static double DegreesToRadians(double deg) => deg * Math.PI / 180.0;
    }
}

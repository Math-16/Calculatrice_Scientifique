namespace Calculatrice_Scientifique.Models
{
    // Simple interface for calculator models. Implementations provide basic math operations.
    public interface ICalculator
    {
        double Calculate(double left, double right, string op);
        double ApplyFunction(string name, double value, bool useDegrees);
    }
}

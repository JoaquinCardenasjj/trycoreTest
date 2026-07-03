namespace TrycoreEvm.Domain.Common;

/// <summary>
/// Valores constantes usados en los cálculos de Valor Ganado (EVM),
/// centralizados aquí para evitar números mágicos dispersos en el código.
/// </summary>
public static class EvmConstants
{
    public const decimal MinPercentage = 0m;
    public const decimal MaxPercentage = 100m;
    public const decimal PercentageDivisor = 100m;
    public const decimal PerformanceIndexTarget = 1m;
    public const decimal MinBudget = 0m;
    public const decimal MinActualCost = 0m;
    public const decimal Zero = 0m;
}

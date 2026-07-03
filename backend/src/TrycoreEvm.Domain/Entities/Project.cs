using TrycoreEvm.Domain.Exceptions;
using TrycoreEvm.Domain.Services;
using TrycoreEvm.Domain.ValueObjects;

namespace TrycoreEvm.Domain.Entities;

/// <summary>
/// Proyecto que agrupa un conjunto de actividades. Es la raíz de agregado: toda
/// modificación a sus actividades pasa a través de sus métodos.
/// </summary>
public class Project
{
    private readonly List<ProjectActivity> _activities = new();

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public IReadOnlyCollection<ProjectActivity> Activities => _activities.AsReadOnly();

    private Project()
    {
        // Constructor requerido por Entity Framework Core.
    }

    public Project(string name)
    {
        Id = Guid.NewGuid();
        Name = ValidateName(name);
    }

    public void Rename(string name)
    {
        Name = ValidateName(name);
    }

    public ProjectActivity AddActivity(
        string name,
        decimal budgetAtCompletion,
        decimal plannedPercentComplete,
        decimal actualPercentComplete,
        decimal actualCost)
    {
        var activity = new ProjectActivity(Id, name, budgetAtCompletion, plannedPercentComplete, actualPercentComplete, actualCost);
        _activities.Add(activity);
        return activity;
    }

   

    /// <summary>
    /// Calcula los indicadores EVM consolidados del proyecto sumando los valores
    /// absolutos (BAC, PV, EV, AC) de todas sus actividades. Si el proyecto no tiene
    /// actividades, todas las sumas son cero y el resultado sigue siendo válido.
    /// </summary>
    public EvmIndicators CalculateConsolidatedIndicators()
    {
        var activityIndicators = _activities.Select(activity => activity.CalculateIndicators()).ToList();

        var totalBudgetAtCompletion = _activities.Sum(activity => activity.BudgetAtCompletion);
        var totalActualCost = _activities.Sum(activity => activity.ActualCost);
        var totalPlannedValue = activityIndicators.Sum(indicators => indicators.PlannedValue);
        var totalEarnedValue = activityIndicators.Sum(indicators => indicators.EarnedValue);

        return EvmCalculator.CalculateConsolidated(totalBudgetAtCompletion, totalPlannedValue, totalEarnedValue, totalActualCost);
    }

    private static string ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainValidationException("El nombre del proyecto es obligatorio.");
        }

        return name.Trim();
    }
}

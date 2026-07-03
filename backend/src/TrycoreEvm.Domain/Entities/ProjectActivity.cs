using TrycoreEvm.Domain.Common;
using TrycoreEvm.Domain.Exceptions;
using TrycoreEvm.Domain.Services;
using TrycoreEvm.Domain.ValueObjects;

namespace TrycoreEvm.Domain.Entities;

/// <summary>
/// Actividad de un proyecto. Es la unidad mínima sobre la que se registra avance,
/// presupuesto y costo, y a partir de la cual se calculan los indicadores EVM.
/// </summary>
public class ProjectActivity
{
    public Guid Id { get; private set; }
    public Guid ProjectId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public decimal BudgetAtCompletion { get; private set; }
    public decimal PlannedPercentComplete { get; private set; }
    public decimal ActualPercentComplete { get; private set; }
    public decimal ActualCost { get; private set; }

    public ProjectActivity()
    {
        // Constructor requerido por Entity Framework Core.
    }

    public ProjectActivity(
        Guid projectId,
        string name,
        decimal budgetAtCompletion,
        decimal plannedPercentComplete,
        decimal actualPercentComplete,
        decimal actualCost)
    {
        Id = Guid.NewGuid();
        ProjectId = projectId;
        UpdateDetails(name, budgetAtCompletion, plannedPercentComplete, actualPercentComplete, actualCost);
    }
  

    public void UpdateDetails(
        string name,
        decimal budgetAtCompletion,
        decimal plannedPercentComplete,
        decimal actualPercentComplete,
        decimal actualCost)
    {
        Name = ValidateName(name);
        BudgetAtCompletion = ValidateNonNegative(budgetAtCompletion, nameof(budgetAtCompletion));
        PlannedPercentComplete = ValidatePercentage(plannedPercentComplete, nameof(plannedPercentComplete));
        ActualPercentComplete = ValidatePercentage(actualPercentComplete, nameof(actualPercentComplete));
        ActualCost = ValidateNonNegative(actualCost, nameof(actualCost));
    }

  

    /// <summary>
    /// Calcula los indicadores EVM de esta actividad de forma aislada.
    /// </summary>
    public EvmIndicators CalculateIndicators()
    {
        return EvmCalculator.CalculateForActivity(
            BudgetAtCompletion,
            PlannedPercentComplete,
            ActualPercentComplete,
            ActualCost);
    }

    private static string ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainValidationException("El nombre de la actividad es obligatorio.");
        }

        return name.Trim();
    }

    private static decimal ValidateNonNegative(decimal value, string fieldName)
    {
        if (value < EvmConstants.MinBudget)
        {
            throw new DomainValidationException($"El campo '{fieldName}' no puede ser negativo.");
        }

        return value;
    }

    private static decimal ValidatePercentage(decimal value, string fieldName)
    {
        if (value < EvmConstants.MinPercentage || value > EvmConstants.MaxPercentage)
        {
            throw new DomainValidationException(
                $"El campo '{fieldName}' debe estar entre {EvmConstants.MinPercentage} y {EvmConstants.MaxPercentage}.");
        }

        return value;
    }
}

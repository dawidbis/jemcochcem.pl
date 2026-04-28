using System;
using System.Collections.Generic;

namespace FitApp.Domain.Entities;

public class MealLog
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime Date { get; set; }
    public List<MealLogItem> Items { get; set; } = new List<MealLogItem>();

    public MacroNutrients GetTotalMacros()
    {
        var total = MacroNutrients.Zero();
        foreach (var item in Items)
        {
            if (item.CalculatedMacros != null)
            {
                total.Protein += item.CalculatedMacros.Protein;
                total.Carbs += item.CalculatedMacros.Carbs;
                total.Fat += item.CalculatedMacros.Fat;
                total.Kcal += item.CalculatedMacros.Kcal;
            }
        }
        return total;
    }
}
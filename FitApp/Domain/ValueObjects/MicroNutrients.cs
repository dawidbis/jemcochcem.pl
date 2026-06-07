namespace FitApp.Domain.ValueObjects;

// Mikroskładniki najważniejsze z punktu widzenia zdrowia.
// Błonnik, cukry i tłuszcze nasycone wyrażamy w gramach (g),
// natomiast sód, wapń i żelazo w miligramach (mg).
public record MicroNutrients(
    decimal Fiber,
    decimal Sugars,
    decimal SaturatedFat,
    decimal Sodium,
    decimal Calcium,
    decimal Iron);

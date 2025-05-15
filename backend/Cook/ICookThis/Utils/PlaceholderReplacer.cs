using ICookThis.Modules.Recipes.Dtos;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ICookThis.Utils
{
    public static class PlaceholderReplacer
    {
        private static readonly Regex _rx = new(@"\{(\w+)\}", RegexOptions.Compiled);

        public static string Replace(
    string template,
    IEnumerable<RecipeIngredientResponse> recipeIngredients,
    IEnumerable<StepIngredientResponse> stepIngredients,
    decimal scale)
        {
            if (string.IsNullOrEmpty(template)
                || recipeIngredients == null
                || stepIngredients == null)
            {
                return template;
            }

            return _rx.Replace(template, match =>
            {
                var key = match.Groups[1].Value; // np. "Water"

                // Bazowy składnik po nazwie:
                RecipeIngredientResponse? baseIng = null;
                foreach (var ri in recipeIngredients)
                {
                    if (ri?.Ingredient?.Name != null
                        && string.Equals(ri.Ingredient.Name, key, StringComparison.OrdinalIgnoreCase))
                    {
                        baseIng = ri;
                        break;
                    }
                }
                if (baseIng == null) return match.Value;

                //  Składnik kroku po nazwie:
                StepIngredientResponse? stepIng = null;
                foreach (var si in stepIngredients)
                {
                    if (si?.Ingredient?.Name != null
                        && string.Equals(si.Ingredient.Name, key, StringComparison.OrdinalIgnoreCase))
                    {
                        stepIng = si;
                        break;
                    }
                }
                if (stepIng == null) return match.Value;

                //  Obliczenie ilości
                var qty = baseIng.Qty * stepIng.Fraction * scale;
                var formattedQty = qty % 1 == 0
                    ? qty.ToString("0", CultureInfo.InvariantCulture)
                    : qty.ToString("0.##", CultureInfo.InvariantCulture);

                // 4) Zwrócenie "450 ml water"
                var unitSym = baseIng.Unit?.Symbol ?? "";
                var name = baseIng.Ingredient.Name.ToLowerInvariant();
                return $"{formattedQty} {unitSym} {name}";
            });
        }
    }
}
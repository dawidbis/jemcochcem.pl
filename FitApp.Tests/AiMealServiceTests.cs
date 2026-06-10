using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FitApp.Domain.Services;
using Xunit;

namespace FitApp.Tests.Services
{
    // Każdy posiłek składa się z produktów, które mają GŁÓWNE makro równe celowi
    // (np. kurczak dostarcza dokładnie `protein` g białka) plus POBOCZNE makra
    // (ryż do kurczaka wnosi też trochę białka). Testy sprawdzają ten kontrakt —
    // nie sumę wszystkich makr, lecz obecność produktu dostarczającego cel dla każdego makra.
    public class AiMealServiceTests
    {
        private readonly AiMealService _service = new();

        [Fact]
        public async Task GenerateMealAsync_WithAllMacros_ShouldReturn3Items()
        {
            var result = await _service.GenerateMealAsync(30, 50, 10);

            result.Should().HaveCount(3);
        }

        [Fact]
        public async Task GenerateMealAsync_WithAllMacros_ShouldContainItemDeliveringProteinGoal()
        {
            double proteinGoal = 30;

            var result = await _service.GenerateMealAsync(proteinGoal, 50, 10);

            result.Should().Contain(i => Math.Abs(i.Protein - proteinGoal) < 0.2,
                "jeden produkt powinien dostarczać dokładnie tyle białka ile wynosi cel");
        }

        [Fact]
        public async Task GenerateMealAsync_WithAllMacros_ShouldContainItemDeliveringCarbsGoal()
        {
            double carbsGoal = 50;

            var result = await _service.GenerateMealAsync(30, carbsGoal, 10);

            result.Should().Contain(i => Math.Abs(i.Carbs - carbsGoal) < 0.2,
                "jeden produkt powinien dostarczać dokładnie tyle węglowodanów ile wynosi cel");
        }

        [Fact]
        public async Task GenerateMealAsync_WithAllMacros_ShouldContainItemDeliveringFatGoal()
        {
            double fatGoal = 10;

            var result = await _service.GenerateMealAsync(30, 50, fatGoal);

            result.Should().Contain(i => Math.Abs(i.Fat - fatGoal) < 0.2,
                "jeden produkt powinien dostarczać dokładnie tyle tłuszczu ile wynosi cel");
        }

        [Fact]
        public async Task GenerateMealAsync_AllItemsShouldHavePositiveWeight()
        {
            var result = await _service.GenerateMealAsync(30, 50, 10);

            result.Should().AllSatisfy(i => i.WeightInGrams.Should().BePositive());
        }

        [Fact]
        public async Task GenerateMealAsync_AllItemsShouldHaveWeightRoundedTo1Decimal()
        {
            var result = await _service.GenerateMealAsync(30, 50, 10);

            result.Should().AllSatisfy(i =>
                i.WeightInGrams.Should().Be(Math.Round(i.WeightInGrams, 1)));
        }

        [Theory]
        [InlineData(0, 50, 10)]  // brak białka → 2 produkty
        [InlineData(30, 0, 10)]  // brak węgli → 2 produkty
        [InlineData(30, 50, 0)]  // brak tłuszczu → 2 produkty
        public async Task GenerateMealAsync_WithZeroMacro_ShouldSkipThatIngredient(double p, double c, double f)
        {
            int expectedCount = (p > 0 ? 1 : 0) + (c > 0 ? 1 : 0) + (f > 0 ? 1 : 0);

            var result = await _service.GenerateMealAsync(p, c, f);

            result.Should().HaveCount(expectedCount);
        }

        [Theory]
        [InlineData(-10, 50, 10)]  // ujemne białko → traktowane jak 0
        [InlineData(30, -5, 10)]   // ujemne węgle → traktowane jak 0
        public async Task GenerateMealAsync_WithNegativeValues_ShouldTreatThemAsZero(double p, double c, double f)
        {
            int expectedCount = (p > 0 ? 1 : 0) + (c > 0 ? 1 : 0) + (f > 0 ? 1 : 0);

            var result = await _service.GenerateMealAsync(p, c, f);

            result.Should().HaveCount(expectedCount);
        }
    }
}

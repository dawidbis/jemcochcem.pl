using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using Xunit;
using Backend.dtos;
namespace Backend.Tests;

public class FoodApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public FoodApiTests(WebApplicationFactory<global::Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetFood_ReturnsCorrectProduct()
    {
        var response = await _client.GetAsync("/food");
        var product = await response.Content.ReadFromJsonAsync<FoodProductDTO>();

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(product);
        Assert.Equal("Twaróg chudy", product.Name);
        Assert.Equal(90, product.Kcal);
    }
}
namespace FitApp.Application.Interfaces;

using FitApp.Application.DTOs;
using System.Threading.Tasks;

public interface IOffApiClient
{
    Task<FoodDto> FetchProductByBarcode(string barcode);
}
namespace Backend.services;
using Backend.dtos;
public interface IOffApiClient{
    Task<FoodProductDTO> FetchFromExternal(String code);
}
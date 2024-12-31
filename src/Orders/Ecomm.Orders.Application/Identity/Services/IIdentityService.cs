using Ecomm.Orders.Application.Identity.DTOs;

namespace Ecomm.Orders.Application.Identity.Services;

public interface IIdentityService
{
    public Task<RegisterUserResponseDto> RegisterAsync(RegisterUserDto registerUserDto);
    public Task<RegisterUserResponseDto> LoginAsync(LoginUserDto loginUserDto);
}
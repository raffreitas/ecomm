using Ecomm.Orders.Application.Identity.DTOs;
using Ecomm.Orders.Application.Identity.Services;

using Microsoft.AspNetCore.Identity;

namespace Ecomm.Orders.Infrastructure.Identity.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<IdentityUser> _userManager;

    public IdentityService(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<RegisterUserResponseDto> RegisterAsync(RegisterUserDto registerUserDto)
    {
        var identityUser = new IdentityUser
        {
            UserName = registerUserDto.Email, Email = registerUserDto.Email, EmailConfirmed = true,
        };

        var result = await _userManager.CreateAsync(identityUser, registerUserDto.Password);

        if (!result.Succeeded)
            throw new Exception("An error occured while registering user");

        return new RegisterUserResponseDto(identityUser.Id);
    }

    public Task<RegisterUserResponseDto> LoginAsync(LoginUserDto loginUserDto)
    {
        throw new NotImplementedException();
    }
}
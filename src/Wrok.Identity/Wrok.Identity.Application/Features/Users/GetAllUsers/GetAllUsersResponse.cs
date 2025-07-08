using Wrok.Identity.Application.Dtos.Users;

namespace Wrok.Identity.Application.Features.Users.GetAllUsers;

public sealed record GetAllUsersResponse(
    int Count, 
    List<UserDto> Data);

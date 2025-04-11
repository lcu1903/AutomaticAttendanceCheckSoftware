using System.Models;
using Core.Interfaces;

namespace System.Service.Interface;
public interface IAccountService : IDomainService
{
    Task<LoginRes> LoginAsync (LoginReq req);
    Task<LoginRes> RegisterAsync (RegisterReq req);
    Task<LoginRes> RefreshTokenAsync (RefreshTokenReq req);
    Task<UserRes?> ChangePasswordAsync (ChangePasswordReq req);
}


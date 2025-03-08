using System.Models;
using Core.Interfaces;

namespace System.Service.Interface;
public interface IAccountService : IDomainService
{
    public Task<LoginRes> LoginAsync (LoginReq req);
    public Task<LoginRes> RegisterAsync (RegisterReq req);
    public Task<LoginRes> RefreshTokenAsync (RefreshTokenReq req);
}


using System;

namespace Ecom.Application.Interfaces
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
    }
}
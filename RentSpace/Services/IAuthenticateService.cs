using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentSpace.Services
{
    public interface IAuthenticateService
    {
        string Authenticate(string email);
    }
}

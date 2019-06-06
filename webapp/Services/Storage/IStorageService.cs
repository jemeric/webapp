using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webapp.Services.Storage
{
    public interface IStorageService
    {
        Task Copy(string fromDir, string toDir);
        Task<bool> Exists(string fileOrDir);
    }
}

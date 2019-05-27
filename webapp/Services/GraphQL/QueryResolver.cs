using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webapp.Models.GraphQL;

namespace webapp.Services.GraphQL
{
    public class QueryResolvers
    {
        public QueryResolvers()
        {

        }

        /* public async Task<AppSettings> GetSettings(int? test)
        {
            Console.WriteLine($"Iteger: {test}");
            return await Task.Run(() => new AppSettings());
        } */

        public async Task<AppSettings> GetSettings(int? test)
        {
            return await Task.Run(() => new AppSettings());
        }

    }
}

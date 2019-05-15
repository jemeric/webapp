using GraphQL.Types;
using Microsoft.AspNetCore.Hosting;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace webapp.Services
{
    public class GraphQLService
    {
        private readonly AsyncLazy<ISchema> schema;

        public GraphQLService(IHostingEnvironment env)
        {
            schema = new AsyncLazy<ISchema>(async () =>
            {
                return await LoadSchema($"{env.ContentRootPath}/Assets/GraphQL/schema.graphql");
            });
        }

        public async Task<ISchema> GetSchema()
        {
            return await schema;
        }

        public static async Task<ISchema> LoadSchema(string path)
        {
            string schema = await File.ReadAllTextAsync(path);
            return Schema.For(schema);
        }
    }
}

﻿using Microsoft.AspNetCore.Hosting;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using webapp.Services.GraphQL;
using HotChocolate;
using HotChocolate.Types;
using webapp.Models.Settings;
using webapp.Models.Settings.Assets;

namespace webapp.Services
{
    public class GraphQLService
    {
/*        public GraphQLService(IHostingEnvironment env)
        {
            schema = new AsyncLazy<ISchema>(async () =>
            {
                return await LoadSchema($"{env.ContentRootPath}/Assets/GraphQL/schema.graphql");
            });
        } */

        public static ISchema LoadSchema(string path)
        {
            string schema = File.ReadAllText(path);
            // TODO there is a nicer way to do this in new version - see https://github.com/ChilliCream/hotchocolate/issues/764#issuecomment-495184415
            return Schema.Create(schema, c =>
            {
                //c.BindType<>().To("Query");
                c.BindResolver<QueryResolvers>().To("Query");
                c.BindResolver<MutationResolver>().To("Mutation");
                c.BindResolver<AppSettingsResolver>().To<AppSettings>();
                c.BindResolver<AssetConfigResolver>().To<AssetConfig>();
                c.BindResolver<AssetInstanceResolver>().To<AssetInstance>();
                c.BindType<AssetVersion>().To("AssetVersion");
                c.BindType<AppClock>().To("AppClock");
                c.RegisterType<DateTimeType>();
                c.RegisterType<LongType>();
                c.RegisterType<TimeSpanType>();
                c.RegisterAuthorizeDirectiveType(); // https://hotchocolate.io/docs/authorization
            });
        }
    }
} 

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using webapp.Models;
using GraphQL.Types;
using webapp.Services;

namespace webapp.Controllers
{
    public class GraphQLController : Controller
    {
        private readonly GraphQLService graphQLService;
        private readonly IDocumentExecuter _documentExecuter;

        public GraphQLController(GraphQLService graphQLService, IDocumentExecuter documentExecuter)
        {
            this.graphQLService = graphQLService;
            _documentExecuter = documentExecuter;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GraphQLQuery query)
        {
            if (query == null) { throw new ArgumentNullException(nameof(query)); }
            Inputs inputs = query.Variables.ToInputs();
            ExecutionOptions executionOptions = new ExecutionOptions
            {
                Schema = await graphQLService.GetSchema(),
                Query = query.Query,
                Inputs = inputs
            };

            var result = await _documentExecuter.ExecuteAsync(executionOptions).ConfigureAwait(false);

            if(result.Errors?.Count > 0)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }


    }
}

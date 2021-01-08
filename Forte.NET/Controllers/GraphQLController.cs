using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using GraphQL.Conventions;
using Microsoft.AspNetCore.Mvc;

namespace Forte.NET.Controllers {
    // [ApiController]
    // [Route("graphql")]
    public class GraphQLController {
        private readonly GraphQLEngine _engine;

        public GraphQLController(GraphQLEngine engine) {
            _engine = engine;
        }

        // [HttpPost]
        // public async Task<IActionResult> Post([FromBody] JsonElement queryJson) {
        //     var query = queryJson.GetRawText();
        //     var result = await _engine.NewExecutor().WithRequest(query).ExecuteAsync();
        //     var response = await _engine.SerializeResultAsync(result);
        //
        //     var status = HttpStatusCode.OK;
        //     switch (result.Errors?.Select(error => error.Code).FirstOrDefault()) {
        //         case "VALIDATION_ERROR":
        //             status = HttpStatusCode.BadRequest;
        //             break;
        //     }
        //
        //     return new ContentResult {
        //         Content = response,
        //         ContentType = "application/json; charset=utf-8",
        //         StatusCode = (int) status
        //     };
        // }
    }
}

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SampleMinimalAPI.Common;

namespace SampleMinimalAPI.Test
{
    public record TestItem(string id,string name);
    public record CreateQuery(string Name, [FromBody]TestItem Item) : IHttpRequest;

    public class CreateQueryHandler : IRequestHandler<CreateQuery,IResult>
    {
        public async Task<IResult> Handle(CreateQuery request,CancellationToken cancellationToken)
        {
            return Results.Ok(request.Name+request.Item.id + request.Item.name);
        }

        
    }

}

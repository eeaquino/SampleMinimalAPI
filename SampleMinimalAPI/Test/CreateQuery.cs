using MediatR;
using Microsoft.AspNetCore.Mvc;
using SampleMinimalAPI.Common;

namespace SampleMinimalAPI.Test
{
    public record TestItem(string id,string name);

    [MediatorGet("api/test1","Test1",false)]
    public record CreateQuery(string Name, string item) : IHttpRequest;

    public class CreateQueryHandler : IRequestHandler<CreateQuery,IResult>
    {
        public async Task<IResult> Handle(CreateQuery request,CancellationToken cancellationToken)
        {
            return Results.Ok(request.Name+request.item);
        }

        
    }
    [MediatorPost("api/test1/Create","Test1",true,DataBindEnum.FromBody)]
    public record CreateCommand(string Name, TestItem Item) : IHttpRequest;

    public class CreateCommandHandler : IRequestHandler<CreateCommand,IResult>
    {
        public async Task<IResult> Handle(CreateCommand request,CancellationToken cancellationToken)
        {
            return Results.Ok(request.Name+request.Item.id + request.Item.name);
        }

        
    }

    [MediatorPost("api/test2/Debug","Test2",false)]
    public record CreateTest2Command(string Name, string Item) : IHttpRequest;

    public class CreateTest2CommandHandler : IRequestHandler<CreateTest2Command,IResult>
    {
        public async Task<IResult> Handle(CreateTest2Command request,CancellationToken cancellationToken)
        {
            return Results.Ok(request.Name+request.Item);
        }

        
    }

}

using API.Common;
using MediatR;

namespace SampleMinimalAPI.Test
{
    public record TestItem(string id,string name);

    [MediatorGet("api/test1","Test1",false)]
    public record CreateQuery(string Name, string item) : IHttpRequest<string>;

    public class CreateQueryHandler : IRequestHandler<CreateQuery,APIResult<string>>
    {
        public async Task<APIResult<string>> Handle(CreateQuery request,CancellationToken cancellationToken)
        {
            return request.Name+request.item;
        }

        
    }
    [MediatorPost("api/test1/Create","Test1",true,DataBind.FromBody)]
    public record CreateCommand(string Name, TestItem Item) : IHttpRequest<string>;

    public class CreateCommandHandler : IRequestHandler<CreateCommand,APIResult<string>>
    {
        public async Task<APIResult<string>> Handle(CreateCommand request,CancellationToken cancellationToken)
        {
            return request.Name+request.Item.id + request.Item.name;
        }

        
    }

    [MediatorPost("api/test2/Debug","Test2",false)]
    public record CreateTest2Command(string Name, string Item) : IHttpRequest<string>;

    public class CreateTest2CommandHandler : IRequestHandler<CreateTest2Command,APIResult<string>>
    {
        public async Task<APIResult<string>> Handle(CreateTest2Command request,CancellationToken cancellationToken)
        {
            return request.Name+request.Item;
        }

        
    }

}

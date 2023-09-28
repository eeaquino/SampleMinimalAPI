using MediatR;

namespace SampleMinimalAPI.Common
{
    public interface IHttpRequest:IRequest<IResult>
    {
    }
}

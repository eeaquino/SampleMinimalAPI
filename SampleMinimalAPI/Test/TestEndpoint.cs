using SampleMinimalAPI.Common;

namespace SampleMinimalAPI.Test
{
    public class TestEndpoint : IEndpointDefinition
    {
        public void DefineEndpoints(WebApplication app)
        {
            app.MediatorPost<CreateQuery>("/api/Test");
        }
    }
}

//using MediatR;
//using Microsoft.AspNetCore.Mvc;

//namespace SampleMinimalAPI.Common
//{
//    public static class MediatrExtensions
//    {
//        public static WebApplication MediatorGet<TRequest>(this WebApplication app, string path)
//            where TRequest : IHttpRequest
//        {
//            app.MapGet(path, async (IMediator mediator, [AsParameters]TRequest request) => await mediator.Send(request)) .RequireAuthorization();
//            return app;
//        }

//        public static WebApplication MediatorPost<TRequest>(this WebApplication app, string path)
//            where TRequest : IHttpRequest
//        {
//            app.MapPost(path, async (IMediator mediator, [AsParameters] TRequest request) => await mediator.Send(request));
//            return app;
//        }

//        public static WebApplication MediatorPut<TRequest>(this WebApplication app, string path)
//            where TRequest : IHttpRequest
//        {
//            app.MapPut(path, async (IMediator mediator, [AsParameters] TRequest request) => await mediator.Send(request));
//            return app;
//        }

//        public static WebApplication MediatorDelete<TRequest>(this WebApplication app, string path)
//            where TRequest : IHttpRequest
//        {
//            app.MapDelete(path, async (IMediator mediator, [AsParameters]TRequest request) => await mediator.Send(request));
//            return app;
//        }

//        public static WebApplication MediatorPatch<TRequest>(this WebApplication app, string path)
//            where TRequest : IHttpRequest
//        {
//            app.MapPatch(path, async (IMediator mediator, [AsParameters] TRequest request) => await mediator.Send(request));
//            return app;
//        }
//    }
//}

using Microsoft.CodeAnalysis;

namespace APIRouteGenerator;

[Generator]
public class GenerateModels : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {

    }

    public void Execute(GeneratorExecutionContext context)
    {
        var DataBind = """
                       using System;
                       namespace API.Common;

                       public enum DataBind
                       {
                           FromBody,
                           FromForm,
                           AsParameters
                       }
                       """;
        context.AddSource("DataBind.g.cs", DataBind);

        var convertToBody = """
                            using System;
                            namespace API.Common;

                            public class ConvertToBodyAttribute : Attribute
                            {
                                private string property { get; set; }
                                public ConvertToBodyAttribute(string property)
                                {
                                    this.property = property;
                                }
                            }
                            """;
        context.AddSource("ConvertToBodyAttribute.g.cs", convertToBody);

        var FastAccessAttribute = """
                                  using System;
                                  namespace API.Common;

                                  public class FastAccessAttribute:Attribute
                                  {
                                  }
                                  """;
        context.AddSource("FastAccessAttribute.g.cs", FastAccessAttribute);

        var apiResponse = """
                          using System;
                          namespace API.Common;

                          public class APIResponse<T>
                          {
                              public T? Data { get; set; } = default;
                              public bool Success { get; set; } = false;
                              public string Error { get; set; } = "";
                          }
                          """;
        context.AddSource("APIResponse.g.cs", apiResponse);

        var apiResult = """
                        using System;
                        using Microsoft.AspNetCore.Http.HttpResults;

                        namespace API.Common;

                        public class APIResult<T> : IResult
                        {
                            //implement decorator patter to add more functionality to the result Results<Ok<APIResponse<T>>,NotFound>
                            private Results<Ok<APIResponse<T>>, NotFound> _result { get; set; }
                            private Ok<APIResponse<T>> Result
                            {
                                get => _result.GetPrivatePropertyValue<Ok<APIResponse<T>>>("Result");
                            }
                            public APIResponse<T>? Value
                            {
                                get => _result.GetPrivatePropertyValue<Ok<APIResponse<T>>>("Result").Value;
                            }
                        
                        
                        
                            public APIResult(Results<Ok<APIResponse<T>>, NotFound> result)
                            {
                                _result = result;
                            }
                        
                            public static implicit operator APIResult<T>(Results<Ok<APIResponse<T>>, NotFound> result)
                            {
                                return new APIResult<T>(result);
                            }
                            //create implicit conversion to allow for APIResult<T> to be returned from a controller
                            public static implicit operator Results<Ok<APIResponse<T>>, NotFound>(APIResult<T> result)
                            {
                                return result._result;
                            }
                        
                            public static implicit operator APIResult<T>(T result)
                            {
                                return new APIResult<T>(result.ToResponse());
                            }
                        
                            public async Task ExecuteAsync(HttpContext httpContext)
                            {
                                await _result.ExecuteAsync(httpContext);
                            }
                        }

                        """;
        context.AddSource("APIResult.g.cs", apiResult);

        var mediatorRequestAttribute = """
                                       using System;
                                       namespace API.Common;

                                       public class MediatorRequestAttributes : Attribute
                                       {
                                           private bool secure;
                                           public string Route { get; set; }
                                           public string Tag { get; set; }
                                           public DataBind DataBind { get; set; }
                                       
                                           public MediatorRequestAttributes(string route, string tag, bool secure = false,
                                               DataBind dataBind = DataBind.AsParameters)
                                           {
                                               this.Route = route;
                                               this.Tag = tag;
                                               this.secure = secure;
                                               this.DataBind = dataBind;
                                           }
                                       }

                                       public class MediatorGetAttribute : MediatorRequestAttributes
                                       {
                                           public MediatorGetAttribute(string route, string tag, bool secure = false, DataBind dataBind = DataBind.AsParameters) : base(route, tag, secure, dataBind)
                                           {
                                           }
                                       }

                                       public class MediatorPostAttribute : MediatorRequestAttributes
                                       {
                                           public MediatorPostAttribute(string route, string tag, bool secure = false, DataBind dataBind = DataBind.AsParameters) : base(route, tag, secure, dataBind)
                                           {
                                           }
                                       }

                                       public class MediatorDeleteAttribute : MediatorRequestAttributes
                                       {
                                           public MediatorDeleteAttribute(string route, string tag, bool secure = false, DataBind dataBind = DataBind.AsParameters) : base(route, tag, secure, dataBind)
                                           {
                                           }
                                       }

                                       public class MediatorPutAttribute : MediatorRequestAttributes
                                       {
                                           public MediatorPutAttribute(string route, string tag, bool secure = false, DataBind dataBind = DataBind.AsParameters) : base(route, tag, secure, dataBind)
                                           {
                                           }
                                       }

                                       public class MediatorPatchAttribute : MediatorRequestAttributes
                                       {
                                           public MediatorPatchAttribute(string route, string tag, bool secure = false, DataBind dataBind = DataBind.AsParameters) : base(route, tag, secure, dataBind)
                                           {
                                           }
                                       }
                                       """;
        context.AddSource("MediatorRequestAttributes.g.cs", mediatorRequestAttribute);

        var endpointDefinitions = """
                                  using System;
                                  using MediatR;
                                  using FluentValidation.Results;
                                  namespace API.Common;

                                  public record ValidationFailed(IEnumerable<ValidationFailure> Errors)
                                  {
                                      public ValidationFailed(ValidationFailure error) : this(new[] { error })
                                      {
                                      }
                                  }
                                  public interface IHttpRequest<T>:IRequest<APIResult<T>>
                                  {
                                  }

                                  public interface IEndpointDefinition
                                  {
                                      void DefineEndpoints(WebApplication app);
                                  }

                                  public static class EndpointExtensions
                                  {
                                      public static void AddEndpointDefinitions(
                                          this IServiceCollection services, params Type[] scanMarkers)
                                      {
                                          var endpointDefinitions = new List<IEndpointDefinition>();
                                  
                                          foreach (var marker in scanMarkers)
                                          {
                                              endpointDefinitions.AddRange(
                                                  marker.Assembly.ExportedTypes
                                                      .Where(x => typeof(IEndpointDefinition).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                                                      .Select(Activator.CreateInstance).Cast<IEndpointDefinition>()
                                              );
                                          }
                                  
                                  
                                  
                                          services.AddSingleton(endpointDefinitions as IReadOnlyCollection<IEndpointDefinition>);
                                      }
                                  
                                      public static void UseEndpointDefinitions(this WebApplication app)
                                      {
                                          var definitions = app.Services.GetRequiredService<IReadOnlyCollection<IEndpointDefinition>>();
                                  
                                          foreach (var endpointDefinition in definitions)
                                          {
                                              endpointDefinition.DefineEndpoints(app);
                                          }
                                      }
                                      
                                  }
                                  """;
        context.AddSource("EndpointExtensions.g.cs", endpointDefinitions);

        var amcException = """
                           using System;
                           namespace API.Common;

                           public class AMCException : Exception
                           {
                               public AMCException(string message) : base(message)
                               {
                               }
                           }
                           """;
        context.AddSource("AMCException.g.cs", amcException);

        var commonExtensions = """
                               using System;
                               using System.Reflection;
                               using Microsoft.AspNetCore.Http.HttpResults;
                               using FluentValidation;
                               using FluentValidation.Results;
                               namespace API.Common;

                               public static class AMCExtensions
                               {
                                    public static T GetPrivatePropertyValue<T>(this object obj, string propName)
                                   {
                                       if (obj == null) throw new ArgumentNullException("obj");
                                       PropertyInfo pi = obj.GetType().GetProperty(propName,
                                           BindingFlags.Public | BindingFlags.NonPublic |
                                           BindingFlags.Instance);
                                       if (pi == null)
                                           throw new ArgumentOutOfRangeException("propName",
                                               string.Format("Property {0} was not found in Type {1}", propName,
                                                   obj.GetType().FullName));
                                       return (T) pi.GetValue(obj, null);
                                   }
                                   public static T GetPrivateFieldValue<T>(this object obj, string propName)
                                   {
                                       if (obj == null) throw new ArgumentNullException("obj");
                                       Type t = obj.GetType();
                                       FieldInfo fi = null;
                                       while (fi == null && t != null)
                                       {
                                           fi = t.GetField(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                                           t = t.BaseType;
                                       }
                                       if (fi == null)
                                           throw new ArgumentOutOfRangeException("propName",
                                               string.Format("Field {0} was not found in Type {1}", propName,
                                                   obj.GetType().FullName));
                                       return (T) fi.GetValue(obj);
                                   }
                                   public static Results<Ok<APIResponse<T>>,NotFound> ToResponse<T>(this T obj)
                                   {
                                       return TypedResults.Ok(new APIResponse<T>()
                                       {
                                           Success = true,
                                           Error = "",
                                           Data = obj
                                       });
                                   }
                                   public static Results<Ok<APIResponse<T>>,NotFound> ToError<T>(this string error)
                                   {
                                       return TypedResults.Ok(new APIResponse<T>()
                                       {
                                           Success = false,
                                           Error = error,
                                           Data = default(T)
                                       });
                                   }
                                   
                                   public static Results<Ok<APIResponse<T>>,NotFound> ToError<T>(this ValidationFailed vf)
                                   {
                                       return TypedResults.Ok( new APIResponse<T>()
                                       {
                                           Success = false,
                                           Error = string.Join(Environment.NewLine,vf) ,
                                           Data = default(T)
                                       });
                                   }
                                   public static Results<Ok<APIResponse<T>>,NotFound> ToError<T>(this ValidationResult? vf)
                                   {
                                       if(vf==null) return TypedResults.Ok(new APIResponse<T>()
                                       {
                                           Success = false,
                                           Error = "Something is Invalid" ,
                                           Data = default(T)
                                       });
                                       return TypedResults.Ok( new APIResponse<T>()
                                       {
                                           Success = false,
                                           Error = string.Join(Environment.NewLine,vf.Errors) ,
                                           Data = default(T)
                                       });
                                   }
                               }
                               """;
        context.AddSource("AMCExtensions.g.cs", commonExtensions);
    }
}
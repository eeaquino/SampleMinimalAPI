using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SourceGenerator.Models;

namespace APIRouteGenerator;


// This generator generates endpoints for all classes that implement IHttpRequest and have the Mediator attribute.
// The generated endpoints are defined in classes that implement IEndpointDefinition and are grouped by the tag value of the Mediator attribute.
// The generated endpoints are defined using the app.Map{method} method and are secured using the .RequireAuthorization() method if the secureValue is true.
// The generated endpoints are defined using the app.Map{method} method and are defined with the routeValue and tagValue of the Mediator attribute.
// The generated endpoints are defined using the app.Map{method} method and are defined with the request object of the class that implements IHttpRequest.
// The generated endpoints are defined using the app.Map{method} method and are defined with the convertToBodyType object of the ConvertToBody attribute if it exists.
// The generated endpoints are defined using the app.Map{method} method and are defined with the databindValue object of the Mediator attribute.
// The generated endpoints are defined using the app.Map{method} method and are defined with the namespace and name of the class that implements IHttpRequest.
// The generated endpoints are defined using the app.Map{method} method and are defined with the IMediator object.
// The generated endpoints are defined using the app.Map{method} method and are defined with the async lambda expression that calls the mediator.Send method with the request object.
// The generated endpoints are defined using the app.Map{method} method and are defined with the WithTags method that takes the tagValue of the Mediator attribute.
// The generated endpoints are defined using the app.Map{method} method and are defined with the [AsParameters], [FromBody], or [FromForm] attribute depending on the databindValue of the Mediator attribute.

[Generator]
public class IncrementalGenerateEndpoint : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: (c, _) => c is RecordDeclarationSyntax && (((RecordDeclarationSyntax)c).Identifier.ToString().EndsWith("Query") || ((RecordDeclarationSyntax)c).Identifier.ToString().EndsWith("Command")),
            transform: (n, _) => (RecordDeclarationSyntax)n.Node
        ).Where(w => w is not null);

        var compilation = context.CompilationProvider.Combine(provider.Collect());

        context.RegisterSourceOutput(compilation,
            (spc, source) => Execute(spc, source.Left, source.Right)
                );
    }

    private void Execute(SourceProductionContext context, Compilation left, ImmutableArray<RecordDeclarationSyntax> recordList)
    {

        Dictionary<string, StringBuilder> generatedCode = new Dictionary<string, StringBuilder>();
        //get project namespace
        var projectNamespace = left.AssemblyName;

        var records = recordList
            .Where(w => (w.Identifier.ToString().EndsWith("Query") || w.Identifier.ToString().EndsWith("Command"))
                      && (w.BaseList?.Types.Select(s => s.Type.ToString()).Any(a => a.StartsWith("IHttpRequest")) ?? false));
        foreach (var r in records)
        {
            //get list of attributes
            var attributes = r.AttributeLists.SelectMany(s => s.Attributes).ToList();
            if (attributes.Any(a => a.Name.ToString().StartsWith("Mediator")))
            {
                var convertToBody = attributes.FirstOrDefault(f => f.Name.ToString().StartsWith("ConvertToBody"));
                var route = attributes.FirstOrDefault(f => f.Name.ToString().StartsWith("Mediator"));
                var name = r.Identifier.ToString();
                //get namespace of class
                var ns = r.Ancestors().OfType<NamespaceDeclarationSyntax>().FirstOrDefault()?.Name.ToString()
                         ?? r.Ancestors().OfType<FileScopedNamespaceDeclarationSyntax>().FirstOrDefault()?.Name.ToString();
                var method = route.Name.ToString().Replace("Mediator", "").Replace("Attribute", "");
                var routeValue = route.ArgumentList.Arguments[0].ToString().TrimStart('"').TrimEnd('"');
                var tagValue = route.ArgumentList.Arguments[1].ToString().TrimStart('"').TrimEnd('"');
                var secureValue = route.ArgumentList.Arguments[2].ToString().TrimStart('"').TrimEnd('"') == "true";
                var databindValue = GetDataBindFromString(route.ArgumentList.Arguments[3].ToString().TrimStart('"').TrimEnd('"'));
                if (!generatedCode.ContainsKey(tagValue))
                {
                    generatedCode.Add(tagValue, InitStringBuilder(tagValue, projectNamespace));
                }
                var generatedCodeForTag = generatedCode[tagValue];
                //Create the app.MapGet(path, async (IMediator mediator, [AsParameters]TRequest request) => await mediator.Send(request));
                if (convertToBody == null)
                {
                    //generatedCodeForTag.AppendLine($"string {RandomString(10)}{counter++} =\" Getting inner class {(convertToBody==null?"null":convertToBody.AttributeClass)}\";");
                    generatedCodeForTag.AppendLine($"            app.Map{method}(\"{routeValue}\", async (IMediator mediator, {GetTypeFromDataBind(databindValue)}{ns}.{name} request) => await mediator.Send(request)).WithTags(\"{tagValue}\"){(secureValue ? ".RequireAuthorization()" : "")};");
                }
                else
                {
                    var convertToBodyType = convertToBody.ArgumentList.Arguments[0].ToString().TrimStart('"').TrimEnd('"');
                    generatedCodeForTag.AppendLine(
                        $"            app.Map{method}(\"{routeValue}\", async (IMediator mediator, {GetTypeFromDataBind(databindValue)}{convertToBodyType} request) => await mediator.Send(new {ns}.{name}(request))).WithTags(\"{tagValue}\"){(secureValue ? ".RequireAuthorization()" : "")};");

                }
            }
        }
        //Complete Generated string and save generated code to file
        foreach (var item in generatedCode)
        {
            var generatedString = CompleteGeneratedString(item.Value);
            context.AddSource($"{item.Key}Endpoints.g.cs", generatedString);
        }

    }

    private DataBind GetDataBindFromString(string db)
    {
        return db switch
        {
            "DataBind.AsParameters" => DataBind.AsParameters,
            "DataBind.FromBody" => DataBind.FromBody,
            "DataBind.FromForm" => DataBind.FromForm,
            _ => DataBind.AsParameters
        };
    }
    private StringBuilder InitStringBuilder(string tag, string projectName)
    {
        var generatedCode = new StringBuilder();
        generatedCode.AppendLine("using MediatR;");
        generatedCode.AppendLine("using Microsoft.AspNetCore.Mvc;");
        generatedCode.AppendLine("using AMC.Common;");
        generatedCode.AppendLine($"using {projectName}.Common;");
        generatedCode.AppendLine($"namespace {projectName}.Generated");
        generatedCode.AppendLine("{");
        generatedCode.AppendLine($"    public class {tag}Endpoints:IEndpointDefinition");
        generatedCode.AppendLine("    {");
        generatedCode.AppendLine("        public void DefineEndpoints(WebApplication app)");
        generatedCode.AppendLine("        {");
        return generatedCode;
    }

    private string CompleteGeneratedString(StringBuilder generatedCode)
    {
        generatedCode.AppendLine("        }");
        generatedCode.AppendLine("    }");
        generatedCode.AppendLine("}");
        return generatedCode.ToString();
    }

    private string GetTypeFromDataBind(DataBind dataBind)
    {
        return dataBind switch
        {
            DataBind.AsParameters => "[AsParameters]",
            DataBind.FromBody => "[FromBody]",
            DataBind.FromForm => "[FromForm]",
            _ => "[AsParameters]"
        };
    }
}


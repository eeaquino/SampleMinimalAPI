using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace APIRouteGenerator
{
    public enum DataBindEnum
    {
        FromBody,
        FromForm,
        AsParameters
    }
    [Generator]
    public class APISourceGenerator:ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            
        }

        public void Execute(GeneratorExecutionContext context)
        {
            Dictionary<string,StringBuilder> generatedCode = new Dictionary<string, StringBuilder>();
            //find all classes that implement IHttpRequest
            var classes = context.Compilation
                .GetSymbolsWithName(x => x.EndsWith("Query") || x.EndsWith("Command"),SymbolFilter.Type)
                .OfType<INamedTypeSymbol>()
                .Where(x => x.Interfaces.Any(y => y.Name == "IHttpRequest"));

            
            
            //get project namespace
            var projectNamespace = context.Compilation.AssemblyName;
            
            foreach (var c in classes)
            {
                var name = c.Name;
                //get namespace of class
                var ns = c.ContainingNamespace;
                var route = c.GetAttributes().FirstOrDefault(x => x.AttributeClass.Name.StartsWith("Mediator") && x.AttributeClass.Name.EndsWith("Attribute"));
                //extract middle part of attribute
                if (route != null)
                {
                    var method = route.AttributeClass.Name.Replace("Mediator","").Replace("Attribute","");
                    var routeValue = (string)route.ConstructorArguments[0].Value;
                    var tagValue = (string)route.ConstructorArguments[1].Value;
                    var secureValue = (bool)route.ConstructorArguments[2].Value;
                    var databindValue = (DataBindEnum)route.ConstructorArguments[3].Value;
                    //get the generated code for this tag
                    if (!generatedCode.ContainsKey(tagValue))
                    {
                        generatedCode.Add(tagValue,InitStringBuilder(tagValue,projectNamespace));
                    }
                    var generatedCodeForTag = generatedCode[tagValue];
                    //Create the app.MapGet(path, async (IMediator mediator, [AsParameters]TRequest request) => await mediator.Send(request));
                    generatedCodeForTag.AppendLine($"            app.Map{method}(\"{routeValue}\", async (IMediator mediator, {GetTypeFromDataBind(databindValue)}{ns}.{name} request) => await mediator.Send(request)).WithTags(\"{tagValue}\"){(secureValue?".RequireAuthorization()":"")};");
                }
            }
            //Complete Generated string and save generated code to file
            foreach (var item in generatedCode)
            {
                var generatedString = CompleteGeneratedString(item.Value);
                context.AddSource($"{item.Key}Endpoints.g.cs",generatedString);
            }
        }

        private StringBuilder InitStringBuilder(string tag,string projectName)
        {
            var generatedCode = new StringBuilder();
            generatedCode.AppendLine("using MediatR;");
            generatedCode.AppendLine("using Microsoft.AspNetCore.Mvc;");
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

        private string GetTypeFromDataBind(DataBindEnum dataBind)
        {
            switch (dataBind)
            {
                case DataBindEnum.AsParameters:
                    return "[AsParameters]";
                case DataBindEnum.FromBody:
                    return "[FromBody]";
                case DataBindEnum.FromForm:
                    return "[FromForm]";
                default:
                    return "[AsParameters]";
            }
        }
    }
}

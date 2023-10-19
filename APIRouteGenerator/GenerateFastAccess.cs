using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace APIRouteGenerator;

[Generator]
public class GenerateFastAccess : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: (c, _) => c is ClassDeclarationSyntax && ((ClassDeclarationSyntax)c).AttributeLists.SelectMany(s => s.Attributes).Any(a => a.Name.ToString().StartsWith("FastAccess")),
            transform: (n, _) => (ClassDeclarationSyntax)n.Node
        ).Where(w => w is not null);

        var compilation = context.CompilationProvider.Combine(provider.Collect());

        context.RegisterSourceOutput(compilation,
            (spc, source) => Execute(spc, source.Left, source.Right)
        );
    }

    private void Execute(SourceProductionContext context, Compilation left, ImmutableArray<ClassDeclarationSyntax> classList)
    {
        var projectNamespace = left.AssemblyName;
        var classesWithAttribute = classList
            
            .Where(x => x.AttributeLists.SelectMany(s => s.Attributes).Any(a => a.Name.ToString().StartsWith("FastAccess"))).ToList();

        Dictionary<string, StringBuilder> generatedCode = new Dictionary<string, StringBuilder>();
        foreach (var c in classesWithAttribute)
        {
            var name = c.Identifier.ToString();
            var ns = c.Ancestors().OfType<NamespaceDeclarationSyntax>().FirstOrDefault()?.Name.ToString()
                     ?? c.Ancestors().OfType<FileScopedNamespaceDeclarationSyntax>().FirstOrDefault()?.Name.ToString();
            if (!generatedCode.ContainsKey(name))
            {
                generatedCode.Add(name, InitStringBuilder(name, ns));
            }
            var generatedCodeForClass = generatedCode[name];
            
            var properties = c.Members.OfType<PropertyDeclarationSyntax>().ToList();

            foreach (var p in properties)
            {
                generatedCodeForClass.AppendLine($"        \"{p.Identifier.ToString()}\"=> {p.Identifier.ToString()},");
            }
            generatedCodeForClass.AppendLine("       _ => throw new NotImplementedException()");
        }
        foreach (var item in generatedCode)
        {
            var generatedString = CompleteGeneratedString(item.Value);
            context.AddSource($"{item.Key}.g.cs", generatedString);
        }
    }
    private StringBuilder InitStringBuilder(string tag, string projectName)
    {
        var generatedCode = new StringBuilder();
        generatedCode.AppendLine($"namespace {projectName}");
        generatedCode.AppendLine("{");
        generatedCode.AppendLine($"    public partial class {tag}");
        generatedCode.AppendLine("    {");
        generatedCode.AppendLine("       public object? GetValue(string property) => property switch {");
        return generatedCode;
    }

    private string CompleteGeneratedString(StringBuilder generatedCode)
    {
        generatedCode.AppendLine("        };");
        generatedCode.AppendLine("    }");
        generatedCode.AppendLine("}");
        return generatedCode.ToString();
    }

}
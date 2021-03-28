namespace CodeGenerate {
  using Microsoft.CodeAnalysis;
  using Microsoft.CodeAnalysis.CSharp.Syntax;
  using Microsoft.CodeAnalysis.Text;
  

  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Text;


  [Generator]
  public class Gen : ISourceGenerator {
    public void Initialize(GeneratorInitializationContext context) {

    }

    public void Execute(GeneratorExecutionContext context) {
      var Txts = context.AdditionalFiles.First(E => E.Path.EndsWith(".txt"))
        .GetText(context.CancellationToken).ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
        .Select(E => $@"Console.WriteLine(""From {E}"");");
      var Code =
        $@"
using System;
namespace CodeGenerateStarter{{
  public static class SampleCode{{
    public static void ReadTxt(){{
      Console.WriteLine(""StartTxt"");
      {string.Join(Environment.NewLine, Txts)}
    }}
  }}
}}
";
      context.AddSource("SampleCode1", SourceText.From(Code, new UTF8Encoding(false)));
    }
  }

  [Generator]
  public class AutoRegisterSourceGenerator : ISourceGenerator {
    private const string AttributeText = @"
using System;
namespace CodeGenerateStarter
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class AutoRegisterAttribute : Attribute
    {
        public AutoRegisterAttribute()
        {
        }
    }
}";
    public void Initialize(GeneratorInitializationContext context) {
      context.RegisterForSyntaxNotifications(() => new MySyntaxReceiver());
    }
    public void Execute(GeneratorExecutionContext context) {
      context.AddSource("AutoRegisterAttribute", SourceText.From(AttributeText, Encoding.UTF8));
    }
  }
  public class MySyntaxReceiver : ISyntaxReceiver {
    public List<ClassDeclarationSyntax> CandidateClasses { get; } = new List<ClassDeclarationSyntax>();
    
    /// <summary>
    /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
    /// </summary>
    public void OnVisitSyntaxNode(SyntaxNode syntaxNode) {
      // any method with at least one attribute is a candidate for property generation
      if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax
          && classDeclarationSyntax.AttributeLists.Count >= 0) {
        CandidateClasses.Add(classDeclarationSyntax);
      }
    }
  }
}

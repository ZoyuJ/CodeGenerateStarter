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
  public class AutoRegister2DI : ISourceGenerator {
    const string AttrCode = @"
namespace CodeGed{
  using System;
  [AttributeUsage(AttributeTargets.Class,Inherited=false,AllowMultiple=false)]
  public class AutoRegisterAttribute:Attribute{}
}
";

    public void Initialize(GeneratorInitializationContext context) {
      context.RegisterForSyntaxNotifications(() => new RegisterReceiver());
    }

    public void Execute(GeneratorExecutionContext context) {
      //if (!Debugger.IsAttached) Debugger.Launch();
      context.AddSource("g2", SourceText.From(AttrCode, new UTF8Encoding(false)));
      if (context.SyntaxReceiver is RegisterReceiver Receiver) {
        CSharpParseOptions Options = (context.Compilation as CSharpCompilation).SyntaxTrees[0].Options as CSharpParseOptions;
        var AttributeTree = CSharpSyntaxTree.ParseText(SourceText.From(AttrCode, new UTF8Encoding(false)), Options);
        var Compilation = context.Compilation.AddSyntaxTrees(AttributeTree);
        var Symbol = Compilation.GetTypeByMetadataName("CodeGed.AutoRegisterAttribute");
        List<string> RegCalls = new List<string>();
        foreach (var C in Receiver.Classes) {
          SemanticModel Model = Compilation.GetSemanticModel(C.SyntaxTree);
          if (Model.GetDeclaredSymbol(C) is ITypeSymbol TSymbol
            && TSymbol.GetAttributes()
            .Any(E => E.AttributeClass.Equals(Symbol, SymbolEqualityComparer.Default))) {
            var Name = C.GetFullDefined() + "." + C.Identifier.Text;
            RegCalls.Add($"Services.AddSingleton<{Name}>();");
          }
        }
        var Full = $@"
          namespace CodeGed{{
            using System;
            using Microsoft.Extensions.DependencyInjection;
            public static class AutoRegKits{{
              public static void GeneratedRegister(this IServiceCollection Services){{
                {string.Join(Environment.NewLine, RegCalls)}
              }}
            }}
          }}
        ";
        context.AddSource("g3", SourceText.From(Full, new UTF8Encoding(false)));
      }
    }

    public class RegisterReceiver : ISyntaxReceiver {


      public List<ClassDeclarationSyntax> Classes { get; } = new List<ClassDeclarationSyntax>();
      public void OnVisitSyntaxNode(SyntaxNode syntaxNode) {
        if (syntaxNode is ClassDeclarationSyntax CDS && CDS.AttributeLists.Count > 0) {
          Classes.Add(CDS);
        }
      }
    }
  }

  public static class SyntaxKits {
    public static bool TryGetParentSyntax<T>(this SyntaxNode Node, out T Parent) where T : SyntaxNode {
      Parent = null;
      if (Node == null) {
        return false;
      }
      try {
        Node = Node.Parent;
        if (Node == null) {
          return false;
        }
        if (Node.GetType() == typeof(T)) {
          Parent = Node as T;
          return true;
        }
        return TryGetParentSyntax<T>(Node, out Parent);
      }
      catch {
        return false;
      }
    }
    public static string GetFullDefined(this SyntaxNode Node) {
      var PNode = Node;
      string Res = "";
      while (PNode != null) {
        if (PNode is NamespaceDeclarationSyntax ND) {
          Res += ".";
          Res = Res + ND.Name.ToString();
        }
        PNode = PNode.Parent;
      }
      return Res.Trim('.');
    }
  }

}

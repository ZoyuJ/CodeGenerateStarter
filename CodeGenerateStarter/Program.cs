namespace CodeGenerateStarter {
  using System;

  using Microsoft.Extensions.DependencyInjection;
  using CodeGed;
  using N1;
  using N1.N2;
   
  class Program {
    static void Main(string[] args) {
      Console.WriteLine("Hello World!");

      SampleCode.ReadTxt();

      Console.WriteLine("Done");
      Console.ReadKey();

    }
  }
   
     [CodeGed.AutoRegister]
  public class Auto1 { 
    public void Call() {
      Console.WriteLine("From Auto Registered Class1");
    }
  }


}

namespace N1 {
  using System;
  [CodeGed.AutoRegister]
  public class Auto2 {
    public void Call() {
      Console.WriteLine("From Auto Registered Class2");
    }
  }
  namespace N2 {
    using System;
    [CodeGed.AutoRegister]
    public class Auto3 {
      public void Call() {
        Console.WriteLine("From Auto Registered Class3");
      }
    }
  }
}

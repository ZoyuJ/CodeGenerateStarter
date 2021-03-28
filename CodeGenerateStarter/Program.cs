namespace CodeGenerateStarter {
  using System;

  class Program {
    static void Main(string[] args) {
      Console.WriteLine("Hello World!");

      SampleCode.ReadTxt();

      Console.WriteLine("Done");
      Console.ReadKey();

    }
  }


  [AutoRegister]
  public class OrderService {
    public OrderService() {
      Console.WriteLine($"{this.GetType()} constructed.");
    }
  }

  [AutoRegister]
  public class ProductService {
    public ProductService() {
      Console.WriteLine($"{this.GetType()} constructed.");
    }

  }
}

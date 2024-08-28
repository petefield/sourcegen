using pfie.OpenApi.Parser;
using pfie.utilities;
using System.Data;
using System.Reflection.Emit;


namespace pfie.animalfacts.console;

internal partial class App : IApp
{
    public App()
    {
    }

    public async Task Run()
    {
        Console.WriteLine("Press any key to run");
        Console.ReadKey();


        var serviceDef = await OpenApiParser.Parse("animals", "http://localhost:7070/swagger/v1/swagger.json");
        var code = pfie.http.sourcegen.CSharpMockBuilder.Build(serviceDef);

        Console.WriteLine(code);
    }
}

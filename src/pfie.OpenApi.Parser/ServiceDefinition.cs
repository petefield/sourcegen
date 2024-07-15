namespace pfie.OpenApi.Parser;


public class ServiceDefinition
{

    public ServiceDefinition(string name)
    {

        Schemas = new List<SchemaDef>();
        Operations = new List<OperationDef>();
        Name = name;
    }

    public string Name { get; set; }

    public List<SchemaDef> Schemas { get; set; }


    public List<OperationDef> Operations { get; set; }
}

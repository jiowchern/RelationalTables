namespace Regulus.RelationalTables.Serialization
{
    public interface ITypeProviable
    {
        object CreateInstance(string type_full_name);
        System.Type GetTypeByFullName(string type_full_name);
    }
}


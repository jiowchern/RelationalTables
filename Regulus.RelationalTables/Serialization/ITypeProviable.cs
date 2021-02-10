namespace Regulus.RelationalTables.Serialization
{
    
    public interface ITypeProviable
    {
        /// <summary>
        /// Create an instance by full name.
        /// </summary>
        /// <param name="type_full_name"></param>
        /// <returns>instance</returns>
        object CreateInstance(string type_full_name);
        /// <summary>
        /// Get type by full name.
        /// </summary>
        /// <param name="type_full_name"></param>
        /// <returns>Type</returns>
        System.Type GetTypeByFullName(string type_full_name);
    }
}


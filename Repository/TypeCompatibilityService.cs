namespace Repository
{
    public static class TypeCompatibilityService
    {
        public static List<string> GetCompatibleTypes(string sqlType)
        {
            return sqlType switch
            {
                "TINYINT" => new() { "TINYINT", "SMALLINT", "INT", "BIGINT" },
                "SMALLINT" => new() { "SMALLINT", "INT", "BIGINT" },
                "INT" => new() { "INT", "BIGINT", "DECIMAL(19,4)" },
                "BIGINT" => new() { "BIGINT", "DECIMAL(19,4)" },
                "DECIMAL(18,4)" => new() { "DECIMAL(18,4)", "DECIMAL(19,4)" },
                "REAL" => new() { "REAL", "FLOAT", "DECIMAL(19,4)" },
                "FLOAT" => new() { "FLOAT", "DECIMAL(19,4)" },
                "NVARCHAR(255)" => new() { "NVARCHAR(255)", "TEXT" },
                "VARCHAR(255)" => new() { "VARCHAR(255)", "TEXT" },
                "TEXT" => new() { "TEXT" },
                "DATETIME" => new() { "DATETIME", "DATE", "TIME" },
                "VARBINARY" => new() { "VARBINARY", "VARBINARY(MAX)" },
                "BIT" => new() { "BIT" },
                _ => new() { sqlType, "UNKNOWN" }
            };
        }
    }
}

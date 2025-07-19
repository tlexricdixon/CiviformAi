using Entities;
using Microsoft.AspNetCore.Http;

namespace Contracts
{
    public interface IAccessSchemaReader<T>
    {
        public List<T> GetAccessSchema(string accdbFilePath);
    }
}

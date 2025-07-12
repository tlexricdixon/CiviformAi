using Contracts;
using Interfaces;
using System.Data;

namespace Repository;
    public class AccessDbService : IAccessDbService
    {
        public async Task SaveTableAsync(string tableName, DataTable tableData)
        {
            // OPTIONAL: Standardize names, strip spaces
            tableName = tableName.Replace(" ", "_");

            // If you want to store generically (No EF models), use ADO.NET:
            // CREATE TABLE if not exists
            // INSERT rows one by one or bulk copy

            //// If you want to map to EF models, do conditional mappings:
            //if (tableName == "Products")
            //    SaveToProductsModel(tableData);
            //else if (tableName == "Customers")
            //    SaveToCustomersModel(tableData);
            //else
            //    SaveToGenericJsonStore(tableData);
        }

    }

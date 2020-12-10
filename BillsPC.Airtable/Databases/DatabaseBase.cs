using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirtableApiClient;

namespace BillsPC.Airtable.Databases
{
    /// <summary>
    /// This is the DatabaseBase, which is the best structure for all of the databases.
    /// All of the methods that return data, are generic types.
    /// So we can use this as a proper base, and get data that is associated with the class.
    /// </summary>
    public class DatabaseBase
    {
        private AirtableBase Base { get; set; } // Airtable base.
        public DatabaseBase(string apiKey, string baseId)
        {
            Base = new AirtableBase(apiKey, baseId);
        }
        /// <summary>
        /// Gets the data from a base, and puts it into a callback function.
        /// </summary>
        /// <param name="tableName">The table's name that we will be accessing.</param>
        /// <param name="callback">the callback function, which will hold our model, which will allow us to get the data that we want.</param>
        /// <typeparam name="T">The Model type</typeparam>
        /// <returns>The data in the Model type</returns>
        public async Task<T> GetData<T>(string tableName, Func<List<AirtableRecord>, T, T> callback) where T : new()
        {
            T model = new T();
            var list = await Base.ListRecords(tableName);
            return callback(list.Records.ToList(), model);
        }
        /// <summary>
        /// Gets the data from a base, and puts it int a callback function.
        /// </summary>
        /// <param name="tableName">The table's name that we will be accessing.</param>
        /// <param name="filter">the airtable's filter, which will allow us to get the user's data.</param>
        /// <param name="callback">the callback function, which will hold our model, which will allow us to get the data that we want.</param>
        /// <typeparam name="T">The Model type</typeparam>
        /// <returns>The data in the Model type</returns>
        public async Task<T> GetData<T>(string tableName, string filter, Func<List<AirtableRecord>, T, T> callback) where T : new()
        {
            T model = new T();
            var list = await Base.ListRecords(tableName, filterByFormula: filter);
            return callback(list.Records.ToList(), model);
        }
        /// <summary>
        /// Inserts data into the record.
        /// </summary>
        /// <param name="tableName">The table's name that we will be accessing.</param>
        /// <param name="filter">the airtable's filter, which will allow us to get the user that we will be inserting data into.</param>
        /// <param name="callback">the callback function, which will hold the fields object, which will allow us to add what we want to insert.</param>
        /// <returns>Returns nothing, because async Task results to void.</returns>
        public async Task InsertData(string tableName, string filter, Func<Fields, Task<Fields>> callback)
        {
            var fields = new Fields();
            var values = await callback(fields);

            await Base.UpdateRecord(tableName, values, await GetRecordId(tableName, filter));
        }
        /// <summary>
        /// Gets the record's id of the user.
        /// </summary>
        /// <param name="tableName">The table's name that we will be accessing.</param>
        /// <param name="filter">the airtable's filter, which will allow us to get the user's record id.</param>
        /// <returns>The record id of the user's record.</returns>
        public async Task<string> GetRecordId(string tableName, string filter)
        {
            var result = await Base.ListRecords(tableName, filterByFormula: filter);
            return result.Records.FirstOrDefault().Id;
        }
        /// <summary>
        /// Creates data for a user.
        /// </summary>
        /// <param name="tableName">The table's name that we will be accessing.</param>
        /// <param name="callback">the callback function, which will allow us to put in data that will be used when creating the record.</param>
        /// <returns>Return nothing, because async Task results to void.</returns>
        public async Task CreateData(string tableName, Func<Fields, Fields> callback)
        {
            var fields = new Fields();
            var values = callback(fields);

            await Base.CreateRecord(tableName, values);
        }
    }
}
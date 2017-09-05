using System;
using System.Configuration;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace TimeTrigger
{
    public static class DeleteAllMessages
    {
        [FunctionName("DeleteAllMessages")]
        public static void Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");

            var connectionString = ConfigurationManager.AppSettings["AzureWebJobsStorage"];

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient client = storageAccount.CreateCloudTableClient();
            CloudTable table = client.GetTableReference("slackMessages");

            TableQuery<DynamicTableEntity> query =
                new TableQuery<DynamicTableEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey",
                    QueryComparisons.Equal, "Messages"));

            foreach (var dynamicTableEntity in table.ExecuteQuery(query))
            {
                TableOperation deleteOperation = TableOperation.Delete(dynamicTableEntity);
                table.Execute(deleteOperation);
                log.Info($"Entity with message '{dynamicTableEntity.Properties["Message"].StringValue}' was deleted");
            }
        }
    }
}

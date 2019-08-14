using System;
using System.Threading.Tasks;
//using WebManageAPI.Function;
using MongoDB.Driver;
using MongoDB.Bson;
using NSOWater.HotMigration.HotModels;
using Microsoft.WindowsAzure.Storage;

namespace NsoGetData
{
    class Program
    {
        static async Task Main(string[] args)
        {
            int count = 0;
            //Nso Real
            //var client = new MongoClient("mongodb://dbagent:Nso4Passw0rd5@mongodbproykgte5e7lvm7y-vm0.southeastasia.cloudapp.azure.com/nso");
            //var database = client.GetDatabase("nso");
            // Nso Dev
            var client = new MongoClient("mongodb://thesdev:Th35Passw0rd5@thes-dev-db.onmana.app/nso2");
            var database = client.GetDatabase("nso2");
            var collection = database.GetCollection<SurveyData>("survey");

            var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=nsosurvey;AccountKey=O8rkX9HlkBC6VDh5XbMFcec3ndofVuh21yCkLdODZXX6d4FeWJ0H20SeFtxqDw9Ii66mWCxRsYLJkVYVsLrckw==;EndpointSuffix=core.windows.net");
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("surveys");

            Console.WriteLine("database : " + database);
            Console.WriteLine("collection : " + collection);

            Console.WriteLine("Please Wait........");

            var uploadLogs = collection.Aggregate()
                .Match(x => x.Province == "10")
                .ToList();

            //var uploadLogs = await collection.Find(x => x.UserId == "10506941").ToListAsync();
            Console.WriteLine("Total data = " + uploadLogs.Count);
            string[] a = new string[uploadLogs.Count];
            foreach (var log in uploadLogs)
            {
                count++;
                Console.Write(count + " : ");
                Console.Write(log.BlobName);
                Console.Write("  ");
                Console.WriteLine(log.ContainerName);
            }
            Console.WriteLine("Total data = " + uploadLogs.Count);
            Console.Read();
        }
    }
}

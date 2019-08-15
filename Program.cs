using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
//using WebManageAPI.Function;
using MongoDB.Driver;
using NSOWater.HotMigration.HotModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;


namespace NsoGetData
{
    class Program
    {
        static async Task Main(string[] args)
        {
            int count = 0;
            string data = "10";
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
            
            BlobContinuationToken blobContinuationToken = null;

            Console.WriteLine("database : " + database);
            Console.WriteLine("collection : " + collection);

            Console.WriteLine("Please Wait........");

            var uploadLogs = collection.Aggregate()
                .Match(x => x.Province == data)
                .ToList();
            string path = @"d:\\demotext.txt";
            //var uploadLogs = await collection.Find(x => x.UserId == "10506941").ToListAsync();
            Console.WriteLine("Total data = " + uploadLogs.Count);
            string[] a = new string[uploadLogs.Count];
            foreach (var log in uploadLogs)
            {
                count++;
                Console.Write(count + " : " + log.CreationDateTime);
                Console.Write(log.BlobName);
                Console.Write("  ");
                Console.WriteLine(log.ContainerName);
                using (StreamWriter writer = new StreamWriter(path, true))
                {
                    writer.Write(log.CreationDateTime + "  ");
                    writer.Write(log.BlobName + " ");
                    writer.WriteLine(log.ContainerName);
                }
            }
            Console.WriteLine("Total data = " + uploadLogs.Count);
            Console.Read();
        }
    }
}

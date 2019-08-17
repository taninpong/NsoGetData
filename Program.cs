using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using MongoDB.Bson;
//using WebManageAPI.Function;
using MongoDB.Driver;
using Newtonsoft.Json;
using NSOWater.HotMigration.HotModels;
using NSOWater.HotMigration.Models;
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
            Console.Title = "Blob To Json V 0.0.1";
            var now = DateTimeOffset.UtcNow;
            int count = 0;
            string data = "10";
            var itemBlob_Building = new List<CloudBlockBlob>();
            var itemBlob_House = new List<CloudBlockBlob>();
            var itemBlob_Community = new List<CloudBlockBlob>();

            //Nso Real
            var client = new MongoClient("mongodb://dbagent:Nso4Passw0rd5@mongodbproykgte5e7lvm7y-vm0.southeastasia.cloudapp.azure.com/nso");
            var database = client.GetDatabase("nso");
            // Nso Dev
            //var client = new MongoClient("mongodb://thesdev:Th35Passw0rd5@thes-dev-db.onmana.app/nso2");
            //var database = client.GetDatabase("nso2");
            var collection = database.GetCollection<SurveyData>("survey");

            var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=nsosurvey;AccountKey=O8rkX9HlkBC6VDh5XbMFcec3ndofVuh21yCkLdODZXX6d4FeWJ0H20SeFtxqDw9Ii66mWCxRsYLJkVYVsLrckw==;EndpointSuffix=core.windows.net");
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("surveys");

            //BlobContinuationToken blobContinuationToken = null;
            Console.WriteLine("Blob To Json Ver. 0.0.1");
            Console.WriteLine($"Province : { data }");
            Console.WriteLine($"database : { database }");
            Console.WriteLine($"collection : { collection }");

            Console.WriteLine("Please Wait........");

            var uploadLogs = collection.Aggregate()
                .Match(x => x.Province == data)
                .Match(y => y.Enlisted == true)
                .ToList();
            string pathbuilding = @"d:\\Nsodata_building.txt";
            string pathhouseHold = @"d:\\Nsodata_houseHold.txt";
            string pathCommunity = @"d:\\Nsodata_Community.txt";

            Console.WriteLine($"Total data = { uploadLogs.Count }");
            foreach (var log in uploadLogs)
            {

                var directory = container.GetDirectoryReference(log.Province).GetDirectoryReference(log.SrcUserId).GetDirectoryReference(log.ContainerName);
                var blob = directory.GetBlockBlobReference(log.BlobName);
                if( log.BlobName.StartsWith("bld"))
                    {
                    itemBlob_Building.Add(blob);
                }else if (log.BlobName.StartsWith("unt"))
                {
                    itemBlob_House.Add(blob);
                }else if (log.BlobName.StartsWith("comm"))
                {
                    itemBlob_Community.Add(blob);
                }
                

            }
            var created = await container.CreateIfNotExistsAsync();
            var containerName = container.Name;
            var sas = container.GetSharedAccessSignature(new SharedAccessBlobPolicy()
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessStartTime = now.AddMinutes(-5),
                SharedAccessExpiryTime = now.AddMinutes(120),
            });

            //เขียน list ลง text
            Console.WriteLine("building..");
            using (StreamWriter writer = new StreamWriter(pathbuilding, true))
            {
                writer.Write("[");

                foreach (var item in itemBlob_Building)
                {
                    count++;
                    var building = ReadModelBlob<BuildingSample>(item).GetAwaiter().GetResult();
                    DateTime Now = DateTime.Now;
                    Console.WriteLine($"[{ Now }] {count} : Success !!");
                    writer.Write(building.ToJson());
                    writer.WriteLine(",");
                }
                writer.Write("]");

            }

            Console.WriteLine("houseHold..");
            using (StreamWriter writer = new StreamWriter(pathhouseHold, true))
            {
                writer.Write("[");

                foreach (var blobHH in itemBlob_House)
                {
                    count++;
                    var houseHold = ReadModelBlob<HouseHoldSample>(blobHH).GetAwaiter().GetResult();
                    DateTime Now = DateTime.Now;
                    Console.WriteLine($"[{ Now }] {count} : Success !!");
                    writer.Write(houseHold.ToJson());
                    writer.WriteLine(",");
                }
                writer.Write("]");
            }

            Console.WriteLine("Community..");
            using (StreamWriter writer = new StreamWriter(pathCommunity, true))
            {
                writer.Write("[");

                foreach (var blobHH in itemBlob_Community)
                {
                    count++;
                    var comm = ReadModelBlob<CommunitySample>(blobHH).GetAwaiter().GetResult();
                    DateTime Now = DateTime.Now;
                    Console.WriteLine($"[{ Now }] {count} : Success !!");
                    writer.Write(comm.ToJson());
                    writer.WriteLine(",");
                }
                writer.Write("]");
            }

            Console.WriteLine($"Total data = { uploadLogs.Count }");
            Console.Read();
        }

        private static async Task<T> ReadModelBlob<T>(CloudBlockBlob blob)
        {
            var blobContent = await blob.DownloadTextAsync();
            var model = JsonConvert.DeserializeObject<T>(blobContent);
            return model;
        }
    }



}

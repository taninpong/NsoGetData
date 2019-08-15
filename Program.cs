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
            var now = DateTimeOffset.UtcNow;
            int count = 0;
            string data = "10506941";
            var itemBlob = new List<CloudBlockBlob>();
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

            //BlobContinuationToken blobContinuationToken = null;

            Console.WriteLine("database : " + database);
            Console.WriteLine("collection : " + collection);

            Console.WriteLine("Please Wait........");

            var uploadLogs = collection.Aggregate()
                .Match(x => x.UserId == data)
                .ToList();
            string pathbuilding = @"d:\\Nsodata_building.txt";
            string pathhouseHold = @"d:\\Nsodata_houseHold.txt";
            string pathCommunity = @"d:\\Nsodata_Community.txt";

            //var uploadLogs = await collection.Find(x => x.UserId == "10506941").ToListAsync();
            Console.WriteLine("Total data = " + uploadLogs.Count);
            foreach (var log in uploadLogs)
            {
                //Console.Write(count + " : " + log.CreationDateTime);
                var directory = container.GetDirectoryReference(log.Province).GetDirectoryReference(log.SrcUserId).GetDirectoryReference(log.ContainerName);
                var blob = directory.GetBlockBlobReference(log.BlobName);
                itemBlob.Add(blob);

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

                foreach (var item in itemBlob)
                {
                    count++;
                    var building = ReadModelBlob<BuildingSample>(item).GetAwaiter().GetResult();
                    Console.WriteLine($"{ count }Success !!");
                    //var url = ($"province / SrcUserId / ContainerName / BuildingId");
                    writer.Write(building.ToJson());
                    writer.WriteLine(",");
                }
                writer.Write("]");

            }

            Console.WriteLine("houseHold..");
            using (StreamWriter writer = new StreamWriter(pathhouseHold, true))
            {
                writer.Write("[");

                foreach (var blobHH in itemBlob)
                {
                    count++;
                    var houseHold = ReadModelBlob<HouseHoldSample>(blobHH).GetAwaiter().GetResult();
                    Console.WriteLine($"{ count }Success !!");
                    //var url = ($"province / SrcUserId / ContainerName / BuildingId");
                    writer.Write(houseHold.ToJson());
                    writer.WriteLine(",");
                }
                writer.Write("]");
            }

            Console.WriteLine("Community..");
            using (StreamWriter writer = new StreamWriter(pathCommunity, true))
            {
                writer.Write("[");

                foreach (var blobHH in itemBlob)
                {
                    count++;
                    var comm = ReadModelBlob<CommunitySample>(blobHH).GetAwaiter().GetResult();
                    Console.WriteLine($"{ count } Success !!");
                    //var url = ($"province / SrcUserId / ContainerName / BuildingId");
                    writer.Write(comm.ToJson());
                    writer.WriteLine(",");
                }
                writer.Write("]");
            }

            Console.WriteLine("Total data = " + uploadLogs.Count);
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

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using MongoDB.Bson;
//using WebManageAPI.Function;
using MongoDB.Driver;
using Newtonsoft.Json;
using NsoGetData.Models;
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
        static void Main(string[] args)
        {
            Console.Title = "Blob To Json V 0.0.1";
            var now = DateTimeOffset.UtcNow;
            int count = 0;
            string data = "43801011099001";
            var itemBlob_Building = new List<CloudBlockBlob>();
            var itemBlob_House = new List<CloudBlockBlob>();
            var blddataAll = new List<BldData2>();
            var blddata = new List<BldData>();
            var untdata = new List<UntData>();
            var untdata2 = new UntData();
            var sumbldunt = new List<SumBldUnt>();
            //var itemBlob_Community = new List<CloudBlockBlob>();

            //Nso Real
            var client = new MongoClient("mongodb://dbagent:Nso4Passw0rd5@mongodbproykgte5e7lvm7y-vm0.southeastasia.cloudapp.azure.com/nso");
            var database = client.GetDatabase("nso");
            // Nso Dev
            //var client = new MongoClient("mongodb://thesdev:Th35Passw0rd5@thes-dev-db.onmana.app/nso2");
            //var database = client.GetDatabase("nso2");
            var collection = database.GetCollection<SurveyData>("survey");

            var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=nsostorage;AccountKey=Lzw/JGZTvtHoRxo8GKjWQy5rm3vprPahD4YsoRXhi7Ai+Gyg34tq+Y+HrEuNf5SOAib1jNNavkFVghaKk/488w==;EndpointSuffix=core.windows.net");
            var blobClient = storageAccount.CreateCloudBlobClient();
            //var container = blobClient.GetContainerReference("surveys");

            //BlobContinuationToken blobContinuationToken = null;
            Console.WriteLine("Blob To Json Ver. 0.0.1");
            Console.WriteLine($"Province : { data }");
            Console.WriteLine($"database : { database }");
            Console.WriteLine($"collection : { collection }");

            Console.WriteLine("Please Wait........");

            var uploadLogs = collection.Aggregate()
                .Match(x => x.EA == data)
                .Match(y => y.Enlisted == true)
                .ToList();
            string pathbuilding = @"d:\\Nsodata_building.txt";
            string pathhouseHold = @"d:\\Nsodata_houseHold.txt";
            string pathCommunity = @"d:\\Nsodata_Community.txt";

            Console.WriteLine($"Total data = { uploadLogs.Count }");
            foreach (var log in uploadLogs)
            {
                var directory = blobClient.GetContainerReference(log.ContainerName);
                //var directory = container.GetDirectoryReference(log.Province).GetDirectoryReference(log.SrcUserId).GetDirectoryReference(log.ContainerName);
                var blob = directory.GetBlockBlobReference(log.BlobName);
                if (log.BlobName.StartsWith("bld"))
                {
                    itemBlob_Building.Add(blob);
                }
                else if (log.BlobName.StartsWith("unt"))
                {
                    itemBlob_House.Add(blob);
                }
                var created = directory.CreateIfNotExistsAsync();
                var containerName = directory.Name;
                var sas = directory.GetSharedAccessSignature(new SharedAccessBlobPolicy()
                {
                    Permissions = SharedAccessBlobPermissions.Read,
                    SharedAccessStartTime = now.AddMinutes(-5),
                    SharedAccessExpiryTime = now.AddMinutes(120),
                });
            }
            ////เขียน list ลง text
            Console.WriteLine("building..");
            using (StreamWriter writer = new StreamWriter(pathbuilding, true))
            {
                //writer.Write("[");

                foreach (var item in itemBlob_Building)
                {
                    count++;
                    var building = ReadModelBlob<BuildingSample>(item).GetAwaiter().GetResult();
                    blddataAll.Add(new BldData2()
                    {
                        _id = building._id,
                        EA = building.EA,
                        Ordering = building.Ordering,
                        Road = building.Road,
                        Alley = building.Alley,
                        Name = building.Name,
                        HouseNo = building.HouseNo,
                        Latitude = building.Latitude,
                        Longitude = building.Longitude,
                        BuildingType = building.BuildingType,
                        Other = building.Other,
                        Accesses = building.Accesses,
                        VacancyCount = building.VacancyCount,
                        AbandonedCount = building.AbandonedCount,
                        UnitCount = building.UnitCount,
                        UnitAccess = building.UnitAccess,
                        VacantRoomCount = building.VacantRoomCount,
                        OccupiedRoomCount = building.OccupiedRoomCount,

                    });
                    DateTime Now = DateTime.Now;
                    Console.WriteLine($"[{ Now }] {count}{"/"}{itemBlob_Building.Count} : Success !!");
                }
                writer.Write(blddataAll.ToJson());
            }

            Console.WriteLine("houseHold..");
            using (StreamWriter writer = new StreamWriter(pathhouseHold, true))
            {
                foreach (var blobHH in itemBlob_House)
                {
                    var count2 = 1;
                    count2++;
                    var houseHold = ReadModelBlob<HouseHoldSample>(blobHH).GetAwaiter().GetResult();
                    if (houseHold.Population.Persons != null)
                    {
                        foreach (var item in houseHold.Population.Persons)
                        {
                            var persondata = new Poppulation();
                            persondata.FirstName = item.FirstName;
                            persondata.LastName = item.LastName;
                            persondata.NameTitle = item.NameTitle;
                            persondata.Relationship = item.Relationship;
                            persondata.Sex = item.Sex;
                            persondata.OtherTitle = item.OtherTitle;
                            persondata.BirthDate = item.BirthDate;
                            persondata.BirthMonth = item.BirthMonth;
                            persondata.BirthYear = item.BirthYear;
                            persondata.Age = item.Age;
                            persondata.Nationality = item.Nationality;
                            persondata.Registration = item.Registration;
                            persondata.OtherProvince = item.OtherProvince;
                            untdata.Add(new UntData
                            {
                                BuildingId = houseHold.BuildingId,
                                Ea = houseHold.EA,
                                PoppulationData = persondata
                            });
                            DateTime Now = DateTime.Now;
                            Console.WriteLine($"[{ Now }] {count2}{"/"}{itemBlob_House.Count} : Success !!");
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            Console.WriteLine(untdata.Count);

            Console.WriteLine("SunBldUnt..");
            using (StreamWriter writer = new StreamWriter(pathCommunity, true))
            {
                var count2 = 1;
                for (int i = 0; i < blddataAll.Count; i++)
                {
                    for (int j = 0; j < untdata.Count; j++)
                    {
                        if (blddataAll[i]._id == untdata[j].BuildingId)
                        {
                            sumbldunt.Add(new SumBldUnt()
                            {
                                BldData = new BldData
                                {
                                    EA = blddataAll[i].EA,
                                    Road = blddataAll[i].Road,
                                    Alley = blddataAll[i].Alley,
                                    BuildingType = blddataAll[i].BuildingType,
                                    HouseNo = blddataAll[i].HouseNo,
                                    Name = blddataAll[i].Name,
                                    Ordering = blddataAll[i].Ordering,
                                    _id = blddataAll[i]._id
                                },
                                UntData = untdata[j]
                            });
                            DateTime Now = DateTime.Now;
                            Console.WriteLine($"[{ Now }] {count2}{"/"}{blddataAll.Count} : Success !!");
                            count2++;
                        }
                    }
                }
                writer.Write(sumbldunt.ToJson());
            }
        }

        private static async Task<T> ReadModelBlob<T>(CloudBlockBlob blob)
        {
            var blobContent = await blob.DownloadTextAsync();
            var model = JsonConvert.DeserializeObject<T>(blobContent);
            return model;
        }


    }

}

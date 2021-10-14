using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

namespace Trips
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press any key to start");
            Console.ReadKey();
            var timer = DateTime.Now;
            Dictionary<int, string> driverlList = LoadDrivers();
            Dictionary<int, Vehicle> vehicles = LoadVehicles();
            List<VehicleDriverIdentification> vdIndentification = LoadVehicleDriver();
            List<VehicleTrip> vehTrip = LoadTrips();

            foreach (var vt in vehTrip)
            {
                String vehicle = vehicles[vt.vehicleTripId].Description.ToString();
                var vehicleId = vehicles[vt.vehicleTripId].VehicleID;
                var driver = vehTrip.Find(x => (x.vehicleId == vehicleId));
                var vehicleDriver = vdIndentification.Find(vd => vd.vehicleId == vehicleId);

                Console.WriteLine("Trip start:"+ vt.tripStart + " driver ID:" + driver.vehicleTripId.ToString() + " Driver Name:" + driverlList[vdIndentification.Find(x => x.vehicleId == vehicleId).driverId]);
            }
            Console.WriteLine("Time elapsed: " + (DateTime.Now - timer).ToString());

            Console.ReadKey();



        }

        private static List<VehicleDriverIdentification> LoadVehicleDriver()
        {
            List<VehicleDriverIdentification> vdInd = new List<VehicleDriverIdentification>();
            using (var reader = new StreamReader("./InputData/VehicleDriverIdentifications.txt"))
            {
                try
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (line == null) continue;
                        var values = Regex.Split(line, " ");
                        var timeOfIdentification = DateTime.Parse(values[0] + " " + values[1]);
                        var vehicleDriver = values[2].Replace("(", "").Replace(")", "").Split("::");
                        var vehicleId = vehicleDriver[0];
                        var driverId = vehicleDriver[1];

                        var vd = new VehicleDriverIdentification { timeOfIdentification = timeOfIdentification, driverId = short.Parse(driverId), vehicleId = short.Parse(vehicleId) };
                        vdInd.Add(vd);
                    }
                }
                catch (Exception e)
                {
                }

                return vdInd;
            }
        }

        private static List<VehicleTrip> LoadTrips()
        {
            var VehicleTrips = new List<VehicleTrip>();
            using (var reader = new BinaryReader(File.Open("./InputData/Trips.dat", FileMode.Open)))
            {

                try
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        var vehicleId = reader.ReadInt16();
                        var tripStart = reader.ReadInt64();
                        var tripStartDate = DateTime.FromBinary(tripStart);
                        var odoStart = reader.ReadSingle();
                        var tripEnd = reader.ReadInt64();
                        var tripEndDate = DateTime.FromBinary(tripEnd);
                        var odoEnd = reader.ReadSingle();
                        var distance = reader.ReadSingle();

                        var tripTime = (tripEnd - tripStart);
                        var tripSpeed = (distance / (tripTime / TimeSpan.TicksPerSecond));


                        VehicleTrip a = new VehicleTrip();
                        a.vehicleId = vehicleId;
                        a.tripStart = tripStartDate;
                        a.vehicleTripId = vehicleId;
                        a.tripTripSteed = tripSpeed;

                        VehicleTrips.Add(a);


                    }
                }
                catch (Exception e)
                {
                }
                var topTen = (from record in VehicleTrips orderby record.tripTripSteed descending select record).Take(10).ToList();
                return topTen;
            }
        }

        private static Dictionary<int, Vehicle> LoadVehicles()
        {
            Dictionary<int, Vehicle> vehicles = new Dictionary<int, Vehicle>();

            using (var reader = new StreamReader("./InputData/VehicleList.csv"))
            {
                try
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (line == null) continue;
                        var values = line.Split(',');

                        vehicles.Add(Int32.Parse(values[0]), new Vehicle { VehicleID = Int32.Parse(values[0]), Description = values[1], RegistrationNumber = values[2] });
                    }
                }
                catch (Exception e)
                {
                }
            }
            return vehicles;
        }

        private static Dictionary<int, string> LoadDrivers()
        {
            Dictionary<int, string> driverList = new Dictionary<int, string>();

            using (var reader = new StreamReader("./InputData/DriverList.csv"))
            {
                try
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (line == null) continue;
                        var values = line.Split(',');
                        driverList.Add(Int32.Parse(values[0]), values[1]);
                    }
                }
                catch (Exception e)
                {
                }
            }
            return driverList;

        }
    }
}



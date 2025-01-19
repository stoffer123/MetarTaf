// See https://aka.ms/new-console-template for more information
using Metar.Decoder;
using Metar.Decoder.Entity;
using MetarTaf_Backend;
using MetarTaf_Backend.Factories;
using MetarTaf_Backend.Models;
using MetarTaf_Backend.Services;
using System.Text;
using Taf.Decoder;
using Taf.Decoder.entity;

await AirportInfoService.createAirportInfo();

AirportController airportController = new AirportController();

airportController.getAirport("EKEB");
airportController.getAirport("EKCH");

string[] strings = airportController.getAirportIcaoList().ToArray();
foreach (string s in strings)
{
    Console.WriteLine(s);
}


Console.ReadKey();
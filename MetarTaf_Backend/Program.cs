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


MetarService metarService = new MetarService();

await metarService.fetchMetars();

//Dictionary<DateTime, MetarReport> ekebMetars = metarService.getMetars("EKEB");

//foreach (KeyValuePair<DateTime, MetarReport> kvp in ekebMetars)
//{

//    Console.WriteLine($"{kvp.Value.reportTime} --- {kvp.Value.decodedMetar.RawMetar}");
//}

metarService.printIcaoList();




Console.ReadKey();
// See https://aka.ms/new-console-template for more information
using Metar.Decoder;
using Metar.Decoder.Entity;
using MetarTaf_Backend.Factories;
using MetarTaf_Backend.Models;
using System.Text;
using Taf.Decoder;
using Taf.Decoder.entity;

MetarFactory metarFactory = new();
TafFactory tafFactory = new();

//TAF skal tilføjes foran den rå streng hvis det ikke står der i forvejen.
TafReport tafReport = tafFactory.createTafReport("TAF AMD EKCH 170327Z 1703/1803 26010KT 9999 BKN008 TEMPO 1706/1708 2000 BR BKN002 TEMPO 1708/1715 4000 BR BKN012 TEMPO 1715/1803 4000 BR BKN004=");
//METAR Tilføjes foran den rå metar streng hvis ikke det står der i forvejen
MetarReport metarReport = metarFactory.createMetar("METAR AUTO EKEB 170220Z 21010KT 1000 R08/P1500N R26/P1500N BR OVC002/// 06/06 Q1034=");

Console.WriteLine(tafReport.reportTime);
Console.WriteLine(metarReport.reportTime);

Console.ReadKey();
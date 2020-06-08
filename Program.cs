using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TelekocsiCSHARP
{
    class Autok
    {
        public string indulas { get; set; }
        public string cel { get; set; }
        public string rendszam { get; set; }
        public string telefonszam { get; set; }
        public int ferohely { get; set; }

        public Autok(string indulas, string cel, string rendszam, string telefonszam, int ferohely)
        {
            this.indulas = indulas;
            this.cel = cel;
            this.rendszam = rendszam;
            this.telefonszam = telefonszam;
            this.ferohely = ferohely;
        }
    }

    class Igenyek
    {
        public string azonosito { get; set; }
        public string indulas { get; set; }
        public string cel { get; set; }
        public int szemelyek { get; set; }

        public Igenyek(string azonosito, string indulas, string cel, int szemelyek)
        {
            this.azonosito = azonosito;
            this.indulas = indulas;
            this.cel = cel;
            this.szemelyek = szemelyek;
        }
    }

    class Program
    {
        public static List<Autok> autokLista = beolvasAutok();
        public static List<Igenyek> igenyekLista = beolvasIgenyek();

        public static List<Autok> beolvasAutok()
        {
            List<Autok> list = new List<Autok>();
            try{
                using (StreamReader sr =new StreamReader(new FileStream("autok.csv",FileMode.Open),Encoding.UTF8))
                {
                    sr.ReadLine();
                    while (!sr.EndOfStream)
                    {
                        var split = sr.ReadLine().Split(';');
                        var o = new Autok(
                                split[0],
                                split[1],
                                split[2],
                                split[3],
                                Convert.ToInt32(split[4])
                            );
                        list.Add(o);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hiba a beolvasásnál. Autók"+ex.Message);
            }
            return list;
        }

        static public List<Igenyek> beolvasIgenyek()
        {
            List<Igenyek> list = new List<Igenyek>();
            try
            {
                using (StreamReader sr = new StreamReader(new FileStream("igenyek.csv", FileMode.Open), Encoding.UTF8))
                {
                    sr.ReadLine();
                    while (!sr.EndOfStream)
                    {
                        var split = sr.ReadLine().Split(';');
                        var o = new Igenyek(
                                split[0],
                                split[1],
                                split[2],
                                Convert.ToInt32(split[3])
                            );
                        list.Add(o);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hiba a beolvasásnál. Igények" + ex.Message);
            }
            return list;
        }

        static void Main(string[] args)
        {
            #region 2. feladat
            Console.WriteLine("2. feladat: \n\t "+autokLista.Count()+" autós hirdet fuvart. ");
            #endregion

            #region 3. feladat
            var db = autokLista.Where(x => x.indulas.Equals("Budapest") && x.cel.Equals("Miskolc")).Sum(y=>y.ferohely);
            Console.WriteLine("3. feladat: \n\tÖsszesen {0} férőhelyet hirdettek az autósok Budapestről Miskocra",db);
            #endregion

            #region 4. feladat
            var utvonalak = autokLista
                .Select(x =>
                    new KeyValuePair<string, int>(x.indulas + " - " + x.cel, x.ferohely))
                .ToList();
            var legt = utvonalak
                .GroupBy(x => x.Key)
                .Select(y=>new
                {
                    key=y.Key,
                    sumFer=y.Sum(x=>x.Value)
                })
                .ToList()
                .OrderByDescending(z=>z.sumFer)
                .First();
            Console.WriteLine("4. feladat:\n\tA legtöbb férőhelyet ({0}-at) a {1} útvonalon ajánlották fel a hirdetők",legt.sumFer,legt.key);
            #endregion

            #region 5. feladat
            Console.WriteLine("5. feladat: ");
            foreach (var igeny in igenyekLista)
            {
                foreach (var jarat in autokLista)
                {
                    if (igeny.indulas.Equals(jarat.indulas) && igeny.cel.Equals(jarat.cel) && igeny.szemelyek<=jarat.ferohely)
                    {
                        Console.WriteLine($"\t{igeny.azonosito} => {jarat.rendszam}");
                    }
                }
            }
            #endregion

            #region 6. feladat
            using (StreamWriter sw=new StreamWriter(new FileStream("utasuzenetek.txt",FileMode.Create),Encoding.UTF8))
            {
                foreach (var igeny in igenyekLista)
                {
                    var talalat = false;
                    foreach (var jarat in autokLista)
                    {
                        if (igeny.indulas.Equals(jarat.indulas) && igeny.cel.Equals(jarat.cel) && igeny.szemelyek <= jarat.ferohely)
                        {
                            sw.WriteLine(igeny.azonosito+": Rendszám: "+jarat.rendszam+", Telefonszám: "+jarat.telefonszam);
                            talalat = true;
                        }
                    }
                    if (talalat==false)
                    {
                        sw.WriteLine(igeny.azonosito+": Sajnos nem sikerült autót találni");
                    }
                }
            }
            Console.WriteLine("6. feladat: utasuzenetek.txt");
            #endregion

            Console.ReadKey();
        }
    }
}

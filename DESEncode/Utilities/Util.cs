using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DESEncode.Utilities
{
    public class Util
    {

        public List<string> options = new List<string>();
        public List<string> optionsLibrary = new List<string>();
        public List<string> exceptions = new List<string>();
        public List<string> info = new List<string>();

        public Util()
        {

            options.Add("Įveskite tekstą:");
            options.Add("Įveskite raktą:");
            options.Add("[0] - Užbaigti programą");
            options.Add("[1] - Pradėti DES algoritmo šifravimą");
            options.Add("[2] - Pradėti DES algoritmo iššifravimą");
            options.Add("[3] - Naudoti bibliotekas");
            options.Add("Įveskite bitus:");
            options.Add("Įveskite '-' norėdami nutraukti įvedimą:");
            optionsLibrary.Add("[9] - Išeiti iš meniu");
            optionsLibrary.Add("[1] - CBC šifravimas");
            optionsLibrary.Add("[2] - CBC iššifravimas");
            optionsLibrary.Add("[3] - CFB šifravimas");
            optionsLibrary.Add("[4] - CFB iššifravimas");

            exceptions.Add("Klaida. Neįvestas tekstas. Įveskite jį vėl.");
            exceptions.Add("Klaida. Neįvestas raktas. Įveskite jį vėl.");
            exceptions.Add("Klaida. Neįvykdytas pasirinkimas.");
            exceptions.Add("");

            info.Add("Jūsų užšifruotas tekstas: ");
            info.Add("Jūsų iššifruotas tekstas: ");
            info.Add("Ivestis atlikta.");
            info.Add("Iveskite IV.");
        }
    }
}

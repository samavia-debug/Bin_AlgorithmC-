using BinCompletionAlgorithm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BinCompletionAlgorithm.Algorithm
{
    public class AlgorithmClass
    {
        public List<Bin> InitialBins { get; set; }
        public List<Item> InitialElements { get; set; }
        public static int MinimumLowerBound { get; set; } = 0;

        public Task<List<Bin>?> Execute()
        {
            SortElements();
            SortBins();
            int BinIndex = 0;
            return Task.FromResult(RecursionExecute(InitialElements, BinIndex, 0));
        }


        private List<Bin>? RecursionExecute(List<Item> Elmnts, int BinIndex, int InitWastedSpace)
        {
            // Creating A New Bins Result Collection For The Rest Of The Elements

            var WastedSpace = InitWastedSpace;

            List<Bin> Bins = new();
            List<Item> Elements = new(Elmnts);
            var lb = (int)((InitialElements.Sum(e => e.Value) + WastedSpace) / InitialBins.Average(b => b.Capacity)) + 1;
            if (BinIndex + 1 > lb)
                return null;
            if (MinimumLowerBound > 0 && lb > MinimumLowerBound)
                return null;
            else if (MinimumLowerBound == 0)
                MinimumLowerBound = lb;
            while (Elements.Any())
            {

                // Get The Work Bin Or Add A New Bin If All Bins Are Empty
                var curr = new Bin();
                if (InitialBins.Count() - 1 >= BinIndex)
                    curr = InitialBins.ElementAt(BinIndex);
                else
                {
                    var usablebins = InitialBins.Where(b => b.Capacity > Elements.First().Value);
                    if (usablebins.Any())
                    {
                        curr = usablebins.OrderBy(b => b.Capacity).First();
                    }
                    else
                    {
                        curr = InitialBins.First();
                    }
                }
                Bin CurrBin = new() { Capacity = curr.Capacity, Label = curr.Label };
                // Get The Biggest First Element Adding It To The Bin
                // And Removing it From The List Of Work Elements
                var elt = Elements.First();
                if (CurrBin.AddItem(elt))
                    Elements.Remove(elt);
                // If We Still Have Space in Our Bin We Go This Route
                if (CurrBin.Rest > 0 && Elements.Any())
                {
                    var restelt = Elements.Where(v => v.Value == CurrBin.Rest).FirstOrDefault();
                    if (restelt is not null)
                    {
                        if (CurrBin.AddItem(restelt))
                            Elements.Remove(restelt);
                    }
                    else
                    {
                        var dominantvals = DominanceRelation(Elements.Where(e => e.Value <= CurrBin.Rest).ToArray(), CurrBin.Rest).OrderByDescending(e => e.Sum(b => b.Value));
                        if (dominantvals.Count() > 1)
                        {
                            Bin solCurr = CurrBin;
                            List<Item> solElem = Elements;
                            List<Bin> solBins = new();
                            foreach (var item in dominantvals)
                            {
                                var tempCurr = CurrBin;
                                var tempElem = Elements;
                                solBins = new();
                                foreach (var elti in item)
                                {
                                    if (tempCurr.AddItem(elti))
                                        tempElem.Remove(elti);
                                }
                                
                                var res = RecursionExecute(tempElem, BinIndex + 1, WastedSpace);
                                if (res is not null && res.Count < solBins.Count)
                                {
                                    solElem = tempElem;
                                    solCurr = tempCurr;
                                    solBins = res;
                                }
                            }
                            CurrBin = solCurr;
                            Elements = solElem;
                            if (solBins is not null)
                                Bins.AddRange(solBins);
                        }
                        else if (dominantvals.Any())
                        {
                            foreach (var elti in dominantvals.First())
                            {
                                if (CurrBin.AddItem(elti))
                                    Elements.Remove(elti);
                            }
                        }
                    }
                }
                Bins.Add(CurrBin);
                WastedSpace += CurrBin.Rest;
                BinIndex++;
            }
            return Bins;
        }
        public List<List<Item>> DominanceRelation(Item[] InitElmnts, int Limit)
        {
            var Elmnts = InitElmnts.OrderByDescending(i => i.Value).ToList();
            if (Elmnts.Count == 0)
            {
                // Return 0 : Meaning We Don't Have A Dominant Value
                return new() { new() { new() { Value = 0 } } };
            }
            else if (Elmnts.Count == 1)
            {
                // Return Single Element : We Only Have A Single Element So We Use It
                return new() { new() { Elmnts.First() } };
            }
            List<List<Item>> DominantValues = new();
            List<Item> DominatedValues = new();
            while (Elmnts.Count > 0)
            {
                int max = InitElmnts.Max(e => e.Value);
                List<Item> CurrVals = new() { Elmnts.First() };
                foreach (var elmnt in Elmnts.Except(CurrVals))
                {
                    var total = CurrVals.Sum(e => e.Value) + elmnt.Value;
                    if (total <= Limit)
                    {
                        CurrVals.Add(elmnt);
                    }
                }
                if (DominantValues.Any())
                {
                    var first = DominantValues.First();
                    if (!(first.Count() == 1 && first.First().Value >= CurrVals.Sum(e => e.Value)))
                    {
                        if (!(first.Count() == CurrVals.Count() && first.TrueForAll(e =>
                        {
                            var i = first.IndexOf(e);
                            return e.Value >= CurrVals.ElementAt(i).Value;
                        })))
                        {
                            DominantValues.Add(CurrVals);
                        }
                    }
                }
                else
                {
                    DominantValues.Add(CurrVals);
                }                
                Elmnts.RemoveAll(e => CurrVals.Contains(e));

            }
            return DominantValues;
        }

        // Sorting Operations
        private void SortElements()
        {
            InitialElements = new(InitialElements.OrderByDescending(i => i.Value).Where(i => i.Value > 0));
        }
        private void SortBins()
        {
            InitialBins = new(InitialBins.OrderByDescending(i => i.Capacity));
        }
    }
}

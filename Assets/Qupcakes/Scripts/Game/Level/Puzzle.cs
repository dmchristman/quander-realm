using System;
using System.Collections.Generic;
using UnityEngine;
using SysDebug = System.Diagnostics.Debug;

/*
 * Puzzle batch info: sets of (cake type, customer order) pair
 * Interfacing with game setup modules
 */
namespace Qupcakery
{
    // A puzzle batch with a list of entries (the order matters!)
    public class Puzzle
    {
        public int Size { get; private set; }
        public (int, int)[] entries { get; private set; }

        // Constructor
        public Puzzle(int maxEntryCnt)
        {
            entries = new (int, int)[maxEntryCnt];
            Size = maxEntryCnt;
        }

        public void UpdatePuzzle(int cakeType, int orderCakeType)
        {
            entries[0] = (cakeType, orderCakeType);
            Size = 1;
        }

        public void UpdatePuzzle(int cakeType0, int orderCakeType0,
            int cakeType1, int orderCakeType1)
        {
            entries[0] = (cakeType0, orderCakeType0);
            entries[1] = (cakeType1, orderCakeType1);
            Size = 2;
        }

        public void UpdatePuzzle(int cakeType0, int orderCakeType0,
           int cakeType1, int orderCakeType1,
           int cakeType2, int orderCakeType2)
        {
            entries[0] = (cakeType0, orderCakeType0);
            entries[1] = (cakeType1, orderCakeType1);
            entries[2] = (cakeType2, orderCakeType2);
            Size = 3;
        }

        public override string ToString()
        {
            string ret = "";
            for (int i = 0; i < Size; i++)
            {
                ret += "[";
                ret += entries[i].Item1.ToString();
                ret += " ,";
                ret += entries[i].Item2.ToString();
                ret += "] \n";
            }
            return ret;
        }


        //// Constructor for size 1 puzzle
        //public Puzzle(int cakeType, int orderCakeType)
        //{
        //    AddEntry(cakeType, orderCakeType);
        //}

        //// Constructor for size 2 puzzle
        //public Puzzle(int cakeType0, int orderCakeType0,
        //    int cakeType1, int orderCakeType1)
        //{
        //    AddEntry(cakeType0, orderCakeType0);
        //    AddEntry(cakeType1, orderCakeType1);
        //}

        //// Constructor for size 3 puzzle
        //public Puzzle(int cakeType0, int orderCakeType0,
        //    int cakeType1, int orderCakeType1,
        //    int cakeType2, int orderCakeType2)
        //{
        //    AddEntry(cakeType0, orderCakeType0);
        //    AddEntry(cakeType1, orderCakeType1);
        //    AddEntry(cakeType2, orderCakeType2);
        //}

        //// Entangle a pair of order
        //public void EntangleOrderPair(int entryInd0, int entryInd1,
        //    EntanglementStatus status)
        //{
        //    /* Check whether the entries are already entangled */
        //    Order order0 = entries[entryInd0].Order;
        //    Order order1 = entries[entryInd1].Order;
        //    if (order0.entanglementStatus != EntanglementStatus.None
        //        || order1.entanglementStatus != EntanglementStatus.None)
        //    {
        //        throw new ArgumentException("EntangleOrderPair: " +
        //            "Entanglement status must be null before entangling the pair");
        //    }

        //    /* Check that the orders have the same mystery cake type */
        //    /* Currently only support the type Vanilla50_Chocolate50 */
        //    if (order0.GetGameCakeType() != GameCakeType.Vanilla50_Chocolate50
        //        || order1.GetGameCakeType() != GameCakeType.Vanilla50_Chocolate50)
        //    {
        //        throw new ArgumentException("EntangleOrderPair: " +
        //            "Entanglement pair must both have type Vanilla50_Chocolate50");
        //    }

        //    order0.SetEntanglementStatus(status);
        //    order1.SetEntanglementStatus(status);

        //    EntangledPair = new Tuple<int, int>(entryInd0, entryInd1);
        //    entanglementStatus = status;

        //    HasEntangledPairs = true; 
        //}

        ////// Adds an entry to the puzzle
        ////private void AddEntry(Cake newCake, Order newOrder)
        ////{
        ////    entries.Add(new PuzzleEntry(newCake, newOrder));
        ////}

        //// Gets a puzzle entry, deep copy
        //public (Cake, Order) GetEntry(int ind)
        //{
        //    SysDebug.Assert(0 <= ind && ind < Size);
        //    Cake cake = new Cake(entries[ind].Cake.GetCakeType());
        //    Order order = new Order(entries[ind].Order.GetGameCakeType());

        //    order.SetEntanglementStatus(entries[ind].Order.entanglementStatus);

        //    return (cake, order);
        //}
    }
}

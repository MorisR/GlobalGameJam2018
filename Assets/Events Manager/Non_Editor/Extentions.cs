using System;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Extentions
{
    public static class Collections
    {

        public static void SwapCells(this IList list, int firstInedx, int secondIndex)
        {
            if (list.IndexInRange(firstInedx) && list.IndexInRange(secondIndex))
            {
                var temp = list[firstInedx];
                list[firstInedx] = list[secondIndex];
                list[secondIndex] = temp;
            }
        }

        public static bool IndexInRange(this IList list, int index)
        {
            return index >= 0 && index < list.Count;
        }



    }
}



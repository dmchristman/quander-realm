using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Qupcakery
{
    public static class Utilities
    {
        public static List<Sprite> helpMenuSprites = new List<Sprite> { };

        // Add levels that have tutorials
        public static void InitializeTutorialAndHelpMenu()
        {
            TutorialManager.UpdateAvailability(-1);
            LoadSprites(helpMenuSprites, "Sprites/Help");
        }

        static void LoadSprites(List<Sprite> list, string folder)
        {
            Sprite[] LoadedObjects = Resources.LoadAll(folder, typeof(Sprite)).Cast<Sprite>().ToArray();
            foreach (var obj in LoadedObjects)
            {
                list.Add(obj);
            }
        }

        public static string GetStringFrom2DIntArray(int[,] arr, int rowCnt, int colCnt)
        {
            string str = "";
            for (int i = 0; i < rowCnt; i++)
            {
                str += "[";
                for (int j = 0; j < colCnt; j++)
                {
                    str += arr[i, j].ToString();
                    if (j < colCnt - 1) str += " ,";
                }
                str += "]\n";
            }
            return str;
        }
    }
}
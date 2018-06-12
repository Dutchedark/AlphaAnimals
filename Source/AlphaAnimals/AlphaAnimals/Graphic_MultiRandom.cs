using RimWorld;
using Verse;
using UnityEngine;
using System;
using UnityObject = UnityEngine.Object;

namespace AlphaAnimals
{
    public class Graphic_MultiRandom : Graphic_Multi
    {
        public Graphic_MultiRandom(Graphic graphic)
        {
            path = graphic.path;
            drawSize = graphic.drawSize;
            data = graphic.data;
            color = graphic.color;
            colorTwo = graphic.colorTwo;

        }

        public Graphic_MultiRandom() : base()
        {
        }

        internal static string PathBase(string req)
        {
            try
            {
                while (true)
                {
                    int x = int.Parse(req.Substring(req.Length - 1));
                    // fail on already having a tex number
                    Debug.Log("Trimming " + x);
                    req = req.Substring(0, req.Length - 1);
                }
            }
            catch (FormatException)
            {
                // not a numbered texpath, go ahead with execution
            };

            return req;
        }

        public Graphic GetColoredVersion(Shader newShader, Color newColor, String textureIndexPath)
        {
            path = PathBase(path);
            path += textureIndexPath;

            return GraphicDatabase.Get<Graphic_Multi>(path, newShader, drawSize, newColor, Color.white, data);
        }

    }
}

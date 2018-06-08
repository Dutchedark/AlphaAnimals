using RimWorld;
using Verse;
using UnityEngine;
using System;
using UnityObject = UnityEngine.Object;

namespace AlphaAnimals
{
    public class Graphic_MultiRandom : Graphic
    {
        private Material[] mats = new Material[3];

        public string GraphicPath
        {
            get
            {
                return this.path;
            }
        }

        public override Material MatSingle
        {
            get
            {
                return this.MatFront;
            }
        }

        public override Material MatFront
        {
            get
            {
                return this.mats[2];
            }
        }

        public override Material MatSide
        {
            get
            {
                return this.mats[1];
            }
        }

        public override Material MatBack
        {
            get
            {
                return this.mats[0];
            }
        }

        public override bool ShouldDrawRotated
        {
            get
            {
                return (UnityObject)this.MatSide == (UnityObject)this.MatBack;
            }
        }

        public override void Init(GraphicRequest req)
        {
            this.data = req.graphicData;
            this.path = req.path;
            this.color = req.color;
            this.colorTwo = req.colorTwo;
            this.drawSize = req.drawSize;

            int TextureCountTotal = TextureCount(this.path);
            if (TextureCountTotal > 0)
            {
                int textureIndex = (new System.Random(1000 * 2)).Next() % TextureCountTotal;
                string safeTextureIndex = (1 + textureIndex).ToString();
                Log.Warning(safeTextureIndex);
                this.path += safeTextureIndex;
            }
            

            Texture2D[] texture2DArray1 = new Texture2D[3]
            {
                ContentFinder<Texture2D>.Get(path + "_back", false),
                null,
                null
            };
            if ((UnityObject)texture2DArray1[0] == (UnityObject)null)
            {
                Log.Error("Failed to find any texture while constructing " + this.ToString());
            }
            else
            {
                texture2DArray1[1] = ContentFinder<Texture2D>.Get(path + "_side", false);
                if ((UnityObject)texture2DArray1[1] == (UnityObject)null)
                    texture2DArray1[1] = texture2DArray1[0];
                texture2DArray1[2] = ContentFinder<Texture2D>.Get(path + "_front", false);
                if ((UnityObject)texture2DArray1[2] == (UnityObject)null)
                    texture2DArray1[2] = texture2DArray1[0];
                Texture2D[] texture2DArray2 = new Texture2D[3];
                if (req.shader.SupportsMaskTex())
                {
                    texture2DArray2[0] = ContentFinder<Texture2D>.Get(path + "_backm", false);
                    if ((UnityObject)texture2DArray2[0] != (UnityObject)null)
                    {
                        texture2DArray2[1] = ContentFinder<Texture2D>.Get(path + "_sidem", false);
                        if ((UnityObject)texture2DArray2[1] == (UnityObject)null)
                            texture2DArray2[1] = texture2DArray2[0];
                        texture2DArray2[2] = ContentFinder<Texture2D>.Get(path + "_frontm", false);
                        if ((UnityObject)texture2DArray2[2] == (UnityObject)null)
                            texture2DArray2[2] = texture2DArray2[0];
                    }
                }
                for (int index = 0; index < 3; ++index)
                    this.mats[index] = MaterialPool.MatFrom(new MaterialRequest()
                    {
                        mainTex = texture2DArray1[index],
                        shader = req.shader,
                        color = this.color,
                        colorTwo = this.colorTwo,
                        maskTex = texture2DArray2[index]
                    });
            }
        }

        public override Graphic GetColoredVersion(Shader newShader, Color newColor, Color newColorTwo)
        {
            return GraphicDatabase.Get<Graphic_Multi>(this.path, newShader, this.drawSize, newColor, newColorTwo, this.data);
        }

        public override string ToString()
        {
            return "MultiRandom(initPath=" + this.path + ", color=" + (object)this.color + ", colorTwo=" + (object)this.colorTwo + ")";
        }

        public override int GetHashCode()
        {
            return Gen.HashCombineStruct<Color>(Gen.HashCombineStruct<Color>(Gen.HashCombine<string>(0, this.path), this.color), this.colorTwo);
        }

        public static int TextureCount(string path)
        {
            // find all assets with custom number tags
            int count = 0;

            UnityObject o = ContentFinder<Texture2D>.Get(path + (count + 1) + "_back", false);

            while (o != null)
            {
                count++;
                o = ContentFinder<Texture2D>.Get(path + (count + 1) + "_back", false);
            }

            return count;
        }
    }
}

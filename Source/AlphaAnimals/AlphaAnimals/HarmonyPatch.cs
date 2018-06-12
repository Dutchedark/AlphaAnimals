using Harmony;
using Verse;
using UnityEngine;

namespace AlphaAnimals
{
    [StaticConstructorOnStartup]
    static class Harmony_GraphicPatch
    {

        static Harmony_GraphicPatch()
        {
            Log.Message("Graphic_MultiRandom patch");
            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.soggynoodle.HarmonyGraphicPatch");
            harmony.Patch(AccessTools.Method(typeof(PawnGraphicSet), "ResolveAllGraphics"),
                new HarmonyMethod(typeof(Harmony_GraphicPatch), nameof(PawnGraphicSet_ResolveAllGraphics_prefix)), null, null);
        }
        [HarmonyPrefix]
        private static bool PawnGraphicSet_ResolveAllGraphics_prefix(PawnGraphicSet __instance)
        {
            if (__instance.pawn.RaceProps.Animal)
            {
                Pawn pawn = __instance.pawn;
                PawnKindLifeStage curKindLifeStage = __instance.pawn.ageTracker.CurKindLifeStage;

                Graphic baseGraphic = pawn.gender != Gender.Female || curKindLifeStage.femaleGraphicData == null ? curKindLifeStage.bodyGraphicData.Graphic : curKindLifeStage.femaleGraphicData.Graphic;
                Color color = baseGraphic.color;
                Color ColorTwo = baseGraphic.colorTwo;
                ShaderType shader = ShaderType.Cutout;

                if (__instance.pawn.ageTracker.CurKindLifeStage.bodyGraphicData.graphicClass == typeof(Graphic_MultiRandom))
                {
                    int TextureCountTotal = TextureCount(baseGraphic.path);
                    if (TextureCountTotal > 0)
                    {
                        int textureIndex = (new System.Random(pawn.thingIDNumber * 2)).Next() % TextureCountTotal;
                        string safeTextureIndex = (1 + textureIndex).ToString();

                        __instance.ClearCache();

                        if (curKindLifeStage.dessicatedBodyGraphicData is GraphicData dessicated)
                        {
                            __instance.dessicatedGraphic = dessicated.GraphicColoredFor(__instance.pawn);
                        }

                        __instance.nakedGraphic = (new Graphic_MultiRandom(baseGraphic)).GetColoredVersion(ShaderDatabase.ShaderFromType(shader), color, safeTextureIndex);
                        return false;

                    }
                }
                return true;
            }
            return true;
        }

        public static int TextureCount(string path)
        {
            // find all assets with custom number tags
            int count = 0;

            Object o = ContentFinder<Texture2D>.Get(path + (count + 1) + "_back", false);

            while (o != null)
            {
                count++;
                o = ContentFinder<Texture2D>.Get(path + (count + 1) + "_back", false);
            }

            return count;
        }
    }
}
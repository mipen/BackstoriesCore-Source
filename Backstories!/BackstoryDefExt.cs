using Verse;

namespace Backstories
{
    public static class BackstoryDefExt
    {
        public static string UniqueSaveKeyFor(this BackstoryDef def)
        {
            if (def.saveKeyIdentifier.NullOrEmpty())
                return "CustomBackstory_" + def.defName;
            else
                return def.saveKeyIdentifier + "_" + def.defName;
        }
    }
}

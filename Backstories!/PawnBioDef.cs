using RimWorld;
using Verse;

namespace Backstories
{
    public class PawnBioDef : Def
    {
        public BackstoryDef childhoodDef;
        public BackstoryDef adulthoodDef;
        public PawnName name;
        public GenderPossibility gender;

        public override void ResolveReferences()
        {
            base.ResolveReferences();

            PawnBio bio = new PawnBio();
            bio.gender = this.gender;
            bio.name = this.name;

            bio.childhood = BackstoryDatabase.GetWithKey(this.childhoodDef.UniqueSaveKeyFor());
            bio.childhood.shuffleable = false;
            bio.childhood.slot = BackstorySlot.Childhood;

            bio.adulthood = BackstoryDatabase.GetWithKey(this.adulthoodDef.UniqueSaveKeyFor());
            bio.adulthood.shuffleable = false;
            bio.adulthood.slot = BackstorySlot.Adulthood;

            bio.name.ResolveMissingPieces();
            //bio.PostLoad();

            bool flag = false;
            foreach (var error in bio.ConfigErrors())
            {
                if (!flag)
                {
                    flag = true;
                    Log.Error("Config error(s) in PawnBioDef " + this.defName + ". Skipping...");
                }
                Log.Error(error);
            }
            if (flag)
            {
                return;
            }
            if (!SolidBioDatabase.allBios.Contains(bio))
                SolidBioDatabase.allBios.Add(bio); 
        }
    }
}

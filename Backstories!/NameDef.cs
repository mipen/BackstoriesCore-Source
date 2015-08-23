using RimWorld;
using Verse;

namespace Backstories
{
    public class NameDef : Def
    {
        public string first;
        public string nick = "";
        public string last;
        public GenderPossibility genderPossibility = GenderPossibility.Either;

        public override void PostLoad()
        {
            base.PostLoad();

            PawnNameDatabaseSolid.AddPlayerContentName(Name, this.genderPossibility);

            //Log.Message(name.ToString());
        }

        public NameTriple Name
        {
            get
            {
                return new NameTriple(first, nick, last);
            }
        }
    }
}

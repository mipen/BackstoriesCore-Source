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

            PawnName name = new PawnName();
            name.first = this.first;
            name.nick = this.nick;
            name.last = this.last;
            name.ResolveMissingPieces();
            PawnNameDatabaseSolid.AddPlayerContentName(name, this.genderPossibility);

            //Log.Message(name.ToString());
        }
    }
}

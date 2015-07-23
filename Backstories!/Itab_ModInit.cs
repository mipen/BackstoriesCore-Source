using UnityEngine;
using Verse;

namespace Backstories
{
    public class Itab_ModInit  : ITab
    {
        public Itab_ModInit()
        {
            GameObject obj = new GameObject("BackstoriesModController");
            obj.AddComponent<Backstories.BackstoriesModController>();
            Log.Message("Initialised backstories module");
        }

        protected override void FillTab()
        {
            return;
        }
    }
}

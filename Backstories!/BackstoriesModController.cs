using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Backstories
{
    public class BackstoriesModController : MonoBehaviour
    {
        public static readonly string GameObjectName = "BackstoriesModController";
        public static readonly string ModName = "BackstoriesCore";
        public static ConfigDef Config;
        private bool gameplay = false;
        private bool modsChanged = false;
        private List<string> previousModsList = new List<string>();

        public virtual void Start()
        {
            Config = DefDatabase<ConfigDef>.GetNamed("BackstoriesConfig");
            this.enabled = true;
            previousModsList = CurrentActiveMods;
            ReloadDefs(true);
        }

        private void ReloadDefs(bool report)
        {
            AddNames(report);
            //AddBackstories(report);
            AddPawnBios(report);
        }

        public void OnLevelWasLoaded(int level)
        {
            if (level == 0)
            {
                gameplay = false;
                enabled = true;
            }
            else if (level == 1)
            {
                gameplay = true;
                enabled = false;
            }
        }

        public virtual void Update()
        {
            try
            {
                if (!gameplay && ModEnabled)
                {
                    CheckModsChanged();
                    if (modsChanged)
                    {
                        ReloadDefs(false);
                        modsChanged = false;
                    }
                }
            }
            catch (Exception ex)
            {
                enabled = false;
                Log.Error(ex.Message);
            }
        }

        private bool ModEnabled
        {
            get
            {
                InstalledMod mod = InstalledModLister.AllInstalledMods.First(m => m.Name.Equals(BackstoriesModController.ModName));
                return mod != null && mod.Active;
            }
        }

        private List<string> CurrentActiveMods
        {
            get
            {
                return ModsConfig.ActiveModNames.ToList();
            }
        }

        private void CheckModsChanged()
        {
            List<string> list = CurrentActiveMods;
            bool flag = false;

            if (previousModsList.Count == CurrentActiveMods.Count)
            {
                for (int i = 0; i < previousModsList.Count; i++)
                {
                    if (previousModsList[i] != list[i])
                    {
                        flag = true;
                        break;
                    }
                }
            }
            else
            {
                flag = true;
            }
            if (flag)
            {
                modsChanged = true;
                previousModsList = CurrentActiveMods;
            }
        }

        private void AddPawnBios(bool report = false)
        {
            int num = 0;
            foreach (var def in DefDatabase<PawnBioDef>.AllDefs)
            {
                PawnBio bio = new PawnBio();
                bio.gender = def.gender;
                bio.name = def.name;

                bio.childhood = BackstoryDatabase.GetWithKey(def.childhoodDef.UniqueSaveKeyFor());
                bio.childhood.shuffleable = false;
                bio.childhood.slot = BackstorySlot.Childhood;

                bio.adulthood = BackstoryDatabase.GetWithKey(def.adulthoodDef.UniqueSaveKeyFor());
                bio.adulthood.shuffleable = false;
                bio.adulthood.slot = BackstorySlot.Adulthood;

                bio.name.ResolveMissingPieces();
                bio.PostLoad();

                bool flag = false;
                foreach (var error in bio.ConfigErrors())
                {
                    if (!flag)
                    {
                        flag = true;
                        Log.Error("Config error(s) in PawnBioDef " + def.defName + ". Skipping...");
                    }
                    Log.Error(error);
                }
                if (flag)
                {
                    continue;
                }
                SolidBioDatabase.allBios.Add(bio);
                num++;
            }
            if (num > 0 && report)
                Log.Warning("BackstoriesCore added: " + num.ToString() + " PawnBios.");
        }

        private void AddNames(bool report = false)
        {
            int num = 0;
            foreach (var def in DefDatabase<NameDef>.AllDefs)
            {
                PawnName name = new PawnName();
                name.first = def.first;
                name.nick = def.nick;
                name.last = def.last;
                name.ResolveMissingPieces();
                PawnNameDatabaseSolid.AddPlayerContentName(name, def.genderPossibility);
                num++;
            }
            if (num > 0 && report)
                Log.Warning("BackstoriesCore added " + num.ToString() + " custom names.");
        }

        private void AddBackstories(bool report)
        {
            if (Config.debugRemoveAllVanillaBackstories)
            {
                BackstoryDatabase.Clear();
            }

            int num = 0;
            foreach (var def in DefDatabase<BackstoryDef>.AllDefs)
            {
                if (!def.addToDatabase) continue;
                if (BackstoryDatabase.allBackstories.ContainsKey(def.UniqueSaveKeyFor())) continue;

                Backstory b = new Backstory();
                if (!def.title.NullOrEmpty())
                    b.title = def.title;
                else
                {
                    Log.Error(def.defName + " backstory has empty title. Skipping...");
                    continue;
                }
                if (!def.titleShort.NullOrEmpty())
                    b.titleShort = def.titleShort;
                else
                    b.titleShort = b.title;
                b.baseDesc = def.baseDescription;
                b.bodyTypeGlobal = def.bodyTypeGlobal;
                b.bodyTypeMale = def.bodyTypeMale;
                b.bodyTypeFemale = def.bodyTypeFemale;
                b.slot = def.slot;
                b.shuffleable = def.shuffleable;
                b.spawnCategories = def.spawnCategories;
                if (def.workAllows.Count > 0)
                {
                    foreach (WorkTags current in Enum.GetValues(typeof(WorkTags)))
                    {
                        if (!def.workAllows.Contains(current))
                        {
                            b.workDisables |= current;
                        }
                    }
                }
                else if (def.workDisables.Count > 0)
                {
                    foreach (var tag in def.workDisables)
                    {
                        b.workDisables |= tag;
                    }
                }
                else
                {
                    b.workDisables = WorkTags.None;
                }
                b.skillGains = def.skillGains.ToDictionary(i => i.defName, i => i.amount);
                b.ResolveReferences();
                b.PostLoad();
                b.uniqueSaveKey = def.UniqueSaveKeyFor();
                bool flag = false;
                foreach (var s in b.ConfigErrors(false))
                {
                    if (!flag)
                    {
                        flag = true;
                        Log.Error("Backstories! backstory errors in custom backstory with defName: " + def.defName + ", backstory will be skipped");
                    }
                    Log.Error(def.defName + " error: " + s);
                }
                if (!flag)
                {
                    BackstoryDatabase.AddBackstory(b);
                    num++;
                }
            }
            if (num > 0 && report)
                Log.Warning("BackstoriesCore added " + num.ToString() + " custom backstories.");

        }
    }
}

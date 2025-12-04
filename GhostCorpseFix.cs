using System.Collections.Generic;
using Oxide.Core.Plugins;
using Oxide.Plugins;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Ghost Corpse Fix", "zhuhlia", "2.1.0")]
    [Description("Clear Ghost player corpse on map")]
    public class GhostCorpseFix : RustPlugin
    {
        private Timer? checkTimer = null;
        private readonly List<PlayerCorpse> trackedCorpses = new List<PlayerCorpse>();

        private void Init()
        {
            try
            {
                var existing = UnityEngine.Object.FindObjectsOfType<PlayerCorpse>();
                if (existing != null && existing.Length > 0)
                {
                    trackedCorpses.AddRange(existing);
                }
            }
            catch { }

            MCCleaner();
        }
        void Unload()
        {
            checkTimer?.Destroy();
            trackedCorpses.Clear();
        }
        private void MCCleaner()
        {
            // Автоочистка
            checkTimer = timer.Every(120f, () =>
            {
                CleanupGhostCorpses();
            });
        }

        // добавляем труп в список
        private void OnEntitySpawned(BaseNetworkable entity)
        {
            if (entity is PlayerCorpse corpse)
            {
                if (!trackedCorpses.Contains(corpse))
                {
                    trackedCorpses.Add(corpse);
                }
            }
        }

        private void CleanupGhostCorpses()
        {
            int removed = 0;
            trackedCorpses.RemoveAll(x => x == null);
            foreach (var corpse in trackedCorpses.ToArray())
            {

                if (corpse.IsDestroyed && corpse.gameObject != null)
                {
                    try
                    {
                        UnityEngine.Object.DestroyImmediate(corpse.gameObject);
                    }
                    catch { }
                    trackedCorpses.Remove(corpse);
                    removed++;
                }
                else if (corpse.IsDestroyed)
                {
                    trackedCorpses.Remove(corpse);
                }
            }
            if (removed > 0)
            {
                PrintError($"Auto-cleanup removed {removed} ghost corpses!");
            }
        }
    }
}
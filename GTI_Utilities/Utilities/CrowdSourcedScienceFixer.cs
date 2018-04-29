using UnityEngine;
using static GTI.Utilities;

namespace GTI
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class CrowdSourcedScienceFixer : MonoBehaviour
    {
        public void Start()
        {
            //Check if ScienceFixer from CrowdSourcedScience is present --> if it does, then abort
            if (PluginExists("ScienceFixer"))
                Destroy(this.gameObject);
            if (!GTIConfig.CrowdSourcedScienceFixer.Activate)
                Destroy(this.gameObject);

            foreach (ConfigNode config in GameDatabase.Instance.GetConfigNodes("EXPERIMENT_DEFINITION"))
            {
                // Safety check
                if (!config.HasNode("RESULTS"))
                    return;

                ConfigNode results = config.GetNode("RESULTS");
                ConfigNode data = new ConfigNode();
                foreach (ConfigNode.Value key in results.values)
                {
                    if (!key.name.StartsWith("default") && !key.name.EndsWith("*"))
                        data.AddValue(key.name + "*", key.value);
                    else
                        data.AddValue(key.name, key.value);
                }
                results.ClearData();
                results.AddData(data);
            }
        }

        private void OnDestroy()
        {
            GTIDebug.Log("CrowdSourcedScienceFixer disabled", GTIConfig.iDebugLevel.DebugInfo);
        }
    }
}
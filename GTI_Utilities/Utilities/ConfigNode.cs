//using System;
//using System.Collections.Generic;
using UnityEngine;


namespace GTI
{
    public static partial class Utilities
    {
        /// <summary>
        /// Retrieves the part configuration node trought available part in partloader
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public static ConfigNode GetPartConfig(this Part part)
        {
            AvailablePart thispart = GetSourcePart(part);

            if (thispart == null)
            {
                Debug.LogError("GetPartConfig: PART NOT FOUND");
                return null;
            }
            else
            {
                return thispart.partConfig;
            }
        }
        /// <summary>
        /// Retrieves the part configuration node trought available part in partloader
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public static ConfigNode GetPartConfig(string part)
        {
            AvailablePart thispart = GetSourcePart(part);

            if (thispart == null)
            {
                Debug.LogError("GetPartConfig: PART NOT FOUND");
                return null;
            }
            else
            {
                return thispart.partConfig;
            }
        }
        /// <summary>
        /// Retrieves the part configuration node trought available part in partloader, incl. extraction of the partName
        /// </summary>
        /// <param name="part"></param>
        /// <param name="partName"></param>
        /// <returns></returns>
        public static ConfigNode GetPartConfig(this Part part, out string partName)
        {
            partName = string.Empty;
            AvailablePart thispart = GetSourcePart(part);

            if (thispart == null)
            {
                Debug.LogError("GetPartConfig: PART NOT FOUND");
                return null;
            }
            else
            {
                partName = thispart.name;
                return thispart.partConfig;
            }
        }

        /// <summary>
        /// Retrieves the part configuration node trought available part in partloader, incl. extraction of the partName and partTitle
        /// </summary>
        /// <param name="part"></param>
        /// <param name="partName"></param>
        /// <param name="partTitle"></param>
        /// <returns></returns>
        public static ConfigNode GetPartConfig(this Part part, out string partName, out string partTitle)
        {
            partName = string.Empty;
            partTitle = string.Empty;
            AvailablePart thispart = GetSourcePart(part);

            if (thispart == null)
            {
                Debug.LogError("GetPartConfig: PART NOT FOUND");
                return null;
            }
            else
            {
                partName = thispart.name;
                partTitle = thispart.title;
                return thispart.partConfig;
            }
        }

        //public ConfigNode[] GetNodes(string name, string valueName, string value)

        /// <summary>
        /// Retrieves the ConfigNodes of the specified node in a part.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="nodeName"></param>
        /// <param name="valueName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ConfigNode[] GetPartModuleConfigs(this Part part, string nodeName, string valueName, string value)
        {
            AvailablePart thispart = GetSourcePart(part);

            if (thispart == null)
            {
                Debug.LogError("GetPartConfig: PART NOT FOUND");
                return null;
            }
            else
            {
                ConfigNode[] resultingNodes = thispart.partConfig.GetNodes(nodeName, valueName, value);
                return resultingNodes;
            }
        }
        /// <summary>
        /// Retrieves the ConfigNodes of the specified node in a part.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public static ConfigNode[] GetPartModuleConfigs(this Part part, string nodeName)
        {
            AvailablePart thispart = GetSourcePart(part);

            if (thispart == null)
            {
                Debug.LogError("GetPartConfig: PART NOT FOUND");
                return null;
            }
            else
            {
                ConfigNode[] resultingNodes = thispart.partConfig.GetNodes(nodeName);
                return resultingNodes;
            }
        }
        /// <summary>
        /// Retrieves the ConfigNode of the specified node in a part.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="nodeName"></param>
        /// <param name="valueName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ConfigNode GetPartModuleConfig(this Part part, string nodeName, string valueName, string value)
        {
            AvailablePart thispart = GetSourcePart(part);

            if (thispart == null)
            {
                Debug.LogError("GetPartConfig: PART NOT FOUND");
                return null;
            }
            else
            {
                ConfigNode resultingNode = thispart.partConfig.GetNode(nodeName, valueName, value);
                return resultingNode;
            }
        }

        /// <summary>
        /// Extracts the part URL through partloader
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public static string GetPartUrl(this Part part)
        {
            AvailablePart thispart = GetSourcePart(part);
            string output = string.Empty;
            if (thispart == null)
            {
                Debug.LogError("GetPartUrl: PART NOT FOUND");
                return "PART NOT FOUND";
            }
            else
            {
                output = thispart.partUrl;
                return output;
            }
        }
        /// <summary>
        /// Retrieves the part in partLoader (available part) based on the source part (object)
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        private static AvailablePart GetSourcePart(this Part part)
        {
            return GetSourcePart(part.name);
        }
        /// <summary>
        /// Retrieves the part in partLoader (available part) based on the source part name
        /// </summary>
        /// <param name="partName"></param>
        /// <returns></returns>
        private static AvailablePart GetSourcePart(string partName)
        {
            AvailablePart sourcePartLoader = new AvailablePart();
            bool _partFound = false;
            for (int i = 0; i < PartLoader.Instance.loadedParts.Count; i++)
            {
                if (partName == PartLoader.Instance.loadedParts[i].name)
                {
                    sourcePartLoader = PartLoader.Instance.loadedParts[i];
                    _partFound = true;
                    break;
                }
            }

            if (!_partFound) { sourcePartLoader = null; }
            return sourcePartLoader;
        }
    }
}

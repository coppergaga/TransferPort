using System.Collections.Generic;

namespace RsTransferPort
{
    public class PortalUtilityNetworkLinkContainer
    {
        private readonly Dictionary<int, Dictionary<string, HashSet<WirelessPowerPort>>> container;


        public PortalUtilityNetworkLinkContainer()
        {
            App.OnPreLoadScene += Clear;
            container =
                new Dictionary<int, Dictionary<string, HashSet<WirelessPowerPort>>>();
        }

        public HashSet<WirelessPowerPort> Add(int worldId, string name,
            WirelessPowerPort wirelessPowerPort)
        {
            Dictionary<string, HashSet<WirelessPowerPort>> nameContainer = null;
            HashSet<WirelessPowerPort> portalWireSet = null;
            if (!container.TryGetValue(worldId, out nameContainer))
            {
                nameContainer = new Dictionary<string, HashSet<WirelessPowerPort>>();
                container.Add(worldId, nameContainer);
            }

            if (!nameContainer.TryGetValue(name, out portalWireSet))
            {
                portalWireSet = new HashSet<WirelessPowerPort>();
                nameContainer.Add(name, portalWireSet);
            }

            portalWireSet.Add(wirelessPowerPort);
            return portalWireSet;
        }

        public void Remove(int worldId, string name, WirelessPowerPort wirelessPowerPort)
        {
            Dictionary<string, HashSet<WirelessPowerPort>> nameContainer = null;
            HashSet<WirelessPowerPort> portalWireSet = null;

            if (container.TryGetValue(worldId, out nameContainer) && nameContainer.TryGetValue(name, out portalWireSet))
            {
                portalWireSet.Remove(wirelessPowerPort);
                if (portalWireSet.Count == 0) nameContainer.Remove(name);
            }
        }

        public bool Contains(int worldId, string name, WirelessPowerPort wirelessPowerPort)
        {
            var portalWireSet = GetPortalWireSet(worldId, name);

            return portalWireSet?.Contains(wirelessPowerPort) ?? false;
        }

        public HashSet<WirelessPowerPort> GetPortalWireSet(int worldId, string name)
        {
            Dictionary<string, HashSet<WirelessPowerPort>> nameContainer = null;
            HashSet<WirelessPowerPort> portalWireSet = null;

            if (container.TryGetValue(worldId, out nameContainer) && nameContainer.TryGetValue(name, out portalWireSet))
                return portalWireSet;

            return null;
        }

        public ICollection<string> GetNamesByWorld(int worldId)
        {
            Dictionary<string, HashSet<WirelessPowerPort>> nameContainer = null;
            if (container.TryGetValue(worldId, out nameContainer)) return nameContainer.Keys;

            return new List<string>();
        }

        public int Count(int worldId, string name)
        {
            return GetPortalWireSet(worldId, name)?.Count ?? 0;
        }

        public void Clear()
        {
            container.Clear();
        }
    }
}
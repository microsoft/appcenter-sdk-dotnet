using System;
using Tizen.Network.Connection;
using static Tizen.Network.Connection.ConnectionManager;
using Tizen.Applications;

namespace Microsoft.Azure.Mobile.Ingestion.Http
{
    public class NetworkStateAdapter : INetworkStateAdapter
    {
        public NetworkStateAdapter()
        {
            // IPAddressChanged Event handles change between networks/ IP change within the same network,
            //              but not disconnection from a network
            IPAddressChanged += (sender, args) =>
            {
                NetworkAddressChanged?.Invoke(sender, args);
            };

            // ConnectionTypeChanged Event handles Disconnection from networks
            ConnectionTypeChanged += (sender, args) =>
            {
                NetworkAddressChanged?.Invoke(sender, args);
            };
        }
        // Check if the state of the currently active connection is connected
        public bool IsConnected => (CurrentConnection.State == ConnectionState.Connected);
        public event EventHandler NetworkAddressChanged;
    }
}

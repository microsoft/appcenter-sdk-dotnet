using System;
using System.Net.NetworkInformation;

namespace Microsoft.Azure.Mobile.Ingestion.Http
{
    public class NetworkStateAdapter : INetworkStateAdapter
    {
        public NetworkStateAdapter()
        {
		// TODO check
            //NetworkChange.NetworkAddressChanged += (sender, args) => NetworkAddressChanged?.Invoke(sender, args);
        }
	// TODO check
        public bool IsConnected => true; /*NetworkInterface.GetIsNetworkAvailable();*/
        public event EventHandler NetworkAddressChanged;
    }
}

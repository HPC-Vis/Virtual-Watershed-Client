using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

public class NetworkManager
{
    // Members
    int index = 0;
    NetworkClient[] clients;

    // Constructor
    public NetworkManager(int size = 4)
    {
        // Check if size is greater than 0
        if( size <= 0 )
        {
            size = 1;
        }

        // Allocate the clients
        clients = new NetworkClient[size];
        for( int i = 0; i < size; ++i )
        {
            clients[i] = new NetworkClient();
        }
    }

    // Methods
    public void AddDownload( DownloadRequest req )
    {
        DataTracker.updateJob(req.Url, "Added to Network Client: " + index);
        // Add request at index
        clients[index].Download(req);

        // Increment index to the next queue (round robin)
        index = (index + 1) % clients.Length;
    }
}

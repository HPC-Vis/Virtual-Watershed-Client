using System;
using System.Net;

public class Client
{
	public bool Done = false;
	NetworkManager nm;
	public Client (NetworkManager NM)
	{
		nm = NM;
	}

	public Client()
	{
		
	}
	// Download Method goes here and will be a blocking download call.
	// Make sure to throw this into a download.
	public void Download(DownloadRequest req)
	{
		#if !(UNITY_WEBGL) || UNITY_EDITOR
		WebClient wc = new WebClient ();
		Logger.WriteLine("Downloading: " + req.Url); 
		// Lets do some downloading here
		GlobalConfig.loading = true;
		if(req.isByte)
		{
			byte[] data = wc.DownloadData(req.Url);
			if(req.byteFunction != null)
			{
				req.Callback(data);
			}
			Logger.WriteLine("Byte Function Complete");
		}
		else
		{
			string data = wc.DownloadString(req.Url);
			if(req.stringFunction != null)
			{
				req.Callback(data);
			}
			Logger.WriteLine("String Function Complete");
		}
		#else
		// Do webgl stuff here.
		#endif
		Done = true;
		if (nm != null) 
		{
			nm.CallDownloadComplete (req.Url);
			Logger.WriteLine ("COMPLETE");
		}
		GlobalConfig.loading = false;

	}
		
}


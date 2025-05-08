using Steamworks;
using UnityEngine;

namespace CodeBase.Network
{
	public class SteamAPIHandler
	{
		private readonly uint _appID;

		public SteamAPIHandler(uint appID)
		{
			_appID = appID;
		}

		public bool Initialize()
		{
			try
			{
				if (SteamAPI.RestartAppIfNecessary((AppId_t)_appID))
				{
					return false;
				}
			}
			catch (System.DllNotFoundException e)
			{
				Debug.LogError($"[Steamworks.NET] DLL not found: {e}");
				return false;
			}

			return SteamAPI.Init();
		}

		public void RunCallbacks()
		{
			SteamAPI.RunCallbacks();
		}

		public void Shutdown()
		{
			SteamAPI.Shutdown();
		}
	}
}
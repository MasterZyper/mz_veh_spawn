using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mz_veh_spawn
{
    public class Main : BaseScript
    {
        readonly List<Vehicle> player_vehs = new List<Vehicle>();
        //CFG
        int CFG_max_trys_to_spawn_veh = -1;
        int CFG_max_veh_per_player = -1;
        bool CFG_use_blacklist = true;
        bool CFG_use_whitelist = true;
        private List<string> whitelist_vehs = new List<string>();
        private List<string> blacklist_vehs = new List<string>();

        //Language
        string LG_wait_loading = "[Missing Text]";
        string LG_veh_spawn = "[Missing Text]";
        string LG_veh_not_spawned_hash = "[Missing Text]";
        string LG_veh_not_spawned_no_veh_free = "[Missing Text]";
        string LG_veh_on_blacklist = "[Missing Text]";
        string LG_veh_not_on_whitelist = "[Missing Text]";
        string LG_info_whitelist = "[Missing Text]";
        string LG_info_blacklist = "[Missing Text]";

        public Main()
        {
            //Load Cfg
            whitelist_vehs = ReadInputAsList("whitelist");
            blacklist_vehs = ReadInputAsList("blacklist");
            CFG_max_trys_to_spawn_veh = ReadInputAsInt("cfg_max_trys_to_spawn_veh");
            CFG_max_veh_per_player = ReadInputAsInt("cfg_max_veh_per_player");
            CFG_use_blacklist = ReadInputAsBool("cfg_use_blacklist");
            CFG_use_whitelist = ReadInputAsBool("cfg_use_whitelist");
            //Load Language
            LG_wait_loading = ReadInputAsString("lg_wait_loading");
            LG_veh_spawn = ReadInputAsString("lg_veh_spawn");
            LG_veh_not_spawned_hash = ReadInputAsString("lg_veh_not_spawned_hash");
            LG_veh_not_spawned_no_veh_free = ReadInputAsString("lg_veh_not_spawned_no_veh_free");
            LG_veh_on_blacklist = ReadInputAsString("lg_veh_on_blacklist");
            LG_veh_not_on_whitelist = ReadInputAsString("lg_veh_not_on_whitelist");
            LG_info_whitelist = ReadInputAsString("lg_info_whitelist");
            LG_info_blacklist = ReadInputAsString("lg_info_whitelist");
            API.RegisterCommand(ReadInputAsString("spawn_cmd"), new Action<int, List<object>, string>(async (player, value, raw) =>
            {
                if (CanPlayerSpawnVeh())
                {
                    string veh_hash = (string)value.ElementAt(0);
                    bool checker = true;

                    if (CFG_use_blacklist && IsVehicleOnBlacklist(veh_hash)) 
                    {
                        checker = false;
                        Debug.Write(LG_veh_on_blacklist + "\n");
                    }
                    if (CFG_use_whitelist && !IsVehicleOnWhitelist(veh_hash)) {
                        checker = false;
                        Debug.Write(LG_veh_not_on_whitelist + "\n");
                    }
                    Debug.Write($"{LG_veh_spawn}: {veh_hash}...\n");
                    Vehicle veh = await SpawnVehicle((uint)API.GetHashKey(veh_hash), Game.PlayerPed.Position);
                    if (veh == null)
                    {
                        Debug.Write(LG_veh_not_spawned_hash+"\n");
                    }
                    else
                    {
                        player_vehs.Add(veh);
                        Game.PlayerPed.Task.WarpIntoVehicle(veh, VehicleSeat.Driver);
                    }
                }
                else 
                {
                    Debug.Write(LG_veh_not_spawned_no_veh_free + "\n");
                }
            }), false);
            if (CFG_use_whitelist)
            {
                API.RegisterCommand(ReadInputAsString("list_whitelist_vehs"), new Action<int, List<object>, string>(async (player, value, raw) =>
                {
                    foreach (string str in whitelist_vehs)
                    {
                        Debug.Write(str + "\n");
                    }
                }), false);
            }
            if (CFG_use_blacklist)
            {
                API.RegisterCommand(ReadInputAsString("list_blacklist_vehs"), new Action<int, List<object>, string>(async (player, value, raw) =>
                {
                    foreach (string str in blacklist_vehs) {
                        Debug.Write(str + "\n");
                    }
                }), false);
            }

        }
        private bool IsVehicleOnWhitelist(string hash)
        {
            foreach (string str in whitelist_vehs)
            {
                if (str.SequenceEqual(hash))
                {
                    return true;
                }
            }
            return false;
        }
        private bool IsVehicleOnBlacklist(string hash) 
        {
            foreach (string str in blacklist_vehs)
            {
                if (str.SequenceEqual(hash))
                {
                    return true;
                }
            }
            return false;
        }       
        private bool AllSeatsFree(Vehicle veh) 
        {
            if (veh.PassengerCount == 0 && veh.IsSeatFree(VehicleSeat.Driver))
            {
                return true;
            }
            else 
            {
                return false;
            }            
        }
        private bool TryToDeleteAnyVehicleFromThePlayer() {
            for (int i = 0; i < player_vehs.Count; i++) {                
                Vehicle veh = player_vehs.ElementAt(i);
                if (!veh.Exists()) 
                {
                    veh.Delete();
                    player_vehs.RemoveAt(i);
                    return true;
                }
                if (veh == null)
                {
                    player_vehs.RemoveAt(i);
                    return true;
                }
                if (AllSeatsFree(veh))
                {
                    veh.Delete();
                    player_vehs.RemoveAt(i);
                    return true;
                }        
            }
            return false;
        }
        private bool CanPlayerSpawnVeh() 
        {
            if (player_vehs.Count < CFG_max_veh_per_player)
            {
                return true;
            }
            else 
            {
                if (TryToDeleteAnyVehicleFromThePlayer())
                {
                    return true;
                }
                else 
                {
                    return false;
                }
            }          
        
        }

        private async Task<Vehicle> SpawnVehicle(uint model_hash, Vector3 position) 
        {
            int counter = 0;
            API.RequestModel(model_hash);
            while (!API.HasModelLoaded(model_hash)) 
            {
                Debug.Write($"{LG_wait_loading} {counter}/{CFG_max_trys_to_spawn_veh}" + "\n");
                await Delay(100);
                counter++;
                if (counter > CFG_max_trys_to_spawn_veh)
                {
                    return null;
                }
            }
            Vehicle veh = await World.CreateVehicle(new Model((int)model_hash), position);
            return veh;            
        }

        private bool ReadInputAsBool(string data_field)
        {
            return Convert.ToBoolean(Convert.ToInt32(API.GetResourceMetadata(API.GetCurrentResourceName(), data_field, 0)));
        }
        private int ReadInputAsInt(string data_field)
        {
            return Convert.ToInt32(API.GetResourceMetadata(API.GetCurrentResourceName(), data_field, 0));
        }
        private string ReadInputAsString(string data_field)
        {
            return API.GetResourceMetadata(API.GetCurrentResourceName(), data_field, 0);
        }
        private List<string> ReadInputAsList(string data_field)
        {
            List<string> result_list = new List<string>();
            int elem_count = API.GetNumResourceMetadata(API.GetCurrentResourceName(), data_field);
            for (int i = 0; i < elem_count; i++)
            {
                result_list.Add(API.GetResourceMetadata(API.GetCurrentResourceName(), data_field, i));
            }
            return result_list;
        }

    }
}

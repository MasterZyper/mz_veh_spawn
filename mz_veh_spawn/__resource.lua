--[[
mz-vh_spawn, allows you to spawn some Vehicles
Copyright (C) 04.10.2019  MasterZyper 🐦
Contact: masterzyper@reloaded-server.de

You like to get a FiveM-Server? 
Visit ZapHosting*: https://zap-hosting.com/a/17444fc14f5749d607b4ca949eaf305ed50c0837
Support us on Patreon: https://www.patreon.com/gtafivemorg
For help with this Script visit: https://gta-fivem.org/

This program is free software; you can redistribute it and/or modify it under the terms of the 
GNU General Public License as published by the Free Software Foundation; either version 3 of 
the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with this program; 
if not, see <http://www.gnu.org/licenses/>.
]]

resource_manifest_version '44febabe-d386-4d18-afbe-5e627f4af937'
client_script 'mz_veh_spawn.net.dll';

--Cmds:
spawn_cmd 'SpawnVeh'				--Command der eingeben werden muss um eine Fahrzeug zu spawnen, gefolgt von dem VehicleHash
list_whitelist_vehs 'list_whitelist_vehs' --Zeigt im Debug Log eine Info über alle Fahrzeuge an die gespawnt werden dürfen
list_blacklist_vehs 'list_blacklist_vehs'--Zeigt im Debug Log eine Info über alle Fahrzeuge an die nicht gespawnt werden dürfen
--Config:
-- 1 = Aktiviert; 0 = Deaktiviert 
cfg_max_veh_per_player '5'			-- Maximale Anzahl von Fahrzeugen die ein Spieler spawnen darf
cfg_max_trys_to_spawn_veh '80'		-- Maximale Anzahl der Versuch eine Fahrzeug zu spawnen
cfg_use_blacklist '1'				-- Wenn aktiviert dürfen keine Fahrzeuge von der Blacklist gespawnt werden
cfg_use_whitelist '1'				-- Wenn Aktivirt dürfen nur Fahrzeuge von der Whitelist gespawnt werden.

--List of alle Hashes: https://wiki.gtanet.work/index.php?title=Vehicle_Models
whitelist {
	'deluxo',	
	't20',	
	'bmx'	
}

blacklist {
	'jet',	
	'hydra',	
	'lazer'	
}

--Language:
lg_wait_loading 'Warte auf Fahrzeugmodel...'
lg_veh_spawn 'Spawne Fahrzeug'
lg_veh_not_spawned_hash 'Fahrzeug konnte nicht gespawnt werden! Ist der Hash falsch?'
lg_veh_not_spawned_no_veh_free 'Du kannst aktuell keine weiteren Fahrzeuge spawnen. Bitte sorge dafür, dass sich keine Spieler in deinen alten Fahrzeugen befinden.'
lg_veh_on_blacklist 'Fahrzeug konnte nicht gespawnt werden, da es auf der Blacklist steht!'
lg_veh_not_on_whitelist 'Dieses Fahrzeug bfindet sich nicht auf der Whitelist!'
lg_info_whitelist 'Folgende Fahrzeuge dürfen gespawnt werden:'
lg_info_blacklist 'Folgende Fahrzeuge dürfen nicht gespawnt werden:'
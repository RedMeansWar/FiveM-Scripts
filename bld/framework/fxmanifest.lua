fx_version 'cerulean'
game 'gta5'

author 'RedMeansWar'
description 'Framework Script written in C# and Lua'
version '1.0.0'

ui_page 'html/index.html'

files {
	'Newtonsoft.Json.dll',
	'html/index.html',
	'html/script.js',
	'html/style.css',
	'html/imgs/*.png'
}

client_script 'Framework.Client.net.dll'

server_script {
	'Framework.Server.net.dll',
	'fxmanifest.lua'
}

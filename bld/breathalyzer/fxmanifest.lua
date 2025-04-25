fx_version 'cerulean'
game 'gta5'

author 'RedMeansWar'
description 'Breathalyzer Script written in C#'
version '1.0.0'

ui_page 'html/index.html'

files {
	'Newtonsoft.Json.dll',
	'html/index.html',
	'html/script.js',
	'html/style.css',
	'html/imgs/bac.png'
}

client_script 'Breathalyzer.Client.net.dll'
server_script 'Breathalyzer.Server.net.dll'
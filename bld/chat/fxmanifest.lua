fx_version 'cerulean'
game 'gta5'

author 'RedMeansWar'
description 'Chat Script written in C#'
version '1.0.0'

ui_page 'html/index.html'

files {
	'Newtonsoft.Json.dll',
	'html/index.html',
	'html/script.js',
	'html/config.css',
	'html/index.css',
	'html/Message.css',
	'html/Suggestions.css',
	'html/vendor/animate.3.5.2.min.css',
	'html/vendor/vue.2.3.3.min.js'
}

client_script 'Chat.Client.net.dll'
server_script 'Chat.Server.net.dll'
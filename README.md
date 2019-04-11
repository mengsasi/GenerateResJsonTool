# GenerateResJsonTool
egret 生成default.res.json工具

##config.json
根据config中配置生成组

resource: 项目resource根路径
generatelocal: 生成defaultresjson键值命名的资源文件

generateWeb: 生成从网上动态加载的资源文件

#### assetsGroups: 定义组
{
	"folderPath": "assets\\singles",
	"urlRoot": "assets",
	"single": true,
	"des": "single为true 只生成一个组"
},
{
	"folderPath": "assets\\pictures",
	"urlRoot": "assets/pictures",
	"single": false,
	"des": "single为false 此文件夹下每个子文件夹为一个组"
}

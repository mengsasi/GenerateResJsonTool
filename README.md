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

### Tools中有一个0.1.0.txt
本地包中，每次有修改的文件，会记录到此文件中
更新网络res.json时，会将这些文件打进去，同时如果打开ftp上传，会将改动过的文件，上传到ftp
部署到http后，可直接在项目中下载到新的文件

####注：egret中，两个res.json文件中相同key的资源，新的会覆盖旧的。

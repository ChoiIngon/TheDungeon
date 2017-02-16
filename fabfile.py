from fabric.api import run, env, execute, task, runs_once
from fabric.operations import local, put 
import time
import shutil

class DeployInfo :
	def __init__(self) :
		pass

DEPLOY_INFO  = {}
DEPLOY_INFO["dev"] = DeployInfo()
DEPLOY_INFO["dev"].hosts = ["14.63.163.120"]

env.user = 'wered'
env.password = 'w2fpem0576'

def environment(env_name) :
	global DEPLOY_INFO
	env.hosts = DEPLOY_INFO[env_name].hosts

def install() :
	run('mkdir -p public_html')

def assetbundle() : 
	execute(install)
	curdatetime = time.strftime("%Y%m%d%H%M%S")		
	run('mkdir -p public_html/AssetBundles.' + curdatetime)
	put('AssetBundles/*', 'public_html/AssetBundles.' + curdatetime)
	run ('cd public_html; rm -rf AssetBundles; ln -sf AssetBundles.'+curdatetime+' AssetBundles; cd')

def apk() : 
	execute(install)
	curdatetime = time.strftime("%Y%m%d%H%M%S")		
	put('TheDungeon.apk', 'public_html/TheDungeon.' + curdatetime + ".apk")
	run ('cd public_html; rm -rf TheDungeon.apk; ln -sf TheDungeon.'+curdatetime+'.apk TheDungeon.apk; cd')

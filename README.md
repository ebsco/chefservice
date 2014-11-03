#Chef Service


Figured we would need a way to execute a chef-client run on demand from a local context.  The easiest way to control all the variables we need is from a chef windows service that we can easily control

  - Running as correct user
  - Runs at startup based on some magic Registry entry (if needed)
  - Any other possible windows workarounds
  

Most of this idea was duplicated from Cloud-Init for Windows:
http://www.cloudbase.it/cloud-init-for-windows-instances/

---

---

##Install and Uninstall

     
###Install :
 - ChefService.exe -install (Will run as local system)
 - ChefService.exe -install -username %name% -password %password%

###Uninstall :

 - ChefService.exe -uninstall

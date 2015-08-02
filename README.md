#**Components**

##ChefService.exe

Figured we would need a way to execute a chef-client run on demand from a local context.  The easiest way to control all the variables we need is from a chef windows service that we can easily control
  
Most of this idea was duplicated from Cloud-Init for Windows:
http://www.cloudbase.it/cloud-init-for-windows-instances/

* **Install**
 - Just copy the executable and the config file and run the following commands:
	     * ChefService.exe -install (Will run as local system)
         * ChefService.exe -install -username %name% -password %password%
* **Uninstall**
 - ChefService.exe -uninstall

---

#EIS-Chef.exe
 
Will call the Chef Service Web Service and execute a Chef-Client run.  This client will have all the output printed to the console to allow tools like knife winrm work as expected.  It will also follow the native chef-client exit code and exit appropriately.

##Key Features
 * Will accept any parameters that native chef-client accepts.  All parameters will be forwarded on to the native chef-client (ie. -F Min, -l Debug, etc)
 * Redirect output of the chef-client run and will print it to screen.  Should look and feel like calling native chef-client
 * Will take the chef-client exit code, and use it for its own.  Works with all native tools like knife winrm exit code propagation.
 * Coded to wait for chef-client to finish and continually grab new output to allow an alike experience with current chef-client

* **Install**
    * No specific instructions.  Just copy the executable and the config file.

 
#**How to get installed as part of the normal Chef workflow**
##Custom Bootstrap file
* See example bootstrap file located [here](https://github.com/ebsco/chefservice/blob/master/SampleBootstrap/SampleBootstrapTemplate.erb).  This was a custom bootstrap file created to deploy these executables as part of the chef normal bootstrap operation.  The important parts of the code can be seen highlighted [here](https://github.com/ebsco/chefservice/blob/master/SampleBootstrap/SampleBootstrapTemplate.erb#L45-L115).

	* It performs a file download of 4 files
		* EIS-Chef.exe
		* EIS-Chef.exe.config
		* ChefService.exe
		* ChefService.exe.config
	* It also uninstalls any previous version of ChefService and deletes it, as well as installs the new one
		* C:/ChefService.exe -uninstall
		* C:/ChefService.exe -install

##Custom ChefClient command call
*  We call the chef-client command using the newly installed components:
    * **knife winrm -a ipaddress -x MyAdministrator -P MyAdministratorPassword "C:\\eis-chef.exe -F min" 1.2.3.4**



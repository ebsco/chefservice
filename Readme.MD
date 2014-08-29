Chef Service
=========

Figured we would need a way to execute a chef-client run on demand at startup time.  The easiest way to control all the variables we need is from a chef windows service that we can easily control

  - Running as correct user
  - Runs at startup based on some magic Registry entry
  - Will reset after run once.   Allows a seperate receipe to come along and set up for a nother reboot at a later time.
  

Most of this idea was duplicated from Cloud-Init for Windows:
http://www.cloudbase.it/cloud-init-for-windows-instances/
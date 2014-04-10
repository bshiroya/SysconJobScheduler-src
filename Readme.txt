Syscon Job Scheduler Ver 1.1.0.0
----------------------------

Dependencies		--	Version
-------------------------------------------------------

SysconCommon.dll	--	1.3.2.0
StringTemplate.dll	--	3.0.1.6846
PLUSManaged.dll		--	1.10.4.0
antlr.runtime.dll	-- 	2.7.7.3
nunit.framework.dll	--	2.5.7.10213

System.Data.SQLite.dll	--	1.0.79.0

---------------------------------------------------------
Installation and Running Instructions
---------------------------------------------------------

After installation, run the ServiceStart.bat file with admin privilege to start the service.
For successfully running the ServiceInstall.bat the user should have admin privilege.

1.	After running the setup, the user can find a batch file in the installation folder named "ServiceStart.bat". 
	Run this batch file as Administrator to start the service or go to Service.msc and there the "JobSchedulerService" 
	will be listed, start the service.

2.  Run the "JobSchedulerUI.exe" with admin privilege.
 
3.  The jobs will be listed in the DataGrid in the main JobSchedulerUI window. Configure each shown job by clicking on 
	the Config button in each row. time in the job config window and check the 'Enqueue' checkbox to add the job to the 
	scheduler service.


Note:-  For running the Work Order Import Job the system should have a licensed Sage 100 Contractor installed. The Sage.SMB.Api.dll 
		version 19.2.180.0 should be installed in the GAC.
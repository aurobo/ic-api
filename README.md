# Innovic API

Database directory is relative i.e. App_Data folder is used for creating a db locally. When deploying on azure, just make sure that connection string on azure has the same name as the connection string name used in context. Azure connection string will just override the local one after deployment. This results in good DX.
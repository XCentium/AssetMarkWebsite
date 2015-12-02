# AssetMarkWebsite

## Installation
The website runs from the CMSWeb\Website folder. There needs to be a Sitecore Data folder next to in CMSWeb\Data.
Sitecore itself is not included and needs to be installed into CMSWeb\Website\Sitecore.

Some additional assemblies are needed to build the solution. They can be found in ExternalAssemblies and should be placed in Build\bin and CMSWeb\Website\bin.

The original setup guide from AssetMark can be found in "Sitecore Setup Guide For Investment Section Redesign.docx"

The contents of the original zip file sent from AssetMark can be found in "C:\RcTools\Assetmark website september 2015" on xdev02. If you find some files are missing from the repository they can most likely be found there.

## Development server
The development server is xdev02.xcentium.net.
The running website can be found in C:\inetpub\wwwroot\CMSWeb and accessed via assetmark.xcentium.net.

The databases used for the solution are:
- Assetmark_CMSSitecore_Core
- Assetmark_CMSSitecore_Master
- Assetmark_CMSSitecore_Web
- Assetmark_CMSSitecore_analytics

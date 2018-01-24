# Repelsteeltje

Repelsteeltje (English: Rumpelstiltskin) is an application that helps in
deciding on a name. It does this by offering a Tinder like approach where the
user is presented two options at the time.

On top of just preferring  one name over the other, there is also the
possibility of veto a name totally. After all, most names are no valid candidate
at all.

## Configuration
The configuration (.config) file gives the option to specify some preferences.

#### Format
This might help to see the personal name combined with the family name.

#### Id
You should specify an ID. It helps the application separating your preferences
from your spouse's.

#### Name types
Pick one, boys or girls. The application is not built to select both at the same time.

#### File location
------------------
If you want to select together with you partner (but on your own device) you
might want to specify a shared file location.

### Names
At the file location you should have a girls.txt file with the girls names
(one per line), and a boys.txt file with the boys names.

``` xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <appSettings>
    <add key="Format" value="{0} FamilyName"/>
    <add key="Id" value="Some Id"/>
    <add key="NameTypes" value="Girls|Boys"/>
    <add key="FilesLocation" value="C:\Code\Repelsteeltje\test"/>
  </appSettings>
</configuration>
```

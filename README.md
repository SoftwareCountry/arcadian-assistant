# arcadian-assistant

##windows development
###server
1. Configure multiple startup projects: `Arcadia.Assistant.Web` and `Arcadia.Assistant.Server.Console`
2. Allow anonymous requests
    Open `Arcadia.Assistant.Web` project propertties in VS and check `Enable Anonymous Authentication` on Debug tab
3. Allow requests from any origin (IIS Express)
    Open `server/.vs/config/applicationhost.config`
    Find section `sites` and choose the one is used
    Update `bindingInformation` field to match `*:%IIS_Express_port_number%:*`

###mobile android
_For basic android setup instructions check mobile/readme.md_
Rewrite url domain to `10.0.2.2` (same as `localhost` on pc), using right api port


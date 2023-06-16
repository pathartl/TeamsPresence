# TeamsPresence
A .NET console application that will update entities in Home Assistant based on Microsoft Teams status

## Initial Configuration

When you run the application for the first time it will create a sample `config.json` file. To get up an running right away, update the `HomeAssistantUrl` and `HomeAssistantToken` values.

**Note:** You can set the `AppDataRoamingPath` to hard code which user profile is used for `%appdata%`

No changes are required to Home Assistant's config. This application will populate the following sensors by default (identifiers and friendly names can be changed in the `config.json` file):

- `sensor.teams_presence_status`
- `sensor.teams_presence_activity`
- `sensor.teams_presence_camera_app`
- `sensor.teams_presence_camera_status`

Once this step is completed, you should be able to start the application and see changes to your Teams status and call activity get updated both in the console and in Home Assistant.
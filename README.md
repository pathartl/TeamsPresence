# TeamsPresence
A .NET console application that will update entities in Home Assistant based on Microsoft Teams status

## Initial Configuration

When you run the application for the first time it will create a sample `config.json` file. To get up an running right away, update the `HomeAssistantUrl` and `HomeAssistantToken` values.

**Note:** You can set the `AppDataRoamingPath` to hard code which user profile is used for `%appdata%`

Some changes to Home Assistant's config is also needed. Add the following to your `configuration.yaml`:

```yaml
sensor:
    - platform: template
      sensors:
        teams_status:
            friendly_name: "Microsoft Teams status"
            value_template: "{{states('input_text.teams_status')}}"
            icon_template: "{{state_attr('input_text.teams_status','icon')}}"
            unique_id: sensor.teams_status
        teams_activity:
            friendly_name: "Microsoft Teams activity"
            value_template: "{{states('input_text.teams_activity')}}"
            unique_id: sensor.teams_activity

input_text:
    teams_status:
        name: Microsoft Teams Status
        icon: mdi:microsoft-teams
    teams_activity:
        name: Microsoft Teams Activity
        icon: mdi:phone-off
```

Once these steps are completed, you should be able to start the application and see changes to your Teams status and call activity get updated both in the console and in Home Assistant.
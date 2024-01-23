## Changelog

### Version 1.0.0

- Initial release.
- Added English and Spanish missions.
- Introduced dynamic mission generation.
- Implemented terminal commands for easy mission access.

### Version 1.0.1

In this version, the following improvements have been made:

- Fixed some minor bugs.
- Added an in-game notification to indicate when new missions are available, as recommended in [[#4](https://github.com/valentin-marquez/LethalMissions/issues/4)]
- Missions are no longer generated on the company's moon. [[#2](https://github.com/valentin-marquez/LethalMissions/issues/2)]
- Depending on your language preference, you will now need to type either `missions` or `misiones` as both options are no longer available. 

### Version 1.0.4

In this version, the following improvements have been made:

- Fixed Only the host sees the mission. [[#3](https://github.com/valentin-marquez/LethalMissions/issues/3)]
- Fixed Always getting message about range of the factories. [[#6](https://github.com/valentin-marquez/LethalMissions/issues/6)]

### Version 1.0.5


In this version, the following improvements have been made:

- Update LethalAPI.Terminal pre-release version. This fixed the following issues:
    - Terminal Issue: Credits Clipping. [[[#1](https://github.com/valentin-marquez/LethalMissions/issues/1)]]


### Version 1.0.8

The following improvements have been made in this version:

- Update of ``LethalAPI.Terminal`` to ``atomic.terminalapi``, with the arrival of `1.5.0` of TerminalApi the library was improved a lot, so to also be able to reach more users was added as a dependency TerminalApi.


### Version 1.1.0

The following improvements have been made in this version:

- Improve the mission generation system, now the missions are generated in a more dynamic way, and the missions are generated in a more balanced way, now the planet conditions and generated objects are taken into account to avoid impossible missions.
- Added `2` new missions, `Find the scrap` and `RepairValve`.
- Remove logs from log to avoid spamming the console.
- Added Menu in-game to view the missions available (Open with `J` key by default Configurable in the game keybindings).
- Added Configure the reward for each mission in mod config file.
- Added to be able to configure the message of new missions, it can be configured to show the notification and sound only the sound.

### Version 1.1.1

The following improvements have been made in this version:

- Improved the configuration of the notification of available missions, now you only have to configure it between these 3 options `None`, `SoundOnly` or `SoundAndBanner`.
- Added `RandomMode` that ignores the number of configured missions and generates random missions.
- You can no longer open the missions menu while the level is being generated to avoid displaying the empty menu.
- Improved the `RepairValves` mission generation.
- Fixed [Bug] No notification #10
## REWST - F O L D E R S !

### Desktop App
A lightweight .Net Portable App written in VB .Net providing access to your Rewst workflows in a folder view generated from workflow tags, including a term in square brackets at the beginning of the workflow's name.

### Version 1.2 Update Notes

 - Added support for Forms
 - Removed dark mode leftovers (controlled through Windows theme)
 
### Version 1.1 Update Notes

 - Added unhandled exception handling
 - Added error handling for workflow execution error
 - Implemented tag list
 - Commented out code related to attempt at dark mode
 - Added missing icons on expand/collapse menu items
 - Moved Search into View menu with Tag List
 - Added right-click to folder tree
 - Added path textbox for selected tree item
 - Added About Box and updated assembly details
 - New app icon
 - Minor refactoring

### Version 1.0 Disclaimer

This app was originally a throw-away program being used to develop the logic of the algorithm that would convert tags into a folder tree in an environment where I knew how to use the code and could concentrate on working out the issues of the algorithm, because I was struggling with designing the algorithm in Jinja. It was just that by the time I got it working correctly, I kind of liked the result and decided to flesh it out into a real application. Consequently, the code is not what I would consider production-ready in terms of layout and organization, but functionally, it works as intended and really only needs some proper high-level error handling.

As/if I continue to develop this, expect a future version to get the refactor and cleanup it needs. I went through two different object data models for the file system, and three different parsing routines so I'm pretty sure there are some artifacts that can be removed.

The primary purpose in publishing this early version was to provide code transparency for the precompiled app download.
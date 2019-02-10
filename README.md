# Duckify

Have you ever been to a party or to a pub and went "Damn... this music sucks"? Yeah, lot of people did and they used Festify to change that.

Now... Have you ever been to a party or to a pub with bunch of IT people that figured in less then 10 minutes how to cheat Festify? 

**Introducing Duckify**

It is still WIP so don't expect all features to work right now.

## Features

* Play any song from Spotify (there might be support for other platforms later)
* Self hosted - use your own Windows laptop or Raspberry Pi
* Secured (Hopefully, but not just yet)
* Fluent Design
* Running under UWP

### Why UWP

It's simple - it's the only C# framework that supports playback of files under Spotify's DRM since it uses Edge for it's web view insted of IE.
I tried to make it work under WPF, but no luck even after trying bunch of Chromium based alternatives. It's not all bad since becouse of this it can be probably run under Windows IoT on Raspberry Pi (not tested since I don't have Pi B+).

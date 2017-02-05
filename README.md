# CitraTouchControl #

CitraTouchControl offers a multitouch able onscreen overlay for the [Nintendo 3DS Emulator Citra](https://github.com/citra-emu/citra).

This is useful for remote play sessions with Citra from a mobile device or if you just don't wan't to sit in front of your PC to play. 

It will auto adjust to match Citra position and size, onscreen controls' keys can be modified and touch/controls can be toggled on and off.

![#c5f015](http://placehold.it/15/c5f015/000000?text=+) **[See it in Action](http://a.pomf.cat/rqiutm.webm)**

![CitraTouchControl](https://i.imgur.com/u38o2VN.png)
## Requirements ##
* Citra-Qt build ([grab the latest bleeding edge build here](https://github.com/citra-emu/citra-bleeding-edge/releases)) 
* Windows PC with Aero (Windows 7 or later) which runs Citra at decent (>30) framerates
* Mobile device which can handle a massive input network-stream
* fast (>75Mbps) network connection between the host-PC and your mobile device (ideally PC->LAN->Router->5G WLAN->Mobile device)


## Usage ##
1. Start Citra and load up your game.
2. Connect to your host-PC via the Remote Desktop app on your mobile device.
3. Start CitraTouchControl via remote control.
4. Enjoy playing 3DS games on your tablet/phone.


## Configuration ##
1. [Enable RDP on your host PC](http://www.howtogeek.com/howto/windows-vista/turn-on-remote-desktop-in-windows-vista/)
  * Make sure services "TermService" and "SessionEnv" are running if you encounter any problems
2. Start Citra and CitraTouchControl, press the menu button on CitraTouchControl (bottom one) to bring up the configuration menu
  1. Press "Configurate Keys" to bring up the next window and set all your controls there
3. Install [Microsoft Remote Desktop Beta](https://play.google.com/store/apps/details?id=com.microsoft.rdc.android.beta) on your mobile device
4. Start it and add a new Desktop connection to your host PC
  1. Insert the local IP and Windows credentials of your host PC
  2. Enable "Custom display resolution" and set it to "788x1260"
  3. If you want sound then set Sound to "Play sound on device" (this will require additional bandwidth)
![Microsoft Remote Desktop Beta](https://i.imgur.com/0RPYSDD.png)


## Options ##
* Readjust Overlay: will get position and size of Citra again and place overlay accordingly. Use this if the overlay is off.
* Enable Touch: will make the overlay "clickthrough" (buttons will still work)
* Hide controls: will remove all controls except the menu button from the overlay. Use this in combinition with enabled touch for a stylus like experience.
* Touch: Tap only: will make all onscreen buttons "static". If you press a button it will send KeyDown and KeyUp. Holding a button will not send multiple keystrokes anymore. Use this if you encounter involuntary KeyPresses (mostly due to lag).
* Keypress: adjust the delay a Key keeps beeing pressed after you removed your finger from the button. Reduce this if you encounter involuntary KeyPresses (mostly due to lag) or increase it if buttons doesn't get recognised anymore.
* Configurate Keys: set keys for all onscreen controls (only works with a physical keyboard, will not work over RDP from a mobile device)


## Tested Remote Desktop Apps ##
| Platorm | App | Working | Information |
| ------------- | ------------- | ------------- | ------------- |
| Android | [Microsoft Remote Desktop Beta](https://play.google.com/store/apps/details?id=com.microsoft.rdc.android.beta) | **_Yes_** |
| Android | [Microsoft Remote Desktop](https://play.google.com/store/apps/details?id=com.microsoft.rdc.android) | no | no portrait mode |
| Android | [TeamViewer](https://play.google.com/store/apps/details?id=com.teamviewer.teamviewer.market.mobile) | no | no portrait mode, distorted sound |
| Android | [RDP Remote Desktop aFreeRDP](https://play.google.com/store/apps/details?id=com.freerdp.afreerdp) | no | no (multi-)touch |
| Android | [aRDP Pro: Secure RDP Client](https://play.google.com/store/apps/details?id=com.iiordanov.aRDP) | no | no (multi-)touch |
| Android | [Remote Desktop Manager](https://play.google.com/store/apps/details?id=com.devolutions.remotedesktopmanager) | no | no (multi-)touch |
| Android | [Remotix VNC RDP Remote Desktop](https://play.google.com/store/apps/details?id=com.nulana.android.remotix) | no | no (multi-)touch, slow |
| Android | [NoMachine](https://play.google.com/store/apps/details?id=com.nomachine.nxplayer) | no | slow, no portrait mode |
| Android | [Remote Desktop Client](https://play.google.com/store/apps/details?id=com.xtralogic.android.rdpclient) | no  | very slow |


## Contributing ##
If you find any Remote Desktop App which is fast enough for this kind of usage and supports portrait mode (also applies for other platforms like iOS or Windows Phone) then please let me know. Either send me a mail or create an issue here.


## Limitations ##
* Due to the fact that Microsofts RDP display driver does not support OpenGL 3.3 Citra will not work if you start it over a RDP-session. Start Citra and load your game **BEFORE** you connect via RDP.
* Configurating keys only works with a physical keyboard, it will not work via Remote Desktop from a mobile device.
* Overlay only checks for a proces called "citra-qt.exe". If your build of Citra is called something else then you'll have to rename it.
* Overlay will not adjust itself after startup. Use the "Readjust Overlay" option in menu if you move/resize Citra later.
* Overlay will not detect a restart of Citra. You'll have to manually restart CitraTouchControl if you close and reopen Citra.
* If Citra runs in admin mode you'll have to start CitraTouchControl as admin too.
* If your connection has lag >50ms CitraTouchControl will eventually hold KeyDown for too long which will result in recognition of multiple KeyPresses. Try the "Touch: Tap only" option in menu or reduce the KeyPress duration.

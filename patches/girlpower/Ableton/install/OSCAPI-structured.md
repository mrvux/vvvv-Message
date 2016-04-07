Live
=====

/live/next/cue                                                          Jumps to the next cue point
/live/prev/cue                                                          Jumps to the previous cue point
/live/play                                                              Starts the song playing
/live/play/continue                                                     Continues playing the song from the current point
/live/play/selection                                                    Plays the current selection
/live/undo                                                              Requests the song to undo the last action
/live/redo                                                              Requests the song to redo the last action

/live/stop                                                              Stops playing the song
/live/quantization (int quantisation)                                   Set the global quantization. 0=None, 1=8bars, 2=4bars, 3=2bars, 4=bar, 5=half, 6=half triplet, 7=quarter, 8=quarter triplet, 9=8th, 10=8thT, 11=16th, 12=16T, 13=32nd

/live/state				() -> (int overdub, float tempo)				Returns status of the current tempo and overdub
*/live/tempo             () -> (float tempo)                            Set the tempo, replies with /live/tempo (float tempo)
*/live/time              () -> (float time)                             Set the time , replies with /live/time (float time)
*/live/overdub           () -> (int on/off)                             Enables/disables overdub
*/live/play 			() -> (int play) 								2 = playing, 1 = stopped


*/live/master/meter (int 0=left, 1=right) (float value)

=== Scene
/live/play/scene        (int scene)                                     Launches scene number scene

/live/scenes            () -> (int scenecount)                          Returns the total number of scenes in the form /live/scenes (int)
/live/name/scene        (int scene) -> (int scene, string name)         Returns a single scene's name in the form /live/name/scene (int scene, string name)
/live/name/sceneblock   (int track, int size)                           Returns a series of scene name starting at (int scene) of length (int size)

*/live/scene 			() -> (int scene)								Returns the currently selected scene index
/live/name/scene        (int scene, string name)                        Sets scene number scene's name to name

__/live/name/scene        () -> (int scene, string name)                  Returns a a series of all the scene names in the form /live/name/scene (int scene, string name)

==== Master

*/live/master/volume     (int track) -> (int track, float volume(0.0 to 1.0))           Sets the master track's volume to volume (0.0 to 1.0)
*/live/master/pan        (int track) -> (int track, float pan(-1.0 to 1.0))             Sets master track's pan to pan (-1.0 to 1.0)
*/live/master/crossfader () -> (float position)                                Set the crossfader position


*/live/track/meter (int track) (int 0=left, 1=right) (float value)

=== Track

/live/track/crossfader  (int track) -> (int track, int assignment)		Gets the current cross fader assignment for track track. 0 = A, 1 = None, 2 = B
/live/track/crossfader  (int track) (int assign)                        Sets the current cross fader assignment for track track to assign

*/live/arm              (int track) -> (int track, int arm) 			Get arm status for track number track
		                (int track, int armed/disarmed)                	Arms/disamrs track number track
*/live/volume           (int track) -> (int track, floatvolume)		Returns the current volume of track number track as: /live/volume (int track, float volume(0.0 to 1.0))
			            (int track, float volume(0.0 to 1.0))           	Sets track number track's volume to volume
*/live/mute             (int track) -> (int track, int mute)           Get mute status for track number track
			            (int track, int mute/unmute)                    	Mutes/unmutes track number track
*/live/solo             (int track) -> (int track, int solo)           Get solo status for track number track
		                (int track, int solo/unsolo)                    	Solos/unsolos track number track
*/live/pan              (int track) -> (int track, float pan)          Returns the pan of track number track as: /live/pan (int track, float pan(-1.0 to 1.0))
		                (int track, float pan(-1.0 to 1.0))             	Sets track number track's pan to pan


*/live/name/track        (int track) -> (int track, string name, int color)
									                                    Returns a single track's name in the form /live/name/track (int track, string name, int color)


*/live/track


/live/name/track                                                        Returns a a series of all the track names in the form /live/name/track (int track, string name, int color)
*/live/name/track        (int track, string name)                        Sets track number track's name to name

/live/track/jump        (int track, float beats)                        Jumps in track's currently running session clip by beats

_/live/tracks            () -> (int trackcount)                          Returns the total number of tracks in the form /live/tracks (int)

__/live/stop/track        (int track)                                     Stops track number track
__/live/track/info        (int track)                                     Returns clip slot status' for all clips in a track in the form /live/track/info (tracknumber, armed  (clipnumber, state, length))
__/live/name/trackblock   (int track, int size)                           Returns a series of track name starting at (int track) of length (int size)


=== Returns
/live/return/mute       (int track) -> (int track, int mute) 			Get mute status for return track number track
/live/return/solo       (int track) -> (int track, int solo)            Get solo status for return track number track
/live/return/volume     (int track) -> (int track, int volume)          ? Returns the current volume of return track number track as: /live/volume (int track, float volume(0.0 to 1.0))
/live/return/pan        (int track) -> (int track, int pan)             ? Returns the pan of return track number track as: /live/pan (int track, float pan(-1.0 to 1.0))

*/live/return/mute
*/live/return/solo
*/live/return/volume 
*/live/return/pan
*/live/return/meter (int track) (int 0=left, 1=right) (float value)

/live/return/crossfader (int return)                                    Gets the current cross fader assignment for return track track
/live/return/crossfader (int return) (int assign)                       Sets the current cross fader assignment for return track track
/live/return/mute       (int track, int mute/unmute)                    Mutes/unmutes return track number track
/live/return/solo       (int track, int solo/unsolo)                    Solos/unsolos return track number track
/live/return/volume     (int track, float volume(0.0 to 1.0))           Sets return track number track's volume to volume
/live/return/pan        (int track, float pan(-1.0 to 1.0))             Sets return track number track's pan to pan

=== Sends

*/live/send              (int track) -> (int track, int send, float level, int send, ...)
									                                     Returns a list of all sends and values on track number track as: /live/send (int track, int send, float level, int send, ...)
						(int track, int send) -> (int track, int send, float level)
						 							                        Returns the send level of send (send) on track number track as: /live/send (int track, int send, float level(0.0 to 1.0))
			            (int track, int send, float level)  Sets the send (send) of track number (track)'s level to level (0.0 to 1.0)


/live/return/send       (int track)                                     ? Returns a list of all sends and values on return track number track as: /live/send (int track, int send, float level, int send, ...)
/live/return/send       (int track, int send)                           Returns the send level of send (send) on return track number track as: /live/send (int track, int send, float level(0.0 to 1.0))

*/live/return/send
/live/return/send       (int track, int send, float level(0.0 to 1.0))  Sets the send (send) of return track number (track)'s level to (level)


=== Clip
/live/name/clip                                                         Returns a a series of all the clip names in the form /live/name/clip (int track, int clip, string name)
/live/name/clip         (int track, int clip, string name)              Sets clip number clip in track number track's name to name

*/live/name/clip         (int track, int clip)                           Returns a single clip's name in the form /live/name/clip (int clip, string name)
*/live/clip/info         (int track, int clip)                           Gets the status of a single clip in the form  /live/clip/info (tracknumber, clipnumber, state)
                                                                        [state: 0 = no clip, 1 = has clip, 2 = playing, 3 = triggered]

/live/clip/loopstart	(int track, int clip)                           Get the loopstart for clip in track
/live/clip/loopend	    (int track, int clip)                           Get the loopend for clip in track
/live/clip/loopstate    (int track, int clip)                           Get the loop state of clip on track

*/live/clip/position (int track, int clip, float position, float length, float loop_start, float loop_end)

/live/clip/loopstart    (int track, int clip, float loopstart)          Set the loop start position for clip in track
/live/clip/loopend      (int track, int clip, float loopend)            Set the loop end position for clip in track
/live/clip/loopstate    (int track, int clip, int on/off)               Set the loop state of clip on track

/live/clip/loopstart_id	(int track, int clip)                           Get the loopstart for clip in track with the track and clip id /live/clip/loopstart_id (int track, int clip, float start)
/live/clip/loopend_id	(int track, int clip)                           Get the loopend for clip in track with the track and clip id /live/clip/loopend_id (int track, int clip, float end)
/live/clip/loopstate_id (int track, int clip)                           Get the loop state of clip on track with the track and clip id /live/clip/loopstate_id (int track, int clip, int state)

/live/clip/warping      (int track, int clip)                           Gets the warping state of the clip
/live/clip/warping      (int track, int clip, int state)                Sets the warping state of the clip

/live/clip/signature    (int track, int clip)                           Gets the time signature of a clip returns 4 4 for example
/live/clip/signature    (int track, int clip, int denom, int num)       Sets the time signature of a clip

/live/pitch             (int track, int clip)                           Returns the ptich of track number track as: /live/pitch (int track, int clip, int coarse(-48 to 48), int fine (-50 to 50))
/live/pitch             (int track, int clip, int coarse, int fine)                           Sets clip number clip in track number track's pitch to coarse(-48 to 48), fine (-50 to 50)


/live/play/clip         (int track, int clip)                           Launches clip number clip in track number track
/live/play/clipslot     (int track, int clip)                           Launches clip number clip in track number track even if a clip isnt present in the slot (ie stops the slot)
/live/stop/clip         (int track, int clip)                           Stops clip number clip in track number track
                                                                        [state: 0 = no clip, 1 = has clip, 2 = playing, 3 = triggered]
__/live/name/clipblock    (int track, int clip, int sizeX, int sizeY)     Returns a series of clip names in a area starting at (int track, int clip) of size (sizeX, sizeY)




=== View

/live/selection (int tr_offset, int sc_offset, int width, int height)   Sets the dimensions and positions of the highlighted region in session view

/live/scene             (int scene)                                     Selects the scene with index scene

/live/track/view        (int track)                                     Selects a track to view
/live/return/view       (int track)                                     Selects a return track to view
/live/master/view                                                       Selects the master track
/live/track/device/view (int track, int device)                         Selects device on track to view
/live/return/device/view (int track, int device)                        Selects device on return track to view
/live/master/device/view (int device)                                   Selects device on the master track

/live/clip/view         (int track, int clip)                           Selects a clip on track to view
/live/detail/view	(int)						Switches detail view [0 = clip, 1 = track]

*/live/device/selected (int track) (int deviceid)
*/live/return/device/selected (int track) (int device)
*/live/master/device/selected (int device)



=== Devices

*/live/device/param (int track) (int device) (int param) (int value) (str name)
*/live/return/device/param (int track) (int device) (int param) (int value) (str name)
*/live/master/device/param (int device) (int param) (int value) (str name)

/live/master/devicelist                                                 Returns a list of all devices and names on the master track as: /live/device (int device, str name, ...)
/live/devicelist        (int track)                                     Returns a list of all devices and names on track number track as: /live/device (int track, int device, str name,..
/live/return/devicelist (int track)                                     Returns a list of all devices and names on track number track as: /live/device (int track, int device, str name, ...)

/live/device            (int track, int device)                         Returns a list of all parameter values and names on device on track number track
                                                                        as: /live/device/allparam (int track, int device, int parameter int value,  str name, ...)
/live/return/device     (int track, int device)                         Returns a list of all parameter values and names on device on track number track
                                                                        as: /live/device/allparam (int track, int device, int parameter int value,  str name, ...)
/live/master/device     (int device)                                    Returns a list of all parameter values and names on device on the master track
                                                                        as: /live/device (int device, int parameter int value,  str name, ...)


/live/device            (int track, int device, int parameter,          Sets parameter on device on track number track to value
                        int value) 
/live/return/device     (int track, int device, int parameter,          Sets parameter on device on track number track to value
                        int value) 
/live/master/device     (int device, int parameter, int value)          Sets parameter on device on track number track to value


/live/device/range      (int track, int device, int parameter)          Returns the min and max value of parameter of device on track in the format /live/device/range (int track, int device, int/float min, int/float max)
/live/return/device/range (int track, int device, int parameter)        Returns the min and max value of parameter of device on return track in the format /live/return/device/range (int track, int device, int/float min, int/float max)                        
/live/return/device/range (int device, int parameter)                   Returns the min and max value of parameter of device on the master track in the format /live/master/device/range (int device, int/float min, int/float max)          


__/live/master/device     (int device, int parameter)                     Returns the name and value of parameter on device on the master track as: /live/device (int device, int parameter, int value)

__/live/device            (int track, int device, int parameter)          Returns the name and value of parameter on device on track as: /live/device/param (int track, int device, int paarmeter, int value, str name)
__/live/return/device     (int track, int device, int parameter)          Returns the name and value of parameter on device on track as: /live/device/param (int track, int device, int parameter, int value)
__/live/return/device/range (int track, int device)                       Returns the min and max value of all parameters of device on return track in the format /live/return/device/range (int track, int device, int/float min, int/float max, ...)

__/live/device/range      (int track, int device)                         Returns the min and max value of all parameters of device on track in the format /live/device/range (int track, int device, int/float min, int/float max, ...)
__/live/return/device/range (int device)                                  Returns the min and max value of all parameters of device on the master track in the format /live/master/device/range (int device, int/float min, int/float max, ...)



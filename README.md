Unity-Free-Flight
=================

A Unity Package to add gliding flight to any Unity Object.

###Description

Have you ever wanted to create a game where you can fly over the precipitous mountainsides? Perhaps you want to show the world from a birds-eye perspective. Or, maybe your game has nothing to do with flight, you just want various birds to systematically poop on everything in a realistic fashion. Now you can!

Unity Free Flight aims to add a realistic layer of flight dynamics to Unity Objects. That is, you want something to fly? Add this script to the object, and now it can! 

It's true, there are plenty of flight simulators out there already. And maybe [they are what you're looking for](http://unityfs.chris-cheetham.com/). But most flight simulators are geared towards powered flight. That is, flying fighter jets or metal crafts. This Free Flight package is intended for slow moving craft, or winged creatures such as birds. Such objects are also at the [mercy of wind](http://www.youtube.com/watch?v=EV6dLtBJVFQ), [thermals](http://www.youtube.com/watch?v=HV5w8EmqV5c)([what are those?](http://en.wikipedia.org/wiki/Thermal)), or [thunder storm gust fronts](http://youtu.be/RkD4u6sW0LU?t=4m25s). 

Slow moving flight allows for some [incredible perspective](http://www.paraglidinghd.com/urban-side-paragliding/).

###Version Releases

You can download try out the latest stable release [here](http://windwardproductions.org/projects/UnityFreeFlight/downloads/)

####Version 0.1.x -- Codename *Paper Airplane*

Version 0.1.x will focus on flight physics, and basic usability. 

* Version 0.1.0
	* Basic free flight possible, with real world simulated physics
	* Properties of flight objects are highly configurable, with weight, wingspan, wingchord, aspect ratio, and much more
* Version 0.1.1
	* Added paper plane model to scene
	* Example now uses a third person perspective camera
	* Basic Flight Controllers added
	* A menu system has been added
* Version 0.1.2
	* An options menu added, controls can be inverted
	* Flight controllers are better
* Version 0.1.3
	* Menu options added
	* Flight statistic information improved
	* Small cosmetic improvements
	* Many backend changes and refactors, in preparation for version 0.2!
* Version 0.1.3R2
	* Bug fix for Windows release only.

	
####Version 0.2.x -- Codename *Grounded*

Version 0.2.x will focus on extending flight controls to include flapping, flaring, and wing-fold diving. Ground controllers will also be introduced, to enable walking/running as well as transition to flight.

* Version 0.2.0
	* New Scene! First Person View, where the player now starts on the ground
	* Brand new controls! (Kinda)
	* Player can now takeoff and land!
	* Introduction of simple flapping
* Version 0.2.1
	* Free Flight dev package is much more stable -- right out of the box
	* Added more intuitive default support to Free Flight dev package
	* Added smooth transition for landings
	* Fixed "fall through world" bug
* Version 0.2.2
	* Added Flight Mechanics!
		* created a system by which developers can manipulate flight physics
		* Flaring flight mechanic added
		* Diving flight mechanic added
		* (Note: Above two mechanics are not fully developed yet)
	* Improved the flapping system
	* Now support more complex, multi-script ground controllers
* Version 0.2.3
	* Added new controls
	* Added a tutorial for learning all of the controls
	* Made the world more beautiful
	* Improved the flaring mechanic 

####Version 0.3.x

Version 0.3.x will add various sounds, such as flapping and wind

####Version 0.4.x

Version 0.4.x will focus on making flight animations easily addable and configurable. 


###Future Features 

There is no support for the following features, but they should be given attention after the above versions are completed. Order isn't significant, and you should speak up if you want some particular feature added first. 

* Wind
	* Dynamic wind-map generation, for any Unity Terrain
* thermals
	* Dynamic Thermal generation, for Terrains
* Mobile Support
	* Physics optimized for small devices

###Contact

Ideas? Suggestions? I would love to hear your feedback! Leave a message at the [contact page](http://windwardproductions.org/contact/).

#Free Flight -- Alpha Testing -- Version 0.2.3

##Information

* Date: Aug 3rd, 2014
* Version: 0.2.3

###Purpose

The main purpose of the Alpha test was to test the Trigger system, and the overall effect the new tutorial had on players. The secondary purpose was to test the new control system, implemented since version 0.2.2. And lastly, the tertiary purpose was to understand how players reacted in a world given no objectives (What did players *do* most of all? What was significant to them? What was it about flight that interested them?). 

###Goals

1. Find out if the tutorial contributed to player understanding of controls
2. Deduce how well players intuitively understand the controls
	* Find out the assumptions players make about the controls
		* Extra credit: Find out why they made these assumptions!
3. Track how long it takes players to become familiar with controls
4. Track what players do once they're familiar with the controls
	* This also means tracking what joy players take from the game

###Time

Meeting was supposed to take approx. 15 minutes. Testing went about 20-25, and the meeting over-extended to about 3 hours and 15 minutes.

###Results

Below summarizes the outcome of the testing. See the testing notes below for more information.

####1. The tutorial

Success! Got information on how the tutorial worked for veteran players!

In this case, the tutorial wasn't all that helpful. For the simple mechanics of walking around, the players already knew how. The tutorial just got in the way here. 

However, for the more complex in-flight mechanics, both players needed the tutorial in order to do the maneuvers. It still failed for a number of reasons(see below). Need more clear wording in the tutorial, in addition to a more carefully planned place for players to execute maneuvers without mountains or other variables affecting them.

####2. Controls

Before I say anything, I should note the preconceptions (thanks J). When players start the game without any expectations for what they will be doing, they make up their own expectations. For example, some players will pretend to be a bird, some a plane, others a giant hunk of floating cheese. They make up wild and unpredictable expectations that may have no connection to how the game actually plays. 

There are huge ramifications when this is applied to controls. The bird player expects the controls "W + Space" to launch forward, while the plane player expects the same controls to pitch down towards the ground. Who knows what the giant hunk of cheese is trying to accomplish. When allowing players to fill in this expectation for themselves, it can be wild and unpredictable, which makes having the player guess the game's control scheme a crap shoot. 
 
That being said, the current control scheme had problems:

* The flare mechanic wasn't well understood, and diving wasn't all that different from simply turning.
* The "W + Space" maneuver wasn't expected, and caused summersaults. 
* Players consistently pitched too far forward when in flight, causing them to loose substantial altitude. 
* Players consistently turned too far (roll and pitch) locking them into a death-spiral. 

####3. Understanding controls

Depends on what the players were trying to accomplish, see above problem #2. 

Players were typically able to understand controls after a few minutes well enough to navigate the world. The advanced maneuvers, diving and flaring, were still pretty unusable.

####4. Player's actions

Again, diverting to problem #2. It depends what the players thought of themselves when playing the game. 

I was surprised when one player said she derives Sense Pleasure (from [Aesthetics of Play](https://www.google.com/?gws_rd=ssl#q=Aesthetics+of+Play)), from the wild angles and heights. She even experienced motion sickness from a death-spiral. It was interesting to hear that most of the pleasure she got was actually form the last version in which the controls gave her more a feeling of flying, compared to this version which creates a feeling of falling. No physics were changed, that's only from the controls. 



##Testing

Before playing, both players were told to go download the game. Neither were given expectations for how the game should be played. Both of them had been players before in version 0.1.x, and so had a previous understanding for a different control set. This made things a bit more interesting, since things had changed a lot since then.

Both are consistent video game players with lots of experience. 

###Player J.S.

Player started the game, but didn't seem to notice the tutorial overhead, *or*, already knew the controls from past experience, making the tutorial information not useful. She had trouble with the basic ground controls being very sensitive at zero movement (which seems pretty universal problem with people). 

On flight, she did an immediate summersault on takeoff. This was because she expected to do a running start before takeoff. This actually happened several times, to the point she fell thorough the world (a feat I thought I had made impossible). 

Once in flight, she told me she saw the tutorial, but didn't understand what it was telling her. Also, the flightpath given to the player on startup doesn't force them to be in a good position for doing in-flight maneuvers; They're trying to dodge mountains around the time the tutorial tells them to barrel roll. Moreover, the tutorial explains things in multiple chunks. All of this contributes to information overload. 

####Notes

* tutorial didn't make sense, information overload
* tutorial didn't give the correct information when needed
	* Specifically, telling the user to "press button X" was much more helpful than saying, "Execute a dive". 
* Several times, she would press W + Space, run forward, and fly directly into the ground.
	* This was because she expected a running + jump/fly strategy to work
	* Fell through world
* Flapping doesn't make you go higher!
	* note that this makes flight very "hard"
* Subjective feeling from the game is terrifying, vs the feeling in the older versions, which was a jolt of amazement at being able to fly. This is because it's hard to gain altitude

* Expectations of controls for what you're flying. 

###Player P.M.

Player started the game, faltered with controls, but understood them after a while. He seemed to understand what the tutorial was telling him, but it took a while to piece things together. The ground controller gave him trouble as well. 

Understood flight after a few minutes, and was flying around normally afterwards. Had lots of trouble with the flare mechanic, and couldn't understand what was actually happening. His thinking was that it stops all forward momentum, and simply drops the player (which was exactly what was happening, but it wasn't clear when or why that would be useful).

####Notes

* Hard to get a sense of scale
* Want to look around while you're flying around and unable to
* Flap should always push you up, but it doesn't
* Shift looses directional control
* Shift stops forward momentum, but not vertical momentum
* First person vs third person: 
	* Can't see what the wing is doing in first person
* recommends: controls like the scorpion tank in halo. Lets you move in any
	direction you want to, and look in a completely different direction.

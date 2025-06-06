# Lee: Hyperreal - A Character Mod for Risk of Rain 2
## Changelog

- v2.1.0 -> Bug bashing + QoL
    - Added new animation for Sprinting when movespeed is high enough (aka Super Sprint)
    - Added Base Shield Stat
        - Should make it easier to engage with enemies
        - 120HP to 100HP/30SHIELD 10 shield per level
    - Set Luminous Shot Charges to build up on Snipe Stance Shot (Primary in snipe stance)
        - Primary non-snipe stance should use these charges
    - Added option to disable the Ultimate Camera animation
    - Changed Notification Style on Hyper Effects
    - Walking doesnt stutter step you at the end anymore
    - Slightly better hunkhud support
    - Slightly enhanced Orb animations
    - 20% more Ultimate pull-in range
    - Fixed some item displays from not dissapearing
    - Fixed Input priority and Move cancels preventing major issues like orbs being eaten and not performing the move
    - Recompiled with new MMHOOK to use appropriate function calls
    - Blue Orb can cancel faster into Primary, move cancelling this has not changed in timing.
- v2.0.5
    - oops (dont worry about it this never happened)
- v2.0.4
    - Full Hunk Hud Support
    - Update for Phase 3
- v2.0.3
    - Changed the sizes of some hitboxes on Primary
    - Changed the wording on some descriptions
    - Fixing some unlocks from breaking due to some incorrect naming
- v2.0.2
    - Added option to remove blue colouring on name (not in descriptions).
    - Fixed No-input dodge from moving weird.
    - Fixed some particles from blinding players
    - Updated RoR2 GameLibs
    - Added Item Displays for Seekers of the Storm
- v2.0.1
    - ExtraSkillSlots has been made mandatory, as there are some weird quirks with how the game loads with HIFU's Inferno mod.
    - Added on-hit snipe particles that were missing.
    - Changed the colours on RoR-Skins for the weapons.
    - Reduced Emission on all skins
    - Updated README with screenshots
    - Reduced brightness on some effects
- v2.0.0
    - Lore mode has been added!
        - By default, Lore mode is on! This changes the following:
            - Voice lines are disabled by default
                - Note: In a cease event, the voiceline will still play
            - RoR2 skins are moved to the front, PGR skins are moved to the back of the list.
            - A randomly generated RoR2 fitting name is used instead of Lee: Hyperreal
            - This can be disabled, but please ensure that people you're playing with have this option synchronised!
                - Disabling reverts the aforementioned changes
    - Achievements
        - Added achievements to unlock skins! There are a total of 7 to unlock
            - If you don't want to unlock it manually, just use Cheat unlocks, we won't shame you
        - <strike>Added Gacha mechanics for unlocks into RoR2</strike>
    - Skins
        - RoR2-like skins have now been added! (Credits to Brynzananas)
    - VFX
        - A unique color is assigned for most of the skins, which changes every VFX colour for that skin
        - Some clones were added in for Hypermatrix Entry
        - New End of Time VFX has been added.
    - Miscellaneous Bug fixes and changes:
        - Fixed some animators from breaking between UI disables
        - Parrying from Primary 3 now chains into Primary 3 instead of Primary 1
        - Dodging should reduce your falling speed when you exit the state.
        - ...And some other fixes I forgot to write
- 1.1.14
    - Primary 4 will now flash between invisible and visible during the move
    - Actually fixing the overlay from not disappearing properly on counters and ultimate freezes
- 1.1.13
    - Added a slight bounce on P5 end
    - Fixed RiskUI affecting the Orb list from glitching between positions during health changes
    - Fixed Overlay from persisting on big parry
    - Fixed double jump platform from sticking to Lee's feet during the effect lifetime
    - Reinitialied the crosshair on Ult end to fix the animator issues on UI objects
    - Reduced instances of Ultimate being eaten up by other inputs during other abilities.
- 1.1.12
    - Recompiled to use new method signatures
    - Added Damage Type changes from new RoR2 version.
        - Primary triggers primary effects.
        - However, when in snipe mode, Primary does secondary effects. 
            - Luminous shot triggers secondary off activation, not on hit, so these damage changes do nothing.
            - However sniping does not trigger the lightning bolt on hit.
            - If this causes issues down the line, I will revert this to use primary.
- 1.1.11
    - Fixed an issue regarding Non-simple control inputs where orb groupings greater than 3 would not trigger 3-ping effects
        - 3-ping effects include:
            - Stronger orb effects
            - Coloured Bullet retrieval
        - These should now run properly if using Non-simple control inputs.
        - Simple mode was not affected in anyway.
- 1.1.10
    - Added more fallbacks to prevent lockups on orb inputs.
    - Changed Armament Barrage 4 to deal uniform damage regardless of distance.
    - Updated dependencies to reduce incompatibilities.
- 1.1.9
    - Fixed Lee hitting himself when doing a blast attack with Artifact of Chaos enabled.
- 1.1.8 
    - Added input reset on yellow orb skill to prevent lockups. 
        - Thanks to .score for finding this bug.
- 1.1.7
    - Added Stun on hit for the following moves:
        - Armament Barrage 3
        - Armament Barrage 5
        - Armament Barrage Aerial Slam
        - Armament Barrage Hypermatrix Aerial Slam
    - Added some logic to allow the character to be played even if the UI doesn't display.
- 1.1.6
    - Added the option to remove Alt orb trigger and opt for 8 separate orb buttons in non-simple mode.
        - If you wish to use this mode, Uncheck simple mode, Uncheck Orb Alt trigger and setup the buttons for 1-8.
    - Added a failsafe for Orbs when unable to be triggered for longer than 3 seconds. 
    - Removed log spam on first stage from Lee's pod.
- 1.1.5
    - Added a list of states that prevent freeze from occurring on some enemies
        - An example of this: Mithrix Item steal was being cancelled and the items would not be applied to either you or Mithrix. This state should be played out entirely.
- 1.1.4
    - Added the ability to cancel Snipe stance by sprinting 
        - RTAutoSprint should not affect this, however if it does, let me know!
    - Added the ability to cancel Snipe stance by jumping.
        - If you don't have any more jumps, you will simply cancel and not jump.
    - Snipe Entry and Exit animations now scale with attack speed.
    - Fixed using Reality Travel from not running when Sniping.
    - Armament Barrage now scales with attack speed up to 135%.
        - This max attack speed is achieved when reaching 250% attack speed.
        - The timing for Parry is not changed even when attacking faster.
    - Reality Travel now scales with movement speed up to 250%
        - This max movement speed is achieved when reaching 2500% movement speed.
        - Timing for invincibility should not be affected.
    - Reality Travel now scales with attack speed
    - Armament Barrage 4 scales with move speed
        - Each pulse allows Lee to travel further with more move speed.
        - Scaling is capped similar to Reality Travel.
- 1.1.3
    - Removing warning on README. (LITERAL README UPDATE AAAGH)
- 1.1.2
    - Fixed an issue making enemies become invincible if they were frozen during their death animation.
    - Added support for Betterhudlite.
- 1.1.1
    - Fixed the item notification popup from duplicating.
    - Removed Extra skill slots as a Hard dependency, can be now included optionally if needed
    - Renamed skill families appropriately.
- 1.1.0
    - Updated Dependencies to support SotS
        - Unfortunately, controller players will not be able to use orbs as we have removed the dependency on ExtraSkillSlots until further notice.
    - Removed dependency on R2API.Sound, was not being used.
    - Fixed an edge case where Hypermatrix Aerial Attacks play the particle stream infinitely due to Hypermatrix ending while in an aerial dive.
	- Fixed the pod from blowing out your eyes on some maps.
	- P2 now behaves more closely to source material
	    - 3 -> 5 hits 
        - 125% -> 70% damage
- 1.0.4
    - Fixed the pod from not being registered on the Network properly.
    - Fixed an issue regarding client's getting spawned in without a pod, causing the server and clients to desync from what would be expected.
    - Fixed the wrong dependency from being compiled in the mod, should now work with the suggest mods in the Thunderstore manifest.
    - Changed the UI Instantiation to use Find when navigating the RoR HUD, instead of indexes. Should be more compatible with mods that modify the UI.
    - Changed the values on the following moves:
        - End of Time cooldown -> 4s to 6s per stack. (Total 60s for 10 stacks)
        - End of Time damage -> 5000% to 6500%
        - End of Time enemy freeze duration -> 8s to 6.9s (This should allow the enemies to start moving 0.2s after the move has ended.)
        - Red Orb Hypermatrix damage -> 175% to 155%
- 1.0.3
    - Removing Debug Chat output that was left in accidentally.
- 1.0.2
    - Following changes have been added to Invincibility and Parry:
        - Invincibility granted from Utility and Orbs now do not allow you to bypass shrines that cost health.
        - Parrying a shrine that costs health now inflicts half the damage it would normally do. 
        - Parrying now returns 25% of the damage that would've been inflicted if you didn't parry.
    - Changed the values on the following moves:
        - Armament Barrage 1 damage -> 150% to 100%
        - Armament Barrage 2 damage -> 200% to 125%
        - Armament Barrage 3 damage -> 300% to 225%
        - Armament Barrage 4 damage -> 100% to 70% per tick
        - Armament Barrage 4 Hit Radius -> 20m to 25m
        - Armament Barrage 5 damage -> 1000% to 600%
        - Armament Barrage Aerial (Non-Hypermatrix) damage -> 500% to 400%
        - Blue Orb Blast damage -> 300% to 225%
        - Blue Orb Shot damage -> 400% to 300%
        - Yellow Orb damage -> 100% to 45%
        - Yellow Orb 3-ping damage -> 1000% to 800%
        - Yellow Orb Hypermatrix damage -> 50% to 45% per hit
        - Red Orb damage -> 150% to 125% per shot
        - Red Orb 3-ping damage -> 600% to 500%
        - Red Orb Hypermatrix damage -> 200% to 175%
        - End of time damage -> 6000% to 5000%
        - Collapsing Realm damage -> 2000% to 1500%
- 1.0.1
    - Fixing soundbank from not loading. 
- 1.0.0
    - Initial Release!
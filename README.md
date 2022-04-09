# little_might
Old-school looking survival game made in Monogame/XNA.

# Building the Project
This project uses Monogame. To install it:

1. [follow these steps](https://docs.monogame.net/articles/getting_started/1_setting_up_your_development_environment_windows.html) from their site.
2. Install the content editor, [found here](https://docs.monogame.net/articles/tools/mgcb_editor.html). To allow you to edit and build the content for the game.
3. install [Monogame Extended](https://www.monogameextended.net/). Which is a separate party library I'm using to help with camera position and rendering.

Then run that mgcb editor you installed in **step 2** in visual studio and "build" the content for the first time. After that you can build the game normally/as you would in visual studio.

# Callouts
Selecting game difficulties bigger than 2 may result in some significant load time. This is because the noise map is generating in the background and there is no current visual cues to indicate that the game hasn't crashed and is indeed still loading.

The ``ItemData.xml`` and ``MonsterData.xml`` files live directly in the debug build folder, NOT in the solution. This is because I haven't prioritized figuring out how to get the mgcb editor to load in data files yet so they're not building/reading any files put into the content folder that weren't put there using that editor. So if you want to edit items or monster info, you'll have to open the raw file that lives in the debug build folder. Sorry :D.

# Crafting
There is crafting in the game, but it's lightweight right now. There aren't any books or anything to tell you what to craft, so I'm just going to build a small table of what's available today so you can make some stuff.

| Item 1 | Item 2 | Item 3 | Item 4 | Crafted Item |
|--------|--------|--------|--------|--------------|
|Stone   |Stone   |Goop    |Campfire|Furnace       |
|Flint   |Stick   |        |        |Campfire      |
|Twine   |Stick   |Stone   |        |Stonesword    |
|Flint   |Pelt    |        |        |Leather Set   |

# General Play Advice
If you have a disease, pick up some herbs in the grass land and eat them, each herb cures one of the common illnesses you can get.

Becareful when drinking water, you can get sick. Slimes are poisonous. Apples are your best friend. Chests drop any type of common items. There is only one dungeon in the game and it's crawling with goblins that don't reward you for killing them (yet). The dungeon is randomly placed but it's map is a preset.

There is an extremely dangerous monster in the forest, you cannot fight him yet, don't try.

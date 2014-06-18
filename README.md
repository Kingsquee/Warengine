warengine
=======

...is an Advance Wars style framework built atop Unity3D. Currently dead as I have come to despise proprietary one-size-fits-all game engines.

The code is readable and should be suitable for continued development. It just won't be by me. 

Features
------------
+ Basic movement.
+ Basic combat.
+ WIP level editor.
+ All object types are runtime-editable. 

Fixer Uppers
------------------
+ Add teams.
+ Make the lookup tables be able to be uploaded / downloaded into a remote SQL database, for distributed balancing and testing.
+ The board is currently a collection of tile meshes. They should probably be merged into single meshes, by type, for increased performance.
+ There should probably be a UIManager, rather than each manager trying to handle it's own UI.

License
-----------
All warengine code is licensed under the [WTFPL] (http://www.wtfpl.net/txt/copying/). Its included libraries fall under their respective licenses.

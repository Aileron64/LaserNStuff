To PolyReduce any of the Voxel Meshes, simply select the desired Prefab-Instance in the Hierachy. 
Then Chose "Tools/MagicaVoxel PolyReduce" from the top menu bar. Alternatively you can just hit "Alt+C", 
which is the default hotkey.

The reduction process then analyzes the MeshFilter contents and produces an optimized version of the geometry. 
This version is then saved in the Project window as an ".asset" and linked into the MeshFilter as the standard mesh.

At the end of the conversion process, a status is printed into the console to show how many polygons were reduced. 
Also, a UV2 set and vertex colors are prepared for the proper lightmapping of the new mesh.

If you hit the menu command unintentionally, just hit "CTRL-Z" to undo.
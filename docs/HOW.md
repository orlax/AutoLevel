# How does this work?


This is a tool intended to turn flat 3d meshes into 3d Maps that are heavily customizable

The current approach: 

- take a Probuilder mesh and load its vertex and edges information
- remain only with shared-vertixes and outside edges
- spawn prefabs on each point and line 
- each spawned prefab implements its own logic to populate it self with one or more childs. 
- each spawned point or line can be further configured. 
- this configurations are saved back to the original probuilder mesh data in some form (via Color, normals or other properties.)
- on subsequent re-generations of a level we load this data to keep lines and points configured.

for example, you set a line as a door and it spawns a door in the center and walls to the sides. 


```jsx
LEVEL BUILDER DATA TABLE: 

vertex Color:
R: LineType - applied only to one of the vertices of an edge the "from"
G: PointType - determines what object is in this corner.
B: Info ID - a reference to a LevelBuilderInfo object that is a way to save a lot more qualities about the auto generated level. 
A: Ignore - if this value is 0 the vertex is ignored when generating



a value of 0 on vertex color, will render as "empty" no matter what
the prefab object is configured as.
```

Info ID: 
for example, we can "flip" the scale or save the "Required Item" that a door with key requires. 
to do the later we need a class that implements the ILevelBuilderInfoGetter interface so that when the line prefab object its created
it can receive the Instance of the Info object and configure it self acordingly. 
after making changes in the instance you should call a "save" function in the class so that you save your changes to the info object
that is saved at the AutoLevel Main object.

## Ideas

- Once we are "sure" a level has reached it's final form we can start "baking" the prefabs for performance.
- develop a way to attach extra objects or special features to a certain vertex (give it a custom ID for example.) so that we can drive some custom prefabs positions using this tool

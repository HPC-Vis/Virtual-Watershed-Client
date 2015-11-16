#pragma strict
private var TerrainComposer: GameObject;
private var tc_script: terraincomposer_save;

private var TerrainComposerClone: GameObject;
private var tc_script2: terraincomposer_save;

private var frames: float;	
private var currentOutput: int = 0;

var generateOnStart: boolean = true;
var createTerrainsOnTheFly: boolean = false;
var autoSpeed: boolean = true;
var targetFrames: int = 90;
var generateSpeed: int = 500;
var heightmapOutput: boolean = false;
var splatOutput: boolean = false;
var treeOutput: boolean = false;
var grassOutput: boolean = false;
var objectOutput: boolean = false;

var seed: int = 10; 
var randomSeed: boolean = false;
var randomizeHeightmapOutput: boolean = false;
var randomizeHeightmapRange: Vector2 = new Vector2(0,1000);
var randomizeTreeOutput: boolean = false;
var randomizeTreeRange: Vector2 = new Vector2(0,1000);
var randomizeGrassOutput: boolean = false;
var randomizeGrassRange: Vector2 = new Vector2(0,1000);
var randomizeObjectOutput: boolean = false;
var randomizeObjectRange: Vector2 = new Vector2(0,1000);

private var myStyle: GUIStyle;

function Start () 
{
	TerrainComposer = GameObject.Find("TerrainComposer_Save");
	tc_script = TerrainComposer.GetComponent(terraincomposer_save);
	
	tc_script.heightmap_output = false;
	tc_script.splat_output = false; 
	
	myStyle = new GUIStyle();

	if (generateOnStart) GenerateStart();
}

function GenerateUpdate () 
{
	frames = 1/Time.deltaTime;
	
	if (tc_script2)
	{
		while (tc_script2.generate)
		{
			tc_script2.generate_output(tc_script2.prelayers[0]);
			yield;
		}
		
		// generation is ready so the clone can be destroyed
		//TerrainsFlush();
	}
}

function GenerateStart()
{
	currentOutput = 0;
	
	if (createTerrainsOnTheFly) {
		tc_script.create_terrain(tc_script.terrains[0],tc_script.terrainTiles);
		tc_script.fit_terrain_tiles(tc_script.terrains[0],true);
		ParentTerrains();
	}
	
	while (SelectCloneOutput()) {
		tc_script2.generate = true;
		GenerateUpdate();
		while (tc_script2.generate) {
			yield;
		}
		if (tc_script2.splat_output) tc_script2.stitch_splatmap();
	}
	GenerateStop();
}

function GenerateStop()
{
	Destroy (TerrainComposerClone);
}

function SelectCloneOutput(): boolean
{
	tc_script.disable_outputs();
	if (randomSeed) seed = Time.realtimeSinceStartup*2000;
	if (currentOutput == 0) {
		currentOutput = 1;
		if (heightmapOutput) {
			tc_script.heightmap_output = true;
			CreateClone();
			if (randomizeHeightmapOutput) tc_script2.randomize_layer_offset(layer_output_enum.heightmap,randomizeHeightmapRange,seed);
			return true;
		}
	}
	if (currentOutput == 1) {
		currentOutput = 2;
		if (splatOutput) {
			tc_script.splat_output = true;
			CreateClone();
			return true;
		} 
	}
	if (currentOutput == 2) {
		currentOutput = 3;
		if (treeOutput) {tc_script.tree_output = true;CreateClone();return true;}
	}
	if (currentOutput == 3) {
		currentOutput = 4;
		if (grassOutput) {tc_script.grass_output = true;CreateClone();return true;}
	}
	if (currentOutput == 4) {
		currentOutput = 5;
		if (objectOutput) {tc_script.object_output = true;CreateClone();return true;}
	}

	// TerrainsFlush();
	return false;
}

function CreateClone()
{
	if (tc_script2) GenerateStop();
	
	TerrainComposerClone = Instantiate(TerrainComposer);
	TerrainComposerClone.name = "<Generating>";
	tc_script2 = TerrainComposerClone.GetComponent(terraincomposer_save);
	tc_script2.script_base = tc_script;

	tc_script2.auto_speed = autoSpeed;
	tc_script2.generate_speed = generateSpeed;
	tc_script2.target_frame = targetFrames;
	tc_script2.runtime = true;
		
	tc_script2.generate_begin();
}

function TerrainsFlush()
{
	for (var count_terrain: int = 0;count_terrain < tc_script.terrains.Count;++count_terrain) {
		tc_script.terrains[count_terrain].terrain.Flush();
	}
	Debug.Log("Flushed!");
}

function ParentTerrains()
{
	var terrains: GameObject = new GameObject();
	terrains.name = "_Terrains";

	for (var count_terrain: int = 0;count_terrain < tc_script.terrains.Count;++count_terrain) {
		tc_script.terrains[count_terrain].terrain.transform.parent = terrains.transform;
	}
}

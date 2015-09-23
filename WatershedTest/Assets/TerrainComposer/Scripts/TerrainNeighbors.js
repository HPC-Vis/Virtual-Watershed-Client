#pragma strict

var left: Terrain;
var top: Terrain;
var right: Terrain;
var bottom: Terrain;

function Start () 
{
	var terrain: Terrain;
	terrain = GetComponent(Terrain);
	terrain.SetNeighbors(left,top,right,bottom);
}


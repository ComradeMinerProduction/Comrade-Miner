using UnityEngine;
using System.Collections;

public class prefabasteroid : MonoBehaviour {

    /// <summary>
    /// / some number of roids
    /// let player know where they are
    /// closest to the station - least caluable
    /// further = better ; found by radar
    ///  ???  giant asteroids vs small ones
    ///  100% may delete field / maybe deleted by the player
    ///  
    /// generate one level - create a new scene
    /// eempty game objects for each asteroid
    /// randomize number of asteroids and particles - define the number and distribute randomly ?
    /// 
    /// 
    /// fill in with procedurally generated roids
    /// 
    /// 
    /// array of prefabs
    /// /  save objects
    /// 
    /// close to the sun, differnt types
    /// random percents within a range based on distance from the sun
    /// size of roids based on distance
    /// 
    /// 
    /// tiers of metals
    /// 
    /// large size of field
    /// minimap generates locations of important roids and stations
    /// players should take some time to fly to different locations
    /// distribution ratios
    /// 
    /// 
    /// Don't forget about AI
    /// /  
    /// 
    /// super crystal
    /// highlights
    /// is cool looking
    /// breaks into a shinier piece of treasure
    /// covers multiple pieces
    /// created only from groups of special ore in a roid
    /// breaking one piece of ore connected to it breaks the crystal
    /// 
    /// 
    /// coding
    /// make an array
    /// define contigous peices to make the center of the asteroid
    /// eleminate 'floating' pieces
    /// choose type for each piece
    /// check to see if there is a possible crystal
    /// make each part of the array destructable
    /// create effeccts for destruction
    /// make a check for continuity during destruction
    /// separate if discontigous
    /// produce resources
    /// make the object rotate / move
    /// 
    /// 
    /// 
    /// 
    /// </summary>


    public float sizeOfField;  // not important for now
    public float sunDistance;   // Changes the distribution of the tiers of asteroids.
    public float numberInField; // amount in an area
    public int distributionType;// the way they are spread out
    public float averageSize;


	// Use this for initialization
	void StartField (float sizeOfField) {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

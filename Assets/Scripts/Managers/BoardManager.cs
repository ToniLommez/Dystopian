using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
//Allows us to use Lists.

//Tells Random to use the Unity Engine random number generator.

namespace WorldBuilder
{
    public class BoardManager : MonoBehaviour
    {
        public int columns = 256; //Number of columns in our game board.
        public int rows = 256; //Number of rows in our game board.

        public Count veinCount = new(5, 20); //Lower and upper limit for our random number of resource tiles per level.
        public Count veinSize = new(5, 35); //Lower and upper limit for our random number of resource tiles per level.
        public int veinMaxWidth = 15;
        public GameObject[] resourceTiles; //Array of resource tiles prefabs.

        public GameObject[] floorTiles; //Array of floor prefabs.

        private Transform boardHolder; //A variable to store a reference to the transform of our Board object.
        private readonly List<List<Vector3>> gridPositions = new(); //A list of possible locations to place tiles.


        //Clears our list gridPositions and prepares it to generate a new board.
        private void InitialiseList()
        {
            //Clear our list gridPositions.
            gridPositions.Clear();

            //Loop through x axis (columns).
            for (var x = 0; x < columns; x++)
            {
                var row = new List<Vector3>();

                //Within each column, loop through y axis (rows).
                for (var y = 0; y < rows; y++)
                    //At each index add a new Vector3 to our list with the x and y coordinates of that position.
                    row.Add(new Vector3(x, y, 0f));

                gridPositions.Add(row);
            }
        }


        //Sets up the outer walls and floor (background) of the game board.
        private void BoardSetup()
        {
            //Instantiate Board and set boardHolder to its transform.
            boardHolder = new GameObject("Board").transform;

            //Loop along x axis.
            for (var x = 0; x < columns; x++)
                //Loop along y axis.
            for (var y = 0; y < rows; y++)
            {
                //Choose a random tile from our array of floor tile prefabs and prepare to instantiate it.
                var toInstantiate = floorTiles[0];

                //Instantiate the GameObject instance using the prefab chosen for toInstantiate at the Vector3 corresponding to current grid position in loop, cast it to GameObject.
                var instance =
                    Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);

                //Set the parent of our newly instantiated object instance to boardHolder, this is just organizational to avoid cluttering hierarchy.
                instance.transform.SetParent(boardHolder);
            }
        }

        //RandomPosition returns a random position on our board.
        internal Vector3 RandomPosition()
        {
            return new Vector3(Random.Range(0, columns), Random.Range(0, rows), 0f);
        }

        //RandomGridPosition returns a random position from our list gridPositions.
        private Vector3 RandomGridPosition()
        {
            //Declare an integer randomIndex, set it's value to a random number between 0 and the count of items in our List gridPositions.
            var randomIndex = Random.Range(0, gridPositions.Count);

            //Declare a variable of type Vector3 called randomPosition, set it's value to the entry at randomIndex from our List gridPositions.
            var randomPositions = gridPositions[randomIndex];

            // Extract from this row.
            randomIndex = Random.Range(0, randomPositions.Count);
            var randomPosition = randomPositions[randomIndex];

            //Remove the entry at randomIndex from the list so that it can't be re-used.
            randomPositions.RemoveAt(randomIndex);

            //Return the randomly selected Vector3 position.
            return randomPosition;
        }

        internal Vector2 GetCenter()
        {
            return new Vector2(columns / 2.0f, rows / 2.0f);
        }


        //LayoutObjectAtRandom accepts an array of game objects to choose from along with a minimum and maximum range for the number of objects to create.
        private void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
        {
            //Choose a random number of objects to instantiate within the minimum and maximum limits
            var objectCount = Random.Range(minimum, maximum + 1);

            //Instantiate objects until the randomly chosen limit objectCount is reached
            for (var i = 0; i < objectCount; i++)
            {
                //Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
                var randomPosition = RandomGridPosition();

                //Choose a random tile from tileArray and assign it to tileChoice
                var tileChoice = tileArray[Random.Range(0, tileArray.Length)];

                //Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
                var obj = Instantiate(tileChoice, randomPosition, Quaternion.identity);
                obj.tag = "Unbuildable";
            }
        }

        //LayoutObjectAtRandom accepts an array of game objects to choose from along with a minimum and maximum range for the number of objects to create.
        private void LayoutVeinAtRandom(GameObject resource, int minimum, int maximum)
        {
            // Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
            var randomPosition = RandomGridPosition();

            //Choose a random number of objects to instantiate within the minimum and maximum limits
            var objectCount = Random.Range(minimum, maximum + 1);
            var maxWidth = Math.Min(objectCount / 3, veinMaxWidth); // Minimum of 3 layers.
            if (maxWidth < 3) maxWidth = objectCount;

            //Instantiate objects until the randomly chosen limit objectCount is reached
            var prevWidth = 0;
            var y = randomPosition.y;
            while (objectCount > 0 && y < rows)
            {
                // Okay We have a vein size, but veins are many levels deep.
                // Let's go through a number levels.
                var minWidth = Math.Max(1, (int)Math.Ceiling(prevWidth / 2.0f));
                var width = Math.Min(objectCount, Random.Range(minWidth, maxWidth));
                if (randomPosition.x + width > columns) width = columns - (int)randomPosition.x;

                // Generate the row.
                var startX = (int)randomPosition.x + Random.Range(0, width / 2);
                for (float x = startX; x < randomPosition.x + width; x++)
                {
                    var mypos = new Vector3(x, y, randomPosition.z);
                    var index = gridPositions[(int)x].IndexOf(mypos);
                    // Check we can lay here using our primative method.
                    if (index == -1)
                    {
                        objectCount--; // In case we get stuck.
                        continue;
                    }

                    gridPositions[(int)x].RemoveAt(index);

                    //Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
                    var obj = Instantiate(resource, mypos, Quaternion.identity);
                    obj.tag = "Unbuildable";

                    objectCount--;
                }

                y++;
            }
        }

        //SetupScene initializes our level and calls the previous functions to lay out the game board
        public void SetupScene()
        {
            //Creates the outer walls and floor.
            BoardSetup();

            //Reset our list of gridpositions.
            InitialiseList();

            //Instantiate a random number of wall tiles based on minimum and maximum, at randomized positions.
            //LayoutObjectAtRandom (resourceTiles, veinCount.minimum, veinCount.maximum);

            // Layout resource veins.
            var count = Random.Range(veinCount.minimum, veinCount.maximum);
            for (var i = 0; i < count; i++)
            {
                //Choose a random tile from tileArray and assign it to tileChoice
                var resource = resourceTiles[Random.Range(0, resourceTiles.Length)];

                LayoutVeinAtRandom(resource, veinSize.minimum, veinSize.maximum);
            }
        }

        // Using Serializable allows us to embed a class with sub properties in the inspector.
        [Serializable]
        public class Count
        {
            public int minimum; //Minimum value for our Count class.
            public int maximum; //Maximum value for our Count class.


            //Assignment constructor.
            public Count(int min, int max)
            {
                minimum = min;
                maximum = max;
            }
        }
    }
}
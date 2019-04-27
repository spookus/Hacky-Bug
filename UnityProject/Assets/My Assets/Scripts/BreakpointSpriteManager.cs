// Jack Hamilton        - 100550931
// Daniel MacCormick    - 100580519
// Wen Bo Yu            - 100579309
// Rob Savaglio         - 100591436

// *****
// BreakpointSpriteManager.cs
// This script controls the animated breakpoints objects
// It controls what faces they have and when to switch to 'happy' (it caught a bug) or 'sad' (the bug dodged it)
// *****

using System.Collections.Generic;
using UnityEngine;    

public class BreakpointSpriteManager : MonoBehaviour
{
    public bool hasChanged = false;

    public int spriteListIndex;

    public List<Sprite> defaultSprites;
    public List<Sprite> happySprites;
    public List<Sprite> sadSprites;


    // Start is called before the first frame update
    void Start()
    {
        spriteListIndex = Random.Range(0, 4);
        this.GetComponent<SpriteRenderer>().sprite = defaultSprites[spriteListIndex];
    }

    public void SetToHappySprite()
    {
       if (!hasChanged)
        {
            hasChanged = true;
            this.GetComponent<SpriteRenderer>().sprite = happySprites[spriteListIndex];
        }
    }

    public void SetToSadSprite()
    {
        if (!hasChanged)
        {
            hasChanged = true;
            this.GetComponent<SpriteRenderer>().sprite = sadSprites[spriteListIndex];
        }
        
    }
}

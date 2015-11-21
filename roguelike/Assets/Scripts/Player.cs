using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player : MovingObject
{
    public int wallDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;
    public Text foodText;
    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;
    
    private Animator animator;
    private int foodPoint;
    private Vector2 touchOrigin = -Vector2.one; // -Vector2.one is out of screen
    
    
    public void LoseFood(int loss)
    {
        animator.SetTrigger("playerHit");
        foodPoint -= loss;
        foodText.text = "-" + loss + " Food: " + foodPoint;
        
        CheckIfGameOver();
    }
    
    protected override void Start()
    {
        animator = GetComponent<Animator>();
        
        foodPoint = GameManager.instance.playerFoodPoints;
        
        foodText.text = "Food: " + foodPoint;
        
        base.Start();
    }
    
    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        foodPoint--;
        foodText.text = "Food: " + foodPoint;
        
        base.AttemptMove<T>(xDir, yDir);
        
        RaycastHit2D hit;
        if (Move(xDir, yDir, out hit))
        {
            SoundManager.instance.RandomizeSFX(moveSound1, moveSound2);
        }
        
        CheckIfGameOver();
        
        GameManager.instance.playersTurn = false;
    }
    
    protected override void OnCantMove<T>(T component)
    {
        Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);
        animator.SetTrigger("playerChop");
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Exit")
        {
            // No function pointer? has to be a string, really?
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }
        else if (other.tag == "Food")
        {
            foodPoint += pointsPerFood;
            foodText.text = "+" + pointsPerFood + " Food: " + foodPoint;
            SoundManager.instance.RandomizeSFX(eatSound1, eatSound2);
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Soda")
        {
            foodPoint += pointsPerSoda;
            foodText.text = "+" + pointsPerSoda + " Food: " + foodPoint;
            SoundManager.instance.RandomizeSFX(drinkSound1, drinkSound2);        
            other.gameObject.SetActive(false);
        }
    }
    
    private void Restart()
    {
        // load the same scene again
        Application.LoadLevel(Application.loadedLevel);
    }
    
    private void OnDisabled()
    {
        GameManager.instance.playerFoodPoints = foodPoint;
    }
    
    private void CheckIfGameOver()
    {
        if (foodPoint <= 0)
        {
            SoundManager.instance.RandomizeSFX(gameOverSound);
            SoundManager.instance.musicSource.Stop();                 
            GameManager.instance.GameOver();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.playersTurn)  return;
        
        int horizontal = 0;
        int vertical = 0;
        
    #if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
        
        horizontal = (int) Input.GetAxisRaw("Horizontal");
        vertical = (int) Input.GetAxisRaw("Vertical");
        
        if (horizontal != 0)
            vertical = 0;
            
    #else
        
        if (Input.touchCount > 0)
        {
            Touch myTouch = Input.touches[0];
            if (myTouch.phase == TouchPhase.Began)
            {
                touchOrigin = myTouch.position;
            }
            else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
            {
                Vector2 touchEnd = myTouch.position;
                float x = touchEnd.x - touchOrigin.x;
                float y = touchEnd.y - touchOrigin.y;
                touchOrigin.x = - 1;
                
                if (Mathf.Abs(x) > Mathf.Abs(y))
                    horizontal = x > 0 ? 1 : -1;
                else
                    vertical = y > 0 ? 1 : -1;
            }
        }
        
    #endif
        
        if (horizontal != 0 || vertical != 0)
            AttemptMove<Wall>(horizontal, vertical);
    }
}

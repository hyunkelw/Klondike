using UnityEngine;

public class Draggable : MonoBehaviour
{

    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] [Min(1)] private float moveSpeed = 10f;

    private float spriteSizeX;
    private float spriteSizeY;

    private float xMin;
    private float xMax;
    private float yMin;
    private float yMax;

    BoxCollider2D cardCollider;

    bool dragging;

    private void Awake()
    {
        // get sprite renderer
        if (!spriteRenderer)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // get size
        if (spriteRenderer)
        {
            spriteSizeX = spriteRenderer.bounds.size.x;
            spriteSizeY = spriteRenderer.bounds.size.y;
        }

        // get card collider
        cardCollider = GetComponent<BoxCollider2D>();
    }

    private void SetUpMoveBoundaries()
    {
        // lock movement inside viewport based on card sprite 
        xMin = Camera.main.ViewportToWorldPoint(Vector3.zero).x + spriteSizeX / 2;
        xMax = Camera.main.ViewportToWorldPoint(Vector3.right).x - spriteSizeX / 2;
        yMin = Camera.main.ViewportToWorldPoint(Vector3.zero).y + spriteSizeY / 2;
        yMax = Camera.main.ViewportToWorldPoint(Vector3.up).y - spriteSizeY / 2;
    }

    void Start()
    {
        // lock movement in the viewport
        SetUpMoveBoundaries();
    }


    void Update()
    {
        InputMove();
    }


    bool IsInHandle(Vector3 inputPosition)
    {
        //get center and size
        Vector2 offset = cardCollider.offset * cardCollider.transform.lossyScale;
        Vector2 center = new Vector2(cardCollider.transform.position.x, cardCollider.transform.position.y) + offset;
        float xSize = cardCollider.size.x * cardCollider.transform.lossyScale.x;
        float ySize = cardCollider.size.y * cardCollider.transform.lossyScale.y;

        //get limits
        float minX = center.x - xSize;
        float maxX = center.x + xSize;
        float minY = center.y - ySize;
        float maxY = center.y + ySize;

        //check if inside
        if(inputPosition.x > minX && inputPosition.x < maxX)
        {
            if(inputPosition.y > minY && inputPosition.y < maxY)
            {
                return true;
            }
        }

        return false;
    }

#if !UNITY_ANDROID
    private void InputMove()
    {
        // check to start dragging
        if (Input.GetMouseButtonDown(0) && dragging == false)
        {
            // get position
            Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // check if inside handle
            if (IsInHandle(worldMousePosition))
            {
                dragging = true;
            }
        }

        // if stop drag
        if (Input.GetKeyUp(KeyCode.Mouse0) && dragging)
        {
            dragging = false;
        }

        // drag
        if (dragging)
            Drag(Input.mousePosition);
    }

#elif UNITY_ANDROID

    private void InputMove()
    {
        //check there is a touch input
        if (Input.touchCount <= 0)
            return;

        //and get it
        Touch touch = Input.GetTouch(0);


        //check to start dragging
        if (touch.phase == TouchPhase.Began && dragging == false)
        {
            //get position
            Vector3 touchPos = cam.ScreenToWorldPoint(touch.position);

            //check if inside handle
            if (IsInHandle(touchPos))
            {
                dragging = true;
            }
        }

        //if stop drag
        if ((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && dragging)
        {
            dragging = false;
        }

        //drag
        if (dragging)
            Drag(touch.position);
    }
#endif


    void Drag(Vector2 inputPosition)
    {
        //get position
        Vector2 worldInputPosition = Camera.main.ScreenToWorldPoint(inputPosition);

        //get sprite position (remove localPosition of the Handle)
        Vector2 offset = cardCollider.offset * cardCollider.transform.lossyScale;
        Vector2 spritePosition = worldInputPosition - (new Vector2(cardCollider.transform.localPosition.x, cardCollider.transform.localPosition.y) + offset);

        //clamp
        float newXPos = Mathf.Clamp(spritePosition.x, xMin, xMax);
        float newYPos = Mathf.Clamp(spritePosition.y, yMin, yMax);

        //set position
        transform.position = Vector2.Lerp(transform.position, new Vector2(newXPos, newYPos), moveSpeed * Time.deltaTime);
    }



    public void SetMovementSpeed(float speed)
    {
        moveSpeed = speed;
    }
}


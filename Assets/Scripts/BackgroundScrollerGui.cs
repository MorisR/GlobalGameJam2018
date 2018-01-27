using UnityEngine;

public class BackgroundScrollerGui : MonoBehaviour
{

    [SerializeField] bool isScrolling;
    [SerializeField] float scrollSpeed;
    [SerializeField] Vector2 dircrtion;
    [SerializeField] CanvasRenderer _renderer;

    public void startScroll()
    {
        isScrolling = true;
    }

    public void stopScroll()
    {
        isScrolling = false;

    }

    // Use this for initialization
    void Start()
    {

        dircrtion = dircrtion.normalized;

    }

    // Update is called once per frame
    void Update()
    {

        if (isScrolling)
        {
            Vector2 offset = dircrtion * Time.time * scrollSpeed;
            _renderer.GetMaterial().mainTextureOffset = offset;
        }

    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMove : MonoBehaviour
{
    [SerializeField] private float limitAttackDistance;

    [SerializeField] private float limitAttackSize;

    [SerializeField] private float duration;

    [SerializeField] private AudioSource hitSound;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MoveAttack());
    }

    public IEnumerator MoveAttack ()
    {
        float elapsedTime = 0;
        Vector3 startingPos = transform.position;
        Vector3 end = new Vector3(startingPos.x, startingPos.y - limitAttackDistance, startingPos.z);
        Vector3 startingSize = transform.localScale;
        Vector3 endSize = 3 * startingSize;
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startingPos, end, (elapsedTime / duration));
            transform.localScale = Vector3.Lerp(startingSize, endSize, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        
        transform.position = end;
        hitSound.Play();
        GetComponent<Collider2D>().enabled = false;
        GameObject.Find("DialBlock").GetComponent<DialBlockManager>().count++;
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class DestructButton : MonoBehaviour
{
    [SerializeField] private GameObject camera;

    [SerializeField] private EndFightCutscene endCutscene;

    private Transform camTransform;

    private Glitch glitchEffect;
    
    public float shake = 0;
    private float shakeAmount;
    private float decreaseFactor;
        
    // Start is called before the first frame update
    void Start()
    {
        camTransform = camera.transform;
        glitchEffect = camera.GetComponent<Glitch>();
    }

    // Update is called once per frame
    void Update()
    {
        if (shake > 0) {
            camTransform.localPosition = Random.insideUnitSphere * shakeAmount;
            camTransform.localPosition = new Vector3(camTransform.localPosition.x, camTransform.localPosition.y, -10);
            shake -= Time.deltaTime * decreaseFactor;

        } else {
            shake = 0.0f;
        }
    }   
    //22s
    public void SetSelfDestruct()
    {
        CoreButton.Instance.RegisterFunction(CoreButton.FunctionType.Destruct, () =>
        {
            StartCoroutine(BarkManager.Instance.ShowIntroOutroBark(BarkManager.MultipleBarkType.Destruct, false,
                () => { }));
            StartCoroutine(SelfDestructSequence());
            Debug.Log("self destructed good bye");
        });
        CoreButton.Instance.SetCoreButton(CoreButton.FunctionType.Destruct);
        ChatLogManager.Instance.ShowText("<color=orange>Self-Destruct?");
    }

    IEnumerator SelfDestructSequence()
    {
        glitchEffect.enabled = true;
        glitchEffect.glitch = 0.25f;
        yield return new WaitForSeconds(10f);
        glitchEffect.glitch = 0.5f;
        yield return new WaitForSeconds(1f);
        shake = 50;
        shakeAmount = 0.1f;
        yield return new WaitForSeconds(6f);
        shakeAmount = 0.2f;
        yield return new WaitForSeconds(2f);
        shakeAmount = 0.3f;
        glitchEffect.glitch = 1f;
        yield return new WaitForSeconds(4f);
        endCutscene.StartCoroutine(endCutscene.PlaySelfDestructSequence());
    }
}

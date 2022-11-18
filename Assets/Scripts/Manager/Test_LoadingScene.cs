using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.PlayerLoop;

public class Test_LoadingScene : MonoBehaviour
{
    public Slider slider;                   // �ε� ��
    public TextMeshProUGUI loadingText;     // �ε� �ؽ�Ʈ
    public string nextSceneName = "TestSeamless";

    AsyncOperation async;                   // �񵿱�� �ε��� �����Ȳ�� ����� �� �� �ִ� Ŭ����

    WaitForSeconds waitForSeconds;          // loadingText ����� ���� ���� ������
    IEnumerator loadingTextCoroutine;       // Loading ... �� ���� �ڷ�ƾ
    IEnumerator loadSceneCoroutine;         // �񵿱� �� �ε带 ���� �ڷ�ƾ

    float loadRatio = 0.0f;                 // �񵿱� �� �ε� ���� ��Ȳ + 0.1f;
    bool loadCompleted = false;             // �񵿱� �� �ε��� �� �غ�Ǿ����� ǥ��(true�� �غ�Ϸ�)

    float sliderUpdateSpeed = 1.0f;         // �ε����� �ּ� �ð��� �����ϱ� ���� ��

    PlayerInputSystem inputActions;         // �ε� �Ϸ� �� �Ѿ�� ���� �Է�

    private void Awake()
    {
        inputActions = new();
    }

    private void OnEnable()
    {
        inputActions.UI.Enable();
        inputActions.UI.Press.performed += MousePress;
    }

    private void OnDisable()
    {
        inputActions.UI.Press.performed -= MousePress;
        inputActions.UI.Disable();
    }

    private void MousePress(InputAction.CallbackContext _)
    {
        if (loadCompleted)  // ���� �ε尡 �Ϸ�Ǿ��� ����
        {
            async.allowSceneActivation = true;  // �� �ε� �۾��� �Ϸ��� �� �ֵ��� ����
        }
    }

    private void Start()
    {
        waitForSeconds = new WaitForSeconds(0.3f);  // Loading ... �ð����� 0.2��
        loadingTextCoroutine = LoadingTestProgress();   // Loading ... �� �ڷ�ƾ ����
        StartCoroutine(loadingTextCoroutine);           // �ڷ��� ����
        loadSceneCoroutine = LoadScene();               // �񵿱� �� �ε��� ���� �ڷ�ƾ ����
        StartCoroutine(loadSceneCoroutine);             // �ڷ�ƾ ����
    }

    private void Update()
    {
        //slider.value = Mathf.Lerp(slider.value, loadRatio, Time.deltaTime * sliderUpdateSpeed);
        if (slider.value < loadRatio)
        {
            // slider�� ���� loadRatio���� �������� �ʾ���.
            slider.value += Time.deltaTime * sliderUpdateSpeed; // slider ���� ��Ű��
        }
    }

    /// <summary>
    /// Loading ...�� ��� ���� �ڷ�ƾ
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadingTestProgress()
    {
        int point = 0;  // 0 ~ 5�� ����� ����
        while (true)
        {
            string text = "Loading";
            for (int i = 0; i < point; i++)
            {
                text += " .";
            }
            loadingText.text = text;        // �ݺ��ؼ� Loading ���� �ڿ� .�� �߰��� ���δ�.

            yield return waitForSeconds;    // ������ �ð����� ���
            point++;                        // point�� ����
            point %= 6;                     // point���� 0 ~ 5�� �ǵ��� ����
        }
    }

    /// <summary>
    /// �񵿱� �� �ε��� �ڷ�ƾ
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadScene()
    {
        async = SceneManager.LoadSceneAsync(nextSceneName); // �񵿱�� �� �ε� �õ�
        async.allowSceneActivation = false;                 // �غ� �Ϸ�Ǿ �ٷ� �ε����� �ʵ��� ����

        while (loadRatio < 1.0f)
        {
            loadRatio = async.progress + 0.1f;              // loadRatio�� �񵿱� ���� ��Ȳ + 0.1�� ���߱� (���� async.progress�� �ִ밪�� 0.9)


            yield return null;
        }

        loadCompleted = true;                               // �ε��� �����ٰ� ǥ��
        Debug.Log("Load Completed!");

        yield return new WaitForSeconds(1.0f);              // 1�� �ڿ� ����� �͵� ���߱�
        StopCoroutine(loadingTextCoroutine);
    }

    //public void OnPointerClick(PointerEventData eventData)
    //{
    //    if (loadCompleted)
    //    {
    //        async.allowSceneActivation = true;
    //    }
    //}
}
